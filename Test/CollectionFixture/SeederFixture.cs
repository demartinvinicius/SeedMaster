using Bogus;
using Microsoft.Extensions.Logging;
using NLog.Config;
using NLog.Extensions.Logging;
using NLog.Targets;
using Test.MockedContext;

namespace Test.CollectionFixture;

public class SeederFixture
{
    public ILoggerFactory LogFactory { get; private set; }
    public Faker Faker { get; private set; }

    public TestContext TestContextInstance { get; private set; }

    public SeederFixture()
    {
        TestContextInstance = new();

        LogFactory = LoggerFactory.Create(conf => { });

        Faker = new Faker
        {
            Random = new Randomizer(1)
        };
    }
}

[CollectionDefinition("Seeder Collection")]
public class SeederCollection : ICollectionFixture<SeederFixture>
{
}