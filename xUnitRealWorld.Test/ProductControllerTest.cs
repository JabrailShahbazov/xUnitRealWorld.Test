using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;
using xUnitRealWorld.Web.Controllers;
using xUnitRealWorld.Web.Models;
using xUnitRealWorld.Web.Repository;

namespace xUnitRealWorld.Test
{
    public class ProductControllerTest
    {
        private readonly Mock<IRepository<Product>> _mockRepo;
        private readonly ProductController _controller;
        private readonly List<Product> _products;
        public ProductControllerTest()
        {
            _mockRepo = new Mock<IRepository<Product>>();
            _controller = new ProductController(_mockRepo.Object);
            _products = new List<Product>()
            {
                new Product(){Id = 1,Color = "Red",Name = "Cup",Price =255,Stock = 23},
                new Product(){Id = 2,Color = "Blue",Name = "Book",Price =11,Stock = 2},
                new Product(){Id = 3,Color = "Green",Name = "Pencil",Price =1,Stock = 234},
            };

        }

        [Fact]
        public async void Index_ActionExecutes_ReturnView()
        {
            var result = await _controller.Index();

            Assert.IsType<ViewResult>(result);
        }

        [Fact]
        public async void Index_ActionExecutes_ReturnProductList()
        {
            //Act
            _mockRepo.Setup(repo => repo.GetAll()).ReturnsAsync(_products);
            var result = await _controller.Index();

            //Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var productList = Assert.IsAssignableFrom<IEnumerable<Product>>(viewResult.Model);
            Assert.Equal<int>(3, productList.Count());
        }

        [Fact]
        public async void Details_IdIsNull_ReturnRedirectToIndex()
        {
            //Act
            var result = await _controller.Details(null);

            //Assert
            var redirect = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirect.ActionName);

        }

        [Fact]
        public async Task Details_IdInValid_ReturnNodFound()
        {
            //Act
            _mockRepo.Setup(x => x.GetById(0)).ReturnsAsync((Product)null);
            var result = await _controller.Details(0);

            //Assert
            var redirect = Assert.IsType<NotFoundResult>(result);
            Assert.Equal<int>(404, redirect.StatusCode);
        }

        [Theory]
        [InlineData(1)]
        public async void Details_ValidId_ReturnProduct(int productId)
        {
            //Act
            Product product = _products.FirstOrDefault(x => x.Id == productId);
            _mockRepo.Setup(repo => repo.GetById(productId)).ReturnsAsync(product);
            var result = await _controller.Details(productId);

            //Assert22
            var viewResult = Assert.IsType<ViewResult>(result);
            var resultProduct = Assert.IsAssignableFrom<Product>(viewResult.Model);
            Assert.Equal(product.Id, resultProduct.Id);
            Assert.Equal(product.Name, resultProduct.Name);
        }

        [Fact]
        public void Create_ActionExecutes_ReturnView()
        {
            var result = _controller.Create();

            Assert.IsType<ViewResult>(result);
        }

        [Fact]
        public async void CreatePOST_InValidModelState_ReturnView()
        {
            _controller.ModelState.AddModelError("Name", "Name is required");

            var result = await _controller.Create(_products.First());

            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.IsType<Product>(viewResult.Model);
        }

        [Fact]
        public async void CreatePOST_ValidModelState_ReturnRedirectToAction()
        {
            var result = await _controller.Create(_products.First());

            var redirect = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirect.ActionName);
        }

        [Fact]
        public async void CreatePOST_ValidModelState_ReturnCreateModelExecute()
        {
            Product newProduct = null;

            _mockRepo.Setup(repo => repo.Create(It.IsAny<Product>())).Callback<Product>(x => newProduct = x);
            var result = await _controller.Create(_products.First());

            _mockRepo.Verify(repo => repo.Create(It.IsAny<Product>()), Times.Once);
            Assert.Equal(_products.First().Id, newProduct.Id);
        }

        [Fact]
        public async void CreatePOST_InValidModelState_ReturnNeverCreateExecute()
        {
            _controller.ModelState.AddModelError("Name", "");
            var result = await _controller.Create(_products.First());

            _mockRepo.Verify(repo => repo.Create(It.IsAny<Product>()), Times.Never);
        }

        [Fact]
        public async void Edit_IdNull_RedirectToIndexAction()
        {
            var result = await _controller.Edit(null);

            var redirect = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirect.ActionName);
        }

        [Fact]
        public async Task Edit_IdInValid_ReturnNotFound()
        {
            _mockRepo.Setup(repo => repo.GetById(0)).ReturnsAsync((Product)null);

            var result = await _controller.Edit(0);

            var redirect = Assert.IsType<NotFoundResult>(result);
            Assert.Equal(404, redirect.StatusCode);
        }

        [Theory]
        [InlineData(2)]
        public async void Edit_ActionExecute_ReturnProduct(int productId)
        {
            var product = _products.First(x => x.Id == productId);
            _mockRepo.Setup(repo => repo.GetById(productId)).ReturnsAsync(product);
            var result = await _controller.Edit(productId);

            var viewResult = Assert.IsType<ViewResult>(result);
            var resultProduct = Assert.IsAssignableFrom<Product>(viewResult.Model);
            Assert.Equal(product.Id, resultProduct.Id);
        }

        [Theory]
        [InlineData(2)]
        public void EditPOST_IdIsNotFound_ReturnNotFound(int productId)
        {
            var result = _controller.Edit(5, _products.First(x => x.Id == productId));

            var redirect = Assert.IsType<NotFoundResult>(result);
        }
        [Theory]
        [InlineData(2)]
        public void EditPOST_ModelStateNotValid_ReturnViewResult(int productId)
        {
            var product = _products.First(x => x.Id == productId);
            _controller.ModelState.AddModelError("Name", "");
            var result = _controller.Edit(productId, product);

            var viewReuslt = Assert.IsType<ViewResult>(result);
            Assert.IsType<Product>(viewReuslt.Model);
        }

        [Theory]
        [InlineData(2)]
        public void EditPOST_ValidModelState_ReturnRedirectToIndexAction(int productId)
        {
            var result = _controller.Edit(productId, _products.First(x => x.Id == productId));

            var redirect = Assert.IsType<RedirectToActionResult>(result);

            Assert.Equal("Index", redirect.ActionName);
        }

        [Theory]
        [InlineData(2)]
        public void EditPOST_ValidModelState_UpdateMethodExecute(int productId)
        {
            var product = _products.First(x => x.Id == productId);
            _mockRepo.Setup(x => x.Update(product));

            _controller.Edit(productId, product);

            _mockRepo.Verify(repo => repo.Update(It.IsAny<Product>()), Times.Once);
        }

        [Fact]
        public async void Delete_IdIsNull_ReturnNotFound()
        {
            var result = await _controller.Delete(null);
            Assert.IsType<NotFoundResult>(result);
        }

        [Theory]
        [InlineData(0)]
        public async void Delete_IdIsNotFound_ReturnNotFound(int productId)
        {
            _mockRepo.Setup(x => x.GetById(productId)).ReturnsAsync((Product)null);

            var result = await _controller.Delete(productId);

            Assert.IsType<NotFoundResult>(result);
        }

        [Theory]
        [InlineData(1)]
        public async void Delete_ActionExecutes_ReturnProduct(int productId)
        {
            var product = _products.First(x => x.Id == productId);
            _mockRepo.Setup(x => x.GetById(productId)).ReturnsAsync(product);
            var result = await _controller.Delete(productId);

            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.IsAssignableFrom<Product>(viewResult.Model);
        }

        [Theory]
        [InlineData(1)]
        public async void DeletePOST_ActionExecutes_ReturnRedirectToIndexResult(int productId)
        {
            var result = await _controller.DeleteConfirmed(productId);
            Assert.IsType<RedirectToActionResult>(result);
        }

        [Theory]
        [InlineData(1)]
        public async void DeletePOST_ActionExecutes_DeleteMethodExecute(int productId)
        {
            var product = _products.First(x => x.Id == productId);
            _mockRepo.Setup(x => x.Delete(product));
            await _controller.DeleteConfirmed(productId);
            _mockRepo.Verify(x => x.Delete(It.IsAny<Product>()), Times.Once);
        }

        [Fact]
        public void ProductExists_ProductIsNull_ReturnFalse()
        {
            _mockRepo.Setup(x => x.GetById(1)).ReturnsAsync((Product)null);
            var result = _controller.ProductExists(1);
            var viewResult = Assert.IsType<bool>(result);
            Assert.False(viewResult);
        }

        [Theory]
        [InlineData(1)]
        public void ProductExists_ProductIsNotNull_ReturnTrue(int productId)
        {
            //Act
            var product = _products.Find(p => p.Id == productId);
            _mockRepo.Setup(repo => repo.GetById(productId)).ReturnsAsync(product);
            var result = _controller.ProductExists(productId);

            //Assert
            var viewResult = Assert.IsType<bool>(result);
            Assert.True(viewResult);
        }

    }
}
