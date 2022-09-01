using Nudes.SeedMaster.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Nudes.SeedMaster;

/// <summary>
/// Class that can be used to find all the seeds from a collection of types.
/// </summary>
public class SeedScanner
{
    public static IEnumerable<ScanResult> FindSeedersInAssembly(params Assembly[] assemblies)
    {
        var exportedTypes = assemblies.SelectMany(d => d.GetExportedTypes().Distinct())
                                      .Where(type => !type.IsAbstract && !type.IsGenericTypeDefinition);

        return exportedTypes.Select(exported => exported.GetInterfaces().FirstOrDefault())
                            .Where(inter => inter != null
                                         && inter.GetGenericTypeDefinition() == typeof(ISeed<,>))
                            .Select(inter => new ScanResult(inter, exportedTypes.Where(implementation => inter.IsAssignableFrom(implementation))
                                                                                                              .Single()));
    }
}