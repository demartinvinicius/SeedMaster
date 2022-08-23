using Bogus;
using Microsoft.Extensions.Logging;
using NLog.Config;
using NLog.Extensions.Logging;
using NLog.Targets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Test.MockedContext;

namespace Test.CollectionFixture;

public class SeederFixture
{
    public ILoggerFactory LoggerF { get; private set; }
    public Faker Faker { get; private set; }

    public TestContext TestContext { get; private set; }
    public SeederFixture()
    {
        TestContext = new();

        var target = new FileTarget()
        {
            FileName = @"C:\tmp\SeederLog.txt"
        };
        var config = new LoggingConfiguration();
        config.AddRuleForAllLevels(target);

        LoggerF = LoggerFactory.Create(conf =>
        {
            conf.AddNLog(config);
        });

        // TODO: Implement a configurable bogus with a particular random seed.
        Faker = new Faker();
        Faker.Random = new Randomizer(1);
    }

    
}

[CollectionDefinition("Seeder Collection")]
public class SeederCollection : ICollectionFixture<SeederFixture>
{

}
