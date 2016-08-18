using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TWNewEgg.Models.DBModels.TWSQLDB;

namespace TWNewEgg.CategoryRepoAdapters.Interface
{
    public interface ICategoryRepoAdapter
    {
        void SetContextTimeOut(int seconds);

        IQueryable<Category> Category_GetAll();

        IQueryable<Category> LoadCategoryChildId(int parentId);

        Category LoadCategoryData(int CategoryId);

        Category UpdateCategory(Category newData);
    }
}
