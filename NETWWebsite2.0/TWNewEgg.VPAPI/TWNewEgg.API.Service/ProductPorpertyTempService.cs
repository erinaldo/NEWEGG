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
    public class ProductPorpertyTempService
    {
        // 記錄訊息
        private static ILog log = LogManager.GetLogger(typeof(ProductPorpertySketchService));

        public Models.ActionResponse<string> SaveProductPropertyTempClick(List<SaveProductProperty> saveProductPropertyCell, int productTempID, int UpdateUserID)
        {
            Models.ActionResponse<string> result = new Models.ActionResponse<string>();

            try
            {
                DB.TWSqlDBContext dbFront = new DB.TWSqlDBContext();

                //刪除取消的資料
                if (dbFront.ProductPropertytemp.Any(x => x.producttempID == productTempID))
                {
                    // 讀取原有的商品屬性資料
                    List<DB.TWSQLDB.Models.ProductPropertytemp> productPropertytempCell = dbFront.ProductPropertytemp.Where(x => x.producttempID == productTempID).ToList();

                    foreach (DB.TWSQLDB.Models.ProductPropertytemp productPropertytemp in productPropertytempCell)
                    {
                        dbFront.Entry(productPropertytemp).State = EntityState.Deleted;
                    }
                }

                if (saveProductPropertyCell.Count > 0)
                {
                    foreach (SaveProductProperty saveProductProperty in saveProductPropertyCell)
                    {
                        DB.TWSQLDB.Models.ProductPropertytemp dbInfo = new DB.TWSQLDB.Models.ProductPropertytemp();

                        dbInfo = SaveProductPropertyTemp(saveProductProperty, productTempID);
                        dbInfo.CreateUser = UpdateUserID.ToString();
                        dbInfo.UpdateUser = UpdateUserID.ToString();

                        //如果沒有null的valueID，seller portal建一筆資料進ItemPropertyValue
                        if (dbInfo.ProductValueID == 0)
                        {
                            CategoryPropertyService cps = new CategoryPropertyService();
                            dbInfo.ProductValueID = cps.AddItemPropertyValue(dbInfo.PropertyNameID);
                        }

                        dbFront.ProductPropertytemp.Add(dbInfo);
                    }
                }

                dbFront.SaveChanges();

                result.IsSuccess = true;
                result.Code = (int)ResponseCode.Success;
                result.Body = "Save changed ProductProperty table data success!";
            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.Code = (int)ResponseCode.Error;
                result.Body = null;
                log.Error(string.Format("ErrorMessage = {0}; StackTrace = {1}.", ex.Message, ex.StackTrace));
            }

            return result;
        }

        public DB.TWSQLDB.Models.ProductPropertytemp SaveProductPropertyTemp(SaveProductProperty data, int productTempID)
        {
            DB.TWSQLDB.Models.ProductPropertytemp Info = new DB.TWSQLDB.Models.ProductPropertytemp();

            DateTime dt = DateTime.UtcNow.AddHours(8);
            dt.GetDateTimeFormats('r')[0].ToString();
            Convert.ToDateTime(dt);

            try
            {
                DB.TWSqlDBContext dbFront = new DB.TWSqlDBContext();
                Info = dbFront.ProductPropertytemp.Where(x => x.producttempID == productTempID && x.PropertyNameID == data.PropertyID).SingleOrDefault();

                if (Info == null)
                {
                    Info = new DB.TWSQLDB.Models.ProductPropertytemp();
                    Info.producttempID = productTempID;
                    Info.Show = 0;
                    Info.Label = 0;
                    Info.CreateUser = data.UpdateUser;
                    Info.CreateDate = dt;
                }

                if (!string.IsNullOrWhiteSpace(data.InputValue))
                {
                    Info.ProductValueID = dbFront.ItemPropertyValue.Where(x => x.PropertyNameID == data.PropertyID && string.IsNullOrEmpty(x.PropertyValueTW) && string.IsNullOrEmpty(x.PropertyValue)).Select(e => e.ID).FirstOrDefault();
                    Info.UserInputValueTW = data.InputValue.Trim();
                }
                else
                {
                    Info.ProductValueID = data.ValueID;
                    Info.UserInputValueTW = null;
                }
                if (Info.ProductID == null)
                {
                    int product_id = dbFront.ItemStocktemp.Where(x => x.producttempID == productTempID).Select(x => x.ProductID).SingleOrDefault();
                    Info.ProductID = product_id;
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

        public Models.ActionResponse<List<PropertyResult>> GetProperty(int categoryID)
        {
            Models.ActionResponse<List<PropertyResult>> massage = new Models.ActionResponse<List<PropertyResult>>();
            List<PropertyResult> result = new List<PropertyResult>();
            List<PropertyName> nameList = new List<PropertyName>();
            List<PropertyValue> valueList = new List<PropertyValue>();


            if (categoryID <= 0)
            {
                massage.IsSuccess = false;
                massage.Code = (int)ResponseCode.Error;
                massage.Msg = "categoryID不可小於等於0，請洽系統工程師！";
                massage.Body = null;

                return massage;
            }

            try
            {
                DB.TWSqlDBContext dbFront = new DB.TWSqlDBContext();
                nameList = (from p in dbFront.ItemPropertyGroup
                            join i in dbFront.ItemPropertyName on p.ID equals i.GroupID
                            select new PropertyName
                            {
                                CategoryID = p.CategoryID,
                                GroupID = p.ID,
                                PropertyID = i.ID,
                                Name = string.IsNullOrEmpty(i.PropertyNameTW) ? i.PropertyName : i.PropertyNameTW
                            }).Where(x => x.CategoryID == categoryID).AsQueryable().ToList();


                foreach (PropertyName name in nameList)
                {
                    PropertyResult resultInfo = new PropertyResult();

                    valueList = (from j in dbFront.ItemPropertyValue
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
                log.Error(massage.Msg);

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

        public Models.ActionResponse<List<SaveProductProperty>> GetProductPropertyTemp(int productTempID)
        {
            //Models.ActionResponse<List<GetProductProperty>> massage = new Models.ActionResponse<List<GetProductProperty>>();
            //List<GetProductProperty> result = new List<GetProductProperty>();

            Models.ActionResponse<List<SaveProductProperty>> massage = new Models.ActionResponse<List<SaveProductProperty>>();
            List<SaveProductProperty> result = new List<SaveProductProperty>();

            if (productTempID <= 0)
            {
                massage.IsSuccess = false;
                massage.Code = (int)ResponseCode.Error;
                massage.Msg = "ProductID不可小於等於0，請洽系統工程師！";
                massage.Body = null;

                return massage;
            }

            try
            {
                DB.TWSqlDBContext dbFront = new DB.TWSqlDBContext();
                var propertyList = (from p in dbFront.ProductPropertytemp
                                    where p.producttempID == productTempID
                                    //select new GetProductProperty
                                    select new SaveProductProperty
                                    {
                                        //ProductID = p.ProductID,
                                        GroupID = p.GroupID,
                                        PropertyID = p.PropertyNameID,
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
                log.Error(massage.Msg);

                return massage;
            }

            if (result == null)
                massage.Msg = "無此productID = " + productTempID + "相關property資料!!";
            else
                massage.Msg = result.Count() + "筆ProductProperty資料!!";
            massage.Code = (int)ResponseCode.Success;
            massage.IsSuccess = true;
            massage.Body = result;

            return massage;
        }
    }
}
