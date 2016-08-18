using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TWNewEgg.DAL.Interface;
using TWNewEgg.Models.DBModels.TWBACKENDDB;

using TWNewEgg.SellerRepoAdapters.Interface;

namespace TWNewEgg.SellerRepoAdapters
{
    /// <summary>    
    /// 依據 BSATW-232 供應商對帳單新增調整項目---------------add by bruce 20160719
    /// </summary>
    public class SellerCorrectionPriceRepoAdapter : ISellerCorrectionPriceRepoAdapter
    {
        private IBackendRepository<SellerCorrectionPriceDB> _list_db;

        public SellerCorrectionPriceRepoAdapter(IBackendRepository<SellerCorrectionPriceDB> list_info)
        {
            _list_db = list_info;
        }

        /// <summary>
        /// 取得一筆
        /// </summary>
        /// <param name="id">系統流水編號</param>
        /// <returns></returns>
        public SellerCorrectionPriceDB GetInfo(int? id)
        {
            //SellerCorrectionPriceDB info;
            //if (id == null) return null;
            var info = _list_db.Get(x => x.ID == id);
            return info;
        }

        /// <summary>
        /// 取得資料
        /// </summary>
        /// <param name="salesOrderCode">購物車編號</param>
        /// <param name="salesOrderItemCode">
        /// 同意癈四機回收
        /// Y=同意, 預設NULL
        /// </param>
        /// <param name="itemID">創建者</param>
        /// <returns></returns>
        public IQueryable<SellerCorrectionPriceDB> GetData(int sellerID, string settlementID, string user_name)
        {
            //IQueryable<SellerCorrectionPriceDB> list_info = _list_db.GetAll().Where(x => x.CategoryId == category_id);

            var list_info = _list_db.GetAll();

            if (!string.IsNullOrEmpty(settlementID))
                list_info = list_info.Where(x => x.SettlementID == settlementID);

            //if (!string.IsNullOrEmpty(salesOrderItemCode))
            //    list_info = list_info.Where(x => x.SalesorderitemCode == salesOrderItemCode);

            if (sellerID > 0)
                list_info = list_info.Where(x => x.SellerID == sellerID);

            return list_info;

        }

        public IQueryable<SellerCorrectionPriceDB> GetAll()
        {
            //IQueryable<SellerCorrectionPriceDB> list_info = _list_db.GetAll();
            var list_info = _list_db.GetAll();
            return list_info;
        }

        public bool Update(SellerCorrectionPriceDB info)
        {
            bool is_ok = false;
            if (info == null) return is_ok;
            try
            {
                _list_db.Update(info);
                is_ok = true;
            }
            catch (Exception ex)
            {
                is_ok = false;
                throw ex;
            }
            return is_ok;

        }

        public SellerCorrectionPriceDB Add(SellerCorrectionPriceDB info)
        {
            bool is_ok = false;
            if (info == null) return info;
            try
            {
                _list_db.Create(info);
                is_ok = true;
            }
            catch (Exception ex)
            {
                is_ok = false;
                throw ex;
            }
            return info;
        }

        public bool Del(SellerCorrectionPriceDB info)
        {
            bool is_ok = false;
            if (info == null) return is_ok;
            try
            {
                _list_db.Delete(info);
                is_ok = true;
            }
            catch (Exception ex)
            {
                is_ok = false;
                throw ex;
            }
            return is_ok;
        }
    }
}
