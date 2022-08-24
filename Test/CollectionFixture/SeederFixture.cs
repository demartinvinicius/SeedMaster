using Bogus;
using Microsoft.Extensions.Logging;
using NLog.Config;
using NLog.Extensions.Logging;
using NLog.Targets;
using Test.MockedContext;

namespace Test.CollectionFixture;

public class SeederFixture
{
    public ILoggerFactory LoggerF { get; private set; }
    public Faker Faker { get; private set; }

    public TestContext TestContextInstance { get; private set; }

    public SeederFixture()
    {
        TestContextInstance = new();
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

        Faker = new Faker();
        Faker.Random = new Randomizer(1);
    }
}

[CollectionDefinition("Seeder Collection")]
public class SeederCollection : ICollectionFixture<SeederFixture>
{
}