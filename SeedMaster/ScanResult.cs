using System;

namespace Nudes.SeedMaster;

public partial class SeedScanner
{
    /// <summary>
    /// Result of performing a scan.
    /// </summary>
    public class ScanResult
    {
        public enum SeedTypes
        {
            GlobalSeed,
            EntitySeed
        };

        /// <summary>
        /// Creates an instance of an ScanResult.
        /// </summary>
        public ScanResult(Type interfaceType, Type implementationType, SeedTypes seedType)
        {
            InterfaceType = interfaceType;
            ImplementationType = implementationType;
            SeedType = seedType;
        }

        /// <summary>
        /// Seed InterfaceType, it should be IActualSeeder if nothing is changed
        /// </summary>
        public Type InterfaceType { get; private set; }

        /// <summary>
        /// Concrete type that implements the IActualSeed Type.
        /// </summary>
        public Type ImplementationType { get; private set; }


        public SeedTypes SeedType { get; private set; }
        
    }
}