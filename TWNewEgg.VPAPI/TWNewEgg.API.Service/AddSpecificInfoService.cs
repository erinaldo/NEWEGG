using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TWNewEgg.API.Service
{
    /// <summary>
    /// Add Specific Info (APIService)
    /// </summary>
    
    //add by Ian & thisway
    public class AddSpecificInfoService
    {

        public Models.ActionResponse<Models.SpecificInfoResult> Create(Models.SpecificInfo ProductSpecificInfo)
        {
            Models.ActionResponse<Models.SpecificInfoResult> result = new Models.ActionResponse<Models.SpecificInfoResult>();

            DB.TWSqlDBContext db = new DB.TWSqlDBContext();

            if (ProductSpecificInfo != null)
            {
                DB.TWSQLDB.Models.Product Product = new DB.TWSQLDB.Models.Product();
                Product.Length = ProductSpecificInfo.Length;
                Product.Width = ProductSpecificInfo.Width;
                Product.Height = ProductSpecificInfo.Height;
                Product.Weight = ProductSpecificInfo.Weight;
                Product.SaleType = ProductSpecificInfo.SaleType;
                Product.Description = ProductSpecificInfo.Description;
                //Product.Note = ProductSpecificInfo.Note;
                //Product.Shop = ProductSpecificInfo.ShipDangerItems;
                //Product.ProhibitAge = ProductSpecificInfo.ProhibitAge;
                //Product.ChokingDanger = ProductSpecificInfo.ChokingDanger;
                
                db.Product.Add(Product);
                db.SaveChanges();
            }

            return result;
        }
    }
}
