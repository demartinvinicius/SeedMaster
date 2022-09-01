using Bogus;
using Bogus.Extensions.Brazil;
using Microsoft.Extensions.Logging;
using Nudes.SeedMaster.Interfaces;
using Test.MockedContext;

namespace Test.MockedSeeds;

public class PersonSeeder : ISeed<MockedDomain.Person, TestContext>
{

    public Task Seed(TestContext context)
    {
        var people = new Faker<MockedDomain.Person>("pt_BR")
            .UseSeed(1)
            .RuleFor(o => o.Name, f => f.Person.FullName)
            .RuleFor(o => o.CPF, f => f.Person.Cpf(true))
            .Generate(30);

        context.AddRange(people);

        return Task.CompletedTask;  
    }
}