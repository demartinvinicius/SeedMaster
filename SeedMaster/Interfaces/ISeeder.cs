using System;
using System.Threading.Tasks;

namespace Nudes.SeedMaster.Interfaces
{
    /// <summary>
    /// Basic seeder manager implementation its purpose is to control the flow of data seeding
    /// </summary>
    public interface ISeeder : IDisposable
    {
        /// <summary>
        /// Clean method should clean all the data in all contexts
        /// </summary>
        Task Clean();

        /// <summary>
        /// Seed method that will inject data
        /// </summary>
        /// <returns></returns>
        Task Seed();

        /// <summary>
        /// Commit method that saves the data alteration
        /// </summary>
        Task Commit();

        /// <summary>
        /// Method that control the flow of data seeding running all necessary steps
        /// Clean
        /// Seed
        /// Commit
        /// </summary>
        Task Run();
    }
}