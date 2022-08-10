using Bogus;
using Bogus.Extensions.Brazil;
using Microsoft.Extensions.Logging;
using NewSeederTester.Data.Domain;
using Nudes.SeedMaster.Interfaces;

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


        context.AddRange(suppliers);
        return true;
    }
}