using Dapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project_Ecommerce.Data_Access.Repository.IRepository
{
    public interface ISP_Category:IDisposable
    {
        void Execute(string procedureName, DynamicParameters param = null); //Create,Update,Delete
        T Single<T>(string procedureName, DynamicParameters param = null); // Find Get or default
        T OneRecord<T>(string procedureName, DynamicParameters param = null);//Find 
        IEnumerable<T> List<T>(string procedureName, DynamicParameters param = null); //Display
        Tuple<IEnumerable<T1>, IEnumerable<T2>> List<T1, T2>
            (string procedureName, DynamicParameters param = null);   //Multiple results

    }
}
