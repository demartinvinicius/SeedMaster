using Nudes.SeedMaster.Interfaces;
using PocApi.Data.Domain;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PocApi.Data.Seeders
{
    public class TestSeeder : ISeed<Test, TestDbContext>
    {
        public Task Seed(TestDbContext dbContext)
        {
            dbContext.Add(new Test
            {
                Id = Guid.NewGuid(),
                Title = "Title 3.0"
            });


            var left1 = new ManyLeft()
            {
                Name = "left1",
                Rights = new List<ManyRight>()
            };

            var left2 = new ManyLeft()
            {
                Name = "left2",
                Rights = new List<ManyRight>()
            };

            var right1 = new ManyRight()
            {
                Name = "right1",
                Lefts = new List<ManyLeft>()
            };

            var right2 = new ManyRight()
            {
                Name = "right2",
                Lefts = new List<ManyLeft>()
            };

            left1.Rights.Add(right1);
            left1.Rights.Add(right2);

            left2.Rights.Add(right2);



            dbContext.Lefts.AddRange(left1, left2);
            dbContext.Rights.AddRange(right1, right2);

            return Task.CompletedTask;
        }
    }
}
