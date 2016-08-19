using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TWNewEgg.CartServices.Interface;
using TWNewEgg.BankInstallmentRepoAdapters.Interface;
using TWNewEgg.ItemInstallmentRepoAdapters.Interface;
using TWNewEgg.Models.DBModels;
using TWNewEgg.Models.DomainModels.Message;
using TWNewEgg.Models.DomainModels.ItemInstallment;
using TWNewEgg.Framework.AutoMapper;
using TWNewEgg.ItemRepoAdapters.Interface;
using TWNewEgg.SellerRepoAdapters.Interface;

namespace TWNewEgg.CartServices
{
    public class ItemInstallmentService : IItemInstallmentService
    {
        private IBankInstallmentRepoAdapter _iBankInstallmentRepoAdapter;
        private IItemInstallmentRuleRepoAdapter _iItemInstallmentRuleRepoAdapter;
        private IItemTopInstallmentRepoAdapter _iItemTopInstallmentRepoAdapter;
        private IItemRepoAdapter _iItemRepoAdapter;
        private IItemDisplayPriceRepoAdapter _iItemDisplayPriceRepoAdapter;
        private IProductRepoAdapter _iProductRepoAdapter;
        private ISellerRepoAdapter _iSellerRepoAdapter;
        private log4net.ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.FullName);

        public ItemInstallmentService(IBankInstallmentRepoAdapter iBankInstallmentRepoAdapter,
            IItemInstallmentRuleRepoAdapter iItemInstallmentRuleRepoAdapter,
            IItemTopInstallmentRepoAdapter iItemTopInstallmentRepoAdapter,
            IItemRepoAdapter iItemRepoAdapter,
            IItemDisplayPriceRepoAdapter iItemDisplayPriceRepoAdapter,
            IProductRepoAdapter iProductRepoAdapter,
            ISellerRepoAdapter iSellerRepoAdapter)
        {
            this._iBankInstallmentRepoAdapter = iBankInstallmentRepoAdapter;
            this._iItemInstallmentRuleRepoAdapter = iItemInstallmentRuleRepoAdapter;
            this._iItemTopInstallmentRepoAdapter = iItemTopInstallmentRepoAdapter;
            this._iItemRepoAdapter = iItemRepoAdapter;
            this._iItemDisplayPriceRepoAdapter = iItemDisplayPriceRepoAdapter;
            this._iProductRepoAdapter = iProductRepoAdapter;
            this._iSellerRepoAdapter = iSellerRepoAdapter;
        }

        #region 分期預設邏輯設定

        /// <summary>
        /// 取得分期預設邏輯設定
        /// </summary>
        /// <return>分期預設邏輯設定清單</return>
        public ResponseMessage<List<DefaultInstallment>> GetDefaultRules()
        {
            ResponseMessage<List<DefaultInstallment>> result = new ResponseMessage<List<DefaultInstallment>>();

            try
            {
                result.Data = (from itemInstallmentRule in this._iItemInstallmentRuleRepoAdapter.GetAll()
                               join installment in this._iBankInstallmentRepoAdapter.GetAvailableInstallments() on itemInstallmentRule.InstallmentID equals installment.ID
                               select new DefaultInstallment()
                               {
                                   ID = itemInstallmentRule.ID,
                                   Installment = installment.Value,
                                   Price = itemInstallmentRule.Price,
                                   Rate = itemInstallmentRule.Rate
                               }).ToList();

                if (result.Data.Count == 0 || result.Data.Any(x => x.Price == -1) == false)
                {
                    // 資料庫資料初使化，新增全部商品開放分期，並賦予其最小的值
                    this.CreateDefaultRule(
                        new DefaultInstallment()
                        {
                            ID = 0,
                            Price = -1,
                            Installment = this._iBankInstallmentRepoAdapter.GetAvailableInstallments().ToList().OrderBy(x => x.Value).Select(x => x.Value).First(),
                            Rate = -1
                        },
                        "IPP");

                    return this.GetDefaultRules();
                }

                // 依分期期數排序
                if (result.Data.Count > 1)
                {
                    result.Data = result.Data.OrderBy(x => x.Installment).ToList();
                }

                result.IsSuccess = true;
                result.Message = string.Format("取得分期預設邏輯設定成功，已取得 {0} 筆資料。", result.Data.Count);
                result.Error = null;
            }
            catch (Exception ex)
            {
                result.Data = null;
                result.IsSuccess = false;
                result.Message = "取得分期預設邏輯設定失敗。";
                result.Error = new ErrorMessage();
                result.Error.Detail = ex.ToString();
            }

            return result;
        }

        /// <summary>
        /// 新增分期預設邏輯設定
        /// </summary>
        /// <param name="createData">分期預設邏輯設定</param>
        /// <param name="createUser">建立人</param>
        /// <returns>錯誤訊息</returns>
        public ResponseMessage<string> CreateDefaultRule(DefaultInstallment createData, string createUser)
        {
            ResponseMessage<string> result = new ResponseMessage<string>();
            ResponseMessage<List<DefaultInstallment>> checkDefaultRule = null;
            Models.DBModels.TWSQLDB.ItemInstallmentRule dbModel = null;
            int createdID = -1;

            // 是否真的發生 Exception
            bool isException = true;

            try
            {
                #region 輸入參數檢查

                if (string.IsNullOrEmpty(createUser))
                {
                    createUser = createUser.Trim();
                }

                if (createData == null)
                {
                    result.Data += "未填寫分期預設邏輯資料。";
                }

                if (string.IsNullOrEmpty(createUser))
                {
                    result.Data += "未填寫建立人資訊。";
                }

                if (string.IsNullOrEmpty(result.Data) == false)
                {
                    isException = false;
                    throw new Exception("輸入參數錯誤。");
                }

                createData.ID = 0;

                // 若是資料庫資料初使化作業，則不檢查
                if (createUser != "IPP")
                {
                    // 檢查新增資料是否可以儲存
                    checkDefaultRule = this.CheckDefaultRules(new List<DefaultInstallment>() { createData });

                    if (checkDefaultRule.IsSuccess == false)
                    {
                        result.Data = checkDefaultRule.Message;

                        isException = false;
                        throw new Exception(checkDefaultRule.Message);
                    }

                    checkDefaultRule = null;
                }

                #endregion 輸入參數檢查

                #region 新增分期預設邏輯設定

                // 組合 DB Model
                dbModel = new Models.DBModels.TWSQLDB.ItemInstallmentRule();
                dbModel.Price = createData.Price;
                dbModel.InstallmentID = this._iBankInstallmentRepoAdapter.GetAvailableInstallments().Where(x => x.Value == createData.Installment).Select(x => x.ID).First();
                dbModel.Rate = createData.Rate;

                // 新增分期預設邏輯設定
                createdID = this._iItemInstallmentRuleRepoAdapter.Create(dbModel, createUser);

                if (createdID == -1)
                {
                    result.Data = "新增失敗。";
                    isException = false;
                    throw new Exception("Create adapter error.");
                }

                dbModel = null;

                #endregion 新增分期預設邏輯設定

                result.IsSuccess = true;
                result.Message = "新增分期預設邏輯設定成功。";
                result.Error = null;
            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.Message = "新增分期預設邏輯設定失敗。";

                if (isException)
                {
                    result.Error.Detail = ex.ToString();
                }
                else
                {
                    result.Error.Detail = ex.Message;
                }
            }
            finally
            {
                if (checkDefaultRule != null)
                {
                    checkDefaultRule = null;
                }

                if (dbModel != null)
                {
                    dbModel = null;
                }
            }

            return result;
        }

        /// <summary>
        /// 更新分期預設邏輯設定
        /// </summary>
        /// <param name="updateDataCell">要更新的分期預設邏輯設定</param>
        /// <param name="updateUser">更新人</param>
        /// <returns>錯誤訊息</returns>
        public ResponseMessage<List<string>> UpdateDefaultRule(List<DefaultInstallment> updateDataCell, string updateUser)
        {
            ResponseMessage<List<string>> result = new ResponseMessage<List<string>>();
            result.Data = new List<string>();
            ResponseMessage<List<DefaultInstallment>> checkDefaultRule = null;
            List<Models.DBModels.TWSQLDB.Installment> installmentCell = null;
            Models.DBModels.TWSQLDB.ItemInstallmentRule dbModel = null;

            // 更新 SerialNumber 結果
            bool addSerialNumber = false;
            
            // 更新 SerialNumber 失敗的 ID 清單
            List<int> errorIdCell = null;

            // 新增到 DB 的結果
            int createResult = -1;

            // 是否真的發生 Exception
            bool isException = true;

            try
            {
                #region 輸入參數檢查

                if (string.IsNullOrEmpty(updateUser))
                {
                    updateUser = updateUser.Trim();
                }

                if (updateDataCell == null || updateDataCell.Count == 0)
                {
                    result.Data.Add("未填寫分期預設邏輯資料。");
                }

                if (string.IsNullOrEmpty(updateUser))
                {
                    result.Data.Add("未填寫更新人資訊。");
                }

                if (result.Data.Count != 0)
                {
                    isException = false;
                    throw new Exception("輸入參數錯誤。");
                }

                #endregion 輸入參數檢查

                // 檢查更新資料是否可以儲存
                checkDefaultRule = this.CheckDefaultRules(updateDataCell);

                if (checkDefaultRule.IsSuccess == false)
                {
                    result.Data.Add(checkDefaultRule.Message);

                    isException = false;
                    throw new Exception(checkDefaultRule.Message);
                }

                // 取得可以儲存的部份
                updateDataCell = checkDefaultRule.Data;

                checkDefaultRule = null;

                if (updateDataCell.Count == 0)
                {
                    isException = false;
                    result.Data.Add("沒有更新項目。");
                    throw new Exception("沒有更新項目。");
                }

                #region 將 SerialNumber +1

                foreach (DefaultInstallment updateData in updateDataCell)
                {
                    addSerialNumber = false;

                    addSerialNumber = this._iItemInstallmentRuleRepoAdapter.AddSerialNumber(updateData.ID, updateUser);

                    if (addSerialNumber == false)
                    {
                        if(errorIdCell == null)
                        {
                            errorIdCell = new List<int>();
                        }

                        errorIdCell.Add(updateData.ID);

                        if(updateData.Price == -1)
                        {
                            result.Data.Add("更新全部商品開放分期失敗。");
                        }
                        else
                        {
                            result.Data.Add(string.Format("{0} 期更新失敗。", updateData.Installment));
                        }
                    }
                }

                #endregion 將 SerialNumber +1

                #region 將更新 SerialNumber 失敗的資料，從更新清單中移除

                if (errorIdCell != null)
                {
                    updateDataCell = updateDataCell.Where(x => !errorIdCell.Contains(x.ID)).ToList();
                }

                errorIdCell = null;

                if (updateDataCell.Count == 0)
                {
                    throw new Exception("更新 SerialNumber 失敗。");
                }

                #endregion 將更新 SerialNumber 失敗的資料，從更新清單中移除

                // 取得信用卡分期資料
                installmentCell = this._iBankInstallmentRepoAdapter.GetAvailableInstallments().ToList();

                // 新增分期預設邏輯設定
                foreach (DefaultInstallment updateData in updateDataCell)
                {
                    dbModel = new Models.DBModels.TWSQLDB.ItemInstallmentRule();
                    dbModel.Price = updateData.Price;
                    dbModel.InstallmentID = installmentCell.Where(x => x.Value == updateData.Installment).Select(x => x.ID).First();
                    dbModel.Rate = updateData.Rate;

                    createResult = this._iItemInstallmentRuleRepoAdapter.Create(dbModel,updateUser);

                    if(createResult == -1)
                    {
                        if (updateData.Price == -1)
                        {
                            result.Data.Add("更新全部商品開放分期失敗。");
                        }
                        else
                        {
                            result.Data.Add(string.Format("{0} 期更新失敗。", updateData.Installment));
                        }
                    }

                    dbModel = null;
                }

                updateDataCell = null;

                if (result.Data.Count > 0)
                {
                    isException = false;
                    throw new Exception("Update adapter error.");
                }

                result.IsSuccess = true;
                result.Message = "更新分期預設邏輯設定成功。";
                result.Error = null;
            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.Message = "更新分期預設邏輯設定失敗。";

                if (isException)
                {
                    result.Error.Detail = ex.ToString();
                }
                else
                {
                    result.Error.Detail = ex.Message;
                }
            }

            return result;
        }

        /// <summary>
        /// 檢查目前資料是否可以儲存
        /// </summary>
        /// <param name="checkDataCell">分期預設邏輯設定項目</param>
        /// <param name="updateIdCell">要被更新的項目編號</param>
        /// <returns>可以儲存的分期預設邏輯設定項目</returns>
        private ResponseMessage<List<DefaultInstallment>> CheckDefaultRules(List<DefaultInstallment> checkDataCell)
        {
            ResponseMessage<List<DefaultInstallment>> result = new ResponseMessage<List<DefaultInstallment>>();

            // 目前分期預設邏輯設定
            ResponseMessage<List<DefaultInstallment>> defaultRulesCell = null;

            // 待檢查項目 ID 清單
            List<int> checkDataIdCell = null;

            int minInstallment = -1;

            bool isAllItemOpenInstallmentError = false;

            // 檢查是否符合賣場售價區間
            ResponseMessage<string> checkPriceRange = null;

            // 是否真的發生 Exception
            bool isException = true;

            try
            {
                #region 輸入參數檢查

                if (checkDataCell == null || checkDataCell.Count == 0)
                {
                    isException = false;
                    result.Message = "沒有更新項目。";
                    throw new Exception("未傳入參數。");
                }

                if (checkDataCell.Count > 1)
                {
                    // 將待檢查項目，依分期期數排序
                    checkDataCell = checkDataCell.OrderBy(x => x.Installment).ToList();
                }

                #endregion 輸入參數檢查

                #region 取得目前分期預設邏輯設定

                // 取得目前分期預設邏輯設定
                defaultRulesCell = this.GetDefaultRules();

                if (defaultRulesCell.IsSuccess == false)
                {
                    isException = false;
                    throw new Exception(defaultRulesCell.Message);
                }

                #endregion 取得目前分期預設邏輯設定

                if (defaultRulesCell.Data.Count > 0)
                {
                    #region 開始檢查前的資料設定

                    if (checkDataCell.Any(x => x.Price == -1))
                    {
                        // 全部商品開放分期期數若沒有變動，先將此筆資料從更新清單中移除
                        if (checkDataCell.Where(x => x.Price == -1).Select(x => x.Installment).First() == defaultRulesCell.Data.Where(x => x.Price == -1).Select(x => x.Installment).First())
                        {
                            checkDataCell.Remove(checkDataCell.Where(x => x.Price == -1).First());
                        }
                        else
                        {
                            // 否則先將全部商品開放分期期數的 ID 補上
                            checkDataCell.Where(x => x.Price == -1).First().ID = defaultRulesCell.Data.Where(x => x.Price == -1).Select(x => x.ID).First();
                        }
                    }

                    if (checkDataCell.Count == 0)
                    {
                        isException = false;
                        result.Message = "沒有更新項目。";
                        throw new Exception("沒有更新項目。");
                    }

                    // 先將目前分期預設邏輯設定，排除與更新清單中相同 ID 的資料，再對要更新的資料，依分期數由小到大進行新增邏輯檢查
                    // 取得要被更新的 ID 清單
                    checkDataIdCell = checkDataCell.Where(x => x.ID > 0).Select(x => x.ID).ToList();

                    // 將目前分期預設邏輯設定，排除與更新清單中相同 ID 的資料
                    defaultRulesCell.Data = defaultRulesCell.Data.Where(x => !checkDataIdCell.Contains(x.ID)).ToList();

                    checkDataIdCell = null;

                    #endregion 開始檢查前的資料設定

                    foreach (DefaultInstallment checkData in checkDataCell)
                    {
                        // 若是全部商品開放分期期數，則檢查是否有大於等於其它的分期設定
                        if (checkData.Price == -1)
                        {
                            // 檢查要更新的資料
                            if (checkDataCell.Where(x => x.Price != -1).Any(x => x.Installment <= checkData.Installment))
                            {
                                isAllItemOpenInstallmentError = true;

                                minInstallment = checkDataCell.Where(x => x.Price != -1).Min(x => x.Installment);
                            }

                            if (defaultRulesCell.Data.Any(x => x.Installment <= checkData.Installment))
                            {
                                isAllItemOpenInstallmentError = true;

                                if (minInstallment == -1)
                                {
                                    minInstallment = defaultRulesCell.Data.Min(x => x.Installment);
                                }
                                else if (minInstallment > defaultRulesCell.Data.Min(x => x.Installment))
                                {
                                    minInstallment = defaultRulesCell.Data.Min(x => x.Installment);
                                }
                            }

                            if (isAllItemOpenInstallmentError)
                            {
                                result.Message += string.Format("全部商品開放分期期數，不可以大於等於目前最低預設分期 " + minInstallment + " 期。");
                            }

                            isAllItemOpenInstallmentError = false;
                            minInstallment = -1;

                            // 不管全部商品開放分期項目是否有錯，都要將全部商品開放分期項目加到目前分期預設邏輯設定中，以利接下來的新增邏輯判斷，所以不使用 continue
                        }
                        else
                        {
                            // 檢查要設定的分期期數，是否小於等於全部商品開放分期期數
                            if (checkData.Installment <= defaultRulesCell.Data.Where(x => x.Price == -1).Select(x => x.Installment).First())
                            {
                                 result.Message += string.Format("分期期數的設定，不可小於等於全部商品開放的分期期數。");
                                continue;
                            }

                            // 檢查要設定的分期期數是否已存在
                            if (defaultRulesCell.Data.Any(x => x.Installment == checkData.Installment))
                            {
                                 result.Message += string.Format("分期期數 {0} 已存在。", checkData.Installment);
                                continue;
                            }

                            // 檢查賣場售價是否可以儲存
                            checkPriceRange = this.CheckPriceRange(defaultRulesCell.Data, checkData.Installment, checkData.Price);

                            if (checkPriceRange.IsSuccess == false)
                            {
                                 result.Message += string.Format("分期期數 {0} {1}", checkData.Installment, checkPriceRange.Data);
                                continue;
                            }
                        }

                        checkPriceRange = null;

                        // 將已檢查通過的加到目前分期預設邏輯設定中，以利接下來的新增邏輯判斷
                        defaultRulesCell.Data.Add(checkData);
                        defaultRulesCell.Data = defaultRulesCell.Data.OrderBy(x => x.Installment).ToList();
                    }
                }

                defaultRulesCell = null;

                if (string.IsNullOrEmpty(result.Message) == false)
                {
                    isException = false;
                    throw new Exception(string.Format("參數值錯誤：{0}", result.Message));
                }

                result.Data = checkDataCell;
                result.IsSuccess = true;
                result.Message = "可以儲存。";
                result.Error = null;
            }
            catch (Exception ex)
            {
                result.Data = null;
                result.IsSuccess = false;

                if (string.IsNullOrEmpty(result.Message))
                {
                    result.Message = "設定值錯誤。";
                }

                if (isException)
                {
                    result.Error.Detail = ex.ToString();
                }
                else
                {
                    result.Error.Detail = ex.Message;
                }
            }
            finally
            {
                if (checkPriceRange != null)
                {
                    checkPriceRange = null;
                }

                if (defaultRulesCell != null)
                {
                    defaultRulesCell = null;
                }
            }

            return result;
        }

        /// <summary>
        /// 檢查賣場售價是否可以儲存
        /// </summary>
        /// <param name="defaultRulesCell">目前的分期預設邏輯設定</param>
        /// <param name="installment">要設定的分期期數</param>
        /// <param name="price">要設定的賣場售價</param>
        /// <returns>錯誤訊息</returns>
        private ResponseMessage<string> CheckPriceRange(List<DefaultInstallment> defaultRulesCell, int installment, int price)
        {
            ResponseMessage<string> result = new ResponseMessage<string>();

            // 目前預設分期項目
            DefaultInstallment tempDefaultInstallment = null;

            // 是否真的發生 Exception
            bool isException = true;

            try
            {
                // 排除全部商品開放設定，並依分期期數排序
                defaultRulesCell = defaultRulesCell.Where(x => x.Price != -1).OrderBy(x => x.Installment).ToList();

                if (defaultRulesCell.Count > 0)
                {
                    // 取得前一個預設分期項目
                    tempDefaultInstallment = defaultRulesCell.FindLast(x => x.Installment < installment);

                    if (tempDefaultInstallment != null)
                    {
                        // 檢查要被設定的賣場售價，是否小於等於前一個預設分期項目的賣場售價
                        if (price <= tempDefaultInstallment.Price)
                        {
                            result.Data = string.Format("賣場售價不可小於等於 {0}。", tempDefaultInstallment.Price);

                            isException = false;
                            throw new Exception(string.Format("要被設定的賣場售價為 {0}，賣場售價不可小於等於 {1}。",
                                price,
                                tempDefaultInstallment.Price));
                        }
                    }

                    tempDefaultInstallment = null;

                    // 取得後一個預設分期項目
                    tempDefaultInstallment = defaultRulesCell.Where(x => x.Installment > installment).FirstOrDefault();

                    if (tempDefaultInstallment != null)
                    {
                        // 檢查要被設定的賣場售價，是否大於等於後一個預設分期項目的賣場售價
                        if (price >= tempDefaultInstallment.Price)
                        {
                            result.Data = string.Format("賣場售價不可大於等於 {0}。", tempDefaultInstallment.Price);

                            isException = false;
                            throw new Exception(string.Format("要被設定的賣場售價為 {0}，賣場售價不可小於等於 {1}。",
                                price,
                                tempDefaultInstallment.Price));
                        }
                    }

                    tempDefaultInstallment = null;
                }

                result.IsSuccess = true;
                result.Message = "可以儲存賣場售價。";
                result.Error = null;
            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.Message = "賣場售價錯誤，無法儲存。";

                if (isException)
                {
                    result.Error.Detail = ex.ToString();
                }
                else
                {
                    result.Error.Detail = ex.Message;
                }
            }

            return result;
        }

        /// <summary>
        /// 刪除分期預設邏輯設定
        /// </summary>
        /// <param name="id">分期預設項目 ID</param>
        /// <param name="deleteUser">刪除人</param>
        /// <returns>錯誤訊息</returns>
        public ResponseMessage<string> DeleteDefaultRule(int id, string deleteUser)
        {
            ResponseMessage<string> result = new ResponseMessage<string>();

            // 是否真的發生 Exception
            bool isException = true;

            try
            {
                #region 輸入檢查

                if (string.IsNullOrEmpty(deleteUser))
                {
                    deleteUser = deleteUser.Trim();
                }

                if (id <= 0)
                {
                    result.Data += "ID 錯誤。";
                }

                if (string.IsNullOrEmpty(deleteUser))
                {
                    result.Data += "未填寫刪除人資訊。";
                }

                if (string.IsNullOrEmpty(result.Data) == false)
                {
                    isException = false;
                    throw new Exception("輸入參數錯誤。");
                }

                #endregion 輸入檢查

                result.IsSuccess = this._iItemInstallmentRuleRepoAdapter.Delete(id, deleteUser);

                if (result.IsSuccess == false)
                {
                    isException = false;
                    result.Data = "刪除失敗。";
                    throw new Exception("Adapter error.");
                }

                result.Message = "刪除分期預設邏輯設定成功。";
                result.Data = null;
                result.Error = null;
            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.Message = "刪除分期預設邏輯設定失敗。";

                if (isException)
                {
                    result.Error.Detail = ex.ToString();
                }
                else
                {
                    result.Error.Detail = ex.Message;
                }
            }

            return result;
        }

        #endregion 分期預設邏輯設定

        #region 分期設定

        /// <summary>
        /// 取得賣場資訊
        /// </summary>
        /// <param name="searchCondition">搜尋條件</param>
        /// <returns>賣場資訊</returns>
        public ResponseMessage<ItemForAddTopRuleReturn> GetItemCell(ItemForAddTopRuleSearchCondition searchCondition)
        {
            ResponseMessage<ItemForAddTopRuleReturn> result = new ResponseMessage<ItemForAddTopRuleReturn>();
            result.Data = new ItemForAddTopRuleReturn();
            result.Data.ItemForAddTopRuleCell = new List<ItemForAddTopRule>();

            IQueryable<ItemForAddTopRule> queryableDataCell = null;

            // 售價和分期數的對照表
            ResponseMessage<List<DefaultInstallment>> defaultRuleCell = null;

            List<ItemForAddTopRule> resultData_temp = null;

            try
            {
                queryableDataCell = (from item in this._iItemRepoAdapter.GetAll()
                                     join itemDisplayPrice in this._iItemDisplayPriceRepoAdapter.GetAll() on item.ID equals itemDisplayPrice.ItemID
                                     join product in this._iProductRepoAdapter.GetAll() on item.ProductID equals product.ID
                                     join seller in this._iSellerRepoAdapter.GetAll() on item.SellerID equals seller.ID into seller_temp
                                     from seller in seller_temp.DefaultIfEmpty()
                                     select new ItemForAddTopRule()
                                     {
                                         ItemID = item.ID,
                                         ItemName = item.Name,
                                         Price = itemDisplayPrice.DisplayPrice,
                                         Cost = product.Cost.HasValue? product.Cost.Value : 0,
                                         CategoryID_Layer2 = item.CategoryID,
                                         SellerID = item.SellerID,
                                         SellerName = seller.Name
                                     }).AsQueryable();

                #region 依搜尋條件篩選

                // 依賣場編號篩選
                if(searchCondition.ItemID > -1)
                {
                    queryableDataCell = queryableDataCell.Where(x => x.ItemID == searchCondition.ItemID).AsQueryable();
                }

                // 依供應商編號篩選
                if (searchCondition.SellerID > -1)
                {
                    queryableDataCell = queryableDataCell.Where(x => x.SellerID == searchCondition.SellerID).AsQueryable();
                }

                // 依分類篩選
                if (searchCondition.CategoryID_Layer2 > -1)
                {
                    queryableDataCell = queryableDataCell.Where(x => x.CategoryID_Layer2 == searchCondition.CategoryID_Layer2).AsQueryable();
                }
                else
                {
                    if (searchCondition.ItemID <= 0)
                    {
                        queryableDataCell = queryableDataCell.Where(x => x.ItemID == -1).AsQueryable();
                    }
                }

                #endregion 依搜尋條件篩選

                result.Data.DataCount = queryableDataCell.Count();

                if (result.Data.DataCount > 0)
                {
                    #region 取得分期預設邏輯設定

                    // 取得分期預設邏輯設定
                    defaultRuleCell = this.GetDefaultRules();

                    if (defaultRuleCell == null || defaultRuleCell.IsSuccess == false || defaultRuleCell.Data == null || defaultRuleCell.Data.Count == 0)
                    {
                        throw new Exception("取得分期預設邏輯設定失敗。");
                    }
                    else
                    {
                        // 將售價和分期數的對照表，依售價由高到低排序
                        if (defaultRuleCell.Data.Count > 1)
                        {
                            defaultRuleCell.Data = defaultRuleCell.Data.OrderByDescending(x => x.Price).ToList();
                        }
                    }

                    #endregion 取得分期預設邏輯設定

                    // 要做 Skip 和 Take 前，要先做過 OrderBy
                    queryableDataCell = queryableDataCell.OrderBy(x => x.ItemID).AsQueryable();

                    for (int i = 0; i <= (result.Data.DataCount / 1000); i++)
                    {
                        resultData_temp = queryableDataCell.Skip(i * 1000).Take(1000).ToList();

                        if (resultData_temp != null && resultData_temp.Count > 0)
                        {
                            foreach (ItemForAddTopRule item in resultData_temp)
                            {
                                #region 計算賣場毛利率

                                item.ItemGrossMargin = this.GetItemGrossMargin(item.Price, item.Cost);

                                #endregion 計算賣場毛利率

                                #region 填寫最高預設分期數

                                // 依售價填寫最高預設分期數
                                foreach (DefaultInstallment defaultRule in defaultRuleCell.Data)
                                {
                                    if (item.Price >= defaultRule.Price)
                                    {
                                        item.TopInstallment = defaultRule.Installment;
                                        break;
                                    }
                                }

                                #endregion 填寫最高預設分期數

                                #region 在 SellerName 後面加上 (SellerID)

                                item.SellerName += string.Format("({0})", item.SellerID);

                                #endregion 在 SellerName 後面加上 (SellerID)
                            }

                            result.Data.ItemForAddTopRuleCell.AddRange(resultData_temp);
                        }

                        resultData_temp = null;
                    }

                    defaultRuleCell = null;

                    #region 排序

                    switch (searchCondition.SortMember)
                    {
                        default:
                        case "ItemID":
                            {
                                if (searchCondition.SortDirection == ListSortDirection.Ascending)
                                {
                                    result.Data.ItemForAddTopRuleCell = result.Data.ItemForAddTopRuleCell.OrderBy(x => x.ItemID).ToList();
                                }
                                else
                                {
                                    result.Data.ItemForAddTopRuleCell = result.Data.ItemForAddTopRuleCell.OrderByDescending(x => x.ItemID).ToList();
                                }

                                break;
                            }
                        case "Price":
                            {
                                if (searchCondition.SortDirection == ListSortDirection.Ascending)
                                {
                                    result.Data.ItemForAddTopRuleCell = result.Data.ItemForAddTopRuleCell.OrderBy(x => x.Price).ToList();
                                }
                                else
                                {
                                    result.Data.ItemForAddTopRuleCell = result.Data.ItemForAddTopRuleCell.OrderByDescending(x => x.Price).ToList();
                                }

                                break;
                            }
                        case "ItemGrossMargin":
                            {
                                if (searchCondition.SortDirection == ListSortDirection.Ascending)
                                {
                                    result.Data.ItemForAddTopRuleCell = result.Data.ItemForAddTopRuleCell.OrderBy(x => x.ItemGrossMargin).ToList();
                                }
                                else
                                {
                                    result.Data.ItemForAddTopRuleCell = result.Data.ItemForAddTopRuleCell.OrderByDescending(x => x.ItemGrossMargin).ToList();
                                }

                                break;
                            }
                        case "TopInstallment":
                            {
                                if (searchCondition.SortDirection == ListSortDirection.Ascending)
                                {
                                    result.Data.ItemForAddTopRuleCell = result.Data.ItemForAddTopRuleCell.OrderBy(x => x.TopInstallment).ToList();
                                }
                                else
                                {
                                    result.Data.ItemForAddTopRuleCell = result.Data.ItemForAddTopRuleCell.OrderByDescending(x => x.TopInstallment).ToList();
                                }

                                break;
                            }
                        case "SellerID":
                            {
                                if (searchCondition.SortDirection == ListSortDirection.Ascending)
                                {
                                    result.Data.ItemForAddTopRuleCell = result.Data.ItemForAddTopRuleCell.OrderBy(x => x.SellerID).ToList();
                                }
                                else
                                {
                                    result.Data.ItemForAddTopRuleCell = result.Data.ItemForAddTopRuleCell.OrderByDescending(x => x.SellerID).ToList();
                                }

                                break;
                            }
                    }

                    #endregion 排序

                    // 分頁處理
                    result.Data.ItemForAddTopRuleCell = result.Data.ItemForAddTopRuleCell.Skip(searchCondition.Page * searchCondition.PageSize).Take(searchCondition.PageSize).ToList();
                }

                result.IsSuccess = true;
                result.Message = string.Format("取得賣場資訊成功，已取得 {0} 筆資料。", (result.Data.DataCount > 0)? result.Data.ItemForAddTopRuleCell.Count : 0);
                result.Error = null;
            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.Message = "取得賣場資訊失敗。";

                result.Data.DataCount = 0;
                result.Data.ItemForAddTopRuleCell = null;

                logger.Error(string.Format("取得賣場資訊失敗(分期新增部份)。Exception = {0}", ex.ToString()));
            }

            return result;
        }

        /// <summary>
        /// 取得賣場最高的分期設定
        /// </summary>
        /// <param name="searchCondition">搜尋條件</param>
        /// <returns>分期設定</returns>
        public ResponseMessage<ItemTopInstallmentReturn> GetItemTopInstallments(ItemTopInstallmentSearchCondition searchCondition)
        {
            ResponseMessage<ItemTopInstallmentReturn> result = new ResponseMessage<ItemTopInstallmentReturn>();
            result.Data = new ItemTopInstallmentReturn();
            result.Data.ItemTopInstallmentCell = new List<ItemTopInstallment>();
            IQueryable<ItemTopInstallment> itemTopInstallmentQueryable = null;
            List<ItemTopInstallment> resultDataCell_Temp = null;

            try
            {
                #region GetAll

                itemTopInstallmentQueryable = (from iItemTopInstallment in this._iItemTopInstallmentRepoAdapter.GetAll()
                                               join item in this._iItemRepoAdapter.GetAll() on iItemTopInstallment.ItemID equals item.ID
                                               join itemDisplayPrice in this._iItemDisplayPriceRepoAdapter.GetAll() on iItemTopInstallment.ItemID equals itemDisplayPrice.ItemID
                                               join product in this._iProductRepoAdapter.GetAll() on item.ProductID equals product.ID
                                               join seller in this._iSellerRepoAdapter.GetAll() on item.SellerID equals seller.ID into seller_temp
                                               from seller in seller_temp.DefaultIfEmpty()
                                               select new ItemTopInstallment()
                                               {
                                                   ID = iItemTopInstallment.ID,
                                                   Edition = iItemTopInstallment.Edition,
                                                   ItemID = iItemTopInstallment.ItemID,
                                                   ItemName = item.Name,
                                                   TopInstallment = iItemTopInstallment.TopInstallment,
                                                   StartDate = iItemTopInstallment.StartDate,
                                                   EndDate = iItemTopInstallment.EndDate,
                                                   Status = iItemTopInstallment.Status,
                                                   SellerID = item.SellerID,
                                                   SellerName = seller.Name,
                                                   Category_Layer2 = item.CategoryID,
                                                   Cost = product.Cost.HasValue? product.Cost.Value : 0,
                                                   Price = itemDisplayPrice.DisplayPrice
                                               }).AsQueryable();

                #endregion GetAll

                #region 依查詢條件篩選

                #region 依賣場編號篩選

                // 依賣場編號篩選
                if (searchCondition.ItemID > 0)
                {
                    itemTopInstallmentQueryable = itemTopInstallmentQueryable.Where(x => x.ItemID == searchCondition.ItemID).AsQueryable();
                }

                #endregion 依賣場編號篩選

                #region 依狀態篩選

                // 依狀態篩選
                switch (searchCondition.Status)
                {
                    case (int)ItemTopInstallmentStatus.Enable:
                    case (int)ItemTopInstallmentStatus.Disable:
                        {
                            itemTopInstallmentQueryable = itemTopInstallmentQueryable.Where(x => x.Status == searchCondition.Status).AsQueryable();
                            break;
                        }
                    default:
                        {
                            break;
                        }
                }

                #endregion 依狀態篩選

                #region 依商家 ID 篩選

                // 依商家 ID 篩選
                if (searchCondition.SellerID > 0)
                {
                    itemTopInstallmentQueryable = itemTopInstallmentQueryable.Where(x => x.SellerID == searchCondition.SellerID).AsQueryable();
                }

                #endregion 依商家 ID 篩選

                #region 依時間區間篩選

                if (searchCondition.StartDate.HasValue)
                {
                    searchCondition.StartDate = new DateTime(searchCondition.StartDate.Value.Year, searchCondition.StartDate.Value.Month, searchCondition.StartDate.Value.Day, 0, 0, 0, DateTimeKind.Local);
                }

                if (searchCondition.EndDate.HasValue)
                {
                    // 將結束日期加一天
                    searchCondition.EndDate = searchCondition.EndDate.Value.AddDays(1);
                    searchCondition.EndDate = new DateTime(searchCondition.EndDate.Value.Year, searchCondition.EndDate.Value.Month, searchCondition.EndDate.Value.Day, 0, 0, 0, DateTimeKind.Local);
                }

                // 依時間類型及時間區間篩選
                switch (searchCondition.DateType)
                {
                    case (int)InstallmentSearchDateType.分期開始時間:
                        {
                            if (searchCondition.StartDate.HasValue)
                            {
                                itemTopInstallmentQueryable = itemTopInstallmentQueryable.Where(x => x.StartDate >= searchCondition.StartDate.Value).AsQueryable();
                            }

                            if (searchCondition.EndDate.HasValue)
                            {
                                itemTopInstallmentQueryable = itemTopInstallmentQueryable.Where(x => x.StartDate < searchCondition.EndDate.Value).AsQueryable();
                            }

                            break;
                        }

                    case (int)InstallmentSearchDateType.分期結束時間:
                        {
                            if (searchCondition.StartDate.HasValue)
                            {
                                itemTopInstallmentQueryable = itemTopInstallmentQueryable.Where(x => x.EndDate >= searchCondition.StartDate.Value).AsQueryable();
                            }

                            if (searchCondition.EndDate.HasValue)
                            {
                                itemTopInstallmentQueryable = itemTopInstallmentQueryable.Where(x => x.EndDate < searchCondition.EndDate.Value).AsQueryable();
                            }

                            break;
                        }

                    default:
                    case (int)InstallmentSearchDateType.活動時間:
                        {
                            if (searchCondition.StartDate.HasValue)
                            {
                                itemTopInstallmentQueryable = itemTopInstallmentQueryable.Where(x => x.StartDate >= searchCondition.StartDate.Value).AsQueryable();
                            }

                            if (searchCondition.EndDate.HasValue)
                            {
                                itemTopInstallmentQueryable = itemTopInstallmentQueryable.Where(x => x.EndDate < searchCondition.EndDate.Value).AsQueryable();
                            }

                            break;
                        }
                }

                #endregion 依時間區間篩選

                #region 依分類篩選

                // 依主分類篩選
                if (searchCondition.Category_Layer2 > 0)
                {
                    itemTopInstallmentQueryable = itemTopInstallmentQueryable.Where(x => x.Category_Layer2 == searchCondition.Category_Layer2).AsQueryable();
                }

                #endregion 依分類篩選

                #endregion 依查詢條件篩選

                result.Data.DataCount = itemTopInstallmentQueryable.Count();

                if (result.Data.DataCount > 0)
                {
                    #region ToList()

                    // 要做 Skip 和 Take 前，要先做過 OrderBy
                    itemTopInstallmentQueryable = itemTopInstallmentQueryable.OrderBy(x => x.ItemID).AsQueryable();

                    for (int i = 0; i <= (result.Data.DataCount / 1000); i++)
                    {
                        resultDataCell_Temp = itemTopInstallmentQueryable.Skip(i * 1000).Take(1000).ToList();

                        if (resultDataCell_Temp != null && resultDataCell_Temp.Count > 0)
                        {
                            foreach (ItemTopInstallment itemTopInstallment in resultDataCell_Temp)
                            {
                                #region 計算賣場毛利率

                                itemTopInstallment.GrossMargin = this.GetItemGrossMargin(itemTopInstallment.Price, itemTopInstallment.Cost);

                                #endregion 計算賣場毛利率

                                #region 在 SellerName 後面加上 (SellerID)

                                itemTopInstallment.SellerName += string.Format("({0})", itemTopInstallment.SellerID);

                                #endregion 在 SellerName 後面加上 (SellerID)
                            }

                            result.Data.ItemTopInstallmentCell.AddRange(resultDataCell_Temp);
                        }

                        resultDataCell_Temp = null;
                    }

                    itemTopInstallmentQueryable = null;

                    #endregion ToList()

                    #region 排序

                    switch (searchCondition.SortMember)
                    {
                        default:
                        case "ItemID":
                            {
                                if (searchCondition.SortDirection == ListSortDirection.Ascending)
                                {
                                    result.Data.ItemTopInstallmentCell = result.Data.ItemTopInstallmentCell.OrderBy(x => x.ItemID).ToList();
                                }
                                else
                                {
                                    result.Data.ItemTopInstallmentCell = result.Data.ItemTopInstallmentCell.OrderByDescending(x => x.ItemID).ToList();
                                }

                                break;
                            }

                        case "SellerID":
                            {
                                if (searchCondition.SortDirection == ListSortDirection.Ascending)
                                {
                                    result.Data.ItemTopInstallmentCell = result.Data.ItemTopInstallmentCell.OrderBy(x => x.SellerID).ToList();
                                }
                                else
                                {
                                    result.Data.ItemTopInstallmentCell = result.Data.ItemTopInstallmentCell.OrderByDescending(x => x.SellerID).ToList();
                                }

                                break;
                            }

                        case "GrossMargin":
                            {
                                if (searchCondition.SortDirection == ListSortDirection.Ascending)
                                {
                                    result.Data.ItemTopInstallmentCell = result.Data.ItemTopInstallmentCell.OrderBy(x => x.GrossMargin).ToList();
                                }
                                else
                                {
                                    result.Data.ItemTopInstallmentCell = result.Data.ItemTopInstallmentCell.OrderByDescending(x => x.GrossMargin).ToList();
                                }

                                break;
                            }

                        case "TopInstallment":
                            {
                                if (searchCondition.SortDirection == ListSortDirection.Ascending)
                                {
                                    result.Data.ItemTopInstallmentCell = result.Data.ItemTopInstallmentCell.OrderBy(x => x.TopInstallment).ToList();
                                }
                                else
                                {
                                    result.Data.ItemTopInstallmentCell = result.Data.ItemTopInstallmentCell.OrderByDescending(x => x.TopInstallment).ToList();
                                }

                                break;
                            }

                        case "StartDate":
                            {
                                if (searchCondition.SortDirection == ListSortDirection.Ascending)
                                {
                                    result.Data.ItemTopInstallmentCell = result.Data.ItemTopInstallmentCell.OrderBy(x => x.StartDate).ToList();
                                }
                                else
                                {
                                    result.Data.ItemTopInstallmentCell = result.Data.ItemTopInstallmentCell.OrderByDescending(x => x.StartDate).ToList();
                                }

                                break;
                            }

                        case "EndDate":
                            {
                                if (searchCondition.SortDirection == ListSortDirection.Ascending)
                                {
                                    result.Data.ItemTopInstallmentCell = result.Data.ItemTopInstallmentCell.OrderBy(x => x.EndDate).ToList();
                                }
                                else
                                {
                                    result.Data.ItemTopInstallmentCell = result.Data.ItemTopInstallmentCell.OrderByDescending(x => x.EndDate).ToList();
                                }

                                break;
                            }

                        case "Status":
                            {
                                if (searchCondition.SortDirection == ListSortDirection.Ascending)
                                {
                                    result.Data.ItemTopInstallmentCell = result.Data.ItemTopInstallmentCell.OrderBy(x => x.Status).ToList();
                                }
                                else
                                {
                                    result.Data.ItemTopInstallmentCell = result.Data.ItemTopInstallmentCell.OrderByDescending(x => x.Status).ToList();
                                }

                                break;
                            }
                    }

                    #endregion 排序

                    // 分頁處理
                    result.Data.ItemTopInstallmentCell = result.Data.ItemTopInstallmentCell.Skip(searchCondition.Page * searchCondition.PageSize).Take(searchCondition.PageSize).ToList();
                }

                result.IsSuccess = true;
                result.Message = string.Format("取得賣場最高的分期設定成功，已取得 {0} 筆資料。", result.Data.ItemTopInstallmentCell.Count);
            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.Message = "取得賣場最高的分期設定失敗。";
                result.Error.Detail = ex.ToString();
            }

            return result;
        }

        /// <summary>
        /// 新增賣場最高的分期設定
        /// </summary>
        /// <param name="createDataCell">賣場最高的分期設定</param>
        /// <param name="createUser">建立人</param>
        /// <returns>錯誤訊息</returns>
        public ResponseMessage<string> CreateItemTopInstallments(List<ItemTopInstallment> createDataCell, string createUser)
        {
            ResponseMessage<string> result = new ResponseMessage<string>();
            string errorMessage = null;

            // 賣場編號
            List<int> itemIdCell = null;

            // 已存在 DB 的賣場最高的分期設定
            List<Models.DBModels.TWSQLDB.ItemTopInstallment> existItemTopInstallmentCell = null;

            List<Models.DBModels.TWSQLDB.ItemTopInstallment> createDataCell_DbModel = null;

            try
            {
                #region 輸入參數檢查

                if (createDataCell == null || createDataCell.Count == 0)
                {
                    errorMessage += "沒有新增項目。";
                }

                if (createDataCell.Any(x => x.StartDate > x.EndDate))
                {
                    errorMessage += "開放期間的結束日期，不可以早於開始日期。";
                }

                if (string.IsNullOrEmpty(createUser) == false)
                {
                    createUser = createUser.Trim();
                }

                if (string.IsNullOrEmpty(createUser))
                {
                    errorMessage += "未填寫建立人資訊。";
                }

                if (string.IsNullOrEmpty(errorMessage) == false)
                {
                    result.Data = errorMessage;
                    throw new Exception(result.Data);
                }

                #endregion 輸入參數檢查

                // 取得所有要新增分期的賣場編號
                itemIdCell = createDataCell.Select(x => x.ItemID).Distinct().ToList();

                // 檢查是否有賣場編號已存在且啟用的資料
                existItemTopInstallmentCell = this._iItemTopInstallmentRepoAdapter.GetAll()
                    .Where(x => x.Status == (int)Models.DBModels.TWSQLDB.ItemTopInstallmentStatus.Enable)
                    .Where(x => itemIdCell.Contains(x.ItemID))
                    .ToList();

                itemIdCell = null;

                if (existItemTopInstallmentCell != null && existItemTopInstallmentCell.Count > 0)
                {
                    foreach (ItemTopInstallment createData in createDataCell)
                    {
                        // 若錯誤訊息中，已經有相同 ItemID，就先跳過，以免出現 2 個相同 ItemID 的提示訊息
                        if (string.IsNullOrEmpty(errorMessage) == false && errorMessage.IndexOf(createData.ItemID.ToString()) != -1)
                        {
                            continue;
                        }

                        // 在已存在清單中，有相同的 ItemID，則檢查時間是否有重疊
                        if (existItemTopInstallmentCell.Any(x => x.ItemID == createData.ItemID))
                        {
                            foreach (Models.DBModels.TWSQLDB.ItemTopInstallment existData in existItemTopInstallmentCell.Where(x => x.ItemID == createData.ItemID).ToList())
                            {
                                if (this.CheckDateRange(createData.StartDate, createData.EndDate, existData.StartDate, existData.EndDate))
                                {
                                    if (string.IsNullOrEmpty(errorMessage) == false)
                                    {
                                        errorMessage += ", ";
                                    }

                                    errorMessage += createData.ItemID.ToString();
                                    break;
                                }
                            }
                        }
                    }
                }

                existItemTopInstallmentCell = null;

                if (string.IsNullOrEmpty(errorMessage) == false)
                {
                    result.Data = string.Format("賣場編號：{0} 分期開放時間重疊。", errorMessage);
                    throw new Exception(result.Data);
                }

                createDataCell_DbModel = ModelConverter.ConvertTo<List<Models.DBModels.TWSQLDB.ItemTopInstallment>>(createDataCell);

                if (createDataCell_DbModel == null || createDataCell_DbModel.Count == 0)
                {
                    throw new Exception("轉換 DB Model 失敗。");
                }

                result.IsSuccess = this._iItemTopInstallmentRepoAdapter.Create(createDataCell_DbModel, createUser);

                if (result.IsSuccess == false)
                {
                    throw new Exception("Adapter error.");
                }
                
                result.Message = "新增賣場最高的分期設定成功。";
                result.Error = null;
            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.Message = "新增賣場最高的分期設定失敗。";

                if (string.IsNullOrEmpty(result.Data))
                {
                    result.Data = result.Message;
                }

                logger.Error(string.Format("新增賣場最高的分期設定失敗。Exception = {0}", ex.ToString()));
            }
            finally
            {
                if (itemIdCell != null)
                {
                    itemIdCell = null;
                }

                if (existItemTopInstallmentCell != null)
                {
                    existItemTopInstallmentCell = null;
                }
            }

            return result;
        }

        /// <summary>
        /// 檢查分期總毛利
        /// </summary>
        /// <param name="chekcDataCell">要儲存的賣場最高分期期數清單</param>
        /// <returns>錯誤訊息</returns>
        public ResponseMessage<string> CheckRate(List<ItemTopInstallment> chekcDataCell)
        {
            ResponseMessage<string> result = new ResponseMessage<string>();
           
            // 小於 1 的警告清單
            string lessThenOneIdCell = null;

            // 小於 0 的錯誤清單
            string lessThenZeroIdCell = null;

            // 其它的讀取錯誤
            string otherErrorMessage = null;

            // 分期與各銀行最大分期利率的對照清單
            Dictionary<int,decimal> installmentAndMaxBankRate = null;

            IQueryable<Models.DBModels.TWSQLDB.ItemDisplayPrice> itemDisplayPriceCell = null;
            IQueryable<Models.DBModels.TWSQLDB.Item> itemCell = null;
            IQueryable<Models.DBModels.TWSQLDB.Product> productCell = null;

            int productId = -1;
            decimal price = 0;
            decimal cost = 0;
            decimal bankRate = -1;
            decimal rate = 0;

            try
            {
                if (chekcDataCell == null || chekcDataCell.Count == 0)
                {
                    throw new Exception("未傳入參數。");
                }

                // 取得分期與各銀行最大分期利率的對照清單
                installmentAndMaxBankRate = GetAllInstallmentAndMaxBankRate();

                if(installmentAndMaxBankRate == null || installmentAndMaxBankRate.Count == 0)
                {
                    result.Data = "取得分期與各銀行最大分期利率的對照清單失敗。";
                     throw new Exception(result.Data);
                }

                itemDisplayPriceCell = this._iItemDisplayPriceRepoAdapter.GetAll();
                itemCell = this._iItemRepoAdapter.GetAll();
                productCell = this._iProductRepoAdapter.GetAll();

                foreach (ItemTopInstallment checkData in chekcDataCell)
                {
                    productId = -1;
                    price = 0;
                    cost = 0;
                    bankRate = -1;
                    rate = 0;

                    // 取得售價
                    price = itemDisplayPriceCell.Where(x => x.ItemID == checkData.ItemID).Select(x => x.DisplayPrice).FirstOrDefault();

                    if(price <= 0)
                    {
                        otherErrorMessage += string.Format("賣場編號：{0}，取得售價失敗。", checkData.ItemID);
                        continue;
                    }

                    // 取得成本
                    productId = itemCell.Where(x => x.ID == checkData.ItemID).Select(x => x.ProductID).FirstOrDefault();
                    cost = productCell.Where(x => x.ID == productId).Select(x => x.Cost.Value).FirstOrDefault();

                    if(cost <= 0)
                    {
                        otherErrorMessage += string.Format("賣場編號：{0}，取得成本失敗。", checkData.ItemID);
                        continue;
                    }

                    // 取得銀行分期率利
                    try
                    {
                        bankRate = installmentAndMaxBankRate.Where(x => x.Key == checkData.TopInstallment).Select(x => x.Value).First();
                    }
                    catch
                    {
                        bankRate = -1;
                    }

                    if (bankRate < 0)
                    {
                        otherErrorMessage += string.Format("賣場編號：{0}，取得銀行分期利率失敗。", checkData.ItemID);
                        continue;
                    }

                    // 計算分期總毛利
                    rate = GetInstallmentRate(GetItemGrossMargin(price, cost), bankRate);

                    if(rate < 0)
                    {
                        if(string.IsNullOrEmpty(lessThenZeroIdCell) == false)
                        {
                            lessThenZeroIdCell += ", ";
                        }

                        lessThenZeroIdCell += checkData.ItemID.ToString();
                        continue;
                    }
                    else if(rate <= 1)
                    {
                        if(string.IsNullOrEmpty(lessThenOneIdCell) == false)
                        {
                            lessThenOneIdCell += ", ";
                        }

                        lessThenOneIdCell += checkData.ItemID.ToString();
                        continue;
                    }
                }

                if(string.IsNullOrEmpty(otherErrorMessage) == false)
                {
                    result.Data = otherErrorMessage;
                    throw new Exception(result.Data);
                }

                if(string.IsNullOrEmpty(lessThenZeroIdCell) == false)
                {
                    result.Data = string.Format("賣場編號：{0} 商品%-分期% < 0%。", lessThenZeroIdCell);
                    throw new Exception(result.Data);
                }

                if(string.IsNullOrEmpty(lessThenOneIdCell) == false)
                {
                    result.Data = string.Format("賣場編號：{0} 商品%-分期% <= 1%，是否確定要開放？", lessThenOneIdCell);
                }

                result.IsSuccess = true;
                result.Error = null;
            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.Message = "檢查分期總毛利失敗。";

                if(string.IsNullOrEmpty(result.Data))
                {
                    result.Data = result.Message;
                }

                logger.Error(string.Format("檢查分期獲利失敗。Exception = {0}", ex.ToString()));
            }

            return result;
        }

        /// <summary>
        /// 取得分期與各銀行最大分期利率的對照清單
        /// </summary>
        /// <returns>分期與各銀行最大分期利率的對照清單</returns>
        private Dictionary<int, decimal> GetAllInstallmentAndMaxBankRate()
        {
            Dictionary<int, decimal> result = new Dictionary<int, decimal>();

            // 分期項目
            List<Models.DBModels.TWSQLDB.Installment> installmentCell = null;

            // 分期項目的 ID
            List<int> installmentIdCell = null;

            // 各銀行分期與 PayType 的對照表
            List<Models.DBModels.TWSQLDB.BankInstallment> bankInstallmentCell = null;

            // PayType
            List<Models.DBModels.TWSQLDB.PayType> payTypeCell = new List<Models.DBModels.TWSQLDB.PayType>();

            // PayType 的 ID
            List<int> payTypeIdCell = null;

            try
            {
                // 取得所有啟用的分期項目
                installmentCell = this._iBankInstallmentRepoAdapter.GetAvailableInstallments().ToList();

                // 取得所有啟用的分期項目的 ID
                installmentIdCell = installmentCell.Select(x => x.ID).ToList();

                // 取得各銀行分期與 PayType 的對照表
                bankInstallmentCell = this._iBankInstallmentRepoAdapter.GetAll().Where(x => installmentIdCell.Contains(x.InstallmentID)).ToList();

                installmentIdCell = null;

                // 取得 PayType
                payTypeCell = this._iBankInstallmentRepoAdapter.GetAllAvailableInstallmentPaytypes().ToList();

                foreach (Models.DBModels.TWSQLDB.Installment installment in installmentCell)
                {
                    // 取得同一個分期的 PayTypeID
                    payTypeIdCell = bankInstallmentCell.Where(x => x.InstallmentID == installment.ID).Select(x => x.PayTypeID).ToList();

                    if (payTypeCell == null || payTypeCell.Count == 0)
                    {
                        continue;
                    }

                    // 取得同一個分期的 PayType 最大的銀行利率
                    List<TWNewEgg.Models.DBModels.TWSQLDB.PayType> payTypes = new List<Models.DBModels.TWSQLDB.PayType>();
                    payTypes = payTypeCell.Where(x => payTypeIdCell.Contains(x.ID)).ToList();
                    if (payTypes.Count > 0)
                    {
                        result.Add(installment.Value, payTypes.Max(x => x.InsRateForBank) * 100);
                    }

                    payTypeIdCell = null;
                }

                payTypeCell = null;
                bankInstallmentCell = null;
                installmentCell = null;
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("取得分期與各銀行最大分期利率的對照清單發生錯誤(exception)。Exception = {0}", ex.ToString()));
            }
            finally
            {
                if (payTypeCell != null)
                {
                    payTypeCell = null;
                }

                if (bankInstallmentCell != null)
                {
                    bankInstallmentCell = null;
                }

                if (installmentCell != null)
                {
                    installmentCell = null;
                }
            }

            return result;
        }

        /// <summary>
        /// 計算賣場的毛利率
        /// </summary>
        /// <param name="price">售價</param>
        /// <param name="cost">成本</param>
        /// <returns>賣場的毛利率</returns>
        private decimal GetItemGrossMargin(decimal price, decimal cost)
        {
            decimal result = 0;

            if (price != 0)
            {
                result = Decimal.Round(((price - cost) / price) * 100, 2);
            }

            return result;
        }

        /// <summary>
        /// 計算總毛利
        /// </summary>
        /// <param name="itemGrossMargin">賣場的毛利率</param>
        /// <param name="maxBankGrossMargin">該分期最高的銀行分期利率</param>
        /// <returns>總毛利</returns>
        private decimal GetInstallmentRate(decimal itemGrossMargin, decimal maxBankRate)
        {
            return itemGrossMargin - maxBankRate;
        }

        /// <summary>
        /// 檢查時間區間是否重疊
        /// </summary>
        /// <param name="startDate1">時間區段 1 的開始時間</param>
        /// <param name="endDate1">時間區段 1 的結束時間</param>
        /// <param name="startDate2">時間區段 2 的開始時間</param>
        /// <param name="endDate2">時間區段 2 的結束時間</param>
        /// <returns>true:時間區間重疊; false:時間區間沒有重疊</returns>
        private bool CheckDateRange(DateTime startDate1, DateTime endDate1, DateTime startDate2, DateTime endDate2)
        {
            bool result = false;

            if (startDate1<=endDate2 && startDate2<=endDate1)
            {
                result = true;
            }
            return result;
        }

        /// <summary>
        /// 關閉賣場最高的分期設定
        /// </summary>
        /// <param name="IDList"></param>
        /// <returns></returns>
        public ResponseMessage<List<string>> CloseItemTopInstallments(List<int> IDList,string updateuser)
        {
            ResponseMessage<List<string>> result = new ResponseMessage<List<string>>();
            result.Data = new List<string>();
            List<TWNewEgg.Models.DBModels.TWSQLDB.ItemTopInstallment> itemTopInstallmentList = null;
            result.Data = this._iItemTopInstallmentRepoAdapter.Deactivate(IDList, updateuser);
            
            return result;
        }

        /// <summary>
        /// 更新賣場最高的分期設定
        /// </summary>
        /// <param name="IDList"></param>
        /// <returns></returns>
        public ResponseMessage<List<string>> UpdateItemTopInstallments(List<ItemTopInstallment> itemTopInstallmentDMList,string updateUser)
        {
            ResponseMessage<List<string>> result = new ResponseMessage<List<string>>();
            result.Data = new List<string>();
            //用來判斷是否可以儲存
            bool timecheck = true;

            // 暫存的 ItemID 
            List<int> itemIdCell = new List<int>();

            // 分期預設羅輯設定清單
            ResponseMessage<List<DefaultInstallment>> defaultRuleCell = null;

            // 賣場售價清單
            List<Models.DBModels.TWSQLDB.ItemDisplayPrice> itemDisplayPriceCell = null;

            // 錯誤訊息
            string errorMessage = null;

            //先檢查是否有同賣場且活動時間重複區間
            #region 時間區間檢查

            foreach (var groupCell in itemTopInstallmentDMList.GroupBy(x => x.ItemID))
            {
                #region 檢查傳入參數

                if (groupCell.Count() >= 2)
                {
                    bool isStop = false;

                    // 檢查要被更新的資料中，時間是否有重複
                    foreach (ItemTopInstallment updateData in groupCell)
                    {
                        foreach (ItemTopInstallment checkData in groupCell.Where(x => x.Edition != updateData.Edition))
                        {
                            if (this.CheckDateRange(checkData.StartDate, checkData.EndDate, updateData.StartDate, updateData.EndDate))
                            {
                                //表示有重複區間
                                timecheck = false;

                                if (!itemIdCell.Contains(updateData.ItemID))
                                {
                                    itemIdCell.Add(updateData.ItemID);
                                }

                                isStop = true;
                                break;
                            }
                        }

                        if (isStop)
                        {
                            break;
                        }
                    }
                }

                #endregion 檢查傳入參數

                #region 檢查 DB 的資料

                // 抓取 DB 中同賣場編號的資料
                List<TWNewEgg.Models.DBModels.TWSQLDB.ItemTopInstallment> dbDataCell = this._iItemTopInstallmentRepoAdapter.GetAll().Where(x => x.ItemID == groupCell.Key).ToList();

                // 移除 DB 資料中，和傳入參數一樣的 Edition
                foreach (ItemTopInstallment updateData in groupCell)
                {
                    if (dbDataCell.Any(x => x.Edition == updateData.Edition))
                    {
                        dbDataCell.Remove(dbDataCell.Where(x => x.Edition == updateData.Edition).First());
                    }
                }

                if (dbDataCell != null && dbDataCell.Count > 0)
                {
                    // 對 DB 資料做檢查
                    foreach (TWNewEgg.Models.DBModels.TWSQLDB.ItemTopInstallment dbData in dbDataCell)
                    {
                        foreach (ItemTopInstallment updateData in groupCell)
                        {
                            if (this.CheckDateRange(dbData.StartDate, dbData.EndDate, updateData.StartDate, updateData.EndDate))
                            {
                                //表示有重複區間
                                timecheck = false;

                                if (!itemIdCell.Contains(updateData.ItemID))
                                {
                                    itemIdCell.Add(updateData.ItemID);
                                }

                                break;
                            }
                        }
                    }
                }

                #endregion 檢查 DB 的資料
            }

            if (timecheck == false)
            {
                if (itemIdCell != null && itemIdCell.Count > 0)
                {
                    if (string.IsNullOrEmpty(errorMessage) == false)
                    {
                        errorMessage += ", ";
                    }

                    foreach (int itemId in itemIdCell)
                    {
                        errorMessage += itemId.ToString();
                    }

                    itemIdCell = null;

                    result.Data.Add(string.Format("賣場編號 : {0}，活動時間有重複區間。", errorMessage));

                    errorMessage = null;
                }

                return result;
            }

            #endregion 時間區間檢查

            #region 檢查開放的分期，是否大於預設分期

            #region 取得賣場售價

            itemIdCell = new List<int>();
            itemIdCell = itemTopInstallmentDMList.Select(x => x.ItemID).Distinct().ToList();

            itemDisplayPriceCell = this._iItemDisplayPriceRepoAdapter.GetAll().Where(x => itemIdCell.Contains(x.ItemID)).ToList();
            itemIdCell = null;

            foreach (ItemTopInstallment updateData in itemTopInstallmentDMList)
            { 
                if(itemDisplayPriceCell.Any(x => x.ItemID == updateData.ItemID))
                {
                    updateData.Price = itemDisplayPriceCell.Where(x => x.ItemID == updateData.ItemID).Select(x => x.DisplayPrice).First();
                }
            }

            itemDisplayPriceCell = null;

            #endregion 取得賣場售價

            #region 取得分期預設羅輯設定

            defaultRuleCell = this.GetDefaultRules();

            if (defaultRuleCell == null || defaultRuleCell.IsSuccess == false || defaultRuleCell.Data.Count == 0)
            {
                result.Data.Add("取得分期預設羅輯設定失敗。");
                return result;
            }

            defaultRuleCell.Data = defaultRuleCell.Data.OrderByDescending(x => x.Price).ToList();

            #endregion 取得分期預設羅輯設定

            itemIdCell = new List<int>();
            errorMessage = null;

            foreach (ItemTopInstallment updateData in itemTopInstallmentDMList)
            {
                foreach (DefaultInstallment defaultRule in defaultRuleCell.Data)
                {
                    if (updateData.Price >= defaultRule.Price && updateData.TopInstallment <= defaultRule.Installment)
                    {
                        itemIdCell.Add(updateData.ItemID);
                        break;
                    }
                }
            }

            defaultRuleCell = null;

            if (itemIdCell != null && itemIdCell.Count > 0)
            {
                if (string.IsNullOrEmpty(errorMessage) == false)
                {
                    errorMessage += ", ";
                }

                foreach (int itemId in itemIdCell)
                {
                    errorMessage += itemId.ToString();
                }

                itemIdCell = null;

                result.Data.Add(string.Format("賣場編號 : {0}，的開放期數，不可以小於等於預設的最高期數。", errorMessage));

                errorMessage = null;

                return result;
            }

            itemIdCell = null;

            #endregion 檢查開放的分期，是否大於預設分期

            foreach (ItemTopInstallment itemTopInstallment in itemTopInstallmentDMList)
            {
                //撈舊資料並把全部的SerialNumber+1並且更新
                bool addSerNum = this._iItemTopInstallmentRepoAdapter.AddSerialNumber(itemTopInstallment.Edition, updateUser);
                if (addSerNum == false)
                {
                    result.Data.Add(string.Format("賣場編號 : {0} ，更新失敗!", itemTopInstallment.ItemID));
                }
                else
                {
                    TWNewEgg.Models.DBModels.TWSQLDB.ItemTopInstallment newitemTopInstallment = new Models.DBModels.TWSQLDB.ItemTopInstallment();
                    newitemTopInstallment.Edition = itemTopInstallment.Edition;
                    newitemTopInstallment.Status = (int)TWNewEgg.Models.DBModels.TWSQLDB.ItemTopInstallmentStatus.Enable;
                    newitemTopInstallment.SerialNumber = 0;
                    newitemTopInstallment.ItemID = itemTopInstallment.ItemID;
                    newitemTopInstallment.TopInstallment = itemTopInstallment.TopInstallment;
                    newitemTopInstallment.StartDate = itemTopInstallment.StartDate;
                    newitemTopInstallment.EndDate = itemTopInstallment.EndDate;
                    newitemTopInstallment.CreateDate = DateTime.Now;
                    newitemTopInstallment.UpdateDate = DateTime.Now;
                    newitemTopInstallment.CreateUser = updateUser;
                    newitemTopInstallment.UpdateUser = updateUser;
                    string createInstallment = this._iItemTopInstallmentRepoAdapter.Update(new List<TWNewEgg.Models.DBModels.TWSQLDB.ItemTopInstallment> { newitemTopInstallment }, updateUser);
                    if (createInstallment != null)
                    {
                        result.Data.Add(string.Format("賣場編號 : {0} ，更新失敗! {1}", itemTopInstallment.ItemID, createInstallment));
                    }
                }
            }
            return result;
        }
        
        #endregion 分期設定

        #region 產生下拉式選單內容

        /// <summary>
        /// 取得所有分期期數
        /// </summary>
        /// <remarks>操作介面的下拉式選單內容</remarks>
        /// <returns>所有分期期數</returns>
        public List<SelectListItem> GetInstallmentSelection()
        {
            List<SelectListItem> result = new List<SelectListItem>();

            try
            {
                result = this._iBankInstallmentRepoAdapter.GetAvailableInstallments().ToList().OrderBy(x => x.Value).Select(x => new SelectListItem()
                {
                    Text = x.Value.ToString(),
                    Value = x.Value.ToString()
                }).ToList();
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("取得所有分期期數失敗(exception)。ErrorMessage = {0}。Exception = {1}。", GetExceptionMessage(ex), ex.ToString()));
            }

            return result;
        }

        #endregion 產生下拉式選單內容

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
