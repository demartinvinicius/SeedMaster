using Microsoft.Extensions.Logging;
using NewSeederTester.Data.Domain;
using Nudes.SeedMaster.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewSeederTester.Data.Seeders;

public class OrderSeeder : INewSeeder<Order, ContextToSeed>
{
    public bool Seed(ContextToSeed context, ILogger logger)
    {
        logger.LogInformation("Populating Orders");
        return true;
    }
}
