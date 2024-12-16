using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Project_Ecommerce.Data_Access.Repository.IRepository;
using Project_Ecommerce.Models;
using Project_Ecommerce.Utility;

namespace Project_Ecommerce.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles =SD.Role_Admin +","+ SD.Role_Employee)]
    public class CompanyController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        public CompanyController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
    
        public IActionResult Index()
        {
            return View();
        }
        #region APIs
        [HttpGet]
        public IActionResult GetAll()
        {
            return Json(new { data = _unitOfWork.Company.GetAll() });
        }
        [HttpDelete]
        public IActionResult Delete(int id)
        {
            var companyindb =  _unitOfWork.Company.Get(id);
            if (companyindb == null)
                return Json(new { success = false, message = "Something Went Wrong !!!" });
            _unitOfWork.Company.Remove(companyindb);
            _unitOfWork.Save();
            return Json(new { success = true, message = "Deleted successfully" });

        }
        #endregion

        public IActionResult Upsert(int?id)
        {
            Company company = new Company();

            if(id==null) return View(company);
            company = _unitOfWork.Company.Get(id.GetValueOrDefault());
            if(company ==null) return NotFound();
            return View(company);
        }
        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public IActionResult Upsert(Company company) 
        {
            if(company ==null) return NotFound();  
            if(!ModelState.IsValid) return View(company);

            if (company.Id == 0)
                _unitOfWork.Company.Add(company);
            else 
                _unitOfWork.Company.Update(company);
            _unitOfWork.Save();
            return RedirectToAction("Index");
        }
    }
}
