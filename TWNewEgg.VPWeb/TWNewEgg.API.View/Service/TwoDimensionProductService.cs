using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;


namespace TWNewEgg.API.View.Service
{
    public class TwoDimensionProductService
    {
        private log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.FullName);

        #region 將 View Model 組成 API 的 Model

        /// <summary>
        /// 組合規格品資料
        /// </summary>
        /// <param name="newItemData">草稿資料</param>
        /// <param name="twoDimenstionProperty">二維屬性資料</param>
        /// <returns>規格品資料</returns>
        public TWNewEgg.API.Models.ActionResponse<TWNewEgg.API.Models.DomainModel.CreateStandardProductSketch> MakeCreateStandardProductSketch(TWNewEgg.API.View.ItemCreationVM newItemData, TWNewEgg.API.View.TwoDimensionProductProperty twoDimenstionProperty)
        {
            TWNewEgg.API.Models.ActionResponse<TWNewEgg.API.Models.DomainModel.CreateStandardProductSketch> result = new API.Models.ActionResponse<API.Models.DomainModel.CreateStandardProductSketch>();
            result.Body = new API.Models.DomainModel.CreateStandardProductSketch();
            result.IsSuccess = true;
            result.Msg = string.Empty;

            // 組合 Model
            TWNewEgg.API.Models.ActionResponse<TWNewEgg.API.Models.ItemSketch> makeItemSketch = MakeItemSketch(newItemData, twoDimenstionProperty.MainPropertyValueCell);
            TWNewEgg.API.Models.ActionResponse<List<TWNewEgg.DB.TWSQLDB.Models.ItemSketchGroup>> makeItemSketchGroup = MakeItemSketchGroup(twoDimenstionProperty);
            TWNewEgg.API.Models.ActionResponse<List<TWNewEgg.DB.TWSQLDB.Models.ItemSketchProperty>> makeItemSketchProperty = MakeItemSketchProperty(twoDimenstionProperty);

            if (makeItemSketch.IsSuccess && makeItemSketchGroup.IsSuccess && makeItemSketchProperty.IsSuccess)
            {
                result.Body.basicItemInfo.Add(makeItemSketch.Body);
                result.Body.twodimProperty.ItemGroup = makeItemSketchGroup.Body;
                result.Body.twodimProperty.ItemProperty = makeItemSketchProperty.Body;
            }
            else
            {
                result.IsSuccess = false;
                log.Info("組合規格品資料失敗");
            }

            result.Code = SetResponseCode(result.IsSuccess);

            return result;
        }

        /// <summary>
        /// 組合草稿資料
        /// </summary>
        /// <param name="newItemData">草稿資料</param>
        /// <param name="mainPropertyValueCell">二維屬性資料</param>
        /// <returns>草稿資料</returns>
        private TWNewEgg.API.Models.ActionResponse<TWNewEgg.API.Models.ItemSketch> MakeItemSketch(TWNewEgg.API.View.ItemCreationVM newItemData, List<TWNewEgg.API.View.MainPropertyValue> mainPropertyValueCell)
        {
            TWNewEgg.API.Models.ActionResponse<TWNewEgg.API.Models.ItemSketch> result = new API.Models.ActionResponse<API.Models.ItemSketch>();
            result.Body = new API.Models.ItemSketch();
            result.IsSuccess = true;
            result.Msg = string.Empty;

            try
            {
                AutoMapper.Mapper.CreateMap<TWNewEgg.API.View.ItemCreationVM, TWNewEgg.API.Models.ItemSketch>();
                result.Body = AutoMapper.Mapper.Map<TWNewEgg.API.Models.ItemSketch>(newItemData);

                #region 圖片處理

                foreach (MainPropertyValue mainPropertyValue in mainPropertyValueCell)
                {
                    if (string.IsNullOrEmpty(mainPropertyValue.PictureURL))
                    {
                        result.Body.Product.PicPatch_Edit.Add("http://" + HttpContext.Current.Request.Url.Authority + "/Themes/Images/Item/defautlNoImage.png");
                    }
                    else
                    {
                        result.Body.Product.PicPatch_Edit.Add(mainPropertyValue.PictureURL);
                    }
                }

                #endregion 圖片處理

                // 填寫使用者資訊
                TWNewEgg.API.View.Service.SellerInfoService sellerInfoService = new Service.SellerInfoService();
                result.Body.CreateAndUpdate.CreateUser = sellerInfoService.UserID;
                result.Body.CreateAndUpdate.UpdateUser = sellerInfoService.UserID;
                result.Body.Item.SellerID = sellerInfoService.currentSellerID;
            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                log.Info(string.Format("組合草稿失敗(expection); ExceptionMessage = {0}; StackTrace = {1}.", GetExceptionMessage(ex), ex.StackTrace));
            }

            result.Code = SetResponseCode(result.IsSuccess);

            return result;
        }

        /// <summary>
        /// 組合群組資料
        /// </summary>
        /// <param name="twoDimenstionProperty">二維屬性資料</param>
        /// <returns>群組資料</returns>
        private TWNewEgg.API.Models.ActionResponse<List<TWNewEgg.DB.TWSQLDB.Models.ItemSketchGroup>> MakeItemSketchGroup(TWNewEgg.API.View.TwoDimensionProductProperty twoDimenstionProperty)
        {
            TWNewEgg.API.Models.ActionResponse<List<TWNewEgg.DB.TWSQLDB.Models.ItemSketchGroup>> result = new TWNewEgg.API.Models.ActionResponse<List<TWNewEgg.DB.TWSQLDB.Models.ItemSketchGroup>>();
            result.Body = new List<TWNewEgg.DB.TWSQLDB.Models.ItemSketchGroup>();
            result.IsSuccess = true;
            result.Msg = string.Empty;

            try
            {
                TWNewEgg.API.View.Service.SellerInfoService sellerInfoService = new Service.SellerInfoService();

                TWNewEgg.DB.TWSQLDB.Models.ItemSketchGroup sketchGroup = new TWNewEgg.DB.TWSQLDB.Models.ItemSketchGroup();
                sketchGroup.Order = 1;
                sketchGroup.PropertyID = twoDimenstionProperty.MainPropertyID;
                sketchGroup.PropertyName = twoDimenstionProperty.MainPropertyName;
                sketchGroup.InUser = sellerInfoService.UserID;
                sketchGroup.UpdateUser = sellerInfoService.UserID;
                result.Body.Add(sketchGroup);

                if (twoDimenstionProperty.SecondPropertyID != null)
                {
                    sketchGroup = new TWNewEgg.DB.TWSQLDB.Models.ItemSketchGroup();
                    sketchGroup.Order = 2;
                    sketchGroup.PropertyID = twoDimenstionProperty.SecondPropertyID;
                    sketchGroup.PropertyName = twoDimenstionProperty.SecondPropertyName;
                    sketchGroup.InUser = sellerInfoService.UserID;
                    sketchGroup.UpdateUser = sellerInfoService.UserID;
                    result.Body.Add(sketchGroup);
                }
            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                log.Info(string.Format("組合群組(ItemSketchGroup)資料失敗(expection); ExceptionMessage = {0}; StackTrace = {1}.", GetExceptionMessage(ex), ex.StackTrace));
            }

            result.Code = SetResponseCode(result.IsSuccess);

            return result;
        }

        /// <summary>
        /// 組合二維屬性資料
        /// </summary>
        /// <param name="twoDimenstionProperty">二維屬性資料</param>
        /// <returns>二維屬性資料</returns>
        private TWNewEgg.API.Models.ActionResponse<List<TWNewEgg.DB.TWSQLDB.Models.ItemSketchProperty>> MakeItemSketchProperty(TWNewEgg.API.View.TwoDimensionProductProperty twoDimenstionProperty)
        {
            TWNewEgg.API.Models.ActionResponse<List<TWNewEgg.DB.TWSQLDB.Models.ItemSketchProperty>> result = new TWNewEgg.API.Models.ActionResponse<List<TWNewEgg.DB.TWSQLDB.Models.ItemSketchProperty>>();
            result.Body = new List<TWNewEgg.DB.TWSQLDB.Models.ItemSketchProperty>();
            result.IsSuccess = true;
            result.Msg = string.Empty;

            if (twoDimenstionProperty.MainPropertyValueCell != null && twoDimenstionProperty.MainPropertyValueCell.Count > 0)
            {
                try
                {
                    TWNewEgg.API.View.Service.SellerInfoService sellerInfoService = new Service.SellerInfoService();

                    foreach (MainPropertyValue mainPropertyValue in twoDimenstionProperty.MainPropertyValueCell)
                    {
                        TWNewEgg.DB.TWSQLDB.Models.ItemSketchProperty sketchProperty = new DB.TWSQLDB.Models.ItemSketchProperty();

                        sketchProperty.PropertyID = twoDimenstionProperty.MainPropertyID.Value;
                        sketchProperty.GroupValueID = mainPropertyValue.MainPropertyValueID;
                        sketchProperty.ValueID = mainPropertyValue.MainPropertyValueID;
                        sketchProperty.ValueName = mainPropertyValue.MainPropertyValueName;
                        sketchProperty.InputValue = mainPropertyValue.InputValue;
                        if (twoDimenstionProperty.SecondPropertyID == null)
                        {
                            sketchProperty.Qty = mainPropertyValue.SecondPropertyValueCell[0].CanSaleQty;
                        }
                        sketchProperty.InUser = sellerInfoService.UserID;
                        sketchProperty.UpdateUser = sellerInfoService.UserID;
                        result.Body.Add(sketchProperty);

                        if (twoDimenstionProperty.SecondPropertyID != null)
                        {
                            foreach (SecondPropertyValue secondPropertyValue in mainPropertyValue.SecondPropertyValueCell)
                            {
                                sketchProperty = new DB.TWSQLDB.Models.ItemSketchProperty();

                                sketchProperty.PropertyID = twoDimenstionProperty.SecondPropertyID.Value;
                                sketchProperty.GroupValueID = mainPropertyValue.MainPropertyValueID;
                                sketchProperty.ValueID = secondPropertyValue.SecondPropertyValueID;
                                sketchProperty.ValueName = secondPropertyValue.SecondPropertyValueName;
                                sketchProperty.Qty = secondPropertyValue.CanSaleQty;
                                sketchProperty.InUser = sellerInfoService.UserID;
                                sketchProperty.UpdateUser = sellerInfoService.UserID;
                                result.Body.Add(sketchProperty);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    result.IsSuccess = false;
                    log.Info(string.Format("組合二維屬性(ItemSketchProperty)資料失敗(expection); ExceptionMessage = {0}; StackTrace = {1}.", GetExceptionMessage(ex), ex.StackTrace));
                }
            }

            result.Code = SetResponseCode(result.IsSuccess);

            return result;
        }

        #endregion 將 View Model 組成 API 的 Model

        #region 取得 Exception 訊息、填寫 Response Code

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

        /// <summary>
        /// 填寫 Response Code
        /// </summary>
        /// <param name="isSuccess">成功、失敗資訊</param>
        /// <returns>Response Code</returns>
        private int SetResponseCode(bool isSuccess)
        {
            if (isSuccess)
            {
                return (int)TWNewEgg.API.Models.ResponseCode.Success;
            }
            else
            {
                return (int)TWNewEgg.API.Models.ResponseCode.Error;
            }
        }

        #endregion 取得 Exception 訊息、填寫 Response Code
    }
}