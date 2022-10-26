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

    public DbSet<AnotherSupplier> OtherSuppliers { get; set; }
}