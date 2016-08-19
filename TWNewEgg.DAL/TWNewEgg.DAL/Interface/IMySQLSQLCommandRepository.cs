using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TWNewEgg.DAL.Interface
{
    public interface IMySQLSQLCommandRepository
    {
        void Create<T>(List<T> model, TWNewEgg.DAL.Model.MySQLModel.Table tableName);
        void Create<T>(T model, TWNewEgg.DAL.Model.MySQLModel.Table tableName);
        List<T> GetData<T>(string sqlCommand);
        void Update(string sqlCommand);
    }
}
