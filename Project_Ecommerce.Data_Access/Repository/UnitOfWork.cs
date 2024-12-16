using Microsoft.EntityFrameworkCore.Metadata;
using Project_Ecommerce.Data_Access.Data;
using Project_Ecommerce.Data_Access.Repository.IRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project_Ecommerce.Data_Access.Repository
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _context;
        public UnitOfWork(ApplicationDbContext context)
        {
            _context = context;
            Category = new CateogryRepository(_context);
            CoverType = new CoverTypeRepository(_context);
            SP_CALL = new SP_CALL(_context);
            Product = new ProductRepository(_context);
            SP_Category = new SP_Category(_context);
            Company = new CompanyRepository(_context);
            ApplicationUsers = new ApplicationUserRepository(_context);
            ShoppingCart= new ShoppingCartRepository(_context);
            OrderHeader= new OrderHeaderRepository(_context);
            OrderDetails= new OrderDetailsRepository(_context); 
        }

        public ICategoryRepository Category { get; private set; }
        public ICoverTypeRepository CoverType { get; private set; }
        public ISP_CALL SP_CALL { get; private set; }
        public IProductRepository Product { get; private set; }
        public ISP_Category SP_Category { get; private set; }   
        public ICompanyRepository Company { get; private set; }
        public IShoppingCartRepository ShoppingCart { get; private set; }
        public IApplicationUsers ApplicationUsers { get; private set; }
        public IOrderHeaderRepository OrderHeader { get; private set; }
        public IOrderDetailsRepository OrderDetails { get; private set; }   
        public void Save()
        {
            _context.SaveChanges();
        }
    }
}
