using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TWNewEgg.PayTypeRepoAdapters.Interface;
using TWNewEgg.DAL.Interface;
using TWNewEgg.Models.DBModels.TWSQLDB;

namespace TWNewEgg.PayTypeRepoAdapters
{
    public class PayTypeRepoAdapter : IPayTypeRepoAdapter
    {
        private IRepository<PayType> _payType;

        public PayTypeRepoAdapter(IRepository<PayType> payType) {
            this._payType = payType;
        }

        public IQueryable<PayType> GetAll()
        {
            return this._payType.GetAll().AsQueryable();
        }
    }
}
