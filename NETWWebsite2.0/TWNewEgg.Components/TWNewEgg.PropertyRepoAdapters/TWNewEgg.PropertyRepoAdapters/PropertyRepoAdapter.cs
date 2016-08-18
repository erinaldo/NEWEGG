using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using TWNewEgg.DAL.Interface;
using TWNewEgg.Models.DBModels.TWSQLDB;
using TWNewEgg.Models.DBModels.TWSQLDBExtModels;
using TWNewEgg.PropertyRepoAdapters.Interface;

namespace TWNewEgg.PropertyRepoAdapters
{
    public class PropertyRepoAdapter : IPropertyRepoAdapter
    {
        private IRepository<ProductProperty> _productPropertyRepo;
        private IRepository<ItemPropertyGroup> _itemPropertyGroupRepo;
        private IRepository<ItemPropertyName> _itemPropertyNameRepo;
        private IRepository<ItemPropertyValue> _itemPropertyValueRepo;

        public PropertyRepoAdapter(IRepository<ProductProperty> productPropertyRepo, IRepository<ItemPropertyGroup> itemPropertyGroupRepo, 
            IRepository<ItemPropertyName> itemPropertyNameRepo, IRepository<ItemPropertyValue> itemPropertyValueRepo)
        {
            this._itemPropertyGroupRepo = itemPropertyGroupRepo;
            this._productPropertyRepo = productPropertyRepo;
            this._itemPropertyNameRepo = itemPropertyNameRepo;
            this._itemPropertyValueRepo = itemPropertyValueRepo;
        }

        public IQueryable<ItemPropertyGroup> GetGroups(int categoryId, string[] hide)
        {
            var result = this._itemPropertyGroupRepo.GetAll().Where(x => x.CategoryID == categoryId && hide.Contains(x.Hide));
            return result;
        }

        public IQueryable<ItemPropertyName> GetNames(List<int> allGroupIds, string[] hide)
        {
            var result = this._itemPropertyNameRepo.GetAll().Where(x => allGroupIds.Contains(x.GroupID) && hide.Contains(x.Hide));
            return result;
        }

        public IQueryable<ItemPropertyValue> GetValues(List<int> allNameIds, string[] hide)
        {
            var result = this._itemPropertyValueRepo.GetAll().Where(x => allNameIds.Contains(x.PropertyNameID) && hide.Contains(x.Hide));
            return result;
        }

        public List<int> FilterProductIds(List<int> productIds, int categoryId, List<int> propertyValueIds)
        {
            List<int> propGroupIds = _itemPropertyGroupRepo.GetAll().Where(x => x.CategoryID == categoryId).Select(x => x.ID).ToList();
            List<ProductProperty> props = _productPropertyRepo.GetAll().Where(x => propertyValueIds.Contains(x.ProductValueID) && propGroupIds.Contains(x.GroupID)).ToList();
            List<int> propNameIDs = props.Select(x => x.PropertyNameID).Distinct().ToList();

            if (propNameIDs == null || propNameIDs.Count == 0)
            {
                propNameIDs = new List<int>();
                return propNameIDs;
            }

            foreach (int nameID in propNameIDs)
            {
                var query = GetQueryPropertyFunc(nameID, productIds.ToList(), propertyValueIds);
                productIds = props.Where(query).Where(x => x.Show == 0).Select(x => x.ProductID).ToList();
            }

            return productIds;
        }

        private Func<ProductProperty, bool> GetQueryPropertyFunc(int nameID, List<int> productIDs, List<int> propertyIDs)
        {
            ParameterExpression pe = Expression.Parameter(typeof(ProductProperty), "x");

            Expression left = Expression.Constant(propertyIDs);
            MethodInfo contains = typeof(List<int>).GetMethod("Contains");
            Expression right = Expression.Property(pe, "ProductValueID");
            Expression call = Expression.Call(typeof(Enumerable), "Contains", new[] { right.Type }, left, right);

            Expression left2 = Expression.Constant(productIDs);
            MethodInfo contains2 = typeof(List<int>).GetMethod("Contains");
            Expression right2 = Expression.Property(pe, "ProductID");
            Expression call2 = Expression.Call(typeof(Enumerable), "Contains", new[] { right2.Type }, left2, right2);

            Expression left3 = Expression.Constant(nameID);
            Expression right3 = Expression.Property(pe, "PropertyNameID");
            Expression call3 = Expression.Equal(left3, right3);

            Expression final = Expression.AndAlso(call, call2);
            final = Expression.AndAlso(final, call3);
            var lambda = Expression.Lambda<Func<ProductProperty, bool>>(final, pe);

            return lambda.Compile();
        }

        #region IPP Read Property
        public IQueryable<DbItemProperty> GetItemPropertyInfo(int categoryID)
        {
            var group = this._itemPropertyGroupRepo.GetAll().Where(x=>x.CategoryID == categoryID);
            var propertyname = this._itemPropertyNameRepo.GetAll();
            var propertyvalue = this._itemPropertyValueRepo.GetAll();
            IQueryable<DbItemProperty> result = group
                .Join(propertyname,
                    i => i.ID,
                    p => p.GroupID,
                    (i, p) => new
                    {
                        group = i,
                        propertyname = p
                    })
                .Join(propertyvalue,
                   ip => ip.propertyname.ID,
                   s => s.PropertyNameID,
                   (ip, s) => new DbItemProperty
                   {
                       ItemPropertyGroup = ip.group,
                       ItemPropertyName = ip.propertyname,
                       ItemPropertyValue = s
                   }).Distinct();

            return result;
        }
        public IQueryable<ProductProperty>GetAllProductProperty()
        {
            try
            {
                IQueryable<ProductProperty> result = this._productPropertyRepo.GetAll();
                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
        #endregion
        #region IPP Create Property
        public ItemPropertyGroup CreatePropertyGroup(TWNewEgg.Models.DBModels.TWSQLDB.ItemPropertyGroup itemPropertyGroup)
        {
            try
            {
                _itemPropertyGroupRepo.Create(itemPropertyGroup);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return itemPropertyGroup;

        }
        public ItemPropertyName CreatePropertyName(TWNewEgg.Models.DBModels.TWSQLDB.ItemPropertyName itemPropertyName)
        {
            try
            {
                _itemPropertyNameRepo.Create(itemPropertyName);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return itemPropertyName;

        }
        public ItemPropertyValue CreatePropertyValue(TWNewEgg.Models.DBModels.TWSQLDB.ItemPropertyValue itemPropertyValue)
        {
            try
            {
                _itemPropertyValueRepo.Create(itemPropertyValue);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return itemPropertyValue;

        }
        #endregion
        #region IPP Update Property
        public ItemPropertyGroup UpdatePropertyGroup(TWNewEgg.Models.DBModels.TWSQLDB.ItemPropertyGroup itemPropertyGroup)
        {
            try
            {
                this._itemPropertyGroupRepo.Update(itemPropertyGroup);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return itemPropertyGroup;

        }
        public ItemPropertyName UpdatePropertyName(TWNewEgg.Models.DBModels.TWSQLDB.ItemPropertyName itemPropertyName)
        {
            try
            {
                this._itemPropertyNameRepo.Update(itemPropertyName);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return itemPropertyName;

        }
        public ItemPropertyValue UpdatePropertyValue(TWNewEgg.Models.DBModels.TWSQLDB.ItemPropertyValue itemPropertyValue)
        {
            try
            {
                this._itemPropertyValueRepo.Update(itemPropertyValue);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return itemPropertyValue;

        }
        #endregion

        public bool DeletePropertyGroup(ItemPropertyGroup itemPropertyGroup)
        {
            try
            {
                this._itemPropertyGroupRepo.Delete(itemPropertyGroup);
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public bool DeletePropertyName(ItemPropertyName itemPropertyName)
        {
            try
            {
                this._itemPropertyNameRepo.Delete(itemPropertyName);
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public bool DeletePropertyValue(ItemPropertyValue itemPropertyValue)
        {
            try
            {
                this._itemPropertyValueRepo.Delete(itemPropertyValue);
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public bool DeleteProductProperty(ProductProperty productProperty)
        {
            try
            {
                this._productPropertyRepo.Delete(productProperty);
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
    }
}
