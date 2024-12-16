using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Project_Ecommerce.Data_Access.Data;
using Project_Ecommerce.Data_Access.Repository.IRepository;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project_Ecommerce.Data_Access.Repository
{
    public class SP_Category : ISP_Category
    {
        private readonly ApplicationDbContext _context;
        private static string connectionString = "";

        public SP_Category(ApplicationDbContext context)
        {
            _context = context;
            connectionString = _context.Database.GetDbConnection().ConnectionString;

        }

        public void Dispose()
        {
            _context.Dispose();
        }

        public void Execute(string procedureName, DynamicParameters param = null)
        {
            using (SqlConnection sqlCon = new SqlConnection(connectionString))
            {
                sqlCon.Open();
                sqlCon.Execute(procedureName, param, commandType: CommandType.StoredProcedure);

            }
        }

        public IEnumerable<T> List<T>(string procedureName, DynamicParameters param = null)
        {
            using (SqlConnection sqlCon = new SqlConnection(connectionString))
            {
                sqlCon.Open();                                                                                //Show Table
                return sqlCon.Query<T>(procedureName, param, commandType: CommandType.StoredProcedure);

            }
        }

        public Tuple<IEnumerable<T1>, IEnumerable<T2>> List<T1, T2>(string procedureName, DynamicParameters param = null)
        {
            using (SqlConnection sqlCon = new SqlConnection(connectionString))
            {
                sqlCon.Open();
                var result = sqlCon.QueryMultiple(procedureName, param, commandType: CommandType.StoredProcedure);
                var Item1 = result.Read<T1>();                                                                          //Multiple record in single-single list (SPLIT) 
                var Item2 = result.Read<T2>();
                if (Item1 != null && Item2 != null)
                    return new Tuple<IEnumerable<T1>, IEnumerable<T2>>(Item1, Item2);
                return new Tuple<IEnumerable<T1>, IEnumerable<T2>>(new List<T1>(), new List<T2>());

            }
        }

        public T OneRecord<T>(string procedureName, DynamicParameters param = null)
        {

            using (SqlConnection sqlCon = new SqlConnection(connectionString))
            {
                sqlCon.Open();
                var value = sqlCon.Query<T>(procedureName, param, commandType: CommandType.StoredProcedure);                //One Record
                return value.FirstOrDefault();
            }
        }

        public T Single<T>(string procedureName, DynamicParameters param = null)
        {
            using (SqlConnection sqlcon = new SqlConnection(connectionString))
            {
                sqlcon.Open();
                return sqlcon.ExecuteScalar<T>(procedureName, param, commandType: CommandType.StoredProcedure);            //ExecuteScaler =for Single Result -Occupies less memory

            }
        }
    }
}
