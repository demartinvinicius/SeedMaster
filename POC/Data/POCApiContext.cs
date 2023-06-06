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

    public DbSet<Supplier> Suppliers { get; set; }

    protected override void OnModelCreating(ModelBuilder mb)
    {
        mb.Entity<Product>(d =>
        {
            d.HasKey(d => d.Id);
            d.HasMany(d => d.Orders).WithMany(d => d.Products);
        });
        mb.Entity<Order>(d =>
        {
            d.HasKey(d => d.Id);
            d.HasMany(d => d.Products).WithMany(d => d.Orders);
            d.HasOne(d => d.Person).WithMany(d => d.Orders).HasForeignKey(d => d.PersonId);
        });

        mb.Entity<Person>(d =>
        {
            d.HasKey(d => d.Id);
            d.HasMany(d => d.Orders).WithOne(d => d.Person).HasForeignKey(d => d.PersonId);
        });
    }
}