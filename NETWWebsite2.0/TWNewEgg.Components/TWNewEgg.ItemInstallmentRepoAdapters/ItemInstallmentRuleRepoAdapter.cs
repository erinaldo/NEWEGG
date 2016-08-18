using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TWNewEgg.ItemInstallmentRepoAdapters.Interface;
using TWNewEgg.DAL.Interface;
using TWNewEgg.Models.DBModels.TWSQLDB;

namespace TWNewEgg.ItemInstallmentRepoAdapters
{
    /// <summary>
    /// 分期預設
    /// </summary>
    public class ItemInstallmentRuleRepoAdapter : IItemInstallmentRuleRepoAdapter
    {
        private IRepository<ItemInstallmentRule> _itemInstallmentRule;
        private IRepository<Installment> _installment;

        public ItemInstallmentRuleRepoAdapter(IRepository<ItemInstallmentRule> itemInstallmentRule, IRepository<Installment> installment)
        {
            this._itemInstallmentRule = itemInstallmentRule;
            this._installment = installment;
        }

        /// <summary>
        /// 取得所有分期預設
        /// </summary>
        /// <remarks>只取得 SerialNumber = 0 的部份</remarks>
        /// <returns>IQueryable ItemInstallmentRule</returns>
        public IQueryable<ItemInstallmentRule> GetAll()
        {
            return this._itemInstallmentRule.GetAll().Where(x => x.SerialNumber == 0 && x.Status == (int)ItemInstallmentRuleStatus.Enable).AsQueryable();
        }

        /// <summary>
        /// 取得賣場售價的可用分期
        /// </summary>
        /// <param name="price">賣場售價</param>
        /// <returns>可用分期</returns>
        public List<int> GetAvailableInstallments(int price)
        {
            List<int> result = new List<int>();

            // 賣場售價可以用的分期期數 ID (不包含全商品開放分期)
            List<int> installmentIdCell = null;

            try
            {
                // 取得賣場售價可以用的分期期數 ID (不包含全商品開放分期)
                installmentIdCell = this.GetAll().Where(x => x.Price <= price && x.Price != -1).Select(x => x.InstallmentID).ToList();

                if (installmentIdCell == null || installmentIdCell.Count == 0)
                {
                    throw new Exception();
                }

                // 依分期期數 ID，取得分期期數
                result = this._installment.GetAll().Where(x => installmentIdCell.Contains(x.ID) && x.Status == (int)InstallmentStatus.Enable).Select(x => x.Value).ToList();

                installmentIdCell = null;

                if (result == null)
                {
                    throw new Exception();
                }
            }
            catch
            {
                result = new List<int>();
            }
            finally
            {
                // 取得全部商品開放分期期數
                result.AddRange(this.GetAllItemInstallmentValueCell());
            }

            return result;
        }

        /// <summary>
        /// 取得全部商品開放分期期數清單
        /// </summary>
        /// <returns>全部商品開放分期期數清單</returns>
        private List<int> GetAllItemInstallmentValueCell()
        { 
            List<int> result = new List<int>();

            // 全部商品開放最高分期期數 ID
            int allItemTopInstallmentId = -1;

            // 全部商品開放最高分期期數
            int allItemTopInstallmentValue = -1;

            try
            {
                // 取得全部商品開放最高分期期數 ID
                allItemTopInstallmentId = this.GetAll().Where(x => x.Price == -1).Select(x => x.InstallmentID).First();

                // 取得全部商品開放最高分期期數
                allItemTopInstallmentValue = this._installment.GetAll().Where(x => x.ID == allItemTopInstallmentId).Select(x => x.Value).First();

                result = this._installment.GetAll().Where(x => x.Status == (int)InstallmentStatus.Enable && x.Value <= allItemTopInstallmentValue).Select(x => x.Value).ToList();
            }
            catch
            {
                result = new List<int>();
            }

            return result;
        }

        /// <summary>
        /// 新增分期預設
        /// </summary>
        /// <param name="createData">新增項目</param>
        /// <param name="createUser">建立人</param>
        /// <returns>新增項目的 ID</returns>
        public int Create(ItemInstallmentRule createData, string createUser)
        {
            int result = -1;

            // 建立日期
            DateTime nowDateTime = DateTime.Now;

            try
            {
                if (createData == null)
                {
                    throw new Exception();
                }

                if (this._itemInstallmentRule.GetAll().Where(x => x.InstallmentID == createData.InstallmentID).Any(x => x.SerialNumber == 0))
                {
                    this.AddSerialNumber(this._itemInstallmentRule.GetAll().Where(x => x.InstallmentID == createData.InstallmentID).Select(x => x.ID).First(), createUser);
                }

                createData.ID = 0;
                createData.Status = (int)ItemInstallmentRuleStatus.Enable;
                createData.SerialNumber = 0;
                createData.CreateDate = nowDateTime;
                createData.UpdateDate = nowDateTime;
                createData.CreateUser = createUser;
                createData.UpdateUser = createUser;

                this._itemInstallmentRule.Create(createData);

                result = GetMaxId();
            }
            catch
            {
                result = -1;
            }

            return result;
        }

        /// <summary>
        /// 取得分期預設表中目前最大的 ID 值
        /// </summary>
        /// <returns>分期預設表中目前最大的 ID 值</returns>
        private int GetMaxId()
        {
            int result = -1;

            try
            {
                result = this._itemInstallmentRule.GetAll().Max(x => x.ID);
            }
            catch
            {
                result = -1;
            }

            return result;
        }

        /// <summary>
        /// 刪除分期預設
        /// </summary>
        /// <param name="id">刪除項目 ID</param>
        /// <param name="updateUser">更新人</param>
        /// <returns>tue:刪除成功, false:刪除失敗</returns>
        public bool Delete(int id, string updateUser)
        {
            bool result = false;

            // 要刪除的資料
            ItemInstallmentRule itemInstallmentRule = null;

            // 更新 SerialNumber 的結果
            bool addSerialNumberResult = false;

            // 更新日期
            DateTime nowDatatime = DateTime.Now;

            try
            {
                #region 輸入參數檢查

                if (string.IsNullOrEmpty(updateUser) == false)
                {
                    updateUser = updateUser.Trim();
                }

                if (id <= 0 || string.IsNullOrEmpty(updateUser))
                {
                    throw new Exception();
                }

                #endregion 輸入參數檢查

                // 取得要刪除的資料
                itemInstallmentRule = this.GetAll().Where(x => x.ID == id).First();

                if (itemInstallmentRule == null)
                {
                    throw new Exception();
                }

                // 將同一個分期數 ID 的 SerialNumber + 1
                addSerialNumberResult = this.AddSerialNumber(id, updateUser);

                if (addSerialNumberResult == false)
                {
                    throw new Exception();
                }

                itemInstallmentRule.Status = (int)ItemInstallmentRuleStatus.Disable;
                itemInstallmentRule.SerialNumber = 0;
                itemInstallmentRule.CreateDate = nowDatatime;
                itemInstallmentRule.UpdateDate = nowDatatime;
                itemInstallmentRule.CreateUser = updateUser;
                itemInstallmentRule.UpdateUser = updateUser;

                this._itemInstallmentRule.Create(itemInstallmentRule);

                itemInstallmentRule = null;

                result = true;
            }
            catch
            {
                result = false;
            }
            finally
            {
                if (itemInstallmentRule != null)
                {
                    itemInstallmentRule = null;
                }
            }

            return result;
        }

        /// <summary>
        /// 將該筆 ID 相關的 SerialNumber + 1
        /// </summary>
        /// <param name="installmentID">分期數 ID</param>
        /// <param name="updateUser">更新人</param>
        /// <returns>tue:更新成功, false:更新失敗</returns>
        public bool AddSerialNumber(int id, string updateUser)
        {
            bool result = false;

            // 該筆 ID 的資料
            ItemInstallmentRule updateData = null;

            // 該筆 ID 相關的分期項目
            List<ItemInstallmentRule> updateDataCell = null;

            // 更新日期
            DateTime nowDatatime = DateTime.Now;

            try
            {
                // 取得該筆 ID 的資料
                updateData = this._itemInstallmentRule.GetAll().Where(x => x.ID == id).First();

                if (updateData == null)
                {
                    throw new Exception();
                }

                // 取得該筆 ID 相關的資料
                if (updateData.Price == -1)
                {
                    updateDataCell = this._itemInstallmentRule.GetAll().Where(x => x.Price == -1).ToList();
                }
                else
                {
                    updateDataCell = this._itemInstallmentRule.GetAll().Where(x => x.InstallmentID == updateData.InstallmentID && x.Price != -1).ToList();
                }

                updateData = null;

                if (updateDataCell == null || updateDataCell.Count == 0)
                {
                    throw new Exception();
                }

                // 將 SerialNumber + 1
                foreach (ItemInstallmentRule itemInstallmentRule in updateDataCell)
                {
                    itemInstallmentRule.SerialNumber++;
                    itemInstallmentRule.UpdateDate = nowDatatime;
                    itemInstallmentRule.UpdateUser = updateUser;
                }

                this._itemInstallmentRule.UpdateRange(updateDataCell);

                result = true;
            }
            catch
            {
                result = false;
            }

            return result;
        }
    }
}
