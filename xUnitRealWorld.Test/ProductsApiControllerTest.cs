using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using xUnitRealWorld.Web.Models;
using xUnitRealWorld.Web.Repository;
using Moq;
using Xunit;
using xUnitRealWorld.Web.Controllers;

namespace xUnitRealWorld.Test
{

    public class ProductsApiControllerTest
    {
        private readonly Mock<IRepository<Product>> _mockRepo;
        private readonly ProductsApiController _controller;
        private readonly List<Product> _products;
        public ProductsApiControllerTest()
        {
            _mockRepo = new Mock<IRepository<Product>>();
            _controller = new ProductsApiController(_mockRepo.Object);
            _products = new List<Product>()
            {
                new Product(){Id = 1,Color = "Red",Name = "Cup",Price =255,Stock = 23},
                new Product(){Id = 2,Color = "Blue",Name = "Book",Price =11,Stock = 2},
                new Product(){Id = 3,Color = "Green",Name = "Pencil",Price =1,Stock = 234},
            };


        }

        [Fact]
        public async void GetProduct_ActionExecute_ReturnOkResultWithProduct()
        {
            _mockRepo.Setup(x => x.GetAll()).ReturnsAsync(_products);

            var result = await _controller.GetProducts();

            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnProduct = Assert.IsAssignableFrom<IEnumerable<Product>>(okResult.Value);
            Assert.Equal(3, returnProduct.ToList().Count);
        }

        [Theory]
        [InlineData(0)]
        public async void GetProduct_IdInValid_ReturnNotFound(int productId)
        {
            _mockRepo.Setup(x => x.GetById(productId)).ReturnsAsync((Product)null);

            var result = await _controller.GetProduct(productId);

            Assert.IsType<NotFoundResult>(result);

        }

        [Theory]
        [InlineData(1)]
        public async void GetProduct_IdIsValid_ReturnOkResultWithProduct(int productId)
        {
            //Act
            var product = _products.First(x => x.Id == productId);
            _mockRepo.Setup(x => x.GetById(productId)).ReturnsAsync(product);
            var result = await _controller.GetProduct(productId);

            //Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnProduct = Assert.IsType<Product>(okResult.Value);
            Assert.Equal(productId, returnProduct.Id);
        }

        [Theory]
        [InlineData(1)]
        public void PutProduct_IdIsNotEqualProduct_ReturnBadRequestResult(int productId)
        {
            var product = _products.First(x => x.Id == productId);
            var result = _controller.PutProduct(2, product);
            Assert.IsType<BadRequestResult>(result);
        }

        [Theory]
        [InlineData(1)]
        public  void PutProduct_ActionExecutes_ReturnNoContent(int productId)
        {
            var product = _products.First(x => x.Id == productId);
            _mockRepo.Setup(x => x.Update(product));

            var result = _controller.PutProduct(productId, product);
            _mockRepo.Verify(x =>x.Update(product),Times.Once);
              Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async void PostProduct_ActionExecutes_ReturnCreateAction()
        {
            var product = _products.First();
            _mockRepo.Setup(x => x.Create(product)).Returns(Task.CompletedTask);

            var result = await _controller.PostProduct(product);
            var createActionResult = Assert.IsType<CreatedAtActionResult>(result);
            _mockRepo.Verify(x =>x.Create(product),Times.Once);

            Assert.Equal("GetProduct",createActionResult.ActionName);
        }

        [Theory]
        [InlineData(0)]
        public async void DeleteProduct_IdInValid_ReturnNotFound(int productId)
        {
            _mockRepo.Setup(x => x.GetById(productId)).ReturnsAsync((Product) null);

            var resultNorFound = await _controller.DeleteProduct(productId);

            Assert.IsType<NotFoundResult>(resultNorFound.Result);
        }

        [Theory]
        [InlineData(1)]
        public async void DeleteProduct_ActionExecute_ReturnNoContent(int productId)
        {
            var product = _products.First(x => x.Id == productId);
            _mockRepo.Setup(x => x.GetById(productId)).ReturnsAsync(product);
            _mockRepo.Setup(x => x.Delete(product));

            var noContentResult =await _controller.DeleteProduct(productId);

            _mockRepo.Verify(x =>x.Delete(product),Times.Once);

            Assert.IsType<NoContentResult>(noContentResult.Result);
        }
    }
}
