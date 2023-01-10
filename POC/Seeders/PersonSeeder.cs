using Bogus;
using Bogus.Extensions.Brazil;
using Microsoft.Extensions.Logging;
using Nudes.SeedMaster.Interfaces;
using POC.Context;

namespace POC.Seeders;

public class PersonSeeder : IActualSeeder<POC.Model.Person, POCApiContext>
{
    public void Seed(POCApiContext context, ILogger logger)
    {
        logger.LogInformation("Populating Person!");
        var people = new Faker<POC.Model.Person>("pt_BR")
            .UseSeed(1)
            .RuleFor(o => o.Name, f => f.Person.FullName)
            .RuleFor(o => o.CPF, f => f.Person.Cpf(true))
            .Generate(30);
        context.AddRange(people);
    }
}