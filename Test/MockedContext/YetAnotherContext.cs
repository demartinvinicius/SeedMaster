using Microsoft.EntityFrameworkCore;
using Nudes.SeedMaster.Attributes;
using Test.MockedDomain;

namespace Test.MockedContext;

public class YetAnotherContext : DbContext
{
    public YetAnotherContext()
    {
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseInMemoryDatabase("YetAnotherDatabaseForTesting");
    }

    [EnableSeeder]
    public DbSet<Supplier> YetAnotherSuppliers { get; set; }
}