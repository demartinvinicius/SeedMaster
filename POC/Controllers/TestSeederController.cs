using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Nudes.Retornator.Core;
using Nudes.SeedMaster.Seeder;
using POC.Context;
using POC.DTO;
using POC.Model;

namespace POC.Controllers;

[ApiController]
[Route("SeederTest")]
public class TestSeederController : Controller
{
    private readonly EfCoreSeeder _coreSeeder;
    private readonly POCApiContext _pocApiContext;

    public TestSeederController(EfCoreSeeder coreSeeder, POCApiContext pocApiContext)
    {
        _coreSeeder = coreSeeder;
        _pocApiContext = pocApiContext;
    }

    [HttpPost]
    public async Task<IActionResult> SeedDataAsync()
    {
        //await _coreSeeder.Seed();
        //await _coreSeeder.Commit();
        await _coreSeeder.Run();

        return Ok();
    }

    [HttpGet]
    [Route("Suppliers")]
    public ResultOf<IEnumerable<Supplier>> GetSuppliers()
    {
        return _pocApiContext.Suppliers;
    }

    [HttpGet]
    [Route("People")]
    public ResultOf<IEnumerable<Person>> GetPeople()
    {
        return _pocApiContext.People;
    }

    [HttpGet]
    [Route("Orders")]
    public ResultOf<IEnumerable<Order>> GetOrdes()
    {
        return _pocApiContext.Orders;
    }

    [HttpGet]
    [Route("OrdersWithDetails")]
    public ResultOf<IEnumerable<EntireOrder>> GetOrdersWithDetails()
    {
        var resultList = new List<EntireOrder>();

        return _pocApiContext.Orders
            .Include(s => s.Person)
            .Include(s => s.OrderItems)
                .ThenInclude(s => s.Product)
                    .ThenInclude(s => s.Supplier)
                    .Select(s => new EntireOrder()
                    {
                        ConsumerName = s.Person.Name,
                        OrderTime = s.OrderTime,
                        TotalPrice = decimal.Round(s.OrderItems.Sum(i => i.Qty * i.Product.Price), 2),
                        OrderItems = s.OrderItems.Select(o => new EachOrderItem()
                        {
                            SupplierName = o.Product.Supplier.Name,
                            ProductName = o.Product.ProductName,
                            QuantityOrdered = o.Qty,
                            UnitPrice = decimal.Round(o.Product.Price, 2)
                        }).ToList()
                    }).ToList();
    }
}