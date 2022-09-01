using Bogus;
using Microsoft.Extensions.Logging;
using Nudes.SeedMaster.Interfaces;
using Test.MockedContext;
using Test.MockedDomain;

namespace Test.MockedSeeds;

public class OrderSeeder : ISeed<Order, TestContext>
{
    public Task Seed(TestContext context)
    {
        var orders = new List<Order>();
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

        return Task.CompletedTask;
    }
}