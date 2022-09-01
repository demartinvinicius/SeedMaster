using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Nudes.SeedMaster.Extensions
{
    public static class SeedMasterExtensions
    {
        public static void ForEach(this IEnumerable<ScanResult> scanResults, Action<ScanResult> action)
        {
            foreach (var result in scanResults)
            {
                action(result);
            }
        }

        public static void AddSeedFrom(this IServiceCollection services, params Assembly[] assemblies)
            => SeedScanner.FindSeedersInAssembly(assemblies)
                          .ForEach(d => services.AddScoped(d.InterfaceType, d.ImplementationType));
    }


}
