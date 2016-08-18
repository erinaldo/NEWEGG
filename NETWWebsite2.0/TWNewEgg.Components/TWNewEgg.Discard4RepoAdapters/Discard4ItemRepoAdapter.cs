using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TWNewEgg.DAL.Interface;
using TWNewEgg.Models.DBModels.TWBACKENDDB;

using TWNewEgg.Discard4RepoAdapters.Interface;

namespace TWNewEgg.Discard4RepoAdapters
{
    /// <summary>
    /// DB橋接GreetingWords-------------add by bruce 20160330
    /// </summary>
    public class Discard4ItemRepoAdapter : IDiscard4ItemRepoAdapter
    {
        private IBackendRepository<Discard4ItemDB> _list_db;

        public Discard4ItemRepoAdapter(IBackendRepository<Discard4ItemDB> list_info)
        {
            _list_db = list_info;
        }

        /// <summary>
        /// 取得一筆
        /// </summary>
        /// <param name="id">系統流水編號</param>
        /// <returns></returns>
        public Discard4ItemDB GetInfo(int? id)
        {
            //Discard4ItemDB info;
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
        public IQueryable<Discard4ItemDB> GetData(string salesOrderCode, string salesOrderItemCode, int itemID)
        {
            //IQueryable<Discard4ItemDB> list_info = _list_db.GetAll().Where(x => x.CategoryId == category_id);

            var list_info = _list_db.GetAll();

            if (!string.IsNullOrEmpty(salesOrderCode))
                list_info = list_info.Where(x => x.SalesorderCode == salesOrderCode);

            if (!string.IsNullOrEmpty(salesOrderItemCode))
                list_info = list_info.Where(x => x.SalesorderitemCode == salesOrderItemCode);

            if (itemID > 0)
                list_info = list_info.Where(x => x.ItemID == itemID);

            return list_info;

        }

        public IQueryable<Discard4ItemDB> GetAll()
        {
            //IQueryable<Discard4ItemDB> list_info = _list_db.GetAll();
            var list_info = _list_db.GetAll();
            return list_info;
        }

        public bool Update(Discard4ItemDB info)
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

        public Discard4ItemDB Add(Discard4ItemDB info)
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

        public bool Del(Discard4ItemDB info)
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
