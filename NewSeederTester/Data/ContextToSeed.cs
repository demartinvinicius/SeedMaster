using Microsoft.EntityFrameworkCore;
using NewSeederTester.Data.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewSeederTester.Data;

public class ContextToSeed : DbContext
{
    public ContextToSeed()
    {

    }
    public ContextToSeed(DbContextOptions<ContextToSeed> options) : base (options)
    {

    }
    public DbSet<Person> People { get; set; }
    public DbSet<Order> Orders { get; set; }

    public DbSet<Product> Products { get; set; }
    public DbSet<OrderItems> OrdersItems { get; set; }  
    public DbSet<Supplier> Suppliers { get; set; }    
   
}
