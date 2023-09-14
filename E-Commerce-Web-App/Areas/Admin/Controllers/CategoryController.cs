using Ecommerce.Models.Models;
using Ecommerce.DataAccess.Data;
using Microsoft.AspNetCore.Mvc;
using Ecommerce.DataAccess.Repository.IRepository;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace E_Commerce_Web_App.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class CategoryController : Controller
    {
        private readonly IUnitOfWork unitOfWork;

        public CategoryController(IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }

        public IActionResult Index()
        {
            List<Category> categoryList = unitOfWork.Category.GetAll().ToList();
            return View(categoryList);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(Category category)
        {
            if (category.Name == category.DisplayOrder.ToString())
                ModelState.AddModelError("name", "Display Order and Name cannot be same.");
            if (ModelState.IsValid)
            {
                unitOfWork.Category.Add(category);
                unitOfWork.Save();
                TempData["success"] = "Category Created Successfully";
                return RedirectToAction("Index");
            }

            return View();
        }

        public IActionResult Edit(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }
            Category category = unitOfWork.Category.Get(m => m.CategoryId == id);
            if (category == null)
            {
                return NotFound();
            }
            return View(category);
        }

        [HttpPost]
        public IActionResult Edit(Category category)
        {
            if (ModelState.IsValid)
            {
                unitOfWork.Category.Update(category);
                unitOfWork.Save();
                TempData["success"] = "Category updated successfully";
                return RedirectToAction("Index");
            }

            return View();
        }

        public IActionResult Delete(int? id)
        {
            Category category = unitOfWork.Category.Get(o => o.CategoryId == id);
            if (category == null)
            {
                return NotFound();
            }
            return View(category);
        }

        [HttpPost, ActionName("Delete")]
        public IActionResult DeletePost(int? id)
        {
            Category c = unitOfWork.Category.Get(o => o.CategoryId == id);
            if (c == null)
            {
                return NotFound();
            }
            unitOfWork.Category.Remove(c);
            unitOfWork.Save();
            TempData["success"] = "Category deleted successfully";
            return RedirectToAction("Index");
        }
    }
}
