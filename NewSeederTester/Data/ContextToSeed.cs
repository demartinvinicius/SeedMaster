﻿using Microsoft.EntityFrameworkCore;
using NewSeederTester.Data.Domain;
using Nudes.SeedMaster.Attributes;

namespace NewSeederTester.Data;

public class ContextToSeed : DbContext
{
    public ContextToSeed()
    {
    }

    public ContextToSeed(DbContextOptions<ContextToSeed> options) : base(options)
    {
    }

    [EnableSeeder]
    public DbSet<Person> People { get; set; }

    [EnableSeeder]
    public DbSet<Order> Orders { get; set; }

    [EnableSeeder]
    public DbSet<Product> Products { get; set; }

    [EnableSeeder]
    public DbSet<OrderItems> OrdersItems { get; set; }

    [EnableSeeder]
    public DbSet<Supplier> Suppliers { get; set; }
}