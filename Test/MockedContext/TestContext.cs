using Microsoft.EntityFrameworkCore;
using Nudes.SeedMaster.Attributes;
using Test.MockedDomain;

namespace Test.MockedContext;

public class TestContext : DbContext
{
    private readonly StreamWriter _logStream = new StreamWriter(@"c:\tmp\logdb.txt", append: false);

    public TestContext()
    {
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseInMemoryDatabase("DatabaseForTesting");
        optionsBuilder.LogTo(_logStream.WriteLine);
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
        _logStream.Dispose();
    }

    public override async ValueTask DisposeAsync()
    {
        await base.DisposeAsync();
        await _logStream.DisposeAsync();
    }
}