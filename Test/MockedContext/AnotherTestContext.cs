using Microsoft.EntityFrameworkCore;
using Nudes.SeedMaster.Attributes;
using Test.MockedDomain;

namespace Test.MockedContext;

public class AnotherTestContext : DbContext
{
    public AnotherTestContext()
    {
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseInMemoryDatabase("AnotherDatabaseForTesting");
    }

    [EnableSeeder]
    public DbSet<Supplier> OtherSuppliers { get; set; }
}