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
    /// 賣場最高分期期數
    /// </summary>
    public class ItemTopInstallmentRepoAdapter : IItemTopInstallmentRepoAdapter
    {
        private IRepository<ItemTopInstallment> _itemTopInstallment;
        private IRepository<Installment> _installment;

        public ItemTopInstallmentRepoAdapter(IRepository<ItemTopInstallment> itemTopInstallment, IRepository<Installment> installment)
        {
            this._itemTopInstallment = itemTopInstallment;
            this._installment = installment;
        }

        /// <summary>
        /// 取得所有賣場最高分期期數
        /// </summary>
        /// <remarks>只取得 SerialNumber = 0 的部份</remarks>
        /// <returns></returns>
        public IQueryable<ItemTopInstallment> GetAll()
        {
            return this._itemTopInstallment.GetAll().Where(x => x.SerialNumber == 0).AsQueryable();
        }

        /// <summary>
        /// 取得賣場的可用分期
        /// </summary>
        /// <param name="itemIdCell">賣場編號清單</param>
        /// <returns>可用分期</returns>
        public List<int> GetAvailableInstallments(IEnumerable<int> itemIdCell)
        {
            List<int> result = new List<int>();
            DateTime nowDataTime = DateTime.Now;

            try
            {
                result.Add(this.GetAll()
                    .Where(x => itemIdCell.Contains(x.ItemID))
                    .Where(x => x.Status == (int)ItemTopInstallmentStatus.Enable)
                    .Where(x => x.StartDate <= nowDataTime && x.EndDate >= nowDataTime)
                    .Min(x => x.TopInstallment));

                if (result.Count > 0)
                {
                    result.AddRange(this._installment.GetAll()
                        .Where(x => x.Status == (int)InstallmentStatus.Enable)
                        .Where(x => x.Value < result[0])
                        .Select(x => x.Value)
                       .ToList());

                    result = result.OrderByDescending(x => x).ToList();
                }
            }
            catch
            {
                result = new List<int>();
            }

            return result;
        }

        /// <summary>
        /// 新增賣場最高分期期數
        /// </summary>
        /// <param name="createDataCell">新增項目清單</param>
        /// <param name="createUser">建立人</param>
        /// <returns>true:新增成功; false:新增失敗</returns>
        public bool Create(List<ItemTopInstallment> createDataCell, string createUser)
        {
            bool result = false;

            // 分期預設表中目前最大的 Edition 值
            int maxEdition = -1;

            // 建立日期
            DateTime nowDateTime = DateTime.Now;

            try
            {
                if (createDataCell == null || createDataCell.Count == 0)
                {
                    throw new Exception();
                }

                // 取得分期預設表中目前最大的 Edition 值
                maxEdition = this.GetMaxEdition();

                if (maxEdition == -1)
                {
                    throw new Exception();
                }

                foreach (ItemTopInstallment createData in createDataCell)
                {
                    createData.ID = 0;
                    createData.Edition = ++maxEdition;
                    createData.Status = (int)ItemTopInstallmentStatus.Enable;
                    createData.SerialNumber = 0;
                    createData.CreateDate = nowDateTime;
                    createData.UpdateDate = nowDateTime;
                    createData.CreateUser = createUser;
                    createData.UpdateUser = createUser;
                }

                this._itemTopInstallment.CreateRange(createDataCell);

                result = true;
            }
            catch
            {
                result = false;
            }

            return result;
        }

        /// <summary>
        /// 更新賣場最高分期期數
        /// </summary>
        /// <param name="updateDataCell">更新項目清單</param>
        /// <param name="updateUser">更新人</param>
        /// <returns>錯誤訊息</returns>
        public string Update(List<ItemTopInstallment> updateDataCell, string updateUser)
        {
            string result = null;

            List<ItemTopInstallment> createDataCell = null;

            // 更新日期
            DateTime nowDateTime = DateTime.Now;

            try
            {
                #region 輸入檢查

                if (updateDataCell == null || updateDataCell.Count == 0)
                {
                    result += "沒有更新項目。";
                    throw new Exception();
                }

                if (string.IsNullOrEmpty(updateUser) == false)
                {
                    updateUser = updateUser.Trim();
                }

                if (string.IsNullOrEmpty(updateUser))
                {
                    result += "未填寫更新人資訊。";
                    throw new Exception();
                }

                #endregion 輸入檢查

                createDataCell = this.MakeNewItemTopInstallmentCell(updateDataCell, updateUser);

                if (createDataCell == null || createDataCell.Count == 0)
                {
                    throw new Exception();
                }

                this._itemTopInstallment.CreateRange(createDataCell);

                createDataCell = null;
            }
            catch
            {
                result = "更新失敗。";
            }
            finally
            {
                if (createDataCell != null)
                {
                    createDataCell = null;
                }
            }

            return result;
        }

        /// <summary>
        /// 組合新的賣場最高分期資料
        /// </summary>
        /// <param name="createDataCell">要新增的資料</param>
        /// <param name="createUser">建立人</param>
        /// <returns>新的賣場最高分期資料</returns>
        private List<ItemTopInstallment> MakeNewItemTopInstallmentCell(List<ItemTopInstallment> createDataCell, string createUser)
        {
            List<ItemTopInstallment> result = new List<ItemTopInstallment>();
            ItemTopInstallment newData = null;
            DateTime now = DateTime.Now;

            if (createDataCell != null && createDataCell.Count > 0)
            {
                foreach (ItemTopInstallment createData in createDataCell)
                {
                    try
                    {
                        newData = new ItemTopInstallment();

                        newData.ID = 0;
                        newData.SerialNumber = 0;
                        newData.Edition = createData.Edition;
                        newData.ItemID = createData.ItemID;
                        newData.TopInstallment = createData.TopInstallment;
                        newData.StartDate = createData.StartDate;
                        newData.EndDate = createData.EndDate;
                        newData.Status = createData.Status;
                        newData.CreateDate = now;
                        newData.UpdateDate = now;
                        newData.CreateUser = createUser;
                        newData.UpdateUser = createUser;

                        result.Add(newData);

                        newData = null;
                    }
                    catch
                    {
                        continue;
                    }
                    finally
                    {
                        if (newData != null)
                        {
                            newData = null;
                        }
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// 取得分期預設表中目前最大的 Edition 值
        /// </summary>
        /// <returns>分期預設表中目前最大的 Edition 值</returns>
        private int GetMaxEdition()
        {
            int result = -1;

            try
            {
                result = this._itemTopInstallment.GetAll().Max(x => x.Edition);
            }
            catch
            {
                result = -1;
            }

            return result;
        }

        /// <summary>
        /// 啟用賣場最高分期期數
        /// </summary>
        /// <param name="idCell">賣場最高分期期數編號清單</param>
        /// <param name="updateUser">更新人</param>
        /// <returns>錯誤訊息</returns>
        public List<string> Activate(List<int> idCell, string updateUser)
        {
            // 錯誤訊息
            List<string> result = new List<string>();

            // 要更新的資料
            List<ItemTopInstallment> updateDataCell = null;

            // 更新 SerialNumber 結果
            bool addSerialNumber = false;

            // 更新失敗的 ID 清單
            List<int> errorIdCell = null;

            // 更新日期
            DateTime nowDateTime = DateTime.Now;

            try
            {
                #region 輸入檢查

                if (string.IsNullOrEmpty(updateUser) == false)
                {
                    updateUser = updateUser.Trim();
                }

                if (idCell == null || idCell.Count == 0)
                {
                    result.Add("未傳入更新資料");
                    throw new Exception();
                }

                if (string.IsNullOrEmpty(updateUser))
                {
                    result.Add("未填寫更新人資訊");
                    throw new Exception();
                }

                #endregion 輸入檢查

                // 取得要更新的資料
                updateDataCell = this.GetAll().Where(x => idCell.Contains(x.ID)).ToList();

                if (updateDataCell == null || updateDataCell.Count == 0)
                {
                    result.Add("查無資料");
                    throw new Exception();
                }

                #region 將 SerialNumber +1

                foreach (ItemTopInstallment updateData in updateDataCell)
                {
                    addSerialNumber = false;
                    addSerialNumber = this.AddSerialNumber(updateData.Edition, updateUser);

                    if (addSerialNumber == false)
                    {
                        if (errorIdCell == null)
                        {
                            errorIdCell = new List<int>();
                        }

                        errorIdCell.Add(updateData.ID);
                    }
                }

                #endregion 將 SerialNumber +1

                #region 將更新 SerialNumber 失敗的資料，從更新清單中移除

                updateDataCell = updateDataCell.Where(x => !errorIdCell.Contains(x.ID)).ToList();

                if (updateDataCell.Count == 0)
                {
                    throw new Exception();
                }

                #endregion 將更新 SerialNumber 失敗的資料，從更新清單中移除

                foreach (ItemTopInstallment updateData in updateDataCell)
                {
                    updateData.ID = 0;
                    updateData.SerialNumber = 0;
                    updateData.Status = (int)ItemInstallmentRuleStatus.Enable;
                    updateData.CreateDate = nowDateTime;
                    updateData.CreateUser = updateUser;
                    updateData.UpdateDate = nowDateTime;
                    updateData.UpdateUser = updateUser;
                }

                this._itemTopInstallment.CreateRange(updateDataCell);

                updateDataCell = null;
            }
            catch
            {
                // 若沒有錯誤訊息，才填寫更新失敗訊息
                if (result.Count == 0)
                {
                    if (errorIdCell != null && errorIdCell.Count > 0)
                    {
                        foreach (int id in errorIdCell)
                        {
                            result.Add(string.Format("ID = {0} 更新失敗。"));
                        }
                    }
                    else
                    {
                        result.Add("更新失敗。");
                    }
                }

                errorIdCell = null;
            }
            finally
            {
                if (updateDataCell != null)
                {
                    updateDataCell = null;
                }

                if (errorIdCell != null)
                {
                    errorIdCell = null;
                }
            }

            return result;
        }

        /// <summary>
        /// 關閉賣場最高分期期數
        /// </summary>
        /// <param name="idCell">賣場最高分期期數編號清單</param>
        /// <param name="updateUser">更新人</param>
        /// <returns>錯誤訊息</returns>
        public List<string> Deactivate(List<int> idCell, string updateUser)
        {
            // 錯誤訊息
            List<string> result = new List<string>();

            // 要更新的資料
            List<ItemTopInstallment> updateDataCell = null;

            // 更新 SerialNumber 結果
            bool addSerialNumber = false;

            // 更新失敗的 ID 清單
            List<int> errorIdCell = new List<int>();

            // 更新日期
            DateTime nowDateTime = DateTime.Now;

            try
            {
                #region 輸入檢查

                if (string.IsNullOrEmpty(updateUser) == false)
                {
                    updateUser = updateUser.Trim();
                }

                if (idCell == null || idCell.Count == 0)
                {
                    result.Add("未傳入更新資料");
                    throw new Exception();
                }

                if (string.IsNullOrEmpty(updateUser))
                {
                    result.Add("未填寫更新人資訊");
                    throw new Exception();
                }

                #endregion 輸入檢查

                // 取得要更新的資料
                updateDataCell = this.GetAll().Where(x => idCell.Contains(x.ID)).ToList();

                if (updateDataCell == null || updateDataCell.Count == 0)
                {
                    result.Add("查無資料");
                    throw new Exception();
                }

                #region 將 SerialNumber +1

                foreach (ItemTopInstallment updateData in updateDataCell)
                {
                    addSerialNumber = false;
                    addSerialNumber = this.AddSerialNumber(updateData.Edition, updateUser);

                    if (addSerialNumber == false)
                    {


                        

                        errorIdCell.Add(updateData.ID);
                    }
                }

                #endregion 將 SerialNumber +1

                #region 將更新 SerialNumber 失敗的資料，從更新清單中移除

                updateDataCell = updateDataCell.Where(x => !errorIdCell.Contains(x.ID)).ToList();

                if (updateDataCell.Count == 0)
                {
                    throw new Exception();
                }

                #endregion 將更新 SerialNumber 失敗的資料，從更新清單中移除
                List<ItemTopInstallment> newitemTopInstallmentList = new List<ItemTopInstallment>();
                foreach (ItemTopInstallment updateData in updateDataCell)
                {
                    ItemTopInstallment newitemTopInstallment = new ItemTopInstallment();
                    newitemTopInstallment.ID = 0;
                    newitemTopInstallment.SerialNumber = 0;
                    newitemTopInstallment.Status = (int)ItemInstallmentRuleStatus.Disable;
                    newitemTopInstallment.StartDate = updateData.StartDate;
                    newitemTopInstallment.Edition = updateData.Edition;
                    newitemTopInstallment.EndDate = updateData.EndDate;
                    newitemTopInstallment.ItemID = updateData.ItemID;
                    newitemTopInstallment.TopInstallment = updateData.TopInstallment;
                    newitemTopInstallment.CreateDate = nowDateTime;
                    newitemTopInstallment.CreateUser = updateUser;
                    newitemTopInstallment.UpdateDate = nowDateTime;
                    newitemTopInstallment.UpdateUser = updateUser;

                    newitemTopInstallmentList.Add(newitemTopInstallment);
                }
                this._itemTopInstallment.CreateRange(newitemTopInstallmentList);


                updateDataCell = null;
            }
            catch
            {
                // 若沒有錯誤訊息，才填寫更新失敗訊息
                if (result.Count == 0)
                {
                    if (errorIdCell != null && errorIdCell.Count > 0)
                    {
                        foreach (int id in errorIdCell)
                        {
                            result.Add(string.Format("ID = {0} 更新失敗。"));
                        }
                    }
                    else
                    {
                        result.Add("更新失敗。");
                    }
                }

                errorIdCell = null;
            }
            finally
            {
                if (updateDataCell != null)
                {
                    updateDataCell = null;
                }

                if (errorIdCell != null)
                {
                    errorIdCell = null;
                }
            }

            return result;
        }

        /// <summary>
        /// 將同一個 Edition 的 SerialNumber + 1
        /// </summary>
        /// <param name="edition">賣場最高分期項目編號</param>
        /// <param name="updateUser">更新人</param>
        /// <returns>tue:更新成功, false:更新失敗</returns>
        public bool AddSerialNumber(int edition, string updateUser)
        {
            bool result = false;

            // 同一個 Edition 的賣場最高分期
            List<ItemTopInstallment> itemTopInstallmentCell = null;

            // 更新日期
            DateTime nowDatatime = DateTime.Now;

            try
            {
                // 取得相同 Edition 的資料
                itemTopInstallmentCell = this._itemTopInstallment.GetAll().Where(x => x.Edition == edition).ToList();

                if (itemTopInstallmentCell == null || itemTopInstallmentCell.Count == 0)
                {
                    throw new Exception();
                }

                // 將 SerialNumber + 1
                foreach (ItemTopInstallment itemInstallmentRule in itemTopInstallmentCell)
                {
                    itemInstallmentRule.SerialNumber++;
                    itemInstallmentRule.UpdateDate = nowDatatime;
                    itemInstallmentRule.UpdateUser = updateUser;
                }

                this._itemTopInstallment.UpdateRange(itemTopInstallmentCell);

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
