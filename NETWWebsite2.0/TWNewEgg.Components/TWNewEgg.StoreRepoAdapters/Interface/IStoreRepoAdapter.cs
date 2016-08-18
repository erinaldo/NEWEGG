using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TWNewEgg.Models.DBModels.TWSQLDB;


namespace TWNewEgg.StoreRepoAdapters.Interface
{
    public interface IStoreRepoAdapter
    {
        IQueryable<Advertising> Advertising_GetAll();
        IQueryable<Category> Category_GetAll();
        IQueryable<SubCategory_OptionStore> OptionStore_GetAll();
        IQueryable<SubCategory_NormalStore> NormalStore_GetAll();
        IQueryable<Subcategorygroup> Subcategorygroup_GetAll();
        IQueryable<Subcategorylogo> Subcategorylogo_GetAll();
        IQueryable<ItemAndSubCategoryMapping_NormalStore> NormalStoreItem_GetAll();
        IQueryable<WindowBlocks> WindowBlocks_GetAll();
        
        //新增subcatrgory_optionstore
        void Create(SubCategory_OptionStore argObjSubCategory_OptionStore);
        //更新subcatrgory_optionstore
        bool Update(SubCategory_OptionStore argObjSubCategory_OptionStore);
        //搜尋Id
        IQueryable<SubCategory_OptionStore> GetById(int CategoryId);
    }
}
