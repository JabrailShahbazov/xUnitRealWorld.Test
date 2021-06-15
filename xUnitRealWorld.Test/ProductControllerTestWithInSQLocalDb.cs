﻿using System;
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
    public class ProductControllerTestWithInSqLocalDb : ProductsControllerTest
    {
        public ProductControllerTestWithInSqLocalDb()
        {
            var sqlConnection =
                @"Server=(localdb)\MSSQLLocalDB;Database=TestDb;Trusted_Connection=true;MultipleActiveResultSets=true";
            SetContextOptions(new DbContextOptionsBuilder<xUnitTestDbContext>()
                .UseSqlServer(sqlConnection).Options);
        }

        [Fact]
        public async Task Create_ModelValidProduct_ReturnRedirectToActionWithSaveProduct()
        {
            var newProduct = new Product() { Name = "Qelem330", Price = 234, Stock = 34, Color = "Black" };

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
                Assert.Equal(newProduct.Name, product.Name);
            }

        }


        [Theory]
        [InlineData(1)]
        public async Task DeleteCategory_ExistCategoryId_DeleteAllProducts(int categoryId)
        {
            using (var context = new xUnitTestDbContext(_contextOptions))
            {
                var category = await context.Categories.FindAsync(categoryId);

                context.Categories.Remove(category);
                await context.SaveChangesAsync();
            }

            using (var context = new xUnitTestDbContext(_contextOptions))
            {
                var product = await context.Products.Where(x => x.CategoryId == categoryId).ToListAsync();
                Assert.Empty(product);
            }
        }
    }
}
