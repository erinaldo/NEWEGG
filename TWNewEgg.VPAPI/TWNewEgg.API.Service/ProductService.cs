using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TWNewEgg.API.Service
{
    public class ProductService
    {
        public Models.ActionResponse<List<DB.TWSQLDB.Models.Product>> QueryProduct(Models.QueryProductCondition condition)
        {
            Models.ActionResponse<List<DB.TWSQLDB.Models.Product>> result = new Models.ActionResponse<List<DB.TWSQLDB.Models.Product>>();
            try
            {
                DB.TWSqlDBContext frontend = new DB.TWSqlDBContext();
                var query = frontend.Product.AsQueryable();
                
                if (condition.ProductID > 0)
                    query = query.Where(x => x.ID == condition.ProductID).AsQueryable();
                if (condition.SellerProductID != null)
                    query = query.Where(x => x.SellerProductID.IndexOf(condition.SellerProductID) >= 0).AsQueryable();
                if (condition.Keyword != null)
                    query = query.Where(x => x.Name.IndexOf(condition.Keyword) >= 0).AsQueryable();
                if (condition.SellerID > 0)
                    query = query.Where(x => x.SellerID == condition.SellerID).AsQueryable();
                if (condition.BeginDate.Year > 1)
                    query = query.Where(x => x.CreateDate >= condition.BeginDate).AsQueryable();
                if (condition.EndDate.Year > 1)
                    query = query.Where(x => x.CreateDate <= condition.EndDate).AsQueryable();

                var products = query.OrderBy(x=>x.ID).Skip(condition.Skip).Take(condition.PageSize).ToList();
                result.Finish(true, 0, "Done", products);
            }
            catch (Exception e)
            {
                result.Finish(false, 0, e.Message, null);
            }
            return result;
        }

        public Models.ActionResponse<DB.TWSQLDB.Models.Product> EditProduct(DB.TWSQLDB.Models.Product product)
        {
            Models.ActionResponse<DB.TWSQLDB.Models.Product> result = new Models.ActionResponse<DB.TWSQLDB.Models.Product>();

            try
            {
                DB.TWSqlDBContext frontend = new DB.TWSqlDBContext();
                frontend.Entry(product).State = System.Data.EntityState.Modified;
                frontend.SaveChanges();
                result.Finish(true, 0, "Done", product);
            }
            catch (Exception e)
            {
                result.Finish(false, 0, e.Message, null);
            }

            return result;
        }

        public Models.ActionResponse<DB.TWSQLDB.Models.Product> CreateProduct(DB.TWSQLDB.Models.Product product)
        {
            Models.ActionResponse<DB.TWSQLDB.Models.Product> result = new Models.ActionResponse<DB.TWSQLDB.Models.Product>();

            try
            {
                DB.TWSqlDBContext frontend = new DB.TWSqlDBContext();
                frontend.Entry(product).State = System.Data.EntityState.Added;
                frontend.SaveChanges();
                result.Finish(true, 0, "Done", product);
            }
            catch (Exception e)
            {
                result.Finish(false, 0, e.Message, null);
            }

            return result;
        }

        public Models.ActionResponse<DB.TWSQLDB.Models.Product> DeleteProduct(DB.TWSQLDB.Models.Product product)
        {
            Models.ActionResponse<DB.TWSQLDB.Models.Product> result = new Models.ActionResponse<DB.TWSQLDB.Models.Product>();

            try
            {
                DB.TWSqlDBContext frontend = new DB.TWSqlDBContext();
                frontend.Entry(product).State = System.Data.EntityState.Deleted;
                frontend.SaveChanges();
                result.Finish(true, 0, "Done", null);
            }
            catch (Exception e)
            {
                result.Finish(false, 0, e.Message, null);
            }

            return result;
        } 
    }
}
