using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TWNewEgg.API.Service
{
    /// <summary>
    /// Add Basic Info (APIService)
    /// </summary>
    
    //add by thisway
    public class AddBasicInfoService
    {

        public Models.ActionResponse<Models.BasicInfoResult> Create(Models.BasicInfo ProductBasicinfo)
        {
            Models.ActionResponse<Models.BasicInfoResult> result = new Models.ActionResponse<Models.BasicInfoResult>();

            DB.TWSqlDBContext db = new DB.TWSqlDBContext();

            if (ProductBasicinfo != null)
            {
                result.IsSuccess = true;
                result.Body = new Models.BasicInfoResult();
                result.Body.SellerProductID = db.Product.Where(x => x.Status == 0).FirstOrDefault().SellerProductID;
            }
            
            return result;
        }

        /// <summary>
        /// 獲取製造商
        /// </summary>
        /// <returns></returns>
        public Models.ActionResponse<Models.BasicInfoResult> GetManufacturer()
        {
            Models.ActionResponse<Models.BasicInfoResult> result = new Models.ActionResponse<Models.BasicInfoResult>();

            DB.TWSellerPortalDBContext db = new DB.TWSellerPortalDBContext();

            

            result.Body = new Models.BasicInfoResult();


            result.Body.ManufacturerName = db.Seller_ManufactureInfo.Where(x =>x.ManufactureName != null).Select(x =>x.ManufactureName).ToList();

            return result;
        }
    }
}
