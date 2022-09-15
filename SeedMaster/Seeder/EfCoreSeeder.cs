using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.Extensions.Logging;
using Nudes.SeedMaster.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static Nudes.SeedMaster.SeedScanner;

namespace Nudes.SeedMaster.Seeder
{
    /// <summary>
    /// EF Core Implementation of seed strategy
    /// It should rely on DependencyInjection to acquire DbContexts and IActualSeeds
    /// </summary>
    public class EfCoreSeeder : ISeeder
    {
        private readonly IEnumerable<DbContext> contexts;
        private readonly IEnumerable<ScanResult> seeders;
        private readonly ILogger<EfCoreSeeder> logger;
        private readonly ILoggerFactory loggerFactory;
        private readonly Queue<IEntityType> seedableQueue;
        private readonly Queue<IEntityType> cleanableQueue;
        private readonly List<string> entitiesAlreadySeeded;
        

        /// <summary>
        /// Try to find a seed for a entityType and invokes it's seed method to populate the entity.
        /// Throws an EntryPointNotFoundException if a IActualSeed is not found for the entity and context 
        /// </summary>
        /// <param name="entityType">The entity to populate</param>
        /// 
        private Task InvokeSeed(IEntityType entityType, DbContext context)
        {
            var seedClass = seeders
                .Where(x => x.SeedType == ScanResult.SeedTypes.EntitySeed)
                .Where(x => x.ContextType == context.GetType())
                .Where(x => x.InterfaceType.GenericTypeArguments.Any(x => x.FullName == entityType.Name)).Select(x => x.ImplementationType).SingleOrDefault();

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
            return Task.CompletedTask;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        /// <exception cref="EntryPointNotFoundException"></exception>
        private void InvokeGlobalSeeds(DbContext context)
        {
            var seedClasses = seeders
                .Where(x => x.SeedType == ScanResult.SeedTypes.GlobalSeed &&
                x.ContextType == context.GetType()).Select(x => x.ImplementationType);

            if (seedClasses == null)
                return;

            foreach(var seedClass in seedClasses)
            {
                var seeder = Activator.CreateInstance(seedClass);
                var loggerForSeeder = loggerFactory.CreateLogger(seedClass.Name);
                var method = seedClass.GetMethod("Seed");
                if (method == null)
                {
                    throw new EntryPointNotFoundException($"Not found a Seed method on a GlobalSeeder");
                }
                method.Invoke(seeder, new object[] { context, loggerForSeeder });   
            }

        }
        /// <summary>
        /// Constructor for the EfCoreSeeder. 
        /// </summary>
        /// <param name="contexts">The DbContexts to be seeded</param>
        /// <param name="seedTypes">The IActualSeeds to be called during the seed process. The list of IActualSeeds can be obtained from an assembly using the SeedScanner class</param>
        /// <param name="loggerFactory">A ILoggerFactory used to create loggers for the seeders and for this class</param>
        public EfCoreSeeder(IEnumerable<DbContext> contexts, IEnumerable<ScanResult> seedTypes, ILoggerFactory loggerFactory)
        {
            seedableQueue = new Queue<IEntityType>();
            cleanableQueue = new Queue<IEntityType>();
            entitiesAlreadySeeded = new List<string>();
            this.contexts = contexts;
            this.logger = loggerFactory.CreateLogger<EfCoreSeeder>();
            this.loggerFactory = loggerFactory;
            
            this.seeders = seedTypes;
        }

        public virtual async Task Clean()
        {
            foreach (DbContext context in contexts)
            {
                cleanableQueue.Clear();
                EfCoreHelpers.FillCleanableEntitiesQueue(context, cleanableQueue);
                int avoidloop = cleanableQueue.Count();
                while (cleanableQueue.Count > 0 && avoidloop > 0)
                {
                    var entityType = cleanableQueue.Peek();
                    if ((entityType.GetNavigations().Where(a => a.IsCollection).Count() == 0) ||
                        (entityType.GetNavigations().Where(a => a.IsCollection).Select(a => a.TargetEntityType)
                        .All(x => !EfCoreHelpers.EntityHasData(context, x))))
                    {
                        logger?.LogInformation($"Cleaning data from entity => {entityType.ClrType.Name}");
                        try
                        {
                            var recordstodelete = context.GetType().GetMethods()
                                .Where(d => d.Name == "Set")
                                .FirstOrDefault(d => d.IsGenericMethod)
                                .MakeGenericMethod(entityType.ClrType).Invoke(context, null);
                            var dbSet = recordstodelete as IQueryable<object>;
                            context.RemoveRange(await dbSet.IgnoreQueryFilters().ToListAsync());
                            await context.SaveChangesAsync();
                        }
                        catch (Exception ex)
                        {
                            logger?.LogWarning($"Error on delete {Environment.NewLine}{ex.Message}");
                            throw;
                        }
                        cleanableQueue.Dequeue();
                        avoidloop = cleanableQueue.Count();
                    }
                    else
                    {
                        logger?.LogWarning($"Queuing entity => {entityType.ClrType.Name}");
                        cleanableQueue.Dequeue();
                        cleanableQueue.Enqueue(entityType);
                        avoidloop--;
                    }
                }
            }
        }

        public virtual async Task Seed()
        {
            foreach (DbContext context in contexts)
            {
                seedableQueue.Clear();
                EfCoreHelpers.FillSeedableQueue(context, seedableQueue);
                InvokeGlobalSeeds(context);

                int avoidloop = seedableQueue.Count;

                while (seedableQueue.Count > 0 && avoidloop >= 0)
                {
                    var entityType = seedableQueue.Peek();

                    if ((entityType.GetForeignKeys().Count() == 0) ||
                        (entityType.GetForeignKeys().All(x => EntityAlreadySeedOrHasData(x.PrincipalEntityType,context))))
                    {
                        logger?.LogInformation($"Populating entity => {entityType.ClrType.Name}");
                        try
                        {
                            await InvokeSeed(entityType,context);
                            entitiesAlreadySeeded.Add(entityType.ClrType.Name);
                        }
                        catch (Exception ex)
                        {
                            logger?.LogWarning($"Error on populate {Environment.NewLine} {ex.Message}");
                            throw;
                        }
                        seedableQueue.Dequeue();
                        avoidloop = seedableQueue.Count();
                    }
                    else
                    {
                        logger?.LogWarning($"Queuing entity => {entityType.ClrType.Name}");
                        seedableQueue.Dequeue();
                        seedableQueue.Enqueue(entityType);
                        avoidloop--;
                    }
                }

                if (seedableQueue.Count() > 0)
                {
                    logger?.LogWarning("The entities queue is not empty. Failed to populate all entities");
                }
                else
                {
                    logger?.LogInformation("Seed finalized");
                }
            }
        }

        private bool EntityAlreadySeedOrHasData(IEntityType entity,DbContext context)
        {
            if (entitiesAlreadySeeded.Contains(entity.ClrType.Name))
                return true;
            return EfCoreHelpers.EntityHasData(context, entity);
        }

        public virtual async Task Commit()
        {
            logger?.LogInformation("Starting commit");

            foreach (var db in contexts)
            {
                logger?.LogInformation("Committing changes to {db}", db);
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