using Bogus;
using Bogus.Extensions.Brazil;
using Microsoft.Extensions.Logging;
using Nudes.SeedMaster.Interfaces;
using POC.Context;
using POC.Model;

namespace POC.Seeders;

public class SupplierSeeder : IActualSeeder<Supplier, POCApiContext>
{
    public void Seed(POCApiContext context, ILogger logger)
    {
        logger.LogInformation("Populating Supplier");

        var suppliers = new Faker<Supplier>("pt_BR")
            .UseSeed(1)
            .RuleFor(x => x.Name, f => f.Company.CompanyName(0))
            .RuleFor(x => x.CNPJ, f => f.Company.Cnpj(true))
            .Generate(20);

        context.AddRange(suppliers);
    }
}