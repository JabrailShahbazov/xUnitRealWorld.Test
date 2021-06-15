using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using xUnitRealWorld.Web.Models;

namespace xUnitRealWorld.Test
{
    public class ProductsControllerTest
    {
        protected DbContextOptions<xUnitTestDbContext> _contextOptions { get; set;}

        protected void SetContextOptions(DbContextOptions<xUnitTestDbContext> contextOptions)
        {
            _contextOptions = contextOptions;
            Seed();
        }

        private void Seed()
        {
            using (xUnitTestDbContext context = new xUnitTestDbContext(_contextOptions))
            {
                context.Database.EnsureDeleted();
                context.Database.EnsureCreated();

                context.Products.Add(new Product
                    {CategoryId = 1, Name = "Qelem 10", Price = 23, Stock = 100, Color = "Red"});
                context.Products.AddAsync(new Product
                    {CategoryId = 1, Name = "Qelem 1", Price = 44, Stock = 10, Color = "Black"});
                context.SaveChanges();
            }
        }
    }
}
