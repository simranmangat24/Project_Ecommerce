using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using NuGet.Versioning;
using Project_Ecommerce.Data_Access.Repository;
using Project_Ecommerce.Data_Access.Repository.IRepository;
using Project_Ecommerce.Models;
using Project_Ecommerce.Models.ViewModels;
using Project_Ecommerce.Utility;
using Stripe;
using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Text;
using Twilio;
using Twilio.Rest.Api.V2010.Account;
using Twilio.Types;
using Microsoft.Extensions.Options;

namespace Project_Ecommerce.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class CartController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private static bool IsEmaiilConfirmed = false;
        private readonly IEmailSender _emailSender;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly TwilioSettings _twilioSettings;
        public CartController(IUnitOfWork unitOfWork, IEmailSender emailSender, UserManager<IdentityUser> userManager, IOptions<TwilioSettings> twilioSettings)
        {
            _unitOfWork = unitOfWork;
            _emailSender = emailSender;
            _userManager = userManager;
            _twilioSettings = twilioSettings.Value;
        }
        [BindProperty]
        public ShoppingCartVM ShoppingCartVM { get; set; }
        public IActionResult Index()
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
            //EmailConfirm
            if (!IsEmaiilConfirmed)
            {
                ViewBag.EmailMessage = "Email has been sent to registered email, Please verify your Email";
                ViewBag.EmailCSS = "text-success";
                IsEmaiilConfirmed = false;
            }
            else
            {
                ViewBag.EmailMessage = "Email must be verified for authorised users ";
                ViewBag.EmailCSS = "text-danger";
            }

            return View(ShoppingCartVM);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("Index")]

        public async Task<IActionResult> IndexPost()
        {
            var claimIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimIdentity.FindFirst(ClaimTypes.NameIdentifier);
            var user = _unitOfWork.ApplicationUsers.FirstOrDefault(au => au.Id == claim.Value);
            if (user == null)
            {
                ModelState.AddModelError(string.Empty, "Email is Empty");
            }
            else
            {
                var userId = await _userManager.GetUserIdAsync(user);
                var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
                var callbackUrl = Url.Page(
                    "/Account/ConfirmEmail",
                    pageHandler: null,
                    values: new { area = "Identity", userId = userId, code = code },
                    protocol: Request.Scheme);

                await _emailSender.SendEmailAsync(user.Email, "Confirm your email",
                    $"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");
            }
            return RedirectToAction("Index");
        }



        public IActionResult Plus(int id)
        {
            var productinCart = _unitOfWork.ShoppingCart.Get(id);
            productinCart.Count += 1;
            _unitOfWork.Save();
            return RedirectToAction("Index");
        }

        public IActionResult Minus(int id)
        {
            var productinCart = _unitOfWork.ShoppingCart.Get(id);
            if (productinCart.Count == 1)
            {
                productinCart.Count = 1;
            }
            else
                productinCart.Count -= 1;
            _unitOfWork.Save();
            return RedirectToAction("Index");
        }

        public IActionResult Delete(int id)
        {
            var productinCart = _unitOfWork.ShoppingCart.Get(id);
            _unitOfWork.ShoppingCart.Remove(productinCart);
            _unitOfWork.Save();

            var claimIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimIdentity.FindFirst(ClaimTypes.NameIdentifier);

            if (claim != null)
            {
                var count = _unitOfWork.ShoppingCart.GetAll(sc => sc.ApplicationUserId == claim.Value).ToList().Count;
                HttpContext.Session.SetInt32(SD.Ss_CartSessionCount, count);
            }
            return RedirectToAction("Index");
        }

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
                OrderHeader = new OrderHeader(),

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
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("Summary")]
        public async Task<IActionResult> SummaryPost(string stripeToken)
        {
            var claimIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimIdentity.FindFirst(ClaimTypes.NameIdentifier);

            ShoppingCartVM.OrderHeader.ApplicationUser =
                _unitOfWork.ApplicationUsers.FirstOrDefault(au => au.Id == claim.Value);

            ShoppingCartVM.ListCart = _unitOfWork.ShoppingCart.GetAll
                (sc => sc.ApplicationUserId == claim.Value, includeProperties: "Product");

            ShoppingCartVM.OrderHeader.OrderStatus = SD.OrderStatusPending;
            ShoppingCartVM.OrderHeader.PaymentStatus = SD.PaymentStatusPending;
            ShoppingCartVM.OrderHeader.OrderDate = DateTime.Now;
            ShoppingCartVM.OrderHeader.ApplicationUserId = claim.Value;
            _unitOfWork.OrderHeader.Add(ShoppingCartVM.OrderHeader);
            _unitOfWork.Save();


            foreach (var list in ShoppingCartVM.ListCart)
            {
                list.Price = SD.GetPriceBasedOnQuantity(list.Count, list.Product.Price, list.Product.Price50, list.Product.Price100);
                OrderDetails orderDetails = new OrderDetails()
                {
                    ProductId = list.ProductId,
                    OrderHeaderId = ShoppingCartVM.OrderHeader.Id,
                    Price = list.Price,
                    Count = list.Count

                };
                ShoppingCartVM.OrderHeader.OrderTotal += (list.Price * list.Count);
                _unitOfWork.OrderDetails.Add(orderDetails);
                _unitOfWork.Save();
            }
            _unitOfWork.ShoppingCart.RemoveRange(ShoppingCartVM.ListCart);
            _unitOfWork.Save();

            //Session
            HttpContext.Session.SetInt32(SD.Ss_CartSessionCount, 0);


            #region Stripe
            if (stripeToken == null)
            {                             //InCAse Of Company User
                ShoppingCartVM.OrderHeader.PaymentDueDate = DateTime.Today.AddDays(30);            //Add DAte of paymnet
                ShoppingCartVM.OrderHeader.PaymentStatus = SD.PaymentStatusDelayedPayment;        //Payment Status
                ShoppingCartVM.OrderHeader.OrderStatus = SD.OrderStatusApproved;                  // Order Approved
            }                          //InCAse of Individual User
            else
            {
                //Payment Process
                var options = new ChargeCreateOptions()
                {
                    Amount = Convert.ToInt32(ShoppingCartVM.OrderHeader.OrderTotal),     //Find amount
                    Currency = "usd",
                    Description = "Order Id :" + ShoppingCartVM.OrderHeader.Id,
                    Source = stripeToken
                };
                //Payment 
                var service = new ChargeService();
                Charge charge = service.Create(options);
                if (charge.BalanceTransactionId == null)
                    ShoppingCartVM.OrderHeader.PaymentStatus = SD.PaymentStatusRejected;
                else
                    ShoppingCartVM.OrderHeader.TransactionId = charge.BalanceTransactionId;         //Find Transaction Id
                if (charge.Status.ToLower() == "succeeded")
                {
                    ShoppingCartVM.OrderHeader.PaymentStatus = SD.PaymentStatusApproved;
                    ShoppingCartVM.OrderHeader.OrderStatus = SD.OrderStatusApproved;
                    ShoppingCartVM.OrderHeader.OrderDate = DateTime.Now;

                    //**


                    //Order Success Email
                    var callbackUrl = Url.Action(
                        "OrderConfirmation",
                        "Cart",
                        new { orderId = ShoppingCartVM.OrderHeader.Id },
                        protocol: Request.Scheme);

                    await _emailSender.SendEmailAsync(ShoppingCartVM.OrderHeader.Name, "Order Confirmation",
                        $"Thankyou for your Order! Your Order ID {ShoppingCartVM.OrderHeader.Id} has been placed successfully." +
                        $"Your Order Will be dispatched soon, And may be deliverable by next few working days. "+
                        $" You can View Your Order by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>  "
                        );
                    //**

                    //** SMS Message For Order Success

                    TwilioClient.Init(_twilioSettings.AccountSid, _twilioSettings.AuthToken);

                    var message = MessageResource.Create(
                        body: $"Thankyou for your Order! Your Order ID {ShoppingCartVM.OrderHeader.Id} has been placed successfully!",
                        from: new PhoneNumber(_twilioSettings.PhoneNumber),
                        to: new PhoneNumber("+91" + ShoppingCartVM.OrderHeader.PhoneNumber)
                        ); 
                    //**
                }
                else
                {
                    ShoppingCartVM.OrderHeader.PaymentStatus = SD.PaymentStatusRejected;
                }
                _unitOfWork.Save();

            }


            #endregion


            return RedirectToAction("OrderConfirmation", "Cart", new { id = ShoppingCartVM.OrderHeader.Id });
        }


        public IActionResult OrderConfirmation(int id)
        {
            return View(id);
        }
    }
}