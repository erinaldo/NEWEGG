using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TWNewEgg.Models.DBModels.TWSQLDB;

namespace TWNewEgg.CategoryRepoAdapters.Interface
{
    public interface ICategoryTopItemRepoAdapter
    {
        /// <summary>
        /// 取得CategoryTopItem data
        /// </summary>
        /// <returns></returns>
        IQueryable<CategoryTopItem> CategoryTopItem_GetAll();

        /// <summary>
        /// 新增或修改CategoryTopItem
        /// </summary>
        /// <param name="newData"></param>
        /// <returns></returns>
        CategoryTopItem SaveCategoryTopItemData(CategoryTopItem newData);

        /// <summary>
        /// 刪除CategoryTopItem
        /// </summary>
        /// <param name="ID"></param>
        void DeleteAdLayer3Items(int ID);
    }
}
