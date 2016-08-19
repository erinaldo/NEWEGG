using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TWNewEgg.DAL.Interface;
using TWNewEgg.Models.DBModels.TWSQLDB;
using TWNewEgg.StoreRepoAdapters.Interface;

namespace TWNewEgg.StoreRepoAdapters
{
    public class StoreRepoAdapter : IStoreRepoAdapter
    {
        private IRepository<Advertising> _advertisingRepo;
        private IRepository<Category> _categoryRepo;
        private IRepository<SubCategory_OptionStore> _optionStoreRepo;
        private IRepository<SubCategory_NormalStore> _normalStoreRepo;
        private IRepository<WindowBlocks> _windowBlocksRepo;
        private IRepository<Subcategorygroup> _subcategorygroupRepo;
        private IRepository<Subcategorylogo> _subcategorylogoRepo;
        private IRepository<ItemAndSubCategoryMapping_NormalStore> _normalStoreItemList;
        public StoreRepoAdapter(IRepository<Advertising> advertisingRepo, IRepository<Category> categoryRepo,
            IRepository<SubCategory_OptionStore> optionStoreRepo, IRepository<Subcategorygroup> subcategorygroupRepo,
            IRepository<Subcategorylogo> subcategorylogoRepo, IRepository<SubCategory_NormalStore> normalStoreRepo,
            IRepository<ItemAndSubCategoryMapping_NormalStore> normalStoreItemList, IRepository<WindowBlocks> windowBlocksRepo)
        {
            this._advertisingRepo = advertisingRepo;
            this._categoryRepo = categoryRepo;
            this._optionStoreRepo = optionStoreRepo;
            this._subcategorygroupRepo = subcategorygroupRepo;
            this._subcategorylogoRepo = subcategorylogoRepo;
            this._normalStoreRepo = normalStoreRepo;
            this._normalStoreItemList = normalStoreItemList;
            this._windowBlocksRepo = windowBlocksRepo;
        }

        public IQueryable<Advertising> Advertising_GetAll()
        {
            return this._advertisingRepo.GetAll();
        }

        public IQueryable<Category> Category_GetAll()
        {
            return this._categoryRepo.GetAll();
        }

        public IQueryable<SubCategory_OptionStore> OptionStore_GetAll()
        {
            return this._optionStoreRepo.GetAll();
        }
        
        public IQueryable<SubCategory_NormalStore> NormalStore_GetAll()
        {
            return this._normalStoreRepo.GetAll();
        }

        public IQueryable<Subcategorygroup> Subcategorygroup_GetAll()
        {
            return this._subcategorygroupRepo.GetAll();
        }

        public IQueryable<Subcategorylogo> Subcategorylogo_GetAll()
        {
            return this._subcategorylogoRepo.GetAll();
        }

        public IQueryable<ItemAndSubCategoryMapping_NormalStore> NormalStoreItem_GetAll()
        {
            return this._normalStoreItemList.GetAll();
        }

        public IQueryable<WindowBlocks> WindowBlocks_GetAll()
        {
            return this._windowBlocksRepo.GetAll();
        }

        //新增subcatrgory_optionstore
        public void Create(SubCategory_OptionStore argObjSubCategory_OptionStore)
        {
            if (argObjSubCategory_OptionStore == null)
            {
               return ;
            }
            try
            {
                this._optionStoreRepo.Create(argObjSubCategory_OptionStore);
            }
            catch (Exception ex)
            {
                throw new NotImplementedException(ex.Message, ex);
            }
        }
        //更新subcatrgory_optionstore
        public bool Update(SubCategory_OptionStore argObjSubCategory_OptionStore)
        {
            if (argObjSubCategory_OptionStore == null)
            {
                return false;
            }

            bool boolExec = false;

            try
            {
                this._optionStoreRepo.Update(argObjSubCategory_OptionStore);
                boolExec = true;
            }
            catch (Exception ex)
            {
                throw new NotImplementedException(ex.Message, ex);
            }

            return boolExec;
        }


        public IQueryable<SubCategory_OptionStore> GetById(int CategoryId)
        {
            IQueryable<SubCategory_OptionStore> queryResult = null;

            try
            {
                queryResult = this._optionStoreRepo.GetAll().Where(x=>x.CategoryID == CategoryId);
            }
            catch (Exception ex)
            {
                throw new NotImplementedException(ex.Message, ex);
            }

            return queryResult;
        }
    }
}

