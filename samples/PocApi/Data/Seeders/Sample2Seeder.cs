using Nudes.SeedMaster.Interfaces;
using System;
using System.Threading.Tasks;
using PocApi.Data.Domain;

namespace PocApi.Data.Seeders
{
    public class Sample2Seed : ISeed<Sample2, Sample2DbContext>
    {
        public Task Seed(Sample2DbContext dbContext)
        {
            dbContext.Samples2.Add(new Sample2
            {
                SampleText = "Sample"
            });

            return Task.CompletedTask;
        }
    }
}
