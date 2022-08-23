using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Test.MockedDomain;



public class Supplier
{
    public Guid Id { get; set; }
    public string Name { get; set; }    
    public string CNPJ { get; set; }
    public List<Product> Products { get; set; }
}
