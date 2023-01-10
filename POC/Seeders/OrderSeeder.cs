using Bogus;
using Microsoft.Extensions.Logging;
using Nudes.SeedMaster.Interfaces;
using POC.Context;
using POC.Model;

namespace POC.Seeders;

public class OrderSeeder : IActualSeeder<Order, POCApiContext>
{
    public void Seed(POCApiContext context, ILogger logger)
    {
        List<Order> orders = new List<Order>();
        logger.LogInformation("Populating Orders");
        foreach (var person in context.People.Local)
        {
            var ordersgen = new Faker<Order>("pt_BR")
                .UseSeed(1)
                .RuleFor(o => o.OrderTime, f => f.Date.Past(2, new DateTime(1976, 04, 12)))
                .RuleFor(o => o.Person, f => person)
                .Generate(30);
            orders.AddRange(ordersgen);
        }
        context.AddRange(orders);
    }
}