using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TWNewEgg.CategoryRepoAdapters.Interface;
using TWNewEgg.DAL.Interface;
using TWNewEgg.Models.DBModels.TWSQLDB;

namespace TWNewEgg.CategoryRepoAdapters
{
    public class CategoryRepoAdapter : ICategoryRepoAdapter
    {
        private IRepository<Category> _categoryRepoAdapter;

        public CategoryRepoAdapter(IRepository<Category> categoryRepoAdapter)
        {
            this._categoryRepoAdapter = categoryRepoAdapter;
        }

        public void SetContextTimeOut(int seconds)
        {
            if (seconds <= 0 && seconds > 120)
            {
                seconds = 15;
            }
            ((IObjectContextAdapter)this._categoryRepoAdapter.GetContext()).ObjectContext.CommandTimeout = seconds;
        }

        public IQueryable<Category> Category_GetAll()
        {
            return this._categoryRepoAdapter.GetAll();
        }

        public IQueryable<Category> LoadCategoryChildId(int parentId)
        {
            IQueryable<Category> Category;

            if (parentId == null)
            {
                return null;
            }

            Category = _categoryRepoAdapter.GetAll().Where(x => x.ParentID == parentId).AsQueryable();

            return Category;

        }

        public Category LoadCategoryData(int CategoryId)
        {
            Category Category;

            if (CategoryId == null)
            {
                return null;
            }

            Category = _categoryRepoAdapter.GetAll().Where(x => x.ID == CategoryId).SingleOrDefault();

            return Category;

        }

        public Category UpdateCategory(Category newData)
        {
            _categoryRepoAdapter.Update(newData);
            return newData;
        }
    }
}
