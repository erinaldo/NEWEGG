using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TWNewEgg.SalesOrderLocalRepoAdapters.Interface
{
    public interface ISalesOrderLocalRepoAdapters
    {
        void MySQLCreate<T>(T model, TWNewEgg.DAL.Model.MySQLModel.Table tableName);
        void MySQLCreate<T>(List<T> model, TWNewEgg.DAL.Model.MySQLModel.Table tableName);
    }
}
