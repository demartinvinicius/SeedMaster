using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewSeederTester.Data.Domain;

public class Product
{
    public Guid Id { get; set; }   
    public string ProductName { get; set; }
    public double Price { get; set; }
    public List<OrderItems> OrderItems { get; set; }
}
