using Microsoft.EntityFrameworkCore;
using Nudes.SeedMaster.Attributes;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Test.MockedDomain;

namespace Test.MockedContext;

public class YetAnotherContext : DbContext
{
    public YetAnotherContext()
    {

    }
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseInMemoryDatabase("YetAnotherDatabaseForTesting");
    }

    [EnableSeeder(true)]
    public DbSet<Supplier> YetAnotherSuppliers { get; set; }
}
