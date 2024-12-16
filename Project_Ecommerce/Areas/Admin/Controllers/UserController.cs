using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Project_Ecommerce.Data_Access.Data;
using Project_Ecommerce.Data_Access.Repository.IRepository;
using Project_Ecommerce.Models;
using Project_Ecommerce.Utility;
using System.Data;

namespace Project_Ecommerce.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.Role_Admin + "," + SD.Role_Employee)]
    public class UserController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ApplicationDbContext _context;
        public UserController(IUnitOfWork unitOfWork,ApplicationDbContext context)
        {
                _unitOfWork= unitOfWork;
            _context= context;
        }
        public IActionResult Index()
        {
            return View();
        }
        #region APIs
        [HttpGet]
        public IActionResult GetAll()
        {
            var userList = _context.ApplicationUsers.ToList(); //AspNetUser        (All Registered Users into List)
            var roles  = _context.Roles.ToList();              // AspNetRoles      (All  User Roles into List)
            var userRoles = _context.UserRoles.ToList();       //AspNetUserRoles   (All Registered Users into List)
            foreach (var  user in userList)                                //      (ALL Users from UserList)    
            {
                var roleId = userRoles.FirstOrDefault(u => u.UserId == user.Id).RoleId;     // (From UserRoles get Role ID With matching userID)
                user.Role = roles.FirstOrDefault(r => r.Id == roleId).Name;                // (From UserRoles get Name With matching roleID)
                if (user.CompanyId != null)
                {
                    user.Company = new Company()
                    {
                        Name = _unitOfWork.Company.Get(Convert.ToInt32(user.CompanyId)).Name                //Get Company Name
                    };
                }
            if (user.CompanyId == null)
                {
                    user.Company = new Company()
                    {
                        Name = ""
                    };
                }
            }
        var adminUser = userList.FirstOrDefault(u=>u.Role == SD.Role_Admin);        //Get Admin Role from UserList 
            userList.Remove(adminUser);                                                //Remove Admin Role from UserList 
            return Json(new { data = userList });

        }
        [HttpPost]
        public IActionResult LockUnlock([FromBody]string id)
        {
            bool isLocked = false;
            var userIndb = _context.ApplicationUsers.FirstOrDefault(au=>au.Id== id);
            if (userIndb == null)
                return Json(new { success = false, message = "Something Went Wrong !!!" });
            if(userIndb !=null && userIndb.LockoutEnd>DateTime.Now)
            {
                userIndb.LockoutEnd= DateTime.Now;
                isLocked = false;
            }
            else
            {
                userIndb.LockoutEnd = DateTime.Today.AddYears(20);
                isLocked= true;
            }
            _context.SaveChanges();
            return Json(new { success = true, message = isLocked == true ? "User Locked Successfully" : "User Unlocked successfully" });
        }

        #endregion
    }
}
