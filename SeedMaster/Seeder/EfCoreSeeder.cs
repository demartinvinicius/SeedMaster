using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.Extensions.Logging;
using Nudes.SeedMaster.Interfaces;
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
            return (bool)specificSeederClass.GetMethod("Seed").Invoke(specificSeeder, new object[] { context, logger1 });
        }

        /// <summary>
        /// Initialize the Seeder
        /// First get all seeders of the running assembly
        /// </summary>
        ///
        public EfCoreSeeder(IEnumerable<DbContext> contexts, Assembly assembly, ILogger<EfCoreSeeder> logger, ILoggerFactory loggerFactory)
        {
            int avoidloop;
            alreadyPopulated = new List<string>();
            entitiesQueue = new Queue<IEntityType>();
            this.contexts = contexts;
            this.logger = logger;
            this.assembly = assembly;
            this.loggerFactory = loggerFactory;
            logger?.LogInformation("Starting Scanning Contexts");
            this.context = contexts.FirstOrDefault();

            var context = contexts.FirstOrDefault();

            seedTypes = from type in assembly.GetExportedTypes()
                        where !type.IsAbstract && !type.IsGenericTypeDefinition
                        let interfaces = type.GetInterfaces()
                        let genericInterfaces = interfaces.Where(i => i.GetTypeInfo().IsGenericType && i.GetGenericTypeDefinition() == typeof(INewSeeder<,>))
                        let matchingInterface = genericInterfaces.FirstOrDefault()
                        where matchingInterface != null
                        select matchingInterface;

            entities = context.Model.GetEntityTypes().ToList();
            foreach (var entity in entities)
            {
                entitiesQueue.Enqueue(entity);
            }

            avoidloop = entitiesQueue.Count;

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
        }

        public virtual async Task Clean()
        {
            //#region Droping

            //logger?.LogInformation("Cleaning started");

            //foreach (var db in contexts)
            //    await CleanDb(db);

            //logger?.LogInformation("Cleaning ended");

            //#endregion
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
            //logger?.LogInformation("Starting seed");

            //foreach (var db in contexts)
            //{
            //    var dbseeds = seeds.Where(d => d.GetType().GetTypeInfo().ImplementedInterfaces.Any(f => f.IsGenericType && f.GetGenericTypeDefinition() == typeof(ISeed<>) && f.GenericTypeArguments.Any(g => g == db.GetType())));
            //    foreach (var seed in dbseeds)
            //    {
            //        logger?.LogInformation("seeding {seed} into {db}", seed, db);
            //        await seed.Seed(db);
            //    }
            //}

            //logger?.LogInformation("Seed finalized");
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