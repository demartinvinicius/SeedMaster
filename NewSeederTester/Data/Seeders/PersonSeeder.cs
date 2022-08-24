using Bogus;
using Bogus.Extensions.Brazil;
using Microsoft.Extensions.Logging;
using Nudes.SeedMaster.Interfaces;

namespace NewSeederTester.Data.Seeders;

public class PersonSeeder : IActualSeeder<NewSeederTester.Data.Domain.Person, ContextToSeed>
{
    public void Seed(ContextToSeed context, ILogger logger)
    {
        logger.LogInformation("Populating Person!");
        var people = new Faker<Domain.Person>("pt_BR")
            .RuleFor(o => o.Name, f => f.Person.FullName)
            .RuleFor(o => o.CPF, f => f.Person.Cpf(true))
            .Generate(30);
        context.AddRange(people);
    }
}