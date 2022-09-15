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
    }

    [EnableSeeder]
    public DbSet<Person> People { get; set; }

    [EnableSeeder]
    public DbSet<Order> Orders { get; set; }

    [EnableSeeder]
    public DbSet<Product> Products { get; set; }

    [EnableSeeder]
    public DbSet<OrderItems> OrdersItems { get; set; }

    [EnableSeeder]
    public DbSet<Supplier> Suppliers { get; set; }
}