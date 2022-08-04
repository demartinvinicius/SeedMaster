using Microsoft.Extensions.Logging;
using NewSeederTester.Data.Domain;
using Nudes.SeedMaster.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bogus;
using Bogus.Extensions.Brazil;

namespace NewSeederTester.Data.Seeders;

public class OrderSeeder : INewSeeder<Order, ContextToSeed>
{
    public List<Order> Seed(ContextToSeed context, ILogger logger)
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
        return orders;
    }
}
