using Microsoft.AspNetCore.Mvc;
using Nudes.Retornator.Core;
using Nudes.SeedMaster.Seeder;
using POC.Context;
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
        await _coreSeeder.Seed();
        await _coreSeeder.Commit();

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
}