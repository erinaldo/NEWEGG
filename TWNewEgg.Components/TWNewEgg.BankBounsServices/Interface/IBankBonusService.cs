using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TWNewEgg.Models.DomainModels.Message;
using TWNewEgg.Models.DomainModels.BankBonus;

namespace TWNewEgg.BankBonusServices.Interface
{
    /// <summary>
    /// 信用卡紅利折抵
    /// </summary>
    public interface IBankBonusService
    {
        #region BankBonus

        /// <summary>
        /// 取得生效的信用卡紅利折抵項目
        /// </summary>
        /// <remarks>只取 SerialNumber 為 0 的</remarks>
        /// <returns>生效的信用卡紅利折抵項目清單</returns>
        ResponseMessage<List<BankBonus_DM>> GetAllBankBonus();

        /// <summary>
        /// 取得單筆生效的信用卡紅利折抵項目
        /// </summary>
        /// <param name="id">信用卡紅利折抵項目編號</param>
        /// <returns>單筆生效的信用卡紅利折抵項目</returns>
        ResponseMessage<BankBonus_DM> GetBankBonusById(int id);

        /// <summary>
        /// 取得上架的信用卡紅利折抵項目
        /// </summary>
        /// <returns>上架的信用卡紅利折抵項目清單</returns>
        ResponseMessage<List<BankBonus_DM>> GetAllEffectiveBankBonus();

        /// <summary>
        /// 儲存排序
        /// </summary>
        /// <param name="bankBonusCell_DM">要更改排序的信用卡紅利折抵項目內容</param>
        ResponseMessage<bool> UpdateOrder(List<BankBonus_DM> bankBonusCell_DM);

        #endregion BankBonus

        #region BankBonusTemp

        /// <summary>
        /// 新增待審的信用卡紅利折抵項目
        /// </summary>
        /// <param name="bankBonusTemp_DM">要新增的信用卡紅利折抵項目內容</param>
        ResponseMessage<bool> CreateBankBonusTemp(BankBonusTemp_DM bankBonusTemp_DM);

        /// <summary>
        /// 刪除待審的信用卡紅利折抵項目
        /// </summary>
        /// <param name="idCell">要刪除的信用卡紅利折抵項目編號</param>
        ResponseMessage<bool> DeleteBankBonusTemp(List<int> idCell);

        /// <summary>
        /// 取得待審的信用卡紅利折抵項目
        /// </summary>
        /// <returns>待審的信用卡紅利折抵項目清單</returns>
        ResponseMessage<List<BankBonusTemp_DM>> GetAllBankBonusTemp();

        /// <summary>
        /// 取得單筆待審的信用卡紅利折抵項目
        /// </summary>
        /// <param name="id">待審的信用卡紅利折抵項目編號</param>
        /// <returns>單筆待審的信用卡紅利折抵項目清單</returns>
        ResponseMessage<BankBonusTemp_DM> GetBankBonusTempById(int id);

        /// <summary>
        /// 更新待審的信用卡紅利折抵項目
        /// </summary>
        /// <param name="bankBonusTemp_DM">要更新的信用卡紅利折抵項目內容</param>
        ResponseMessage<bool> UpdateBankBonusTemp(BankBonusTemp_DM bankBonusTemp_DM);

        /// <summary>
        /// 審核通過
        /// </summary>
        /// <param name="idCell">要審核通過的信用卡紅利折抵項目編號</param>
        /// <param name="updateUser">更新者</param>
        /// <returns>審核通過的信用卡紅利折抵項目清單</returns>
        ResponseMessage<List<BankBonus_DM>> ApproveBankBonus(List<int> idCell, string updateUser);

        #endregion BankBonusTemp
    }
}
