using Ecommerce.DataAccess.Repository.IRepository;
using Ecommerce.Models.Models;
using Ecommerce.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace E_Commerce_Web_App.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ProductController : Controller
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IWebHostEnvironment webHostEnvironment;

        public ProductController(IUnitOfWork unitOfWork, IWebHostEnvironment webHostEnvironment)
        {
            this.unitOfWork = unitOfWork;
            this.webHostEnvironment = webHostEnvironment;
        }

        public IActionResult Index()
        {
            List<Product> productList = unitOfWork.Product.GetAll(includeProperties:"Category").ToList();
            return View(productList);
        }

        public IActionResult Upsert(int? id)
        {
            ProductVM productVM = new()
            {
                CategoryList = unitOfWork.Category
                .GetAll().Select(u => new SelectListItem
                {
                    Text = u.Name,
                    Value = u.CategoryId.ToString(),
                }),
                Product = new Product()
            };
            if(id==null || id == 0)
            {
                return View(productVM);
            }
            else
            {
                productVM.Product = unitOfWork.Product.Get(m => m.ProductId == id);
                return View(productVM);
            }
        }

        [HttpPost]
        public IActionResult Upsert(ProductVM obj, IFormFile? file)
        {
            if (ModelState.IsValid)
            {
                string wwwRootPath = webHostEnvironment.WebRootPath;
                if(file!=null)
                {
                    string fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                    string productPath = Path.Combine(wwwRootPath, @"images/product");

                    if(!string.IsNullOrEmpty(obj.Product.ImageUrl))
                    {
                        var oldPath = Path.Combine(wwwRootPath, obj.Product.ImageUrl.TrimStart('\\'));
                        if(System.IO.File.Exists(oldPath))
                        {
                            System.IO.File.Delete(oldPath);
                        }
                    }

                    using (var fileStream = new FileStream(Path.Combine(productPath, fileName), FileMode.Create))
                    {
                        file.CopyTo(fileStream);
                    }
                    obj.Product.ImageUrl = @"\images\product\" + fileName;
                }
                if (obj.Product.ProductId == 0)
                {
                    unitOfWork.Product.Add(obj.Product);
                    TempData["success"] = "Product Created Successfully";
                }
                else
                {
                    unitOfWork.Product.Update(obj.Product);
                    TempData["success"] = "Product Updated Successfully";
                }
                    
                unitOfWork.Save();
                return RedirectToAction("Index");
            }
            else
            {
                obj.CategoryList = unitOfWork.Category
                .GetAll().Select(u => new SelectListItem
                {
                    Text = u.Name,
                    Value = u.CategoryId.ToString(),
                });
                return View(obj);
            }

        }

        #region API CALLS

        [HttpGet]
        public IActionResult GetAll()
        {
            List<Product> productList = unitOfWork.Product.GetAll(includeProperties: "Category").ToList();
            return Json(new { data = productList });
        }

        [HttpDelete]
        public IActionResult Delete(int? id)
        {
            var productDelete = unitOfWork.Product.Get(p => p.ProductId == id);
            if(productDelete == null)
            {
                return Json(new { success = false, message = "Error while deleting" });
            }
            
            if(!string.IsNullOrEmpty(productDelete.ImageUrl)) 
            {
                var oldPath = Path.Combine(webHostEnvironment.WebRootPath, productDelete.ImageUrl.TrimStart('\\'));
                if (System.IO.File.Exists(oldPath))
                {
                    System.IO.File.Delete(oldPath);
                }
            }

            unitOfWork.Product.Remove(productDelete); 
            unitOfWork.Save();

            return Json(new { succes = true, message = "Delete Successful" });
        }

        #endregion
    }
}
