using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TWNewEgg.Models.DBModels.TWSQLDB;

namespace TWNewEgg.BankBonusRepoAdapters.Interface
{
    /// <summary>
    /// 信用卡紅利折抵
    /// </summary>
    public interface IBankBonusRepoAdapter
    {
        #region BankBonus

        /// <summary>
        /// 新增生效的信用卡紅利折抵項目
        /// </summary>
        /// <param name="bankBonus">要新增的信用卡紅利折抵內容</param>
        void CreateBankBonus(List<BankBonus> bankBonus);

        /// <summary>
        /// 取得生效的信用卡紅利折抵項目
        /// </summary>
        /// <remarks>只取 SerialNumber 為 0 的</remarks>
        /// <returns>IQueryable BankBonus</returns>
        IQueryable<BankBonus> GetAllBankBonus();

        /// <summary>
        /// 取得上架的信用卡紅利折抵項目
        /// </summary>
        /// <returns>IQueryable BankBonus</returns>
        IQueryable<BankBonus> GetAllEffectiveBankBonus();

        /// <summary>
        /// 取得前一次的變更記錄
        /// </summary>
        /// <remarks>只取 SerialNumber 為 1 的</remarks>
        /// <param name="idCell">生效區的項目編號清單</param>
        /// <returns>IQueryable BankBonus</returns>
        IQueryable<BankBonus> GetLastLog(List<int> idCell);

        /// <summary>
        /// 更新生效的信用卡紅利折抵項目(只更新 SerialNumber 欄位)
        /// </summary>
        /// <remarks></remarks>
        /// <param name="idCell">生效區的項目編號清單</param>
        /// <param name="updateUser">更新者</param>
        void UpdateSerialNumber(List<int> idCell, string updateUser);

        /// <summary>
        /// 更新排序
        /// </summary>
        /// <param name="updateData">要更新排序的生效信用卡紅利折抵項目內容</param>
        void UpdateOrder(List<BankBonus> updateData);

        #endregion BankBonus

        #region BankBonusTemp

        /// <summary>
        /// 新增待審的信用卡紅利折抵項目
        /// </summary>
        /// <param name="bankBonusTemp">要新增的信用卡紅利折抵項目內容</param>
        void CreateBankBonusTemp(BankBonusTemp bankBonusTemp);

        /// <summary>
        /// 刪除待審的信用卡紅利折抵項目
        /// </summary>
        /// <param name="idCell">要刪除的信用卡紅利折抵項目編號</param>
        void DeleteBankBonusTemp(List<int> idCell);

        /// <summary>
        /// 取得待審的信用卡紅利折抵項目
        /// </summary>
        /// <returns>IQueryable BankBonusTemp</returns>
        IQueryable<BankBonusTemp> GetAllBankBonusTemp();

        /// <summary>
        /// 更新待審的信用卡紅利折抵項目
        /// </summary>
        /// <param name="bankBonusTemp">要更新的信用卡紅利折抵項目內容</param>
        void UpdateBankBonusTemp(BankBonusTemp bankBonusTemp);

        #endregion BankBonnsTemp
    }
}
