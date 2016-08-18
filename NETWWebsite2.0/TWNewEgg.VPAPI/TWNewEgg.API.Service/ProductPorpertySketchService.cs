using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TWNewEgg.API.Models;
using TWNewEgg.DB;
using log4net;
using log4net.Config;
using System.Transactions;
using System.Data;

namespace TWNewEgg.API.Service
{
    public class ProductPorpertySketchService
    {
        private DB.TWSqlDBContext dbSearch = new DB.TWSqlDBContext();
        private DB.TWSqlDBContext dbSave = new DB.TWSqlDBContext();
        private DB.TWSqlDBContext dbDelete = new DB.TWSqlDBContext();
        private DB.TWSqlDBContext dbInsertValue = new DB.TWSqlDBContext();

        // 記錄訊息
        private static ILog log = LogManager.GetLogger(typeof(ProductPorpertySketchService));

        public Models.ActionResponse<List<PropertyResult>> GetProperty(int CategoryID)
        {
            Models.ActionResponse<List<PropertyResult>> massage = new Models.ActionResponse<List<PropertyResult>>();
            List<PropertyResult> result = new List<PropertyResult>();
            List<PropertyName> nameList = new List<PropertyName>();
            List<PropertyValue> valueList = new List<PropertyValue>();


            if (CategoryID <= 0)
            {
                massage.IsSuccess = false;
                massage.Code = (int)ResponseCode.Error;
                massage.Msg = "categoryID不可小於等於0，請洽系統工程師！";
                massage.Body = null;

                return massage;
            }

            try
            {
                nameList = (from p in dbSearch.ItemPropertyGroup
                            join i in dbSearch.ItemPropertyName on p.ID equals i.GroupID
                            select new PropertyName
                            {
                                CategoryID = p.CategoryID,
                                GroupID = p.ID,
                                PropertyID = i.ID,
                                Name = string.IsNullOrEmpty(i.PropertyNameTW) ? i.PropertyName : i.PropertyNameTW
                            }).Where(x => x.CategoryID == CategoryID).AsQueryable().ToList();

                //var propertyList = (from p in dbSearch.ItemPropertyGroup
                //                    join i in dbSearch.ItemPropertyName on p.ID equals i.GroupID
                //                    select new PropertyResult
                //                    {
                //                        CategoryID = p.CategoryID,
                //                        GroupID = p.ID,
                //                        PropertyID = i.ID,
                //                        PropertyName = string.IsNullOrEmpty(i.PropertyNameTW) ? i.PropertyName : i.PropertyNameTW//,
                //                        ValueInfo = (from j in dbSearch.ItemPropertyValue
                //                                     where j.PropertyNameID == i.ID
                //                                     select new PropertyValue
                //                                     {
                //                                         PropertyID = j.PropertyNameID,
                //                                         ValueID = j.ID,
                //                                         Value = string.IsNullOrEmpty(j.PropertyValueTW) ? j.PropertyValue : j.PropertyValueTW
                //                                     }).AsQueryable()
                //                    }).Where(x => x.CategoryID == CategoryID).AsQueryable();


                foreach (PropertyName name in nameList)
                {
                    PropertyResult resultInfo = new PropertyResult();

                    valueList = (from j in dbSearch.ItemPropertyValue
                                 where j.PropertyNameID == name.PropertyID
                                 select new PropertyValue
                                 {
                                     PropertyID = j.PropertyNameID,
                                     ValueID = j.ID,
                                     Value = string.IsNullOrEmpty(j.PropertyValueTW) ? j.PropertyValue : j.PropertyValueTW
                                 }).AsQueryable().ToList();

                    resultInfo.CategoryID = name.CategoryID;
                    resultInfo.GroupID = name.GroupID;
                    resultInfo.PropertyID = name.PropertyID;
                    resultInfo.PropertyName = name.Name;
                    resultInfo.ValueInfo = valueList;

                    result.Add(resultInfo);
                }

            }
            catch (Exception ex)
            {
                massage.IsSuccess = false;
                massage.Code = (int)ResponseCode.Error;
                massage.Msg = "例外發生: " + ex.Message;
                massage.Body = null;
                log.Error(string.Format("取得 Property 資料失敗(expection); ErrorMessage = {0}; StackTrack = {2}.",
                    GetExceptionMessage(ex),
                    ex.StackTrace));

                return massage;
            }

            if (result == null)
                massage.Msg = "無此category的相關property資料，請洽系統管理員建立資料!!";
            else
                massage.Msg = result.Count() + "筆Property資料!!";
            massage.Code = (int)ResponseCode.Success;
            massage.IsSuccess = true;
            massage.Body = result;

            return massage;
        }

        public Models.ActionResponse<string> SaveProductPropertyClick(List<SaveProductProperty> proInfo, int intProductID, int UpdateUserID)
        {
            Models.ActionResponse<string> massage = new Models.ActionResponse<string>();

            if (proInfo != null)
            {
                try
                {
                    using (TransactionScope scope = new TransactionScope())
                    {
                        //刪除取消的資料
                        if (dbSearch.ProductPropertySketch.Where(x => x.ItemSketchID == intProductID).Any())
                        {
                            List<DB.TWSQLDB.Models.ProductPropertySketch> oldInfo = new List<DB.TWSQLDB.Models.ProductPropertySketch>();
                            List<DB.TWSQLDB.Models.ProductPropertySketch> deleteInfo = new List<DB.TWSQLDB.Models.ProductPropertySketch>();
                            oldInfo = dbDelete.ProductPropertySketch.Where(x => x.ItemSketchID == intProductID).AsQueryable().ToList();

                            deleteInfo = (from old in oldInfo
                                          where !(from pro in proInfo
                                                  select pro.PropertyID)
                                                  .Contains(old.PropertyNameID.Value)
                                          select old).AsQueryable().ToList();
                            foreach (DB.TWSQLDB.Models.ProductPropertySketch data in deleteInfo)
                            {
                                dbSave.Entry(data).State = EntityState.Deleted;
                            }

                            dbSave.SaveChanges();
                        }

                        foreach (SaveProductProperty data in proInfo)
                        {
                            DB.TWSQLDB.Models.ProductPropertySketch dbInfo = new DB.TWSQLDB.Models.ProductPropertySketch();
                            //List<DB.TWSQLDB.Models.ProductProperty> deleteInfo = new List<DB.TWSQLDB.Models.ProductProperty>();
                            DB.TWSQLDB.Models.ProductPropertySketch deleteInfo = new DB.TWSQLDB.Models.ProductPropertySketch();

                            dbInfo = SaveProductProperty(data, intProductID);
                            dbInfo.CreateUser = UpdateUserID.ToString();
                            dbInfo.UpdateUser = UpdateUserID.ToString();
                            //如果沒有null的valueID，seller portal建一筆資料進ItemPropertyValue
                            if (dbInfo.ProductValueID == 0)
                            {
                                dbInfo.ProductValueID = AddItemPropertyValue(dbInfo.PropertyNameID.Value);
                            }

                            //用抓到的資料再檢查一次是否已經存在此PropertyID的資料，存在即刪除
                            if (dbSearch.ProductPropertySketch.Where(x => x.ItemSketchID == intProductID && x.PropertyNameID == data.PropertyID).Any())
                            {
                                deleteInfo = dbDelete.ProductPropertySketch.Where(x => x.ItemSketchID == intProductID && x.PropertyNameID == data.PropertyID).Single();
                                dbSave.Entry(deleteInfo).State = EntityState.Deleted;
                            }
                            dbSave.ProductPropertySketch.Add(dbInfo);

                        }

                        dbSave.SaveChanges();
                        scope.Complete();
                    }

                    massage.IsSuccess = true;
                    massage.Code = (int)ResponseCode.Success;
                    massage.Body = "Save changed ProductProperty table data success!";
                    massage.Msg = "Property資料更新成功，已儲存!";

                }
                catch (Exception ex)
                {
                    massage.IsSuccess = false;
                    massage.Code = (int)ResponseCode.Error;
                    massage.Body = null;
                    massage.Msg = "例外發生: " + ex.Message;
                    log.Error(string.Format("Property 資料更新失敗(expection); ErrorMessage = {0}; StackTrack = {2}.",
                    GetExceptionMessage(ex),
                    ex.StackTrace));
                }
            }
            else
            {
                massage.IsSuccess = true;
                massage.Code = (int)ResponseCode.Success;
                massage.Body = "No data save!!";
                massage.Msg = "Property無資料更新!";
            }
            return massage;
        }

        public DB.TWSQLDB.Models.ProductPropertySketch SaveProductProperty(SaveProductProperty data, int intProductID)
        {
            DB.TWSQLDB.Models.ProductPropertySketch Info = new DB.TWSQLDB.Models.ProductPropertySketch();

            DateTime dt = DateTime.UtcNow.AddHours(8);
            dt.GetDateTimeFormats('r')[0].ToString();
            Convert.ToDateTime(dt);

            try
            {
                Info = dbSearch.ProductPropertySketch.Where(x => x.ItemSketchID == intProductID && x.PropertyNameID == data.PropertyID).SingleOrDefault();

                if (Info == null)
                {
                    Info = new DB.TWSQLDB.Models.ProductPropertySketch();
                    Info.ItemSketchID = intProductID;
                    Info.Show = 0;
                    Info.Label = 0;
                    Info.CreateUser = data.UpdateUser;
                    Info.CreateDate = dt;
                }

                if (!string.IsNullOrWhiteSpace(data.InputValue))
                {
                    Info.ProductValueID = dbSearch.ItemPropertyValue.Where(x => x.PropertyNameID == data.PropertyID && string.IsNullOrEmpty(x.PropertyValueTW) && string.IsNullOrEmpty(x.PropertyValue)).Select(e => e.ID).FirstOrDefault();
                    Info.UserInputValueTW = data.InputValue.Trim();
                }
                else
                {
                    Info.ProductValueID = data.ValueID;
                    Info.UserInputValueTW = null;
                }

                Info.GroupID = data.GroupID;
                Info.PropertyNameID = data.PropertyID;
                Info.UpdateUser = data.UpdateUser;
                Info.UpdateDate = dt;
            }
            catch (Exception ex)
            {
                log.Error(ex.Message);
            }
            return Info;
        }

        public int AddItemPropertyValue(int intPropertyID)
        {
            DB.TWSQLDB.Models.ItemPropertyValue Info = new DB.TWSQLDB.Models.ItemPropertyValue();
            int intValueID = 0;

            DateTime dt = DateTime.UtcNow.AddHours(8);
            dt.GetDateTimeFormats('r')[0].ToString();
            Convert.ToDateTime(dt);

            try
            {
                Info.PropertyNameID = intPropertyID;
                Info.PropertyValue = null;
                Info.PropertyValueTW = null;
                Info.PropertyCode = "SellerPortal";
                Info.CreateUser = "SellerPortal";
                Info.CreateDate = dt;
                Info.UpdateUser = "SellerPortal";
                Info.UpdateDate = dt;

                dbInsertValue.ItemPropertyValue.Add(Info);
                dbInsertValue.SaveChanges();

                intValueID = dbSearch.ItemPropertyValue.Where(x => x.PropertyNameID == intPropertyID && string.IsNullOrEmpty(x.PropertyValueTW) && string.IsNullOrEmpty(x.PropertyValue)).Select(e => e.ID).FirstOrDefault();
            }
            catch (Exception ex)
            {
                log.Error(ex.Message);
            }
            return intValueID;
        }

        //public Models.ActionResponse<List<GetProductProperty>> GetProductProperty(int ProductID)
        public Models.ActionResponse<List<SaveProductProperty>> GetProductProperty(int ProductID)
        {
            //Models.ActionResponse<List<GetProductProperty>> massage = new Models.ActionResponse<List<GetProductProperty>>();
            //List<GetProductProperty> result = new List<GetProductProperty>();

            Models.ActionResponse<List<SaveProductProperty>> massage = new Models.ActionResponse<List<SaveProductProperty>>();
            List<SaveProductProperty> result = new List<SaveProductProperty>();

            if (ProductID <= 0)
            {
                massage.IsSuccess = false;
                massage.Code = (int)ResponseCode.Error;
                massage.Msg = "ProductID不可小於等於0，請洽系統工程師！";
                massage.Body = null;

                return massage;
            }

            try
            {
                var propertyList = (from p in dbSearch.ProductPropertySketch
                                    where p.ItemSketchID == ProductID
                                    //select new GetProductProperty
                                    select new SaveProductProperty
                                    {
                                        //ProductID = p.ProductID,
                                        GroupID = p.GroupID.Value,
                                        PropertyID = p.PropertyNameID.Value,
                                        ValueID = p.ProductValueID,
                                        InputValue = p.UserInputValueTW,
                                        UpdateUser = p.UpdateUser
                                    }).AsQueryable();

                result = propertyList.ToList();
            }
            catch (Exception ex)
            {
                massage.IsSuccess = false;
                massage.Code = (int)ResponseCode.Error;
                massage.Msg = "例外發生: " + ex.Message;
                massage.Body = null;
                log.Error(string.Format("取得 Property 資料更新失敗(expection); ErrorMessage = {0}; StackTrack = {2}.",
                    GetExceptionMessage(ex),
                    ex.StackTrace));

                return massage;
            }

            if (result == null)
                massage.Msg = "無此productID = " + ProductID + "相關property資料!!";
            else
                massage.Msg = result.Count() + "筆ProductProperty資料!!";
            massage.Code = (int)ResponseCode.Success;
            massage.IsSuccess = true;
            massage.Body = result;

            return massage;
        }

        /// <summary>
        /// 取得 Exception 錯誤訊息
        /// </summary>
        /// <param name="ex">Exception 內容</param>
        /// <returns>Exception 錯誤訊息</returns>
        private string GetExceptionMessage(System.Exception ex)
        {
            string errorMessage = string.Empty;

            if (ex.Message.IndexOf("See the inner exception for details.") != -1)
            {
                errorMessage = ex.InnerException.Message;

                if (errorMessage.IndexOf("See the inner exception for details.") != -1)
                {
                    errorMessage = GetExceptionMessage(ex.InnerException);
                }
            }
            else
            {
                errorMessage = ex.Message;
            }

            return errorMessage;
        }
    }
}
