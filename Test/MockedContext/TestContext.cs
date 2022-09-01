using Microsoft.EntityFrameworkCore;
using Nudes.SeedMaster.Attributes;
using Test.MockedDomain;

namespace Test.MockedContext;

public class TestContext : DbContext
{
    public TestContext()
    {
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseInMemoryDatabase("DatabaseForTesting");
        optionsBuilder.EnableSensitiveDataLogging();
    }

    [EnableSeeder(true)]
    public DbSet<Person> People { get; set; }

    [EnableSeeder(true)]
    public DbSet<Order> Orders { get; set; }

    [EnableSeeder(true)]
    public DbSet<Product> Products { get; set; }

    [EnableSeeder(true)]
    public DbSet<OrderItems> OrdersItems { get; set; }

    [EnableSeeder(true)]
    public DbSet<Supplier> Suppliers { get; set; }

    public override void Dispose()
    {
        base.Dispose();
    }

    public override async ValueTask DisposeAsync()
    {
        await base.DisposeAsync();
    }
}