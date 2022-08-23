using Microsoft.Extensions.Logging;
using Nudes.SeedMaster.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bogus;
using Test.MockedDomain;
using Test.MockedContext;

namespace Test.MockedSeeds;

public class OrdersItemsSeeder : IActualSeeder<OrderItems, TestContext>
{
    public void Seed(TestContext context, ILogger logger)
    {
        List<OrderItems> orderItems = new List<OrderItems>();
                
        var orders = context.Orders.Take(3).ToList();
        var products = context.Products.Take(4).ToList();

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
        
    }
}
