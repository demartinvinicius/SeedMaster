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
            factqueue.Enqueue(_fixture.TestContextInstance.People.EntityType);
            factqueue.Enqueue(_fixture.TestContextInstance.Suppliers.EntityType);
            factqueue.Enqueue(_fixture.TestContextInstance.Orders.EntityType);
            factqueue.Enqueue(_fixture.TestContextInstance.Products.EntityType);
            factqueue.Enqueue(_fixture.TestContextInstance.OrdersItems.EntityType);
            EfCoreHelpers.FillCleanableEntitiesQueue(_fixture.TestContextInstance, testqueue);
            Assert.All(testqueue, a => Assert.Contains(a, factqueue));
            Assert.All(factqueue, a => Assert.Contains(a, testqueue));
        }

        [Fact]
        public void TestFillSeedableQueue()
        {
            var testqueue = new Queue<IEntityType>();
            var factqueue = new Queue<IEntityType>();

            factqueue.Enqueue(_fixture.TestContextInstance.People.EntityType);
            factqueue.Enqueue(_fixture.TestContextInstance.Orders.EntityType);
            factqueue.Enqueue(_fixture.TestContextInstance.Products.EntityType);
            factqueue.Enqueue(_fixture.TestContextInstance.OrdersItems.EntityType);
            factqueue.Enqueue(_fixture.TestContextInstance.Suppliers.EntityType);
            EfCoreHelpers.FillSeedableQueue(_fixture.TestContextInstance, testqueue);
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
            _fixture.TestContextInstance.People.Add(person);
            await _fixture.TestContextInstance.SaveChangesAsync();
            Assert.True(EfCoreHelpers.EntityHasData(_fixture.TestContextInstance, _fixture.TestContextInstance.People.EntityType));
        }

        [Fact]
        public async void TestEntityHasDataWithNoData()
        {
            _fixture.TestContextInstance.People.RemoveRange(_fixture.TestContextInstance.People.Select(a => a));
            await _fixture.TestContextInstance.SaveChangesAsync();

            Assert.False(EfCoreHelpers.EntityHasData(_fixture.TestContextInstance, _fixture.TestContextInstance.People.EntityType));
        }

        [Fact]
        public async void TestCanCleanData()
        {
            List<DbContext> contexts = new()
            {
                _fixture.TestContextInstance
            };
            var logger = _fixture.LoggerF.CreateLogger<EfCoreSeeder>();

            var person = new Person
            {
                CPF = _fixture.Faker.Person.Cpf(false),
                Name = _fixture.Faker.Person.FullName
            };
            _fixture.TestContextInstance.People.Add(person);
            var order = new Order
            {
                OrderTime = DateTime.Now,
                Person = person
            };
            _fixture.TestContextInstance.Orders.Add(order);
            await _fixture.TestContextInstance.SaveChangesAsync();

            EfCoreSeeder seeder = new EfCoreSeeder(contexts, SeedScanner.GetSeeds(Assembly.GetExecutingAssembly()), logger, _fixture.LoggerF);

            // Act
            await seeder.Clean();
            await _fixture.TestContextInstance.SaveChangesAsync();

            Assert.False(EfCoreHelpers.EntityHasData(_fixture.TestContextInstance, _fixture.TestContextInstance.People.EntityType) ||
                         EfCoreHelpers.EntityHasData(_fixture.TestContextInstance, _fixture.TestContextInstance.Orders.EntityType));
        }

        [Fact]
        public async void CanSeedData()
        {
            List<DbContext> contexts = new()
            {
                _fixture.TestContextInstance
            };
            var logger = _fixture.LoggerF.CreateLogger<EfCoreSeeder>();
            EfCoreSeeder seeder = new EfCoreSeeder(contexts, SeedScanner.GetSeeds(Assembly.GetExecutingAssembly()), logger, _fixture.LoggerF);
            await seeder.Clean();
            await _fixture.TestContextInstance.SaveChangesAsync();
            await seeder.Seed();
            await _fixture.TestContextInstance.SaveChangesAsync();
            var Person = await _fixture.TestContextInstance.People.FirstOrDefaultAsync(a => a.CPF == "012.035.398-94");
            var Supplier = await _fixture.TestContextInstance.Suppliers.FirstOrDefaultAsync(a => a.CNPJ == "47.643.916/0001-23");
            var Orders = await _fixture.TestContextInstance.Orders.FirstOrDefaultAsync(a => a.OrderTime.Ticks == 623180064900943727);

            var Product = _fixture.TestContextInstance.Suppliers.Join(
                _fixture.TestContextInstance.Products,
                supp => supp.Id,
                prod => prod.Supplier.Id,
                (supp, prod) => new { Supp = supp, Prod = prod })
                .Where(prods => prods.Supp.Name == "Barros EIRELI" &&
                                prods.Prod.ProductName == "Inteligente Madeira Sapatos");

            Assert.Equal("Bryan Barros", Person?.Name);
            Assert.Equal("Xavier S.A.", Supplier?.Name);
            Assert.NotNull(Orders);
            Assert.NotNull(Product);
        }
    }
}