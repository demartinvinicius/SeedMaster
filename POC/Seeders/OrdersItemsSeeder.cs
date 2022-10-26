using Bogus;
using Microsoft.Extensions.Logging;
using Nudes.SeedMaster.Interfaces;
using POC.Context;
using POC.Model;

namespace POC.Seeders;

public class OrdersItemsSeeder : IActualSeeder<OrderItems, POCApiContext>
{
    public void Seed(POCApiContext context, ILogger logger)
    {
        List<OrderItems> orderItems = new List<OrderItems>();

        var orders = context.Orders.Local.Take(3).ToList();
        var products = context.Products.Local.Take(4).ToList();

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