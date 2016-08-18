using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using TWNewEgg.Models.DBModels.TWSQLDB;

namespace TWNewEgg.ItemSketchRepoAdapters.Interface
{
    public interface IItemSketchRepoAdapter
    {
        /// <summary>
        /// 取得所有的 ItemSketch
        /// </summary>
        /// <returns></returns>
        IQueryable<ItemSketch> GetAll();

        /// <summary>
        /// 取得一筆 ItemSketch 
        /// </summary>
        /// <returns></returns>
        IQueryable<ItemSketch> GetById(int argSketch_ID);

        /// <summary>
        /// 新增 ItemSketch
        /// </summary>
        /// <param name="argItemSketch"></param>
        /// <returns></returns>
        ItemSketch Create(ItemSketch argItemSketch);

        /// <summary>
        /// 修改 ItemSketch
        /// </summary>
        /// <param name="argItemSketch"></param>
        /// <returns></returns>
        ItemSketch Update(ItemSketch argItemSketch);

        /// <summary>
        /// 刪除單筆 by ItemSketch.ID
        /// </summary>
        /// <param name="argItemSketchID"></param>
        /// <returns></returns>
        bool Delete(int argItemSketchID, string UpdateUserID);

        /// <summary>
        /// 刪除多筆
        /// </summary>
        /// <param name="argItemSketchID"></param>
        /// <returns></returns>
        bool Delete(List<int> argItemSketchID, string UpdateUserID);
    }
}
