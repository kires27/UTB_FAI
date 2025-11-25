using Microsoft.AspNetCore.Mvc;
using Moq;
using UTB.eshop25.Application.Abstraction;
using UTB.eshop25.Domain.Entities;
using UTB.eshop25.Web.Areas.Admin.Controllers;

namespace UTB.eshop25.Web.Tests
{
    public class ProductControllerTests
    {
        [Fact]
        public void CreateTestPositive()
        {
            //priprava vstupu, vytvareni instanci
            Mock<IProductAppService> mockProd = new Mock<IProductAppService>();
            mockProd.Setup(productAppService => productAppService.Create(It.IsAny<Product>())).Callback(() => { return; });

            ProductController productController = new ProductController(mockProd.Object);

            //spusteni metody
            var result = productController.Create(null);

            //assert - kontrola vystupu
            Assert.IsType<RedirectToActionResult>(result);
        }

        [Fact]
        public void CreateTestNegative()
        {
            //priprava vstupu, vytvareni instanci
            Mock<IProductAppService> mockProd = new Mock<IProductAppService>();
            mockProd.Setup(productAppService => productAppService.Create(It.IsAny<Product>())).Callback(() => { return; });

            ProductController productController = new ProductController(mockProd.Object);

            productController.ModelState.AddModelError(nameof(Product.Name), $"The {nameof(Product.Name)} field is required.");

            //spusteni metody
            var result = productController.Create(null);

            //assert - kontrola vystupu
            Assert.IsType<ViewResult>(result);
        }
    }
}
