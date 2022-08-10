using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nudes.SeedMaster.Attributes;

[AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
public class EnableSeederAttribute : Attribute
{
    
    public EnableSeederAttribute() 
    {
        
    }

}
