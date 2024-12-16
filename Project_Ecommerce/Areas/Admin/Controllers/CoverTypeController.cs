using Dapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Project_Ecommerce.Data_Access.Repository.IRepository;
using Project_Ecommerce.Models;
using Project_Ecommerce.Utility;

namespace Project_Ecommerce.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles =SD.Role_Admin)]
    public class CoverTypeController : Controller
    {
        private readonly IUnitOfWork _unitofwork;
        public CoverTypeController(IUnitOfWork unitofwork)
        {
            _unitofwork = unitofwork;                     //Dependency Injection
        }

        public IActionResult Index()
        {
            return View();
        }
        #region APIs
        [HttpGet]
        public IActionResult GetAll()
        {
            //var coverTypeList = _unitofwork.CoverType.GetAll();
            //return Json(new { data = coverTypeList });
            return Json(new { data = _unitofwork.SP_CALL.List<CoverType>(SD.Proc_GetCoverTypes) });
            
        }
        [HttpDelete]
        public IActionResult Delete(int id)
        {
            var coverTypeInDb = _unitofwork.CoverType.Get(id);
            if (coverTypeInDb == null)
                return Json(new { success = false, message = "Something Went Wrong" });
            //**
            DynamicParameters param=    new DynamicParameters();
            param.Add("id", id);
            _unitofwork.SP_CALL.Execute(SD.Proc_DeleteCoverType, param);
            //**
            //_unitofwork.CoverType.Remove(coverTypeInDb);
            //_unitofwork.Save();
            return Json(new { success = true, message = "Data Deleted Successfully" });
        }


        #endregion
        public IActionResult Upsert(int? id)
        {
            CoverType coverType = new CoverType();
            if (id == null) return View(coverType); //Save

            //**
            DynamicParameters param = new DynamicParameters();
            param.Add("id",id.GetValueOrDefault()); 
            coverType=_unitofwork.SP_CALL.OneRecord<CoverType>
                (SD.Proc_GetCoverType, param);  

            //**
            //coverType = _unitofwork.CoverType.Get(id.GetValueOrDefault());         //Create
            if (coverType == null) return NotFound();
            return View(coverType);
        }
        [HttpPost]
        public IActionResult Upsert(CoverType coverType)
        {
            if (coverType == null) return NotFound();
            if (!ModelState.IsValid) return View(coverType);
            //**
            DynamicParameters param = new DynamicParameters();
            param.Add("name",coverType.Name);
            //**
            if (coverType.Id == 0)
                //_unitofwork.CoverType.Add(coverType);
                _unitofwork.SP_CALL.Execute(SD.Proc_CreateCoverType, param);
            else
            {
                //_unitofwork.CoverType.Update(coverType);
                param.Add("id",coverType.Id);
                _unitofwork.SP_CALL.Execute(SD.Proc_UpdateCoverType, param);
            }
            //_unitofwork.Save();
            return RedirectToAction("Index");
        }
    }
}
