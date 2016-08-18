using salesordergroup.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TWNewEgg.DAL.Interface;
using TWNewEgg.Framework.AutoMapper;
using TWNewEgg.Models.DBModels.TWSQLDB;

namespace TWNewEgg.SalesOrderGrouprRepoAdapter
{
    public class SalesordergroupTempRepoAdapter : ISalesOrderGroupRepoAdapters
    {
        private IRepository<SalesOrderGroupTemp> _salesOrderGroupTemp;
        private IRepository<SalesOrderGroup> _salesOrderGroup;

        public SalesordergroupTempRepoAdapter(IRepository<SalesOrderGroupTemp> salesOrderGroupTemp, IRepository<SalesOrderGroup> salesOrderGroup)
        {
            this._salesOrderGroupTemp = salesOrderGroupTemp;
            this._salesOrderGroup = salesOrderGroup;
        }

        public void insertSalesordergroup(SalesOrderGroup _salesOrderGroupModel)
        {
            SalesOrderGroupTemp soGroupTemp = ModelConverter.ConvertTo<SalesOrderGroupTemp>(_salesOrderGroupModel);
            this._salesOrderGroupTemp.Create(soGroupTemp);
            _salesOrderGroupModel.ID = soGroupTemp.ID;
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
