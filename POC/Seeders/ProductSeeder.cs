using Bogus;
using Microsoft.Extensions.Logging;
using Nudes.SeedMaster.Interfaces;
using POC.Context;
using POC.Model;

namespace POC.Seeders;

public class ProductSeeder : IActualSeeder<Product, POCApiContext>
{
    public void Seed(POCApiContext context, ILogger logger)
    {
        logger.LogInformation("Populating data to product");
        List<Product> products1 = new List<Product>();

        foreach (var supplier in context.Suppliers.Local.OrderBy(a => a.Name))
        {
            var products = new Faker<Product>("pt_BR")
                .UseSeed(1)
                .RuleFor(o => o.Supplier, f => supplier)
                .RuleFor(o => o.ProductName, f => f.Commerce.ProductName())
                .RuleFor(o => o.Price, f => f.Random.Decimal(100, 200))
                .Generate(3);

            products1.AddRange(products);
        }
        context.AddRange(products1);
    }
}