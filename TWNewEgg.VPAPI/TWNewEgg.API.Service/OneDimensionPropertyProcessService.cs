using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TWNewEgg.API.Models;
using TWNewEgg.DB;
using System.Transactions;

namespace TWNewEgg.API.Service
{
    public class OneDimensionPropertyProcessService
    {
        private static ILog log = LogManager.GetLogger(typeof(OneDimensionPropertyProcessService));
        log4net.ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.FullName);

        public ActionResponse<string> OneDimensionExamine(List<int> toExamineId, int userid, int sellerid)
        {
            ActionResponse<string> result = new ActionResponse<string>();
            TWNewEgg.DB.TWSqlDBContext dbFront = new TWSqlDBContext();
            List<TWNewEgg.DB.TWSQLDB.Models.ItemSketch> _list_ItemSketch = new List<DB.TWSQLDB.Models.ItemSketch>();
            //利用要送審資料的 Id 從資料庫拉回對應的送審資料
            _list_ItemSketch = dbFront.ItemSketch.Where(p => toExamineId.Contains(p.ID)).ToList();
            #region 檢查送審資料是否完整
            //要送審之前先檢查資料是否完整
            ActionResponse<string> checkResult = checkModelElement(_list_ItemSketch, 1);
            //有錯誤則回傳, 不送審
            if (checkResult.IsSuccess == false)
            {
                result.IsSuccess = false;
                result.Msg = checkResult.Msg;
                return result;
            }
            #endregion
            List<TWNewEgg.DB.TWSQLDB.Models.ItemSketchProperty> _list_ItemSketchProperty = new List<DB.TWSQLDB.Models.ItemSketchProperty>();
            List<TWNewEgg.DB.TWSQLDB.Models.ItemSketchGroup> _list_ItemSketchGroup = new List<DB.TWSQLDB.Models.ItemSketchGroup>();
            List<TWNewEgg.DB.TWSQLDB.Models.ItemCategorySketch> _listItemCategorySketch = dbFront.ItemCategorySketch.Where(p => toExamineId.Contains(p.ItemSketchID)).ToList();
            //抓取 order = 1 的屬性 PropertyID
            _list_ItemSketchGroup = dbFront.ItemSketchGroup.Where(p => toExamineId.Contains(p.ItemSketchID) && p.Order == 1).ToList();
            //抓取對應 itemsketch_id 下的 ItemSketchProperty 資料(第一順位, 可售數量)
            _list_ItemSketchProperty = dbFront.ItemSketchProperty.Where(p => toExamineId.Contains(p.ItemSketchID)).ToList();
            //using (TransactionScope scope = new TransactionScope())
            //{
            foreach (int itemSketch_id in toExamineId)
            {
                TWNewEgg.API.Models.BatchExamineModel batchExamineModel = new BatchExamineModel();
                //抓取第一順位的 PropertyID
                int? itemSketch_PropertyID = _list_ItemSketchGroup.Where(p => p.ItemSketchID == itemSketch_id && p.Order == 1).Select(p => p.PropertyID).FirstOrDefault();
                //抓取第一順位所選擇的屬性, 例如顏色
                var MydefineColorAndSelectColor = _list_ItemSketchProperty.Where(p => p.PropertyID == itemSketch_PropertyID && p.ItemSketchID == itemSketch_id).ToList();
                #region 組合第二順位送審要用的 Property Model
                foreach (var item in MydefineColorAndSelectColor)
                {
                    TWNewEgg.API.Models.colorSizeModel _colorSizeModel = new Models.colorSizeModel();
                    //第一順位的自定義
                    _colorSizeModel.inputValue = item.InputValue;
                    //第一順位的 value id
                    _colorSizeModel.colorValueId = item.GroupValueID;
                    //利用第一順位的 GroupValueID 和 itemSketch_id 抓回對應的 ItemSketchProperty 屬性資料(第二順位的資料)
                    var sizeProperty = _list_ItemSketchProperty.Where(p => p.ItemSketchID == itemSketch_id && p.GroupValueID == item.GroupValueID).ToList();
                    List<TWNewEgg.API.Models.propertyModel> listpropertyModel = new List<propertyModel>();
                    foreach (var sizeitem in sizeProperty)
                    {
                        // GroupValueID = ValueID 就是第一順位
                        if (sizeitem.GroupValueID == sizeitem.ValueID)
                        {
                            TWNewEgg.API.Models.propertyModel propertyModel = new propertyModel();
                            //因為這是一維屬性的商品, 不會有第二順位, 所以 proValueId 直接給 0
                            propertyModel.proValueId = 0;
                            propertyModel.proQty = sizeitem.Qty;
                            listpropertyModel.Add(propertyModel);
                        }
                        else
                        {
                            TWNewEgg.API.Models.propertyModel propertyModel = new propertyModel();
                            propertyModel.proValueId = sizeitem.ValueID;
                            propertyModel.proQty = sizeitem.Qty;
                            listpropertyModel.Add(propertyModel);
                            logger.Info("一維屬性商品組合的 Model 錯誤(第二順位屬性有值)");
                        }
                        _colorSizeModel.listProperty = listpropertyModel;
                        batchExamineModel.colorsizeProperty.Add(_colorSizeModel);
                    }
                }
                #endregion
                #region join 要送審的資料
                ActionResponse<List<TWNewEgg.API.Models.propertyJoinModel>> joinResult = this.join_ItemPropertyValue_ItemPropertyName_Result(batchExamineModel);
                if (joinResult.IsSuccess == false)
                {
                    result.IsSuccess = false;
                    result.Msg = joinResult.Msg;
                    return result;
                }
                #endregion
                //從草稿區利用 itemSketchid 取回要送審的一般資料
                TWNewEgg.DB.TWSQLDB.Models.ItemSketch itemsketch = _list_ItemSketch.Where(p => p.ID == itemSketch_id).FirstOrDefault();
                List<int> subCategoryid = new List<int>();
                subCategoryid = _listItemCategorySketch.Where(p => p.ItemSketchID == itemSketch_id).Select(p => p.CategoryID).ToList();
                if (subCategoryid.Count == 0 || subCategoryid == null)
                {
                }
                else
                {
                    if (subCategoryid.Count == 1)
                    {
                        batchExamineModel.ItemCategory.SubCategoryID_1_Layer2 = subCategoryid[0];
                    }
                    else
                    {
                        batchExamineModel.ItemCategory.SubCategoryID_1_Layer2 = subCategoryid[0];
                        batchExamineModel.ItemCategory.SubCategoryID_2_Layer2 = subCategoryid[1];
                    }
                }
                //開始一維屬性商品的送審
                ActionResponse<string> OneDimensionExamineResult = this.OneDimensionExamineInsertItemGroupItemGroupProperty(itemsketch, batchExamineModel, userid, sellerid, joinResult);
                if (OneDimensionExamineResult.IsSuccess == false)
                {
                    result.IsSuccess = false;
                    result.Msg = OneDimensionExamineResult.Msg;
                    break;
                }
                else
                {
                    try
                    {
                        //送審成功之後把狀態改成 99
                        itemsketch.Status = 99;
                        itemsketch.ItemTempGroupID = Convert.ToInt16(OneDimensionExamineResult.Body);
                        dbFront.SaveChanges();
                        result.IsSuccess = true;
                        result.Msg = OneDimensionExamineResult.Msg;
                    }
                    catch (Exception error)
                    {
                        result.IsSuccess = false;
                        result.Msg = "送審失敗";
                        logger.Error("[Message]: " + error.Message + " ;[StackTrace]: " + error.StackTrace + " ;[InnerException]: " + this.ExceptionMessage(error));
                    }
                    //發生 Exception 錯誤, 則不繼續執行
                    if (result.IsSuccess == false)
                    {
                        break;
                    }
                }
            }//foreach end
                //if (result.IsSuccess == false)
                //{
                //    scope.Dispose();
                //}
                //else
                //{
                //    scope.Complete();
                //}
            //}
            
            return result;
        }
        #region 送審第一步, 把資料寫入 ItemGroup, ItemGroupProperty
        public ActionResponse<string> OneDimensionExamineInsertItemGroupItemGroupProperty(TWNewEgg.DB.TWSQLDB.Models.ItemSketch itemsketch, TWNewEgg.API.Models.BatchExamineModel batchExamineModel, int userid, int sellerid, ActionResponse<List<TWNewEgg.API.Models.propertyJoinModel>> joinResult)
        {
            ActionResponse<string> result = new ActionResponse<string>();
            try
            {
                int amount = batchExamineModel.colorsizeProperty.Count;
                //把送審資料寫入 ItemGroup 
                var itemgroup = this.ItemGroup(amount, joinResult.Body[0].ItemPropertyValue_propertyNameID, userid, sellerid);
                if (itemgroup.IsSuccess == false)
                {
                    result.IsSuccess = false;
                    result.Msg = itemgroup.Msg;
                }
                else
                {
                    //取得 ItemGroup 的 id
                    int groupid = itemgroup.Body;
                    int propertyId = batchExamineModel.colorsizeProperty[0].colorValueId;
                    var itemGroupProperty = this.ItemGroupProperty(joinResult.Body, groupid, userid, propertyId);
                    if (itemGroupProperty.IsSuccess == false)
                    {
                        result.IsSuccess = false;
                        result.Msg = itemgroup.Msg;
                    }
                    else
                    {
                        ActionResponse<string> OneDimensionExamineInsertFinal_Result = OneDimensionExamineInsertFinal(batchExamineModel, itemsketch, sellerid, userid, joinResult.Body, groupid);
                        if (OneDimensionExamineInsertFinal_Result.IsSuccess == false)
                        {
                            result.IsSuccess = false;
                            result.Msg = OneDimensionExamineInsertFinal_Result.Msg;
                        }
                        else
                        {
                            result.IsSuccess = true;
                            result.Body = groupid.ToString();
                            result.Msg = "送審成功";
                        }
                    }
                }
            }
            catch (Exception error)
            {
                result.IsSuccess = false;
                result.Msg = "資料處理錯誤";
                logger.Error("[Message]: " + error.Message + " ;[StackTrace]: " + error.StackTrace);
            }
            return result;
        }
        #endregion
        #region 把資料寫入 productTemp, itemtemp, Itemstocktemp, ItemCategoryTemp, productPropertytemp, ItemGroupDetailProperty, 圖片處理
        public ActionResponse<string> OneDimensionExamineInsertFinal(TWNewEgg.API.Models.BatchExamineModel batchExamineModel, TWNewEgg.DB.TWSQLDB.Models.ItemSketch itemSketchModel, int sellerid, int userid, List<TWNewEgg.API.Models.propertyJoinModel> jModel, int groupId)
        {
            ActionResponse<string> result = new ActionResponse<string>();
            //初始化回傳值
            result.IsSuccess = true;
            result.Msg = string.Empty;
            int itemTempid = 0;
            string inputValue = string.Empty;
            int colorValueId = 0;
            int mapTempint = 1;
            //對每個第一順位(顏色)開始寫入相關的資料
            foreach (var itemModel in batchExamineModel.colorsizeProperty)
            {
                inputValue = string.Empty;
                inputValue = itemModel.inputValue;
                //MasterPropertyId
                colorValueId = itemModel.colorValueId;
                //對每個第二順位(尺寸)開始寫入相關資料, 沒有 Valueid 但是有 proQty
                foreach (var itemProperty in itemModel.listProperty)
                {
                    #region productTemp
                    ActionResponse<int> productTemp = this.productTempInser(itemSketchModel, sellerid, userid, itemModel, jModel);
                    if (productTemp.IsSuccess == false)
                    {
                        result.IsSuccess = false;
                        result.Msg = productTemp.Msg;
                        break;
                    }
                    int productTempid = productTemp.Body;
                    //判斷 trigger 是否有將 productTempId 回寫回來
                    if (productTempid == 0 || productTempid == null)
                    {
                        result.IsSuccess = false;
                        result.Msg = "資料處理回寫錯誤";
                        logger.Error("sellerid: " + sellerid + "; userid: " + userid + "productTempid 沒有回寫回來");
                        break;
                    }
                    #endregion
                    #region itemTemp
                    var itemtemp = this.itemtempInsert(itemSketchModel, productTempid, userid);
                    if (itemtemp.IsSuccess == false)
                    {
                        result.IsSuccess = false;
                        result.Msg = itemtemp.Msg;
                        break;
                    }
                    itemTempid = itemtemp.Body;
                    //判斷 trigger 是否有將 itemTempid 回寫回來
                    if (itemTempid == 0 || itemTempid == null)
                    {
                        result.IsSuccess = false;
                        result.Msg = "資料處理回寫錯誤";
                        logger.Error("sellerid: " + sellerid + "; userid: " + userid + "itemTempid 沒有回寫回來");
                        break;
                    }
                    #endregion
                    #region Itemstocktemp
                    var Itemstocktemp = this.ItemstocktempInsert(itemProperty.proQty, productTempid, userid, itemSketchModel.InventorySafeQty);
                    if (Itemstocktemp.IsSuccess == false)
                    {
                        result.IsSuccess = false;
                        result.Msg = Itemstocktemp.Msg;
                        break;
                    }
                    #endregion
                    #region ItemCategoryTemp
                    List<int> categoryid = new List<int>();
                    //判斷是否有傳入誇分類 1
                    if (batchExamineModel.ItemCategory.SubCategoryID_1_Layer2 != 0 && batchExamineModel.ItemCategory.SubCategoryID_1_Layer2 != null)
                    {
                        categoryid.Add(batchExamineModel.ItemCategory.SubCategoryID_1_Layer2.GetValueOrDefault());
                    }
                    //判斷是否有傳入誇分類 2
                    if (batchExamineModel.ItemCategory.SubCategoryID_2_Layer2 != 0 && batchExamineModel.ItemCategory.SubCategoryID_2_Layer2 != null)
                    {
                        categoryid.Add(batchExamineModel.ItemCategory.SubCategoryID_2_Layer2.GetValueOrDefault());
                    }
                    //有選填誇分類，subCategoryId.Count 不是 1 就是 2，是 0 的話就沒必要建立誇分類
                    if (categoryid.Count != 0)
                    {
                        var itemCategoryTemp = this.ItemCategoryTemp(itemTempid, categoryid, userid);
                        if (itemCategoryTemp.IsSuccess == false)
                        {
                            result.IsSuccess = false;
                            result.Msg = itemCategoryTemp.Msg;
                            break;
                        }
                    }
                    #endregion
                    #region productPropertytemp
                    var productProperty_ItemPropertyValue_ItemPropertyName = jModel.Where(p => p.ItemPropertyValue_ID == itemModel.colorValueId).ToList();
                    var productPropertytemp = this.productPropertytempInsert(productProperty_ItemPropertyValue_ItemPropertyName, sellerid, userid, inputValue, productTempid);
                    if (productPropertytemp.IsSuccess == false)
                    {
                        result.IsSuccess = false;
                        result.Msg = productPropertytemp.Msg;
                        break;
                    }
                    #endregion
                    #region ItemGroupDetailProperty
                    var itemGroupPropertyDetail = this.ItemGroupDetailProperty(groupId, itemTempid, sellerid, userid, jModel, itemModel, itemProperty);
                    if (itemGroupPropertyDetail.IsSuccess == false)
                    {
                        result.IsSuccess = false;
                        result.Msg = productPropertytemp.Msg;
                        break;
                    }
                    #endregion
                    #region 圖片處理
                    var ImageProcessResult = this.imgProcess(itemSketchModel.PicStart, itemSketchModel.PicEnd, itemSketchModel.ID, itemTempid, mapTempint);
                    if (ImageProcessResult.IsSuccess == false)
                    {
                        logger.Error("sellerid: " + sellerid + " ;[mapTempint]: " + mapTempint + "[Message]: " + ImageProcessResult.Msg);
                    }
                    #endregion
                }
                mapTempint++;
            }
            return result;
        }
        #endregion
        #region 把資料寫入 productTemp 表
        public ActionResponse<int> productTempInser(TWNewEgg.DB.TWSQLDB.Models.ItemSketch IModel, int sellerid, int userid, colorSizeModel itemModel, List<TWNewEgg.API.Models.propertyJoinModel> jModel)
        {
            ActionResponse<int> result = new ActionResponse<int>();
            TWSqlDBContext db_before = new TWSqlDBContext();
            DateTime dateTimeMillisecond = new DateTime(0001, 01, 01, 01, 01, 01, 01);
            TWNewEgg.API.Service.ImageService imgService = new ImageService();
            string timeString = dateTimeMillisecond.ToString("yyyy-MM-dd hh:mm:ss fff");
            dateTimeMillisecond = DateTime.Now;
            DB.TWSQLDB.Models.ProductTemp productTempAdd = new DB.TWSQLDB.Models.ProductTemp();
            productTempAdd.SellerProductID = IModel.SellerProductID;
            #region 填寫商品名稱

            #region 取得主項目屬性名稱

            string firstPropertyValueName = string.Empty;

            propertyJoinModel propertyJoinModel = jModel.Where(p => p.ItemPropertyValue_ID == itemModel.colorValueId).FirstOrDefault();

            if (string.IsNullOrEmpty(propertyJoinModel.ItemPropertyValue_propertyValue))
            {
                firstPropertyValueName = propertyJoinModel.ItemPropertyValue_propertyValue;
            }
            else
            {
                firstPropertyValueName = propertyJoinModel.ItemPropertyValue_propertyValueTW;
            }

            if (!string.IsNullOrEmpty(firstPropertyValueName))
            {
                firstPropertyValueName = "(" + firstPropertyValueName + ")";
            }

            #endregion 取得主項目屬性名稱

            string tempName = string.Empty;           
            if (IModel.ShowOrder == (int)TWNewEgg.DB.TWSQLDB.Models.Item.ShowOrderStatus.AddtionalItemForCart)
            {
                if (!string.IsNullOrEmpty(IModel.Name))
                {
                    if (IModel.Name.IndexOf("加購_") == 0)
                    {
                        tempName = IModel.Name.Replace("加購_", "");
                    }
                }
            }
            else
            {
                tempName = IModel.Name;
            }

            productTempAdd.Name = tempName + firstPropertyValueName;
            productTempAdd.NameTW = tempName + firstPropertyValueName;

            #endregion 填寫商品名稱
            productTempAdd.Description = IModel.Description;
            productTempAdd.DescriptionTW = IModel.Description;
            productTempAdd.SPEC = ""; //SPEC已不使用;
            productTempAdd.ManufactureID = IModel.ManufactureID.GetValueOrDefault();
            productTempAdd.Model = IModel.Model;
            productTempAdd.BarCode = IModel.BarCode;
            productTempAdd.SellerID = sellerid;
            productTempAdd.DelvType = IModel.DelvType;
            productTempAdd.PicStart = 1;
            productTempAdd.PicEnd = 1;
            productTempAdd.Cost = IModel.Cost;
            productTempAdd.Status = 1;
            productTempAdd.InvoiceType = 0;//default value
            productTempAdd.SaleType = 0;//default value
            productTempAdd.Length = IModel.Length;
            productTempAdd.Width = IModel.Width;
            productTempAdd.Height = IModel.Height;
            productTempAdd.Weight = IModel.Weight;
            productTempAdd.CreateUser = userid.ToString();
            productTempAdd.CreateDate = dateTimeMillisecond;
            productTempAdd.Updated = 0;//default value
            productTempAdd.UpdateDate = dateTimeMillisecond;
            productTempAdd.UpdateUser = userid.ToString();
            productTempAdd.TradeTax = IModel.TradeTax == null ? 0 : IModel.TradeTax;
            productTempAdd.Tax = 0;//default value
            productTempAdd.Warranty = IModel.Warranty;
            productTempAdd.UPC = IModel.UPC;
            productTempAdd.Note = IModel.Note;
            productTempAdd.IsMarket = "Y";
            productTempAdd.Is18 = IModel.Is18;
            productTempAdd.IsShipDanger = IModel.IsShipDanger;
            productTempAdd.IsChokingDanger = IModel.IsChokingDanger;
            productTempAdd.MenufacturePartNum = IModel.MenufacturePartNum;
            db_before.ProductTemp.Add(productTempAdd);
            try
            {
                db_before.SaveChanges();
                result.IsSuccess = true;
                result.Body = productTempAdd.ID;
                //result.Msg = delcType.Body.ToString();//拿 Msg 存 delcType
            }
            catch (Exception error)
            {
                result.Msg = "資料處理錯誤";
                result.IsSuccess = false;
                result.Body = 0;
                logger.Error("[Message]: " + error.Message + " ;[StackTrace]: " + error.StackTrace + "; sellerid: " + sellerid + "; userid:" + userid);
            }
            return result;
        }
        #endregion
        #region 把資料寫入 itemtemp 表
        public ActionResponse<int> itemtempInsert(TWNewEgg.DB.TWSQLDB.Models.ItemSketch IModel, int productId, int userid)
        {
            ActionResponse<int> result = new ActionResponse<int>();
            DB.TWSqlDBContext db_before = new TWSqlDBContext();
            DateTime dateTimeMillisecond = new DateTime(0001, 01, 01, 01, 01, 01, 01);
            string timeString = dateTimeMillisecond.ToString("yyyy-MM-dd hh:mm:ss fff");
            dateTimeMillisecond = DateTime.Now;

            DB.TWSQLDB.Models.ItemTemp itemtempAdd = new DB.TWSQLDB.Models.ItemTemp();
            itemtempAdd.ProduttempID = productId;
            itemtempAdd.Name = IModel.Name;
            itemtempAdd.Sdesc = IModel.Sdesc;
            itemtempAdd.DescTW = IModel.Description;
            itemtempAdd.ItemTempDesc = IModel.Description;
            itemtempAdd.SpecDetail = "";//defalut value
            itemtempAdd.Spechead = IModel.Spechead;
            itemtempAdd.SaleType = 1;//defalut value
            itemtempAdd.PayType = 0;//defalut value
            itemtempAdd.Layout = 0;//defalut value
            itemtempAdd.DelvType = IModel.DelvType.GetValueOrDefault();
            itemtempAdd.DelvData = IModel.DelvDate ?? "";
            itemtempAdd.ItemNumber = "";//defalut value
            itemtempAdd.CategoryID = IModel.CategoryID.GetValueOrDefault();
            itemtempAdd.Model = IModel.Model == null ? "" : IModel.Model;
            itemtempAdd.SellerID = IModel.SellerID.GetValueOrDefault();
            itemtempAdd.DateStart = IModel.DateStart.GetValueOrDefault();
            itemtempAdd.DateEnd = IModel.DateEnd.GetValueOrDefault();
            itemtempAdd.DateDel = IModel.DateDel.GetValueOrDefault().AddDays(1);
            itemtempAdd.Pricesgst = 0;//defalut value
            itemtempAdd.PriceCard = IModel.PriceCard.GetValueOrDefault();
            itemtempAdd.PriceCash = IModel.PriceCash.GetValueOrDefault();
            itemtempAdd.ServicePrice = 0;
            itemtempAdd.PricehpType1 = 0;//defalut value
            itemtempAdd.Pricehpinst1 = 0;//defalut value
            itemtempAdd.PricehpType2 = 0;//defalut value
            itemtempAdd.Pricehpinst2 = 0;//defalut value
            itemtempAdd.Inst0Rate = 0;//defalut value
            itemtempAdd.RedmfdbckRate = 0;//defalut value
            itemtempAdd.Coupon = "0";//defalut value
            itemtempAdd.PriceCoupon = 0;//defalut value
            itemtempAdd.PriceLocalship = IModel.PriceLocalship == null ? 0 : (int)IModel.PriceLocalship;
            itemtempAdd.PriceGlobalship = 0;//default value
            itemtempAdd.Qty = IModel.ItemQty.GetValueOrDefault();
            itemtempAdd.SafeQty = 0;
            itemtempAdd.QtyLimit = IModel.QtyLimit.GetValueOrDefault();
            itemtempAdd.LimitRule = "";//defalut value
            itemtempAdd.QtyReg = 0;//defalut value
            itemtempAdd.PhotoName = "";//defalut value
            itemtempAdd.HtmlName = "";//defalut value
            itemtempAdd.Showorder = 0;//defalut value
            itemtempAdd.Class = 1;//defalut value
            itemtempAdd.Status = 1;//defalut value
            itemtempAdd.ManufactureID = IModel.ManufactureID.GetValueOrDefault();
            itemtempAdd.StatusNote = "";//defalut value
            itemtempAdd.StatusDate = dateTimeMillisecond;
            itemtempAdd.Note = string.IsNullOrEmpty(IModel.Note) == true ? "" : IModel.Note;
            /*
            上下架狀態
            0：上架
            1：下架、未上架
            2：強制下架(無上架機會)
            3：售價異常(系統判斷下架)           
             */
            itemtempAdd.ItemStatus = 1;
            itemtempAdd.CreateDate = dateTimeMillisecond;
            itemtempAdd.CreateUser = IModel.CreateUser;
            itemtempAdd.Updated = 0;//defalut value
            itemtempAdd.UpdateUser = IModel.UpdateUser;
            //UpdateDate(更新日期)：不給值(因為有更新才給)
            //itemtemp.UpdateDate
            itemtempAdd.PicStart = 1;
            itemtempAdd.PicEnd = 1;
            //itemtempAdd.PicStart = IModel.PicStart;
            //itemtempAdd.PicEnd = IModel.PicEnd;
            itemtempAdd.MarketPrice = IModel.MarketPrice;
            itemtempAdd.WareHouseID = IModel.WarehouseID;
            itemtempAdd.ShipType = IModel.ShipType;
            itemtempAdd.Taxfee = 0;//defalut value
            itemtempAdd.ItemPackage = IModel.ItemPackage;
            itemtempAdd.IsNew = IModel.IsNew;
            itemtempAdd.GrossMargin = IModel.GrossMargin;
            //itemtemp.ApproveMan = item
            //itemtemp.ApproveDate = 
            itemtempAdd.SubmitMan = userid.ToString();
            itemtempAdd.SubmitDate = dateTimeMillisecond;//不給值會出現datetime2無法轉換成datetime錯誤
            itemtempAdd.ApproveDate = null;
            db_before.ItemTemp.Add(itemtempAdd); 
            try
            {
                db_before.SaveChanges();
                result.Body = itemtempAdd.ID;
                result.IsSuccess = true;
                result.Msg = "success";
                result.Code = (int)ResponseCode.Success;
            }
            catch (Exception error)
            {
                logger.Error("[Message]: " + error.Message + " ;[StackTrace]: " + error.StackTrace + " ;userid: " + userid);
                result.IsSuccess = false;
                result.Msg = "資料錯誤";
                result.Code = (int)ResponseCode.Error;
            }
            return result;
        }
        #endregion
        #region 把資料寫入 ItemstocktempInsert
        public ActionResponse<string> ItemstocktempInsert(int proQty, int productTempId, int userid, int? InventorySafeQty)
        {
            ActionResponse<string> result = new ActionResponse<string>();
            DB.TWSqlDBContext db_before = new TWSqlDBContext();
            DateTime dateTimeMillisecond = new DateTime(0001, 01, 01, 01, 01, 01, 01);
            string timeString = dateTimeMillisecond.ToString("yyyy-MM-dd hh:mm:ss fff");
            dateTimeMillisecond = DateTime.Now;
            DB.TWSQLDB.Models.ItemStocktemp _itemStocktemp = new DB.TWSQLDB.Models.ItemStocktemp();
            _itemStocktemp.producttempID = productTempId;
            _itemStocktemp.Qty = proQty;
            _itemStocktemp.QtyReg = 0;
            _itemStocktemp.SafeQty = InventorySafeQty.GetValueOrDefault();
            _itemStocktemp.Fdbcklmt = 0;
            _itemStocktemp.CreateUser = userid.ToString();
            _itemStocktemp.Updated = 0;
            _itemStocktemp.CreateDate = dateTimeMillisecond;
            _itemStocktemp.UpdateUser = userid.ToString();
            _itemStocktemp.UpdateDate = dateTimeMillisecond;
            db_before.ItemStocktemp.Add(_itemStocktemp);
            try
            {
                db_before.SaveChanges();
                result.IsSuccess = true;
                result.Code = (int)ResponseCode.Success;
                result.Msg = "success";
            }
            catch (Exception error)
            {
                logger.Error("Message: " + error.Message + " ;[StackTrace]: " + error.StackTrace);
                result.IsSuccess = false;
                result.Msg = "資料錯誤";
                result.Code = (int)ResponseCode.Error;
            }
            return result;
        }
        #endregion
        #region 把資料寫入 ItemCategoryTemp
        public ActionResponse<string> ItemCategoryTemp(int itemTempid, List<int> subCategoryId, int userid)
        {
            ActionResponse<string> result = new ActionResponse<string>();
            TWNewEgg.DB.TWSqlDBContext db_before = new TWSqlDBContext();
            foreach (int itemSubCategoryId in subCategoryId)
            {
                DB.TWSQLDB.Models.ItemCategorytemp _itemcategorytemp = new DB.TWSQLDB.Models.ItemCategorytemp();
                _itemcategorytemp.itemtempID = itemTempid;
                _itemcategorytemp.CategoryID = itemSubCategoryId;
                _itemcategorytemp.FromSystem = "1";//0: PM; 1: sellerPortal
                _itemcategorytemp.CreateUser = userid.ToString();
                _itemcategorytemp.CreateDate = DateTime.Now;
                _itemcategorytemp.UpdateDate = DateTime.Now;
                _itemcategorytemp.UpdateUser = userid.ToString();
                db_before.ItemCategorytemp.Add(_itemcategorytemp);
            }
            try
            {
                db_before.SaveChanges();
                result.IsSuccess = true;
            }
            catch (Exception error)
            {
                logger.Error("[Message]: " + error.Message + "; [StackTrace]: " + error.StackTrace);
                result.IsSuccess = false;
                result.Msg = "資料錯誤";
            }
            return result;
        }
        #endregion
        #region 把資料寫入 productPropertytempInsert
        public ActionResponse<string> productPropertytempInsert(List<TWNewEgg.API.Models.propertyJoinModel> jModel, int sellerid, int userid, string inputValue, int productTempId)
        {
            ActionResponse<string> result = new ActionResponse<string>();
            DateTime dateTimeMillisecond = new DateTime(0001, 01, 01, 01, 01, 01, 01);
            string timeString = dateTimeMillisecond.ToString("yyyy-MM-dd hh:mm:ss fff");
            dateTimeMillisecond = DateTime.Now;
            TWSqlDBContext dbFront = new TWSqlDBContext();

            //因為這是一維商品的送審, 一維商品只會有第一順位, 所以 jModel.Count 必須是 1
            if (jModel.Count != 1)
            {
                result.IsSuccess = false;
                result.Msg = "資料處理錯誤";
                logger.Error("sellerid: " + sellerid + "; 沒有對應的 ItemPropertyValue 或 ItemPropertyName 資料");
                return result;
            }
            List<TWNewEgg.API.Models.SaveProductProperty> _listSaveProductProperty = new List<SaveProductProperty>();
            TWNewEgg.API.Models.SaveProductProperty saveproPropertyColor = new SaveProductProperty();
            saveproPropertyColor.GroupID = jModel[0].ItemPropertyName_groupId;
            saveproPropertyColor.PropertyID = jModel[0].ItemPropertyValue_propertyNameID;
            saveproPropertyColor.ValueID = jModel[0].ItemPropertyValue_ID;
            saveproPropertyColor.InputValue = string.Empty;
            saveproPropertyColor.UpdateUser = userid.ToString();
            TWNewEgg.API.Service.ProductPorpertyTempService productPorpertyTempService = new ProductPorpertyTempService();
            _listSaveProductProperty.Add(saveproPropertyColor);
            var productPropertyTemp = productPorpertyTempService.SaveProductPropertyTempClick(_listSaveProductProperty, productTempId, userid);
            if (productPropertyTemp.IsSuccess == false)
            {
                result.IsSuccess = false;
                result.Msg = productPropertyTemp.Msg;
                return result;
            }
            else
            {
                result.IsSuccess = true;
                return result;
            }
        }
        #endregion
        #region 把資料寫入 ItemGroupDetailProperty
        public ActionResponse<string> ItemGroupDetailProperty(int groupid, int itemTempid, int sellerid,int userid, List<TWNewEgg.API.Models.propertyJoinModel> jModel, colorSizeModel color_size_model, propertyModel property)
        {
            ActionResponse<string> result = new ActionResponse<string>();
            TWNewEgg.DB.TWSqlDBContext dbFront = new TWSqlDBContext();
            TWNewEgg.DB.TWSQLDB.Models.ItemGroupDetailProperty itemGroupDetailProperty_insert = new DB.TWSQLDB.Models.ItemGroupDetailProperty();
            // 利用 colorValueId 在 jModel 抓取對應的 MasterPropertyID 和 GroupValueId(第一順位)
            var MasterPropertyID_GroupValueId = jModel.Where(p => p.ItemPropertyValue_ID == color_size_model.colorValueId).FirstOrDefault();
            //利用 proValueId  在 jModel 抓取對應的 valueName 和 propertyid(第二順位)
            var valueId_valueName_propertyid = jModel.Where(p => p.ItemPropertyValue_ID == property.proValueId).FirstOrDefault();
            
            itemGroupDetailProperty_insert.GroupID = groupid;
            itemGroupDetailProperty_insert.ItemTempID = itemTempid;
            itemGroupDetailProperty_insert.SellerID = sellerid;
            itemGroupDetailProperty_insert.MasterPropertyID = MasterPropertyID_GroupValueId.ItemPropertyName_ID;
            itemGroupDetailProperty_insert.PropertyID = valueId_valueName_propertyid == null ? 0 : valueId_valueName_propertyid.ItemPropertyName_ID;
            itemGroupDetailProperty_insert.GroupValueID = MasterPropertyID_GroupValueId.ItemPropertyValue_ID;
            itemGroupDetailProperty_insert.ValueID = valueId_valueName_propertyid == null ? 0 : valueId_valueName_propertyid.ItemPropertyValue_ID;
            itemGroupDetailProperty_insert.ValueName = valueId_valueName_propertyid == null ? string.Empty : valueId_valueName_propertyid.ItemPropertyValue_propertyValue;
            itemGroupDetailProperty_insert.InputValue = color_size_model.inputValue;
            itemGroupDetailProperty_insert.CreateDate = DateTime.Now;
            itemGroupDetailProperty_insert.InUser = userid;
            itemGroupDetailProperty_insert.UpdateDate = DateTime.Now;
            itemGroupDetailProperty_insert.UpdateUser = userid.ToString();
            dbFront.ItemGroupDetailProperty.Add(itemGroupDetailProperty_insert);
            try
            {
                dbFront.SaveChanges();
                result.IsSuccess = true;
            }
            catch (Exception error)
            {
                result.IsSuccess = false;
                result.Msg = "資料錯誤";
                logger.Error("[Message]: " + error.Message + " ;[StackTrace]: " + error.StackTrace);
            }
            return result;
        }
        #endregion
        #region 把資料寫入 ItemGroupProperty 表
        public ActionResponse<string> ItemGroupProperty(List<TWNewEgg.API.Models.propertyJoinModel> jModel, int groupid, int userid, int propertyid)
        {
            ActionResponse<string> result = new ActionResponse<string>();
            TWSqlDBContext dbFront = new TWSqlDBContext();
            TWNewEgg.DB.TWSQLDB.Models.ItemGroupProperty _itemgroupProperty = new DB.TWSQLDB.Models.ItemGroupProperty();
            jModel = jModel.Distinct().ToList();
            jModel = jModel.Take(1).ToList();
            _itemgroupProperty.GroupID = groupid;
            _itemgroupProperty.PropertyID = jModel[0].ItemPropertyName_ID;
            _itemgroupProperty.Order = 1;
            _itemgroupProperty.PropertyName = string.IsNullOrEmpty(jModel[0].ItemPropertyName_propertyNameTW) == true ? jModel[0].ItemPropertyName_propertyName : jModel[0].ItemPropertyName_propertyNameTW;
            _itemgroupProperty.CreateDate = DateTime.Now;
            _itemgroupProperty.InUser = userid;
            _itemgroupProperty.UpdateDate = DateTime.Now;
            _itemgroupProperty.UpdateUser = userid;
            dbFront.ItemGroupProperty.Add(_itemgroupProperty);
            try
            {
                dbFront.SaveChanges();
                result.IsSuccess = true;
            }
            catch (Exception error)
            {
                result.IsSuccess = false;
                result.Msg = "資料處理錯誤";
                logger.Error("[Message]: " + error.Message + "; [StackTrace]: " + error.StackTrace);
            }
            return result;
        }
        #endregion
        #region 把資料寫入 ItemGroup 表
        public ActionResponse<int> ItemGroup(int amount, int master, int userid, int sellerid)
        {
            TWNewEgg.DB.TWSqlDBContext dbFront = new TWSqlDBContext();
            ActionResponse<int> result = new ActionResponse<int>();
            try
            {
                TWNewEgg.DB.TWSQLDB.Models.ItemGroup insertItemGroup = new DB.TWSQLDB.Models.ItemGroup();
                insertItemGroup.Amount = amount;
                insertItemGroup.MasterPropertyID = master;
                insertItemGroup.SellerID = sellerid;
                insertItemGroup.CreateDate = DateTime.Now;
                insertItemGroup.InUser = userid;
                insertItemGroup.UpdateDate = DateTime.Now;
                insertItemGroup.UpdateUser = userid;
                dbFront.ItemGroup.Add(insertItemGroup);
                dbFront.SaveChanges();
                result.IsSuccess = true;
                result.Body = insertItemGroup.ID;//groupId
            }
            catch (Exception error)
            {
                logger.Error("[Msg] " + error.Message + "; [StackTrace]: " + error.StackTrace);
                result.IsSuccess = false;
                result.Msg = "資料錯誤";
            }
            return result;
        }
        #endregion
        #region 圖片處理
        public ActionResponse<List<string>> imgProcess(int? picStart, int? picEnd, int itemsketchid, int itemTempid, int mapindex)
        {
            TWNewEgg.API.Service.ImageService imgService = new ImageService();
            ActionResponse<List<string>> result = new ActionResponse<List<string>>();
            bool ConfigIsNoError = true;
            string images = string.Empty;
            int picID = itemsketchid;
            // 使用ItemID產生對應圖片網址        
            string pid = picID.ToString("00000000");
            string pidf4 = pid.Substring(0, 4);
            string pidl4 = pid.Substring(4, 4);
            try
            {
                images = System.Configuration.ConfigurationManager.AppSettings["Images"];
                ConfigIsNoError = true;
            }
            catch (Exception error)
            {
                ConfigIsNoError = false;
                logger.Error("[MSG]: " + error.Message + " ;[StackTrace]: " + error.StackTrace);
            }
            if (ConfigIsNoError == false)
            {
                result.IsSuccess = false;
                result.Msg = "送審資料處理錯誤";
                return result;
            }
            List<string> urlList = new List<string>();
            string url = images + "/pic/itemsketch/" + pid.Substring(0, 4) + "/" + pid.Substring(4, 4) + "_" + mapindex + "_640.jpg";
            urlList.Add(url);
            //for (int index = 1; index <= picEnd; index++)
            //{
            //    string url = images + "/pic/itemsketch/" + pid.Substring(0, 4) + "/" + pid.Substring(4, 4) + "_" + index + "_640.jpg";
            //    urlList.Add(url);
            //}
            var ImgResult = imgService.ImageProcess(urlList, "pic\\itemtemp", "pic\\pic\\itemtemp", itemTempid);
            if (ImgResult.IsSuccess == false)
            {
                result.IsSuccess = false;
                result.Msg = ImgResult.Msg;
                return result;
            }
            else
            {
                result.IsSuccess = true;
                result.Msg = ImgResult.Msg;
                //result.Body = urlList;
                return result;
            }
            
        }
        #endregion
        #region 組合 ItemPropertyValue 和 ItemPropertyName
        public ActionResponse<List<TWNewEgg.API.Models.propertyJoinModel>> join_ItemPropertyValue_ItemPropertyName_Result(TWNewEgg.API.Models.BatchExamineModel batchNodelJoin)
        {
            ActionResponse<List<TWNewEgg.API.Models.propertyJoinModel>> result = new ActionResponse<List<propertyJoinModel>>();
            TWSqlDBContext dbFront = new TWSqlDBContext();
            List<int> idModel = new List<int>();
            //收集要 join 的 第一順位的 id
            foreach (var iditem in batchNodelJoin.colorsizeProperty)
            {
                //因為一維屬性商品不會有第二順位的屬性, 所以不必把第二順位的 id 加到 List 去 Join 相關資料
                idModel.Add(iditem.colorValueId);
            }
            List<TWNewEgg.API.Models.propertyJoinModel> JoinModel = new List<propertyJoinModel>();
            try
            {
                JoinModel = (from p in idModel
                             join q in dbFront.ItemPropertyValue on p equals q.ID
                             join r in dbFront.ItemPropertyName on q.PropertyNameID equals r.ID
                             select new TWNewEgg.API.Models.propertyJoinModel
                             {
                                 ItemPropertyValue_ID = p,
                                 ItemPropertyValue_propertyNameID = q.PropertyNameID,
                                 ItemPropertyValue_propertyValue = q.PropertyValue,
                                 ItemPropertyValue_propertyValueTW = q.PropertyValueTW,
                                 ItemPropertyName_groupId = r.GroupID,
                                 ItemPropertyName_ID = r.ID,
                                 ItemPropertyName_propertyName = r.PropertyName,
                                 ItemPropertyName_propertyNameTW = r.PropertyNameTW
                             }).ToList();
                if (idModel.Count != JoinModel.Count)
                {
                    result.IsSuccess = false;
                    result.Msg = "資料處理錯誤";
                    logger.Error("輸入的 idModel count 與 JoinModel Count 不相等");
                }
                else
                {
                    result.IsSuccess = true;
                    result.Body = JoinModel;
                }
            }
            catch (Exception error)
            {
                result.IsSuccess = false;
                result.Msg = "資料處理錯誤";
                logger.Error("[Message]: " + error.Message + "; [StackTrace]: " + error.StackTrace);
            }
            return result;
        }
        #endregion
        #region 草稿區送審時檢查資料完整性
        public ActionResponse<string> checkModelElement(List<DB.TWSQLDB.Models.ItemSketch> itemsketch, int dimensionCount = 0)
        {

            ActionResponse<string> result = new ActionResponse<string>();
            TWNewEgg.DB.TWSqlDBContext dbFront = new TWSqlDBContext();
            List<int> itemsketch_id = new List<int>();
            itemsketch_id = itemsketch.Select(p => p.ID).ToList();
            var list_ItemSketchProperty = dbFront.ItemSketchProperty.Where(p => itemsketch_id.Contains(p.ItemSketchID)).ToList();
            result.IsSuccess = true;
            result.Msg = string.Empty;
            foreach (var item in itemsketch)
            {
                #region 商品類別
                if (item.CategoryID == null || item.CategoryID == 0)
                {
                    result.IsSuccess = false;
                    result.Msg = "產品編號為: " + item.ID + ",請選擇商品主分類";
                    break;
                }
                #endregion
                #region 製造商
                if (item.ManufactureID == null || item.ManufactureID == 0)
                {
                    result.IsSuccess = false;
                    result.Msg = "產品編號為: " + item.ID + ",請選擇製造商";
                    break;
                }
                #endregion
                #region 材積(公分) 長
                if (item.Length == null)
                {
                    result.IsSuccess = false;
                    result.Msg = "產品編號為: " + item.ID + ",請填選材積\"長\"";
                    break;
                }
                else
                {
                    if (item.Length <= 0)
                    {
                        result.IsSuccess = false;
                        result.Msg = "產品編號為: " + item.ID + ",材積(公分) 長: 必須大於等於 0";
                        break;
                    }
                }
                #endregion
                #region 材積(公分) 寬
                if (item.Width == null)
                {
                    result.IsSuccess = false;
                    result.Msg = "產品編號為: " + item.ID + ",請填選材積\"寬\"";
                    break;
                }
                else
                {
                    if (item.Width <= 0)
                    {
                        result.IsSuccess = false;
                        result.Msg = "產品編號為: " + item.ID + ",材積(公分) 寬: 必須大於等於 0";
                        break;
                    }
                }
                #endregion
                #region 材積(公分) 高
                if (item.Height == null)
                {
                    result.IsSuccess = false;
                    result.Msg = "產品編號為: " + item.ID + ",請填選材積\"高\"";
                    break;
                }
                else
                {
                    if (item.Height <= 0)
                    {
                        result.IsSuccess = false;
                        result.Msg = "產品編號為: " + item.ID + ",材積(公分) 長: 必須大於等於 0";
                        break;
                    }
                }
                #endregion
                #region 重量(公斤)
                if (item.Weight == null)
                {
                    result.IsSuccess = false;
                    result.Msg = "產品編號為: " + item.ID + ",請填選材積\"重量\"";
                    break;
                }
                else
                {
                    if (item.Weight <= 0)
                    {
                        result.IsSuccess = false;
                        result.Msg = "產品編號為: " + item.ID + ",重量(公斤) : 必須大於等於 0";
                        break;
                    }
                }
                #endregion
                #region 窒息危險性
                if (string.IsNullOrEmpty(item.IsChokingDanger) == true)
                {
                    result.IsSuccess = false;
                    result.Msg = "產品編號為: " + item.ID + ",請填選\"窒息危險性\"";
                    break;
                }
                #endregion
                #region 遞送危險物料
                if (string.IsNullOrEmpty(item.IsShipDanger) == true)
                {
                    result.IsSuccess = false;
                    result.Msg = "產品編號為: " + item.ID + ",請填選\"遞送危險物料\"";
                    break;
                }
                #endregion
                #region 遞送方式
                if (string.IsNullOrEmpty(item.ItemPackage) == true)
                {
                    result.IsSuccess = false;
                    result.Msg = "產品編號為: " + item.ID + ",請填選\"遞送方式\"";
                    break;
                }
                #endregion
                #region 售價
                if (item.PriceCash == null)
                {
                    result.IsSuccess = false;
                    result.Msg = "產品編號為: " + item.ID + ",請填選\"售價\"";
                    break;
                }
                #endregion
                #region 成本
                if (item.Cost == null)
                {
                    result.IsSuccess = false;
                    result.Msg = "產品編號為: " + item.ID + ",請填選\"成本\"";
                    break;
                }
                #endregion
                #region 毛利率
                if ((item.PriceCash.HasValue && item.Cost.HasValue) && item.Cost.Value > item.PriceCash.Value)
                {
                    result.IsSuccess = false;
                    result.Msg = "產品編號為: " + item.ID + ",\"毛利率\"為負數，請重新設定售價或成本";
                    break;
                }
                #endregion
                #region 安全庫存
                if (item.InventorySafeQty <= 0)
                {
                    result.IsSuccess = false;
                    result.Msg = "產品編號為: " + item.ID + ",\"安全庫存\"不可為0";
                    break;
                }
                else
                {
                    if (dimensionCount == 1)
                    {
                        var list_ItemSketchProperty_Detail = list_ItemSketchProperty.Where(p => p.ItemSketchID == item.ID).ToList();
                        foreach (var check_item_ItemSketchProperty in list_ItemSketchProperty_Detail)
                        {
                            if ((item.InventorySafeQty - check_item_ItemSketchProperty.Qty) > 0)
                            {
                                result.IsSuccess = false;
                                result.Msg = "產品編號為: " + item.ID + ",\"安全庫存不可大於可售數量\"";
                                break;
                            }

                        }
                    }
                    else if (dimensionCount == 2)
                    {
                        var list_ItemSketchProperty_Detail = list_ItemSketchProperty.Where(p => p.ItemSketchID == item.ID && p.GroupValueID != p.ValueID).ToList();
                        foreach (var check_item_ItemSketchProperty in list_ItemSketchProperty_Detail)
                        {
                            if ((item.InventorySafeQty - check_item_ItemSketchProperty.Qty) > 0)
                            {
                                result.IsSuccess = false;
                                result.Msg = "產品編號為: " + item.ID + ",\"安全庫存不可大於可售數量\"";
                                break;
                            }
                        }
                    }
                    else
                    {
                        result.IsSuccess = false;
                        result.Msg = "系統錯誤";
                        break;
                    }
                }
                #endregion
                #region 商品名稱
                if (string.IsNullOrEmpty(item.Name) == true)
                {
                    result.IsSuccess = false;
                    result.Msg = "產品編號為: " + item.ID + ",請填選\"商品名稱\"";
                    break;
                }
                #endregion
                #region 商品簡要描述
                if (string.IsNullOrEmpty(item.Spechead) == true)
                {
                    result.IsSuccess = false;
                    result.Msg = "產品編號為: " + item.ID + ",請填選\" 商品簡要描述\"";
                    break;
                }
                #endregion
                #region 商品特色標題
                if (string.IsNullOrEmpty(item.Sdesc) == true)
                {
                    result.IsSuccess = false;
                    result.Msg = "產品編號為: " + item.ID + ",請填選\" 商品特色標題\"";
                    break;
                }
                #endregion
                #region 商品中文說明
                if (string.IsNullOrEmpty(item.Description) == true)
                {
                    result.IsSuccess = false;
                    result.Msg = "產品編號為: " + item.ID + ",請填選\" 商品中文說明\"";
                    break;
                }
                #endregion
                #region 賣場開賣時間
                //賣場開賣時間
                if (item.DateStart == null)
                {
                    result.IsSuccess = false;
                    result.Msg = "產品編號為: " + item.ID + ",請填選\"賣場開賣日期錯誤\"";
                    break;
                }
                else
                {
                    DateTime timeTemp;
                    bool dateSuccess = DateTime.TryParse(item.DateStart.ToString(), out timeTemp);
                    if (dateSuccess == false)
                    {
                        result.IsSuccess = false;
                        result.Msg = "產品編號為: " + item.ID + ",請填選\"賣場開賣日期錯誤\"";
                        break;
                    }
                }
                //賣場結束日期
                if (item.DateEnd == null)
                {
                    result.IsSuccess = false;
                    result.Msg = "產品編號為: " + item.ID + ",賣場結束日期錯誤";
                    break;
                }
                else
                {
                    DateTime timeTemp;
                    bool dateSuccess = DateTime.TryParse(item.DateEnd.ToString(), out timeTemp);
                    if (dateSuccess == false)
                    {
                        result.IsSuccess = false;
                        result.Msg = "產品編號為: " + item.ID + ",賣場結束日期錯誤";
                        break;
                    }
                }
                #endregion

            }

            return result;
        }
        #endregion

        #region 抓出 Exception 錯誤訊息
        public string ExceptionMessage(Exception error)
        {
            string result = string.Empty;
            result = error.InnerException != null ? error.InnerException.Message : "";
            return result;
        }
        #endregion

    }
}
