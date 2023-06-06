using Bogus;
using Microsoft.Extensions.Logging;
using Nudes.SeedMaster.Interfaces;
using POC.Context;
using POC.Model;

namespace POC.Seeders;

public class OrdersItemsSeeder : IActualSeeder<Order, Product, POCApiContext>
{
    public void Seed(POCApiContext context, ILogger logger)
    {
        context.SaveChanges();
        var orders = context.Orders;
        var products = context.Products;

        foreach (var order in orders)
        {
            var numitems = new Faker().Random.Int(1, 4);
            order.Products = new Faker().PickRandom(products, numitems).ToList();
        }

        foreach (var product in products)
        {
            var numitems = new Faker().Random.Int(1, 4);
            product.Orders = new Faker().PickRandom(orders, numitems).ToList();

        }

        context.SaveChanges();
    }
}