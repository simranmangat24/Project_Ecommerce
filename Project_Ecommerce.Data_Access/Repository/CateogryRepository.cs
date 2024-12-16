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
    public class CateogryRepository : Repository<Category>,ICategoryRepository
    {
        private readonly ApplicationDbContext _context;
        public CateogryRepository(ApplicationDbContext context):base(context)
        {
            _context = context; 
        }
    }
}
