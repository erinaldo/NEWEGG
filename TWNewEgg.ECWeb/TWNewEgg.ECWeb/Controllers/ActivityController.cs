using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using TWNewEgg.ECWeb.PrivilegeFilters;
using TWNewEgg.Framework.AutoMapper;
using TWNewEgg.Framework.ServiceApi;
using TWNewEgg.Models.DomainModels.Activity;
using TWNewEgg.Models.ViewModels.Activity;

namespace TWNewEgg.ECWeb.Controllers
{
    [AllowNonSecures]
    [AllowAnonymous]
    public class ActivityController : Controller
    {
        private log4net.ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.FullName);

        public ActionResult Deal(string name)
        {
            int number = 0;
            return View(name, number);
        }

        public ActionResult Show(string name)
        {
            
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            ActivitySectionInfor ActivitySectionInforData = new ActivitySectionInfor();
            ActivityVM rltActivityVM = Processor.Request<ActivityVM, ActivityDM>("ActivityService", "GetActivityByName", name).results;

            if (rltActivityVM == null)
            {
                return RedirectToAction("Index", "Home");
            }
            if (rltActivityVM.ShowType == 1 )
            {
                return RedirectToAction("Index", "Home");
            }   
            #region 解析Json ActivitySectionInfor
            rltActivityVM.SectionInfor = rltActivityVM.SectionInfor ?? string.Empty;
            rltActivityVM.SectionInfor = rltActivityVM.SectionInfor.ToLower().Replace(@"""", "").Replace(@"\", "").Replace(@"null", "false");
            ActivitySectionInforData = serializer.Deserialize<ActivitySectionInfor>(rltActivityVM.SectionInfor);
            AutoMapper.Mapper.Map(ActivitySectionInforData, rltActivityVM);
            #endregion
            rltActivityVM.HtmlContext = WebUtility.HtmlDecode(rltActivityVM.HtmlContext);
            

            return View(rltActivityVM);
        }

        public ActionResult ActivityList(/*List<TWNewEgg.Models.ViewModels.PromoActive.PromoActive> showModel*/)
        {

            return View();
        }

        [HttpGet]
        public ActionResult Index(int pageNumber = 1, string activityType = "Newest")
        {
            DateTime NowTime = DateTime.Now;
            //呼叫查詢資料的 API
            var PromotionList = Processor.Request<List<TWNewEgg.Models.ViewModels.PromoActive.PromoActive>, List<TWNewEgg.Models.ViewModels.PromoActive.PromoActive>>("PromoActiveServices", "GetAllPromoActiveDM", pageNumber, 10, activityType);        
            //API 有錯誤, 回首頁
            if (PromotionList.error != null)
            {
                return RedirectToAction("Index", "Home");
            }
            else//API 無錯誤
            {
                //先判斷回傳的 model 是否為空的
                if (PromotionList.results == null)
                {
                    //TWNewEgg.Models.ViewModels.PromoActive.PromoActiveView _promoActiveViewNoData = null;
                    return View(/*_promoActiveViewNoData*/);
                }
                List<TWNewEgg.Models.ViewModels.PromoActive.PromoActive> returnModel = new List<TWNewEgg.Models.ViewModels.PromoActive.PromoActive>();
                returnModel = PromotionList.results;
                #region 修改回傳的 model 裡面的一些資料, 並且判斷活動是否結束、剩下幾天
                returnModel.ForEach(p =>
                {
                    #region 判斷活動是否進行中還是已經結束
                    //活動已經結束
                    if (p.EndDate > NowTime)
                    {
                        p.ActivityOrNot = (int)TWNewEgg.Models.ViewModels.PromoActive.ActivityStatus.進行中;
                        try
                        {
                            //計算活動剩下的天數
                            string timeSpan = p.EndDate.Value.Subtract(DateTime.Now).Duration().Days.ToString();
                            //活動天數小於 1 天, 直接給 1 天
                            if (timeSpan == "0")
                            {
                                p.ActivityRemainDate = "1";
                            }
                            else
                            {
                                p.ActivityRemainDate = timeSpan;
                            }
                        }
                        catch (Exception error)
                        {
                            logger.Error("[Msg]: " + error.Message + " ;[StackTrace]: " + error.StackTrace);
                            p.ActivityRemainDate = string.Empty;
                        }
                    }
                    else if (p.EndDate <= NowTime)
                    {
                        p.ActivityOrNot = (int)TWNewEgg.Models.ViewModels.PromoActive.ActivityStatus.已結束;
                        p.ActivityRemainDate = string.Empty;
                    }
                    else
                    {
                        p.ActivityRemainDate = string.Empty;
                    }
                    #endregion
                });
                #endregion
                #region 紀錄需要分頁的頁數
                int totalPage = 0;
                int totalCount = returnModel == null ? 0 : (returnModel[0].totalDataCount == 0 ? 0 : returnModel[0].totalDataCount);
                //計算需要分頁幾頁
                totalPage = this.countPageNumber(totalCount, 10);
                #endregion
                //目前選的頁數
                ViewBag.selectPage = pageNumber;
                #region 呼叫計算頁數的 service
                TWNewEgg.ECWeb.Services.Page.CalculationsPage calPage = new Services.Page.CalculationsPage();
                List<TWNewEgg.Models.ViewModels.Page.ShowPage> Pagedata = calPage.getShowPages(totalPage, pageNumber);
                #endregion
                #region 回塞需要回傳到 view 的資料到 return model
                TWNewEgg.Models.ViewModels.PromoActive.PromoActiveView _promoActiveView = new TWNewEgg.Models.ViewModels.PromoActive.PromoActiveView();
                _promoActiveView.listPromoActive = returnModel;
                _promoActiveView.showPage = Pagedata;
                _promoActiveView.totalPage = totalPage;
                //紀錄選擇"最新活動"還是"即將結束" 以方便在 view 上面做判斷
                _promoActiveView.searchFrom = activityType;
                #endregion
                //判斷是否是從 ajax 呼叫來的
                if (Request.IsAjaxRequest())
                {
                    return PartialView("ActivityPageView", _promoActiveView);
                }
                return View(_promoActiveView);
            }
            
        }
        /// <summary>
        /// 計算畫面下面的分頁
        /// </summary>
        /// <param name="totalDataCount">總資料量</param>
        /// <param name="pageDataCount">一頁只需要的資料數</param>
        /// <returns></returns>
        public int countPageNumber(int totalCount = 0, int pageDataCount = 0)
        {
            int totalPage = 0;
            int page_quotient = totalCount / pageDataCount;
            int page_remainder = totalCount % pageDataCount;
            //商 = 0 的話表示總資料量小於等於 10 比
            try
            {
                if (page_quotient == 0)
                {
                    totalPage = 1;
                }
                else
                {
                    if (page_remainder == 0)
                    {
                        totalPage = page_quotient;
                    }
                    else
                    {
                        //有餘數的話, 要再多一頁去記錄剩下不到 10 筆資料的頁
                        totalPage = page_quotient + 1;
                    }
                }
            }
            catch (Exception error)
            {
                logger.Error("[Msg]: " + error.Message + " ;[StackTrace]: " + error.StackTrace);
                totalPage = 0;
            }
            return totalPage;
        }
        #region 中獎名單明細
        public ActionResult AwardDetial(string id = "")
        {
            #region 判斷是否有傳入活動 id
            //判斷是否有傳入活動 id 
            if (string.IsNullOrEmpty(id) == true)
            {
                return RedirectToAction("Index", "Home");
            }
            #endregion
            #region 呼叫 service 然後判斷是否有錯誤, 或者是沒有發生錯誤 但是回傳回來的 model 是 null
            var returnModel = Processor.Request<List<TWNewEgg.Models.ViewModels.PromoAwardLog.PromoAwardLog>, List<TWNewEgg.Models.ViewModels.PromoAwardLog.PromoAwardLog>>("PromoAwardLogService", "Detail_PromoAwardLogDM", id);
            //Service 發生錯誤
            if (returnModel.error != null)
            {
                return RedirectToAction("Index", "Home");
            }
            //service 正確但是回傳的 model is null
            if (returnModel.results == null)
            {
                return View();
            }
            #endregion
            #region 組合要回傳在畫面上的 model
            List<TWNewEgg.Models.ViewModels.PromoAwardLog.PromoAwardLogVM> list_PromoAwardLogVM = new List<TWNewEgg.Models.ViewModels.PromoAwardLog.PromoAwardLogVM>();
            try
            {
                //利用 ViewBag.ActivityName 存放活動名稱, 顯示在畫面上
                ViewBag.ActivityName = returnModel.results[0].activityName;
                //組合要回傳顯示在畫面上的 model
                list_PromoAwardLogVM = this.PromoAwardLogVMShow(returnModel.results);
            }
            catch (Exception error)
            {
                logger.Error("[Msg]: " + error.Message + " ;[StackTrace]: " + error.StackTrace);
                ViewBag.ActivityName = string.Empty;
                list_PromoAwardLogVM = null;
            }
            #endregion
            return View(list_PromoAwardLogVM);
        }
        #endregion
        #region 組合要顯示在畫面上的 view model
        public List<TWNewEgg.Models.ViewModels.PromoAwardLog.PromoAwardLogVM> PromoAwardLogVMShow(List<TWNewEgg.Models.ViewModels.PromoAwardLog.PromoAwardLog> list_promoAwardLog = null)
        {
            //判斷是否有接收到 model
            if (list_promoAwardLog == null)
            {
                logger.Error("中獎名單明細畫面沒有傳入 list_promoAwardLog action name: PromoAwardLogVMShow");
                return null;
            }
            #region 開始組合要回傳給畫面並顯示的 model
            List<TWNewEgg.Models.ViewModels.PromoAwardLog.PromoAwardLogVM> listResult = new List<TWNewEgg.Models.ViewModels.PromoAwardLog.PromoAwardLogVM>();
            bool isNOError = true;
            foreach (var item in list_promoAwardLog)
            {
                //先判斷是否有活動名稱, 沒有則直接回傳
                if (string.IsNullOrEmpty(item.AwardName) == true)
                {
                    logger.Error("中獎名單明細的 foreach 中 AwardName 有空值, 這欄位是必填, accountid: " + item.AccountID + ", action name: PromoAwardLogVMShow");
                    //用來記錄是否有活動名稱為空
                    isNOError = false;
                    break;
                }
                string AwardName = item.AwardName.Trim();
                //先判斷在 list 有無這筆資料
                var AwardNameExistOrNot = listResult.Where(p => p.AwardName == AwardName).FirstOrDefault();
                //要回傳的 list model 中沒有 AwardName 這筆資料, 所以必須用新增 
                if (AwardNameExistOrNot == null)
                {
                    //收集 account 的 Email 和 姓名
                    TWNewEgg.Models.ViewModels.PromoAwardLog.CollectEmailAndName _collectEmailAndName = new TWNewEgg.Models.ViewModels.PromoAwardLog.CollectEmailAndName();
                    //收集獎項名稱的 model
                    TWNewEgg.Models.ViewModels.PromoAwardLog.PromoAwardLogVM _promoAwardLogVM = new TWNewEgg.Models.ViewModels.PromoAwardLog.PromoAwardLogVM();
                    //獎項名稱
                    _promoAwardLogVM.AwardName = AwardName;
                    //會員帳號
                    _collectEmailAndName.AccountEmail = this.substringEmail(item.Email);
                    //會員姓名
                    _collectEmailAndName.AccountName = this.ChNameOrEngName(item.ChName,item.EngFirstName, item.EngLastName);
                    _promoAwardLogVM.EmailName.Add(_collectEmailAndName);
                    listResult.Add(_promoAwardLogVM);
                }
                else
                {
                    TWNewEgg.Models.ViewModels.PromoAwardLog.CollectEmailAndName _collectEmailAndName = new TWNewEgg.Models.ViewModels.PromoAwardLog.CollectEmailAndName();
                    //會員帳號
                    _collectEmailAndName.AccountEmail = this.substringEmail(item.Email);
                    //會員姓名
                    _collectEmailAndName.AccountName = this.ChNameOrEngName(item.ChName,item.EngFirstName, item.EngLastName);
                    AwardNameExistOrNot.EmailName.Add(_collectEmailAndName);
                }
            }
            #endregion
            //沒有錯誤才計算中獎比數
            if (isNOError == true)
            {
                //計算獎項數量並回填
                listResult.ForEach(p =>
                {
                    //利用中獎人的數量來計算獎項有多少
                    p.Awardcount = p.EmailName == null ? 0 : p.EmailName.Count;
                });
            }
            return listResult;
        }
        #endregion
        #region 切割 Email 特定長度以上要取代前後字元
        public string substringEmail(string email = "")
        {
            string resultStr = string.Empty;
            if (string.IsNullOrEmpty(email) == true)
            {
                return resultStr;
            }
            try
            {
                //取得 @ 前面的 Email
                string arrayFront = email.Split('@')[0];
                //取得 @ 後面的 domail
                string arrayEnd = email.Split('@')[1];
                if (arrayFront.Length >= 5)
                {
                    resultStr = arrayFront.Substring(2, arrayFront.Length - 4);
                    return "XX" + resultStr + "XX@" + arrayEnd;
                }
                else
                {
                    //如果 mail @ 前面的字串是 1 直接用 X 取代
                    if (arrayFront.Length == 1)
                    {
                        resultStr = "X@" + arrayEnd;
                        return resultStr;
                    }
                    if (arrayFront.Length == 2)
                    {
                        string firstword = arrayFront.Substring(1, 1);
                        resultStr = "X" + firstword + "@" + arrayEnd;
                        return resultStr;
                    }
                    resultStr = arrayFront.Substring(1, arrayFront.Length - 2);
                    return "X" + resultStr + "X@" + arrayEnd;
                }
            }
            catch (Exception error)
            {
                logger.Error("[Msg]: " + error.Message + " ;[StackTrace]: " + error.StackTrace);
            }
            return resultStr;
        }
        #endregion
        #region 中英文名字顯示變更
        /// <summary>
        /// 
        /// </summary>
        /// <param name="ChName">中文名字</param>
        /// <param name="EngFirstName">英文名字</param>
        /// <param name="EngLastName">英文姓氏</param>
        /// <returns></returns>
        public string ChNameOrEngName(string ChName = "", string EngFirstName = "", string EngLastName = "")
        {
            //沒有中文名字, 就利用英文名字, 英文名字則隱藏英文姓氏, 所以直接回傳英文名字
            if (string.IsNullOrEmpty(ChName) == true)
            {
                int EngLastNameInt = EngLastName.Length;
                string replaceEngLastName = new string('X', EngLastNameInt);
                return EngFirstName + " " + replaceEngLastName;
            }
            else
            {
                return this.splitChName(ChName);
            }
        }
        #endregion
        #region 中文名字切名字, 中間替換成 X
        public string splitChName(string ChName = "")
        {
            string perpelName = string.Empty;
            if (string.IsNullOrEmpty(ChName) == true)
            {
                return perpelName;
            }
            try
            {
                if (ChName.Length == 1)
                {
                    perpelName = "X";
                }
                else if (ChName.Length == 2)
                {
                    string firstword = ChName.Substring(0, 1);
                    perpelName = firstword + "X";
                }
                else
                {
                    //取得名字的第一個字
                    string firstWord = ChName.Substring(0, 1);
                    int ChNameLength = ChName.Length;
                    //取的名字的最後一個字
                    string lastWord = ChName.Substring(ChNameLength - 1, 1);
                    string str_result = string.Empty;
                    //計算要取代名字為 X 的字數
                    int ChNameBetweenLength = ChNameLength - 2;
                    str_result = new string('X', ChNameBetweenLength);
                    perpelName = firstWord + str_result + lastWord;
                }
            }
            catch (Exception error)
            {
                logger.Error("[Msg]: " + error.Message + " ;[StackTrace]: " + error.StackTrace);
                perpelName = string.Empty;
            }
            return perpelName;

        }
        #endregion

        #region 中獎名單

        /// <summary>
        /// 中獎名單 View
        /// </summary>
        /// <returns>中獎名單 View</returns>
        public ActionResult AwardList()
        {
            return View();
        }

        /// <summary>
        /// 讀取中獎名單頁活動清單
        /// </summary>
        /// <param name="searchCondition">搜尋條件</param>
        /// <returns>中獎名單頁活動清單</returns>
        [HttpPost]
        public ActionResult GetAwardList(AwardListSearchCondition searchCondition)
        {
            #region 變數宣告

            ResponsePacket<TWNewEgg.Models.ViewModels.Activity.Award> result = new ResponsePacket<TWNewEgg.Models.ViewModels.Activity.Award>();
            result.results = new Award();

            // 搜尋條件的 DomainModel
            TWNewEgg.Models.DomainModels.PromoActive.AwardListSearchConditionDM searchConditionDM = new TWNewEgg.Models.DomainModels.PromoActive.AwardListSearchConditionDM();

            // 連接分頁計算
            TWNewEgg.ECWeb.Services.Page.CalculationsPage calculationsPage = new Services.Page.CalculationsPage();

            // 中獎名單 partial view 的 model
            ViewBag.viewModel = new Award();

            // 中獎名單的 partial view 轉成 html 的內容
            string awardListHtml = string.Empty;

            #endregion 變數宣告

            #region 轉換搜尋絛件的 Model 為 DomainModel

            try
            {
                searchConditionDM = ModelConverter.ConvertTo<TWNewEgg.Models.DomainModels.PromoActive.AwardListSearchConditionDM>(searchCondition);
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("轉換搜尋絛件的 Model 失敗(exception); ErrorMessage = {0}; Exception = {1}", GetExceptionMessage(ex), ex.ToString()));
                return Json(new { isSuccess = false, html = "" });
            }

            #endregion 轉換搜尋絛件的 Model 為 DomainModel

            #region 讀取中獎名單頁的活動清單

            try
            {
                result = Processor.Request<ResponsePacket<TWNewEgg.Models.ViewModels.Activity.Award>, ResponsePacket<TWNewEgg.Models.DomainModels.PromoActive.AwardDM>>("PromoActiveServices", "GetAwardList", searchConditionDM).results;
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("讀取中獎名單頁的活動清單失敗(exception); ErrorMessage = {0}; Exception = {1}", GetExceptionMessage(ex), ex.ToString()));
                return Json(new { isSuccess = false, html = "" });
            }

            if (string.IsNullOrEmpty(result.error) == false)
            {
                logger.Error(string.Format("讀取中獎名單頁的活動清單失敗; ErrorMessage = {0}.", result.error));
                return Json(new { isSuccess = false, html = "" });
            }

            #endregion 讀取中獎名單頁的活動清單

            #region 分頁計算

            try
            {
                result.results.ShowPage = calculationsPage.getShowPages(result.results.MaxPage, result.results.PageIndex);
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("分頁計算失敗(exception); ErrorMessage = {0}; Exception = {1}", GetExceptionMessage(ex), ex.ToString()));
                return Json(new { isSuccess = false, html = "" });
            }

            #endregion 分頁計算

            #region 將 view 轉為 string

            ViewBag.viewModel = result.results;

            try
            {
                awardListHtml = RenderView("AwardList_ActivityList");
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("將 view 轉為 string 時失敗(exception); ErrorMessage = {0}; Exception = {1}", GetExceptionMessage(ex), ex.ToString()));
                return Json(new { isSuccess = false, html = "" });
            }

            #endregion 將 view 轉為 string

            return Json(new { isSuccess = true, html = awardListHtml });
        }

        #endregion 中獎名單

        /// <summary>
        /// 將該View轉成string
        /// </summary>
        /// <param name="partialView">View的名稱</param>
        /// <returns>返回string</returns>
        private string RenderView(string partialView)
        {
            string result = string.Empty;
            using (StringWriter sw = new StringWriter())
            {
                ViewEngineResult viewResult = ViewEngines.Engines.FindPartialView(this.ControllerContext, partialView);
                ViewContext viewContext = new ViewContext(this.ControllerContext, viewResult.View, this.ViewData, this.TempData, sw);
                viewResult.View.Render(viewContext, sw);
                result = sw.GetStringBuilder().ToString();
            }

            return result;
        }

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
