using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project_Ecommerce.Models.ViewModels
{
    public class ProductVM
    {
        public Product Product { get; set; }
       //public IEnumerable<Category> CategoryList { get; set; }  
        //public IEnumerable<CoverType> CoverTypeList { get; set; }

        public IEnumerable<SelectListItem>CategoryList { get; set; }
        public IEnumerable<SelectListItem>CoverTypeList { get; set; }
    }
}
