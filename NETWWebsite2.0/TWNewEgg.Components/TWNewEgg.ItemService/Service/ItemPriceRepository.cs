using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using TWNewEgg.DB;
using TWNewEgg.DB.TWSQLDB.Models;
using TWNewEgg.DB.TWSQLDB.Models.ExtModels;
using TWNewEgg.DB.TWBACKENDDB.Models;
using TWNewEgg.ItemService.Models;
using System.IO;
using System.Data.SqlClient;
using System.Data.Entity.Infrastructure;
using System.Data.Objects;
using System.Diagnostics;

namespace TWNewEgg.ItemService.Service
{
    public class ItemPriceRepository : IItemPrice, IDisposable
    {
        private ItemDisplayPriceDBService ItemDisplayPriceDB = new ItemDisplayPriceDBService();
        private IItemService ItemData = new ItemServiceRepository();
        private TWSqlDBContext db = new TWSqlDBContext();
        private static int reGenerateFlag = 0; // 0: normal flag, 99: re-generate itemdisplayprice

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                this.db.Dispose();
                //this.db = null;
                this.ItemDisplayPriceDB.Dispose();
                //this.ItemDisplayPriceDB = null;
            }
        }


        /// <summary>
        /// Get the ItemDisplayPrice and update Item's pricecash
        /// </summary>
        /// <param name="itemIDs"></param>
        /// <returns></returns>
        public string UpdateItemPriceCashByIDs(List<int> itemIDs)
        {
            string message = string.Empty;
            Dictionary<int, ItemDisplayPrice> returnDisplayPrice = new Dictionary<int, ItemDisplayPrice>();
            List<ItemDisplayPrice> allItemDisplayPrices = new List<ItemDisplayPrice>();
            if (itemIDs.Count > 0)
            {
                itemIDs = itemIDs.Distinct().OrderBy(x => x).ToList();
                DateTime dateTimeNow = DateTime.UtcNow.AddHours(8);
                allItemDisplayPrices = this.ItemDisplayPriceDB.GetItemDisplayPriceByItemIDs(itemIDs, dateTimeNow, null, null);
            }

            for (int i = 0; i < itemIDs.Count; i++)
            {
                var aItemDisplay = allItemDisplayPrices.Where(x => x.ItemID == itemIDs[i]).OrderByDescending(x => x.PriceType).ThenBy(x => x.ID).FirstOrDefault();
                if (aItemDisplay != null)
                {
                    if (!returnDisplayPrice.ContainsKey(itemIDs[i]))
                    {
                        returnDisplayPrice.Add(itemIDs[i], aItemDisplay);
                    }
                }
            }

            foreach (var aReturnDisplayPrice in returnDisplayPrice)
            {
                message += this.ItemDisplayPriceDB.UpdateItemPriceCashAndProductCost(aReturnDisplayPrice.Key, aReturnDisplayPrice.Value);
            }
            return message;

        }



        /// <summary>
        /// Get ItemDisplayPrices by ItemIDs
        /// </summary>
        /// <param name="itemIDs"></param>
        /// <returns></returns>
        public Dictionary<int, ItemDisplayPrice> GetItemDisplayPriceByIDs(List<int> itemIDs)
        {
            Dictionary<int, ItemDisplayPrice> returnDisplayPrice = new Dictionary<int, ItemDisplayPrice>();
            List<ItemDisplayPrice> allItemDisplayPrices = new List<ItemDisplayPrice>();
            if (itemIDs.Count > 0)
            {
                itemIDs = itemIDs.Distinct().OrderBy(x => x).ToList();
                DateTime dateTimeNow = DateTime.UtcNow.AddHours(8);
                allItemDisplayPrices = this.ItemDisplayPriceDB.GetItemDisplayPriceByItemIDs(itemIDs, dateTimeNow, null, null);
            }

            for (int i = 0; i < itemIDs.Count; i++)
            {
                var aItemDisplay = allItemDisplayPrices.Where(x => x.ItemID == itemIDs[i]).OrderByDescending(x => x.PriceType).ThenBy(x => x.ID).FirstOrDefault();
                if (aItemDisplay != null)
                {
                    if (!returnDisplayPrice.ContainsKey(itemIDs[i]))
                    {
                        returnDisplayPrice.Add(itemIDs[i], aItemDisplay);
                    }
                }
            }

            return returnDisplayPrice;
        }


        /// <summary>
        /// Set Whole Site's Item by Delvtype, if Delvtype == 65535, then it's means Whole Items.
        /// </summary>
        /// <param name="delvType"></param>
        /// <returns></returns>
        public string SetWholeDBItemDisplayPrice(int delvType)
        {
            int takeNumber = 1000;
            List<int> allItemIDs = new List<int>();
            var allTempItem = this.ItemData.GetAllItem().Where(x => x.Status == 0).Distinct();

            if (delvType != 65535)
            {
                allTempItem = allTempItem.Where(x => x.DelvType == delvType);
            }

            allItemIDs = allTempItem.Select(x => x.ID).Distinct().OrderByDescending(x => x).ToList();
            var totalNumber = allItemIDs.Count;
            string message = string.Empty;
            for (int i = 0; i <= (totalNumber / takeNumber); i++)
            {
                var itemIDs = allItemIDs.Skip(i * takeNumber).Take(takeNumber).ToList();
                message += this.SetItemDisplayPriceByIDs(itemIDs);
                this.Dispose(true);
                this.ItemDisplayPriceDB = new ItemDisplayPriceDBService();
                this.db = new TWSqlDBContext();
#if DEBUG
                Debug.WriteLine((i * takeNumber).ToString() + "     " + takeNumber.ToString());
                Debug.WriteLine("1000 : " + DateTime.Now.ToString());
#endif
            }

            string path = AppDomain.CurrentDomain.BaseDirectory + "Log\\Product_Value\\";
            string writeStringend = string.Empty;
            writeStringend = string.Format(message) + Environment.NewLine;
            this.LogtoFileWrite(path, writeStringend);
            
            return message;
        }

        public string SetItemDisplayPriceByIDs(List<int> itemIDs)
        {
#if DEBUG
            Debug.WriteLine("SetItemDisplayPriceByIDs1 : " + DateTime.Now.ToString());
#endif
            string message = string.Empty;

            if (itemIDs.Count == 0)
            {
                message = "No Item ID.";
                return message;
            }
            var allItems = this.ItemData.GetItemsByIDs(itemIDs, 1);
            if (allItems.Count == 0)
            {
                message = "No Items in DB.";
                return message;
            }
            var allSellerIDs = allItems.Select(x => x.SellerID).Distinct().ToList();
            var allSeller = this.ItemData.GetSellersByIDs(allSellerIDs);
            if (allSeller.Count == 0)
            {
                message = "No Sellers in DB.";
                return message;
            }
#if DEBUG
            Debug.WriteLine("SetItemDisplayPriceByIDs2 : " + DateTime.Now.ToString());
#endif
            for (int i = 0; i < allItems.Count; i++)
            {
                decimal tax = 0.00M, shippingFee = 0.00M, taxandShippingFee = 0.00M, displayPrice = 0.00M, servicePrice = 0.00M;
                var aSeller = allSeller.Where(x => x.ID == allItems[i].SellerID).FirstOrDefault();
                if (aSeller == null)
                {
                    message += " [ Cann't find seller in DB when ItemID : " + allItems[i].ID.ToString() + " ] ";
                    continue;
                }
                var aProduct = this.ItemData.GetProductsByIDs(new List<int> { allItems[i].ProductID });
                if (aProduct.Count == 0)
                {
                    message += " [ Cann't find product in DB when ItemID : " + allItems[i].ID.ToString() + " ] ";
                    continue;
                }


                List<BuyingItems> PostData = new List<BuyingItems>();
                BuyingItems BuyingItemmodel = new BuyingItems();

                BuyingItemmodel.buyingNumber = 1;
                BuyingItemmodel.buyItemID = allItems[i].ID;
                BuyingItemmodel.buyItemID_DelvType = allItems[i].DelvType;
                BuyingItemmodel.buyItemID_Seller = allItems[i].SellerID;
                BuyingItemmodel.buyItemLists = new List<BuyingItemList>();

                PostData.Add(BuyingItemmodel);
                ShipTaxService shippingCosts = new ShipTaxService();
#if DEBUG
                Debug.WriteLine("SetItemDisplayPriceByIDs3 : " + DateTime.Now.ToString());
#endif
                try
                {
                    shippingCosts = this.GetShippingCosts(PostData, "index");
                }
                catch (Exception e)
                {
                    message += " [ weight is zero when ItemID : " + allItems[i].ID.ToString() + " ] ";
                    continue;
                }
#if DEBUG
                Debug.WriteLine("SetItemDisplayPriceByIDs4 : " + DateTime.Now.ToString());
#endif
                string v0 = shippingCosts.ShippingTaxCost.First().Value.First().pricetaxdetail.Split(',')[0]; // 原產地
                string v1 = shippingCosts.ShippingTaxCost.First().Value.First().pricetaxdetail.Split(',')[1]; // 台幣售價
                string v2 = shippingCosts.ShippingTaxCost.First().Value.First().pricetaxdetail.Split(',')[2]; // 稅賦
                string v3 = shippingCosts.ShippingTaxCost.First().Value.First().pricetaxdetail.Split(',')[3]; // 服務費
                string v4 = shippingCosts.ShippingTaxCost.First().Value.First().pricetaxdetail.Split(',')[4]; // 關稅
                string v5 = shippingCosts.ShippingTaxCost.First().Value.First().pricetaxdetail.Split(',')[5]; // VAT
                string v6 = shippingCosts.ShippingTaxCost.First().Value.First().pricetaxdetail.Split(',')[6]; // 貨物稅
                string v7 = shippingCosts.ShippingTaxCost.First().Value.First().pricetaxdetail.Split(',')[7]; // 推廣貿易服務費
                string v8 = shippingCosts.ShippingTaxCost.First().Value.First().pricetaxdetail.Split(',')[8]; // 運費

                tax = Math.Round(Convert.ToDecimal(v2), 0, MidpointRounding.AwayFromZero);
                servicePrice = Math.Round(Convert.ToDecimal(v3), 0, MidpointRounding.AwayFromZero);
                shippingFee = Math.Round(Convert.ToDecimal(v8), 0, MidpointRounding.AwayFromZero);


                switch (aSeller.CountryID)
                {
                    case 1:
                        switch (allItems[i].DelvType)
                        {
                            // case 0:
                            //    break;
                            case 1:
                                shippingFee = shippingFee + servicePrice;
                                displayPrice = allItems[i].PriceCash + shippingFee;
                                // taxandShippingFee = tax + shippingFee;
                                // 葉姊說只有不需商檢的商品才會是間配，所以不需多加此費用
                                //if ((displayPrice + tax - servicePrice) >= 29000)
                                //{

                                //    tax = tax + 600 + 1500;
                                //}

                                displayPrice = displayPrice + tax;
                                taxandShippingFee = shippingFee + tax;

                                break;
                            // case 2:
                            //    break;
                            case 3:
                                shippingFee = shippingFee + Math.Round(Convert.ToDecimal(v3), 0, MidpointRounding.AwayFromZero);
                                displayPrice = allItems[i].PriceCash + shippingFee;

                                if ((displayPrice + tax - servicePrice) >= 29000)
                                {
                                    tax = tax + 600 + 1500;
                                }

                                displayPrice = displayPrice + tax;
                                taxandShippingFee = shippingFee + tax;

                                break;
                            case 4:
                                tax = Math.Round((allItems[i].Taxfee ?? 0), 0, MidpointRounding.AwayFromZero);
                                shippingFee = Math.Round(allItems[i].PriceGlobalship, 0, MidpointRounding.AwayFromZero) + servicePrice;
                                displayPrice = allItems[i].PriceCash + shippingFee + tax;
                                taxandShippingFee = shippingFee;
                                break;
                            case 5:
                                displayPrice = allItems[i].PriceCash + shippingFee + tax;
                                taxandShippingFee = tax + shippingFee;

                                break;
                            case 6:
                                displayPrice = allItems[i].PriceCash + shippingFee + tax;
                                taxandShippingFee = tax + shippingFee;

                                break;
                            // case 7:
                            //    break;
                            // case 8:
                            //    break;
                            // case 9:
                            //    break;
                            // case 10:
                            //    break;
                            // case 11:
                            //    break;
                            // case 12:
                            //    break;
                            default:
                                displayPrice = allItems[i].PriceCash;
                                break;
                        }
                        break;
                    case 2:
                        switch (allItems[i].DelvType)
                        {
                            // case 0:
                            //    break;
                            case 1:
                                shippingFee = shippingFee + servicePrice;
                                displayPrice = allItems[i].PriceCash + shippingFee;
                                // taxandShippingFee = tax + shippingFee;
                                // 葉姊說只有不需商檢的商品才會是間配，所以不需多加此費用
                                //if ((displayPrice + tax - servicePrice) >= 29000)
                                //{

                                //    tax = tax + 600 + 1500;
                                //}

                                displayPrice = displayPrice + tax;
                                taxandShippingFee = shippingFee + tax;

                                break;
                            // case 2:
                            //    break;
                            case 3:
                                shippingFee = shippingFee + Math.Round(Convert.ToDecimal(v3), 0, MidpointRounding.AwayFromZero);
                                displayPrice = allItems[i].PriceCash + shippingFee;

                                if ((displayPrice + tax - servicePrice) >= 29000)
                                {
                                    tax = tax + 600 + 1500;
                                }

                                displayPrice = displayPrice + tax;
                                taxandShippingFee = shippingFee + tax;

                                break;
                            case 4:
                                tax = Math.Round((allItems[i].Taxfee ?? 0), 0, MidpointRounding.AwayFromZero);
                                shippingFee = Math.Round(allItems[i].PriceGlobalship, 0, MidpointRounding.AwayFromZero) + servicePrice;
                                displayPrice = allItems[i].PriceCash + shippingFee + tax;
                                taxandShippingFee = shippingFee;
                                break;
                            case 5:
                                displayPrice = allItems[i].PriceCash + shippingFee + tax;
                                taxandShippingFee = tax + shippingFee;

                                break;
                            case 6:
                                displayPrice = allItems[i].PriceCash + shippingFee + tax;
                                taxandShippingFee = tax + shippingFee;


                                break;
                            // case 7:
                            //    break;
                            // case 8:
                            //    break;
                            // case 9:
                            //    break;
                            // case 10:
                            //    break;
                            // case 11:
                            //    break;
                            // case 12:
                            //    break;
                            default:
                                displayPrice = allItems[i].PriceCash;
                                break;
                        }
                        break;
                    case 3:
                        switch (allItems[i].DelvType)
                        {
                            // case 0:
                            //    break;
                            case 1:
                                shippingFee = shippingFee + servicePrice;
                                displayPrice = allItems[i].PriceCash + shippingFee;
                                // taxandShippingFee = tax + shippingFee;
                                // 葉姊說只有不需商檢的商品才會是間配，所以不需多加此費用
                                //if ((displayPrice + tax - servicePrice) >= 29000)
                                //{

                                //    tax = tax + 600 + 1500;
                                //}

                                displayPrice = displayPrice + tax;
                                taxandShippingFee = shippingFee + tax;

                                break;
                            // case 2:
                            //    break;
                            case 3:
                                shippingFee = shippingFee + Math.Round(Convert.ToDecimal(v3), 0, MidpointRounding.AwayFromZero);
                                displayPrice = allItems[i].PriceCash + shippingFee;

                                if ((displayPrice + tax - servicePrice) >= 29000)
                                {
                                    tax = tax + 600 + 1500;
                                }

                                displayPrice = displayPrice + tax;
                                taxandShippingFee = shippingFee + tax;

                                break;
                            case 4:
                                tax = Math.Round((allItems[i].Taxfee ?? 0), 0, MidpointRounding.AwayFromZero);
                                shippingFee = Math.Round(allItems[i].PriceGlobalship, 0, MidpointRounding.AwayFromZero) + servicePrice;
                                displayPrice = allItems[i].PriceCash + shippingFee + tax;
                                taxandShippingFee = shippingFee;
                                break;
                            case 5:
                                displayPrice = allItems[i].PriceCash + shippingFee + tax;
                                taxandShippingFee = tax + shippingFee;

                                break;
                            case 6:
                                displayPrice = allItems[i].PriceCash + shippingFee + tax;
                                taxandShippingFee = tax + shippingFee;

                                break;
                            // case 7:
                            //    break;
                            // case 8:
                            //    break;
                            // case 9:
                            //    break;
                            // case 10:
                            //    break;
                            // case 11:
                            //    break;
                            // case 12:
                            //    break;
                            default:
                                displayPrice = allItems[i].PriceCash;
                                break;
                        }
                        break;
                    case 4:
                        switch (allItems[i].DelvType)
                        {
                            // case 0:
                            //    break;
                            case 1:
                                shippingFee = shippingFee + servicePrice;
                                displayPrice = allItems[i].PriceCash + shippingFee;
                                // taxandShippingFee = tax + shippingFee;
                                // 葉姊說只有不需商檢的商品才會是間配，所以不需多加此費用
                                //if ((displayPrice + tax - servicePrice) >= 29000)
                                //{

                                //    tax = tax + 600 + 1500;
                                //}

                                displayPrice = displayPrice + tax;
                                taxandShippingFee = shippingFee + tax;

                                break;
                            // case 2:
                            //    break;
                            case 3:
                                shippingFee = shippingFee + Math.Round(Convert.ToDecimal(v3), 0, MidpointRounding.AwayFromZero);
                                displayPrice = allItems[i].PriceCash + shippingFee;

                                if ((displayPrice + tax - servicePrice) >= 29000)
                                {
                                    tax = tax + 600 + 1500;
                                }

                                displayPrice = displayPrice + tax;
                                taxandShippingFee = shippingFee + tax;

                                break;
                            case 4:
                                tax = Math.Round((allItems[i].Taxfee ?? 0), 0, MidpointRounding.AwayFromZero);
                                shippingFee = Math.Round(allItems[i].PriceGlobalship, 0, MidpointRounding.AwayFromZero) + servicePrice;
                                displayPrice = allItems[i].PriceCash + shippingFee + tax;
                                taxandShippingFee = shippingFee;
                                break;
                            case 5:
                                displayPrice = allItems[i].PriceCash + shippingFee + tax;
                                taxandShippingFee = tax + shippingFee;

                                break;
                            case 6:
                                displayPrice = allItems[i].PriceCash + shippingFee + tax;
                                taxandShippingFee = tax + shippingFee;

                                break;
                            // case 7:
                            //    break;
                            // case 8:
                            //    break;
                            // case 9:
                            //    break;
                            // case 10:
                            //    break;
                            // case 11:
                            //    break;
                            // case 12:
                            //    break;
                            default:
                                displayPrice = allItems[i].PriceCash;
                                break;
                        }
                        break;
                    case 5:
                        switch (allItems[i].DelvType)
                        {
                            // case 0:
                            //    break;
                            case 1:
                                shippingFee = shippingFee + servicePrice;
                                displayPrice = allItems[i].PriceCash + shippingFee;
                                // taxandShippingFee = tax + shippingFee;
                                // 葉姊說只有不需商檢的商品才會是間配，所以不需多加此費用
                                //if ((displayPrice + tax - servicePrice) >= 29000)
                                //{

                                //    tax = tax + 600 + 1500;
                                //}

                                displayPrice = displayPrice + tax;
                                taxandShippingFee = shippingFee + tax;

                                break;
                            // case 2:
                            //    break;
                            case 3:
                                shippingFee = shippingFee + Math.Round(Convert.ToDecimal(v3), 0, MidpointRounding.AwayFromZero);
                                displayPrice = allItems[i].PriceCash + shippingFee;

                                if ((displayPrice + tax - servicePrice) >= 29000)
                                {
                                    tax = tax + 600 + 1500;
                                }

                                displayPrice = displayPrice + tax;
                                taxandShippingFee = shippingFee + tax;

                                break;
                            case 4:
                                tax = Math.Round((allItems[i].Taxfee ?? 0), 0, MidpointRounding.AwayFromZero);
                                shippingFee = Math.Round(allItems[i].PriceGlobalship, 0, MidpointRounding.AwayFromZero) + servicePrice;
                                displayPrice = allItems[i].PriceCash + shippingFee + tax;
                                taxandShippingFee = shippingFee;
                                break;
                            case 5:
                                displayPrice = allItems[i].PriceCash + shippingFee + tax;
                                taxandShippingFee = tax + shippingFee;

                                break;
                            case 6:
                                displayPrice = allItems[i].PriceCash + shippingFee + tax;
                                taxandShippingFee = tax + shippingFee;

                                break;
                            // case 7:
                            //    break;
                            // case 8:
                            //    break;
                            // case 9:
                            //    break;
                            // case 10:
                            //    break;
                            // case 11:
                            //    break;
                            // case 12:
                            //    break;
                            default:
                                displayPrice = allItems[i].PriceCash;
                                break;
                        }
                        break;
                }
                // Todo
                var dateTimeNow = DateTime.UtcNow.AddHours(8);
                // var dateTimeForever = DateTime.UtcNow.AddYears(100);
#if DEBUG
                Debug.WriteLine("SetItemDisplayPriceByIDs5 : " + DateTime.Now.ToString());
#endif
                var flag = this.ItemDisplayPriceDB.setItemDisplayPriceByID(allItems[i].ID, (int)ItemDisplayPrice.PriceTypeEnum.系統自動, 1, 1, dateTimeNow, dateTimeNow, displayPrice, tax, shippingFee, (aProduct.First().Cost ?? 0), this.getProductCost((aProduct.First().Cost ?? 0), taxandShippingFee, aSeller.CountryID.Value), null, dateTimeNow, "SYS");
#if DEBUG
                Debug.WriteLine("SetItemDisplayPriceByIDs6 : " + DateTime.Now.ToString());
#endif
                if (flag == 101)
                {
                    message += " [ Create failed when ItemID : " + allItems[i].ID.ToString() + " ] ";
                }
                if (flag == 102)
                {
                    message += " [ Update failed when ItemID : " + allItems[i].ID.ToString() + " ] ";
                }
            }



            // return flag;

            return message;

            throw new NotImplementedException();
        }

        private decimal getProductCost(decimal productCost, decimal taxandShippingFee, int countryID)
        {
            decimal reallyProductCost = new decimal();


            var aCurrency = this.ItemData.GetACurrency(countryID);

            reallyProductCost = productCost * ((aCurrency != null) ? aCurrency.AverageexchangeRate : 1.0M);
            reallyProductCost += taxandShippingFee;

            return reallyProductCost;
        }

        /// <summary>
        /// Get shipping cost and taxdetail, first is count by program, last is call from stored procedure , this is for local items
        /// </summary>
        /// <param name="PostData"></param>
        /// <returns></returns>
        public ShipTaxService GetShippingCosts(List<BuyingItems> PostData, string shoptype)
        {
            // TWSqlDBContext db = new TWSqlDBContext();
            // var itemProducts;
            ShipTaxService shipTaxService = new ShipTaxService();
            // List<BuyingItems> PostData = GetBuyingCartItems(buyingCartItems);
            Dictionary<string, decimal> shoppingCosts = new Dictionary<string, decimal>();
            Dictionary<string, List<GetItemTaxDetail>> itemTaxDetail = new Dictionary<string, List<GetItemTaxDetail>>();
            // List<BuyingItems> postData = new List<BuyingItems>();
            // postData = PostData;

            // List<string> itemSellers = new List<string>();
            // itemSellers = db.Item.Where(y => (PostData.Select(x => x.buyItemID).ToList()).Contains(y.item_id)).Select(z => z.item_id).Distinct().ToList();
            List<int> returnIDs = new List<int>();
            if (PostData == null || PostData.Count <= 0)
            {
                return shipTaxService;
            }
            var buyitemIDs = PostData.Select(x => x.buyItemID);
            if (buyitemIDs.Count() <= 0)
            {
                return shipTaxService;
            }
            var itemIDSellerID = PostData.Select(z => new { item_sellerid = z.buyItemID_Seller, item_id = z.buyItemID }).ToList();
            if (itemIDSellerID.Count <= 0)
            {
                return shipTaxService;
            }
            var sellerIDs = itemIDSellerID.Select(x => x.item_sellerid).Distinct().ToList();
            returnIDs = sellerIDs;
            foreach (var returnID in returnIDs)
            {
                List<string> itemforSP = new List<string>(); // this is feeding for stored procedure
                List<string> itemlistforSP = new List<string>(); // this is feeding for stored procedure
                List<string> priceforSP = new List<string>(); // this is feeding for stored procedure
                List<decimal> priceforSPTemp = new List<decimal>(); // this is feeding for stored procedure
                List<decimal> eachweight = new List<decimal>(); // this is stored each item or itemlist's weight
                List<int> eachItemNumber = new List<int>();
                List<decimal> eachItemShippingFee = new List<decimal>();

                decimal weight = new decimal();
                weight = 0;
                var sellerItems = itemIDSellerID.Where(x => x.item_sellerid == returnID).ToList();
                foreach (var sellerItem in sellerItems)
                {
                    var items = PostData.Where(x => x.buyItemID == sellerItem.item_id).FirstOrDefault();
                    int itemID;
                    if (items.item_AttrID != null && items.item_AttrID != 0)
                    {
                        itemID = items.item_AttrID.Value;

                        itemforSP.Add(items.buyItemID.ToString()); // set item id into string 
                        itemlistforSP.Add(items.item_AttrID.Value.ToString()); // set itemlist id into string, which had attribute

                        decimal tempWeight = this.GetItemWeight(itemID, items.buyingNumber, "Itemlist");
                        // 存partWeight
                        // BuyingItems tempPostData = postData.Where(x => x.item_AttrID == items.item_AttrID.Value).Select(x => x).FirstOrDefault();
                        // tempPostData.buyItemWeight = tempWeight;

                        weight += tempWeight;
                        eachweight.Add(tempWeight); // set weight into string
                    }
                    else
                    {
                        itemID = items.buyItemID;

                        itemforSP.Add(items.buyItemID.ToString()); // set item id into string
                        itemlistforSP.Add("0"); // set itemlist id into string, which had no attribute

                        decimal tempWeight = this.GetItemWeight(itemID, items.buyingNumber, "Item");
                        // 存partWeight
                        // BuyingItems tempPostData = postData.Where(x => x.buyItemID == items.buyItemID).Select(x => x).FirstOrDefault();
                        // tempPostData.buyItemWeight = tempWeight;

                        weight += tempWeight;
                        eachweight.Add(tempWeight); // set weight into string
                    }

                    foreach (var itemlists in items.buyItemLists)
                    {
                        int itemlistID;
                        if (itemlists.item_AttrID != null && itemlists.item_AttrID != 0)
                        {
                            itemlistID = itemlists.item_AttrID.Value;

                            itemlistforSP.Add(itemlists.item_AttrID.Value.ToString()); // set itemlist id into string, which had attribute

                        }
                        else
                        {
                            itemlistID = itemlists.buyItemlistID;

                            itemlistforSP.Add(itemlists.buyItemlistID.ToString()); // set itemlist id into string, which had no attribute
                        }

                        itemforSP.Add(items.buyItemID.ToString()); // set item id into string 

                        decimal tempWeight = this.GetItemWeight(itemlistID, itemlists.buyingNumber, "Itemlist");
                        // 存partWeight
                        // BuyingItemList tempPostData = postData.Where(x => x.buyItemID == items.buyItemID).Select(x => x).FirstOrDefault().buyItemLists.Where(y => y.buyItemlistID == itemlistID).Select(y => y).FirstOrDefault();
                        // tempPostData.buyItemListWeight = tempWeight;

                        weight += tempWeight;
                        eachweight.Add(tempWeight); // set weight into string

                    }

                }
                decimal totalItemShipping = this.GetItemShipping(weight, returnID, shoptype); // count total shipping cost
                shoppingCosts.Add(returnID.ToString(), totalItemShipping); // set seller with all seller's items

                int seller_id = sellerItems[0].item_sellerid;
                int? seller_country = this.db.Seller.Where(x => x.ID == seller_id).Select(x => x.CountryID).ToList().FirstOrDefault();
                // string seller_currencytype = db.Seller.Where(x => x.seller_id == seller_id).Select(x => x.seller_currencytype).FirstOrDefault();
                if (seller_country != 1)
                {
                    foreach (var itemweight in eachweight)
                    {
                        if (weight == 0)
                        {
                            throw new Exception("weight is zero!!!");
                        }
                        priceforSPTemp.Add(Convert.ToInt32(Math.Round(((totalItemShipping * itemweight) / weight), 0, MidpointRounding.AwayFromZero))); // count each item's shipping cost
                    }
                }
                else
                {
                    if (weight != 0)
                    {
                        foreach (var itemweight in eachweight)
                        {
                            if (weight == 0)
                            {
                                throw new Exception("weight is zero!!!");
                            }
                            priceforSPTemp.Add(Convert.ToInt32(Math.Round(((totalItemShipping * itemweight) / weight), 0, MidpointRounding.AwayFromZero))); // count each item's shipping cost
                        }
                    }
                    else
                    {
                        foreach (var itemweight in eachweight)
                        {
                            if (weight == 0)
                            {
                                throw new Exception("weight is zero!!!");
                            }
                            priceforSPTemp.Add(Convert.ToInt32(Math.Round(((totalItemShipping * itemweight) / 1), 0, MidpointRounding.AwayFromZero))); // count each item's shipping cost
                        }
                    }
                }
                decimal totalPriceforSP = new decimal();
                totalPriceforSP = 0;
                foreach (var eachPrice in priceforSPTemp)
                {
                    totalPriceforSP += eachPrice;
                }
                priceforSPTemp[0] += totalItemShipping - totalPriceforSP;

                foreach (var eachPrice in priceforSPTemp)
                {
                    priceforSP.Add(eachPrice.ToString());
                }

                var PostDatatemp = PostData.Where(x => x.buyItemID_Seller == seller_id).ToList();
                for (int i = 0, itemp = 0; i < priceforSP.Count; i++, itemp++)
                {
                    eachItemShippingFee.Add(Convert.ToDecimal(priceforSP[i]));
                    priceforSP[i] = Math.Round(Convert.ToDecimal(priceforSP[i]) / PostDatatemp[itemp].buyingNumber, 0, MidpointRounding.AwayFromZero).ToString();
                    eachItemNumber.Add(PostDatatemp[itemp].buyingNumber);
                    if (PostDatatemp[itemp].buyItemLists.Count > 0)
                    {
                        int y = 1;
                        for (y = 1; y <= PostDatatemp[itemp].buyItemLists.Count; y++)
                        {
                            // eachItemShippingFee.Add(Convert.ToDecimal(priceforSP[i + y])); Don't consider itemlist in the future
                            priceforSP[i + y] = Math.Round(Convert.ToDecimal(priceforSP[i + y]) / PostDatatemp[itemp].buyItemLists[y - 1].buyingNumber, 0, MidpointRounding.AwayFromZero).ToString();
                            // eachItemNumber.Add(PostDatatemp[itemp].buyItemLists[y - 1].buyingNumber); Don't consider itemlist in the future
                        }
                        i = i + y - 1;
                    }
                }

                /* Coz shipping cart don't need each shipping, just need one shipping, so don't need to count each item's shipping fee
                // count each item shipping plus item number, make sure each shipping fee is fit the total shipping fee
                totalPriceforSP = 0;
                foreach (var eachPrice in priceforSP)
                {
                    totalPriceforSP += Convert.ToDecimal(eachPrice) * PostDatatemp[0].buyingNumber;
                }

                priceforSP[0] = (Convert.ToDecimal(priceforSP[0]) + Math.Round((totalItemShipping - totalPriceforSP) / PostDatatemp[0].buyingNumber, 0, MidpointRounding.AwayFromZero)).ToString();
                */

                string taxItem = "", taxItemlist = "", taxShippingPrice = "";
                // combine string for stroed procedure
                for (int i = 0; i < itemforSP.Count; i++) 
                {
                    if (taxItem != "")
                    {
                        taxItem += ",";
                        taxItemlist += ",";
                        taxShippingPrice += ",";
                    }
                    if (itemlistforSP[i] == "0")
                    {
                        taxItem += itemforSP[i];
                    }
                    else
                    {
                        // taxItem += itemforSP[i];
                        taxItem += "0";
                    }
                    taxItemlist += itemlistforSP[i];
                    taxShippingPrice += priceforSP[i];
                }

                itemTaxDetail.Add(returnID.ToString(), this.GetItemTaxDetails(taxItem, taxItemlist, taxShippingPrice).ToList()); // call stored procedure and save taxdetail with seller id

                this.SetItemTaxDetailData(itemTaxDetail, returnID.ToString(), itemforSP, totalItemShipping, eachItemNumber, eachItemShippingFee, shoptype); // setting item taxdetail
                // GetItemTaxDetails("0,0,0", "73,74,110", "174,174,174");///////////////////////////////////
                
            }

            shipTaxService.ShippingCost = shoppingCosts; // set total shipping cost

            shipTaxService.ShippingTaxCost = itemTaxDetail; // set item tax detail

            // shipTaxService.PostData = postData;

            return shipTaxService;

        }

        /// <summary>
        /// count item and itemlist's length, width, and weight and compare with each weight, choose max one 
        /// </summary>
        /// <param name="itemIDs"></param>
        /// <param name="number"></param>
        /// <param name="table"></param>
        /// <returns></returns>
        public decimal GetItemWeight(int itemIDs, int number, string table)
        {
            decimal over5KG = decimal.Parse(System.Configuration.ConfigurationManager.AppSettings.Get("ProductWeightOver5KG"));
            decimal below5KG = decimal.Parse(System.Configuration.ConfigurationManager.AppSettings.Get("ProductWeightBelow5KG"));
            // TWSqlDBContext db = new TWSqlDBContext();
            // using (TWSqlDBContext db = new TWSqlDBContext())
            // {
            // Get itemID or itemlistID

            var itemData = this.db.Item.Where(x => x.ID == itemIDs).FirstOrDefault();
            if (itemData.DelvType == 6)
            {
                over5KG = 1.0M;
                below5KG = 1.0M;
            }
            int sellerid = itemData.SellerID;
            int countryID = (int)this.db.Seller.Where(x => x.ID == sellerid).FirstOrDefault().CountryID;

            if (countryID == 4)
            {
                // 沒有乘上數量
                if (sellerid == 5) 
                {
                    int ID = new int();
                    if (table == "Item")
                    {
                        ID = this.db.Item.Where(x => x.ID == itemIDs).FirstOrDefault().ProductID;
                    }
                    else if (table == "Itemlist")
                    {
                        ID = this.db.ItemList.Where(x => x.ID == itemIDs).FirstOrDefault().ItemlistProductID;
                    }
                    if (ID != null || ID != 0)
                    {
                        // Get Product's Volume
                        var volume = this.db.Product.Where(x => x.ID == ID).Select(y => new { product_length = y.Length, product_width = y.Width, product_height = y.Height, product_weight = y.Weight }).FirstOrDefault();

                        if (volume.product_length == null || volume.product_width == null || volume.product_height == null || volume.product_weight == null)
                        {
                            string path = AppDomain.CurrentDomain.BaseDirectory + "Log\\Product_Value\\";
                            string writeStringend = "";
                            writeStringend = string.Format("{0:yyyy/MM/dd HH:mm:ss}  Item_id:{1}　Product_id:{2}　table:{3}　材積有誤!!", DateTime.Now, itemIDs, ID, table) + Environment.NewLine;
                            this.LogtoFileWrite(path, writeStringend);
                        }

                        // Select which one is more heavy
                        decimal weight = (volume.product_length.Value * volume.product_width.Value * volume.product_height.Value) / 6000;

                        if (weight <= volume.product_weight.Value)
                        {
                            weight = volume.product_weight.Value;
                        }

                        // Set Weight 
                        return weight;
                    }
                    else
                    {
                        throw new ArgumentNullException("GetItemWeight ERROR");
                    }
                }
                else if (sellerid == 6)
                {
                    int ID = new int();
                    if (table == "Item")
                    {
                        ID = this.db.Item.Where(x => x.ID == itemIDs).FirstOrDefault().ProductID;
                    }
                    else if (table == "Itemlist")
                    {
                        ID = this.db.ItemList.Where(x => x.ID == itemIDs).FirstOrDefault().ItemlistProductID;
                    }
                    if (ID != null || ID != 0)
                    {
                        // Get Product's Volume
                        var volume = this.db.Product.Where(x => x.ID == ID).Select(y => new { product_length = y.Length, product_width = y.Width, product_height = y.Height, product_weight = y.Weight }).FirstOrDefault();

                        if (volume.product_length == null || volume.product_width == null || volume.product_height == null || volume.product_weight == null)
                        {
                            string path = AppDomain.CurrentDomain.BaseDirectory + "Log\\Product_Value\\";
                            string writeStringend = "";
                            writeStringend = string.Format("{0:yyyy/MM/dd HH:mm:ss}  Item_id:{1}　Product_id:{2}　table:{3}　材積有誤!!", DateTime.Now, itemIDs, ID, table) + Environment.NewLine;
                            this.LogtoFileWrite(path, writeStringend);
                        }

                        // Select which one is more heavy
                        decimal weight = (volume.product_length.Value * volume.product_width.Value * volume.product_height.Value) / 6000;

                        if (weight <= volume.product_weight.Value)
                        {
                            weight = volume.product_weight.Value;
                        }

                        // Set Weight * Number 
                        weight = weight * number;

                        return weight;
                    }
                    else
                    {
                        throw new ArgumentNullException("GetItemWeight ERROR");
                    }
                }
                else
                {
                    int ID = new int();
                    if (table == "Item")
                    {
                        ID = this.db.Item.Where(x => x.ID == itemIDs).FirstOrDefault().ProductID;
                    }
                    else if (table == "Itemlist")
                    {
                        ID = this.db.ItemList.Where(x => x.ID == itemIDs).FirstOrDefault().ItemlistProductID;
                    }
                    if (ID != null || ID != 0)
                    {
                        // Get Product's Volume
                        var volume = this.db.Product.Where(x => x.ID == ID).Select(y => new { product_length = y.Length, product_width = y.Width, product_height = y.Height, product_weight = y.Weight }).FirstOrDefault();

                        if (volume.product_length == null || volume.product_width == null || volume.product_height == null || volume.product_weight == null)
                        {
                            string path = AppDomain.CurrentDomain.BaseDirectory + "Log\\Product_Value\\";
                            string writeStringend = "";
                            writeStringend = string.Format("{0:yyyy/MM/dd HH:mm:ss}  Item_id:{1}　Product_id:{2}　table:{3}　材積有誤!!", DateTime.Now, itemIDs, ID, table) + Environment.NewLine;
                            this.LogtoFileWrite(path, writeStringend);
                        }

                        // Select which one is more heavy
                        decimal weight = (volume.product_length.Value * volume.product_width.Value * volume.product_height.Value) / 6000;

                        if (weight <= volume.product_weight.Value)
                        {
                            weight = volume.product_weight.Value;
                        }

                        // Set Weight * Number 
                        weight = weight * number;

                        return weight;
                    }
                    else
                    {
                        throw new ArgumentNullException("GetItemWeight ERROR");
                    }
                }
            }
            else 
            {
                int ID = new int();
                if (table == "Item")
                {
                    ID = this.db.Item.Where(x => x.ID == itemIDs).FirstOrDefault().ProductID;
                }
                else if (table == "Itemlist")
                {
                    ID = this.db.ItemList.Where(x => x.ID == itemIDs).FirstOrDefault().ItemlistProductID;
                }
                if (ID != null || ID != 0)
                {
                    // Get Product's Volume
                    var volume = this.db.Product.Where(x => x.ID == ID).Select(y => new { product_length = y.Length, product_width = y.Width, product_height = y.Height, product_weight = y.Weight }).FirstOrDefault();

                    if (volume.product_length == null || volume.product_width == null || volume.product_height == null || volume.product_weight == null)
                    {
                        string path = AppDomain.CurrentDomain.BaseDirectory + "Log\\Product_Value\\";
                        string writeStringend = "";
                        writeStringend = string.Format("{0:yyyy/MM/dd HH:mm:ss}  Item_id:{1}　Product_id:{2}　table:{3}　材積有誤!!", DateTime.Now, itemIDs, ID, table) + Environment.NewLine;
                        this.LogtoFileWrite(path, writeStringend);
                    }

                    // Select which one is more heavy
                    decimal weight = (volume.product_length.Value * volume.product_width.Value * volume.product_height.Value) / 5000;

                    if (weight <= volume.product_weight.Value)
                    {
                        weight = volume.product_weight.Value;
                    }

                    // Set Weight * Number 
                    weight = weight * number;

                    // Set Weight * 2 for shipping, coz the original product didn't package, so weight may need more heavy.
                    if (weight >= 5)
                    {
                        weight = weight * over5KG;
                    }
                    else
                    {
                        weight = weight * below5KG;
                    }

                    return weight;
                }
                else
                {
                    throw new ArgumentNullException("GetItemWeight ERROR");
                }
            }
            // }
        }
        /// <summary>
        /// acturally this function was changed all by Steven.S.Chen, so... XD  
        /// </summary>
        /// <param name="itemTaxDetail"></param>
        /// <param name="sellerID"></param>
        /// <param name="itemforSP"></param>
        /// <param name="totalItemShipping"></param>
        public void SetItemTaxDetailData(Dictionary<string, List<GetItemTaxDetail>> itemTaxDetail, string sellerID, List<string> itemforSP, decimal totalItemShipping, List<int> eachItemNumber, List<decimal> eachItemShippingFee, string shoptype = "index")
        {
            // TWSqlDBContext db = new TWSqlDBContext();
            int i = 0;
            foreach (var itemTax in itemTaxDetail[sellerID])
            {
                itemTax.item_id = int.Parse(itemforSP[i]);
                Item itemdetail = this.db.Item.Where(x => x.ID == itemTax.item_id).ToList().FirstOrDefault();

                ItemDisplayPrice aItemDisplayPrice = new ItemDisplayPrice();
                var getItemPrice = this.GetItemDisplayPriceByIDs(new List<int>() { itemdetail.ID });
                if (!getItemPrice.TryGetValue(itemdetail.ID, out aItemDisplayPrice))
                {
                    if (reGenerateFlag == 0)
                    {
                        reGenerateFlag = 99;
                        var generateMessage = this.SetItemDisplayPriceByIDs(new List<int>() { itemdetail.ID });
                        if (generateMessage == "")
                        {
                            getItemPrice = this.GetItemDisplayPriceByIDs(new List<int>() { itemdetail.ID });
                            if (!getItemPrice.TryGetValue(itemdetail.ID, out aItemDisplayPrice))
                            {
                                throw new ArgumentNullException("SetItemTaxDetailData ERROR, no ItemDisplayPrice");
                            }
                            reGenerateFlag = 0;
                        }
                        else
                        {
                            throw new ArgumentNullException(generateMessage);
                        }
                    }
                    else
                    {
                        aItemDisplayPrice = new ItemDisplayPrice();
                        aItemDisplayPrice.DisplayPrice = itemdetail.PriceCash + totalItemShipping;
                    }
                }

                List<decimal> allPrice = new List<decimal>();
                int seller_id = itemdetail.SellerID;
                int? seller_country = this.db.Seller.Where(x => x.ID == seller_id).Select(x => x.CountryID).ToList().FirstOrDefault();
                string seller_currencytype = this.db.Seller.Where(x => x.ID == seller_id).Select(x => x.CurrencyType).ToList().FirstOrDefault();

                string v0 = itemTax.pricetaxdetail.Split(',')[0]; // 原產地
                string v1 = itemTax.pricetaxdetail.Split(',')[1]; // 台幣售價
                string v2 = itemTax.pricetaxdetail.Split(',')[2]; // 稅賦
                string v3 = itemTax.pricetaxdetail.Split(',')[3]; // 服務費
                string v4 = itemTax.pricetaxdetail.Split(',')[4]; // 關稅
                string v5 = itemTax.pricetaxdetail.Split(',')[5]; // VAT
                string v6 = itemTax.pricetaxdetail.Split(',')[6]; // 貨物稅
                string v7 = itemTax.pricetaxdetail.Split(',')[7]; // 推廣貿易服務費
                string v9 = string.Empty; // 單個商品總價
                string v10 = string.Empty; // 單個商品總價 * 數量
                string v11 = string.Empty; // 折扣金額
                string v12 = string.Empty; // 實際購買金額


                if (seller_country == 1)
                {
                    if (itemdetail.DelvType != 1 && itemdetail.DelvType != 3 && itemdetail.DelvType != 4)
                    {
                        v3 = "0";
                    }
                    if (itemdetail.DelvType == 0)
                    {
                        itemTax.pricetaxdetail = "\"---\"" + "," + itemdetail.PriceCash + "," + v2 + "," + v3 + "," + v4 + "," + v5 + "," + v6 + "," + v7;
                        itemTax.pricetaxdetail += "," + totalItemShipping.ToString();

                        // Get DisplayPrice, Discount, RealPrice Start
                        allPrice = this.getTotalPriceAndRealPrice(seller_country.Value, itemdetail.DelvType, eachItemNumber[i], aItemDisplayPrice.DisplayPrice, itemdetail.PriceCash, v2, v3, eachItemShippingFee[i]);
                        v9 = allPrice[0].ToString();
                        v10 = allPrice[1].ToString();
                        v11 = allPrice[2].ToString();
                        v12 = allPrice[3].ToString();
                        itemTax.pricetaxdetail += "," + v9 + "," + v10 + "," + v11 + "," + v12;
                        // Get DisplayPrice, Discount, RealPrice End

                    }
                    else if (itemdetail.DelvType == 2)
                    {
                        itemTax.pricetaxdetail = "\"---\"" + "," + itemdetail.PriceCash + "," + v2 + "," + v3 + "," + v4 + "," + v5 + "," + v6 + "," + v7;
                        itemTax.pricetaxdetail += "," + totalItemShipping.ToString();

                        // Get DisplayPrice, Discount, RealPrice Start
                        allPrice = this.getTotalPriceAndRealPrice(seller_country.Value, itemdetail.DelvType, eachItemNumber[i], aItemDisplayPrice.DisplayPrice, itemdetail.PriceCash, v2, v3, eachItemShippingFee[i]);
                        v9 = allPrice[0].ToString();
                        v10 = allPrice[1].ToString();
                        v11 = allPrice[2].ToString();
                        v12 = allPrice[3].ToString();
                        itemTax.pricetaxdetail += "," + v9 + "," + v10 + "," + v11 + "," + v12;
                        // Get DisplayPrice, Discount, RealPrice End

                    }
                    else if (itemdetail.DelvType == 7)
                    {
                        itemTax.pricetaxdetail = "\"---\"" + "," + itemdetail.PriceCash + "," + v2 + "," + v3 + "," + v4 + "," + v5 + "," + v6 + "," + v7;
                        itemTax.pricetaxdetail += "," + totalItemShipping.ToString();

                        // Get DisplayPrice, Discount, RealPrice Start
                        allPrice = this.getTotalPriceAndRealPrice(seller_country.Value, itemdetail.DelvType, eachItemNumber[i], aItemDisplayPrice.DisplayPrice, itemdetail.PriceCash, v2, v3, eachItemShippingFee[i]);
                        v9 = allPrice[0].ToString();
                        v10 = allPrice[1].ToString();
                        v11 = allPrice[2].ToString();
                        v12 = allPrice[3].ToString();
                        itemTax.pricetaxdetail += "," + v9 + "," + v10 + "," + v11 + "," + v12;
                        // Get DisplayPrice, Discount, RealPrice End

                    }
                    else if (itemdetail.DelvType == 6)
                    {
                        v1 = ((int)Math.Floor((double)itemdetail.PriceCash + 0.6)).ToString(); // 台幣售價
                        v2 = "0";
                        if (itemdetail.Taxfee != null)
                        {
                            v2 = ((int)(itemdetail.Taxfee)).ToString(); // itemTax.pricetaxdetail.Split(',')[2]; // 稅賦
                        }
                        else
                        {
                            v2 = "0";
                        }
                        itemTax.pricetaxdetail = "\"---\"" + "," + v1 + "," + v2 + "," + v3 + "," + v4 + "," + v5 + "," + v6 + "," + v7;
                        if (itemdetail.PriceGlobalship != null)
                        {
                            itemTax.pricetaxdetail += "," + ((int)Math.Floor((double)itemdetail.PriceGlobalship + 0.6)).ToString();

                            // Get DisplayPrice, Discount, RealPrice Start
                            allPrice = this.getTotalPriceAndRealPrice(seller_country.Value, itemdetail.DelvType, eachItemNumber[i], aItemDisplayPrice.DisplayPrice, itemdetail.PriceCash, v2, v3, Math.Round(itemdetail.PriceGlobalship, 0, MidpointRounding.AwayFromZero));
                            v9 = allPrice[0].ToString();
                            v10 = allPrice[1].ToString();
                            v11 = allPrice[2].ToString();
                            v12 = allPrice[3].ToString();
                            itemTax.pricetaxdetail += "," + v9 + "," + v10 + "," + v11 + "," + v12;
                            // Get DisplayPrice, Discount, RealPrice End

                        }
                        else
                        {
                            itemTax.pricetaxdetail += "," + (0).ToString();

                            // Get DisplayPrice, Discount, RealPrice Start
                            allPrice = this.getTotalPriceAndRealPrice(seller_country.Value, itemdetail.DelvType, eachItemNumber[i], aItemDisplayPrice.DisplayPrice, itemdetail.PriceCash, v2, v3, Math.Round(0.00M, 0, MidpointRounding.AwayFromZero));
                            v9 = allPrice[0].ToString();
                            v10 = allPrice[1].ToString();
                            v11 = allPrice[2].ToString();
                            v12 = allPrice[3].ToString();
                            itemTax.pricetaxdetail += "," + v9 + "," + v10 + "," + v11 + "," + v12;
                            // Get DisplayPrice, Discount, RealPrice End
                        }
                    }
                    else
                    {
                        v1 = (itemdetail.PriceCash + itemdetail.PriceGlobalship + itemdetail.Taxfee).ToString();
                        itemTax.pricetaxdetail = "\"---\"" + "," + itemdetail.PriceCash + "," + v2 + "," + v3 + "," + v4 + "," + v5 + "," + v6 + "," + v7;
                        itemTax.pricetaxdetail += "," + totalItemShipping.ToString();

                        // Get DisplayPrice, Discount, RealPrice Start
                        allPrice = this.getTotalPriceAndRealPrice(seller_country.Value, itemdetail.DelvType, eachItemNumber[i], aItemDisplayPrice.DisplayPrice, itemdetail.PriceCash, v2, v3, eachItemShippingFee[i]);
                        v9 = allPrice[0].ToString();
                        v10 = allPrice[1].ToString();
                        v11 = allPrice[2].ToString();
                        v12 = allPrice[3].ToString();
                        itemTax.pricetaxdetail += "," + v9 + "," + v10 + "," + v11 + "," + v12;
                        // Get DisplayPrice, Discount, RealPrice End
                    }
                }
                else if (seller_country == 2) 
                {
                    // 美國
                    if (itemdetail.DelvType != 1 && itemdetail.DelvType != 3 && itemdetail.DelvType != 4)
                    {
                        v3 = "0";
                    }
                    if (itemdetail.DelvType == 6)
                    {
                        v1 = ((int)Math.Floor((double)itemdetail.PriceCash + 0.6)).ToString(); // 台幣售價
                        v2 = "0";
                        if (itemdetail.Taxfee != null)
                        {
                            v2 = ((int)(itemdetail.Taxfee)).ToString(); // itemTax.pricetaxdetail.Split(',')[2]; // 稅賦
                        }
                        else
                        {
                            v2 = "0";
                        }
                        itemTax.pricetaxdetail = "\"---\"" + "," + v1 + "," + v2 + "," + v3 + "," + v4 + "," + v5 + "," + v6 + "," + v7;
                        if (itemdetail.PriceGlobalship != null)
                        {
                            itemTax.pricetaxdetail += "," + ((int)Math.Floor((double)itemdetail.PriceGlobalship + 0.6)).ToString();

                            // Get DisplayPrice, Discount, RealPrice Start
                            allPrice = this.getTotalPriceAndRealPrice(seller_country.Value, itemdetail.DelvType, eachItemNumber[i], aItemDisplayPrice.DisplayPrice, itemdetail.PriceCash, v2, v3, Math.Round(itemdetail.PriceGlobalship, 0, MidpointRounding.AwayFromZero));
                            v9 = allPrice[0].ToString();
                            v10 = allPrice[1].ToString();
                            v11 = allPrice[2].ToString();
                            v12 = allPrice[3].ToString();
                            itemTax.pricetaxdetail += "," + v9 + "," + v10 + "," + v11 + "," + v12;
                            // Get DisplayPrice, Discount, RealPrice End
                        }
                        else
                        {
                            itemTax.pricetaxdetail += "," + (0).ToString();

                            // Get DisplayPrice, Discount, RealPrice Start
                            allPrice = this.getTotalPriceAndRealPrice(seller_country.Value, itemdetail.DelvType, eachItemNumber[i], aItemDisplayPrice.DisplayPrice, itemdetail.PriceCash, v2, v3, Math.Round(0.00M, 0, MidpointRounding.AwayFromZero));
                            v9 = allPrice[0].ToString();
                            v10 = allPrice[1].ToString();
                            v11 = allPrice[2].ToString();
                            v12 = allPrice[3].ToString();
                            itemTax.pricetaxdetail += "," + v9 + "," + v10 + "," + v11 + "," + v12;
                            // Get DisplayPrice, Discount, RealPrice End
                        }
                    }
                    else
                    {
                        itemTax.pricetaxdetail = "\"" + seller_currencytype + " " + v0 + "\"," + v1 + "," + v2 + "," + v3 + "," + v4 + "," + v5 + "," + v6 + "," + v7;
                        itemTax.pricetaxdetail += "," + totalItemShipping.ToString();

                        // Get DisplayPrice, Discount, RealPrice Start
                        allPrice = this.getTotalPriceAndRealPrice(seller_country.Value, itemdetail.DelvType, eachItemNumber[i], aItemDisplayPrice.DisplayPrice, itemdetail.PriceCash, v2, v3, eachItemShippingFee[i]);
                        v9 = allPrice[0].ToString();
                        v10 = allPrice[1].ToString();
                        v11 = allPrice[2].ToString();
                        v12 = allPrice[3].ToString();
                        itemTax.pricetaxdetail += "," + v9 + "," + v10 + "," + v11 + "," + v12;
                        // Get DisplayPrice, Discount, RealPrice End
                    }
                }
                else if (seller_country == 4) 
                {
                    // 中國大陸
                    if (itemdetail.DelvType != 1 && itemdetail.DelvType != 3 && itemdetail.DelvType != 4)
                    {
                        v3 = "0";
                    }
                    v2 = "0";
                    if (itemdetail.Taxfee != null)
                    {
                        v2 = ((int)(itemdetail.Taxfee)).ToString(); // itemTax.pricetaxdetail.Split(',')[2]; // 稅賦
                    }
                    else
                    {
                        v2 = "0";
                    }
                    if (itemdetail.DelvType == 6)
                    {
                        v1 = ((int)Math.Floor((double)itemdetail.PriceCash + 0.6)).ToString(); // 台幣售價
                        // v2 = "0";
                        // if (itemdetail.Taxfee != null)
                        // {
                        //     v2 = ((int)(itemdetail.Taxfee)).ToString();//itemTax.pricetaxdetail.Split(',')[2];//稅賦
                        // }
                        // else
                        // {
                        //     v2 = "0";
                        // }
                        itemTax.pricetaxdetail = "\"---\"" + "," + v1 + "," + v2 + "," + v3 + "," + v4 + "," + v5 + "," + v6 + "," + v7;
                        if (itemdetail.PriceGlobalship != null)
                        {
                            itemTax.pricetaxdetail += "," + ((int)Math.Floor((double)itemdetail.PriceGlobalship + 0.6)).ToString();

                            // Get DisplayPrice, Discount, RealPrice Start
                            allPrice = this.getTotalPriceAndRealPrice(seller_country.Value, itemdetail.DelvType, eachItemNumber[i], aItemDisplayPrice.DisplayPrice, itemdetail.PriceCash, v2, v3, Math.Round((itemdetail.PriceGlobalship * eachItemNumber[i]), 0, MidpointRounding.AwayFromZero));
                            v9 = allPrice[0].ToString();
                            v10 = allPrice[1].ToString();
                            v11 = allPrice[2].ToString();
                            v12 = allPrice[3].ToString();
                            itemTax.pricetaxdetail += "," + v9 + "," + v10 + "," + v11 + "," + v12;
                            // Get DisplayPrice, Discount, RealPrice End
                        }
                        else
                        {
                            itemTax.pricetaxdetail += "," + (0).ToString();

                            // Get DisplayPrice, Discount, RealPrice Start
                            allPrice = this.getTotalPriceAndRealPrice(seller_country.Value, itemdetail.DelvType, eachItemNumber[i], aItemDisplayPrice.DisplayPrice, itemdetail.PriceCash, v2, v3, Math.Round((0.00M * eachItemNumber[i]), 0, MidpointRounding.AwayFromZero));
                            v9 = allPrice[0].ToString();
                            v10 = allPrice[1].ToString();
                            v11 = allPrice[2].ToString();
                            v12 = allPrice[3].ToString();
                            itemTax.pricetaxdetail += "," + v9 + "," + v10 + "," + v11 + "," + v12;
                            // Get DisplayPrice, Discount, RealPrice End
                        }
                    }
                    if (shoptype == "index")
                    {
                        if (itemdetail.DelvType == 4)
                        {
                            if (itemdetail.PriceGlobalship != null)
                            {
                                v1 = ((int)(itemdetail.PriceCash + itemdetail.PriceGlobalship)).ToString();
                            }
                            if (itemdetail.Taxfee != null)
                            {
                                v1 = ((int)(itemdetail.PriceCash + itemdetail.Taxfee)).ToString();
                            }
                        }
                        itemTax.pricetaxdetail = "\"---\"" + "," + v1 + "," + "0" + "," + v3 + "," + "0" + "," + "0" + "," + "0" + "," + "0";
                        itemTax.pricetaxdetail += "," + totalItemShipping.ToString();

                        // Get DisplayPrice, Discount, RealPrice Start
                        allPrice = this.getTotalPriceAndRealPrice(seller_country.Value, itemdetail.DelvType, eachItemNumber[i], aItemDisplayPrice.DisplayPrice, itemdetail.PriceCash, v2, v3, Math.Round((itemdetail.PriceGlobalship * eachItemNumber[i]), 0, MidpointRounding.AwayFromZero));
                        v9 = allPrice[0].ToString();
                        v10 = allPrice[1].ToString();
                        v11 = allPrice[2].ToString();
                        v12 = allPrice[3].ToString();
                        itemTax.pricetaxdetail += "," + v9 + "," + v10 + "," + v11 + "," + v12;
                        // Get DisplayPrice, Discount, RealPrice End
                    }
                    else
                    {
                        itemTax.pricetaxdetail = "\"" + seller_currencytype + " " + v0 + "\"," + v1 + "," + v2 + "," + v3 + "," + v4 + "," + v5 + "," + v6 + "," + v7;
                        itemTax.pricetaxdetail += "," + totalItemShipping.ToString();

                        // Get DisplayPrice, Discount, RealPrice Start
                        allPrice = this.getTotalPriceAndRealPrice(seller_country.Value, itemdetail.DelvType, eachItemNumber[i], aItemDisplayPrice.DisplayPrice, itemdetail.PriceCash, v2, v3, eachItemShippingFee[i]);
                        v9 = allPrice[0].ToString();
                        v10 = allPrice[1].ToString();
                        v11 = allPrice[2].ToString();
                        v12 = allPrice[3].ToString();
                        itemTax.pricetaxdetail += "," + v9 + "," + v10 + "," + v11 + "," + v12;
                        // Get DisplayPrice, Discount, RealPrice End
                    }
                }
                else
                {
                    if (itemdetail.DelvType != 1 && itemdetail.DelvType != 3 && itemdetail.DelvType != 4)
                    {
                        v3 = "0";
                    }
                    if (itemdetail.DelvType == 6)
                    {
                        v1 = ((int)Math.Floor((double)itemdetail.PriceCash + 0.6)).ToString(); // 台幣售價
                        v2 = "0";
                        if (itemdetail.Taxfee != null)
                        {
                            v2 = ((int)(itemdetail.Taxfee)).ToString(); // itemTax.pricetaxdetail.Split(',')[2]; // 稅賦
                        }
                        else
                        {
                            v2 = "0";
                        }
                        itemTax.pricetaxdetail = "\"---\"" + "," + v1 + "," + v2 + "," + v3 + "," + v4 + "," + v5 + "," + v6 + "," + v7;
                        if (itemdetail.PriceGlobalship != null)
                        {
                            itemTax.pricetaxdetail += "," + ((int)Math.Floor((double)itemdetail.PriceGlobalship + 0.6)).ToString();

                            // Get DisplayPrice, Discount, RealPrice Start
                            allPrice = this.getTotalPriceAndRealPrice(seller_country.Value, itemdetail.DelvType, eachItemNumber[i], aItemDisplayPrice.DisplayPrice, itemdetail.PriceCash, v2, v3, Math.Round(itemdetail.PriceGlobalship, 0, MidpointRounding.AwayFromZero));
                            v9 = allPrice[0].ToString();
                            v10 = allPrice[1].ToString();
                            v11 = allPrice[2].ToString();
                            v12 = allPrice[3].ToString();
                            itemTax.pricetaxdetail += "," + v9 + "," + v10 + "," + v11 + "," + v12;
                            // Get DisplayPrice, Discount, RealPrice End
                        }
                        else
                        {
                            itemTax.pricetaxdetail += "," + (0).ToString();

                            // Get DisplayPrice, Discount, RealPrice Start
                            allPrice = this.getTotalPriceAndRealPrice(seller_country.Value, itemdetail.DelvType, eachItemNumber[i], aItemDisplayPrice.DisplayPrice, itemdetail.PriceCash, v2, v3, Math.Round(0.00M, 0, MidpointRounding.AwayFromZero));
                            v9 = allPrice[0].ToString();
                            v10 = allPrice[1].ToString();
                            v11 = allPrice[2].ToString();
                            v12 = allPrice[3].ToString();
                            itemTax.pricetaxdetail += "," + v9 + "," + v10 + "," + v11 + "," + v12;
                            // Get DisplayPrice, Discount, RealPrice End
                        }
                    }
                    else
                    {
                        itemTax.pricetaxdetail = "\"" + seller_currencytype + " " + v0 + "\"," + v1 + "," + v2 + "," + v3 + "," + v4 + "," + v5 + "," + v6 + "," + v7;
                        itemTax.pricetaxdetail += "," + totalItemShipping.ToString();

                        // Get DisplayPrice, Discount, RealPrice Start
                        allPrice = this.getTotalPriceAndRealPrice(seller_country.Value, itemdetail.DelvType, eachItemNumber[i], aItemDisplayPrice.DisplayPrice, itemdetail.PriceCash, v2, v3, eachItemShippingFee[i]);
                        v9 = allPrice[0].ToString();
                        v10 = allPrice[1].ToString();
                        v11 = allPrice[2].ToString();
                        v12 = allPrice[3].ToString();
                        itemTax.pricetaxdetail += "," + v9 + "," + v10 + "," + v11 + "," + v12;
                        // Get DisplayPrice, Discount, RealPrice End
                    }
                }

                i++;
            }

        }
        /// <summary>
        /// Get item's displayPrice and discount.
        /// </summary>
        /// <param name="itemNumber">Buying number</param>
        /// <param name="displayPrice">DisplayPrice from itemdisplayprice table</param>
        /// <param name="tax">tax value from stored procedure</param>
        /// <param name="serviveFee">service value from stored procedure</param>
        /// <param name="eachShippingFee">item shipping fee</param>
        /// <returns>Return an decimal array. First: Single DisplayPrice. Second: DisplayPrice plus buyingnumber. Third: Discount. Fouth: RealPrice that user buying.</returns>
        private List<decimal> getTotalPriceAndRealPrice(int countryID, int delvType, int itemNumber, decimal displayPrice, decimal priceCash, string tax, string serviveFee, decimal eachShippingFee)
        {
            List<decimal> allPrice = new List<decimal>();

            allPrice.Add(displayPrice);

            var totalPrice = displayPrice * itemNumber;
            allPrice.Add(totalPrice);

            decimal taxDeciaml = new decimal();
            if (!decimal.TryParse(tax, out taxDeciaml))
            {
                throw new ArgumentNullException("Tax tryparse failed.");
            }
            var totalTax = taxDeciaml * itemNumber;

            decimal serviceFeeDeciaml = new decimal();
            if (!decimal.TryParse(serviveFee, out serviceFeeDeciaml))
            {
                throw new ArgumentNullException("ServiceFee tryparse failed.");
            }
            var totalServiceFee = serviceFeeDeciaml * itemNumber;
            var inspectionFee = this.GetInspectionFee(countryID, delvType, priceCash, taxDeciaml, serviceFeeDeciaml, eachShippingFee);

            var realPrice = (priceCash * itemNumber) + eachShippingFee + totalTax + totalServiceFee + inspectionFee;
            var discount = totalPrice - realPrice;

            if (discount < 0)
            {
                discount = 0;
            }

            allPrice.Add(discount);
            allPrice.Add(realPrice);

            return allPrice;
        }
        /// <summary>
        /// Get Inspection Fee, if price oven than 29000 TWD (1000 USD) then plus 2100 TWD.
        /// </summary>
        /// <param name="countryID"></param>
        /// <param name="delvType"></param>
        /// <param name="priceCash"></param>
        /// <param name="tax"></param>
        /// <param name="shippingFee"></param>
        /// <returns></returns>
        private decimal GetInspectionFee(int countryID, int delvType, decimal priceCash, decimal tax, decimal serviceFee, decimal shippingFee)
        {
            decimal inspectionFee = 0.0m;
            //decimal totalPrice = priceCash + tax + serviceFee + shippingFee;
            decimal totalPrice = priceCash + tax + shippingFee;
            if (totalPrice < 29000)
            {
                return inspectionFee;
            }
            switch (countryID)
            {
                case 2:
                    // 葉姊說只有不需商檢的商品才會是間配，所以不需多加此費用
                    if (delvType == 3)// || delvType == 1)
                    {
                        inspectionFee = 600 + 1500;
                    }
                    else
                    {
                        inspectionFee = 0;
                    }
                    break;
                default:
                    inspectionFee = 0;
                    break;
            }
            return inspectionFee;
        }
        /// <summary>
        /// detemer weight and get shipping cost from db
        /// </summary>
        /// <param name="weight"></param>
        /// <param name="sellerID"></param>
        /// <returns></returns>
        public decimal GetItemShipping(decimal weight, int sellerID, string shoptype = "index")
        {
            // TWSqlDBContext db = new TWSqlDBContext();
            // using (TWSqlDBContext db = new TWSqlDBContext())
            // {
            // decimal newWeight = (weight / 1000);
            decimal newWeight = weight; // The unit in DB is kg. not g.
            decimal shippingCosts = new decimal();
            decimal? fulcharge = new decimal();
            // fulcharge = db.Logistic.Where(x => x.SellerID == sellerID).Select(y => y.FulCharge).FirstOrDefault() + 1;
            int CountryID = (int)this.db.Seller.Where(x => x.ID == sellerID).FirstOrDefault().CountryID;
            if (fulcharge == null || fulcharge < 1)
            {
                fulcharge = 1;
            }
            if (sellerID == 1) 
            {
                // This is for Taiwan's shipping, if the item is in taiwan so it don't need shipping cost.
                fulcharge = 0;
            }

            if (CountryID == 1)
            {
                return 0;
            }
            else if (sellerID == 5)
            {
                if (shoptype == "cart")
                {
                    if (newWeight == 0)
                    {
                        shippingCosts = 0;
                        return Math.Round(shippingCosts);
                    }

                    int over1kg = 0;
                    decimal first1kg = this.db.Logistic.Where(x => x.SellerID == sellerID).FirstOrDefault().Expense;
                    decimal currency = this.db.Currency.Where(x => x.CountryID == 4).OrderByDescending(x => x.CreateDate).FirstOrDefault().AverageexchangeRate;
                    if (sellerID == 5)
                    {
                        over1kg = int.Parse(System.Configuration.ConfigurationManager.AppSettings.Get("JieMaiProductWeightOverPerkg"));
                    }
                    else if (sellerID == 6)
                    {
                        over1kg = int.Parse(System.Configuration.ConfigurationManager.AppSettings.Get("NeweggCNProductWeightOverPerkg"));
                    }

                    if (newWeight == 0)
                    {
                        shippingCosts = 0;
                        return Math.Round(shippingCosts * currency, 0, MidpointRounding.AwayFromZero);
                    }
                    else if (newWeight <= 1)
                    {
                        newWeight = Math.Ceiling(newWeight);
                        shippingCosts = first1kg;

                        return Math.Round(shippingCosts * currency, 0, MidpointRounding.AwayFromZero);
                    }
                    else
                    {
                        newWeight = Math.Ceiling(newWeight);
                        shippingCosts = first1kg + (newWeight - 1) * over1kg;
                        return Math.Round(shippingCosts * currency, 0, MidpointRounding.AwayFromZero);
                    }
                }
                else
                {
                    return 0;
                }
            }
            else if (sellerID == 6)
            {
                if (newWeight == 0)
                {
                    shippingCosts = 0;
                    return Math.Round(shippingCosts);
                }

                int over1kg = 0;
                decimal first1kg = this.db.Logistic.Where(x => x.SellerID == sellerID).FirstOrDefault().Expense;
                decimal currency = this.db.Currency.Where(x => x.CountryID == 4).OrderByDescending(x => x.CreateDate).FirstOrDefault().AverageexchangeRate;
                if (sellerID == 5)
                {
                    over1kg = int.Parse(System.Configuration.ConfigurationManager.AppSettings.Get("JieMaiProductWeightOverPerkg"));
                }
                else if (sellerID == 6)
                {
                    over1kg = int.Parse(System.Configuration.ConfigurationManager.AppSettings.Get("NeweggCNProductWeightOverPerkg"));
                }

                if (newWeight == 0)
                {
                    shippingCosts = 0;
                    return Math.Round(shippingCosts * currency, 0, MidpointRounding.AwayFromZero);
                }
                else if (newWeight <= 1)
                {
                    newWeight = Math.Ceiling(newWeight);
                    shippingCosts = first1kg;

                    return Math.Round(shippingCosts * currency, 0, MidpointRounding.AwayFromZero);
                }
                else
                {
                    newWeight = Math.Ceiling(newWeight);
                    shippingCosts = first1kg + (newWeight - 1) * over1kg;
                    return Math.Round(shippingCosts * currency, 0, MidpointRounding.AwayFromZero);
                }
            }
            else
            {
                if (newWeight == 0)
                {
                    shippingCosts = 0;
                    return Math.Round(shippingCosts * 0, 0, MidpointRounding.AwayFromZero);
                }
                else if (newWeight <= 99 && newWeight > 0) 
                {
                    // if weight over 0kg and below 21kg then get shipping cost form db
                    Logistic Logisticitem = new Logistic();
                    // Logisticitem = db.Logistic.Where(x => x.Weight > newWeight && x.Weight <= (newWeight + 0.5M) && x.SellerID == sellerID).FirstOrDefault();
                    Logisticitem = this.db.Logistic.Where(x => x.Weight >= newWeight && x.SellerID == sellerID).OrderBy(x => x.Weight).FirstOrDefault();
                    shippingCosts = Logisticitem.Expense;
                    fulcharge = Logisticitem.FulCharge;
                    // shippingCosts = db.Logistic.Where(x => x.Weight > newWeight && x.Weight <= (newWeight + 0.5M) && x.SellerID == sellerID).Select(y => y.Expense).FirstOrDefault();
                    if (shippingCosts == null || shippingCosts == 0)
                    {
                        if ((newWeight + 0.5M) > 99M)
                        {
                            shippingCosts = 170 * Math.Ceiling(newWeight);
                        }
                        else
                        {
                            shippingCosts = 0;
                            string path = AppDomain.CurrentDomain.BaseDirectory + "Log\\Product_Value\\";
                            string writeStringend = "";
                            writeStringend = string.Format("{0:yyyy/MM/dd HH:mm:ss}  運費有誤!!", DateTime.Now) + Environment.NewLine;
                            this.LogtoFileWrite(path, writeStringend);
                        }
                    }
                    if (fulcharge == null) { fulcharge = 1; }

                    // return Math.Ceiling(shippingCosts * fulcharge.Value);
                    return Math.Round(shippingCosts * (fulcharge.Value + 1), 0, MidpointRounding.AwayFromZero);
                }
                /*else if (newWeight < 45 && newWeight >= 21)  // if weight over 21kg and below 45kg then each kg plus 170
                {
                    shippingCosts = 170 * (Math.Round(newWeight, 2, MidpointRounding.AwayFromZero));
                    return Math.Round(shippingCosts * fulcharge.Value, 0, MidpointRounding.AwayFromZero);
                }*/
                // else if (newWeight < 71 && newWeight >= 45) // if weight over 45kg and below 71kg then each kg plus 163
                else if (newWeight > 99) 
                {
                    // if weight over 45kg then each kg plus 163
                    shippingCosts = 163 * Math.Ceiling(newWeight);
                    fulcharge = 1.18M;
                    return Math.Round(shippingCosts * fulcharge.Value, 0, MidpointRounding.AwayFromZero);
                }
                else
                {
                    throw new Exception("Too Light");
                }
            }
            // }
        }
        /// <summary>
        /// call stored procedure to get item tax detail
        /// </summary>
        /// <param name="itemid"></param>
        /// <param name="itemlistid"></param>
        /// <param name="shippingpricelist"></param>
        /// <returns></returns>
        public IEnumerable<GetItemTaxDetail> GetItemTaxDetails(string itemid, string itemlistid, string shippingpricelist)
        {
            // List<GetItemTaxDetail> ItemTaxDetail = new List<GetItemTaxDetail>();
            // DatabaseContext db_SP = new DatabaseContext();
            using (var db_SP = new TWSqlDBContext())
            {
#if DEBUG
                Debug.WriteLine("GetItemTaxDetails1 : " + DateTime.Now.ToString());
#endif
                db_SP.Database.Initialize(force: false);
                var cmd = db_SP.Database.Connection.CreateCommand();



                SqlParameter paramOne = new SqlParameter();
                paramOne.ParameterName = "@itemid";
                paramOne.Value = itemid;
                cmd.Parameters.Add(paramOne);

                SqlParameter paramTwo = new SqlParameter();
                paramTwo.ParameterName = "@itemlistid";
                paramTwo.Value = itemlistid;
                cmd.Parameters.Add(paramTwo);

                SqlParameter paramThree = new SqlParameter();
                paramThree.ParameterName = "@shippingpricelist";
                paramThree.Value = shippingpricelist;
                cmd.Parameters.Add(paramThree);
                // shippingpricelist
                cmd.CommandText = "[dbo].[UP_EC_GetItemTaxDetail] @itemid, @itemlistid, @shippingpricelist";

                try
                {
                    db_SP.Database.Connection.Open();
                    var reader = cmd.ExecuteReader();

                    IEnumerable<GetItemTaxDetail> ItemTaxDetail = ((IObjectContextAdapter)db_SP).ObjectContext.Translate<GetItemTaxDetail>(reader, "GetItemTaxDetail", MergeOption.NoTracking).ToList();


#if DEBUG
                    Debug.WriteLine("GetItemTaxDetails2 : " + DateTime.Now.ToString());
#endif
                    return ItemTaxDetail;

                }
                catch (Exception e)
                {
                    return null;
                }
            }

        }
        private void LogtoFileWrite(string path, string writeStringendtoFile)
        {

            string filename = path + string.Format("\\{0:yyyy}\\{0:MM}\\{0:yyyy-MM-dd}.txt", DateTime.Now);
            FileInfo finfo = new FileInfo(filename);

            if (finfo.Directory.Exists == false)
            {
                finfo.Directory.Create();
            }

            System.IO.File.AppendAllText(filename, writeStringendtoFile, Encoding.Unicode);

        }






    }
}
