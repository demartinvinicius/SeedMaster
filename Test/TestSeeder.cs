using Bogus.Extensions.Brazil;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Nudes.SeedMaster;
using Nudes.SeedMaster.Interfaces;
using Nudes.SeedMaster.Seeder;
using System.Reflection;
using Test.CollectionFixture;
using Test.MockedContext;
using Test.MockedDomain;
using Test.MockedSeeds;
using static Nudes.SeedMaster.SeedScanner;

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

            seeds.Should().HaveCount(7, "We have 7 ActualSeeders in this test");
        }

        [Fact]
        public void GetGlobalSeederTester()
        {
            var seeds = SeedScanner.GetSeeds(Assembly.GetExecutingAssembly());
            seeds.Should().ContainSingle(x => x.SeedType == ScanResult.SeedTypes.GlobalSeed);
            seeds.Should().ContainSingle(x => x.ImplementationType == typeof(GlobalSeed));
            seeds.Should().ContainSingle(x => x.InterfaceType == typeof(IActualSeeder<TestContext>));
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
            _fixture.TestContextInstance.OrdersItems.RemoveRange(_fixture.TestContextInstance.OrdersItems.IgnoreQueryFilters().ToList());
            await _fixture.TestContextInstance.SaveChangesAsync();
            _fixture.TestContextInstance.Orders.RemoveRange(_fixture.TestContextInstance.Orders.IgnoreQueryFilters().ToList());
            await _fixture.TestContextInstance.SaveChangesAsync();
            _fixture.TestContextInstance.People.RemoveRange(_fixture.TestContextInstance.People.IgnoreQueryFilters().ToList());
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

            EfCoreSeeder seeder = new EfCoreSeeder(contexts, SeedScanner.GetSeeds(Assembly.GetExecutingAssembly()), _fixture.LoggerF);

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

            EfCoreSeeder seeder = new EfCoreSeeder(contexts, SeedScanner.GetSeeds(Assembly.GetExecutingAssembly()), _fixture.LoggerF);

            await seeder.Run();

            var Person = await _fixture.TestContextInstance.People.FirstOrDefaultAsync(a => a.CPF == "012.035.398-94");
            var PersonGlobal = await _fixture.TestContextInstance.People.FirstOrDefaultAsync(a => a.CPF == "026.835.079-50");
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
            Assert.Equal("Joana Santos", PersonGlobal?.Name);
            Assert.Equal("Xavier S.A.", Supplier?.Name);
            Assert.NotNull(Orders);
            Assert.NotNull(Product);
        }

        [Fact]
        public async void EnsureExceptionThrowedOnNoSeeder()
        {
            List<DbContext> contexts = new()
            {
                _fixture.TestContextInstance,
                _fixture.YetAnotherContextInstance
            };

            EfCoreSeeder seeder = new EfCoreSeeder(contexts, SeedScanner.GetSeeds(Assembly.GetExecutingAssembly()), _fixture.LoggerF);
            await seeder.Clean();
            await _fixture.TestContextInstance.SaveChangesAsync();
            await _fixture.YetAnotherContextInstance.SaveChangesAsync();
            await Assert.ThrowsAsync<EntryPointNotFoundException>(() => seeder.Seed());
        }

        [Fact]
        public async void CanSeedDataOnMultipleContexts()
        {
            List<DbContext> contexts = new()
            {
                _fixture.TestContextInstance,
                _fixture.AnotherTestContextInstance
            };

            EfCoreSeeder seeder = new EfCoreSeeder(contexts, GetSeeds(Assembly.GetExecutingAssembly()), _fixture.LoggerF);
            await seeder.Run();

            var supplier1 = await _fixture.TestContextInstance.Suppliers.FirstOrDefaultAsync(a => a.CNPJ == "47.643.916/0001-23");
            var supplier2 = await _fixture.AnotherTestContextInstance.OtherSuppliers.FirstOrDefaultAsync(a => a.CNPJ == "19.138.420/0001-67");

            Assert.Equal("Xavier S.A.", supplier1?.Name);
            Assert.Equal("Saraiva EIRELI", supplier2?.Name);
        }
    }
}