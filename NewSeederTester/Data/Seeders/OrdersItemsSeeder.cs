using Microsoft.Extensions.Logging;
using NewSeederTester.Data.Domain;
using Nudes.SeedMaster.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewSeederTester.Data.Seeders;

public class OrdersItemsSeeder : INewSeeder<OrderItems, ContextToSeed>
{
    public bool Seed(ContextToSeed context, ILogger logger)
    {
        logger.LogInformation("Order items populated");
        return true;
    }
}
