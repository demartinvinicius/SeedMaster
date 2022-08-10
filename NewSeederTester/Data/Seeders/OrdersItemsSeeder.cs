using Microsoft.Extensions.Logging;
using NewSeederTester.Data.Domain;
using Nudes.SeedMaster.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bogus;

namespace NewSeederTester.Data.Seeders;

public class OrdersItemsSeeder : INewSeeder<OrderItems, ContextToSeed>
{
    public bool Seed(ContextToSeed context, ILogger logger)
    {
        List<OrderItems> orderItems = new List<OrderItems>();
        logger.LogInformation("Order items populated");
        
        var orders = context.Orders.ToList();
        var products = context.Products.ToList();

        foreach(var order in orders)
        {
            foreach(var product in products)
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
        return true;
    }
}
