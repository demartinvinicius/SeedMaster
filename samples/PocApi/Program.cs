using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Nudes.SeedMaster;
using Nudes.SeedMaster.Extensions;
using Nudes.SeedMaster.Interfaces;
using Nudes.SeedMaster.Seeder;
using PocApi.Data;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddMvc();

// Adding DbContext For Sample and Sample 2 using the same database
builder.Services.AddDbContext<SampleDbContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("Sample")).EnableSensitiveDataLogging(), ServiceLifetime.Transient);
builder.Services.AddDbContext<Sample2DbContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("Sample2")).EnableSensitiveDataLogging(), ServiceLifetime.Transient);
// Adding DbContext For Test in another database
builder.Services.AddDbContext<TestDbContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("Test")).EnableSensitiveDataLogging(), ServiceLifetime.Transient);

// Adding all dbcontext as a service retrieved for DbContext so we can call all of them as IEnumerable<DbContext>
builder.Services.AddScoped<DbContext>(provider => provider.GetService<SampleDbContext>());
builder.Services.AddScoped<DbContext>(provider => provider.GetService<Sample2DbContext>());
builder.Services.AddScoped<DbContext>(provider => provider.GetService<TestDbContext>());

// Scanning our assembly and adding all ISeed
builder.Services.AddSeedFrom(Assembly.GetExecutingAssembly());

// Adding our EfCoreSeeder service
builder.Services.AddScoped<ISeeder, EfCoreSeeder>();


var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var contexts = scope.ServiceProvider.GetService<IEnumerable<DbContext>>();
    await EnsureDatabasesAreaCreated(contexts);


    var seeder = scope.ServiceProvider.GetService<ISeeder>();
    await seeder.Run();

}

app.Run();

async Task EnsureDatabasesAreaCreated(IEnumerable<DbContext> contexts)
{
    foreach (var context in contexts)
    {
        await context.Database.EnsureCreatedAsync();
    }
}