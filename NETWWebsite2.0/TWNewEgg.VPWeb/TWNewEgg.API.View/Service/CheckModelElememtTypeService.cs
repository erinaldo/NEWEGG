using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TWNewEgg.API.View.Service
{
    public class CheckModelElememtTypeService
    {
        public TWNewEgg.API.Models.ActionResponse<string> modelStatusCheck<T>()
        {
            TWNewEgg.API.Models.ActionResponse<string> result = new API.Models.ActionResponse<string>();
            //    System.Type myType = typeof(T);
            //    var ttt = myType.Attributes;
            //    List<string> aaa = new List<string>();
            //    foreach (var item in myType.GetProperties())
            //    {
            //        var test = item.Attributes;
            //        var ii = item.GetMethod.CustomAttributes.ToList();
            //        var i = item.PropertyType.FullName;
            //        aaa.Add(i.ToString());
            //        try
            //        {

            //            var j = Convert.ChangeType(123, System.Type.GetType(i));
            //            var j1 = Convert.ChangeType("AAA", System.Type.GetType(i));
            //        }
            //        catch (Exception error)
            //        {

            //        }
            //    }
            return result;
        }
        public TWNewEgg.API.Models.ActionResponse<string> modelStatusCheckAttr(List<TWNewEgg.API.Models.ItemSketch> _itemstetch, string stetchSwich)
        {
            TWNewEgg.API.Models.ActionResponse<string> result = new API.Models.ActionResponse<string>();
            int checkItemMaretProce = 0;
            decimal dec_check = 0;
            result.Code = (int)TWNewEgg.API.Models.ResponseCode.Success;
            result.IsSuccess = true;
            switch (stetchSwich)
            {
                case "sketchFirst":
                    foreach (var _item in _itemstetch)
                    {
                        #region 建議售價
                        //型別轉換碩物
                        if (decimal.TryParse(_item.Item.MarketPrice.ToString(), out dec_check) == false)
                        {
                            result.Code = (int)TWNewEgg.API.Models.ResponseCode.Error;
                            result.IsSuccess = false;
                            result.Msg = "建議售價(元) 格式錯誤";
                            break;
                        }
                        else
                        {
                            //輸入為小數
                            if (_item.Item.MarketPrice - (int)_item.Item.MarketPrice != 0)
                            {
                                result.Code = (int)TWNewEgg.API.Models.ResponseCode.Error;
                                result.IsSuccess = false;
                                result.Msg = "建議售價(元) 格式錯誤";
                                break;
                            }
                            else
                            {
                                if (_item.Item.MarketPrice > 99999999)
                                {
                                    result.Code = (int)TWNewEgg.API.Models.ResponseCode.Error;
                                    result.IsSuccess = false;
                                    result.Msg = "建議售價(元) 格式錯誤，最多為 99999999";
                                    break;
                                }
                            }
                        }
                        //if (_item.Item.MarketPrice != 0)
                        //{
                        //    checkItemMaretProce = 0;
                        //    int.TryParse(_item.Item.MarketPrice.ToString(), out checkItemMaretProce);
                        //    if (checkItemMaretProce == 0)
                        //    {
                        //        result.Code = (int)TWNewEgg.API.Models.ResponseCode.Error;
                        //        result.IsSuccess = false;
                        //        result.Msg = "建議售價(元) 格式錯誤";
                        //        break;
                        //    }
                        //}
                        #endregion
                        #region 售價
                        if (decimal.TryParse(_item.Item.PriceCash.ToString(), out dec_check) == false)
                        {
                            result.Code = (int)TWNewEgg.API.Models.ResponseCode.Error;
                            result.IsSuccess = false;
                            result.Msg = "售價(元) 格式錯誤";
                            break;
                        }
                        else
                        {
                            if (_item.Item.PriceCash - (int)_item.Item.PriceCash != 0)
                            {
                                result.Code = (int)TWNewEgg.API.Models.ResponseCode.Error;
                                result.IsSuccess = false;
                                result.Msg = "售價(元) 格式錯誤";
                                break;
                            }
                            else
                            {
                                if (_item.Item.PriceCash > 99999999)
                                {
                                    result.Code = (int)TWNewEgg.API.Models.ResponseCode.Error;
                                    result.IsSuccess = false;
                                    result.Msg = "售價(元) 格式錯誤，最多為 99999999";
                                    break;
                                }
                            }
                        }


                        //if (_item.Item.PriceCash != 0)
                        //{
                        //    checkItemMaretProce = 0;
                        //    int.TryParse(_item.Item.PriceCash.ToString(), out checkItemMaretProce);
                        //    if (checkItemMaretProce == 0)
                        //    {
                        //        result.Code = (int)TWNewEgg.API.Models.ResponseCode.Error;
                        //        result.IsSuccess = false;
                        //        result.Msg = "售價(元) 格式錯誤";
                        //        break;
                        //    }
                        //}
                        #endregion
                        #region 成本價
                        if (decimal.TryParse(_item.Product.Cost.ToString(), out dec_check) == false)
                        {
                            result.Code = (int)TWNewEgg.API.Models.ResponseCode.Error;
                            result.IsSuccess = false;
                            result.Msg = "成本價(元) 格式錯誤";
                            break;
                        }
                        else
                        {
                            if (_item.Product.Cost - (int)_item.Product.Cost != 0)
                            {
                                result.Code = (int)TWNewEgg.API.Models.ResponseCode.Error;
                                result.IsSuccess = false;
                                result.Msg = "成本價(元) 格式錯誤";
                            }
                            else
                            {
                                if (_item.Product.Cost > 99999999)
                                {
                                    result.Code = (int)TWNewEgg.API.Models.ResponseCode.Error;
                                    result.IsSuccess = false;
                                    result.Msg = "成本價(元) 格式錯誤，最多為 99999999";
                                    break;
                                }
                            }
                        }
                        //if (_item.Product.Cost != 0)
                        //{
                        //    checkItemMaretProce = 0;
                        //    int.TryParse(_item.Product.Cost.ToString(), out checkItemMaretProce);
                        //    if (checkItemMaretProce == 0)
                        //    {
                        //        result.Code = (int)TWNewEgg.API.Models.ResponseCode.Error;
                        //        result.IsSuccess = false;
                        //        result.Msg = "成本價(元) 格式錯誤";
                        //        break;
                        //    }
                        //}
                        #endregion

                        #region 毛利率

                        if ((_item.Product.Cost.HasValue && _item.Item.PriceCash.HasValue) && (_item.Product.Cost.Value > _item.Item.PriceCash.Value))
                        {
                            result.Code = (int)TWNewEgg.API.Models.ResponseCode.Error;
                            result.IsSuccess = false;
                            result.Msg = "毛利率為負數，請重新設定售價或成本";
                            break;
                        }

                        #endregion 毛利率

                        #region 可售數量
                        if (int.TryParse(_item.ItemStock.CanSaleQty.ToString(), out checkItemMaretProce) == false)
                        {
                            result.Code = (int)TWNewEgg.API.Models.ResponseCode.Error;
                            result.IsSuccess = false;
                            result.Msg = "可售數量 格式錯誤";
                            break;
                        }
                        else
                        {
                            if (_item.ItemStock.CanSaleQty > 99999999)
                            {
                                result.Code = (int)TWNewEgg.API.Models.ResponseCode.Error;
                                result.IsSuccess = false;
                                result.Msg = "可售數量，最多為 99999999";
                                break;
                            }

                        }
                        #endregion
                        #region 安全庫存量
                        if (int.TryParse(_item.ItemStock.InventorySafeQty.ToString(), out checkItemMaretProce) == false)
                        {
                            result.Code = (int)TWNewEgg.API.Models.ResponseCode.Error;
                            result.IsSuccess = false;
                            result.Msg = "安全庫存量 格式錯誤";
                            break;
                        }
                        else
                        {
                            if (_item.ItemStock.InventorySafeQty > 99999999)
                            {
                                result.Code = (int)TWNewEgg.API.Models.ResponseCode.Error;
                                result.IsSuccess = false;
                                result.Msg = "安全庫存量，最多為 99999999";
                                break;
                            }

                        }

                        #endregion
                        #region 限量可售
                        if (int.TryParse(_item.Item.CanSaleLimitQty.ToString(), out checkItemMaretProce) == false)
                        {
                            result.Code = (int)TWNewEgg.API.Models.ResponseCode.Error;
                            result.IsSuccess = false;
                            result.Msg = "限量可售 格式錯誤";
                            break;
                        }
                        else
                        {
                            if (_item.Item.CanSaleLimitQty > 99999999)
                            {
                                result.Code = (int)TWNewEgg.API.Models.ResponseCode.Error;
                                result.IsSuccess = false;
                                result.Msg = "限量可售，最多為 99999999";
                                break;
                            }
                        }
                        #endregion
                        if (_item.CreateAndUpdate.CreateDate != null)
                        {
                            DateTime date = DateTime.Now;
                            if (DateTime.TryParse(_item.CreateAndUpdate.CreateDate.ToString(), out date) == false)
                            {
                                result.Code = (int)TWNewEgg.API.Models.ResponseCode.Error;
                                result.IsSuccess = false;
                                result.Msg = "開始銷售日期(起) 格式錯誤";
                                break;
                            }
                        }
                    }

                    break;
                case "propertySketchProperty":
                    #region propertySketchProperty
                    foreach (var item in _itemstetch)
                    {
                        DateTime dateTransform = new DateTime();
                        #region 建議售價
                        if (item.Item.MarketPrice != null)
                        {
                            bool isInteger = System.Text.RegularExpressions.Regex.IsMatch(item.Item.MarketPrice.ToString(), @"^-?\d+$", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                            if (isInteger == false)
                            {
                                result.IsSuccess = false;
                                result.Msg = "建議售價格式錯誤";
                                break;
                            }
                            else
                            {
                                if (item.Item.MarketPrice < 0)
                                {
                                    result.IsSuccess = false;
                                    result.Msg = "建議售價不可小於 0";
                                    break;
                                }
                                else
                                {
                                    if (item.Item.MarketPrice > 99999999)
                                    {
                                        result.Code = (int)TWNewEgg.API.Models.ResponseCode.Error;
                                        result.IsSuccess = false;
                                        result.Msg = "建議售價(元) 格式錯誤，最多為 99999999";
                                        break;
                                    }
                                }
                            }
                        }
                        #endregion 建議售價
                        #region 售價(元)
                        if (item.Item.PriceCash != null)
                        {
                            bool isInteger = System.Text.RegularExpressions.Regex.IsMatch(item.Item.PriceCash.ToString(), @"^-?\d+$", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                            if (isInteger == false)
                            {
                                result.IsSuccess = false;
                                result.Msg = "售價(元)格式錯誤";
                                break;
                            }
                            else
                            {
                                if (item.Item.PriceCash < 0)
                                {
                                    result.IsSuccess = false;
                                    result.Msg = "售價(元)不可小於 0";
                                    break;
                                }
                                else
                                {
                                    if (item.Item.PriceCash > 99999999)
                                    {
                                        result.Code = (int)TWNewEgg.API.Models.ResponseCode.Error;
                                        result.IsSuccess = false;
                                        result.Msg = "售價(元)格式錯誤最多為 99999999";
                                        break;
                                    }
                                }
                            }
                        }
                        #endregion 售價(元)
                        #region 成本價(元)
                        if (item.Product.Cost != null)
                        {
                            bool isInteger = System.Text.RegularExpressions.Regex.IsMatch(item.Product.Cost.ToString(), @"^-?\d+$", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                            if (isInteger == false)
                            {
                                result.IsSuccess = false;
                                result.Msg = "成本價(元)格式錯誤";
                                break;
                            }
                            else
                            {
                                if (item.Product.Cost < 0)
                                {
                                    result.IsSuccess = false;
                                    result.Msg = "成本價(元)不可小於 0";
                                    break;
                                }
                                else
                                {
                                    if (item.Product.Cost > 99999999)
                                    {
                                        result.Code = (int)TWNewEgg.API.Models.ResponseCode.Error;
                                        result.IsSuccess = false;
                                        result.Msg = "成本價(元)格式錯誤，最多為 99999999";
                                        break;
                                    }
                                }
                            }
                        }
                        #endregion 成本

                        #region 毛利率

                        if ((item.Product.Cost.HasValue && item.Item.PriceCash.HasValue) && (item.Product.Cost.Value > item.Item.PriceCash.Value))
                        {
                            result.Code = (int)TWNewEgg.API.Models.ResponseCode.Error;
                            result.IsSuccess = false;
                            result.Msg = "毛利率為負數，請重新設定售價或成本";
                            break;
                        }

                        #endregion 毛利率

                        #region 可售數量
                        if (item.ItemStock.CanSaleQty != null)
                        {
                            bool isInteger = System.Text.RegularExpressions.Regex.IsMatch(item.ItemStock.CanSaleQty.ToString(), @"^-?\d+$", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                            if (isInteger == false)
                            {
                                result.IsSuccess = false;
                                result.Msg = "可售數量格式錯誤";
                                break;
                            }
                            else
                            {
                                if (item.ItemStock.CanSaleQty < 0)
                                {
                                    result.IsSuccess = false;
                                    result.Msg = "可售數量不可小於 0";
                                    break;
                                }
                                else
                                {
                                    if (item.ItemStock.CanSaleQty > 99999999)
                                    {
                                        result.Code = (int)TWNewEgg.API.Models.ResponseCode.Error;
                                        result.IsSuccess = false;
                                        result.Msg = "可售數量格式錯誤，最多為 99999999";
                                        break;
                                    }
                                }
                            }
                        }
                        #endregion 可售數量
                        #region 安全庫存量
                        if (item.ItemStock.InventorySafeQty != null)
                        {
                            bool isInteger = System.Text.RegularExpressions.Regex.IsMatch(item.ItemStock.InventorySafeQty.ToString(), @"^-?\d+$", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                            if (isInteger == false)
                            {
                                result.IsSuccess = false;
                                result.Msg = "安全庫存量格式錯誤";
                                break;
                            }
                            else
                            {
                                if (item.ItemStock.InventorySafeQty < 0)
                                {
                                    result.IsSuccess = false;
                                    result.Msg = "安全庫存量不可小於 0";
                                    break;
                                }
                                else
                                {
                                    if (item.ItemStock.InventorySafeQty > 99999999)
                                    {
                                        result.Code = (int)TWNewEgg.API.Models.ResponseCode.Error;
                                        result.IsSuccess = false;
                                        result.Msg = "安全庫存量格式錯誤，最多為 99999999";
                                        break;
                                    }
                                }
                            }
                        }
                        #endregion 安全庫存量
                        #region 限量可售
                        if (item.Item.CanSaleLimitQty != null)
                        {
                            bool isInteger = System.Text.RegularExpressions.Regex.IsMatch(item.Item.CanSaleLimitQty.ToString(), @"^-?\d+$", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                            if (isInteger == false)
                            {
                                result.IsSuccess = false;
                                result.Msg = "限量可售格式錯誤";
                                break;
                            }
                            else
                            {
                                if (item.Item.CanSaleLimitQty < 0)
                                {
                                    result.IsSuccess = false;
                                    result.Msg = "限量可售不可小於 0";
                                    break;
                                }
                                else
                                {
                                    if (item.Item.CanSaleLimitQty > 99999999)
                                    {
                                        result.Code = (int)TWNewEgg.API.Models.ResponseCode.Error;
                                        result.IsSuccess = false;
                                        result.Msg = "限量可售格式錯誤，最多為 99999999";
                                        break;
                                    }
                                }
                            }
                        }
                        #endregion 賣場限量
                        #region 開始銷售日期(起)
                        if (DateTime.TryParse(item.Item.DateStart.ToString(), out dateTransform) == false)
                        {
                            result.IsSuccess = false;
                            result.Msg = "開始銷售日期(起)錯誤";
                            break;
                        }
                        #endregion 開始銷售日期(起)
                    }
                    #endregion propertySketchProperty
                    break;
                case "propertyListCheck":
                    foreach (var item in _itemstetch)
                    {
                        #region 可售數量
                        if (item.ItemStock.CanSaleQty != null)
                        {
                            bool isInteger = System.Text.RegularExpressions.Regex.IsMatch(item.ItemStock.CanSaleQty.ToString(), @"^-?\d+$", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                            //不是整數
                            if (isInteger == false)
                            {
                                result.Code = (int)TWNewEgg.API.Models.ResponseCode.Error;
                                result.IsSuccess = false;
                                result.Msg = "可售數量格式錯誤";
                                break;
                            }
                            else
                            {
                                if (item.ItemStock.CanSaleQty < 0)
                                {
                                    result.Code = (int)TWNewEgg.API.Models.ResponseCode.Error;
                                    result.IsSuccess = false;
                                    result.Msg = "可售數量不可小於 0";
                                    break;
                                }
                                else
                                {
                                    if (item.ItemStock.CanSaleQty > 99999999)
                                    {
                                        result.Code = (int)TWNewEgg.API.Models.ResponseCode.Error;
                                        result.IsSuccess = false;
                                        result.Msg = "可售數量，最多為 99999999";
                                        break;
                                    }
                                }

                            }
                        }
                        #endregion
                    }
                    break;
                case "sketchSelect":
                    break;
                default:
                    break;
            }
            return result;
        }
    }


}