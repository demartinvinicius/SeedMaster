using Microsoft.EntityFrameworkCore;
using Nudes.SeedMaster.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

    [EnableSeeder(true)]
    public DbSet<Supplier> OtherSuppliers { get; set; }
}
