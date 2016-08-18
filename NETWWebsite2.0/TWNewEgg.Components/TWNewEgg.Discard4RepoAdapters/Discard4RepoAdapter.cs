using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TWNewEgg.DAL.Interface;
using TWNewEgg.Models.DBModels.TWSQLDB;

using TWNewEgg.Discard4RepoAdapters.Interface;

namespace TWNewEgg.Discard4RepoAdapters
{
    /// <summary>
    /// DB橋接GreetingWords-------------add by bruce 20160330
    /// </summary>
    public class Discard4RepoAdapter : IDiscard4RepoAdapter
    {
        private IRepository<Discard4DB> _list_db;

        public Discard4RepoAdapter(IRepository<Discard4DB> list_info)
        {
            _list_db = list_info;
        }

        /// <summary>
        /// 取得一筆
        /// </summary>
        /// <param name="id">系統流水編號</param>
        /// <returns></returns>
        public Discard4DB GetInfo(int? id)
        {
            //Discard4DB info;
            //if (id == null) return null;
            var info = _list_db.Get(x => x.ID == id);
            return info;
        }

        /// <summary>
        /// 取得資料
        /// </summary>
        /// <param name="salesOrderGroupID">購物車編號</param>
        /// <param name="agreedDiscard4">
        /// 同意癈四機回收
        /// Y=同意, 預設NULL
        /// </param>
        /// <param name="createUser">創建者</param>
        /// <returns></returns>
        public IQueryable<Discard4DB> GetData(int salesOrderGroupID, string agreedDiscard4, string createUser)
        {
            //IQueryable<Discard4DB> list_info = _list_db.GetAll().Where(x => x.CategoryId == category_id);

            var list_info = _list_db.GetAll();

            if (salesOrderGroupID > 0)
                list_info = list_info.Where(x => x.SalesOrderGroupID == salesOrderGroupID);

            if (!string.IsNullOrEmpty(agreedDiscard4))
                list_info = list_info.Where(x => x.AgreedDiscard4 == agreedDiscard4);

            if (!string.IsNullOrEmpty(createUser))
                list_info = list_info.Where(x => x.CreateUser == createUser);

            return list_info;

        }

        public IQueryable<Discard4DB> GetAll()
        {
            //IQueryable<Discard4DB> list_info = _list_db.GetAll();
            var list_info = _list_db.GetAll();
            return list_info;
        }

        public bool Update(Discard4DB info)
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

        public Discard4DB Add(Discard4DB info)
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

        public bool Del(Discard4DB info)
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
