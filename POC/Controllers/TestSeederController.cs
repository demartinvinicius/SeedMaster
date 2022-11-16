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

        var orders = _pocApiContext.Orders.Include(s => s.OrderItems).ToList();
        foreach (var order in orders)
        {
            resultList.Add(new EntireOrder()
            {
                ConsumerName = _pocApiContext.People.Where(p => p.Id == order.PersonId).Select(p => p.Name).Single(),
                OrderTime = order.OrderTime,
                TotalPrice = order.OrderItems.Sum(o => o.Qty * _pocApiContext.Products.Where(p => p.Id == o.ProductId).Select(p => p.Price).Single()),
                OrderItems = order.OrderItems.Select(o => new EachOrderItem()
                {
                    ProductName = _pocApiContext.Products.Where(p => p.Id == o.ProductId).Select(p => p.ProductName).Single(),
                    QuantityOrdered = o.Qty,
                    SupplierName = _pocApiContext.Suppliers.Where(s => _pocApiContext.Products.Where(p => p.Id == o.ProductId).Single().SupplierId == s.Id).Select(s => s.Name).Single(),
                    UnitPrice = _pocApiContext.Products.Where(p => p.Id == o.ProductId).Select(p => p.Price).Single()
                }).ToList()
            });
        }

        //var orderx = _pocApiContext.People.Join(_pocApiContext.Orders, k => k, x => x.Person, (y, x) => new EntireOrder()
        //{
        //    ConsumerName = y.Name,
        //    OrderTime = x.OrderTime,
        //    OrderItems = x.OrderItems.Select(z => new EachOrderItem()
        //    {
        //        SupplierName = z.Product.Supplier.Name,
        //        ProductName = z.Product.ProductName,
        //        QuantityOrdered = z.Qty,
        //        UnitPrice = z.Product.Price
        //    }).ToList()
        //}).ToList();

        return resultList;
    }
}