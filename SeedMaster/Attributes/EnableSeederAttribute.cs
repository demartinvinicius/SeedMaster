using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nudes.SeedMaster.Attributes;

[AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
public class EnableSeederAttribute : Attribute
{
    public bool Clean { get; private set; }
    
    public EnableSeederAttribute(bool Clean = false) 
    {
        this.Clean = Clean;
        
    }

}
