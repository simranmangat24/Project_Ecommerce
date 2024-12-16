using Project_Ecommerce.Data_Access.Data;
using Project_Ecommerce.Data_Access.Repository;
using Project_Ecommerce.Data_Access.Repository.IRepository;
using Project_Ecommerce.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project_Ecommerce.Data_Access.Repository
{
    public class ProductRepository : Repository<Product>, IProductRepository
    {
        private readonly ApplicationDbContext _context;
        public ProductRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }
    }
}

