using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewSeederTester.Data.Domain;


public class OrderItems
{
    public Guid Id { get; set; }
    public Order Order { get; set; }
    public Product Product { get; set; }
    public uint Qty { get; set; }   


}
