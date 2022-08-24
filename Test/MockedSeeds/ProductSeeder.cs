using Bogus;
using Microsoft.Extensions.Logging;
using Nudes.SeedMaster.Interfaces;
using Test.MockedContext;
using Test.MockedDomain;

namespace Test.MockedSeeds;

public class ProductSeeder : IActualSeeder<Product, TestContext>
{
    public void Seed(TestContext context, ILogger logger)
    {
        logger.LogInformation("Populating data to product");
        List<Product> products1 = new List<Product>();

        foreach (var supplier in context.Suppliers.Local.OrderBy(a => a.Name))
        {
            var products = new Faker<Product>("pt_BR")
                .UseSeed(1)
                .RuleFor(o => o.Supplier, f => supplier)
                .RuleFor(o => o.ProductName, f => f.Commerce.ProductName())
                .RuleFor(o => o.Price, f => f.Random.Double(100, 200))
                .Generate(3);

            products1.AddRange(products);
        }
        context.AddRange(products1);
    }
}