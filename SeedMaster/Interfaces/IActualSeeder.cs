using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Nudes.SeedMaster.Interfaces;

/// <summary>
/// Interfaces seeder class
/// Use the following interfaces to create your own seeders that will be called to populate the database.
/// The classes derived from the interface IActualSeeder<TDbSet, TContext> are used to populate a single table on the database.In this case SeedMaster will analyze the database structure to find out the correct order to call the seeders respecting the relationships between tables.
/// The classes derived from the interface IActualSeeder<TContext> are global seeders that are called before the other seeds.These seeders can be used to populate all the database.In these cases, no analyze is performed against the database structure before calling the seeders.
/// </summary>
/// <typeparam name="TContext">Context Type</typeparam>
/// <typeparam name="TDbSet">DbSet Type</typeparam>
public interface IActualSeeder<TDbSet, TContext> where TContext : DbContext
{
    public abstract void Seed(TContext context, ILogger logger);
}

public interface IActualSeeder<TContext> where TContext : DbContext
{
    public abstract void Seed(TContext context, ILogger logger);
}