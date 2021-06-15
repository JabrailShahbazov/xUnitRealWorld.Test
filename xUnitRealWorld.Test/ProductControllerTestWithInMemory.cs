using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Xunit;
using xUnitRealWorld.Web.Controllers;
using xUnitRealWorld.Web.Models;

namespace xUnitRealWorld.Test
{
    public class ProductControllerTestWithInMemory : ProductsControllerTest
    {
        public ProductControllerTestWithInMemory()
        {
            SetContextOptions(new DbContextOptionsBuilder<xUnitTestDbContext>()
                .UseInMemoryDatabase("xUnitTestInMemoryDb").Options);
        }

        [Fact]
        public async Task Create_ModelValidProduct_ReturnRedirectToActionWithSaveProduct()
        {
            var newProduct = new Product() { Name = "Qelem330", Price = 234, Stock = 34 };

            using (var context = new xUnitTestDbContext(_contextOptions))
            {
                var category = context.Categories.First();
                newProduct.CategoryId = category.Id;
                var controller = new ProductsController(context);

                var result = await controller.Create(newProduct);

                var redirect = Assert.IsType<RedirectToActionResult>(result);
                Assert.Equal("Index", redirect.ActionName);

            }

            using (var context = new xUnitTestDbContext(_contextOptions))
            {
                var product = context.Products.FirstOrDefault(x => x.Name == newProduct.Name);
                Assert.Equal(newProduct.Name,product.Name);
            }

        }

    }
}
