using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TWNewEgg.API.Service
{
    public class SellerUserGroupRelationshipSearchService
    {
        private DB.TWSellerPortalDBContext db = new DB.TWSellerPortalDBContext();

        public Models.ActionResponse<DB.TWSELLERPORTALDB.Models.User_Group> GetUser_GroupByUser(string user, int type)
        {
            Models.ActionResponse<DB.TWSELLERPORTALDB.Models.User_Group> UserGroup = new Models.ActionResponse<DB.TWSELLERPORTALDB.Models.User_Group>();
            try
            {
                UserGroup.Body = new DB.TWSELLERPORTALDB.Models.User_Group();

                if (type == 0)
                {
                    //用ID查詢
                    int userid = 0;
                    Int32.TryParse(user, out userid);
                    UserGroup.Body = (from usr in db.Seller_User
                                      join grp in db.User_Group on usr.GroupID equals grp.GroupID
                                      where usr.UserID == userid
                                      select grp).FirstOrDefault();
                }
                else if (type == 1)
                {
                    //用Email查詢
                    UserGroup.Body = (from usr in db.Seller_User
                                      join grp in db.User_Group on usr.GroupID equals grp.GroupID
                                      where usr.UserEmail == user
                                      select grp).FirstOrDefault();
                }
                UserGroup.Code = 0;
                UserGroup.IsSuccess = true;
                UserGroup.Msg = "OK";
            }
            catch (Exception ex)
            {
                UserGroup.Code = 0;
                UserGroup.IsSuccess = false;
                UserGroup.Msg = ex.Message;
                UserGroup.Body = null;
            }
            return UserGroup;
        }

        public Models.ActionResponse<List<DB.TWSELLERPORTALDB.Models.User_Group>> GetUser_GroupBySeller(string seller, int type)
        {
            
            Models.ActionResponse<List<DB.TWSELLERPORTALDB.Models.User_Group>> UserGroup = new Models.ActionResponse<List<DB.TWSELLERPORTALDB.Models.User_Group>>();
            try
            {
                UserGroup.Body = new List<DB.TWSELLERPORTALDB.Models.User_Group>();

                if (type == 0)
                {
                    //用ID查詢
                    int id = 0;
                    Int32.TryParse(seller, out id);

                    UserGroup.Body = (from slr in db.Seller_BasicInfo
                                      join usr in db.Seller_User on slr.SellerID equals usr.SellerID
                                      join grp in db.User_Group on usr.GroupID equals grp.GroupID
                                      where slr.SellerID == id
                                      select grp).ToList<DB.TWSELLERPORTALDB.Models.User_Group>();
                }
                else if (type == 1)
                {
                    //用Email查詢
                    UserGroup.Body = (from slr in db.Seller_BasicInfo
                                      join usr in db.Seller_User on slr.SellerID equals usr.SellerID
                                      join grp in db.User_Group on usr.GroupID equals grp.GroupID
                                      where slr.EmailAddress == seller
                                      select grp).ToList<DB.TWSELLERPORTALDB.Models.User_Group>();
                }

                UserGroup.Code = 0;
                UserGroup.IsSuccess = true;
                UserGroup.Msg = "OK";
            }
            catch (Exception ex)
            {
                UserGroup.Code = 0;
                UserGroup.IsSuccess = false;
                UserGroup.Msg = ex.Message;
                UserGroup.Body = null;
            }
            return UserGroup;
        }
    }
}
