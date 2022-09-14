using Bogus;
using Bogus.Extensions.Brazil;
using Microsoft.Extensions.Logging;
using Nudes.SeedMaster.Interfaces;
using Test.MockedContext;

namespace Test.MockedSeeds;

public class GlobalSeed : IActualSeeder<TestContext>
{
    public void Seed(TestContext context, ILogger logger)
    {
        logger.LogInformation("Populating By GlobalSeed Person!");
        var people = new Faker<MockedDomain.Person>("pt_BR")
            .UseSeed(2)
            .RuleFor(o => o.Name, f => f.Person.FullName)
            .RuleFor(o => o.CPF, f => f.Person.Cpf(true))
            .Generate(30);
        context.AddRange(people);
    }
}