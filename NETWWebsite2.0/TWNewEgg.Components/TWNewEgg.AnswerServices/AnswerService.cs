using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//using TWNewEgg.AnswerServices.Interface;
using TWNewEgg.AnswerAdapters.Interface;
using TWNewEgg.Models.DomainModels.Account;
using TWNewEgg.Models.DBModels.TWSQLDB;
using TWNewEgg.Framework.AutoMapper;
using TWNewEgg.Models.DomainModels.Answer;
using TWNewEgg.AnswerServices.Interface;
using TWNewEgg.CartRepoAdapters.Interface;
using TWNewEgg.ItemServices.Interface;

namespace TWNewEgg.AnswerServices
{
    public class AnswerService : IAnswerService
    {
        private IAnswerAdapter _answerRepoAdapter;
        private ISORepoAdapter _soRepoAdapter;
        private IItemInfoService _itemInfoService;
        public AnswerService(IAnswerAdapter answerRepoAdapter, ISORepoAdapter soRepoAdapter, IItemInfoService itemInfoService) 
        {
            this._answerRepoAdapter = answerRepoAdapter;
            this._soRepoAdapter = soRepoAdapter;
            this._itemInfoService = itemInfoService;
        }
        public List<AnswerInfo> GetPrblmRecode(int accID, int Mouth, string Email, string Salceorder) 
        {

            List<AnswerInfo> AnswerInfos = new List<AnswerInfo>();
            var IQeryProblem = _answerRepoAdapter.GetAccountProbelmInfoByAccID(accID);
            //var IQeryProblem = _answerRepoAdapter.GetAccountProbelmInfo(Email);
            // 只撈取過去三個月的資料
            DateTime startday = DateTime.Now.AddMonths(-Mouth);
            var ProductFromWSListdetail = IQeryProblem.Where(x => x.Source == 1);
            if (string.IsNullOrWhiteSpace(Salceorder) == false && string.IsNullOrEmpty(Salceorder) == false)
            {
                var Problemlist = ProductFromWSListdetail.Where(x => x.CreateDate > startday && x.BlngCode == Salceorder).GroupBy(x => x.BlngCode).ToList();
                
                for (int i = 0; i < Problemlist.Count; i++)
                {
                    List<AnswerBase> AnswerInfoDetil = new List<Models.DomainModels.Answer.AnswerBase>();
                    AnswerInfo AnswerInfo = new AnswerInfo();
                    var ProbelBase = ModelConverter.ConvertTo<ProbelmBase>(Problemlist[i].Select(x => x).FirstOrDefault());
                    string prCode = Problemlist[i].Select(x => x.Code).FirstOrDefault();
                    var Answerlist = _answerRepoAdapter.GetAnswerForProbelm(prCode).ToList();
                   
                    foreach (var AnswerInfodetil in Answerlist)
                    {

                        var Answers = ModelConverter.ConvertTo<AnswerBase>(AnswerInfodetil);
                        AnswerInfoDetil.Add(Answers);

                    }
                    AnswerInfo.AnswerList = AnswerInfoDetil;
                    AnswerInfo.Probelm = ProbelBase;
                    if (string.IsNullOrWhiteSpace(Problemlist[i].Select(x => x.BlngCode).FirstOrDefault()) == false)
                    {
                        string SOCode = Problemlist[i].Select(x => x.BlngCode).FirstOrDefault();
                        var soItem = this._soRepoAdapter.GetSOItems(SOCode).FirstOrDefault();
                    

                        var soItemBase = ModelConverter.ConvertTo<SalesOrderItemInfo>(soItem);
                        AnswerInfo.SalesOrderItem = new List<SalesOrderItemInfo>();
                        AnswerInfo.SalesOrderItem.Add(soItemBase);
                    }
                    AnswerInfos.Add(AnswerInfo);
                }
            }
            else 
            {


                var Problemlist = ProductFromWSListdetail.Where(x => x.CreateDate > startday).OrderByDescending(x => x.CreateDate).ToList();
                for (int i = 0; i < Problemlist.Count; i++)
                {
                    List<AnswerBase> AnswerInfoDetil = new List<Models.DomainModels.Answer.AnswerBase>();
                    AnswerInfo AnswerInfo = new AnswerInfo();
                    var ProbelBase = ModelConverter.ConvertTo<ProbelmBase>(Problemlist[i]);
                    string prCode = Problemlist[i].Code;
                    var Answerlist = _answerRepoAdapter.GetAnswerForProbelm(prCode).ToList();

                    foreach (var AnswerInfodetil in Answerlist)
                    {

                        var Answers = ModelConverter.ConvertTo<AnswerBase>(AnswerInfodetil);
                        AnswerInfoDetil.Add(Answers);

                    }
                    AnswerInfo.AnswerList = AnswerInfoDetil;
                    AnswerInfo.Probelm = ProbelBase;
                    if (string.IsNullOrWhiteSpace(Problemlist[i].BlngCode) == false)
                    {
                        string SOCode = Problemlist[i].BlngCode;
                        var soItem = this._soRepoAdapter.GetSOItems(SOCode).FirstOrDefault();
                        var soItemBase = ModelConverter.ConvertTo<SalesOrderItemInfo>(soItem);
                        AnswerInfo.SalesOrderItem = new List<SalesOrderItemInfo>();
                        AnswerInfo.SalesOrderItem.Add(soItemBase);
                    }
                    AnswerInfos.Add(AnswerInfo);
                }
            
            }
         //var SOInfo = this._answerRepoAdapter.GetAnswerInfo(accID, Mouth);
         if (AnswerInfos != null)
         {
          
             return AnswerInfos;
         }
         else
         {

             return null;
         }
        }
        public AnswerInfo GetSalceOrderInfo(string SalesOrderCode, int accID,string Name)
        {
            AnswerInfo AnswerInfoDetil = new AnswerInfo();
            List<SalesOrderItemInfo> SalesOrderItemInfoDetail = new List<SalesOrderItemInfo>();
            if (string.IsNullOrWhiteSpace(SalesOrderCode) == true)
            {
                AnswerInfoDetil.SalesOrder = new SalesOrderInfo();
                AnswerInfoDetil.SalesOrder.Name = Name;
          
            }
            else
            {
                var So = this._soRepoAdapter.GetSOs(new List<string>() { SalesOrderCode });
               var SoItem = this._soRepoAdapter.GetSOItems(SalesOrderCode);
                 var SOInfo = (from soInfo in So join SoItemDetil in SoItem on soInfo.Code equals SoItemDetil.SalesorderCode where soInfo.Code == SalesOrderCode select new { SoItemDetil, soInfo }).ToList();
                TWNewEgg.Models.DBModels.TWSQLDB.SalesOrder SODetil = SOInfo.Select(x => x.soInfo).FirstOrDefault();
                var sobase = ModelConverter.ConvertTo<SalesOrderInfo>(SODetil);
                foreach (var SoItembases in SOInfo.Select(x => x.SoItemDetil))
                {
                    SalesOrderItemInfo SalesOrderItemInfo = new Models.DomainModels.Answer.SalesOrderItemInfo();
                    var SoItembase = ModelConverter.ConvertTo<SalesOrderItemInfo>(SoItembases);
                    SalesOrderItemInfoDetail.Add(SoItembase);
                }
                AnswerInfoDetil.SalesOrder = new SalesOrderInfo();
                AnswerInfoDetil.SalesOrder = sobase;
                AnswerInfoDetil.SalesOrderItem = SalesOrderItemInfoDetail;
            }

            if (AnswerInfoDetil != null)
           {
             
               return AnswerInfoDetil;
           }
           else 
           {

               return null;
           }

        }
        public Models.DomainModels.Redeem.ActionResponse<AnswerInfo>  AddSalseOrderForAnswerInfo(SalesOrderInfo SalesOrderInfo,int? ItemID, short? faqtypeval, string maintext, int accID)
        {
            Models.DomainModels.Redeem.ActionResponse<AnswerInfo> AddSalseOrderForAnswerInfo = new Models.DomainModels.Redeem.ActionResponse<AnswerInfo>();
            Problem AnswerInfoDetil = new Problem();

            Models.DomainModels.Redeem.ActionResponse<AnswerInfo> Actioresualt = new Models.DomainModels.Redeem.ActionResponse<AnswerInfo>();
            try
            {
                AnswerInfo AnswerInfo = new Models.DomainModels.Answer.AnswerInfo();
                Problem Probelm = new Models.DBModels.TWSQLDB.Problem();
                if (string.IsNullOrWhiteSpace(SalesOrderInfo.Code) != true)
                {
                    var _so = this._soRepoAdapter.GetSO(SalesOrderInfo.Code);
                    var _soItem = this._soRepoAdapter.GetSOItems(SalesOrderInfo.Code).FirstOrDefault();
                  
                    if (_so != null)
                    {
                        if (_so.AccountID == accID)
                        {
                            Probelm.Code = GetAutoSN("PR");
                            Probelm.AccountID = accID;
                            Probelm.BlngCode = SalesOrderInfo.Code;
                            Probelm.ItemID = ItemID;
                            Probelm.Name = SalesOrderInfo.RecvName;
                            Probelm.TEL = SalesOrderInfo.Mobile;
                            Probelm.Email = SalesOrderInfo.Email;
                            Probelm.IntClass = faqtypeval;
                            Probelm.Cont = maintext.Replace("\n", "<br>");
                            Probelm.Source = (int)Problem.pbsource.信件;
                            Probelm.Status = (int)Problem.pbstatus.未處理;
                            Probelm.StatusDate = DateTime.Now;
                            Probelm.CreateDate = DateTime.Now;
                          Probelm= _answerRepoAdapter.AddProblem(Probelm);
                          
                            var Probelmbase = ModelConverter.ConvertTo<ProbelmBase>(Probelm);
                            AnswerInfo.Probelm = Probelmbase;
                            AnswerInfo.SalesOrder = SalesOrderInfo;
                            Actioresualt.Body = AnswerInfo;
                            Actioresualt.IsSuccess = true;
                            Actioresualt.Msg = "1";
                            return Actioresualt;
                        }
                        else
                        {
                            Actioresualt.IsSuccess = false;
                            Actioresualt.Msg = "SalesOrder_Account_error";
                            return Actioresualt;

                        }

                    }
                    else
                    {
                        Actioresualt.IsSuccess = false;
                        Actioresualt.Msg = "SalesOrder_code_error";
                       

                    }
                }
                else
                {

                    Probelm.Code = GetAutoSN("PR");
                    Probelm.AccountID = accID;
                    Probelm.BlngCode = SalesOrderInfo.Code;
                    Probelm.ItemID = ItemID;
                    Probelm.Name = SalesOrderInfo.Name;
                    Probelm.TEL = SalesOrderInfo.Mobile;
                    Probelm.Email = SalesOrderInfo.Email;
                    Probelm.IntClass = faqtypeval;
                    Probelm.Cont = maintext.Replace("\n", "<br>");
                    Probelm.Source = (int)Problem.pbsource.信件;
                    Probelm.Status = (int)Problem.pbstatus.未處理;
                    Probelm.StatusDate = DateTime.Now;
                    Probelm.CreateDate = DateTime.Now;
                    Probelm = _answerRepoAdapter.AddProblem(Probelm);
                    var Probelmbase = ModelConverter.ConvertTo<ProbelmBase>(Probelm);
                    AnswerInfo.Probelm = Probelmbase;
                    AnswerInfo.SalesOrder = SalesOrderInfo;
                    Actioresualt.Body = AnswerInfo;
                    Actioresualt.IsSuccess = true;
                    Actioresualt.Msg = "1";
             

                }


            }
            catch (Exception ex)
            {
                Actioresualt.IsSuccess = false;
                Actioresualt.Msg = "SalesOrder_Account_error";
           


                throw ex;

            }
           // var AnswerInfo = this._answerRepoAdapter.AddSalseOrderForAnswerInfo(SalesOrderInfo,ItemID, faqtypeval, maintext, accID);
            if (Actioresualt != null) 
            {
                AddSalseOrderForAnswerInfo = Actioresualt;

                return AddSalseOrderForAnswerInfo;
            }
            else
            {

                return null;
            }
        }
        public string GetAutoSN(string type)
        {
            string result = "";
            var Prblem = _answerRepoAdapter.GetALLProbelm();
            var Answer = _answerRepoAdapter.GetALLAnswer();
            int max = 0;
            type = type.ToUpper();
            switch (type)
            {
                case "PR":
                    max = 0;
                    string code = (from x in Prblem select x.Code).Max();
                    if (code != null)
                    {
                        code = code.Substring(8);
                        max = int.Parse(code);
                    }
                    max++;
                    result = type + DateTime.Now.ToString("yyMMdd") + max.ToString("0000000");
                    break;
                case "AN":
                    max = 0;
                    //prblm_code
                    max = 0;
                    code = (from row in Answer select row.Code).Max();
                    if (code != null)
                    {
                        code = code.Substring(8);
                        max = int.Parse(code);
                    }


                    return code;
                    max++;
                    result = type + DateTime.Now.ToString("yyMMdd") + max.ToString("0000000");
                    break;
            }
            return result;
        }
        #region 我要發問的問答歷史紀錄
        #region 利用問題單編號查詢對應的資料
        /// <summary>
        /// 根據傳進來的問題單編號讀取問題單相關的資料
        /// </summary>
        /// <param name="accID"></param>
        /// <param name="Mouth"></param>
        /// <param name="Email"></param>
        /// <param name="Salceorder"></param>
        /// <param name="ProblemId"></param>
        /// <returns></returns>
        public AnswerInfo GetPrblmRecodeSelect(int accID, int Mouth, string Email, string Salceorder, string ProblemId)
        {
            List<AnswerInfo> ansInfo = new List<AnswerInfo>();
            #region 先取得所有的問題單與回答單
            //先取得所有的問題單與回答單
            ansInfo = this.GetPrblmRecode(accID, Mouth, Email, Salceorder);
            //沒有對應的資料則回傳 null
            if (ansInfo == null)
            {
                return null;
            }
            #endregion
            #region 利用進來的 ProductId 取得對應的資料
            //利用進來的 ProductId 取得對應的資料
            var selectProblem = ansInfo.Where(p => p.Probelm.Code == ProblemId).FirstOrDefault();
            //判斷是否有對應的問題單編號的問題, 沒有則回傳 null
            if (selectProblem == null)
            {
                return null;
            }
            #endregion
            //必須把 Cont (主旨) 是空的或是 null 過濾掉
            var selectProblemRemoveContEmpty = selectProblem.AnswerList.Where(p => p.Cont.Replace(" ", "") != "" && p.Cont.Replace(" ", "") != null);
            //利用 Probelm.BlngCode 取得在 SaleOrderItem 對應的資料(為了算總價用)
            var SaleOrderItem = _soRepoAdapter.GetSOItems(selectProblem.Probelm.BlngCode);
            //選擇問題單的 type
            var SalesOrderItemInfo = this.ProblemType(SaleOrderItem.ToList(), selectProblem.Probelm);
            
            AnswerInfo _ansInfo = new AnswerInfo();
            _ansInfo.Probelm = selectProblem.Probelm;
            _ansInfo.AnswerList = selectProblemRemoveContEmpty.ToList();
            _ansInfo.SalesOrderItem = SalesOrderItemInfo;
            return _ansInfo;
        }
        #endregion
        #region 利用 problem.IntClass 判斷問題單的種類作相對應的邏輯運算
        /// <summary>
        /// 根據條件取得相關的資料或者是做 automap
        /// </summary>
        /// <param name="saleOrderItem"></param>
        /// <param name="problem"></param>
        /// <returns></returns>
        public List<SalesOrderItemInfo> autoMapSalesOrderItemInfo_SalesOrderItem(List<SalesOrderItem> saleOrderItem, ProbelmBase problem)
        {
            List<SalesOrderItemInfo> returnSalesOrderItemInfoModel = new List<SalesOrderItemInfo>();
            #region 其他問題
            //詐騙相關問題, 其他問題, 其他問題 不必要 item name
            if (problem.IntClass == (int)TWNewEgg.Models.DomainModels.Answer.ProbelmBase.pbcase.詐騙相關問題 ||
                problem.IntClass == (int)TWNewEgg.Models.DomainModels.Answer.ProbelmBase.pbcase.其他問題 ||
                problem.IntClass == (int)TWNewEgg.Models.DomainModels.Answer.ProbelmBase.pbcase.系統網頁問題)
            {
                //不抓 item name(前端沒有用到)
                SalesOrderItemInfo _SOItemInfoTemp = new SalesOrderItemInfo();
                //給暫存 item name 欄位給空值
                _SOItemInfoTemp.Name = string.Empty;
                returnSalesOrderItemInfoModel.Add(_SOItemInfoTemp);
                return returnSalesOrderItemInfoModel;
            }
            #endregion
            #region 賣場相關(沒有 saleOrderItem 所以沒有訂單, 無法從訂單取得商品名稱)
            string itemName = string.Empty;
            // saleOrderItem 是空的無法取得 item name 所以利用 problem 的 itemid 去讀取 item 對應的 item name
            if (saleOrderItem == null || saleOrderItem.Count == 0)
            {
                //先判斷 item id 是否有值, 有再去讀取對應的資料
                if (problem.ItemID.HasValue == true)
                {
                    TWNewEgg.Models.DomainModels.Item.ItemInfo getGetItemInfo = _itemInfoService.GetItemInfo(problem.ItemID.Value);
                    if (getGetItemInfo != null)
                    {
                        TWNewEgg.Models.DomainModels.Item.ItemBase iteminfo = getGetItemInfo.ItemBase;
                        //有對應的資料再把 item name 抓出來
                        if (iteminfo != null)
                        {
                            itemName = iteminfo.Name;
                        }
                    }
                }

                SalesOrderItemInfo _SOItemInfoTemp = new SalesOrderItemInfo();
                _SOItemInfoTemp.Name = itemName;
                returnSalesOrderItemInfoModel.Add(_SOItemInfoTemp);
                return returnSalesOrderItemInfoModel;
            }
            #endregion
            #region 訂單進度, 退換維修, 帳款與發票(因為有訂單)
            //開始 automap
            foreach (var item in saleOrderItem)
            {
                SalesOrderItemInfo SOItemInfoTemp = new SalesOrderItemInfo();
                SOItemInfoTemp = ModelConverter.ConvertTo<SalesOrderItemInfo>(item);
                returnSalesOrderItemInfoModel.Add(SOItemInfoTemp);
            }
            return returnSalesOrderItemInfoModel;
            #endregion
        }
        #endregion
        #region 根據問題單的種類, 讀取需要的資料
        /// <summary>
        /// 根據問題單的種類, 讀取需要的資料
        /// </summary>
        /// <param name="saleOrderItem"></param>
        /// <param name="problem"></param>
        /// <returns></returns>
        public List<SalesOrderItemInfo> ProblemType(List<SalesOrderItem> saleOrderItem, ProbelmBase problem)
        {
            List<SalesOrderItemInfo> returnSalesOrderItemInfoModel = new List<SalesOrderItemInfo>();
            #region 其他問題
            if (problem.IntClass == (int)TWNewEgg.Models.DomainModels.Answer.ProbelmBase.pbcase.詐騙相關問題 ||
                problem.IntClass == (int)TWNewEgg.Models.DomainModels.Answer.ProbelmBase.pbcase.其他問題 ||
                problem.IntClass == (int)TWNewEgg.Models.DomainModels.Answer.ProbelmBase.pbcase.系統網頁問題)
            {
                //不需要利用 saleorder 去讀取商品資料, 所以不用做任何運算
                //new _SOItemInfoTemp 並加入到 List 是為了防止前端讀取時因為沒有資料而發生錯誤
                SalesOrderItemInfo _SOItemInfoTemp = new SalesOrderItemInfo();
                returnSalesOrderItemInfoModel.Add(_SOItemInfoTemp);
                return returnSalesOrderItemInfoModel;
            }
            #endregion
            #region 沒有訂單
            if (saleOrderItem == null || saleOrderItem.Count == 0)
            {
                //沒有訂單的情形下, 無法直接取得商品名稱, 所以必須呼叫 item 的 serivce 取的 item name
                string itemName = string.Empty;
                //呼叫 function 取的 item name
                itemName = this.GetItemName(problem.ItemID);
                SalesOrderItemInfo _SOItemInfoTemp = new SalesOrderItemInfo();
                _SOItemInfoTemp.Name = itemName;
                returnSalesOrderItemInfoModel.Add(_SOItemInfoTemp);
                return returnSalesOrderItemInfoModel;
            }
            #endregion
            #region 有訂單的情形下
            //因為有訂單, 表示有 saleOrderItem, 所以進行 automap 回 SalesOrderItemInfo Model(必須利用來計算總價)
            returnSalesOrderItemInfoModel = this.automapSalesOrderItemToItemInfo(saleOrderItem);
            #endregion
            return returnSalesOrderItemInfoModel;
        }
        #endregion
        #region Get information by using item id
        /// <summary>
        /// 利用 item id 取的 item name
        /// </summary>
        /// <param name="itemid"></param>
        /// <returns></returns>
        public string GetItemName(int? itemid)
        {
            string returnStr = string.Empty;
            //先判斷 itemid 是否有值
            if (itemid.HasValue == true)
            {
                try
                {
                    // 取得 item 相關資料
                    TWNewEgg.Models.DomainModels.Item.ItemInfo getGetItemInfo = _itemInfoService.GetItemInfo(itemid.Value);
                    if (getGetItemInfo != null)
                    {
                        TWNewEgg.Models.DomainModels.Item.ItemBase iteminfo = getGetItemInfo.ItemBase;
                        // 有對應的資料再把 item name 抓出來
                        if (iteminfo != null)
                        {
                            returnStr = iteminfo.Name;
                        }
                        else
                        {
                            // 沒有 item 相關資料還是給空值
                            returnStr = string.Empty;
                        }
                    }
                }
                catch (Exception)
                {
                    //發生 Exception 所以還是給空值
                    returnStr = string.Empty;
                }
            }
            else
            {
                //沒有傳進 item id 所以還是給空值
                returnStr = string.Empty;
            }
            return returnStr;
        }
        #endregion
        #region using saleOrderItem model to automap to SalesOrderItemInfo model and return
        /// <summary>
        /// SalesOrderItem auto to SalesOrderItemInfo
        /// </summary>
        /// <param name="saleOrderItem"></param>
        /// <returns></returns>
        public List<SalesOrderItemInfo> automapSalesOrderItemToItemInfo(List<SalesOrderItem> saleOrderItem)
        {
            List<SalesOrderItemInfo> returnSalesOrderItemInfoModel = new List<SalesOrderItemInfo>();
            foreach (var item in saleOrderItem)
            {
                SalesOrderItemInfo SOItemInfoTemp = new SalesOrderItemInfo();
                SOItemInfoTemp = ModelConverter.ConvertTo<SalesOrderItemInfo>(item);
                returnSalesOrderItemInfoModel.Add(SOItemInfoTemp);
            }
            return returnSalesOrderItemInfoModel;
        }
        #endregion
        #endregion
    }



}
