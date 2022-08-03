using Microsoft.Extensions.Logging;
using NewSeederTester.Data.Domain;
using Nudes.SeedMaster.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bogus;
using Bogus.Extensions.Brazil;

namespace NewSeederTester.Data.Seeders;

public class ProductSeeder : INewSeeder<Product,ContextToSeed>
{
    public bool Seed(ContextToSeed context, ILogger logger)
    {
        logger.LogInformation("Populating data to product");

        foreach(var supplier in context.Suppliers)
        {
            var products = new Faker<Product>("pt_BR")
                .RuleFor(o => o.Supplier, f => supplier)
                .RuleFor(o => o.ProductName, f => f.Commerce.ProductName())
                .RuleFor(o => o.Price, f => f.Random.Double(100, 200))
                .Generate(3);

            context.AddRange(products);
        }



        context.SaveChangesAsync().Wait();
        return true;
    }
}
