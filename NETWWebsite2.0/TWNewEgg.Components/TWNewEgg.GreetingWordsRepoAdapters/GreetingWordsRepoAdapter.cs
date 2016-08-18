using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TWNewEgg.DAL.Interface;
using TWNewEgg.Models.DBModels.TWSQLDB;

using TWNewEgg.GreetingWordsRepoAdapters.Interface;

namespace TWNewEgg.GreetingWordsRepoAdapters
{
    /// <summary>
    /// DB橋接GreetingWords-------------add by bruce 20160330
    /// </summary>
    public class GreetingWordsRepoAdapter : IGreetingWordsRepoAdapter
    {
        private IRepository<GreetingWordsDB> _list_db;

        public GreetingWordsRepoAdapter(IRepository<GreetingWordsDB> list_info)
        {
            _list_db = list_info;
        }

        /// <summary>
        /// 取得一筆
        /// </summary>
        /// <param name="id">系統流水編號</param>
        /// <returns></returns>
        public GreetingWordsDB GetInfo(int? id)
        {
            //GreetingWordsDB info;
            //if (id == null) return null;
            var info = _list_db.Get(x => x.ID == id);
            return info;
        }

        /// <summary>
        /// 取得資料
        /// </summary>
        /// <param name="category_id">
        /// 分類Id
        /// 0 首頁熱門關鍵字
        /// 1 登入問候語
        /// 2 節日問候卡
        /// </param>
        /// <param name="description">文字描述</param>
        /// <param name="codetext"> 
        /// 自訂代碼文字
        /// use for 登入問候語
        /// </param>
        /// <param name="showall">是否顯示, 1:顯示, 0:不顯示</param>
        /// <returns></returns>
        public IQueryable<GreetingWordsDB> GetData(int category_id, string description, string codetext, int? showall)
        {
            //IQueryable<GreetingWordsDB> list_info = _list_db.GetAll().Where(x => x.CategoryId == category_id);

            var list_info = _list_db.GetAll().Where(x => x.CategoryId == category_id);

            if (category_id != null)
                list_info = list_info.Where(x => x.Description == description);

            if (!string.IsNullOrEmpty(codetext))
                list_info = list_info.Where(x => x.CodeText == codetext);

            if (showall != null)
                list_info = list_info.Where(x => x.ShowAll >= showall);

            return list_info;
        }

        public IQueryable<GreetingWordsDB> GetAll()
        {
            //IQueryable<GreetingWordsDB> list_info = _list_db.GetAll();
            var list_info = _list_db.GetAll();
            return list_info;
        }

        public bool Update(GreetingWordsDB info)
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

        public GreetingWordsDB Add(GreetingWordsDB info)
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

        public bool Del(GreetingWordsDB info)
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
