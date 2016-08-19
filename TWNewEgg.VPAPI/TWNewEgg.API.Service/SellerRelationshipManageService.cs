using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Dynamic;
using System.Text;
using System.Threading.Tasks;
using TWNewEgg.API.Models;
using TWNewEgg.DB.TWSELLERPORTALDB.Models;
using AutoMapper;
using log4net;
using log4net.Config;


namespace TWNewEgg.API.Service
{
    
    interface ISellerRelationshipManageRepository
    {
        API.Models.ActionResponse<IEnumerable<VM_Seller_BasicInfo>> GetAll();
        //API.Models.ActionResponse<VM_Seller_BasicInfo> Get(int id);
        API.Models.ActionResponse<VM_Seller_BasicInfo> Get(int id);
        API.Models.ActionResponse<Seller_BasicInfo> Add(Seller_BasicInfo item);
        API.Models.ActionResponse<bool> Update(Seller_BasicInfo item);
        API.Models.ActionResponse<bool> Delete(int? id);
    }

    public class SellerRelationshipManageService : ISellerRelationshipManageRepository
    {
        // 記錄訊息
        private static ILog log = LogManager.GetLogger(typeof(SellerRelationshipManageService));

        public enum ResponseCode
        {
            Success = 0,
            Error = 1
        }

        private DB.TWSellerPortalDBContext db = new DB.TWSellerPortalDBContext();
        private DB.TWSqlDBContext sqldb = new DB.TWSqlDBContext();


        #region 資料取得

        public API.Models.ActionResponse<IEnumerable<VM_Seller_BasicInfo>> GetAll()
        {
            API.Models.ActionResponse<IEnumerable<VM_Seller_BasicInfo>> ar = new ActionResponse<IEnumerable<VM_Seller_BasicInfo>>();
            IEnumerable<Seller_BasicInfo> q;
            IEnumerable<VM_Seller_BasicInfo> q2;
            Mapper.CreateMap<Seller_BasicInfo, VM_Seller_BasicInfo>();

            try
            {
                q2 = db.Seller_BasicInfo.AsEnumerable().Select(role => Mapper.Map<Seller_BasicInfo, VM_Seller_BasicInfo>(role)).ToList();
                foreach (var item in q2)
                    item.SellerCountryCodeName = getCountryCodeName(item.SellerCountryCode);

                ar.Code = (int)ResponseCode.Success;
                ar.IsSuccess = true;
                ar.Msg = "Success";
                ar.Body = q2;
            }
            catch (Exception ex)
            {
                ar.Code = (int)ResponseCode.Error;
                ar.IsSuccess = false;
                ar.Msg = ex.Message;
            }
            return ar;

        }

        public API.Models.ActionResponse<VM_Seller_BasicInfo> Get(int id)
        {
            //API.Models.ActionResponse<VM_Seller_BasicInfo> ar = new ActionResponse<VM_Seller_BasicInfo>();
            API.Models.ActionResponse<VM_Seller_BasicInfo> ar = new ActionResponse<VM_Seller_BasicInfo>();

            Seller_BasicInfo obj = null;
            //Mapper.CreateMap<Seller_BasicInfo, VM_Seller_BasicInfo>();

            try
            {
                //obj = db.Seller_BasicInfo.Find(id);
                obj = db.Seller_BasicInfo.Where(x => x.SellerID == id).FirstOrDefault();
                
                VM_Seller_BasicInfo obj2 = Mapper.Map<Seller_BasicInfo, VM_Seller_BasicInfo>(obj); //copy
                obj2.SellerCountryCodeName = getCountryCodeName(obj.SellerCountryCode);

                ar.Code = (int)ResponseCode.Success;
                ar.IsSuccess = true;
                ar.Msg = "Success";
                ar.Body = obj2;


            }
            catch (Exception ex)
            {
                ar.Code = (int)ResponseCode.Error;
                ar.IsSuccess = false;
                ar.Msg = ex.Message;
            }
            return ar;

        }

        //取得中文名稱.
        private string getCountryCodeName(string c_code)
        {
            return db.Seller_Country.Where(c => c.CountryCode == c_code).Select(c => c.Name).FirstOrDefault();
        }

        public API.Models.ActionResponse<IEnumerable<VM_Seller_BasicInfo>> GetbyQuery(SellerRMQuery sQuery)
        {
            API.Models.ActionResponse<IEnumerable<VM_Seller_BasicInfo>> ar = new ActionResponse<IEnumerable<VM_Seller_BasicInfo>>();

            IEnumerable<Seller_BasicInfo> q;
            IEnumerable<VM_Seller_BasicInfo> q2;

            try
            {

                q = db.Seller_BasicInfo.ToList();
                
                //查詢條件如下:

                if (sQuery.SellerID != null)
                {
                    q = q.Where(c => c.SellerID == sQuery.SellerID);
                }


                if (!string.IsNullOrEmpty(sQuery.SellerName))
                {
                    q = q.Where(c => c.SellerName != null).Where(c => c.SellerName.Contains(sQuery.SellerName));

                }


                if (!string.IsNullOrEmpty(sQuery.UserEmail))
                {
                    q = q.Where(c => c.SellerEmail != null).Where(c => c.SellerEmail.Contains(sQuery.UserEmail));

                }

                var totalCount = q.Count();

                if (sQuery.PageSize != null) //SL 的 Code 的使用.
                {
                    //paging
                    int numberOfObjectsPerPage = sQuery.PageSize.Value;
                    //StartRowIndex = pageIndex * pageSize
                    q = q.AsQueryable().OrderBy(sQuery.SortField)
                        //q = q.AsQueryable().OrderBy("SellerID")
                      .Skip(sQuery.StartRowIndex.Value)
                      .Take(numberOfObjectsPerPage).ToList();
                    ar.Code = totalCount;
                }
                //暫用 totalCount;
                //ar.Code = (int)ResponseCode.Success;
                Mapper.CreateMap<Seller_BasicInfo, VM_Seller_BasicInfo>();
                q2 = q.Select(role => Mapper.Map<Seller_BasicInfo, VM_Seller_BasicInfo>(role)).ToList();
                foreach (var item in q2)
                    item.SellerCountryCodeName = getCountryCodeName(item.SellerCountryCode);

                ar.IsSuccess = true;
                ar.Msg = "Success";
                ar.Body = q2;
            }
            catch (Exception ex)
                {
                ar.Code = (int)ResponseCode.Error;
                ar.IsSuccess = false;
                ar.Msg = ex.Message;
                }

            return ar;
        }

        /// <summary>
        /// 搜尋商家關係清單
        /// </summary>
        /// <param name="searchCondition">搜尋條件</param>
        /// <returns>商家清單</returns>
        public API.Models.ActionResponse<List<VM_Seller_BasicInfo>> SearchSellerForRelationship(SellerRelationshipSearchCondition searchCondition)
        {
            API.Models.ActionResponse<List<VM_Seller_BasicInfo>> result = new ActionResponse<List<VM_Seller_BasicInfo>>();
            result.Body = new List<VM_Seller_BasicInfo>();
            result.IsSuccess = true;
            result.Msg = string.Empty;

            // 檢查輸入
            ActionResponse<bool> checkSellerRelationshipSearchCondition = CheckSellerRelationshipSearchCondition(searchCondition);

            if (checkSellerRelationshipSearchCondition.IsSuccess)
            {
                DB.TWSellerPortalDBContext dbSellerPortal = new DB.TWSellerPortalDBContext();

                // 從資料庫取得資料
                List<DB.TWSELLERPORTALDB.Models.Seller_BasicInfo> sellerBasicInfoCell = new List<Seller_BasicInfo>();

                try
                {
                    sellerBasicInfoCell = dbSellerPortal.Seller_BasicInfo.OrderByDescending(x => x.UpdateDate).ToList();

                    // 篩選 Seller ID
                    // 但在有管理權限，且 SellerID 為 null 時不篩選，此時將顯示所有 SellerID 的資料
                    if (!(searchCondition.IsAdmin == true && searchCondition.SellerID == null))
                    {
                        sellerBasicInfoCell = sellerBasicInfoCell.Where(x => x.SellerID == searchCondition.SellerID).ToList();
                    }

                    #region 搜尋條件

                    // 篩選狀態
                    if (searchCondition.SellerStatus != SellerStatus.All)
                    {
                        switch (searchCondition.SellerStatus)
                        {
                            case SellerStatus.Active:
                                {
                                    sellerBasicInfoCell = sellerBasicInfoCell.Where(x => x.SellerStatus == "A").ToList();
                                    break;
                                }
                            case SellerStatus.Inactive:
                                {
                                    sellerBasicInfoCell = sellerBasicInfoCell.Where(x => x.SellerStatus == "I").ToList();
                                    break;
                                }
                            case SellerStatus.Closed:
                                {
                                    sellerBasicInfoCell = sellerBasicInfoCell.Where(x => x.SellerStatus == "C").ToList();
                                    break;
                                }
                            default:
                                {
                                    sellerBasicInfoCell = new List<Seller_BasicInfo>();
                                    break;
                                }
                        }
                    }

                    // 篩選建立日期
                    if (string.IsNullOrEmpty(searchCondition.CreateDateStart) == false && string.IsNullOrEmpty(searchCondition.CreateDateEnd) == false)
                    {
                        DateTime createDateStart;
                        DateTime createDateEnd;

                        if (DateTime.TryParse(searchCondition.CreateDateStart, out createDateStart)
                            && DateTime.TryParse(searchCondition.CreateDateEnd, out createDateEnd))
                        {
                            sellerBasicInfoCell = sellerBasicInfoCell.Where(x => x.InDate > createDateStart.Date && x.InDate < createDateEnd.Date.AddDays(1)).ToList();
                        }
                    }

                    // 篩選建立日期
                    if (string.IsNullOrEmpty(searchCondition.UpdateDateStart) == false && string.IsNullOrEmpty(searchCondition.UpdateDateEnd) == false)
                    {
                        DateTime updateDateStart;
                        DateTime updateDateEnd;

                        if (DateTime.TryParse(searchCondition.UpdateDateStart, out updateDateStart)
                            && DateTime.TryParse(searchCondition.UpdateDateEnd, out updateDateEnd))
                        {
                            sellerBasicInfoCell = sellerBasicInfoCell.Where(x => x.UpdateDate > updateDateStart.Date && x.UpdateDate < updateDateEnd.Date.AddDays(1)).ToList();
                        }
                    }

                    // 篩選地區
                    if (searchCondition.SellerCountryCode != CountryCode.All)
                    {
                        switch (searchCondition.SellerCountryCode)
                        {
                            case CountryCode.Canada:
                                {
                                    sellerBasicInfoCell = sellerBasicInfoCell.Where(x => x.SellerCountryCode == "CA").ToList();
                                    break;
                                }
                            case CountryCode.China:
                                {
                                    sellerBasicInfoCell = sellerBasicInfoCell.Where(x => x.SellerCountryCode == "CN").ToList();
                                    break;
                                }
                            case CountryCode.HongKong:
                                {
                                    sellerBasicInfoCell = sellerBasicInfoCell.Where(x => x.SellerCountryCode == "HK").ToList();
                                    break;
                                }
                            case CountryCode.Taiwan:
                                {
                                    sellerBasicInfoCell = sellerBasicInfoCell.Where(x => x.SellerCountryCode == "TW").ToList();
                                    break;
                                }
                            case CountryCode.UnitedStates:
                                {
                                    sellerBasicInfoCell = sellerBasicInfoCell.Where(x => x.SellerCountryCode == "US").ToList();
                                    break;
                                }
                            default:
                                {
                                    sellerBasicInfoCell = new List<Seller_BasicInfo>();
                                    break;
                                }
                        }
                    }

                    #endregion 搜尋條件

                    if (sellerBasicInfoCell.Count > 0)
                    {
                        Mapper.CreateMap<Seller_BasicInfo, VM_Seller_BasicInfo>();
                        result.Body = AutoMapper.Mapper.Map(sellerBasicInfoCell, result.Body);
                    }
                }
                catch (Exception ex)
                {
                    result.IsSuccess = false;
                    log.Info(string.Format("取得商家資訊失敗; ExceptionMessage = {0}; StackTrace = {1}", GetExceptionMessage(ex), ex.StackTrace));
                }

                List<DB.TWSELLERPORTALDB.Models.Seller_User> sellerUserCell = dbSellerPortal.Seller_User.ToList();
                foreach (VM_Seller_BasicInfo sellerBasicInfo_ViewModel in result.Body)
                {
                    #region 轉換地區

                    switch (sellerBasicInfo_ViewModel.SellerCountryCode)
                    {
                        case "CA":
                            {
                                sellerBasicInfo_ViewModel.SellerCountryCodeName = "Canada";
                                break;
                            }
                        case "CN":
                            {
                                sellerBasicInfo_ViewModel.SellerCountryCodeName = "China";
                                break;
                            }
                        case "HK":
                            {
                                sellerBasicInfo_ViewModel.SellerCountryCodeName = "Hong Kong";
                                break;
                            }
                        case "TW":
                            {
                                sellerBasicInfo_ViewModel.SellerCountryCodeName = "Taiwan";
                                break;
                            }
                        case "US":
                            {
                                sellerBasicInfo_ViewModel.SellerCountryCodeName = "United States";
                                break;
                            }
                        default:
                            {
                                sellerBasicInfo_ViewModel.SellerCountryCodeName = string.Empty;
                                break;
                            }
                    }

                    #endregion 轉換地區

                    // 判斷是否為"邀請中"(I) 的狀態，才可以觸發再邀請功能
                    if (sellerUserCell.Any(x => x.UserEmail == sellerBasicInfo_ViewModel.SellerEmail))
                    {
                        DB.TWSELLERPORTALDB.Models.Seller_User sellerUser = sellerUserCell.Where(x => x.UserEmail == sellerBasicInfo_ViewModel.SellerEmail).FirstOrDefault();
                        if (sellerUser.Status == "I")
                        {
                            result.Body.Find(x => x.SellerEmail == sellerBasicInfo_ViewModel.SellerEmail).SellerUserStatus = true;
                        }
                    }
                }
            }
            else
            {
                result.IsSuccess = false;
            }

            if (result.IsSuccess)
            {
                if (result.Body.Count > 0)
                {
                    result.Msg = "查詢成功。";
                }
                else
                {
                    result.Msg = "查無資料。";
                }
            }
            else
            {
                result.Msg = "查詢失敗。";
            }

            result.Code = SetResponseCode(result.IsSuccess);

            return result;
        }

        /// <summary>
        /// 檢查商家關係搜尋條件
        /// </summary>
        /// <param name="searchCondition">商家關係搜尋條件</param>
        /// <returns>檢查結果</returns>
        private ActionResponse<bool> CheckSellerRelationshipSearchCondition(SellerRelationshipSearchCondition searchCondition)
        {
            ActionResponse<bool> result = new ActionResponse<bool>();
            result.IsSuccess = true;
            result.Msg = string.Empty;

            ActionResponse<bool> checkSellerID = CheckSellerID(searchCondition.SellerID);
            if (checkSellerID.IsSuccess == false)
                {
                result.IsSuccess = false;
                log.Info(result.Msg);
                }
                
            ActionResponse<bool> checkCreateDateStart = IsDateTiem(searchCondition.CreateDateStart);
            if(checkSellerID.IsSuccess == false)
                {
                result.IsSuccess = false;
                log.Info(string.Format("創建日期(開始日期)錯誤，{0}", checkSellerID.Msg));
            }

            ActionResponse<bool> checkCreateDateEnd = IsDateTiem(searchCondition.CreateDateEnd);
            if (checkSellerID.IsSuccess == false)
                    {
                result.IsSuccess = false;
                log.Info(string.Format("創建日期(結束日期)錯誤，{0}", checkSellerID.Msg));
                    }

            ActionResponse<bool> checkUpdateDateStart = IsDateTiem(searchCondition.UpdateDateStart);
            if (checkSellerID.IsSuccess == false)
                    {
                result.IsSuccess = false;
                log.Info(string.Format("最後編輯日期(開始日期)錯誤，{0}", checkSellerID.Msg));
                    }

            ActionResponse<bool> checkUpdateDateEnd = IsDateTiem(searchCondition.UpdateDateEnd);
            if (checkSellerID.IsSuccess == false)
            {
                result.IsSuccess = false;
                log.Info(string.Format("最後編輯日期(結束日期)錯誤，{0}", checkSellerID.Msg));
                }

            result.Body = result.IsSuccess;
            result.Code = SetResponseCode(result.IsSuccess);

            return result;
            }

        /// <summary>
        /// 檢查商家 ID
        /// </summary>
        /// <param name="sellerID">商家 ID</param>
        /// <returns>檢查結果</returns>
        private ActionResponse<bool> CheckSellerID(int? sellerID)
        {
            ActionResponse<bool> result = new ActionResponse<bool>();
            result.IsSuccess = true;
            result.Msg = string.Empty;

            if (sellerID != null && sellerID < 0)
            {
                result.IsSuccess = false;
                result.Msg = "商家 ID 不可以小於 0。";
            }

            result.Body = result.IsSuccess;
            result.Code = SetResponseCode(result.IsSuccess);

            return result;
            }

        /// <summary>
        /// 檢查是否為時間日期格式
        /// </summary>
        /// <param name="value">輸入值</param>
        /// <returns>檢查結果</returns>
        private ActionResponse<bool> IsDateTiem(string value)
        {
            ActionResponse<bool> result = new ActionResponse<bool>();
            result.IsSuccess = true;
            result.Msg = string.Empty;

            if (string.IsNullOrEmpty(value) == false)
            {
                DateTime dateTiem;
                if (DateTime.TryParse(value, out dateTiem) == false)
                {
                    result.IsSuccess = false;
                    result.Msg = "請輸入正確的時間格式。";
                }
            }

            result.Body = result.IsSuccess;
            result.Code = SetResponseCode(result.IsSuccess);

            return result;
        }

        #endregion

        #region 資料異動
        public API.Models.ActionResponse<Seller_BasicInfo> Add(Seller_BasicInfo item)
        {

            API.Models.ActionResponse<Seller_BasicInfo> ar = new ActionResponse<Seller_BasicInfo>();
            if (item == null) //無參數
            {
                //throw new ArgumentNullException("item");
                ar.Code = (int)ResponseCode.Error;
                ar.IsSuccess = false;
                ar.Msg = new ArgumentNullException("item").Message;
                return ar;
            }

            try
            {
                db.Seller_BasicInfo.Add(item);
                db.SaveChanges(); //新增

                ar.Code = (int)ResponseCode.Success;
                ar.IsSuccess = true;
                ar.Msg = "Success";
                ar.Body = item;
            }
            catch (Exception ex)
            {
                ar.Code = (int)ResponseCode.Error;
                ar.IsSuccess = false;
                ar.Msg = ex.Message;
            }
            return ar;
        }

        public API.Models.ActionResponse<bool> Update(Seller_BasicInfo item)
        {
            API.Models.ActionResponse<bool> ar = new ActionResponse<bool>();
            if (item == null) //無參數
            {
                //throw new ArgumentNullException("item");
                ar.Code = (int)ResponseCode.Error;
                ar.IsSuccess = false;
                ar.Msg = new ArgumentNullException("item").Message;
                ar.Body = false;
                return ar;
            }

            try
            {
                db.Entry(item).State = EntityState.Modified;
                db.SaveChanges();//異動

                ar.Code = (int)ResponseCode.Success;
                ar.IsSuccess = true;
                ar.Msg = "Success";
                ar.Body = true;
            }
            catch (Exception ex)
            {
                ar.Code = (int)ResponseCode.Error;
                ar.IsSuccess = false;
                ar.Msg = ex.Message;
                ar.Body = false;
            }
            return ar;
        }




        public API.Models.ActionResponse<bool> UpdateList(List<Seller_BasicInfo> itemList)
        {
            API.Models.ActionResponse<bool> ar = new ActionResponse<bool>();
            try
            {
                foreach (var item in itemList)
                {
                    if (item == null) continue;

                    //default Value;
                    if (item.SellerEmail == null)
                        item.SellerEmail = string.Empty;
                    if (item.LanguageCode == null)
                        item.LanguageCode = string.Empty;
                    if (item.AccountTypeCode == null)
                        item.AccountTypeCode = string.Empty;
                    //if (item.CountryCode == null)
                    //    item.CountryCode = 0;

                    item.UpdateDate = DateTime.Now;

                    db.Entry(item).State = EntityState.Modified;
                }
                db.SaveChanges();//異動
                ar.Code = (int)ResponseCode.Success;
                ar.IsSuccess = true;
                ar.Msg = "Success";
                ar.Body = true;
            }
            catch (Exception ex)
            {
                ar.Code = (int)ResponseCode.Error;
                ar.IsSuccess = false;
                ar.Msg = ex.InnerException.InnerException.Message;
                ar.Body = false;
            }

            return ar;
        }

        public API.Models.ActionResponse<bool> Delete(int? id)
        {
            // TO DO : Code to remove the records from database
            API.Models.ActionResponse<bool> ar = new ActionResponse<bool>();
            if (id == null) //無參數
            {
                //throw new ArgumentNullException("item");
                ar.Code = (int)ResponseCode.Error;
                ar.IsSuccess = false;
                ar.Msg = new ArgumentNullException("id").Message;
                ar.Body = false;
                return ar;
            }

            try
            {
                Seller_BasicInfo seller_basicinfo = db.Seller_BasicInfo.Find(id);
                db.Seller_BasicInfo.Remove(seller_basicinfo);
                db.SaveChanges();//刪除

                ar.Code = (int)ResponseCode.Success;
                ar.IsSuccess = true;
                ar.Msg = "Success";
                ar.Body = true;
            }
            catch (Exception ex)
            {
                ar.Code = (int)ResponseCode.Error;
                ar.IsSuccess = false;
                ar.Msg = ex.Message;
                ar.Body = false;
            }
            return ar;



        }
        #endregion

        #region SelectItemList
        public API.Models.ActionResponse<List<ValueTextItem>> GetCMList() //CM 
        {
            API.Models.ActionResponse<List<ValueTextItem>> ar = new ActionResponse<List<ValueTextItem>>();

            List<ValueTextItem> lst = new List<ValueTextItem>();

            try
            {
                //lst.Add(new ValueTextItem() { Value = "", Text = "All" });

                // CM Group id = 4.
                var q = db.Seller_User.Where(c => c.GroupID == 1).AsEnumerable();

                foreach (var item in q)
                {
                    //列出所有的 e-mail
                    lst.Add(new ValueTextItem() { Value = item.UserID.ToString(), Text = item.UserEmail });
                }

                ar.Code = (int)ResponseCode.Success;
                ar.IsSuccess = true;
                ar.Msg = "Success";
                ar.Body = lst;
            }
            catch (Exception ex)
            {
                ar.Code = (int)ResponseCode.Error;
                ar.IsSuccess = false;
                ar.Msg = ex.Message;
            }
            return ar;

        }

        //＊ 狀態: Active / Inactive / Close  三種就好。
        public List<ValueTextItem> GetStatusList() //Status
        {
            return new List<ValueTextItem>()
            {
                new ValueTextItem() { Value = "A", Text = "Active" },
                new ValueTextItem() { Value = "I", Text = "Inactive" },
                new ValueTextItem() { Value = "C", Text = "Closed" }

                //new ValueTextItem() { Value = "1", Text = "New" },
                //new ValueTextItem() { Value = "2", Text = "Active" },
                //new ValueTextItem() { Value = "3", Text = "Suspended" },
                //new ValueTextItem() { Value = "4", Text = "Inactive" },
                //new ValueTextItem() { Value = "5", Text = "Terminated" },
                //new ValueTextItem() { Value = "6", Text = "Closed" }
            };
        }

        public List<ValueTextItem> GetTypeList() //Type
        {
            return new List<ValueTextItem>()
            {
                new ValueTextItem() { Value = "1", Text = "All" },
                new ValueTextItem() { Value = "2", Text = "Domestic Seller" },
                new ValueTextItem() { Value = "3", Text = "International Seller" }
            };
        }

        public List<ValueTextItem> GetDateList() //Date
        {
            return new List<ValueTextItem>()
            {
                new ValueTextItem() { Value = "1", Text = "All" },
                new ValueTextItem() { Value = "2", Text = "Today" },
                new ValueTextItem() { Value = "3", Text = "Last 3 Days" },
                new ValueTextItem() { Value = "4", Text = "Last 7 Days" },
                new ValueTextItem() { Value = "5", Text = "Last 30 Days" },
                new ValueTextItem() { Value = "6", Text = "Specified Date" }
            };
        }

        //TWSQLDB.country
        public API.Models.ActionResponse<List<ValueTextItem>> GetRegionList() //Region
        {
            API.Models.ActionResponse<List<ValueTextItem>> ar = new ActionResponse<List<ValueTextItem>>();

            List<ValueTextItem> lst = new List<ValueTextItem>();

            //lst.Add(new ValueTextItem() { Value = "", Text = "All" });

            try
            {
                var q = sqldb.Country.AsEnumerable();

                foreach (var item in q)
                {
                    lst.Add(new ValueTextItem() { Value = item.ID.ToString(), Text = item.Name });
                }

                ar.Code = (int)ResponseCode.Success;
                ar.IsSuccess = true;
                ar.Msg = "Success";
                ar.Body = lst;
            }
            catch (Exception ex)
            {
                ar.Code = (int)ResponseCode.Error;
                ar.IsSuccess = false;
                ar.Msg = ex.Message;
            }
            return ar;

        }

        public object GetFTPList() //FTP
        {
            return new List<ValueTextItem>()
            {
                new ValueTextItem() { Value = "1", Text = "Inital" },
                new ValueTextItem() { Value = "2", Text = "Enabled" },
                new ValueTextItem() { Value = "3", Text = "Disabled" }
            };
        }

        #endregion

        public ActionResponse<List<TWNewEgg.API.Models.DomainModel.AutoCompleteModel>> getAll_SellerBasic()
        {
            TWNewEgg.DB.TWSellerPortalDBContext spdb = new DB.TWSellerPortalDBContext();

            ActionResponse<List<TWNewEgg.API.Models.DomainModel.AutoCompleteModel>> result = new ActionResponse<List<TWNewEgg.API.Models.DomainModel.AutoCompleteModel>>();

            var allSeller_BasicInfo = spdb.Seller_BasicInfo.Select(r => 
                new TWNewEgg.API.Models.DomainModel.AutoCompleteModel { 
                        sellerName = r.SellerName,
                        sellerid = r.SellerID,
                        SellerStatus = r.SellerStatus,
                        AccountTypeCode = r.AccountTypeCode
                    }).ToList();

            try
            {
                if (allSeller_BasicInfo == null)
                {
                    result.IsSuccess = false;
                    result.Code = (int)ResponseCode.Error;
                    result.Msg = "取得 Vendor 資料失敗";
                    result.Body = null;
                }
                else
                {
                    result.IsSuccess = true;
                    result.Code = (int)ResponseCode.Success;
                    result.Msg = "取得 Vendor 資料成功";
                    result.Body = allSeller_BasicInfo;
                }
            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.Code = (int)ResponseCode.Error;
                result.Msg = "取得 Vendor 資料失敗";
                result.Body = null;
                log.Error("Msg: " + ex.Message + ", " + ex.StackTrace);
            }

            return result;
        }

        /// <summary>
        /// 設定 ResponseCode
        /// </summary>
        /// <param name="isSuccess">成功失敗訊息</param>
        /// <returns>ResponseCode</returns>
        private int SetResponseCode(bool isSuccess)
        {
            if (isSuccess)
            {
                return (int)ResponseCode.Success;
            }
            else
            {
                return (int)ResponseCode.Error;
            }
        }

        #region 商家關係維護與管理修改資料(拉回相關要修改的資料)
        public ActionResponse<string> sellerrelationshipmanagement_Edit(List<TWNewEgg.DB.TWSELLERPORTALDB.Models.Seller_BasicInfo> editData, string userid)
        {
            ActionResponse<string> result = new ActionResponse<string>();
            #region 初始化回傳值
            result.IsSuccess = true;
            result.Msg = string.Empty;
            #endregion
            #region 利用傳過來資料的 sellerid 抓取對應要的資料
            List<TWNewEgg.DB.TWSELLERPORTALDB.Models.Seller_BasicInfo> sellerInfoListToUpdate_modified = new List<DB.TWSELLERPORTALDB.Models.Seller_BasicInfo>();
            try
            {
                foreach (var item in editData)
                {
                    var editTempDate = this.getSeller_BasicInfo_Data(item.SellerEmail, item.AccountTypeCode, item.SellerID);
                    if (editTempDate.IsSuccess == false)
                    {
                        result.IsSuccess = false;
                        result.Msg = editTempDate.Msg;
                        break;
                    }
                    else
                    {
                        #region 畫面上可以修改的欄位
                        editTempDate.Body.Identy = item.Identy;
                        editTempDate.Body.SellerStatus = item.SellerStatus;
                        editTempDate.Body.Currency = item.Currency;
                        editTempDate.Body.BillingCycle = item.BillingCycle == 0 ? null : item.BillingCycle;
                        //editTempDate.Body.BillingCycle = item.BillingCycle;
                        editTempDate.Body.CompanyCode = item.CompanyCode;
                        editTempDate.Body.UpdateUserID = Convert.ToInt16(userid);
                        editTempDate.Body.UpdateDate = DateTime.Now;
                        sellerInfoListToUpdate_modified.Add(editTempDate.Body);
                        #endregion
                    }
                }
            }
            catch (Exception error)
            {
                result.IsSuccess = false;
                log.Error("[Message]: " + error.Message + " ;[StackTrace]: " + error.StackTrace);
                result.Msg = "資料錯誤";
            }
            #endregion
            if (result.IsSuccess == false)
            {
                return result;
            }
            using (System.Transactions.TransactionScope scope = new System.Transactions.TransactionScope())
            {
                #region 修改 seller portal 的 表
                ActionResponse<string> EditResult = this.sellerrelationshipmanagement_Edit_InserDB(sellerInfoListToUpdate_modified);
                if (EditResult.IsSuccess == false)
                {
                    result.IsSuccess = false;
                    result.Msg = EditResult.Msg;
                }
                #endregion
                else
                {
                    #region 更新前台表
                    TWNewEgg.API.Service.TWService twService = new TWService();
                    ActionResponse<string> FrontSellerUpdate = this.sellerrelationshipmanagement_Front_Seller(sellerInfoListToUpdate_modified);
                    if (FrontSellerUpdate.IsSuccess == false)
                    {
                        result.IsSuccess = false;
                        result.Msg = FrontSellerUpdate.Msg;
                    }
                    else
                    {
                        result.IsSuccess = true;
                        result.Msg = "修改成功";
                    }
                    #endregion
                }
                if (result.IsSuccess == false)
                {
                    scope.Dispose();
                }
                else
                {
                    scope.Complete();
                }

            }
            return result;
        }
        #endregion
        #region 商家關係維護與管理修改資料(開始把資料寫入資料庫(sellerPortal))
        public ActionResponse<string> sellerrelationshipmanagement_Edit_InserDB(List<TWNewEgg.DB.TWSELLERPORTALDB.Models.Seller_BasicInfo> itemList)
        {
            ActionResponse<string> result = new ActionResponse<string>();
            TWNewEgg.DB.TWSellerPortalDBContext spdb = new DB.TWSellerPortalDBContext();
            foreach (var item in itemList)
            {
                item.UpdateDate = DateTime.Now;
                //db.Entry(item).State = EntityState.Modified;
                spdb.Entry(item).State = EntityState.Modified;
            }
            try
            {
                spdb.SaveChanges();
                result.IsSuccess = true;
            }
            catch (Exception error)
            {
                log.Error("[Message]: " + error.Message + " ;[StackTrace]:" + error.StackTrace);
                result.IsSuccess = false;
                result.Msg = "資料錯誤";
            }
            return result;
        }
        #endregion
        #region 商家關係維護與管理修改資料(開始把資料寫入資料庫(前台的 seller))
        public ActionResponse<string> sellerrelationshipmanagement_Front_Seller(List<TWNewEgg.DB.TWSELLERPORTALDB.Models.Seller_BasicInfo> itemList)
        {
            ActionResponse<string> result = new ActionResponse<string>();
            #region 給予回傳值預設值
            result.IsSuccess = true;
            result.Msg = "修改成功";
            #endregion
            TWNewEgg.API.Service.TWService twService = new TWService();
            using (System.Transactions.TransactionScope scope = new System.Transactions.TransactionScope())
            {
                foreach (var item in itemList)
                {
                    string SellerEmail = string.Empty;
                    string accountType = string.Empty;
                    SellerEmail = item.SellerEmail;
                    accountType = item.AccountTypeCode;
                    ActionResponse<string> updaDateResult = twService.UpdateTWSeller(SellerEmail, accountType);
                    if (updaDateResult.IsSuccess == false)
                    {
                        result.IsSuccess = false;
                        result.Msg = updaDateResult.Msg;
                        break;
                    }
                }
                if (result.IsSuccess == false)
                {
                    scope.Dispose();
                }
                else
                {
                    scope.Complete();
                }
            }
            return result;
        }
        #endregion
        #region 抓取要修改的資料
        public ActionResponse<Seller_BasicInfo> getSeller_BasicInfo_Data(string Email, string accountTypeCode, int sellerid)
        {
            TWNewEgg.DB.TWSellerPortalDBContext spdb = new DB.TWSellerPortalDBContext();
            Seller_BasicInfo Seller_BasicInfoReturnData = new Seller_BasicInfo();
            ActionResponse<Seller_BasicInfo> result = new ActionResponse<Seller_BasicInfo>();

            try
            {
                Seller_BasicInfoReturnData = spdb.Seller_BasicInfo.Where(p => p.SellerEmail == Email && p.AccountTypeCode == accountTypeCode && p.SellerID == sellerid).FirstOrDefault();
                if (Seller_BasicInfoReturnData == null)
                {
                    result.IsSuccess = false;
                    result.Msg = "無此筆資料";
                }
                else
                {
                    result.IsSuccess = true;
                    result.Body = Seller_BasicInfoReturnData;
                }
            }
            catch (Exception error)
            {
                result.IsSuccess = false;
                result.Msg = "資料錯誤";
                log.Error("[Message]: " + error.Message + " ;[StackTrace]: " + error.StackTrace);
            }
            return result;
        }
        #endregion
        /// <summary>
        /// 取得 Exception 錯誤訊息
        /// </summary>
        /// <param name="ex">Exception 內容</param>
        /// <returns>Exception 錯誤訊息</returns>
        private string GetExceptionMessage(System.Exception ex)
        {
            string errorMessage = string.Empty;

            if (ex.Message.IndexOf("See the inner exception for details.") != -1)
            {
                errorMessage = ex.InnerException.Message;

                if (errorMessage.IndexOf("See the inner exception for details.") != -1)
                {
                    errorMessage = GetExceptionMessage(ex.InnerException);
                }
            }
            else
            {
                errorMessage = ex.Message;
            }

            return errorMessage;
        }
    }
}
