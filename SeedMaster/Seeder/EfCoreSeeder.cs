using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.Extensions.Logging;
using Nudes.SeedMaster.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Nudes.SeedMaster.Seeder
{
    /// <summary>
    /// EF Core Implementation of seed strategy
    /// It should rely on DependencyInjection to acquire DbContexts and ISeeds
    /// </summary>
    public class EfCoreSeeder : ISeeder
    {
        private readonly IEnumerable<ScanResult> seeders;

        private readonly ILogger<EfCoreSeeder> logger;

        private readonly Queue<IEntityType> seedableQueue;
        private readonly Queue<IEntityType> cleanableQueue;
        private readonly List<string> entitiesAlreadySeeded;
        private readonly DbContext context;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="context"></param>
        /// <param name="seedTypes"></param>
        /// <param name="logger"></param>
        /// <param name="loggerFactory"></param>
        public EfCoreSeeder(DbContext context, IEnumerable<ScanResult> seedTypes, ILogger<EfCoreSeeder> logger)
        {
            seedableQueue = new Queue<IEntityType>();
            cleanableQueue = new Queue<IEntityType>();
            entitiesAlreadySeeded = new List<string>();

            this.logger = logger;
            this.seeders = seedTypes;
            this.context = context;

            EfCoreHelpers.FillSeedableQueue(context, seedableQueue);
            EfCoreHelpers.FillCleanableEntitiesQueue(context, cleanableQueue);
        }

        /// <summary>
        /// Try to find a seed for a entityType and invokes it's seed method to populate the entity.
        /// </summary>
        /// <param name="entityType">The entity to populate</param>
        /// <returns>true when the method is successful</returns>
        private void InvokeSeed(IEntityType entityType)
        {
            var seedClasses = seeders.Where(x => x.InterfaceType.GenericTypeArguments.Any(x => x.FullName == entityType.Name)).Select(x => x.ImplementationType)
                                     .ToList();

            seedClasses.ForEach(_seedType =>
            {
                if (_seedType == null)
                {
                    throw new EntryPointNotFoundException($"Not found a interface seeder for the entity {entityType.Name}");
                }

                var seeder = Activator.CreateInstance(_seedType);
                var method = _seedType.GetMethod("Seed");

                if (method == null)
                {
                    throw new EntryPointNotFoundException($"Not found a Seed method on the seeder for the entity {entityType.Name}");
                }

                _seedType.GetMethod("Seed").Invoke(seeder, new object[] { context });
            });
        }

        public virtual async Task Clean()
        {
            int avoidloop = cleanableQueue.Count;
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
                    }
                    cleanableQueue.Dequeue();
                    avoidloop = cleanableQueue.Count();
                }
                else
                {
                    logger?.LogWarning($"Queueing entity => {entityType.ClrType.Name}");
                    cleanableQueue.Dequeue();
                    cleanableQueue.Enqueue(entityType);
                    avoidloop--;
                }
            }
        }

        public virtual Task Seed()
        {
            int avoidloop = seedableQueue.Count;

            while (seedableQueue.Count > 0 && avoidloop >= 0)
            {
                var entityType = seedableQueue.Peek();

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
                    seedableQueue.Dequeue();
                    avoidloop = seedableQueue.Count();
                }
                else
                {
                    logger?.LogWarning($"Queueing entity => {entityType.ClrType.Name}");
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

            return Task.CompletedTask;
        }

        private bool EntityAlreadySeedOrHasData(IEntityType entity)
        {
            if (entitiesAlreadySeeded.Contains(entity.ClrType.Name))
                return true;

            return EfCoreHelpers.EntityHasData(context, entity);
        }

        public virtual async Task Commit()
        {
            logger?.LogInformation("Starting commit");

            await context.SaveChangesAsync();

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