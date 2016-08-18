using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TWNewEgg.CartRepoAdapters.Interface;
using TWNewEgg.DAL.Interface;
using TWNewEgg.Models.DBModels.TWSQLDB;

namespace TWNewEgg.CartRepoAdapters
{
    public class PaymentTermRepoAdapter : IPaymentTermRepoAdapter
    {
        private IRepository<PaymentTerm> _PaymentTermRepo;

        /// <summary>
        /// 建構函式
        /// </summary>
        /// <param name="argPaymentTermRepo"></param>
        public PaymentTermRepoAdapter(IRepository<PaymentTerm> argPaymentTermRepo)
        {
            this._PaymentTermRepo = argPaymentTermRepo;
        }

        /// <summary>
        /// 使用PaymentTermID獲得PaymentTerm資料
        /// </summary>
        /// <param name="PaymentTermID">PaymentTermID</param>
        /// <returns>PaymentTerm List</returns>
        public IQueryable<PaymentTerm> GetPaymentTermByID(List<string> listPaymentTermID)
        {
            IQueryable<PaymentTerm> paymentTermModel = null;

            if (listPaymentTermID != null)
            {
                paymentTermModel = this._PaymentTermRepo.GetAll().Where(x => listPaymentTermID.Contains(x.ID));
            }

            return paymentTermModel;
        }
    }
}
