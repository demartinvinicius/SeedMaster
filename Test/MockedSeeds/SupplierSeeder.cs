using Bogus;
using Bogus.Extensions.Brazil;
using Microsoft.Extensions.Logging;
using Nudes.SeedMaster.Interfaces;
using Test.MockedContext;
using Test.MockedDomain;

namespace Test.MockedSeeds;

public class SupplierSeeder : IActualSeeder<Supplier, TestContext>
{
    public void Seed(TestContext context, ILogger logger)
    {
        logger.LogInformation("Populating Supplier");

        var suppliers = new Faker<Supplier>("pt_BR")

            .RuleFor(x => x.Name, f => f.Company.CompanyName(0))
            .RuleFor(x => x.CNPJ, f => f.Company.Cnpj(true))
            .Generate(20);


        context.AddRange(suppliers);

    }
}