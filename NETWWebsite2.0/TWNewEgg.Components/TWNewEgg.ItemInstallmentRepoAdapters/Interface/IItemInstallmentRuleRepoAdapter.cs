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
    public interface IItemInstallmentRuleRepoAdapter
    {
        /// <summary>
        /// 取得所有分期預設
        /// </summary>
        /// <remarks>只取得 SerialNumber = 0 的部份</remarks>
        /// <returns>IQueryable ItemInstallmentRule</returns>
        IQueryable<ItemInstallmentRule> GetAll();

        /// <summary>
        /// 取得賣場售價的可用分期
        /// </summary>
        /// <param name="price">賣場售價</param>
        /// <returns>可用分期</returns>
        List<int> GetAvailableInstallments(int price);

        /// <summary>
        /// 新增分期預設
        /// </summary>
        /// <param name="createData">新增項目</param>
        /// <param name="createUser">建立人</param>
        /// <returns>新增項目的 ID</returns>
        int Create(ItemInstallmentRule createData, string createUser);

        /// <summary>
        /// 刪除分期預設
        /// </summary>
        /// <param name="deleteID">刪除項目 ID</param>
        /// <param name="updateUser">更新人</param>
        /// <returns>tue:刪除成功, false:刪除失敗</returns>
        bool Delete(int id, string updateUser);

        /// <summary>
        /// 將該筆 ID 相關的 SerialNumber + 1
        /// </summary>
        /// <param name="installmentID">分期數 ID</param>
        /// <param name="updateUser">更新人</param>
        /// <returns>tue:更新成功, false:更新失敗</returns>
        bool AddSerialNumber(int id, string updateUser);
    }
}
