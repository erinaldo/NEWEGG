using salesordergroup.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TWNewEgg.DAL.Interface;
using TWNewEgg.Models.DBModels.TWSQLDB;
using TWNewEgg.Framework.AutoMapper;

namespace TWNewEgg.SalesOrderGrouprRepoAdapter
{
    public class SalesordergroupRepoAdapter : ISalesOrderGroupRepoAdapters
    {
        private IRepository<SalesOrderGroup> _salesOrderGroup;

        public SalesordergroupRepoAdapter(IRepository<SalesOrderGroup> salesOrderGroup)
        {
            this._salesOrderGroup = salesOrderGroup;
        }

        public void insertSalesordergroup(SalesOrderGroup _salesOrderGroupModel)
        {
            this._salesOrderGroup.Create(_salesOrderGroupModel);
        }
        public IQueryable<SalesOrderGroup> GetAll()
        {
            return this._salesOrderGroup.GetAll();
        }

        public bool isConnected()
        {
            return this._salesOrderGroup.isConnected();
        }
    }
}
