using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TWNewEgg.Framework.AutoMapper;
using TWNewEgg.Models.WebsiteMappingRules;
using TWNewEgg.Models.DBModels.TWSQLDB;

using TWNewEgg.Discard4RepoAdapters.Interface;
using TWNewEgg.Discard4Services.Interface;
using TWNewEgg.Models.DomainModels.Discard4;

//廢四機同意
namespace TWNewEgg.Discard4Services
{
    /// <summary>
    /// 依據 BSATW-173 廢四機需求增加 癈四機賣場商品, 1=是癈四機 ---------------add by bruce 20160429
    /// 廢四機同意
    /// </summary>
    public class Discard4Service : IDiscard4Service
    {
        /// <summary>
        /// db a table adapter
        /// </summary>
        private IDiscard4RepoAdapter _adapter;

        public Discard4Service(IDiscard4RepoAdapter adapter)
        {
            _adapter = adapter;
        }


        /// <summary>
        /// 初始化廢四機同意
        /// </summary>
        /// <param name="salesOrderGroupID"></param>
        /// <param name="agreedDiscard4"></param>
        /// <param name="user_name"></param>
        /// <returns></returns>
        public Discard4DM InitData(int salesOrderGroupID, string agreedDiscard4, string user_name)
        {
            Discard4DM domain_info = null;
            try
            {
                Discard4DM search_info = new Discard4DM();
                search_info.SalesOrderGroupID = salesOrderGroupID;
                search_info.AgreedDiscard4 = agreedDiscard4;
                var list_info = this.GetData(search_info);
                
                foreach (var each_info in list_info)
                {
                    //each_info.AgreedDiscard4 = agreedDiscard4;
                    //each_info.SalesOrderGroupID = salesOrderGroupID;
                    //each_info.UpdateUser = user_name;
                    //this.Update(each_info, user_name);
                    
                    this.Del(each_info.ID);

                }

                ////沒有資料
                //if (list_info.Count() == 0) {
                //    Discard4DM add_info = new Discard4DM();
                //    add_info.AgreedDiscard4 = agreedDiscard4;
                //    add_info.SalesOrderGroupID = salesOrderGroupID;
                //    add_info.UpdateUser = user_name;
                //    domain_info = this.Add(add_info, user_name);
                //}

                Discard4DM add_info = new Discard4DM();
                add_info.AgreedDiscard4 = agreedDiscard4;
                add_info.SalesOrderGroupID = salesOrderGroupID;
                add_info.UpdateUser = user_name;
                domain_info = this.Add(add_info, user_name);

                return domain_info;

            }
            catch (Exception ex)
            {
                throw ex;
            }

            

        }


        /// <summary>
        /// 取得一筆資料
        /// </summary>
        /// <param name="id">系統流水編號</param>
        /// <returns></returns>
        public Discard4DM GetInfo(int? id)
        {

            try
            {
                Discard4DM domain_info = null;
                Discard4DB db_info = this._adapter.GetInfo(id);
                if (db_info != null)
                {
                    domain_info = ModelConverter.ConvertTo<Discard4DM>(db_info);
                    return domain_info;
                }
                return null;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 取得資料
        /// </summary>
        /// <param name="info">Discard4DM</param>
        /// <returns></returns>
        public List<Discard4DM> GetData(Discard4DM info)
        {

            List<Discard4DM> list_dm = null;
            List<Discard4DB> list_db_result = null;
            try
            {
                string codetext = string.Empty;
                //IQueryable<Discard4DB> list_db = this._adapter.GetData(salesOrderGroupID, agreedDiscard4, createUser);
                IQueryable<Discard4DB> list_db = this._adapter.GetAll();

                if (!string.IsNullOrEmpty(info.AgreedDiscard4))
                    list_db = list_db.Where(x => x.AgreedDiscard4 == info.AgreedDiscard4);

                //if (info.CreateDate != null)
                //    list_db = list_db.Where(x => x.CreateDate == info.CreateDate);
                if (info.CreateDate.Year >= 1911)
                    list_db = list_db.Where(x => x.CreateDate == info.CreateDate);

                if (!string.IsNullOrEmpty(info.CreateUser))
                    list_db = list_db.Where(x => x.CreateUser == info.CreateUser);

                if (info.SalesOrderGroupID > 0)
                    list_db = list_db.Where(x => x.SalesOrderGroupID == info.SalesOrderGroupID);

                list_db_result = list_db.ToList();

                if (list_db_result != null)
                    list_dm = ModelConverter.ConvertTo<List<Discard4DM>>(list_db_result);
            }
            catch (Exception ex)
            {
                throw new NotImplementedException(ex.Message, ex);
            }
            return list_dm;


        }

        /// <summary>
        /// 取得全部資料
        /// </summary>
        /// <returns>List<Discard4DM></returns>
        public List<Discard4DM> GetAll()
        {
            List<Discard4DM> list_dm = null;
            List<Discard4DB> list_db_result = null;
            try
            {
                IQueryable<Discard4DB> list_db = this._adapter.GetAll();
                list_db_result = list_db.ToList();
                if (list_db_result != null)
                    list_dm = ModelConverter.ConvertTo<List<Discard4DM>>(list_db_result);
            }
            catch (Exception ex)
            {
                throw new NotImplementedException(ex.Message, ex);
            }
            return list_dm;
        }




        /// <summary>
        /// 修改資料
        /// </summary>
        /// <param name="info">Discard4DM</param>
        /// <param name="user_name"></param>
        /// <returns></returns>
        public bool Update(Discard4DM info, string user_name)
        {

            bool is_ok = false;
            if (info == null) return is_ok;
            try
            {
                if (string.IsNullOrEmpty(info.AgreedDiscard4))
                    info.AgreedDiscard4 = string.Empty;
                else
                    info.AgreedDiscard4 = info.AgreedDiscard4.ToUpper();

                info.UpdateDate = DateTime.UtcNow.AddHours(8);
                info.UpdateUser = user_name;
                Discard4DB db_info = ModelConverter.ConvertTo<Discard4DB>(info);
                this._adapter.Update(db_info);
                is_ok = true;
            }
            catch (Exception ex)
            {
                is_ok = false;
                throw ex;
            }
            return is_ok;

        }

        /// <summary>
        /// 建立資料
        /// </summary>
        /// <param name="info">Discard4DM</param>
        /// <param name="user_name"></param>
        /// <returns></returns>
        public Discard4DM Add(Discard4DM info, string user_name)
        {

            bool is_ok = false;
            //if (info == null) return is_ok;
            if (info == null) return info;
            try
            {
                info.CreateDate = DateTime.UtcNow.AddHours(8);
                info.CreateUser = user_name;
                info.UpdateDate = null;
                info.UpdateUser = string.Empty;

                if (string.IsNullOrEmpty(info.AgreedDiscard4))
                    info.AgreedDiscard4 = string.Empty;
                else
                    info.AgreedDiscard4 = info.AgreedDiscard4.ToUpper();

                Discard4DB db_info = ModelConverter.ConvertTo<Discard4DB>(info);
                db_info = this._adapter.Add(db_info);
                info = ModelConverter.ConvertTo<Discard4DM>(db_info);

                return info;

                //is_ok = true;
            }
            catch (Exception ex)
            {
                is_ok = false;
                throw ex;
            }
            //return is_ok;
        }

        /// <summary>
        /// 刪除
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public bool Del(int? id)
        {
            bool is_ok = false;
            try
            {
                Discard4DM domain_info = null;
                var db_info = this._adapter.GetAll().FirstOrDefault(x => x.ID == id);
                if (db_info != null)
                {
                    domain_info = ModelConverter.ConvertTo<Discard4DM>(db_info);
                    this._adapter.Del(db_info);
                    is_ok = true;
                }
                else
                    is_ok = false;
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
