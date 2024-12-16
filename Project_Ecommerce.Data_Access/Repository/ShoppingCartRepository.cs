using Project_Ecommerce.Data_Access.Data;
using Project_Ecommerce.Data_Access.Repository.IRepository;
using Project_Ecommerce.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project_Ecommerce.Data_Access.Repository
{
    public class ShoppingCartRepository : Repository<ShoppingCart>,IShoppingCartRepository
    {
        private readonly ApplicationDbContext _context;
        public ShoppingCartRepository(ApplicationDbContext context):base(context) 
        {
            _context = context;
        }
    }
}
