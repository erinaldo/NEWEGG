using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TWNewEgg.SalesOrderGroupLocalRepoAdapters.Interface
{
    public interface ISalesOrderGroupLocalRepoAdapters
    {
        void MySQLCreate<T>(T model, TWNewEgg.DAL.Model.MySQLModel.Table tableName);
        void MySQLCreate<T>(List<T> model, TWNewEgg.DAL.Model.MySQLModel.Table tableName);
        List<T> getData<T>(string sqlCommand);
        void MysqlUpdate(string listsqlCommand);
    }
}
