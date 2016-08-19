using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TWNewEgg.Models.DomainModels.Message;
using TWNewEgg.Models.DomainModels.ItemInstallment;

namespace TWNewEgg.CartServices.Interface
{
    public interface IItemInstallmentService
    {
        #region 分期預設邏輯設定

        /// <summary>
        /// 取得分期預設邏輯設定
        /// </summary>
        /// <return>分期預設邏輯設定清單</return>
        ResponseMessage<List<DefaultInstallment>> GetDefaultRules();

        /// <summary>
        /// 新增分期預設邏輯設定
        /// </summary>
        /// <param name="createData">分期預設邏輯設定</param>
        /// <param name="createUser">建立人</param>
        /// <returns>錯誤訊息</returns>
        ResponseMessage<string> CreateDefaultRule(DefaultInstallment createData, string createUser);

        /// <summary>
        /// 更新分期預設邏輯設定
        /// </summary>
        /// <param name="updateDataCell">要更新的分期預設邏輯設定</param>
        /// <param name="updateUser">更新人</param>
        /// <returns>錯誤訊息</returns>
        ResponseMessage<List<string>> UpdateDefaultRule(List<DefaultInstallment> updateDataCell, string updateUser);

        /// <summary>
        /// 刪除分期預設邏輯設定
        /// </summary>
        /// <param name="id">分期預設項目 ID</param>
        /// <param name="deleteUser">刪除人</param>
        /// <returns>錯誤訊息</returns>
        ResponseMessage<string> DeleteDefaultRule(int id, string deleteUser);

        #endregion 分期預設邏輯設定

        #region 分期設定查詢

        /// <summary>
        /// 取得賣場資訊
        /// </summary>
        /// <param name="searchCondition">搜尋條件</param>
        /// <returns>賣場資訊</returns>
        ResponseMessage<ItemForAddTopRuleReturn> GetItemCell(ItemForAddTopRuleSearchCondition searchCondition);

        /// <summary>
        /// 取得賣場最高的分期設定
        /// </summary>
        /// <param name="searchCondition">搜尋條件</param>
        /// <returns>分期設定</returns>
        ResponseMessage<ItemTopInstallmentReturn> GetItemTopInstallments(ItemTopInstallmentSearchCondition searchCondition);

        /// <summary>
        /// 新增賣場最高的分期設定
        /// </summary>
        /// <param name="createDataCell">賣場最高的分期設定</param>
        /// <param name="createUser">建立人</param>
        /// <returns>錯誤訊息</returns>
        ResponseMessage<string> CreateItemTopInstallments(List<ItemTopInstallment> createDataCell, string createUser);

        /// <summary>
        /// 關閉分期設定查詢
        /// </summary>
        /// <param name="IDList"></param>
        /// <returns></returns>
        ResponseMessage<List<string>> CloseItemTopInstallments(List<int> IDList, string updateuser);

        /// <summary>
        /// 更新分期設定查詢
        /// </summary>
        /// <param name="itemTopInstallmentDMList"></param>
        /// <param name="updateUser"></param>
        /// <returns></returns>
        ResponseMessage<List<string>> UpdateItemTopInstallments(List<ItemTopInstallment> itemTopInstallmentDMList, string updateUser);

        /// <summary>
        /// 檢查分期總毛利
        /// </summary>
        /// <param name="chekcDataCell">要儲存的賣場最高分期期數清單</param>
        /// <returns>錯誤訊息</returns>
        ResponseMessage<string> CheckRate(List<ItemTopInstallment> chekcDataCell);

        #endregion 分期設定查詢

        #region 產生下拉式選單內容

        /// <summary>
        /// 取得所有分期期數
        /// </summary>
        /// <remarks>操作介面的下拉式選單內容</remarks>
        /// <returns>所有分期期數</returns>
        List<SelectListItem> GetInstallmentSelection();
        
        #endregion 產生下拉式選單內容
    }
}
