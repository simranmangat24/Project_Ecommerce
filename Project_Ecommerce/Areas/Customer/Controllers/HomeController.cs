using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Project_Ecommerce.Data_Access.Repository.IRepository;
using Project_Ecommerce.Models;
using Project_Ecommerce.Models.ViewModels;
using Project_Ecommerce.Utility;
using System.Diagnostics;
using System.Security.Claims;

namespace Project_Ecommerce.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IUnitOfWork _unitOfWork;

        public HomeController(ILogger<HomeController> logger,IUnitOfWork unitOfWork)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
        }

        public IActionResult Index()
        {
            var claimIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimIdentity.FindFirst(ClaimTypes.NameIdentifier);
            if(claim != null)
            {
                var count = _unitOfWork.ShoppingCart.GetAll
                    (sc=>sc.ApplicationUserId == claim.Value).ToList().Count;
                HttpContext.Session.SetInt32(SD.Ss_CartSessionCount, count);
            }

            var prodList = _unitOfWork.Product.GetAll(includeProperties:"Category,CoverType");
            return View(prodList);
        }

        public IActionResult Privacy()
        {
            return View();
        }
        #region APi
        [HttpGet]
        public IActionResult PrivacyAPI()
        {
            return Json(new { data = _unitOfWork.Product.GetAll(includeProperties: "Category,CoverType") });
        }

        #endregion

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public IActionResult Details(int id)
        {
            var productindb = _unitOfWork.Product.FirstOrDefault(p=>p.Id== id,includeProperties:"Category,CoverType");
            if (productindb == null) return NotFound();
            var shoppingCart = new ShoppingCart()
            {
                Product = productindb,
                ProductId = productindb.Id
            };

            return View(shoppingCart);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public IActionResult Details(ShoppingCart shoppingCart)
        {
            shoppingCart.Id = 0;
            if(ModelState.IsValid)
            {
                var claimIdentity = (ClaimsIdentity)User.Identity;
                var claim = claimIdentity.FindFirst(ClaimTypes.NameIdentifier);
                if (claim == null) return NotFound();

                var shoppingCartInDb = _unitOfWork.ShoppingCart.FirstOrDefault
                    (sc=>sc.ApplicationUserId == claim.Value && sc.ProductId  == shoppingCart.ProductId);
                shoppingCart.ApplicationUserId = claim.Value;

                if (shoppingCartInDb == null)
                    _unitOfWork.ShoppingCart.Add(shoppingCart);
                else
                    shoppingCartInDb.Count += shoppingCart.Count;
                _unitOfWork.Save();

                return RedirectToAction("Index");


            }
            else
            {
                var productindb = _unitOfWork.Product.FirstOrDefault(p => p.Id == shoppingCart.Id, includeProperties: "Category,CoverType");
                if(productindb == null) return NotFound();
                shoppingCart = new ShoppingCart()
                {
                    Product = productindb,
                    ProductId = productindb.Id
                };
                return View(shoppingCart);
            }
        }
    }
}