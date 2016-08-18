using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TWNewEgg.DB;
using TWNewEgg.API.Models;
using System.Data.Entity.Validation;


namespace TWNewEgg.BulletinsMessageService.Service
{
    public class BulletinsMessageService : TWNewEgg.BulletinsMessageService.Interface.IBulletinsMessage
    {
        public API.Models.ActionResponse<DB.TWSELLERPORTALDB.Models.Seller_BulletinsMessage> GetBulletinsMessage(string System)
        {
            TWNewEgg.DB.TWSellerPortalDBContext spdb = new TWSellerPortalDBContext();
            API.Models.ActionResponse<DB.TWSELLERPORTALDB.Models.Seller_BulletinsMessage> QueryResult = new ActionResponse<DB.TWSELLERPORTALDB.Models.Seller_BulletinsMessage>();
            QueryResult.Body = new DB.TWSELLERPORTALDB.Models.Seller_BulletinsMessage();

            try
            {
                var result = spdb.Seller_BulletinsMessage.Where(x => x.FromSystem == System.ToLower() && x.Enable == "Y").FirstOrDefault();
                
                if (result != null)
                {
                    QueryResult.Body = result;
                    QueryResult.IsSuccess = true;
                    QueryResult.Code = (int)ResponseCode.Success;
                }
                else
                {
                    QueryResult.Body = null;
                    QueryResult.IsSuccess = false;
                    QueryResult.Code = (int)ResponseCode.Error;
                    QueryResult.Msg = "查無資料";
                }
            }
            catch (Exception ex)
            {
                throw new Exception("取公佈欄資料發生意外錯誤!", ex);
            }

            return QueryResult;
        }

        public ActionResponse<List<TWNewEgg.DB.TWSELLERPORTALDB.Models.Seller_BulletinsMessage>> GetBulletinsMessageList(string System)
        {
            TWNewEgg.DB.TWSellerPortalDBContext spdb = new TWSellerPortalDBContext();
            API.Models.ActionResponse<List<TWNewEgg.DB.TWSELLERPORTALDB.Models.Seller_BulletinsMessage>> QueryResult = new ActionResponse<List<TWNewEgg.DB.TWSELLERPORTALDB.Models.Seller_BulletinsMessage>>();
            QueryResult.Body = new List<TWNewEgg.DB.TWSELLERPORTALDB.Models.Seller_BulletinsMessage>();

            try
            {
                var result = spdb.Seller_BulletinsMessage.Where(x => x.FromSystem == System).ToList();

                if (result != null)
                {
                    QueryResult.Body = result;
                    QueryResult.IsSuccess = true;
                    QueryResult.Code = (int)ResponseCode.Success;
                }
                else
                {
                    QueryResult.Body = null;
                    QueryResult.IsSuccess = false;
                    QueryResult.Code = (int)ResponseCode.Error;
                    QueryResult.Msg = "查無資料";
                }
            }
            catch (Exception ex)
            {
                throw new Exception("取公佈欄資料發生意外錯誤!", ex);
            }

            return QueryResult;
        }

        public API.Models.ActionResponse<string> CreateBulletinsMessage(DB.TWSELLERPORTALDB.Models.Seller_BulletinsMessage CreateModel)
        {
            TWNewEgg.DB.TWSellerPortalDBContext spdb = new TWSellerPortalDBContext();
            API.Models.ActionResponse<string> QueryResult = new ActionResponse<string>();

            try
            {
                if (string.IsNullOrEmpty(CreateModel.FromSystem))
                {
                    QueryResult.IsSuccess = false;
                    QueryResult.Code = (int)ResponseCode.Error;
                    QueryResult.Msg = "FromSystem 不得為空";

                    return QueryResult;
                }
                else
                {
                    CreateModel.FromSystem = CreateModel.FromSystem.ToLower();

                    spdb.Seller_BulletinsMessage.Add(CreateModel);
                    spdb.SaveChanges();

                    QueryResult.IsSuccess = true;
                    QueryResult.Code = (int)ResponseCode.Success;
                }
            }
            catch (DbEntityValidationException ex)
            {
                var entityError = ex.EntityValidationErrors.SelectMany(x => x.ValidationErrors).Select(x => x.ErrorMessage);
                var getFullMessage = string.Join("; ", entityError);
                QueryResult.Finish(false, (int)ResponseCode.Error, string.Concat("errors are: ", getFullMessage), null);
            }
            catch (Exception ex)
            {
                throw new Exception("建立公佈欄發生意外錯誤!", ex);
            }

            return QueryResult;
        }

        public API.Models.ActionResponse<string> UpdateBulletinsMessage(DB.TWSELLERPORTALDB.Models.Seller_BulletinsMessage UpdateModel)
        {
            TWNewEgg.DB.TWSellerPortalDBContext spdb = new TWSellerPortalDBContext();
            API.Models.ActionResponse<string> QueryResult = new ActionResponse<string>();

            try
            {
                var result = spdb.Seller_BulletinsMessage.Where(x => x.ID == UpdateModel.ID && x.FromSystem == UpdateModel.FromSystem).FirstOrDefault();

                if (result.Updated > UpdateModel.Updated)
                {
                    QueryResult.IsSuccess = false;
                    QueryResult.Code = (int)ResponseCode.Error;
                    QueryResult.Msg = "資料已經有他人編輯過，請重新整理畫面取得最新資料!";
                }
                else
                {
                    UpdateModel.Updated = result.Updated + 1;

                    result.MessageContent = UpdateModel.MessageContent;
                    result.Updated = UpdateModel.Updated;
                    result.UpdateDate = DateTime.Now;
                    result.UpdateUser = UpdateModel.UpdateUser; 

                    spdb.Entry(result).Property(x => x.MessageContent).IsModified = true;
                    spdb.Entry(result).Property(x => x.Updated).IsModified = true;
                    spdb.Entry(result).Property(x => x.UpdateDate).IsModified = true;
                    spdb.Entry(result).Property(x => x.UpdateUser).IsModified = true;
                    spdb.SaveChanges();

                    QueryResult.IsSuccess = true;
                    QueryResult.Code = (int)ResponseCode.Success;
                    QueryResult.Msg = "更新成功!";
                }
            }
            catch (DbEntityValidationException ex)
            {
                var entityError = ex.EntityValidationErrors.SelectMany(x => x.ValidationErrors).Select(x => x.ErrorMessage);
                var getFullMessage = string.Join("; ", entityError);
                QueryResult.Finish(false, (int)ResponseCode.Error, string.Concat("errors are: ", getFullMessage), null);
            }
            catch (Exception ex)
            {
                throw new Exception("更新公佈欄發生意外錯誤!", ex);
            }

            return QueryResult;
        }

        public API.Models.ActionResponse<string> EnableBulletinsMessage(DB.TWSELLERPORTALDB.Models.Seller_BulletinsMessage EnableModel)
        {
            TWNewEgg.DB.TWSellerPortalDBContext spdb = new TWSellerPortalDBContext();
            API.Models.ActionResponse<string> QueryResult = new ActionResponse<string>();

            try
            {
                var result = spdb.Seller_BulletinsMessage.Where(x => x.FromSystem == EnableModel.FromSystem).ToList();

                foreach (var index in result)
                {
                    if (index.ID == EnableModel.ID)
                    {
                        index.Enable = "Y";
                        
                    }
                    else
                    {
                        index.Enable = "N";
                    }

                    index.Updated = index.Updated + 1;
                    index.UpdateUser = EnableModel.UpdateUser;
                    index.UpdateDate = DateTime.Now;
                }

                spdb.SaveChanges();

                QueryResult.IsSuccess = true;
                QueryResult.Code = (int)ResponseCode.Success;
            }
            catch (Exception ex)
            {
                throw new Exception("更新公佈欄發生意外錯誤!", ex);
            }

            return QueryResult;
        }

        public API.Models.ActionResponse<string> DisableBulletinsMessage(DB.TWSELLERPORTALDB.Models.Seller_BulletinsMessage DisableModel)
        {
            TWNewEgg.DB.TWSellerPortalDBContext spdb = new TWSellerPortalDBContext();
            API.Models.ActionResponse<string> QueryResult = new ActionResponse<string>();

            try
            {
                var result = spdb.Seller_BulletinsMessage.Where(x => x.FromSystem == DisableModel.FromSystem).FirstOrDefault();

                if (result.Updated > DisableModel.Updated)
                {

                }
                else
                {
                    result.Enable = "N";
                    result.Updated = result.Updated + 1;
                    result.UpdateDate = DateTime.Now;
                    result.UpdateUser = DisableModel.UpdateUser;

                    spdb.SaveChanges();
                }

                QueryResult.IsSuccess = true;
                QueryResult.Code = (int)ResponseCode.Success;
            }
            catch (Exception ex)
            {
                throw new Exception("更新公佈欄發生意外錯誤!", ex);
            }

            return QueryResult;
        }
    }
}
