using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TWNewEgg.Models.DomainModels.Category;
using TWNewEgg.Models.DomainModels.Message;

namespace TWNewEgg.CategoryServices.Interface
{
    public interface ICategoryServices
    {
        ResponseMessage<List<CategoryDM>> GetCategoryByParentID(int parentId);
        ResponseMessage<CategoryDM> UpdateCategoryByCategoryID(CategoryDM NewData);
        ResponseMessage<List<CategoryDM>> UpdateCategoryByCategoryIDList(List<CategoryDM> NewDataList);
        /// <summary>
        /// 撈category資料
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        CategoryDM GetCategoryByCategoryID(int Id);
        /// <summary>
        /// Get all parent category by categoryIDs.
        /// </summary>
        /// <param name="categoryIDs">Category IDs</param>
        /// <returns></returns>
        List<Category_TreeItem> GetAllParentCategoriesByCIDs(List<int> categoryIDs);
        /// <summary>
        /// Get all parent category by itemIDs.
        /// </summary>
        /// <param name="categoryIDs">Category IDs</param>
        /// <returns></returns>
        List<Category_TreeItem> GetAllParentCategoriesByItemIDs(List<int> itemIDs);
    }
}
