using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Project_Ecommerce.Data_Access.Data;
using Project_Ecommerce.Data_Access.Repository.IRepository;
using Project_Ecommerce.Models.ViewModels;
using Project_Ecommerce.Models;
using Project_Ecommerce.Utility;
using System.Security.Claims;

namespace Project_Ecommerce.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize]
    public class OrderController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ApplicationDbContext _context;
        public OrderController(IUnitOfWork unitOfWork,ApplicationDbContext context)
        {
                    _unitOfWork = unitOfWork;
                   _context = context;
        }
        [BindProperty]
        public ShoppingCartVM ShoppingCartVM { get; set; }
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult PendingOrders()
        {
            return View();
        }

        public IActionResult ApprovedOrders()
        {
            return View();
        }

        #region Api
        [HttpGet]
        public IActionResult GetAll() 
        {
            var orderList = _unitOfWork.OrderHeader.GetAll();
            return Json(new {data= orderList});
        }
        [HttpGet]
        public IActionResult GetPendingOrders()
        {
            var pendingOrders = _context.OrderHeaders.Where(po => po.OrderStatus == "Pending").ToList();
            return Json(new {data=pendingOrders});
        }

        [HttpGet]
        public IActionResult GetApprovedOrders()
        {
            
            var approvedOrders = _context.OrderHeaders.Where(po => po.OrderStatus == "Approved").ToList();
            return Json(new { data = approvedOrders });
        }
        #endregion
        public IActionResult Summary()
        {
            var claimIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimIdentity.FindFirst(ClaimTypes.NameIdentifier);

            if (claim == null)
            {
                ShoppingCartVM = new ShoppingCartVM()
                {
                    ListCart = new List<ShoppingCart>()
                };
                return View(ShoppingCartVM);
            }
            ShoppingCartVM = new ShoppingCartVM()
            {
                ListCart = _unitOfWork.ShoppingCart.GetAll(sc => sc.ApplicationUserId == claim.Value, includeProperties: "Product"),
                OrderHeader = new OrderHeader()
            };

            ShoppingCartVM.OrderHeader.OrderTotal = 0;
            ShoppingCartVM.OrderHeader.ApplicationUser = _unitOfWork.ApplicationUsers.FirstOrDefault(au => au.Id == claim.Value);

            foreach (var list in ShoppingCartVM.ListCart)
            {
                list.Price = SD.GetPriceBasedOnQuantity(list.Count, list.Product.Price, list.Product.Price50, list.Product.Price100);
                ShoppingCartVM.OrderHeader.OrderTotal += (list.Price * list.Count);
                if (list.Product.Description.Length > 100)
                {
                    list.Product.Description = list.Product.Description.Substring(0, 100) + "...";
                }
            }

            ShoppingCartVM.OrderHeader.Name = ShoppingCartVM.OrderHeader.ApplicationUser.Name;
            ShoppingCartVM.OrderHeader.StreetAddress = ShoppingCartVM.OrderHeader.ApplicationUser.StreetAddress;
            ShoppingCartVM.OrderHeader.City = ShoppingCartVM.OrderHeader.ApplicationUser.City;
            ShoppingCartVM.OrderHeader.State = ShoppingCartVM.OrderHeader.ApplicationUser.State;
            ShoppingCartVM.OrderHeader.PhoneNumber = ShoppingCartVM.OrderHeader.ApplicationUser.PhoneNumber;
            ShoppingCartVM.OrderHeader.PostalCode = ShoppingCartVM.OrderHeader.ApplicationUser.PostalCode;
            return View(ShoppingCartVM);
        }



    }
}
