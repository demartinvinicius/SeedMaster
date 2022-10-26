using Bogus;
using Bogus.Extensions.Brazil;
using Microsoft.Extensions.Logging;
using Nudes.SeedMaster.Interfaces;
using Test.MockedContext;
using Test.MockedDomain;

namespace Test.MockedSeeds;

public class SupplierSeederForAnotherContext : IActualSeeder<AnotherSupplier, AnotherTestContext>
{
    public void Seed(AnotherTestContext context, ILogger logger)
    {
        logger.LogInformation("Populating Supplier for Another Context");

        var suppliers = new Faker<AnotherSupplier>("pt_BR")
            .UseSeed(2)
            .RuleFor(x => x.Name, f => f.Company.CompanyName(0))
            .RuleFor(x => x.CNPJ, f => f.Company.Cnpj(true))
            .Generate(20);

        context.AddRange(suppliers);
    }
}