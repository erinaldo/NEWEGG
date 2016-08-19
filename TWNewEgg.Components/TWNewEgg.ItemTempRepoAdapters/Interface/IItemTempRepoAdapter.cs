using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using TWNewEgg.Models.DBModels.TWSQLDB;

namespace TWNewEgg.ItemTempRepoAdapters.Interface
{
    public interface IItemTempRepoAdapter
    {
        /// <summary>
        /// 取得所有的 ItemTemp
        /// </summary>
        /// <returns></returns>
        IQueryable<ItemTemp> GetAll();

        /// <summary>
        /// 取得一筆 ItemTemp 
        /// </summary>
        /// <returns></returns>
        IQueryable<ItemTemp> GetById(int argTemp_ID);

        /// <summary>
        /// 新增 ItemTemp
        /// </summary>
        /// <param name="argItemTemp"></param>
        /// <returns></returns>
        ItemTemp Create(ItemTemp argItemTemp);

        /// <summary>
        /// 修改 ItemTemp
        /// </summary>
        /// <param name="argItemTemp"></param>
        /// <returns></returns>
        ItemTemp Update(ItemTemp argItemTemp);

        /// <summary>
        /// 刪除單筆 by ItemTemp.ID
        /// </summary>
        /// <param name="argItemTemp"></param>
        /// <returns></returns>
        bool Delete(int argItemTempID, string UpdateUser);

        /// <summary>
        /// 刪除多筆
        /// </summary>
        /// <param name="argItemTempID"></param>
        /// <returns></returns>
        bool Delete(List<int> argItemTempID, string UpdateUser);
    }
}
