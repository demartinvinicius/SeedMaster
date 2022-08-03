using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nudes.SeedMaster.Interfaces;

/// <summary>
/// Interface seeder class
/// 
/// </summary>
/// <typeparam name="TContext">Context Type</typeparam>
/// <typeparam name="TDbSet">DbSet Type</typeparam>
public interface INewSeeder<TDbSet,TContext> where TContext : DbContext
{ 
    public abstract bool Seed(TContext context,ILogger logger);
}
