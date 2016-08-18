using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;


namespace TWNewEgg.API.Service
{
    public class SellerChargeService
    {
        public enum ResponseCode
        {
            Success = 0,
            Error = 1
        }

        /// <summary>
        /// 取得DB內的資料
        /// </summary>
        /// <returns></returns>
        public API.Models.ActionResponse<List<DB.TWSELLERPORTALDB.Models.Seller_Charge>> GetChargeList(string countryCode)
        {

            DB.TWSellerPortalDBContext spdb = new DB.TWSellerPortalDBContext();

            API.Models.ActionResponse<List<DB.TWSELLERPORTALDB.Models.Seller_Charge>> result = new Models.ActionResponse<List<DB.TWSELLERPORTALDB.Models.Seller_Charge>>();

            List<DB.TWSELLERPORTALDB.Models.Seller_Charge> chargeList = new List<DB.TWSELLERPORTALDB.Models.Seller_Charge>();

            chargeList = spdb.Seller_Charge.Where(x => x.SellerID == 0 && x.CountryCode ==countryCode).ToList();

            if (chargeList.Count == 0)
            {
                result.Code = (int)ResponseCode.Error;
                result.IsSuccess = false;
                result.Msg = "查無資料";
            }
            else
            {
                result.Code = (int)ResponseCode.Success;
                result.IsSuccess = true;
                result.Msg = "List資料傳回";
                result.Body = chargeList;
            }

            return result;
        }

        /*
        public API.Models.ActionResponse<string> SaveSellerCharge(DB.TWSELLERPORTALDB.Models.Seller_Charge sellerCharge)
        {
            DB.TWSellerPortalDBContext spdb = new DB.TWSellerPortalDBContext();
            Models.ActionResponse<string> result = new Models.ActionResponse<string>();
            var checkSeller = spdb.Seller_Charge.Where(x => x.SellerID == sellerCharge.SellerID && x.CategoryID == sellerCharge.CategoryID).FirstOrDefault();

            if (checkSeller == null && sellerCharge.SN == 0)
            {
                sellerCharge.InDate = DateTime.UtcNow.AddHours(8);
                sellerCharge.InUserID = 1;

                spdb.Seller_Charge.Add(sellerCharge);
                spdb.SaveChanges();

                result.Code = (int)ResponseCode.Success;
                result.IsSuccess = true;
                result.Msg = "新增成功";
            }
            else if (checkSeller != null)
            {
                result.Code = (int)ResponseCode.Error;
                result.IsSuccess = false;
                result.Msg = "此Seller該筆佣金已給定";

            }
            else
            {
                sellerCharge.UpdateDate = DateTime.UtcNow.AddHours(8);
                sellerCharge.UpdateUserID = 1;


                spdb.Entry(sellerCharge).State = EntityState.Modified;
                spdb.SaveChanges();

                result.Code = (int)ResponseCode.Success;
                result.IsSuccess = true;
                result.Msg = "修改成功";
            }
            
            return result;
        }*/

        public API.Models.ActionResponse<string> SaveSellerCharge(TWNewEgg.API.Models.SaveSellerCharge sellerInvitation)
        {
            DB.TWSellerPortalDBContext spdb = new DB.TWSellerPortalDBContext();
            Models.ActionResponse<string> result = new Models.ActionResponse<string>();
            DB.TWSELLERPORTALDB.Models.Seller_Charge charge = new DB.TWSELLERPORTALDB.Models.Seller_Charge();

            var refList = spdb.Seller_Charge.Where(x => x.SellerID == 0 && x.CountryCode == sellerInvitation.SellerCountryCode && x.ChargeType == "S").ToList();
            if (sellerInvitation.ChargeType == "S")
            {               
                for (int i = 0; i < refList.Count(); i++)
                {
                    charge.SellerID = sellerInvitation.SellerID;
                    charge.CountryCode = sellerInvitation.SellerCountryCode;
                    charge.ChargeType = "S";
                    charge.CategoryID = refList[i].CategoryID;
                    charge.Commission = refList[i].Commission;
                    charge.InDate = DateTime.UtcNow.AddHours(8);
                    charge.InUserID = 1;
                    spdb.Seller_Charge.Add(charge);
                    spdb.SaveChanges();

                    result.Code = (int)ResponseCode.Success;
                    result.IsSuccess = true;
                    result.Msg = "S";
                    result.Body = "寫入成功";
                }
            }
            else
            {
                for (int i = 0; i < refList.Count(); i++)
                {
                    charge.SellerID = sellerInvitation.SellerID;
                    charge.CountryCode = sellerInvitation.SellerCountryCode;
                    charge.ChargeType = "A";
                    charge.CategoryID = refList[i].CategoryID;
                    charge.Commission = sellerInvitation.CommissionRate.Where(x => x.CategoryID == charge.CategoryID).Select(x => x.Commission).FirstOrDefault();
                    charge.InDate = DateTime.UtcNow.AddHours(8);
                    charge.InUserID = 1;
                    spdb.Seller_Charge.Add(charge);
                    spdb.SaveChanges();

                    result.Code = (int)ResponseCode.Success;
                    result.IsSuccess = true;
                    result.Msg = "A";
                    result.Body = "寫入成功";
                }
            }
            return result;
        }

    }
}
