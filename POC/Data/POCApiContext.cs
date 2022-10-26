using Microsoft.EntityFrameworkCore;
using Nudes.SeedMaster.Attributes;
using POC.Model;

namespace POC.Context;

public class POCApiContext : DbContext
{
    public POCApiContext()
    {
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseInMemoryDatabase("DatabaseForTesting");
    }

    public DbSet<Person> People { get; set; }

    public DbSet<Order> Orders { get; set; }

    public DbSet<Product> Products { get; set; }

    public DbSet<OrderItems> OrdersItems { get; set; }

    public DbSet<Supplier> Suppliers { get; set; }
}