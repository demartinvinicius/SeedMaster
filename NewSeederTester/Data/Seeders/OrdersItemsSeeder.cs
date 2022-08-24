using Bogus;
using Microsoft.Extensions.Logging;
using NewSeederTester.Data.Domain;
using Nudes.SeedMaster.Interfaces;

namespace NewSeederTester.Data.Seeders;

public class OrdersItemsSeeder : IActualSeeder<OrderItems, ContextToSeed>
{
    public void Seed(ContextToSeed context, ILogger logger)
    {
        List<OrderItems> orderItems = new List<OrderItems>();

        var orders = context.Orders.Take(3).ToList();
        var products = context.Products.Take(4).ToList();

        foreach (var order in orders)
        {
            foreach (var product in products)
            {
                var numitems = new Faker().Random.Int(1, 4);
                var orderi = new Faker<OrderItems>("pt_BR")
                    .RuleFor(o => o.Qty, f => f.Random.UInt(1, 3))
                    .RuleFor(o => o.Order, f => order)
                    .RuleFor(o => o.Product, f => product)
                    .Generate(numitems);
                orderItems.AddRange(orderi);
            }
        }
        context.AddRange(orderItems);

    }
}
