using Bogus;
using Bogus.Extensions.Brazil;
using Microsoft.Extensions.Logging;
using NewSeederTester.Data.Domain;
using Nudes.SeedMaster.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewSeederTester.Data.Seeders;

public class SupplierSeeder : INewSeeder<Supplier, ContextToSeed>
{
    public bool Seed(ContextToSeed context, ILogger logger)
    {
        logger.LogInformation("Populating Supplier");

        var suppliers = new Faker<Supplier>("pt_BR")

            .RuleFor(x => x.Name, f => f.Company.CompanyName(0))
            .RuleFor(x => x.CNPJ, f => f.Company.Cnpj(true))
            .Generate(20);

        context.Suppliers.AddRange(suppliers);
        context.SaveChangesAsync().Wait();

            


        return true;
    }
}
