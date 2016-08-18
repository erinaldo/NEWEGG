using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TWNewEgg.DAL.Interface;
using TWNewEgg.BankBonusRepoAdapters.Interface;
using TWNewEgg.Models.DBModels.TWSQLDB;

namespace TWNewEgg.BankBonusRepoAdapters
{
    /// <summary>
    /// 信用卡紅利折抵
    /// </summary>
    public class BankBonusRepoAdapter : IBankBonusRepoAdapter
    {
        private IRepository<BankBonus> _bankBonus;
        private IRepository<BankBonusTemp> _bankBonusTemp;

        public BankBonusRepoAdapter(IRepository<BankBonus> bankBonus, IRepository<BankBonusTemp> bankBonusTemp)
        {
            this._bankBonus = bankBonus;
            this._bankBonusTemp = bankBonusTemp;
        }

        #region BankBonus

        /// <summary>
        /// 新增生效的信用卡紅利折抵項目
        /// </summary>
        /// <param name="bankBonus">要新增的信用卡紅利折抵內容</param>
        public void CreateBankBonus(List<BankBonus> bankBonus)
        {
            DateTime now = DateTime.Now;

            bankBonus.ForEach(x =>
            {
                x.CreateDate = now;
                x.UpdateDate = now;
                x.AuditDate = now;
                this._bankBonus.Create(x);
            });
        }

        /// <summary>
        /// 取得生效的信用卡紅利折抵項目
        /// </summary>
        /// <remarks>只取 SerialNumber 為 0 的</remarks>
        /// <returns>IQueryable BankBonus</returns>
        public IQueryable<BankBonus> GetAllBankBonus()
        {
            return this._bankBonus.GetAll().Where(x => x.SerialNumber == 0);
        }

        /// <summary>
        /// 取得上架的信用卡紅利折抵項目
        /// </summary>
        /// <returns>IQueryable BankBonus</returns>
        public IQueryable<BankBonus> GetAllEffectiveBankBonus()
        {
            return this.GetAllBankBonus().Where(x => x.Status == 0);
        }

        /// <summary>
        /// 取得前一次的變更記錄
        /// </summary>
        /// <remarks>只取 SerialNumber 為 1 的</remarks>
        /// <param name="idCell">生效區的項目編號清單</param>
        /// <returns>IQueryable BankBonus</returns>
        public IQueryable<BankBonus> GetLastLog(List<int> idCell)
        {
            return this._bankBonus.GetAll().Where(x => x.SerialNumber == 1 && idCell.Contains(x.ID));
        }

        /// <summary>
        /// 更新 SerialNumber
        /// </summary>
        /// <remarks>審核通過時，不覆蓋原項目，而將原項目，及原項目相同銀行代碼的所有項目，SerialNumber 欄位 + 1</remarks>
        /// <param name="idCell">生效區的項目編號清單</param>
        /// <param name="updateUser">更新者</param>
        public void UpdateSerialNumber(List<int> idCell, string updateUser)
        {
            List<BankBonus> bankBonusCell = null;
            string bankCode = null;
            DateTime now = DateTime.Now;

            foreach (int id in idCell)
            {
                // 取得原項目的 BankCode
                bankCode = null;
                bankCode = this._bankBonus.GetAll().Where(x => x.ID == id).Select(x => x.BankCode).First();

                // 找出原項目相同銀行代碼的所有項目
                bankBonusCell = new List<BankBonus>();
                bankBonusCell = this._bankBonus.GetAll().Where(x => x.BankCode == bankCode).ToList();

                if (bankBonusCell.Count > 0)
                {
                    // 將 SerialNumber 欄位 + 1
                    bankBonusCell.ForEach(x =>
                    {
                        x.UpdateDate = now;
                        x.UpdateUser = updateUser;
                        x.SerialNumber++;
                        this._bankBonus.Update(x);
                    });
                }
            }
        }

        /// <summary>
        /// 更新排序
        /// </summary>
        /// <param name="updateData">要更新排序的生效信用卡紅利折抵項目內容</param>
        public void UpdateOrder(List<BankBonus> updateData)
        {
            List<BankBonus> dbBankBonusCell = null;
            List<int> updateIdCell = null;
            DateTime now = DateTime.Now;

            // 讀取要更新的編號
            updateIdCell = updateData.Select(x => x.ID).ToList();

            // 從要更新的編號，讀取資料庫中要被更新的項目
            dbBankBonusCell = this.GetAllBankBonus().Where(x => updateIdCell.Contains(x.ID)).ToList();

            foreach(BankBonus bankbonus in dbBankBonusCell)
            {
                bankbonus.Order = updateData.Where(x => x.ID == bankbonus.ID).Select(x => x.Order).First();
                bankbonus.UpdateUser = updateData.Select(x => x.UpdateUser).First();
                bankbonus.UpdateDate = now;
                this._bankBonus.Update(bankbonus);
            }
        }

        #endregion BankBonus

        #region BankBonusTemp

        /// <summary>
        /// 新增待審的信用卡紅利折抵項目
        /// </summary>
        /// <param name="bankBonusTemp">要新增的信用卡紅利折抵項目內容</param>
        public void CreateBankBonusTemp(BankBonusTemp bankBonusTemp)
        {
            DateTime now = DateTime.Now;

            bankBonusTemp.CreateDate = now;
            bankBonusTemp.UpdateDate = now;
            this._bankBonusTemp.Create(bankBonusTemp);
        }

        /// <summary>
        /// 刪除待審的信用卡紅利折抵項目
        /// </summary>
        /// <param name="idCell">要刪除的信用卡紅利折抵項目編號</param>
        public void DeleteBankBonusTemp(List<int> idCell)
        {
            BankBonusTemp bankBonusTemp = null;
            
            foreach(int id in idCell)
            {
                bankBonusTemp = new BankBonusTemp();
                bankBonusTemp = this._bankBonusTemp.GetAll().Where(x => x.ID == id).First();
                this._bankBonusTemp.Delete(bankBonusTemp);
            }
        }

        /// <summary>
        /// 取得待審的信用卡紅利折抵項目
        /// </summary>
        /// <returns>IQueryable BankBonusTemp</returns>
        public IQueryable<BankBonusTemp> GetAllBankBonusTemp()
        {
            return this._bankBonusTemp.GetAll();
        }

        /// <summary>
        /// 更新待審的信用卡紅利折抵項目
        /// </summary>
        /// <param name="updateData">要更新的待審信用卡紅利折抵項目內容</param>
        public void UpdateBankBonusTemp(BankBonusTemp updateData)
        {
            BankBonusTemp dbData = null;
            DateTime now = DateTime.Now;

            dbData = this.GetAllBankBonusTemp().Where(x => x.ID == updateData.ID).First();

            dbData.BankCode = updateData.BankCode;
            dbData.ConsumeLimit = updateData.ConsumeLimit;
            dbData.DescriptionFormat = updateData.DescriptionFormat;
            dbData.OffsetMax = updateData.OffsetMax;
            dbData.PhotoName = updateData.PhotoName;
            dbData.PointLimit = updateData.PointLimit;
            dbData.ProportionMoney = updateData.ProportionMoney;
            dbData.ProportionPoint = updateData.ProportionPoint;
            dbData.PublishBank = updateData.PublishBank;
            dbData.PublishBankPhone = updateData.PublishBankPhone;
            dbData.Status = updateData.Status;
            dbData.UpdateDate = now;
            dbData.UpdateUser = updateData.UpdateUser;

            this._bankBonusTemp.Update(dbData);
        }

        #endregion BankBonusTemp
    }
}
