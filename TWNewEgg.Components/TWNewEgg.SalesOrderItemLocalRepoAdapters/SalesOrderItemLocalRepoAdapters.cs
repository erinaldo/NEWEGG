﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TWNewEgg.DAL.Interface;
using TWNewEgg.SalesOrderItemLocalRepoAdapters.Interface;

namespace TWNewEgg.SalesOrderItemLocalRepoAdapters
{
    public class SalesOrderItemLocalRepoAdapters : ISalesOrderItemLocalRepoAdapters
    {
        private IMySQLSQLCommandRepository _iMySQLSQLCommandRepository;
        public SalesOrderItemLocalRepoAdapters(IMySQLSQLCommandRepository iMySQLSQLCommandRepository)
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
    }
}
