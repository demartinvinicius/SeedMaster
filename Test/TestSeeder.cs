using Bogus.Extensions.Brazil;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.Extensions.Logging;
using Nudes.SeedMaster;
using Nudes.SeedMaster.Extensions;
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
            var seeds = SeedScanner.FindSeedersInAssembly(Assembly.GetExecutingAssembly());

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
            var implements = SeedScanner.FindSeedersInAssembly(Assembly.GetExecutingAssembly()).Select(a => a.ImplementationType);
            Assert.Contains(seeder, implements);
        }

        [Theory]
        [InlineData(typeof(ISeed<Order, TestContext>))]
        [InlineData(typeof(ISeed<OrderItems, TestContext>))]
        [InlineData(typeof(ISeed<Person, TestContext>))]
        [InlineData(typeof(ISeed<Product, TestContext>))]
        [InlineData(typeof(ISeed<Supplier, TestContext>))]
        public void GetAllSeedersInterfacesTester(Type interfacename)
        {
            var interfacesnames = SeedScanner.FindSeedersInAssembly(Assembly.GetExecutingAssembly()).Select(a => a.InterfaceType.FullName);
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
            var logger = _fixture.LogFactory.CreateLogger<EfCoreSeeder>();

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

            var seeder = new EfCoreSeeder(_fixture.TestContextInstance, SeedScanner.FindSeedersInAssembly(Assembly.GetExecutingAssembly()), logger);

            // Act
            await seeder.Clean();
            await _fixture.TestContextInstance.SaveChangesAsync();

            Assert.False(EfCoreHelpers.EntityHasData(_fixture.TestContextInstance, _fixture.TestContextInstance.People.EntityType) ||
                         EfCoreHelpers.EntityHasData(_fixture.TestContextInstance, _fixture.TestContextInstance.Orders.EntityType));
        }

        [Fact]
        public async void CanSeedData()
        {
            var logger = _fixture.LogFactory.CreateLogger<EfCoreSeeder>();

            var seeder = new EfCoreSeeder(_fixture.TestContextInstance, SeedScanner.FindSeedersInAssembly(Assembly.GetExecutingAssembly()), logger);

            await seeder.Run();

            var person = await _fixture.TestContextInstance.People.FirstOrDefaultAsync(a => a.CPF == "012.035.398-94");
            var supplier = await _fixture.TestContextInstance.Suppliers.FirstOrDefaultAsync(a => a.CNPJ == "47.643.916/0001-23");
            var orders = await _fixture.TestContextInstance.Orders.FirstOrDefaultAsync(a => a.OrderTime.Ticks == 623180064900943727);

            var product = _fixture.TestContextInstance.Suppliers.Join(
                _fixture.TestContextInstance.Products,
                supp => supp.Id,
                prod => prod.Supplier.Id,
                (supp, prod) => new { Supp = supp, Prod = prod })
                .Where(prods => prods.Supp.Name == "Barros EIRELI" &&
                                prods.Prod.ProductName == "Inteligente Madeira Sapatos");

            person.Should().NotBeNull();
            person.Name.Should().Be("Bryan Barros");

            Assert.Equal("Xavier S.A.", supplier?.Name);
            
            Assert.NotNull(orders);
            
            Assert.NotNull(product);
        }
    }
}