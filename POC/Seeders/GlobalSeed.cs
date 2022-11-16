using Bogus;
using Bogus.Extensions.Brazil;
using Microsoft.Extensions.Logging;
using Nudes.SeedMaster.Interfaces;
using POC.Context;

namespace POC.Seeders;

public class GlobalSeed : IActualSeeder<POCApiContext>
{
    public void Seed(POCApiContext context, ILogger logger)
    {
        logger.LogInformation("Populating By GlobalSeed Person!");
        var people = new Faker<POC.Model.Person>("pt_BR")
            .UseSeed(2)
            .RuleFor(o => o.Name, f => f.Person.FullName)
            .RuleFor(o => o.CPF, f => f.Person.Cpf(true))
            .Generate(2);
        context.AddRange(people);
    }
}