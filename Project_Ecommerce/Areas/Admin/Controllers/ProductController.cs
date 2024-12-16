using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Project_Ecommerce.Data_Access.Repository.IRepository;
using Project_Ecommerce.Models;
using Project_Ecommerce.Models.ViewModels;
using Project_Ecommerce.Utility;
using System.Security.Cryptography.X509Certificates;
using System.Security.Cryptography.Xml;

namespace Project_Ecommerce.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles =SD.Role_Admin)]
    public class ProductController : Controller
    {
        private readonly IUnitOfWork _unitofwork;
        private readonly IWebHostEnvironment _webHostEnvironment;
        public ProductController(IUnitOfWork unitofwork, IWebHostEnvironment webHostEnvironment)
        {
            _unitofwork = unitofwork;
            _webHostEnvironment = webHostEnvironment;
        }

        public IActionResult Index()
        {
            return View();
        }
        #region APIs
        [HttpGet]
        public IActionResult GetAll()
        {
            var productlist = _unitofwork.Product.GetAll(includeProperties: "Category,CoverType");
            return Json(new { data = productlist });

        }
        [HttpDelete]
        public IActionResult Delete(int id)
        {
            var prodInDb = _unitofwork.Product.Get(id);
            if (prodInDb == null)
                return Json(new { success = false, message = "Something Went Wrong !!!" });
            //
            var webRootPath = _webHostEnvironment.WebRootPath;
            var imagePath = Path.Combine(webRootPath, prodInDb.ImageUrl.Trim('\\'));
            if(System.IO.File.Exists(imagePath))
            {
                System.IO.File.Delete(imagePath);
            }
            //

            _unitofwork.Product.Remove(prodInDb);
            _unitofwork.Save();
            return Json(new { success = true, message = "Product Deleted Successfully" });
        }
        #endregion
        public IActionResult Upsert(int? id)
        {
            ProductVM productVM = new ProductVM()
            {
                Product = new Product(),
                CategoryList = _unitofwork.Category.GetAll().Select(cl => new SelectListItem()
                {
                    Text = cl.Name,
                    Value = cl.Id.ToString()
                }),
                CoverTypeList = _unitofwork.CoverType.GetAll().Select(ctl => new SelectListItem()
                {
                    Text = ctl.Name,
                    Value = ctl.Id.ToString()
                })
            };
            if (id == null) return View(productVM);
            productVM.Product = _unitofwork.Product.Get(id.GetValueOrDefault());
            return View(productVM);
        }
        //Upsert Image And Details
        [HttpPost]
        public IActionResult Upsert(ProductVM productVM)
        {
            if (ModelState.IsValid)                 
            { 
                var webRootPath = _webHostEnvironment.WebRootPath;                     //Image Path
                var files = HttpContext.Request.Form.Files;                            //Request from "FORM" Image File tag
                if (files.Count() > 0)                                                 //If NO Image
                {
                    var fileName = Guid.NewGuid().ToString();                          //Grnerate Random ID
                    var extension = Path.GetExtension(files[0].FileName);              //Add Extension of file uploaded
                    var uploads = Path.Combine(webRootPath, @"images/products");       //Save Into 
                       if (productVM.Product.Id != 0)
                    {                   //For Image EDIT                                                                            
                        var imageExists = _unitofwork.Product.Get(productVM.Product.Id).ImageUrl;         //URL       
                        productVM.Product.ImageUrl = imageExists;
                    }
                    if (productVM.Product.ImageUrl != null)
                    {             //Image Update (Delete Previous Uploaded image)                         
                        var imagePath = Path.Combine(webRootPath, productVM.Product.ImageUrl.Trim('\\'));   //Get Path
                        if (System.IO.File.Exists(imagePath))          
                        {                                             
                            System.IO.File.Delete(imagePath);                                               //Delete Previous Uploaded File and upload new
                        }
                    }
                    using (var filesStream = new FileStream(Path.Combine(uploads, fileName + extension), FileMode.Create))
                    {                                                                                    //Add File
                        files[0].CopyTo(filesStream);
                    }
                    productVM.Product.ImageUrl = @"\images\products\" + fileName + extension;
                }
                else
                {
                    if (productVM.Product.Id != 0)
                    {
                        var imageExists = _unitofwork.Product.Get(productVM.Product.Id).ImageUrl;
                        productVM.Product.ImageUrl = imageExists;
                    }
                }
                if (productVM.Product.Id == 0)
                {
                    _unitofwork.Product.Add(productVM.Product);
                }
                else
                {
                    _unitofwork.Product.Update(productVM.Product);
                }
                _unitofwork.Save();
                return RedirectToAction("Index");
            }
            else
            {
                productVM = new ProductVM()
                {

                    Product = new Product(),
                    CategoryList = _unitofwork.Category.GetAll().Select(cl => new SelectListItem()
                    {
                        Text = cl.Name,
                        Value = cl.Id.ToString()
                    }),
                    CoverTypeList = _unitofwork.CoverType.GetAll().Select(ctl => new SelectListItem()
                    {
                        Text = ctl.Name,
                        Value = ctl.Id.ToString()
                    })
                };
                if(productVM.Product.Id !=0)
                {
                    productVM.Product = _unitofwork.Product.Get(productVM.Product.Id);
                }
                return View(productVM);

            }
        }
        //Upsert Image And Details

    }
}
