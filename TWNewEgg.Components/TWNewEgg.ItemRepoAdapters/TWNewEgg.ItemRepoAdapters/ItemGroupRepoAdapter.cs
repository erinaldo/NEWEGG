using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TWNewEgg.DAL.Interface;
using TWNewEgg.ItemRepoAdapters.Interface;

namespace TWNewEgg.ItemRepoAdapters
{
    public class ItemGroupRepoAdapter : IItemGroupRepoAdapter
    {
        private IRepository<TWNewEgg.Models.DBModels.TWSQLDB.ItemGroup> _ItemGroupRepo;
        private IRepository<TWNewEgg.Models.DBModels.TWSQLDB.ItemGroupProperty> _ItemGroupPropertyRepo;
        private IRepository<TWNewEgg.Models.DBModels.TWSQLDB.ItemGroupDetailProperty> _ItemGroupDetailPropertyRepo;
        private IRepository<TWNewEgg.Models.DBModels.TWSQLDB.ItemPropertyGroup> _PropertyGroupRepo;
        private IRepository<TWNewEgg.Models.DBModels.TWSQLDB.ItemPropertyName> _PropertyNameRepo;
        private IRepository<TWNewEgg.Models.DBModels.TWSQLDB.ItemPropertyValue> _PropertyValueRepo;

        public ItemGroupRepoAdapter(IRepository<TWNewEgg.Models.DBModels.TWSQLDB.ItemGroup> argItemGroupRepo,
            IRepository<TWNewEgg.Models.DBModels.TWSQLDB.ItemGroupProperty> argItemGroupPropertyRepo,
            IRepository<TWNewEgg.Models.DBModels.TWSQLDB.ItemGroupDetailProperty> argItemGroupDetailPropertyRepo,
            IRepository<TWNewEgg.Models.DBModels.TWSQLDB.ItemPropertyGroup> argPropertyGroupRepo,
            IRepository<TWNewEgg.Models.DBModels.TWSQLDB.ItemPropertyName> argPropertyNameRepo,
            IRepository<TWNewEgg.Models.DBModels.TWSQLDB.ItemPropertyValue> argPropertyValueRepo)
        {
            this._ItemGroupRepo = argItemGroupRepo;
            this._ItemGroupPropertyRepo = argItemGroupPropertyRepo;
            this._ItemGroupDetailPropertyRepo = argItemGroupDetailPropertyRepo;
            this._PropertyGroupRepo = argPropertyGroupRepo;
            this._PropertyNameRepo = argPropertyNameRepo;
            this._PropertyValueRepo = argPropertyValueRepo;
        }

        #region ItemGroup
        public IQueryable<TWNewEgg.Models.DBModels.TWSQLDB.ItemGroup> GetAllItemGroup()
        {
            IQueryable<TWNewEgg.Models.DBModels.TWSQLDB.ItemGroup> objResult = null;
            objResult = this._ItemGroupRepo.GetAll();
            return objResult;
        }

        public IQueryable<TWNewEgg.Models.DBModels.TWSQLDB.ItemGroup> GetItemGroupById(int argNumId)
        {
            IQueryable<TWNewEgg.Models.DBModels.TWSQLDB.ItemGroup> objResult = null;
            objResult = this._ItemGroupRepo.GetAll().Where(x=>x.ID == argNumId);
            return objResult;
        }
        #endregion

        #region ItemGroupProperty
        public IQueryable<TWNewEgg.Models.DBModels.TWSQLDB.ItemGroupProperty> GetAllItemGroupProperty()
        {
            IQueryable<TWNewEgg.Models.DBModels.TWSQLDB.ItemGroupProperty> objResult = null;
            objResult = this._ItemGroupPropertyRepo.GetAll();
            return objResult;
        }

        public IQueryable<TWNewEgg.Models.DBModels.TWSQLDB.ItemGroupProperty> GetItemGroupPropertyByGroupId(int argNumGroupId)
        {
            IQueryable<TWNewEgg.Models.DBModels.TWSQLDB.ItemGroupProperty> objResult = null;
            objResult = this._ItemGroupPropertyRepo.GetAll().Where(x=>x.GroupID == argNumGroupId);
            return objResult;
        }

        public IQueryable<TWNewEgg.Models.DBModels.TWSQLDB.ItemGroupProperty> GetItemGroupPropertyByGroupIdAndPropertyId(int argNumGroupId, int argNumPropertyId)
        {
            IQueryable<TWNewEgg.Models.DBModels.TWSQLDB.ItemGroupProperty> objResult = null;
            objResult = this._ItemGroupPropertyRepo.GetAll().Where(x=>x.GroupID == argNumGroupId && x.PropertyID == argNumPropertyId);
            return objResult;
        }
        #endregion

        #region ItemGroupDetailProperty
        public IQueryable<TWNewEgg.Models.DBModels.TWSQLDB.ItemGroupDetailProperty> GetAllItemGroupDetailProperty()
        {
            IQueryable<TWNewEgg.Models.DBModels.TWSQLDB.ItemGroupDetailProperty> objResult = null;
            objResult = this._ItemGroupDetailPropertyRepo.GetAll();
            return objResult;
        }

        public IQueryable<TWNewEgg.Models.DBModels.TWSQLDB.ItemGroupDetailProperty> GetItemGroupDetailPropertyByGroupId(int argNumGroupId)
        {
            IQueryable<TWNewEgg.Models.DBModels.TWSQLDB.ItemGroupDetailProperty> objResult = null;
            objResult = this._ItemGroupDetailPropertyRepo.GetAll().Where(x=>x.GroupID == argNumGroupId);
            return objResult;
        }

        public IQueryable<TWNewEgg.Models.DBModels.TWSQLDB.ItemGroupDetailProperty> GetItemGroupDetailPropertyByItemId(int argNumItemId)
        {
            IQueryable<TWNewEgg.Models.DBModels.TWSQLDB.ItemGroupDetailProperty> objResult = null;
            objResult = this._ItemGroupDetailPropertyRepo.GetAll().Where(x=>x.ItemID == argNumItemId);
            return objResult;
        }
        #endregion

        #region ItemPropertyGroup
        public IQueryable<TWNewEgg.Models.DBModels.TWSQLDB.ItemPropertyGroup> GetAllPropertyGroup()
        {
            IQueryable<TWNewEgg.Models.DBModels.TWSQLDB.ItemPropertyGroup> objResult = null;
            objResult = this._PropertyGroupRepo.GetAll();
            return objResult;
        }

        public IQueryable<TWNewEgg.Models.DBModels.TWSQLDB.ItemPropertyGroup> GetPropertyGroupById(int argNumId)
        {
            IQueryable<TWNewEgg.Models.DBModels.TWSQLDB.ItemPropertyGroup> objResult = null;
            objResult = this._PropertyGroupRepo.GetAll().Where(x=>x.ID == argNumId);
            return objResult;
        }

        public IQueryable<TWNewEgg.Models.DBModels.TWSQLDB.ItemPropertyGroup> GetPropertyGroupByCategoryId(int argNumCategoryId)
        {
            IQueryable<TWNewEgg.Models.DBModels.TWSQLDB.ItemPropertyGroup> objResult = null;
            objResult = this._PropertyGroupRepo.GetAll().Where(x=>x.CategoryID == argNumCategoryId);
            return objResult;
        }
        #endregion

        #region ItemPropertyName
        public IQueryable<TWNewEgg.Models.DBModels.TWSQLDB.ItemPropertyName> GetAllPropertyName()
        {
            IQueryable<TWNewEgg.Models.DBModels.TWSQLDB.ItemPropertyName> objResult = null;
            objResult = this._PropertyNameRepo.GetAll();
            return objResult;
        }

        public IQueryable<TWNewEgg.Models.DBModels.TWSQLDB.ItemPropertyName> GetPropertyNameById(int argNumId)
        {
            IQueryable<TWNewEgg.Models.DBModels.TWSQLDB.ItemPropertyName> objResult = null;
            objResult = this._PropertyNameRepo.GetAll().Where(x=>x.ID == argNumId);
            return objResult;
        }

        public IQueryable<TWNewEgg.Models.DBModels.TWSQLDB.ItemPropertyName> GetPropertyNameByGroupId(int argNumGroupId)
        {
            IQueryable<TWNewEgg.Models.DBModels.TWSQLDB.ItemPropertyName> objResult = null;
            objResult = this._PropertyNameRepo.GetAll().Where(x=>x.GroupID == argNumGroupId);
            return objResult;
        }
        #endregion

        #region ItemPropertyValue
        public IQueryable<TWNewEgg.Models.DBModels.TWSQLDB.ItemPropertyValue> GetAllPropertyValue()
        {
            IQueryable<TWNewEgg.Models.DBModels.TWSQLDB.ItemPropertyValue> objResult = null;
            objResult = this._PropertyValueRepo.GetAll();
            return objResult;
        }

        public IQueryable<TWNewEgg.Models.DBModels.TWSQLDB.ItemPropertyValue> GetPropertyValueById(int argNumId)
        {
            IQueryable<TWNewEgg.Models.DBModels.TWSQLDB.ItemPropertyValue> objResult = null;
            objResult = this._PropertyValueRepo.GetAll().Where(x=>x.ID == argNumId);
            return objResult;
        }
        public IQueryable<TWNewEgg.Models.DBModels.TWSQLDB.ItemPropertyValue> GetPropertyValueByPropertyNameId(int argNumNameId)
        {
            IQueryable<TWNewEgg.Models.DBModels.TWSQLDB.ItemPropertyValue> objResult = null;
            objResult = this._PropertyValueRepo.GetAll().Where(x=>x.PropertyNameID == argNumNameId);
            return objResult;
        }
        #endregion
    }
}
