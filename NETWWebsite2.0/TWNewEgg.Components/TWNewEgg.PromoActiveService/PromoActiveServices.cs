using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TWNewEgg.PromoActiveService.Interface;
using TWNewEgg.Models.DomainModels.PromoActive;
using TWNewEgg.Models.DomainModels.Message;
using TWNewEgg.Models.DBModels.TWSQLDB;
using TWNewEgg.PromoActiveRepoAdapters.Interface;
using TWNewEgg.Framework.AutoMapper;
using TWNewEgg.CategoryRepoAdapters;
using TWNewEgg.CategoryRepoAdapters.Interface;
using TWNewEgg.PromoAwardLogRepoAdapters.Interface;
using TWNewEgg.PromoAwardLogRepoAdapters;
using TWNewEgg.Framework.ServiceApi;


namespace TWNewEgg.PromoActiveService
{
    public class PromoActiveServices : IPromoActiveService
    {
        private IPromoActiveRepoAdapters _promoActiveRepoAdapters;
        private ICategoryRepoAdapter _categoryRepoAdapter;
        private IPromoAwardLogRepoAdapters _promoAwardLogRepoAdapters;

        public PromoActiveServices(IPromoActiveRepoAdapters promoActiveRepoAdapters, ICategoryRepoAdapter categoryRepoAdapter, IPromoAwardLogRepoAdapters promoAwardLogRepoAdapters)
        {
            this._promoActiveRepoAdapters = promoActiveRepoAdapters;
            this._categoryRepoAdapter = categoryRepoAdapter;
            this._promoAwardLogRepoAdapters = promoAwardLogRepoAdapters;
        }

        public List<TWNewEgg.Models.DomainModels.PromoActive.PromoActiveDM> GetAllPromoActiveDM(int pageNumber = 1, int pageDataNumber = 10, string activityType = "Newest")
        {
            DateTime dateNow = DateTime.Now;
            int totalCount = 0;
            //計算需要略過的資料筆數
            int skipNumber = (pageNumber - 1) * pageDataNumber;
            //查詢出所有活動的資料
            var PromoActiveData = _promoActiveRepoAdapters.GetAllPromoActive();
            //只選取上線的資料(即 status == 1)
            PromoActiveData = PromoActiveData.Where(p => p.Status == 1);
            //只選 FunctionType = 0 or 1 即 Both 和 優惠活動
            PromoActiveData = PromoActiveData.Where(p => p.FuncType == 0 || p.FuncType == 1);
            //判斷是否有資料 沒有資料直接回傳 null
            if (PromoActiveData.Any() == false)
            {
                return null;
            }
            IEnumerable<PromoActive> totalResult = null;
            //過期但未滿7天
            var SoonFinishProcessResult = this.ProcessSevenDates(PromoActiveData);
            //超過固定字數必須換行
            //var SoonFinishProcessAddBrResult = this.ProcessAddBr(SoonFinishProcessResult);
            //totalResult = SoonFinishProcessAddBrResult;
            totalResult = SoonFinishProcessResult;
            #region 20151126
            ////最快結束的活動排在前面
                       // totalCount = PromoActiveData.OrderBy(p => p.EndDate).Count();
                       // PromoActiveData = PromoActiveData.OrderBy(p => p.EndDate).Skip(skipNumber).Take(pageDataNumber);

                        //break;
                    //}
                //case "Newest":
                    //{
                        //var SoonFinishProcessResult = this.ProcessSevenDates(PromoActiveData);
                        //var SoonFinishProcessAddBrResult = this.ProcessAddBr(SoonFinishProcessResult);
                        //totalResult = SoonFinishProcessAddBrResult;

                        //totalCount = PromoActiveData.OrderByDescending(p => p.StartDate).Count();
                        //PromoActiveData = PromoActiveData.OrderByDescending(p => p.StartDate).Skip(skipNumber).Take(pageDataNumber);
                        //break;
                    //}
                //default:
                    //
                        //break;
                    //}
            //}
            //轉換 DB MODEL 為 DM MODEL
            //totalResult
            #endregion
            //查詢沒有符合條件的資料
            if (totalResult.Any() == false)
            {
                return null;
            }
            List<TWNewEgg.Models.DomainModels.PromoActive.PromoActiveDM> convertPromoActiveData = new List<PromoActiveDM>();
            convertPromoActiveData = TWNewEgg.Framework.AutoMapper.ModelConverter.ConvertTo<List<TWNewEgg.Models.DomainModels.PromoActive.PromoActiveDM>>(totalResult);
            totalCount = convertPromoActiveData.Count();
            //即將結束
            if (activityType == "SoonFinish")
            {   
                //totalCount = convertPromoActiveData.Count();
                //最快結束得活動排在最前面
                convertPromoActiveData = convertPromoActiveData.OrderBy(p => p.EndDate).Skip(skipNumber).Take(pageDataNumber).ToList();
                //借用第 0 筆的資料回傳總共有幾筆資料量
                //convertPromoActiveData[0].totalDataCount = totalCount;
                //return convertPromoActiveData;
            }
            else//最新活動
            {                
                //totalCount = convertPromoActiveData.Count();
                //最晚開始的活動排在最前面
                convertPromoActiveData = convertPromoActiveData.OrderByDescending(p => p.StartDate).Skip(skipNumber).Take(pageDataNumber).ToList();
                //借用第 0 筆的資料回傳總共有幾筆資料量
                //convertPromoActiveData[0].totalDataCount = totalCount;
                //return convertPromoActiveData;
            }
            convertPromoActiveData[0].totalDataCount = totalCount;
            return convertPromoActiveData;
            #region 20151126
            //List<TWNewEgg.Models.DomainModels.PromoActive.PromoActiveDM> removePromoActiveDMTemp = new List<PromoActiveDM>();
            ////利用第0筆的資料暫放所有資料的比數
            //convertPromoActiveData[0].totalDataCount = totalCount;
            ////超過長度 10 的必須跳行
            //convertPromoActiveData.ForEach(p =>
            //{
            //    if (string.IsNullOrEmpty(p.Name) == false)
            //    {
            //        if (p.Name.Length > 10)
            //        {
            //            p.Name = this.strSubByNumber(p.Name, 10);
            //        }
            //    }
            //    if (string.IsNullOrEmpty(p.Description) == false)
            //    {
            //        if (p.Description.Length > 10)
            //        {
            //            p.Description = this.strSubByNumber(p.Description, 10);
            //        }
            //    }
            //    if (p.EndDate.Value.AddDays(7) < dateNow)
            //    {
            //        removePromoActiveDMTemp.Add(p);
            //    }
            //});
            //convertPromoActiveData = convertPromoActiveData.Except(removePromoActiveDMTemp).ToList();
            //if (convertPromoActiveData.Count == 0 || convertPromoActiveData == null)
            //{
            //    convertPromoActiveData[0].totalDataCount = 0;
            //}
            //else
            //{
            //    convertPromoActiveData[0].totalDataCount = totalCount;
            //}
            //return convertPromoActiveData;
            #endregion
        }
        #region 過期但是未滿 7 天
        /// <summary>
        /// 過期但是未滿 7 天
        /// </summary>
        /// <param name="processModel"></param>
        /// <returns></returns>
        public IEnumerable<PromoActive> ProcessSevenDates(IEnumerable<PromoActive> processModel)
        {
            DateTime dateNow = DateTime.Now;
            IEnumerable<PromoActive> returnModel = null;
            //活動結束 D + 7 天(不含) 後的資料不顯示於畫面上
            //結束時間 + 7 天 小於現在時間 表示此活動已經結束超過 7 天 所以要移除
            var removePromoActiveDMTemp = processModel.Where(p => p.EndDate.Value.AddDays(7) < dateNow);
            //移除結束時間超過現在時間 7 天的活動
            processModel = processModel.Except(removePromoActiveDMTemp);
            returnModel = processModel;
            return returnModel;
        }
        #endregion 
        #region 根據條件加入換行符號
        public IEnumerable<PromoActive> ProcessAddBr(IEnumerable<PromoActive> processModel)
        {
            IEnumerable<PromoActive> returnModel = null;
            //針對活動名稱和活動贈品進行是否加入跳行符號
            foreach (var item in processModel)
            {
                //不為空(活動名稱)
                if (string.IsNullOrEmpty(item.Name) == false)
                {
                    //長度大於限制 15
                    if (item.Name.Length > 15)
                    {
                        item.Name = this.strSubByNumber(item.Name, 15);
                    }
                }
                //不為空(活動贈品)
                if (string.IsNullOrEmpty(item.Description) == false)
                {
                    //長度大於限制 15
                    if (item.Description.Length > 15)
                    {
                        item.Description = this.strSubByNumber(item.Description, 15);
                    }
                }
            }
            returnModel = processModel;
            return returnModel;
        }
        #endregion
        #region 在字串中加入換行符號
        /// <summary>
        /// 在字串中加入換行符號
        /// </summary>
        /// <param name="strTemp"></param>
        /// <param name="subNumber"></param>
        /// <returns></returns>
        public string strSubByNumber(string strTemp, int subNumber = 0)
        {
            if (string.IsNullOrEmpty(strTemp) == true)
            {
                return "";
        }
            if (subNumber == 0)
        {
                return "";
            }
            int strTempLength = strTemp.Length;
            int Quotient = strTempLength / subNumber;
            string str_Result = string.Empty;
            for (int i = 0; i < Quotient; i++)
            {
                string strTemptemp = strTemp.Substring(0, subNumber);
                str_Result = str_Result + strTemptemp + "<br>";
                strTemp = strTemp.Remove(0, subNumber);
            }
            str_Result = str_Result + strTemp;
            return str_Result;
        }
        #endregion
        //public string CategoryName(int categoryid, IQueryable<Category> getcategotyAll)
        //{
        //    string result = string.Empty;
        //    result = getcategotyAll.Where(p => p.ID == categoryid).Select(p => p.Description).FirstOrDefault();
        //    if (result == null)
        //    {
        //        return string.Empty;
        //    }
        //    return result;
        //}

        #region IPP

        /// <summary>
        /// 搜尋行銷活動 (IPP 行銷活動與中獎公告搜尋)
        /// </summary>
        /// <param name="searchPromoModel">搜尋絛件</param>
        /// <returns>行銷活動清單</returns>
        public ResponsePacket<List<PromoActiveDM>> PromoActiveSearch(TWNewEgg.Models.DomainModels.PromoActive.SearchPromoModel searchPromoModel)
        {
            #region 變數宣告

            ResponsePacket<List<PromoActiveDM>> result = new ResponsePacket<List<PromoActiveDM>>();
            result.results = new List<PromoActiveDM>();
            result.error = null;
            
            // 查詢條件
            IQueryable<PromoActive> promoActive_Queryable;

            // 行銷活動清單 (DB model)
            List<PromoActive> promoActiveCell = new List<PromoActive>();

            #endregion 變數宣告

            #region 設定搜尋條件

            try
            {
                promoActive_Queryable = _promoActiveRepoAdapters.GetAllPromoActive();
                
                // 依公佈類別(行銷活動或中獎名單)篩選
                if (searchPromoModel.FuncType != 0)
                {
                    promoActive_Queryable = promoActive_Queryable.Where(x => x.FuncType == searchPromoModel.FuncType || x.FuncType == 0);
                }

                // 依行銷活動編號篩選
                if (searchPromoModel.ID != 0)
                {
                    promoActive_Queryable = promoActive_Queryable.Where(x => x.ID == searchPromoModel.ID);
                }

                // 依活動開始日期篩選
                if (searchPromoModel.Search_StartDate != null)
                {
                    promoActive_Queryable = promoActive_Queryable.Where(x => x.StartDate >= searchPromoModel.Search_StartDate);
                }

                // 依活動結束日期篩選
                if (searchPromoModel.Search_EndDate != null)
                {
                    promoActive_Queryable = promoActive_Queryable.Where(x => x.EndDate <= searchPromoModel.Search_EndDate);
                }

                // 依活動名稱篩選
                if (!string.IsNullOrEmpty(searchPromoModel.Search_KeyWord))
                {
                    searchPromoModel.Search_KeyWord = searchPromoModel.Search_KeyWord.Trim();
                    promoActive_Queryable = promoActive_Queryable.Where(x => x.Name.IndexOf(searchPromoModel.Search_KeyWord) != -1);
                }
            }
            catch (Exception exception)
            {
                result.results = null;
                result.error = string.Format("設定 PromoActive 的查詢條件失敗 (exception); ExceptionMessage = {0}.", GetInnerExceptionMessage(exception));
                return result;
            }

            #endregion 設定搜尋條件

            #region 讀取 DB 資料

            // 讀取 DB 資料
            try
            {
                promoActiveCell = promoActive_Queryable.ToList();
            }
            catch (Exception exception)
            {
                result.results = null;
                result.error = string.Format("讀取 PromoActive 失敗 (exception); ExceptionMessage = {0}.", GetInnerExceptionMessage(exception));
                return result;
            }

            #endregion 讀取 DB 資料

            #region 轉為 DomainModel

            // 轉換 DBModel 到 DomainModel
            try
            {
                result.results = TWNewEgg.Framework.AutoMapper.ModelConverter.ConvertTo<List<TWNewEgg.Models.DomainModels.PromoActive.PromoActiveDM>>(promoActiveCell);
            }
            catch (Exception exception)
            {
                result.results = null;
                result.error = string.Format("轉換 DBModel 到 DomainModel 失敗 (exception); ExceptionMessage = {0}.", GetInnerExceptionMessage(exception));
                return result;
            }

            #endregion 轉為 DomainModel

            #region 判斷是否已上傳中獎名單

            foreach (PromoActiveDM promoActiveDM in result.results)
            {
                List<PromoAwardLog> promoAwardLogList = _promoAwardLogRepoAdapters.GetPromoAwardLogList(promoActiveDM.ID).ToList();

                if (promoAwardLogList == null)
                {
                    promoActiveDM.IsImport = false;
                }
                else
                {
                    if (promoAwardLogList.Count() > 0)
                    {
                        promoActiveDM.IsImport = true;
                    }
                    else
                    {
                        promoActiveDM.IsImport = false;
                    }
                }
            }
        
            #endregion 判斷是否已上傳中獎名單

            return result;
        }
        /// <summary>
        /// 後台設定，新增或更新單筆行銷活動跟中獎公告 (IPP 行銷活動與中獎公告設定)
        /// </summary>
        /// <param name="promoActiveDM"></param>
        /// <returns></returns>
        public PromoActiveDM StorePromoActiveDetail(TWNewEgg.Models.DomainModels.PromoActive.PromoActiveDM promoActiveDM)
        {
            PromoActiveDM result = new PromoActiveDM();
            PromoActive promoActiveresult = new PromoActive();
            //撈出要修改的單筆資料
            PromoActive promoActivetemp = _promoActiveRepoAdapters.GetPromoActive(promoActiveDM.ID);
            //判斷資料是否存在於table裡
            if (promoActivetemp == null)
            {
                PromoActive newpromoActivetemp = new PromoActive();
                //autoMap
                ModelConverter.ConvertTo<PromoActiveDM, PromoActive>(promoActiveDM, newpromoActivetemp);
                newpromoActivetemp.CreateUser = promoActiveDM.UpdateUser;
                newpromoActivetemp.CreateDate = DateTime.UtcNow.AddHours(8);
                newpromoActivetemp.UpdateDate = DateTime.UtcNow.AddHours(8);
                //活動與館別連結為非必填所以可能為null但DB不可存null
                newpromoActivetemp.NameLink = newpromoActivetemp.NameLink == null ? "" : newpromoActivetemp.NameLink;
                newpromoActivetemp.CategoryLink = newpromoActivetemp.CategoryLink == null ? "" : newpromoActivetemp.CategoryLink;
                promoActiveresult = _promoActiveRepoAdapters.CreatePromoActiveDetail(newpromoActivetemp);
            }
            else
            {
                int taketypetemp = (int)promoActiveDM.TakeType;
                if (promoActiveDM.TakeType == 0)
                {
                    taketypetemp = (int)promoActivetemp.TakeType;
                }
                //類型與FuncType一新增完就不可再修改，但以防萬一所以先撈DB已存的資料，再塞回給即將要存的資料裡
                int typetemp = promoActivetemp.Type;
                int funcTypetemp = promoActivetemp.FuncType;
                //autoMap
                ModelConverter.ConvertTo<PromoActiveDM, PromoActive>(promoActiveDM, promoActivetemp);
                promoActivetemp.UpdateDate = DateTime.UtcNow.AddHours(8);
                //活動與館別連結為非必填所以可能為null但DB不可存null
                promoActivetemp.NameLink = promoActiveDM.NameLink == null ? "" : promoActiveDM.NameLink;
                promoActivetemp.CategoryLink = promoActiveDM.CategoryLink == null ? "" : promoActiveDM.CategoryLink;
                //把上面剛暫存起來的資料塞回給即將要存的資料
                promoActivetemp.Type = typetemp;
                promoActivetemp.FuncType = funcTypetemp;
                promoActivetemp.TakeType = taketypetemp;
                promoActiveresult = _promoActiveRepoAdapters.UpdatePromoActiveDetail(promoActivetemp);
            }
            result = ModelConverter.ConvertTo<PromoActiveDM>(promoActiveresult);
            return result;
        }

        #endregion

        #region 中獎名單

        #region 讀取中獎名單頁的活動清單

        /// <summary>
        /// 取得中獎名單頁的活動清單
        /// </summary>
        /// <param name="searchCondition">搜尋條件</param>
        /// <returns>中獎名單頁的活動清單</returns>
        public ResponsePacket<TWNewEgg.Models.DomainModels.PromoActive.AwardDM> GetAwardList(TWNewEgg.Models.DomainModels.PromoActive.AwardListSearchConditionDM searchCondition)
        {
            #region 變數宣告

            ResponsePacket<TWNewEgg.Models.DomainModels.PromoActive.AwardDM> result = new ResponsePacket<TWNewEgg.Models.DomainModels.PromoActive.AwardDM>();
            result.results = new TWNewEgg.Models.DomainModels.PromoActive.AwardDM();
            result.error = null;

            // 行銷活動清單
            ResponsePacket<List<PromoActiveDM>> getPromoActiveCell = new ResponsePacket<List<PromoActiveDM>>();

            // 行銷活動搜尋絛件
            SearchPromoModel searchPromoModel = new SearchPromoModel();

            // 設定行銷活動搜尋絛件，只列出在中將名單頁顯示的行銷活動
            searchPromoModel.FuncType = 2;

            #endregion 變數宣告

            #region 取得行銷活動清單

            getPromoActiveCell = PromoActiveSearch(searchPromoModel);

            // 若取得行銷活動清單失敗，則直接回傳失敗訊息
            if (string.IsNullOrEmpty(getPromoActiveCell.error) == false || getPromoActiveCell.results == null)
            {
                result.results = null;
                result.error = getPromoActiveCell.error;
                return result;
            }
            else if(getPromoActiveCell.results.Count == 0)
            {
                // 若查無行銷活動，則直接回傳
                return result;
            }
            
            #endregion 取得行銷活動清單

            #region 依中獎名單畫面顯示需求篩選

            // 只列出後台設定為顯示的
            getPromoActiveCell.results = getPromoActiveCell.results.Where(x => x.Status == 1).ToList();

            // 活動結束超過 45 天的不顯示
            getPromoActiveCell.results = getPromoActiveCell.results.Where(x => x.EndDate.HasValue && x.EndDate.Value.AddDays(45) > DateTime.Today).ToList();

            // 若篩選結果已為 0 筆，則直接回傳
            if(getPromoActiveCell.results.Count == 0)
            {
                return result;
            }

            #endregion 依中獎名單畫面顯示需求篩選

            #region 依使用者所選的排序項目排序

            switch (searchCondition.OrderBy)
            {
                default:
                case (int)ActivityListOrderByTypeDM.最新活動:
                    {
                        getPromoActiveCell.results = getPromoActiveCell.results.OrderByDescending(x => x.StartDate).ToList();
                        break;
                    }
                case (int)ActivityListOrderByTypeDM.即將結束:
                    {
                        getPromoActiveCell.results = getPromoActiveCell.results.OrderBy(x => x.EndDate).ToList();
                        break;
                    }
            }

            #endregion 依使用者所選的排序項目排序

            #region 計算總分頁數並取指定分頁的資料

            #region 計算總分頁數

            result.results.MaxPage = getPromoActiveCell.results.Count / 10;

            // 若活動清單資料筆數無法被 10 整除，則中獎名單頁最大頁數 +1
            if ((getPromoActiveCell.results.Count % 10) != 0)
            {
                result.results.MaxPage++;
            }

            #endregion 計算總分頁數

            #region 讀取指定分頁的資料

            result.results.ActivityList = getPromoActiveCell.results.Skip((searchCondition.PageIndex - 1) * 10).Take(10).ToList();

            #endregion 讀取指定分頁的資料

            #endregion 計算總分頁數並取指定分頁的資料

            #region 組合顯示資料

            result.results.OrderBy = searchCondition.OrderBy;
            result.results.PageIndex = searchCondition.PageIndex;

            foreach (PromoActiveDM promoActiveDM in result.results.ActivityList)
            {
                #region 館別連結、活動連結，只保留到活動結束 D+7 天

                if (promoActiveDM.EndDate.HasValue == false || promoActiveDM.EndDate.Value.Date.AddDays(7) < DateTime.Today)
                {
                    promoActiveDM.CategoryLink = null;
                    promoActiveDM.NameLink = null;
                }

                #endregion 館別連結、活動連結，只保留到活動結束 D+7 天

                #region 填寫行銷活動類型名稱

                switch (promoActiveDM.Type)
                {
                    case (int)TWNewEgg.Models.DBModels.TWSQLDB.PromoActive.PromoActiveType.滿額折:
                        {
                            promoActiveDM.TypeName = TWNewEgg.Models.DBModels.TWSQLDB.PromoActive.PromoActiveType.滿額折.ToString() + "(現折)";
                            break;
                        }
                    case (int)TWNewEgg.Models.DBModels.TWSQLDB.PromoActive.PromoActiveType.贈獎:
                        {
                            promoActiveDM.TypeName = TWNewEgg.Models.DBModels.TWSQLDB.PromoActive.PromoActiveType.贈獎.ToString() + "(獎品)";
                            break;
                        }
                    case (int)TWNewEgg.Models.DBModels.TWSQLDB.PromoActive.PromoActiveType.折價券:
                    case (int)TWNewEgg.Models.DBModels.TWSQLDB.PromoActive.PromoActiveType.回饋金:
                    case (int)TWNewEgg.Models.DBModels.TWSQLDB.PromoActive.PromoActiveType.紅利點數:
                    case (int)TWNewEgg.Models.DBModels.TWSQLDB.PromoActive.PromoActiveType.抽獎:
                    case (int)TWNewEgg.Models.DBModels.TWSQLDB.PromoActive.PromoActiveType.銀行:
                    case (int)TWNewEgg.Models.DBModels.TWSQLDB.PromoActive.PromoActiveType.折扣:
                        {
                            promoActiveDM.TypeName = Enum.GetName(typeof(TWNewEgg.Models.DBModels.TWSQLDB.PromoActive.PromoActiveType), promoActiveDM.Type);
                            break;
                        }
                    default:
                        {
                            break;
                        }
                }

                #endregion 填寫行銷活動類型名稱

                #region 填寫領獎方式名稱

                if (promoActiveDM.TakeType != null)
                {
                    promoActiveDM.TakeTypeName = Enum.GetName(typeof(TWNewEgg.Models.DBModels.TWSQLDB.PromoActive.PromoActiveTakeType), promoActiveDM.TakeType.Value);
                }

                #endregion 填寫領獎方式名稱
            }

            #endregion 組合顯示資料

            return result;
        }

        #endregion 讀取中獎名單頁的活動清單

        /// <summary>
        /// 取得 Exception 錯誤訊息
        /// </summary>
        /// <remarks>解決 Exception message 出現 See the inner exception for details. 時，無法得知 Inner exception message 的問題。</remarks>
        /// <param name="exception">Exception 內容</param>
        /// <returns>Exception 錯誤訊息</returns>
        private string GetInnerExceptionMessage(Exception exception)
        {
            // 如果 Exception 的 Message 有 See the inner exception for details. 的話
            if (exception.Message.IndexOf("See the inner exception for details.") != -1)
            {
                // 再檢查是否又有 InnerException
                return GetInnerExceptionMessage(exception.InnerException);
            }
            else
            {
                // 回傳 Exception Message
                return exception.Message;
            }
        }

        #endregion 中獎名單
    }
}
