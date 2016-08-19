using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TWNewEgg.API.Service
{
    public class FindSellerService
    {
        private DB.TWSellerPortalDBContext db = new DB.TWSellerPortalDBContext();

        /// <summary>
        /// autocomplete search word
        /// need to All seller name
        /// </summary>
        public Models.ActionResponse<List<string>> SellerName()
        {
            Models.ActionResponse<List<string>> SellerName = new Models.ActionResponse<List<string>>();
            SellerName.Body = new List<string>();
                
            SellerName.Body = db.Seller_BasicInfo.Where(x => x.SellerID != null).Select(x => x.SellerName).ToList<string>();
            if (SellerName.Body == null)
            {
                SellerName.Msg = "Table Seller_Financial doesn't have any Data!";
                SellerName.Code = 0;
                SellerName.IsSuccess = false;
            }
            else
            {
                SellerName.Msg = "Success";
                SellerName.Code = 0;
                SellerName.IsSuccess = true;
            }
            return SellerName;
        }

        /// <summary>
        /// Search Seller 
        /// type : 0. by ID
        ///        1. by Name
        ///        2. by Email
        ///        3. by Phone
        /// </summary>
        /// <param name="type">Table Seller_BasicInfo search seller by type</param>
        /// <param name="searchword">search Table Seller_BasicInfo words</param>
        /// <param name="star">Begain </param>
        /// <param name="end">Table Seller_BasicInfo SellerID</param>
        public Models.ActionResponse<List<DB.TWSELLERPORTALDB.Models.Seller_BasicInfo>> SearchSeller(int type = 0, string searchword = "")
        {
            Models.ActionResponse<List<DB.TWSELLERPORTALDB.Models.Seller_BasicInfo>> SellerInfo = new  Models.ActionResponse<List<DB.TWSELLERPORTALDB.Models.Seller_BasicInfo>>();
            List<DB.TWSELLERPORTALDB.Models.Seller_BasicInfo> SearchSeller = new List<DB.TWSELLERPORTALDB.Models.Seller_BasicInfo>();

            if (searchword == "")
            {
                SearchSeller = db.Seller_BasicInfo.ToList<DB.TWSELLERPORTALDB.Models.Seller_BasicInfo>();

            }
            else { 
                switch (type) { 

                    case 0:
                        int id = Int16.Parse(searchword);
                        SearchSeller = db.Seller_BasicInfo.Where(x => x.SellerID == id).ToList<DB.TWSELLERPORTALDB.Models.Seller_BasicInfo>();
                        break;

                    case 1:
                        SearchSeller = db.Seller_BasicInfo.Where(x => x.SellerName.Contains(searchword)).ToList<DB.TWSELLERPORTALDB.Models.Seller_BasicInfo>();
                        break;

                    case 2:
                        SearchSeller = db.Seller_BasicInfo.Where(x => x.EmailAddress.Contains(searchword)).ToList<DB.TWSELLERPORTALDB.Models.Seller_BasicInfo>();
                        break;

                    case 3:
                        SearchSeller = db.Seller_BasicInfo.Where(x => x.Phone.Contains(searchword)).ToList<DB.TWSELLERPORTALDB.Models.Seller_BasicInfo>();
                        break;
                }
            
            }

            SellerInfo.Body = GiveSellerStatusFullName(SearchSeller);

            if (SellerInfo.Body.Count == 0)
            {
                SellerInfo.Msg = "Table Seller_BasicInfo can't find data!";
                SellerInfo.Code = 0;
                SellerInfo.IsSuccess = false;
            }
            else
            {
                SellerInfo.Msg = "Success";
                SellerInfo.Code = 0;
                SellerInfo.IsSuccess = true;
            }

            return SellerInfo;
        }

        private List<TWNewEgg.DB.TWSELLERPORTALDB.Models.Seller_BasicInfo> GiveSellerStatusFullName(List<TWNewEgg.DB.TWSELLERPORTALDB.Models.Seller_BasicInfo> OriginalData) { 
            List<TWNewEgg.DB.TWSELLERPORTALDB.Models.Seller_BasicInfo> ChangeStatus = new List<DB.TWSELLERPORTALDB.Models.Seller_BasicInfo>();
            
            foreach(TWNewEgg.DB.TWSELLERPORTALDB.Models.Seller_BasicInfo FullStatusName in OriginalData){
                switch (FullStatusName.SellerStatus) { 
                    case "A":
                        FullStatusName.SellerStatus = "Acitve";
                        break;
                    case "C":
                        FullStatusName.SellerStatus = "Close";
                        break;
                    case "I":
                        FullStatusName.SellerStatus = "InActive";
                        break;
                }

                ChangeStatus.Add(FullStatusName);
            }
            
            return ChangeStatus;
        }
    }
}
