﻿using Microsoft.Extensions.Logging;
using Nudes.SeedMaster.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bogus;
using Bogus.Extensions.Brazil;
using Test.MockedContext;

namespace Test.MockedSeeds;

public class PersonSeeder : IActualSeeder<MockedDomain.Person,TestContext>
{

    public void Seed(TestContext context, ILogger logger)
    {
        logger.LogInformation("Populating Person!");
        var people = new Faker<MockedDomain.Person>("pt_BR")
            .RuleFor(o => o.Name, f => f.Person.FullName)
            .RuleFor(o => o.CPF, f => f.Person.Cpf(true))
            .Generate(30);
        context.AddRange(people);

        
    }
}
