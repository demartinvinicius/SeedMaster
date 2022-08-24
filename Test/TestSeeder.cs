using Bogus.Extensions.Brazil;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.Extensions.Logging;
using Nudes.SeedMaster;
using Nudes.SeedMaster.Interfaces;
using Nudes.SeedMaster.Seeder;
using System.Reflection;
using Test.CollectionFixture;
using Test.MockedContext;
using Test.MockedDomain;
using Test.MockedSeeds;

namespace Test
{
    [Collection("Seeder Collection")]
    public class TestSeeder
    {
        private readonly SeederFixture _fixture;

        public TestSeeder(SeederFixture fixture)
        {
            _fixture = fixture;
        }

        [Fact]
        public void GetSeedNumberTester()
        {
            var seeds = SeedScanner.GetSeeds(Assembly.GetExecutingAssembly());

            seeds.Should().HaveCount(5, "We have 5 ActualSeeders in this test");
        }

        [Theory]
        [InlineData(typeof(OrderSeeder))]
        [InlineData(typeof(OrdersItemsSeeder))]
        [InlineData(typeof(PersonSeeder))]
        [InlineData(typeof(ProductSeeder))]
        [InlineData(typeof(SupplierSeeder))]
        public void GetAllSeedersImplementationsTester(Type seeder)
        {
            var implements = SeedScanner.GetSeeds(Assembly.GetExecutingAssembly()).Select(a => a.ImplementationType);
            Assert.Contains(seeder, implements);
        }

        [Theory]
        [InlineData(typeof(IActualSeeder<Order, TestContext>))]
        [InlineData(typeof(IActualSeeder<OrderItems, TestContext>))]
        [InlineData(typeof(IActualSeeder<Person, TestContext>))]
        [InlineData(typeof(IActualSeeder<Product, TestContext>))]
        [InlineData(typeof(IActualSeeder<Supplier, TestContext>))]
        public void GetAllSeedersInterfacesTester(Type interfacename)
        {
            var interfacesnames = SeedScanner.GetSeeds(Assembly.GetExecutingAssembly()).Select(a => a.InterfaceType.FullName);
            Assert.Contains(interfacename.FullName, interfacesnames);
        }

        [Fact]
        public void TestFillClenableQueue()
        {
            var testqueue = new Queue<IEntityType>();
            var factqueue = new Queue<IEntityType>();
            factqueue.Enqueue(_fixture.TestContext.People.EntityType);
            factqueue.Enqueue(_fixture.TestContext.Suppliers.EntityType);
            factqueue.Enqueue(_fixture.TestContext.Orders.EntityType);
            EfCoreHelpers.FillCleanableEntitiesQueue(_fixture.TestContext, testqueue);
            Assert.All(testqueue, a => Assert.Contains(a, factqueue));
            Assert.All(factqueue, a => Assert.Contains(a, testqueue));
        }
        [Fact]
        public void TestFillSeedableQueue()
        {
            var testqueue = new Queue<IEntityType>();
            var factqueue = new Queue<IEntityType>();

            factqueue.Enqueue(_fixture.TestContext.People.EntityType);
            factqueue.Enqueue(_fixture.TestContext.Orders.EntityType);
            factqueue.Enqueue(_fixture.TestContext.Products.EntityType);
            factqueue.Enqueue(_fixture.TestContext.OrdersItems.EntityType);
            factqueue.Enqueue(_fixture.TestContext.Suppliers.EntityType);
            EfCoreHelpers.FillSeedableQueue(_fixture.TestContext, testqueue);
            Assert.All(testqueue, a => Assert.Contains(a, factqueue));

        }

        [Fact]
        public async void TestEntityHasDataWithData()
        {
            Person person = new Person()
            {
                CPF = _fixture.Faker.Person.Cpf(false),
                Name = _fixture.Faker.Person.FullName,
            };
            _fixture.TestContext.People.Add(person);
            await _fixture.TestContext.SaveChangesAsync();
            Assert.True(EfCoreHelpers.EntityHasData(_fixture.TestContext, _fixture.TestContext.People.EntityType));
        }

        [Fact]
        public async void TestEntityHasDataWithNoData()
        {
            _fixture.TestContext.People.RemoveRange(_fixture.TestContext.People.Select(a => a));
            await _fixture.TestContext.SaveChangesAsync();

            Assert.False(EfCoreHelpers.EntityHasData(_fixture.TestContext, _fixture.TestContext.People.EntityType));
        }

        [Fact]
        public async void TestCanCleanData()
        {
            List<DbContext> contexts = new()
            {
                _fixture.TestContext
            };
            var logger = _fixture.LoggerF.CreateLogger<EfCoreSeeder>();

            var person = new Person
            {
                CPF = _fixture.Faker.Person.Cpf(false),
                Name = _fixture.Faker.Person.FullName
            };
            _fixture.TestContext.People.Add(person);
            var order = new Order
            {
                OrderTime = DateTime.Now,
                Person = person
            };
            _fixture.TestContext.Orders.Add(order);
            await _fixture.TestContext.SaveChangesAsync();


            EfCoreSeeder seeder = new EfCoreSeeder(contexts, SeedScanner.GetSeeds(Assembly.GetExecutingAssembly()), logger, _fixture.LoggerF);

            // Act
            await seeder.Clean();
            await _fixture.TestContext.SaveChangesAsync();

            Assert.False(EfCoreHelpers.EntityHasData(_fixture.TestContext, _fixture.TestContext.People.EntityType) ||
                         EfCoreHelpers.EntityHasData(_fixture.TestContext, _fixture.TestContext.Orders.EntityType));
        }

        [Fact]
        public async void CanSeedData()
        {
            Assert.True(true);
        }
    }
}