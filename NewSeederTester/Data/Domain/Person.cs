using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewSeederTester.Data.Domain;

public class Person
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string CPF { get; set; }
    public List<Order> Orders { get; set; }
}
