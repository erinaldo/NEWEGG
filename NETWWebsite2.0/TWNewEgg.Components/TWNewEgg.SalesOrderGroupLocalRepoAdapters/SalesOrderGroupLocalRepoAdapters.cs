using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TWNewEgg.DAL.Interface;
using TWNewEgg.SalesOrderGroupLocalRepoAdapters.Interface;

namespace TWNewEgg.SalesOrderGroupLocalRepoAdapters
{
    public class SalesOrderGroupLocalRepoAdapters : ISalesOrderGroupLocalRepoAdapters
    {
        private IMySQLSQLCommandRepository _iMySQLSQLCommandRepository;

        public SalesOrderGroupLocalRepoAdapters(IMySQLSQLCommandRepository iMySQLSQLCommandRepository)
        {
            this._iMySQLSQLCommandRepository = iMySQLSQLCommandRepository;
        }

        public void MySQLCreate<T>(T model, TWNewEgg.DAL.Model.MySQLModel.Table tableName)
        {
            this._iMySQLSQLCommandRepository.Create<T>(model, tableName);
        }
        public void MySQLCreate<T>(List<T> model, TWNewEgg.DAL.Model.MySQLModel.Table tableName)
        {
            this._iMySQLSQLCommandRepository.Create<T>(model, tableName);
        }
        public List<T> getData<T>(string sqlCommand)
        {
            return this._iMySQLSQLCommandRepository.GetData<T>(sqlCommand);
        }
        public void MysqlUpdate(string listsqlCommand)
        {
            this._iMySQLSQLCommandRepository.Update(listsqlCommand);
        }
    }
}
