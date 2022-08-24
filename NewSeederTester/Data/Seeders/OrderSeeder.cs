using Bogus;
using Microsoft.Extensions.Logging;
using NewSeederTester.Data.Domain;
using Nudes.SeedMaster.Interfaces;

namespace NewSeederTester.Data.Seeders;

public class OrderSeeder : IActualSeeder<Order, ContextToSeed>
{
    public void Seed(ContextToSeed context, ILogger logger)
    {
        List<Order> orders = new List<Order>();
        logger.LogInformation("Populating Orders");
        foreach (var person in context.People)
        {
            var ordersgen = new Faker<Order>("pt_BR")
                .RuleFor(o => o.OrderTime, f => f.Date.Past(2))
                .RuleFor(o => o.Person, f => person)
                .Generate(30);
            orders.AddRange(ordersgen);
        }
        context.AddRange(orders);
    }
}