using Ecommerce.DataAccess.Repository.IRepository;
using Ecommerce.Models.Models;
using Ecommerce.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace E_Commerce_Web_App.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.Role_Admin)]
    public class CompanyController : Controller
    {
        private readonly IUnitOfWork unitOfWork;

        public CompanyController(IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }

        public IActionResult Index()
        {
            List<Company> companyList = unitOfWork.Company.GetAll().ToList();
            return View(companyList);
        }

        public IActionResult Upsert(int? id)
        {
            
            if(id==null || id == 0)
            {
                return View(new Company());
            }
            else
            {
                Company company = unitOfWork.Company.Get(m => m.CompanyId == id);
                return View(company);
            }
        }

        [HttpPost]
        public IActionResult Upsert(Company obj)
        {
            if (ModelState.IsValid)
            {
                if (obj.CompanyId == 0)
                {
                    unitOfWork.Company.Add(obj);
                    TempData["success"] = "Company Created Successfully";
                }
                else
                {
                    unitOfWork.Company.Update(obj);
                    TempData["success"] = "Company Updated Successfully";
                }
                    
                unitOfWork.Save();
                return RedirectToAction("Index");
            }
            else
            {
                return View(obj);
            }

        }

        #region API CALLS

        [HttpGet]
        public IActionResult GetAll()
        {
            List<Company> companyList = unitOfWork.Company.GetAll().ToList();
            return Json(new { data = companyList });
        }

        [HttpDelete]
        public IActionResult Delete(int? id)
        {
            var companyDelete = unitOfWork.Company.Get(c => c.CompanyId == id);

            if(companyDelete == null)
            {
                return Json(new { success = false, message = "Error while deleting" });
            }
            
            unitOfWork.Company.Remove(companyDelete); 
            unitOfWork.Save();

            return Json(new { succes = true, message = "Delete Successful" });
        }

        #endregion
    }
}
