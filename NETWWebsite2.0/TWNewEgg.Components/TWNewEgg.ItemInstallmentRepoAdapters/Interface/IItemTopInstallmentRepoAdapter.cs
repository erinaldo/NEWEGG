using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TWNewEgg.Models.DBModels.TWSQLDB;

namespace TWNewEgg.ItemInstallmentRepoAdapters.Interface
{
    /// <summary>
    /// 賣場最高分期期數
    /// </summary>
    public interface IItemTopInstallmentRepoAdapter
    {
        /// <summary>
        /// 取得所有賣場最高分期期數
        /// </summary>
        /// <remarks>只取得 SerialNumber = 0 的部份</remarks>
        /// <returns></returns>
        IQueryable<ItemTopInstallment> GetAll();

        /// <summary>
        /// 取得賣場的可用分期
        /// </summary>
        /// <param name="itemIdCell">賣場編號清單</param>
        /// <returns>可用分期</returns>
        List<int> GetAvailableInstallments(IEnumerable<int> itemIdCell);

        /// <summary>
        /// 新增賣場最高分期期數
        /// </summary>
        /// <param name="createDataCell">新增項目清單</param>
        /// <param name="createUser">建立人</param>
        /// <returns>true:新增成功; false:新增失敗</returns>
        bool Create(List<ItemTopInstallment> createDataCell, string createUser);

        /// <summary>
        /// 更新賣場最高分期期數
        /// </summary>
        /// <param name="updateDataCell">更新項目清單</param>
        /// <param name="updateUser">更新人</param>
        /// <returns>錯誤訊息</returns>
        string Update(List<ItemTopInstallment> updateDataCell, string updateUser);

        /// <summary>
        /// 啟用賣場最高分期期數
        /// </summary>
        /// <param name="id">賣場最高分期期數編號清單</param>
        /// <param name="updateUser">更新人</param>
        /// <returns>錯誤訊息</returns>
        List<string> Activate(List<int> id, string updateUser);

        /// <summary>
        /// 關閉賣場最高分期期數
        /// </summary>
        /// <param name="id">賣場最高分期期數編號清單</param>
        /// <param name="updateUser">更新人</param>
        /// <returns>錯誤訊息</returns>
        List<string> Deactivate(List<int> id, string updateUser);

        /// <summary>
        /// 將同一個 Edition 的 SerialNumber + 1
        /// </summary>
        /// <param name="edition">賣場最高分期項目編號</param>
        /// <param name="updateUser">更新人</param>
        /// <returns>tue:更新成功, false:更新失敗</returns>
        bool AddSerialNumber(int edition, string updateUser);
    }
}
