using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TWNewEgg.CategoryService.Models;
using TWNewEgg.DB.TWSQLDB.Models;

namespace TWNewEgg.CategoryService.Interface
{
    public interface ICategoryService 
    {
        Dictionary<string, List<Category_TreeItem>> GetCategory(int? CategoryID, int? LayerID, int? CountryID, int? BrandID, int? SellerID, bool? isSecure = null);
        IEnumerable<MapPath> GetParents(int? CategoryID, int? LayerID, int? CountryID, int? BrandID, int? SellerID);
        IEnumerable<BrandInfo> GetBrands(int? CategoryID, int? LayerID, int? CountryID, int? BrandID, int? SellerID);
        Dictionary<string, string> XMLGenerate(int? password);
        Manufacture getManufactureById(int BrandID);
    }
}
