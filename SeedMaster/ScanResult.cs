using System;

namespace Nudes.SeedMaster;

public partial class SeedScanner 
{
    /// <summary>
    /// Result of performing a scan.
    /// </summary>
    public class ScanResult
    {
        /// <summary>
        /// Creates an instance of an ScanResult.
        /// </summary>
        public ScanResult(Type interfaceType, Type implementationType)
        {
            InterfaceType = interfaceType;
            ImplementationType = implementationType;
        }

        /// <summary>
        /// Seed InterfaceType, it should be ISeed if nothing is changed
        /// </summary>
        public Type InterfaceType { get; private set; }

        /// <summary>
        /// Concrete type that implements the ISeed Type.
        /// </summary>
        public Type ImplementationType { get; private set; }
        
    }

}