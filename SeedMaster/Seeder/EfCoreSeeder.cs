using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.Extensions.Logging;
using Nudes.SeedMaster.Interfaces;
using Nudes.SeedMaster.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Nudes.SeedMaster.Seeder
{
    /// <summary>
    /// EF Core Implementation of seed strategy
    /// It should rely on DependencyInjection to acquire DbContexts and ISeeds
    /// </summary>
    public class EfCoreSeeder : ISeeder
    {
        private readonly IEnumerable<DbContext> contexts;
        private readonly IEnumerable<INewSeeder<object, DbContext>> seeds;
        private readonly IEnumerable<Type> seedTypes;
        private readonly ILogger<EfCoreSeeder> logger;
        private readonly ILoggerFactory loggerFactory;
        private readonly Assembly assembly;
        private readonly IEnumerable<IEntityType> entities;
        private readonly Queue<IEntityType> entitiesQueue;
        private readonly List<string> alreadyPopulated;
        private readonly DbContext context;

        /// <summary>
        /// Try to find a seed for a entityType and invokes it's seed method to populate the entity.
        /// </summary>
        /// <param name="entityType">The entity to populate</param>
        /// <returns>true when the method is successful</returns>
        private bool InvokeSeed(IEntityType entityType)
        {
            var specificSeederInterface = seedTypes.Where(x => x.GenericTypeArguments.Any(x => x.FullName == entityType.Name)).FirstOrDefault();
            if (specificSeederInterface == null)
                return false;

            var specificSeederClass = assembly.GetTypes().Where(p => specificSeederInterface.IsAssignableFrom(p)).FirstOrDefault();
            if (specificSeederClass == null)
                return false;

            var logger1 = loggerFactory.CreateLogger(specificSeederClass.Name);
            var specificSeeder = Activator.CreateInstance(specificSeederClass);

            var result = specificSeederClass.GetMethod("Seed").Invoke(specificSeeder, new object[] { context, logger1 });


            
            if (result == null)
                return false;

            //context.AddRange(result);
            context.SaveChangesAsync().Wait();
            return true;
        }

        /// <summary>
        /// Initialize the Seeder
        /// First get all seeders of the running assembly
        /// </summary>
        ///
        public EfCoreSeeder(IEnumerable<DbContext> contexts, Assembly assembly, ILogger<EfCoreSeeder> logger, ILoggerFactory loggerFactory)
        {
            alreadyPopulated = new List<string>();
            entitiesQueue = new Queue<IEntityType>();
            this.contexts = contexts;
            this.logger = logger;
            this.assembly = assembly;
            this.loggerFactory = loggerFactory;

            this.context = contexts.FirstOrDefault();

            seedTypes = from type in assembly.GetExportedTypes()
                        where !type.IsAbstract && !type.IsGenericTypeDefinition
                        let interfaces = type.GetInterfaces()
                        let genericInterfaces = interfaces.Where(i => i.GetTypeInfo().IsGenericType && i.GetGenericTypeDefinition() == typeof(INewSeeder<,>))
                        let matchingInterface = genericInterfaces.FirstOrDefault()
                        where matchingInterface != null
                        select matchingInterface;
        }

        public virtual async Task Clean()
        {
            var rootentities = context.Model.GetEntityTypes().Where(x => x.GetForeignKeys().Count() == 0);
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

        protected virtual async Task CleanDb(DbContext db)
        {
            //logger?.LogInformation("Cleaning context {db}", db);

            //foreach (var type in db.Model.GetEntityTypes())
            //    await CleanEntity(db, type);
        }

        protected virtual async Task CleanEntity(DbContext db, IEntityType type)
        {
            //logger?.LogInformation("Cleaning entity {typeName}", type.Name);
            //if (type.ClrType == typeof(Dictionary<string, object>))
            //{
            //    logger?.LogWarning("type {typeName} is a many to many, skipping", type.Name);
            //    return;
            //}

            //if (type.IsOwned())
            //{
            //    logger?.LogWarning("type {typeName} is owned, skipping", type.Name);
            //    return;
            //}

            //var boxedDbSet = db.GetType().GetMethods()
            //                             .Where(d => d.Name == "Set")
            //                             .FirstOrDefault(d => d.IsGenericMethod)
            //                             .MakeGenericMethod(type.ClrType).Invoke(db, null);

            //var dbSet = boxedDbSet as IQueryable<object>;
            //db.RemoveRange(await dbSet.IgnoreQueryFilters().ToListAsync());
        }

        
        public virtual async Task Seed()
        {
            //Clean().Wait();
            //context.SaveChangesAsync().Wait();

            var entities = context.Model.GetEntityTypes().Where(x => x.GetType().GetCustomAttribute<EnableSeederAttribute>() != null);
            foreach (var entity in entities)
            {
                entitiesQueue.Enqueue(entity);
            }
            int avoidloop = entitiesQueue.Count;

            while (entitiesQueue.Count() > 0 && avoidloop > 0)
            {
                var entityType = entitiesQueue.Peek();

                if (entityType.GetForeignKeys().Count() == 0) // Root Entity
                {
                    logger?.LogInformation($"Found Root Entity => {entityType.Name}");

                    if (!InvokeSeed(entityType))
                    {
                        logger?.LogWarning($"Failed to seed {entityType.ClrType.Name}");
                        entitiesQueue.Dequeue();
                        entitiesQueue.Enqueue(entityType);
                        avoidloop--;
                        continue;
                    }
                    else
                    {
                        logger?.LogInformation($"Entity {entityType.ClrType.Name} populated.");
                        alreadyPopulated.Add(entityType.ClrType.Name);
                        entitiesQueue.Dequeue();
                        avoidloop = entitiesQueue.Count();
                    }
                }
                else
                {
                    var teste = entityType.GetForeignKeys().ToList();

                    if (!entityType.GetForeignKeys().All(x => alreadyPopulated.Contains(x.PrincipalEntityType.ClrType.Name)))
                    {
                        logger.LogInformation(String.Join(" ", entityType.GetForeignKeys().Select(a => a.PrincipalEntityType)));
                        entitiesQueue.Dequeue();
                        entitiesQueue.Enqueue(entityType);
                    }
                    else
                    {
                        if (!InvokeSeed(entityType))
                        {
                            logger?.LogWarning($"Failed to seed {entityType.ClrType.Name}");
                            entitiesQueue.Dequeue();
                            entitiesQueue.Enqueue(entityType);
                            avoidloop--;
                            continue;
                        }
                        else
                        {
                            logger?.LogInformation($"Entity {entityType.ClrType.Name} populated.");
                            alreadyPopulated.Add(entityType.ClrType.Name);
                            entitiesQueue.Dequeue();
                            avoidloop = entitiesQueue.Count();
                        }
                    }
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

        public virtual void Dispose()
        {
            //    foreach (var db in contexts)
            //        db?.Dispose();
        }
    }
}