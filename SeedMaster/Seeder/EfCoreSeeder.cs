using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.Extensions.Logging;
using Nudes.SeedMaster.Attributes;
using Nudes.SeedMaster.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using static Nudes.SeedMaster.SeedScanner;

namespace Nudes.SeedMaster.Seeder
{
    /// <summary>
    /// EF Core Implementation of seed strategy
    /// It should rely on DependencyInjection to acquire DbContexts and ISeeds
    /// </summary>
    public class EfCoreSeeder : ISeeder
    {
        private readonly IEnumerable<DbContext> contexts;
        private readonly IEnumerable<ScanResult> seeders;
        private readonly ILogger<EfCoreSeeder> logger;
        private readonly ILoggerFactory loggerFactory;
        private readonly Queue<IEntityType> entitiesQueue;
        private readonly List<string> entitiesAlreadySeeded;

        private readonly DbContext context;

        /// <summary>
        /// Try to find a seed for a entityType and invokes it's seed method to populate the entity.
        /// </summary>
        /// <param name="entityType">The entity to populate</param>
        /// <returns>true when the method is successful</returns>
        private void InvokeSeed(IEntityType entityType)
        {
            var seedClass = seeders.Where(x => x.InterfaceType.GenericTypeArguments.Any(x => x.FullName == entityType.Name)).Select(x => x.ImplementationType).SingleOrDefault();
            if (seedClass == null)
            {
                throw new EntryPointNotFoundException($"Not found a interface seeder for the entity {entityType.Name}");
            }
            var seeder = Activator.CreateInstance(seedClass);
            var loggerForSeeder = loggerFactory.CreateLogger(seedClass.Name);
            var method = seedClass.GetMethod("Seed");
            if (method == null)
            {
                throw new EntryPointNotFoundException($"Not found a Seed method on the seeder for the entity {entityType.Name}");
            }
            seedClass.GetMethod("Seed").Invoke(seeder, new object[] { context, loggerForSeeder });

        }

        public EfCoreSeeder(IEnumerable<DbContext> contexts, IEnumerable<ScanResult> seedTypes, ILogger<EfCoreSeeder> logger, ILoggerFactory loggerFactory)
        {
            entitiesQueue = new Queue<IEntityType>();
            entitiesAlreadySeeded = new List<string>();
            this.contexts = contexts;
            this.logger = logger;
            this.loggerFactory = loggerFactory;
            this.context = contexts.FirstOrDefault();
            this.seeders = seedTypes;
            FillQueue();
        }

        public virtual async Task Clean()
        {
            var rootentities = entitiesQueue.ToList()
                .Where(x => x.GetForeignKeys().Count() == 0);

            
            foreach (var entity in rootentities)
            {
                var recordstodelete = context.GetType().GetMethods()
                             .Where(d => d.Name == "Set")
                             .FirstOrDefault(d => d.IsGenericMethod)
                             .MakeGenericMethod(entity.ClrType).Invoke(context, null);

                var dbSet = recordstodelete as IQueryable<object>;
                context.RemoveRange(await dbSet.IgnoreQueryFilters().ToListAsync());
            }
        }
        public virtual async Task Seed()
        {
            
            int avoidloop = entitiesQueue.Count;

            while (entitiesQueue.Count > 0 && avoidloop > 0)
            {
                var entityType = entitiesQueue.Peek();
 
                if ((entityType.GetForeignKeys().Count() == 0) ||
                    (entityType.GetForeignKeys().All(x => EntityAlreadySeedOrHasData(x.PrincipalEntityType))))
                {
                    logger?.LogInformation($"Populating entity => {entityType.ClrType.Name}");
                    try
                    {
                        InvokeSeed(entityType);
                        entitiesAlreadySeeded.Add(entityType.ClrType.Name);
                    }
                    catch (Exception ex)
                    {
                        logger?.LogWarning($"Error on populate {Environment.NewLine} {ex.Message}");
                    }
                    entitiesQueue.Dequeue();
                    avoidloop = entitiesQueue.Count();
                }
                else
                {
                    logger?.LogWarning($"Queueing entity => {entityType.ClrType.Name}");
                    entitiesQueue.Dequeue();
                    entitiesQueue.Enqueue(entityType);
                }
            }

            if (entitiesQueue.Count() > 0)
            {
                logger?.LogWarning("The entities queue is not empty. Failed to populate all entities");
            }
            else
            {
                logger?.LogInformation("Seed finalized");
            }
        }
        private bool EntityAlreadySeedOrHasData(IEntityType entity)
        {
            if (entitiesAlreadySeeded.Contains(entity.ClrType.Name))
                return true;
            return EntityHasData(entity);
        }

        private bool EntityHasData(IEntityType entity)
        {
            MethodInfo methodinfo = context.GetType().GetMethods().Single(p => p.Name == nameof(DbContext.Set) && !p.GetParameters().Any());
            var method = methodinfo.MakeGenericMethod(entity.ClrType).Invoke(context, null) as IEnumerable<object>;
            if (method.Any())
                return true;
            return false;
        }
        private void FillQueue()
        {
            IEnumerable<string> atributes = context.GetType().GetProperties().Where(x => x.GetCustomAttribute<EnableSeederAttribute>() != null).Select(x => x.PropertyType.GenericTypeArguments.FirstOrDefault().Name);
            var entities = context.Model.GetEntityTypes().Where(y => atributes.Any(x => x == y.ClrType.Name));
            foreach (var entity in entities)
            {
                entitiesQueue.Enqueue(entity);
            }
        }

        public virtual async Task Commit()
        {
            logger?.LogInformation("Starting commit");

            foreach (var db in contexts)
            {
                logger?.LogInformation("Commiting changes to {db}", db);
                await db.SaveChangesAsync();
            }

            logger?.LogInformation("Commit finalized");
        }

        public virtual async Task Run()
        {
            await Clean();
            await Commit();

            await Seed();
            await Commit();
        }

        public void Dispose()
        {
            
        }
    }
}