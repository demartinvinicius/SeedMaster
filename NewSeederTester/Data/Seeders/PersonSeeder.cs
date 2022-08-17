using Microsoft.Extensions.Logging;
using NewSeederTester.Data.Domain;
using Nudes.SeedMaster.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bogus;
using Bogus.Extensions.Brazil;

namespace NewSeederTester.Data.Seeders;

public class PersonSeeder : INewSeeder<NewSeederTester.Data.Domain.Person,ContextToSeed>
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
