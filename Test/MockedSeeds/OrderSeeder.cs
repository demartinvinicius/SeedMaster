using Microsoft.Extensions.Logging;

using Nudes.SeedMaster.Interfaces;
using Bogus;
using Bogus.Extensions.Brazil;
using Test.MockedDomain;
using Test.MockedContext;

namespace Test.MockedSeeds;

public class OrderSeeder : IActualSeeder<Order, TestContext>
{
    public void Seed(TestContext context, ILogger logger)
    {
        List<Order> orders = new List<Order>();
        logger.LogInformation("Populating Orders");
        foreach(var person in context.People)
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
