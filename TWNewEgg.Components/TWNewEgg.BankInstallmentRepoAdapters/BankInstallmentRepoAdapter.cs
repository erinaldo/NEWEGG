using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TWNewEgg.BankInstallmentRepoAdapters.Interface;
using TWNewEgg.DAL.Interface;
using TWNewEgg.Models.DBModels.TWSQLDB;

namespace TWNewEgg.BankInstallmentRepoAdapters
{
    /// <summary>
    /// 銀行與分期
    /// </summary>
    public class BankInstallmentRepoAdapter : IBankInstallmentRepoAdapter
    {
        private IRepository<BankInstallment> _bankInstallment;
        private IRepository<Installment> _installment;
        private IRepository<PayType> _paytype;
        
        public BankInstallmentRepoAdapter(IRepository<BankInstallment> bankInstallment, IRepository<Installment> installment, IRepository<PayType> paytype)
        {
            this._bankInstallment = bankInstallment;
            this._installment = installment;
            this._paytype = paytype;
        }

        public IQueryable<BankInstallment> GetAll()
        {
            return this._bankInstallment.GetAll().AsQueryable();
        }

        /// <summary>
        /// 取得所有啟用的信用卡分期
        /// </summary>
        /// <returns>IQueryable Installment</returns>
        public IQueryable<Installment> GetAvailableInstallments()
        {
            return this._installment.GetAll().Where(x => x.Status == (int)InstallmentStatus.Enable).AsQueryable();
        }

        /// <summary>
        /// 取得分期期數的 PayType
        /// </summary>
        /// <param name="installments">分期期數清單</param>
        /// <returns>IQueryable PayType</returns>
        public IQueryable<PayType> GetPayTypesByInstallments(IEnumerable<int> installments)
        {
            IQueryable<PayType> result = null;

            // 分期期數的 ID
            List<int> installmentIdCell = null;

            // PayTypeID
            List<int> payTypeIdCell = null;

            try
            {
                if (installments == null && installments.Count() == 0)
                {
                    throw new Exception();
                }

                // 依分期期數，取得分期期數的 ID
                installmentIdCell = this.GetAvailableInstallments().Where(x => installments.Contains(x.Value)).Select(x => x.ID).ToList();

                if (installmentIdCell == null || installmentIdCell.Count == 0)
                {
                    throw new Exception();
                }

                // 依分期期數的 ID，取得 PayTypeID
                payTypeIdCell = this._bankInstallment.GetAll().Where(x => installmentIdCell.Contains(x.InstallmentID) && x.SerialNumber == 0 && x.Status == (int)BankInstallmentStatus.Enable).Select(x => x.PayTypeID).ToList();

                if (payTypeIdCell == null || payTypeIdCell.Count == 0)
                {
                    throw new Exception();
                }

                // 取得 PayType
                result = this._paytype.GetAll()
                    .Where(x => x.Status == 0)
                    .Where(x => payTypeIdCell.Contains(x.ID)).AsQueryable();

                if (result == null)
                {
                    throw new Exception();
                }
            }
            catch
            {
                if (result == null)
                {
                    result = this._paytype.GetAll().AsQueryable();
                }

                result = result.Where(x => x.ID == -1).AsQueryable();
            }
            
            return result;
        }

        /// <summary>
        /// 取得所有啟用的分期 PayType
        /// </summary>
        /// <returns>IQueryable PayType</returns>
        public IQueryable<PayType> GetAllAvailableInstallmentPaytypes()
        {
            IQueryable<PayType> result = null;

            // 分期期數
            List<int> installmentValueCell = null;

            try
            {
                // 取得所有的分期期數
                installmentValueCell = this.GetAvailableInstallments().Select(x => x.Value).ToList();

                if (installmentValueCell == null || installmentValueCell.Count == 0)
                {
                    throw new Exception();
                }

                // 取得分期期數的 PayType
                result = this.GetPayTypesByInstallments(installmentValueCell);

                installmentValueCell = null;

                if (result == null)
                {
                    throw new Exception();
                }
            }
            catch
            {
                if (result == null)
                {
                    result = this._paytype.GetAll().AsQueryable();
                }

                result = result.Where(x => x.ID == -1).AsQueryable();
            }
            finally
            {
                if (installmentValueCell != null)
                {
                    installmentValueCell = null;
                }
            }

            return result;
        }
    }
}
