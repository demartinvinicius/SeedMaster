using System;

namespace Nudes.SeedMaster.Attributes;

[AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
public class EnableSeederAttribute : Attribute
{
    public bool Clean { get; private set; }

    /// <summary>
    /// Attribute used to mark a DbSet to be seeded
    /// </summary>
    /// <param name="Clean">If true the table will be clean during the seed process</param>
    public EnableSeederAttribute(bool Clean = true)
    {
        this.Clean = Clean;
    }
}