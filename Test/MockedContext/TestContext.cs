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

    [EnableSeeder(true)]
    public DbSet<Person> People { get; set; }

    [EnableSeeder(true)]
    public DbSet<Order> Orders { get; set; }

    [EnableSeeder(false)]
    public DbSet<Product> Products { get; set; }

    [EnableSeeder(false)]
    public DbSet<OrderItems> OrdersItems { get; set; }

    [EnableSeeder(true)]
    public DbSet<Supplier> Suppliers { get; set; }
}
