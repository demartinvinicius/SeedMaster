using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Nudes.SeedMaster.Interfaces;

/// <summary>
/// Interface seeder class
///
/// </summary>
/// <typeparam name="TContext">Context Type</typeparam>
/// <typeparam name="TDbSet">DbSet Type</typeparam>
public interface IActualSeeder<TDbSet, TContext> where TContext : DbContext
{
    public abstract void Seed(TContext context, ILogger logger);
}