using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TWNewEgg.Framework.AutoMapper;
using TWNewEgg.Models.WebsiteMappingRules;
using TWNewEgg.Models.DBModels.TWBACKENDDB;

using TWNewEgg.Discard4RepoAdapters.Interface;
using TWNewEgg.Discard4Services.Interface;
using TWNewEgg.Models.DomainModels.Discard4;



//using TWNewEgg.ItemRepoAdapters.Interface;
using TWNewEgg.Models.DBModels.TWSQLDB;

using TWNewEgg.ItemRepoAdapters.Interface;
using TWNewEgg.SalesOrderItemRepoAdapters.Interface;


//癈四機回收四聯單
namespace TWNewEgg.Discard4Services
{
    /// <summary>
    /// 依據 BSATW-173 廢四機需求增加 廢四機賣場商品, 1=是廢四機 ---------------add by bruce 20160502
    /// 癈四機回收四聯單
    /// </summary>
    public class Discard4ItemService : IDiscard4ItemService
    {

        /// <summary>
        /// db a table adapter
        /// </summary>
        private IDiscard4ItemRepoAdapter _adapter; //癈四機回收四聯單
        private IItemRepoAdapter _item_adapter; //賣場品
        private ISalesOrderItemRepoAdapters _orderitem_adapter; //已購入的賣場品

        public Discard4ItemService(IDiscard4ItemRepoAdapter adapter, IItemRepoAdapter item_adapter, ISalesOrderItemRepoAdapters orderitem_adapter)
        {
            _adapter = adapter;
            _item_adapter = item_adapter;
            _orderitem_adapter = orderitem_adapter;
        }

        /// <summary>
        /// 儲存即有的或新的資料
        /// </summary>
        /// <param name="list_info"></param>
        /// <returns>List<bool></returns>
        public List<bool> Save(List<TWNewEgg.Models.DomainModels.Discard4.Discard4ItemDM> list_info, string user_name)
        {
            List<bool> list_result = new List<bool>();
            if (list_info == null) return list_result;

            foreach (TWNewEgg.Models.DomainModels.Discard4.Discard4ItemDM each_info in list_info)
            {
                if (each_info.ID == 0)
                {
                    //each_info.CreateDate = DateTime.UtcNow.AddHours(8);
                    TWNewEgg.Models.DomainModels.Discard4.Discard4ItemDM info = null;
                    info = this.Add(each_info, user_name);
                    if (info != null) list_result.Add(true);
                }
                if (each_info.ID > 0)
                {
                    //each_info.UpdateDate = DateTime.UtcNow.AddHours(8);
                    //TWNewEgg.Models.DomainModels.Discard4.Discard4ItemDM update_info = null;
                    //update_info = this.GetInfo(each_info.ID);
                    //update_info = each_info;
                    list_result.Add(this.Update(each_info, user_name));
                }
            }
            return list_result;
        }

        /// <summary>
        /// 初始化癈四機回收四聯單
        /// </summary>
        /// <param name="salesorderCode"></param>
        /// <param name="user_name"></param>
        /// <returns></returns>
        public List<TWNewEgg.Models.DomainModels.Discard4.Discard4ItemDM> InitData(string salesorderCode, string user_name)
        {
            List<Discard4ItemDM> list_dm = null;
            list_dm = new List<Discard4ItemDM>();
            List<Discard4ItemDB> list_db_result = null;
            list_db_result = new List<Discard4ItemDB>();
            try
            {

                //取得購車商品
                List<string> list_code = new List<string>();
                list_code.Add(salesorderCode);
                IQueryable<SalesOrderItem> list_orderitem = _orderitem_adapter.GetSalesOrderItemListBySalesOrderCode(list_code);
                List<SalesOrderItem> list_orderitem_result = list_orderitem.ToList();

                foreach (SalesOrderItem each_orderitem in list_orderitem_result)
                {
                    //取得賣場品
                    int item_id = each_orderitem.ItemID;

                    var item_db = this._item_adapter.GetIfAvailable(item_id);

                    //這個項目是否為廢四機
                    string discard4 = item_db.Discard4;
                    if (string.IsNullOrEmpty(discard4))
                        discard4 = string.Empty;
                    else
                        discard4 = discard4.ToUpper();

                    //是廢四機商品
                    if (discard4 == "Y")
                    {
                        //刪除現有的廢四機回收四聯單
                        Discard4ItemDM current_info = new Discard4ItemDM();
                        current_info.SalesorderitemCode = each_orderitem.Code;
                        List<Discard4ItemDM> list_discard4 = this.GetData(current_info);
                        foreach (Discard4ItemDM each_discard4 in list_discard4)
                            this.Del(each_discard4.ID);

                        //建立廢四機回收四聯單
                        Discard4ItemDM new_info = new Discard4ItemDM();
                        new_info.CreateDate = DateTime.UtcNow.AddHours(8);
                        new_info.CreateUser = user_name;
                        new_info.UpdateDate = null;
                        new_info.UpdateUser = string.Empty;
                        new_info.Discard4Flag = string.Empty;
                        new_info.ItemID = item_id;
                        new_info.NumberCode = string.Empty;
                        new_info.SalesorderCode = salesorderCode;
                        new_info.SalesorderitemCode = each_orderitem.Code;
                        new_info = this.Add(new_info, user_name);

                    }
                }

                //取得資料
                IQueryable<Discard4ItemDB> list_db = this._adapter.GetAll();
                list_db = list_db.Where(x => x.SalesorderCode == salesorderCode);
                list_db_result = list_db.ToList();
                if (list_db_result != null)
                    list_dm = ModelConverter.ConvertTo<List<Discard4ItemDM>>(list_db_result);

            }
            catch (Exception ex)
            {
                throw new NotImplementedException(ex.Message, ex);
            }
            return list_dm;
        }

        /// <summary>
        /// 取得一筆資料
        /// </summary>
        /// <param name="id">系統流水編號</param>
        /// <returns></returns>
        public TWNewEgg.Models.DomainModels.Discard4.Discard4ItemDM GetInfo(int? id)
        {

            try
            {
                TWNewEgg.Models.DomainModels.Discard4.Discard4ItemDM domain_info = null;
                TWNewEgg.Models.DBModels.TWBACKENDDB.Discard4ItemDB db_info = this._adapter.GetInfo(id);
                if (db_info != null)
                {
                    domain_info = ModelConverter.ConvertTo<TWNewEgg.Models.DomainModels.Discard4.Discard4ItemDM>(db_info);
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
        /// <param name="info">Discard4ItemDM</param>
        /// <returns></returns>
        public List<TWNewEgg.Models.DomainModels.Discard4.Discard4ItemDM> GetData(TWNewEgg.Models.DomainModels.Discard4.Discard4ItemDM info)
        {

            List<TWNewEgg.Models.DomainModels.Discard4.Discard4ItemDM> list_dm = new List<TWNewEgg.Models.DomainModels.Discard4.Discard4ItemDM>();
            List<TWNewEgg.Models.DBModels.TWBACKENDDB.Discard4ItemDB> list_db_result = null;

            try
            {
                if (info == null) return list_dm;

                IQueryable<Discard4ItemDB> list_db = this._adapter.GetAll();

                if (!string.IsNullOrEmpty(info.CreateUser))
                    list_db = list_db.Where(x => x.CreateUser == info.CreateUser);

                if (!string.IsNullOrEmpty(info.Discard4Flag))
                    list_db = list_db.Where(x => x.Discard4Flag == info.Discard4Flag);

                if (info.InstalledDate != null)
                    list_db = list_db.Where(x => x.InstalledDate == info.InstalledDate);

                if (info.ItemID > 0)
                    list_db = list_db.Where(x => x.ItemID == info.ItemID);

                if (!string.IsNullOrEmpty(info.NumberCode))
                    list_db = list_db.Where(x => x.NumberCode == info.NumberCode);

                if (!string.IsNullOrEmpty(info.SalesorderCode))
                    list_db = list_db.Where(x => x.SalesorderCode == info.SalesorderCode);

                if (!string.IsNullOrEmpty(info.SalesorderitemCode))
                    list_db = list_db.Where(x => x.SalesorderitemCode == info.SalesorderitemCode);

                list_db_result = list_db.ToList();
                if (list_db_result != null)
                    list_dm = ModelConverter.ConvertTo<List<TWNewEgg.Models.DomainModels.Discard4.Discard4ItemDM>>(list_db_result);
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
        /// <returns></returns>
        public List<TWNewEgg.Models.DomainModels.Discard4.Discard4ItemDM> GetAll()
        {
            List<Discard4ItemDM> list_dm = null;
            List<Discard4ItemDB> list_db_result = null;
            try
            {
                IQueryable<TWNewEgg.Models.DBModels.TWBACKENDDB.Discard4ItemDB> list_db = this._adapter.GetAll();
                list_db_result = list_db.ToList();
                if (list_db_result != null)
                    list_dm = ModelConverter.ConvertTo<List<TWNewEgg.Models.DomainModels.Discard4.Discard4ItemDM>>(list_db_result);
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
        /// <param name="info">domain model</param>
        /// <returns></returns>
        public bool Update(TWNewEgg.Models.DomainModels.Discard4.Discard4ItemDM info, string user_name)
        {

            bool is_ok = false;
            if (info == null) return is_ok;
            try
            {

                if (info.InstalledDate != null)
                {
                    //DateTime installed_date;
                    //bool is_date = DateTime.TryParse(info.InstalledDate.Value.ToString("yyyy/MM/dd"), out installed_date);
                    //if (is_date) info.InstalledDate = new DateTime(info.InstalledDate.Value.Year, info.InstalledDate.Value.Month, info.InstalledDate.Value.Day, 23, 59, 59);
                    info.InstalledDate = new DateTime(info.InstalledDate.Value.Year, info.InstalledDate.Value.Month, info.InstalledDate.Value.Day, 23, 59, 59);
                }

                if (string.IsNullOrEmpty(info.Discard4Flag))
                    info.Discard4Flag = string.Empty;

                if (string.IsNullOrEmpty(info.NumberCode))
                    info.NumberCode = string.Empty;
                else
                    info.NumberCode = info.NumberCode.ToUpper();

                if (string.IsNullOrEmpty(info.SalesorderCode))
                    info.SalesorderCode = string.Empty;

                if (string.IsNullOrEmpty(info.SalesorderitemCode))
                    info.SalesorderitemCode = string.Empty;

                if (string.IsNullOrEmpty(info.CreateUser))
                    info.CreateUser = user_name;

                if (info.CreateDate.Year == 1)
                    info.CreateDate = DateTime.UtcNow.AddHours(8);

                
                info.UpdateDate = DateTime.UtcNow.AddHours(8);
                info.UpdateUser = user_name;
                Discard4ItemDB db_info = ModelConverter.ConvertTo<Discard4ItemDB>(info);
                this._adapter.Update(db_info);
                is_ok = true;
            }
            catch (Exception ex)
            {
                is_ok = false;
                //throw ex;
            }
            return is_ok;

        }

        /// <summary>
        /// 建立資料
        /// </summary>
        /// <param name="info">Discard4ItemDM</param>
        /// <param name="user_name"></param>
        /// <returns></returns>
        public TWNewEgg.Models.DomainModels.Discard4.Discard4ItemDM Add(TWNewEgg.Models.DomainModels.Discard4.Discard4ItemDM info, string user_name)
        {

            bool is_ok = false;
            //if (info == null) return is_ok;
            if (info == null) return info;
            try
            {
                DateTime now_date = DateTime.UtcNow.AddHours(8);
                now_date = new DateTime(now_date.Year, now_date.Month, now_date.Day, 0, 0, 0);
                info.CreateDate = DateTime.UtcNow.AddHours(8);
                info.CreateUser = user_name;
                info.UpdateDate = null;
                info.UpdateUser = string.Empty;

                if (info.InstalledDate != null)
                {
                    //DateTime installed_date;
                    //bool is_date = DateTime.TryParse(info.InstalledDate.Value.ToString("yyyy/MM/dd"), out installed_date);
                    //if (is_date) info.InstalledDate = new DateTime(info.InstalledDate.Value.Year, info.InstalledDate.Value.Month, info.InstalledDate.Value.Day, 23, 59, 59);
                    info.InstalledDate = new DateTime(info.InstalledDate.Value.Year, info.InstalledDate.Value.Month, info.InstalledDate.Value.Day, 23, 59, 59);
                }


                if (string.IsNullOrEmpty(info.Discard4Flag))
                    info.Discard4Flag = string.Empty;

                if (string.IsNullOrEmpty(info.NumberCode))
                    info.NumberCode = string.Empty;
                else
                    info.NumberCode = info.NumberCode.ToUpper();

                if (string.IsNullOrEmpty(info.SalesorderCode))
                    info.SalesorderCode = string.Empty;

                if (string.IsNullOrEmpty(info.SalesorderitemCode))
                    info.SalesorderitemCode = string.Empty;

                Discard4ItemDB db_info = ModelConverter.ConvertTo<Discard4ItemDB>(info);
                db_info = this._adapter.Add(db_info);
                info = ModelConverter.ConvertTo<Discard4ItemDM>(db_info);

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
                Discard4ItemDM domain_info = null;
                var db_info = this._adapter.GetAll().FirstOrDefault(x => x.ID == id);
                if (db_info != null)
                {
                    domain_info = ModelConverter.ConvertTo<Discard4ItemDM>(db_info);
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


        public bool Empty()
        {
            bool is_ok = false;

            try
            {

            }
            catch (Exception ex)
            {
                throw new NotImplementedException(ex.Message, ex);
            }

            return is_ok;
        }

       



    }
}
