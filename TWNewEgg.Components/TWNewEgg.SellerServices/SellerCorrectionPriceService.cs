using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TWNewEgg.Framework.AutoMapper;
using TWNewEgg.SellerServices.Interface;
using TWNewEgg.SellerRepoAdapters.Interface;

using TWNewEgg.Models.DBModels.TWBACKENDDB;
using TWNewEgg.Models.DomainModels.Seller;

//供應商對帳單新增調整項目
namespace TWNewEgg.SellerServices
{
    /// <summary>
    /// 依據 BSATW-232 供應商對帳單新增調整項目---------------add by bruce 20160720
    /// </summary>
    public class SellerCorrectionPriceService : ISellerCorrectionPriceService
    {

        /// <summary>
        /// db a table adapter
        /// </summary>
        private ISellerCorrectionPriceRepoAdapter _adapter;

        public SellerCorrectionPriceService(ISellerCorrectionPriceRepoAdapter adapter)
        {
            _adapter = adapter;
        }

        /// <summary>
        /// 儲存即有的或新的資料
        /// </summary>
        /// <param name="list_info"></param>
        /// <returns>List<bool></returns>
        public List<bool> Save(List<TWNewEgg.Models.DomainModels.Seller.SellerCorrectionPriceDM> list_info, string user_name)
        {
            List<bool> list_result = new List<bool>();
            if (list_info == null) return list_result;

            foreach (TWNewEgg.Models.DomainModels.Seller.SellerCorrectionPriceDM each_info in list_info)
            {

                if (each_info.ID == 0)
                {
                    each_info.CreateDate = DateTime.UtcNow.AddHours(8);
                    each_info.CreateUser = user_name;
                    each_info.UpdateDate = null;
                    each_info.UpdateUser = string.Empty;

                    TWNewEgg.Models.DomainModels.Seller.SellerCorrectionPriceDM info = null;
                    info = this.Add(each_info, user_name);
                    if (info != null) list_result.Add(true);
                }
                if (each_info.ID > 0)
                {
                    each_info.UpdateDate = DateTime.UtcNow.AddHours(8);
                    each_info.UpdateUser = user_name;

                    list_result.Add(this.Update(each_info, user_name));
                }
            }
            return list_result;
        }

        
        /// <summary>
        /// 儲存發票後執行
        /// </summary>
        /// <param name="input_info">
        /// 改為以model方式-----------------------add by bruce 20160729
        /// </param>
        /// <param name="user_name"></param>
        /// <returns></returns>
        //public List<bool> Save1(int sellerID, string settlementID, string user_name)
        public List<bool> Save1(TWNewEgg.Models.DomainModels.Seller.SellerCorrectionPriceSearchDM input_info, string user_name)
        {
            List<bool> list_result = new List<bool>();
            //if (sellerID == 0) return list_result;
            if (input_info.SellerIDs.Count() == 0) return list_result;

            List<TWNewEgg.Models.DomainModels.Seller.SellerCorrectionPriceDM> list_info = new List<SellerCorrectionPriceDM>();

            TWNewEgg.Models.DomainModels.Seller.SellerCorrectionPriceSearchDM search_info = new SellerCorrectionPriceSearchDM();
            //search_info.SellerIDs.Add(sellerID);
            search_info.SellerIDs = input_info.SellerIDs;
            search_info.FinanStatus = "V"; //對帳單調整項狀態：I=已匯入, V=Vendor押發票時變動;V=已開發票                           
            search_info.SettlementIDs.Clear();
            //search_info.SettlementIDs.Add(settlementID);
            search_info.SettlementIDs = input_info.SettlementIDs;

            int sellerID=0;
            string settlementID = string.Empty;
            if (input_info.SettlementIDs.Count() > 0 && input_info.SettlementIDs.Count() > 0)
            {
                sellerID = input_info.SellerIDs[0];
                settlementID = input_info.SettlementIDs[0];
            }

            //先找調整項有沒有押過對帳單號-----------add by bruce 20160726
            IQueryable<SellerCorrectionPriceDB> list_result3 = this._adapter.GetData(sellerID, settlementID, user_name);
            if (list_result3.Count() > 0) return list_result; //己押過就不處理


            search_info.FinanStatus = "I"; //對帳單調整項狀態：I=已匯入, V=Vendor押發票時變動;V=已開發票                           
            search_info.SettlementIDs.Clear();

            IQueryable<SellerCorrectionPriceDB> list_info2 = this._adapter.GetData(sellerID, "", user_name);
            list_info2 = list_info2.Where(x => x.FinanStatus == search_info.FinanStatus); //只取I=已匯入的-----------add by bruce 20160727
            List<SellerCorrectionPriceDB> list_db = list_info2.ToList();

            foreach (SellerCorrectionPriceDB each_info in list_db)
            {
                each_info.SettlementID = settlementID; //對帳單編號
                each_info.FinanStatus = "V"; ////對帳單調整項狀態：I=已匯入, V=Vendor押發票時變動;V=已開發票

                if (each_info.ID == 0)
                {
                    each_info.CreateDate = DateTime.UtcNow.AddHours(8);
                    each_info.CreateUser = user_name;

                    each_info.UpdateDate = null;
                    each_info.UpdateUser = string.Empty;

                    TWNewEgg.Models.DBModels.TWBACKENDDB.SellerCorrectionPriceDB db_info = null;
                    db_info = this._adapter.Add(each_info);
                    if (db_info != null) list_result.Add(true);
                }
                if (each_info.ID > 0)
                {
                    each_info.UpdateDate = DateTime.UtcNow.AddHours(8);
                    each_info.UpdateUser = user_name;

                    list_result.Add(this._adapter.Update(each_info));
                }
            }


            return list_result;
        }


        /// <summary>
        /// 取得一筆資料
        /// </summary>
        /// <param name="id">系統流水編號</param>
        /// <returns></returns>
        public TWNewEgg.Models.DomainModels.Seller.SellerCorrectionPriceDM GetInfo(int? id)
        {

            try
            {
                TWNewEgg.Models.DomainModels.Seller.SellerCorrectionPriceDM domain_info = null;
                TWNewEgg.Models.DBModels.TWBACKENDDB.SellerCorrectionPriceDB db_info = this._adapter.GetInfo(id);
                if (db_info != null)
                {
                    domain_info = ModelConverter.ConvertTo<TWNewEgg.Models.DomainModels.Seller.SellerCorrectionPriceDM>(db_info);
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
        /// <param name="info">SellerCorrectionPriceDM</param>
        /// <returns></returns>
        public List<TWNewEgg.Models.DomainModels.Seller.SellerCorrectionPriceDM> GetGroupBy(TWNewEgg.Models.DomainModels.Seller.SellerCorrectionPriceSearchDM input_info)
        {

            List<TWNewEgg.Models.DomainModels.Seller.SellerCorrectionPriceDM> list_dm = new List<TWNewEgg.Models.DomainModels.Seller.SellerCorrectionPriceDM>();
            List<TWNewEgg.Models.DBModels.TWBACKENDDB.SellerCorrectionPriceDB> list_db_result = null;

            try
            {
                if (input_info == null) return list_dm;

                if (string.IsNullOrEmpty(input_info.FinanStatus)) input_info.FinanStatus = string.Empty;
                //轉大寫
                input_info.FinanStatus = input_info.FinanStatus.ToUpper();

                //先找調整項有沒有押過對帳單號-----------add by bruce 20160726      
                TWNewEgg.Models.DomainModels.Seller.SellerCorrectionPriceSearchDM search_info2 = new SellerCorrectionPriceSearchDM();
                search_info2.SellerIDs = input_info.SellerIDs;
                search_info2.SettlementIDs = input_info.SettlementIDs;

                search_info2.FinanStatus = "V"; //對帳單調整項狀態：I=已匯入, V=Vendor押發票時變動;V=已開發票                           
                List<TWNewEgg.Models.DomainModels.Seller.SellerCorrectionPriceDM> list_current = new List<SellerCorrectionPriceDM>();
                list_current = this.GetData(search_info2);

                //沒找到己勾稭的調整項資料
                if (list_current.Count == 0)
                {
                    //找己匯入的
                    search_info2.FinanStatus = "I"; //對帳單調整項狀態：I=已匯入, V=Vendor押發票時變動;V=已開發票 
                    search_info2.SettlementIDs.Clear();
                }

                if (list_current.Count == 0)
                {
                    //"S:已結算"  ,"V:已開發票", "C:已匯款"
                    //與rehtt討論後結論, V跟C不計算--------add by bruce 20160729
                    if (input_info.FinanStatus == "V" || input_info.FinanStatus == "C")
                    {
                        return list_dm;
                    }
                }

                List<TWNewEgg.Models.DomainModels.Seller.SellerCorrectionPriceDM> list_current2 = new List<SellerCorrectionPriceDM>();
                list_current2 = this.GetData(search_info2);

                //有可能是已存檔過,未取到就取有對帳單號碼的
                if (list_current2.Count == 0) search_info2.FinanStatus = "V";

                IQueryable<SellerCorrectionPriceDB> list_db = this._adapter.GetAll();

                //對帳單調整項狀態：I=已匯入, V=Vendor押發票時變動;V=已開發票   
                if (!string.IsNullOrEmpty(search_info2.FinanStatus))
                    list_db = list_db.Where(x => x.FinanStatus == search_info2.FinanStatus);

                //商家編號;供應商代號
                if (search_info2.SellerIDs.Count >= 1)
                    list_db = list_db.Where(p => search_info2.SellerIDs.Contains(p.SellerID));

                //對帳單編號
                if (search_info2.SettlementIDs.Count >= 1)
                    list_db = list_db.Where(p => search_info2.SettlementIDs.Contains(p.SettlementID));

                list_db_result = list_db.ToList();

                //it data will on group by 
                var group_result = list_db_result.GroupBy(g => new { g.SellerID, g.SettlementID }).Select(x => new
                {
                    SellerID = x.Key.SellerID,
                    SettlementID = x.Key.SettlementID,
                    TotalAmount = x.Sum(y => y.TotalAmount),
                    PurePrice = x.Sum(y => y.PurePrice),
                    Tax = x.Sum(y => y.Tax),
                });

                //填入資料
                foreach (var each_info in group_result)
                {

                    Console.WriteLine("SellerID: {0}, TotalAmount: {1}", each_info.SellerID, each_info.TotalAmount);

                    SellerCorrectionPriceDM data_info = new SellerCorrectionPriceDM();
                    data_info.TotalAmount = each_info.TotalAmount;
                    data_info.Tax = each_info.Tax;
                    data_info.PurePrice = each_info.PurePrice;
                    data_info.SellerID = each_info.SellerID;
                    data_info.SettlementID = each_info.SettlementID;

                    data_info.CreateUser = string.Empty;
                    data_info.Description = string.Empty;
                    data_info.FinanStatus = string.Empty;
                    data_info.ID = 0;
                    data_info.Subject = string.Empty;
                    data_info.UpdateUser = string.Empty;

                    list_dm.Add(data_info);
                }

            }
            catch (Exception ex)
            {
                throw new NotImplementedException(ex.Message, ex);
            }
            //輸出
            return list_dm;


        }

        /// <summary>
        /// 取得資料
        /// </summary>
        /// <param name="info">SellerCorrectionPriceDM</param>
        /// <returns></returns>
        public List<TWNewEgg.Models.DomainModels.Seller.SellerCorrectionPriceDM> GetData(TWNewEgg.Models.DomainModels.Seller.SellerCorrectionPriceSearchDM search_info)
        {

            List<TWNewEgg.Models.DomainModels.Seller.SellerCorrectionPriceDM> list_dm = new List<TWNewEgg.Models.DomainModels.Seller.SellerCorrectionPriceDM>();
            List<TWNewEgg.Models.DBModels.TWBACKENDDB.SellerCorrectionPriceDB> list_db_result = null;

            try
            {
                if (search_info == null) return list_dm;

                //轉大寫
                search_info.FinanStatus = search_info.FinanStatus.ToUpper();

                IQueryable<SellerCorrectionPriceDB> list_db = this._adapter.GetAll();

                //對帳單調整項狀態：I=已匯入, V=Vendor押發票時變動;V=已開發票   
                if (!string.IsNullOrEmpty(search_info.FinanStatus))
                    list_db = list_db.Where(x => x.FinanStatus == search_info.FinanStatus);

                //商家編號;供應商代號
                if (search_info.SellerIDs.Count >= 1)
                    list_db = list_db.Where(p => search_info.SellerIDs.Contains(p.SellerID));

                //對帳單編號
                if (search_info.SettlementIDs.Count >= 1)
                    list_db = list_db.Where(p => search_info.SettlementIDs.Contains(p.SettlementID));

                //排序
                if (search_info.OrderByID)
                    list_db = list_db.OrderBy(x => x.ID);

                if (search_info.DescByCreateDate)
                    list_db = list_db.OrderByDescending(x => x.CreateDate);

                list_db_result = list_db.ToList();
                if (list_db_result != null)
                    list_dm = ModelConverter.ConvertTo<List<TWNewEgg.Models.DomainModels.Seller.SellerCorrectionPriceDM>>(list_db_result);
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
        public List<TWNewEgg.Models.DomainModels.Seller.SellerCorrectionPriceDM> GetAll()
        {
            List<SellerCorrectionPriceDM> list_dm = null;
            List<SellerCorrectionPriceDB> list_db_result = null;
            try
            {
                IQueryable<TWNewEgg.Models.DBModels.TWBACKENDDB.SellerCorrectionPriceDB> list_db = this._adapter.GetAll();
                list_db_result = list_db.ToList();
                if (list_db_result != null)
                    list_dm = ModelConverter.ConvertTo<List<TWNewEgg.Models.DomainModels.Seller.SellerCorrectionPriceDM>>(list_db_result);
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
        public bool Update(TWNewEgg.Models.DomainModels.Seller.SellerCorrectionPriceDM info, string user_name)
        {

            bool is_ok = false;
            if (info == null) return is_ok;
            try
            {

                //if (info.InstalledDate != null)
                //{
                //    //DateTime installed_date;
                //    //bool is_date = DateTime.TryParse(info.InstalledDate.Value.ToString("yyyy/MM/dd"), out installed_date);
                //    //if (is_date) info.InstalledDate = new DateTime(info.InstalledDate.Value.Year, info.InstalledDate.Value.Month, info.InstalledDate.Value.Day, 23, 59, 59);
                //    info.InstalledDate = new DateTime(info.InstalledDate.Value.Year, info.InstalledDate.Value.Month, info.InstalledDate.Value.Day, 23, 59, 59);
                //}

                //if (!string.IsNullOrEmpty(info.SettlementID))
                //    info.SettlementID = string.Empty;

                //if (string.IsNullOrEmpty(info.NumberCode))
                //    info.NumberCode = string.Empty;
                //else
                //    info.NumberCode = info.NumberCode.ToUpper();

                //if (string.IsNullOrEmpty(info.SalesorderCode))
                //    info.SalesorderCode = string.Empty;

                //if (string.IsNullOrEmpty(info.SalesorderitemCode))
                //    info.SalesorderitemCode = string.Empty;

                if (string.IsNullOrEmpty(info.CreateUser))
                    info.CreateUser = user_name;

                //if (info.CreateDate.Year == 1)
                //    info.CreateDate = DateTime.UtcNow.AddHours(8);


                info.UpdateDate = DateTime.UtcNow.AddHours(8);
                info.UpdateUser = user_name;

                //SellerCorrectionPriceDB db_info = ModelConverter.ConvertTo<SellerCorrectionPriceDB>(info);

                SellerCorrectionPriceDB db_info = new SellerCorrectionPriceDB();
                db_info.CreateDate = info.CreateDate;
                db_info.CreateUser = info.CreateUser;
                db_info.Description = info.Description;
                db_info.FinanStatus = info.FinanStatus;
                db_info.ID = info.ID;
                db_info.PurePrice = info.PurePrice;
                db_info.SellerID = info.SellerID;
                db_info.SettlementID = info.SettlementID;
                db_info.Subject = info.Subject;
                db_info.Tax = info.Tax;
                db_info.TotalAmount = info.TotalAmount;
                db_info.UpdateDate = info.UpdateDate;
                db_info.UpdateUser = info.UpdateUser;

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
        /// <param name="info">SellerCorrectionPriceDM</param>
        /// <param name="user_name"></param>
        /// <returns></returns>
        public TWNewEgg.Models.DomainModels.Seller.SellerCorrectionPriceDM Add(TWNewEgg.Models.DomainModels.Seller.SellerCorrectionPriceDM info, string user_name)
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

                //if (info.InstalledDate != null)
                //{
                //    //DateTime installed_date;
                //    //bool is_date = DateTime.TryParse(info.InstalledDate.Value.ToString("yyyy/MM/dd"), out installed_date);
                //    //if (is_date) info.InstalledDate = new DateTime(info.InstalledDate.Value.Year, info.InstalledDate.Value.Month, info.InstalledDate.Value.Day, 23, 59, 59);
                //    info.InstalledDate = new DateTime(info.InstalledDate.Value.Year, info.InstalledDate.Value.Month, info.InstalledDate.Value.Day, 23, 59, 59);
                //}


                //if (string.IsNullOrEmpty(info.Discard4Flag))
                //    info.Discard4Flag = string.Empty;

                //if (string.IsNullOrEmpty(info.NumberCode))
                //    info.NumberCode = string.Empty;
                //else
                //    info.NumberCode = info.NumberCode.ToUpper();

                //if (string.IsNullOrEmpty(info.SalesorderCode))
                //    info.SalesorderCode = string.Empty;

                //if (string.IsNullOrEmpty(info.SalesorderitemCode))
                //    info.SalesorderitemCode = string.Empty;

                SellerCorrectionPriceDB db_info = ModelConverter.ConvertTo<SellerCorrectionPriceDB>(info);
                db_info = this._adapter.Add(db_info);
                info = ModelConverter.ConvertTo<SellerCorrectionPriceDM>(db_info);

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
                SellerCorrectionPriceDM domain_info = null;
                var db_info = this._adapter.GetAll().FirstOrDefault(x => x.ID == id);
                if (db_info != null)
                {
                    domain_info = ModelConverter.ConvertTo<SellerCorrectionPriceDM>(db_info);
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
