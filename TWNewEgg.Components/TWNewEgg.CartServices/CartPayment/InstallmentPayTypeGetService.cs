using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TWNewEgg.BankInstallmentRepoAdapters.Interface;
using TWNewEgg.CartRepoAdapters.Interface;
using TWNewEgg.CartServices.Interface;
using TWNewEgg.DAL.Interface;
using TWNewEgg.Framework.Autofac;
using TWNewEgg.Framework.AutoMapper;
using TWNewEgg.ItemInstallmentRepoAdapters.Interface;
using TWNewEgg.ItemRepoAdapters.Interface;
using TWNewEgg.Models.DomainModels.CartPayment;
using TWNewEgg.Models.DomainModels.Message;
//using TWNewEgg.Models.DomainModels.PayType;
using TWNewEgg.Models.DomainModels.ShoppingCartPayType;

namespace TWNewEgg.CartServices.CartPayment
{
    public class InstallmentPayTypeGetService : IPayTypeGetService
    {
        private IBankInstallmentRepoAdapter _iBankInstallmentRepoAdapter;
        private IPayTypeRepoAdapter _iPayTypeRepoAdapter;
        private IItemRepoAdapter _iItemRepoAdapter;
        private IItemDisplayPriceRepoAdapter _iItemDisplayPriceRepoAdapter;
        private IItemTopInstallmentRepoAdapter _iItemTopInstallmentRepoAdapter;
        private IProductRepoAdapter _iProductRepoAdapter;

        private log4net.ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.FullName);

        public InstallmentPayTypeGetService(IPayTypeRepoAdapter iPayTypeRepoAdapter,
            IBankInstallmentRepoAdapter iBankInstallmentRepoAdapter,
            IProductRepoAdapter iProductRepoAdapter, 
            IItemRepoAdapter iItemRepoAdapter,
            IItemTopInstallmentRepoAdapter iItemTopInstallmentRepoAdapter,
            IItemDisplayPriceRepoAdapter iItemDisplayPriceRepoAdapter)
        {
            this._iPayTypeRepoAdapter = iPayTypeRepoAdapter;
            this._iItemRepoAdapter = iItemRepoAdapter;
            this._iItemDisplayPriceRepoAdapter = iItemDisplayPriceRepoAdapter;
            this._iBankInstallmentRepoAdapter = iBankInstallmentRepoAdapter;
            this._iItemTopInstallmentRepoAdapter = iItemTopInstallmentRepoAdapter;
            this._iProductRepoAdapter = iProductRepoAdapter;
        }

        public IEnumerable<DM_PayType> GetPayTypes(DM_ShoppingCart shoppingCart)
        {

            ResponseMessage<List<DM_PayType>> queryResult = new ResponseMessage<List<DM_PayType>>();
            //ResponseMessage<List<DM_PayType>> result = new ResponseMessage<List<DM_PayType>>();
            //購物車內的商品
            List<int> itemIdCell = new List<int>();
            itemIdCell = shoppingCart.Items;

            queryResult = this.GetInstallmentPayTypes(itemIdCell);
            
            return queryResult.Data;
       
        }

        /// <summary>
        /// 取得分期數
        /// </summary>
        /// <param name="itemIdCell"></param>
        /// <returns></returns>
        protected ResponseMessage<List<DM_PayType>> GetInstallmentPayTypes(List<int> itemIdCell)
        {
            ResponseMessage<List<DM_PayType>> result = new ResponseMessage<List<DM_PayType>>();

            List<ItemAndPayType> itemAndPayTypeCell = null;

            // 最高分期數設定
            List<TWNewEgg.Models.DBModels.TWSQLDB.ItemTopInstallment> itemTopInstallmentCell = null;

            // 現在時間
            DateTime now = DateTime.Now;

            // 連接 ItemInstallmentService
            IItemInstallmentService itemInstallmentService = null;

            // 分期預設邏輯設定
            ResponseMessage<List<Models.DomainModels.ItemInstallment.DefaultInstallment>> defaultRuleCell = null;

            // 全商品開放分期期數
            int allOpen_Installment = -1;

            // 全商品開放分期數的 PayType
            List<DM_PayType> allOpen_PayTypeCell = null;

            // 要查詢 PayType 的分期期數
            List<int> installmentCell = null;

            // PayType DB Model
            List<Models.DBModels.TWSQLDB.PayType> paytypeCell_DbModel = null;

            Models.DomainModels.ItemInstallment.DefaultInstallment defaultRule = null;

            try
            {
                if (itemIdCell == null || itemIdCell.Count() == 0)
                {
                    result.Message = "未傳入場賣編號清單。";
                    throw new Exception(result.Message);
                }

                //取Item售價成本
                itemAndPayTypeCell = GetItemPriceAndCost(itemIdCell, itemAndPayTypeCell);

                if (itemAndPayTypeCell.Count == 0)
                {
                    result.Message = "取得售價及成本失敗。";
                    throw new Exception(result.Message);
                }

                // 計算商品毛利率
                itemAndPayTypeCell = GetGrossMargin(itemAndPayTypeCell);

                // 先取得賣場最高分期期數中，有被設定的資料
                itemAndPayTypeCell = GetItemTopInstallment(itemIdCell, itemAndPayTypeCell, itemTopInstallmentCell, now);

                // 連接 ItemInstallmentService
                itemInstallmentService = AutofacConfig.Container.Resolve<IItemInstallmentService>();

                if (itemInstallmentService == null)
                {
                    result.Message = "連接 ItemInstallmentService 失敗。";
                    throw new Exception(result.Message);
                }

                // 取得分期預設邏輯設定
                defaultRuleCell = itemInstallmentService.GetDefaultRules();

                itemInstallmentService = null;

                if (defaultRuleCell.IsSuccess == false || defaultRuleCell.Data == null || defaultRuleCell.Data.Count == 0)
                {
                    result.Message = "取得分期預設邏輯設定失敗。";
                    throw new Exception(defaultRuleCell.Message);
                }

                // 若是查不到賣場最高分期期數，或是還有要取得預設分期的部分
                itemAndPayTypeCell = GetPersetInstallment(itemAndPayTypeCell, defaultRuleCell.Data);

                #region 取得 PayType

                #region 取得全商品開放分期的 PayType

                // 取得全商品開放分期期數
                allOpen_Installment = defaultRuleCell.Data.Where(x => x.Price == -1).Select(x => x.Installment).FirstOrDefault();

                // 取得小於等於全商品開放分期期數的所有分期
                allOpen_PayTypeCell = GetAllInstallment(allOpen_Installment);

                #endregion 取得全商品開放分期的 PayType

                if (itemAndPayTypeCell.Any(x => x.TopInstallment <= allOpen_Installment))
                {
                    result.Data = allOpen_PayTypeCell;
                }
                else
                {
                    foreach (ItemAndPayType itemAndPayType in itemAndPayTypeCell)
                    {
                        // 取得小於等於該賣場最高分期期數，且大於全商品開放分期期數的所有分期
                        installmentCell = GetConformInstallment(allOpen_Installment, itemAndPayType);

                        if (installmentCell == null || installmentCell.Count == 0)
                        {
                            installmentCell = null;
                            continue;
                        }

                        // 將分期數由大到小排序
                        if (installmentCell.Count > 1)
                        {
                            installmentCell = installmentCell.OrderByDescending(x => x).ToList();
                        }

                        // 依分期數檢查每個 PayType
                        foreach (int installment in installmentCell)
                        {
                            // 取得目前分期數的所有 PayType
                            paytypeCell_DbModel = this._iBankInstallmentRepoAdapter.GetPayTypesByInstallments(new List<int>() { installment }).ToList();

                            if (paytypeCell_DbModel == null || paytypeCell_DbModel.Count == 0)
                            {
                                paytypeCell_DbModel = null;
                                continue;
                            }

                            // 取得目前分期數的分期預設邏輯設定
                            defaultRule = defaultRuleCell.Data.Where(x => x.Installment == installment).FirstOrDefault();

                            if (defaultRule == null)
                            {
                                paytypeCell_DbModel = null;
                                continue;
                            }

                            // 檢查每個 PayType 是否符合分期預設邏輯設定
                            foreach (Models.DBModels.TWSQLDB.PayType payType_DbModel in paytypeCell_DbModel)
                            {
                                if (payType_DbModel.InsRate.HasValue == false)
                                {
                                    continue;
                                }

                                // 將符合分期預設邏輯設定的 PayType 加到該商品的 PayTypeCell 中
                                if ((itemAndPayType.GrossMargin - (payType_DbModel.InsRate.Value * 100)) >= defaultRule.Rate)
                                {
                                    itemAndPayType.PayTypeCell.Add(ModelConverter.ConvertTo<DM_PayType>(payType_DbModel));
                                }
                            }

                        }
                    }

                    // 取所有賣場的 PayType 的交集
                    result.Data = GetItemPayTypeJoin(itemAndPayTypeCell);
                    
                    // 加上全商品開放分期數的 PayType
                    result.Data.AddRange(allOpen_PayTypeCell);
                }

                #endregion 取得 PayType

                result.IsSuccess = true;
                result.Message = string.Format("取得賣場編號清單的 PayType 成功，已取得 {0} 筆資料。", result.Data.Count);
            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.Data = null;
                result.Error.Detail = GetExceptionMessage(ex);

                if (string.IsNullOrEmpty(result.Message))
                {
                    result.Message = "取得賣場編號清單的 PayType 失敗。";
                }

                logger.Error(string.Format("取得賣場編號清單的 PayType 失敗。Exception = {0}", ex.ToString()));
            }
            finally
            {
                if (itemAndPayTypeCell != null)
                {
                    itemAndPayTypeCell = null;
                }

                if (itemInstallmentService != null)
                {
                    itemInstallmentService = null;
                }

                if (defaultRuleCell != null)
                {
                    defaultRuleCell = null;
                }

                if (installmentCell != null)
                {
                    installmentCell = null;
                }

                if (paytypeCell_DbModel != null)
                {
                    paytypeCell_DbModel = null;
                }
            }
            
            return result;
        }

        private List<ItemAndPayType> GetPersetInstallment(List<ItemAndPayType> itemAndPayTypeCell, List<Models.DomainModels.ItemInstallment.DefaultInstallment> defaultRule)
        {
            if (itemAndPayTypeCell.Any(x => x.TopInstallment == 0))
            {
                // 將分期預設邏輯設定依售價做降冪排序
                if (defaultRule.Count > 1)
                {
                    defaultRule = defaultRule.OrderByDescending(x => x.Price).ToList();
                }

                // 依售價，取得預設分期數
                foreach (ItemAndPayType itemAndTopInstallment in itemAndPayTypeCell)
                {
                    if (itemAndTopInstallment.TopInstallment > 0)
                    {
                        continue;
                    }

                    foreach (Models.DomainModels.ItemInstallment.DefaultInstallment defaultInstallment in defaultRule)
                    {
                        if (itemAndTopInstallment.Pirce >= defaultInstallment.Price)
                        {
                            itemAndTopInstallment.TopInstallment = defaultInstallment.Installment;
                            break;
                        }
                    }
                }
            }
            return itemAndPayTypeCell;
        }

        private List<DM_PayType> GetItemPayTypeJoin(List<ItemAndPayType> itemAndPayTypeCell)
        {
            List<DM_PayType> result = null;

            if (itemAndPayTypeCell.Count == 1)
            {
                result = itemAndPayTypeCell[0].PayTypeCell.ToList();
            }
            else
            {
                result = this.Intersect_PayType(itemAndPayTypeCell[0].PayTypeCell, itemAndPayTypeCell[1].PayTypeCell);

                if (itemAndPayTypeCell.Count > 2)
                {
                    for (int i = 2; i < itemAndPayTypeCell.Count; i++)
                    {
                        result= this.Intersect_PayType(result, itemAndPayTypeCell[i].PayTypeCell);
                    }
                }
            }
            return result;
        }

        private List<DM_PayType> GetAllInstallment(int allOpen_Installment)
        {
            ResponseMessage<List<DM_PayType>> result = new ResponseMessage<List<DM_PayType>>();
            List<DM_PayType> allOpen_PayTypeCell = null;
            List<int> installmentCell = null;
            List<Models.DBModels.TWSQLDB.PayType> paytypeCell_DbModel = null;

            installmentCell = this._iBankInstallmentRepoAdapter.GetAvailableInstallments().Where(x => x.Value <= allOpen_Installment).Select(x => x.Value).ToList();

            if (installmentCell == null || installmentCell.Count == 0)
            {
                result.Message = "取得小於等於全商品開放分期期數的所有分期失敗。";
                throw new Exception(result.Message);
            }

            // 取得全商品開放分期的所有 PayType
            paytypeCell_DbModel = this._iBankInstallmentRepoAdapter.GetPayTypesByInstallments(installmentCell).ToList();

            if (paytypeCell_DbModel == null || paytypeCell_DbModel.Count == 0)
            {
                result.Message = "取得全商品開放分期的所有 PayType 失敗。";
                throw new Exception(result.Message);
            }

            // 轉為 Domain Model
            allOpen_PayTypeCell = ModelConverter.ConvertTo<List<DM_PayType>>(paytypeCell_DbModel).ToList();

            if (allOpen_PayTypeCell == null || allOpen_PayTypeCell.Count == 0)
            {
                result.Message = "轉換全商品開放分期的所有 PayType Model 失敗。";
                throw new Exception(result.Message);
            }
            return allOpen_PayTypeCell;
        }

        private List<int> GetConformInstallment(int allOpen_Installment , ItemAndPayType itemAndPayType)
        {
            List<int> installmentCell = null;
            installmentCell = this._iBankInstallmentRepoAdapter.GetAvailableInstallments()
                .Where(x => x.Value <= itemAndPayType.TopInstallment)
                .Where(x => x.Value > allOpen_Installment)
                .Select(x => x.Value).ToList();
            return installmentCell;
        }



        private List<ItemAndPayType> GetItemTopInstallment(List<int> itemIdCell, List<ItemAndPayType> itemAndPayTypeCell, List<TWNewEgg.Models.DBModels.TWSQLDB.ItemTopInstallment> itemTopInstallmentCell, DateTime now)
        {
            itemTopInstallmentCell = this._iItemTopInstallmentRepoAdapter.GetAll()
                .Where(x => x.Status == (int)Models.DBModels.TWSQLDB.ItemTopInstallmentStatus.Enable)
                .Where(x => x.StartDate <= now && x.EndDate >= now)
                .Where(x => itemIdCell.Contains(x.ItemID))
                .ToList();

            if (itemTopInstallmentCell != null && itemTopInstallmentCell.Count > 0)
            {
                foreach (ItemAndPayType itemAndPayType in itemAndPayTypeCell)
                {
                    if (itemTopInstallmentCell.Any(x => x.ItemID == itemAndPayType.ItmeID))
                    {
                        itemAndPayType.TopInstallment = itemTopInstallmentCell.Where(x => x.ItemID == itemAndPayType.ItmeID).Select(x => x.TopInstallment).FirstOrDefault();
                    }
                }
            }
            return itemAndPayTypeCell;
        }
        
        private List<ItemAndPayType> GetGrossMargin(List<ItemAndPayType> itemAndPayTypeCell)
        {
            foreach (ItemAndPayType itemAndPayType in itemAndPayTypeCell)
            {
                if (itemAndPayType.Pirce <= 0 || itemAndPayType.Pirce == itemAndPayType.Cost)
                {
                    itemAndPayType.GrossMargin = 0;
                }
                else
                {
                    // 商品毛利率 = ((售價 - 成本) / 售價) * 100%
                    itemAndPayType.GrossMargin = Decimal.Round(((itemAndPayType.Pirce - itemAndPayType.Cost) / itemAndPayType.Pirce) * 100, 2);
                }
            }
            return itemAndPayTypeCell;
        }

        private List<ItemAndPayType> GetItemPriceAndCost(List<int> itemIdCell, List<ItemAndPayType> itemAndPayTypeCell)
        {
            itemAndPayTypeCell = (from itemId in itemIdCell
                                  join item in _iItemRepoAdapter.GetAll().Where(x => itemIdCell.Contains(x.ID)) on itemId equals item.ID into item_temp
                                  from item in item_temp.DefaultIfEmpty()
                                  join itemDisplayPrice in _iItemDisplayPriceRepoAdapter.GetAll().Where(x => itemIdCell.Contains(x.ItemID)).ToList() on itemId equals itemDisplayPrice.ItemID into itemDisplayPrice_temp
                                  from itemDisplayPrice in itemDisplayPrice_temp.DefaultIfEmpty()

                                  select new ItemAndPayType()
                                  {
                                      ItmeID = itemId,
                                      Pirce = itemDisplayPrice.DisplayPrice,
                                  }).ToList();
            foreach (var item in itemAndPayTypeCell)
            {
                int itemDetailProductID = _iItemRepoAdapter.GetAll().Where(x => x.ID == item.ItmeID).Select(x => x.ProductID).FirstOrDefault();
                item.Cost = _iProductRepoAdapter.GetAll().Where(x => x.ID == itemDetailProductID).Select(x => x.Cost).FirstOrDefault().Value;
            }
            return itemAndPayTypeCell;
        }


        public IQueryable<Models.DomainModels.CartPayment.DM_PayType> GetAllPayTypes()
        {
            IQueryable<Models.DBModels.TWSQLDB.PayType> querySearch;
            IQueryable<Models.DomainModels.CartPayment.DM_PayType> queryResult;

            try
            { 
                querySearch = this._iPayTypeRepoAdapter.getAll();
                queryResult = ModelConverter.ConvertTo<IQueryable<Models.DomainModels.CartPayment.DM_PayType>>(querySearch);
                
            }
            catch (Exception ex)
            {
                throw new NotImplementedException(ex.Message, ex);
            }

            return queryResult;
        }

        /// <summary>
        /// 取得 2 個 PayType List 的交集
        /// </summary>
        /// <param name="payTypeCell_1">PayType List 1</param>
        /// <param name="payTypeCell_2">PayType List 2</param>
        /// <returns>交集 的 PayType List</returns>
        private List<DM_PayType> Intersect_PayType(List<DM_PayType> payTypeCell_1, List<DM_PayType> payTypeCell_2)
        {
            List<DM_PayType> result = new List<DM_PayType>();

            foreach (DM_PayType payType in payTypeCell_1)
            {
                if (payTypeCell_2.Any(x => x.ID == payType.ID))
                {
                    result.Add(payType);
                }
            }

            return result;
        }

        /// <summary>
        /// 取得 Exception 錯誤訊息
        /// </summary>
        /// <remarks>解決若有 InnerException 時，無法取得 InnerException Message 的問題</remarks>
        /// <param name="ex">Exception 內容</param>
        /// <returns>Exception 錯誤訊息</returns>
        private string GetExceptionMessage(System.Exception ex)
        {
            if (ex.Message.IndexOf("See the inner exception for details.") != -1)
            {
                return GetExceptionMessage(ex.InnerException);
            }
            else
            {
                return ex.Message;
            }
        }
        
    }
}
