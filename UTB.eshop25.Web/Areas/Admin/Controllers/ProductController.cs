using Microsoft.AspNetCore.Mvc;
using UTB.eshop25.Application.Abstraction;
using UTB.eshop25.Domain.Entities;

namespace UTB.eshop25.Web.Areas.Admin.Controllers
{
    [Area(nameof(Admin))]
    public class ProductController : Controller
    {
        IProductAppService _productAppService;
        public ProductController(IProductAppService productAppService)
        {
            _productAppService = productAppService;
        }

        public IActionResult Select()
        {
            var products = _productAppService.SelectAll();
            return View(products);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(Product product)
        {
            if (ModelState.IsValid)
            {
                _productAppService.Create(product);
                return RedirectToAction(nameof(Select));
            }
            return View(product);
        }

        public IActionResult Delete(int id)
        {
            bool deleted = _productAppService.Delete(id);
            if (deleted)
            {
                return RedirectToAction(nameof(Select));
            }
            return NotFound();
        }
    }
}
