using Microsoft.EntityFrameworkCore;
using Nudes.SeedMaster.Interfaces;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Nudes.SeedMaster;

/// <summary>
/// Class that can be used to find all the seeds from a collection of types.
/// </summary>
public partial class SeedScanner
{
    public static IEnumerable<ScanResult> GetSeeds(Assembly assembly)
    {
        var exportedTypes = assembly.GetExportedTypes()
            .Where(type => !type.IsAbstract && !type.IsGenericTypeDefinition);

        return exportedTypes.Select(exported => exported.GetInterfaces().FirstOrDefault())
            .Where(inter => inter != null && inter.GetGenericTypeDefinition() == typeof(IActualSeeder<,>))
            .Select(inter => 
                   new ScanResult(inter, exportedTypes
                         .Where(implemation => inter.IsAssignableFrom(implemation)).Single()));
    }


   
 

}