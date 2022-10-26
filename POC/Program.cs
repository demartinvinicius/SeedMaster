using Microsoft.EntityFrameworkCore;
using Nudes.Retornator.AspnetCore;
using Nudes.SeedMaster;
using Nudes.SeedMaster.Seeder;
using POC.Context;
using System.Reflection;
using static Nudes.SeedMaster.SeedScanner;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers().AddRetornator();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddErrorTranslator(ErrorHttpTranslatorBuilder.Default);
builder.Services.AddDbContext<POCApiContext>();
builder.Services.AddLogging();
builder.Services.AddScoped<IEnumerable<DbContext>>(x => new List<DbContext> { x.GetService<POCApiContext>() });
builder.Services.AddScoped<IEnumerable<ScanResult>>(x => SeedScanner.GetSeeds(Assembly.GetExecutingAssembly()));
builder.Services.AddScoped<EfCoreSeeder>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();