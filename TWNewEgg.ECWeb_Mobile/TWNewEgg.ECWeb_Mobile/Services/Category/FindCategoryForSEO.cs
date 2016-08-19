using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TWNewEgg.Models.ViewModels.Category;

namespace TWNewEgg.ECWeb_Mobile.Services.Category
{
    public class FindCategoryForSEO
    {
        /// <summary>
        /// Find current category ID by layer
        /// </summary>
        /// <param name="category"></param>
        /// <param name="layer"></param>
        /// <returns></returns>
        public int FindCategoryForURL(Category_TreeItem category, int layer)
        {
            if (category == null)
            {
                return 0;
            }
            if (category.Parents == null)
            {
                return category.category_id;
            }
            if (category.category_layer == layer)
            {
                return category.category_id;
            }
            return FindCategoryForURL(category.Parents, layer++);
        }
    }
}