using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project_Ecommerce.Data_Access.Repository.IRepository
{
    public interface IUnitOfWork
    {
        ICategoryRepository Category { get; }
        ICoverTypeRepository CoverType { get; }
        ISP_CALL SP_CALL { get; }
        IProductRepository Product { get; }
        ISP_Category SP_Category { get; }
        ICompanyRepository Company { get; } 
        IShoppingCartRepository ShoppingCart { get; }
        IApplicationUsers ApplicationUsers { get; }
        IOrderHeaderRepository OrderHeader { get; }
        IOrderDetailsRepository OrderDetails { get; }
        void Save();
    }
}
