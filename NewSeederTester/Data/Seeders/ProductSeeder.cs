using Bogus;
using Microsoft.Extensions.Logging;
using NewSeederTester.Data.Domain;
using Nudes.SeedMaster.Interfaces;

namespace NewSeederTester.Data.Seeders;

public class ProductSeeder : IActualSeeder<Product, ContextToSeed>
{
    public void Seed(ContextToSeed context, ILogger logger)
    {
        logger.LogInformation("Populating data to product");
        List<Product> products1 = new List<Product>();

        foreach (var supplier in context.Suppliers)
        {
            var products = new Faker<Product>("pt_BR")
                .RuleFor(o => o.Supplier, f => supplier)
                .RuleFor(o => o.ProductName, f => f.Commerce.ProductName())
                .RuleFor(o => o.Price, f => f.Random.Double(100, 200))
                .Generate(3);

            products1.AddRange(products);
        }
        context.AddRange(products1);
    }
}