using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TWNewEgg.DAL.Interface;
using TWNewEgg.Models.DBModels.TWSQLDB;
using TWNewEgg.PaymentTermRepoAdapters.Interface;

namespace TWNewEgg.PaymentTermRepoAdapters
{
    public class PaymentTermRepoAdapters : IPaymentTermRepoAdapters
    {
        private IRepository<PaymentTerm> _PaymentTermRepo;

        /// <summary>
        /// 建構函式
        /// </summary>
        /// <param name="argPaymentTermRepo"></param>
        public PaymentTermRepoAdapters(IRepository<PaymentTerm> argPaymentTermRepo)
        {
            this._PaymentTermRepo = argPaymentTermRepo;
        }

        /// <summary>
        /// 新增
        /// </summary>
        /// <param name="argObjPaymentTerm"></param>
        public bool Create(PaymentTerm argObjPaymentTerm)
        {
            if (argObjPaymentTerm == null)
            {
                return false;
            }

            try
            {
                this._PaymentTermRepo.Create(argObjPaymentTerm);
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        /// <summary>
        /// 讀取全部
        /// </summary>
        /// <returns></returns>
        public List<PaymentTerm> ReadAll()
        {
            List<PaymentTerm> listPaymentTermModel = _PaymentTermRepo.GetAll().ToList();

            return listPaymentTermModel;
        }

        /// <summary>
        /// 更新
        /// </summary>
        /// <param name="argObjPaymentTerm"></param>
        public bool Update(PaymentTerm argObjPaymentTerm)
        {
            if (argObjPaymentTerm == null)
            {
                return false;
            }

            try
            {
                this._PaymentTermRepo.Update(argObjPaymentTerm);
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
    }
}
