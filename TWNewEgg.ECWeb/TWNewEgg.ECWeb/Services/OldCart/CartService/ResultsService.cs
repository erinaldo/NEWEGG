using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Data.SqlClient;
using TWNewEgg.Website.ECWeb.Models;
using TWNewEgg.DB.TWSQLDB.Models;
using TWNewEgg.DB.TWSQLDB.Models.ExtModels;
using TWNewEgg.DB.TWBACKENDDB.Models;
using TWNewEgg.ItemService.Models;
using TWNewEgg.DB;

namespace TWNewEgg.Website.ECWeb.Service
{
    public class ResultsService : IDisposable
    {
        private string DB_IP = "10.16.131.43";  //Set DB Data for stored procedure , this should be move to a class or localfile.
        private string DB_Name = "TWSQLDB"; //Set DB Data for stored procedure , this should be move to a class or localfile.
        private string DB_User = "twec"; //Set DB Data for stored procedure , this should be move to a class or localfile.
        private string DB_Pass = "ABS301egg"; //Set DB Data for stored procedure , this should be move to a class or localfile.
        private TWSqlDBContext db = new TWSqlDBContext();

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
                db = null;
            }
        }
        //Combine two stored procedure "getItemTaxDetail" and "getSalesOrdersBySONumber" and caculate total service fee.
        public decimal SetResultsItem(List<ResultCart> Result, Dictionary<string, List<GetItemTaxDetail>> itemTaxDetail, Dictionary<string, decimal> shippingCosts)
        {
            decimal totalService = new decimal();
            foreach (var itemTax in itemTaxDetail)
            {

                decimal statePrice = new decimal();
                decimal stateService = new int();
                decimal stateTotal = new decimal();
                foreach (var ResultItem in Result.Where(x => x.itemSellerID == int.Parse(itemTax.Key))) //Find each result item which in same sellerid.
                {

                    string[] priceTaxService = itemTax.Value.Where(x => x.item_id == ResultItem.itemID && x.itemlist_id == ResultItem.itemAttrID).Select(y => y.pricetaxdetail).FirstOrDefault().Split(','); //Combine stored procedure "getItemTaxDetail" and results.
                    var delvType = db.Item.Where(x => x.ID == ResultItem.itemID).FirstOrDefault();
                    switch (delvType.DelvType)
                    {
                        case (int)Item.tradestatus.國外直購:
                            //ResultItem.itemTWprice -= decimal.Parse(priceTaxService[2]);//coz Rex insert the price with tax into salesorderitem, so here have to minus tax.
                            ResultItem.itemLocalprice = "---";
                            //ResultItem.itemTWprice = delvType.PriceCash;
                            ResultItem.itemTWprice = delvType.PriceCash + (delvType.Taxfee ?? 0) + delvType.PriceGlobalship;
                            ResultItem.priceSum = ResultItem.itemTWprice * ResultItem.itemQty;//plus pricesum after minus tax.
                            ResultItem.itemTax = 0; //plus qty *************it may not need plus...
                            //ResultItem.priceSum += Convert.ToInt32(ResultItem.itemTax.Value);//plus tax and TWPrice to statePrice
                            statePrice += ResultItem.priceSum.Value; //count state total price(without shipping cost).
                            ResultItem.serviceFees = decimal.Parse(priceTaxService[3]) * ResultItem.itemQty; //plus qty ************* it may not need plus...
                            ResultItem.shipping = 0;
                            ResultItem.discount = Convert.ToDecimal(priceTaxService[11]); // Set discount from store procedure
                            ResultItem.realPrice = Convert.ToDecimal(priceTaxService[12]); // Set Real Price from store procedure
                            stateService += ResultItem.serviceFees.Value; //count total state service fee.
                            stateTotal += ResultItem.priceSum.Value; //count state total price (with shipping cost).

                            foreach (var ResultItemlist in ResultItem.itemList) //Fine each result item's itemlist.
                            {
                                if (ResultItemlist.itemAttrID == 0)
                                {
                                    priceTaxService = itemTax.Value.Where(x => x.item_id == ResultItem.itemID && x.itemlist_id == ResultItemlist.itemID).Select(y => y.pricetaxdetail).FirstOrDefault().Split(','); //Combine stored procedure "getItemTaxDetail" and results.
                                }
                                else
                                {
                                    priceTaxService = itemTax.Value.Where(x => x.item_id == ResultItem.itemID && x.itemlist_id == ResultItemlist.itemAttrID).Select(y => y.pricetaxdetail).FirstOrDefault().Split(','); //Combine stored procedure "getItemTaxDetail" and results.
                                }
                                //statePrice += ResultItemlist.priceSum.Value;  //count state total price(without shipping cost).
                                if (priceTaxService[0] != "0") //if the local price have no price then set it to TWPrice
                                {
                                    ResultItemlist.itemLocalprice = priceTaxService[0];
                                }
                                else
                                {
                                    ResultItemlist.itemLocalprice = ResultItemlist.itemTWprice.Value.ToString();
                                }
                                ResultItemlist.itemTax = decimal.Parse(priceTaxService[2]) * ResultItemlist.itemQty; //plus qty *************it may not need plus...
                                ResultItemlist.priceSum += Convert.ToInt32(ResultItemlist.itemTax.Value);//plus tax and TWPrice to statePrice
                                statePrice += ResultItemlist.priceSum.Value; //count state total price(without shipping cost).
                                ResultItemlist.serviceFees = decimal.Parse(priceTaxService[3]) * ResultItemlist.itemQty; //plus qty ************* it may not need plus...

                                stateService += ResultItemlist.serviceFees.Value; //count total state service fee.
                                stateTotal += ResultItemlist.priceSum.Value;  //count state total price (with shipping cost).
                            }
                            break;
                        default:
                            ResultItem.itemTWprice -= decimal.Parse(priceTaxService[2]);//coz Rex insert the price with tax into salesorderitem, so here have to minus tax.
                            if (priceTaxService[0] != "0") //if the local price have no price then set it to TWPrice.
                            {
                                ResultItem.itemLocalprice = priceTaxService[0];
                            }
                            else
                            {
                                ResultItem.itemLocalprice = ResultItem.itemTWprice.Value.ToString();
                            }
                            ResultItem.priceSum = ResultItem.itemTWprice * ResultItem.itemQty;//plus pricesum after minus tax.
                            ResultItem.itemTax = decimal.Parse(priceTaxService[2]) * ResultItem.itemQty; //plus qty *************it may not need plus...
                            ResultItem.priceSum += Convert.ToInt32(ResultItem.itemTax.Value);//plus tax and TWPrice to statePrice
                            statePrice += ResultItem.priceSum.Value; //count state total price(without shipping cost).
                            ResultItem.serviceFees = decimal.Parse(priceTaxService[3]) * ResultItem.itemQty; //plus qty ************* it may not need plus...
                            ResultItem.shipping = Convert.ToInt32(shippingCosts[ResultItem.itemSellerID.ToString()]);
                            ResultItem.discount = Convert.ToDecimal(priceTaxService[11]); // Set discount from store procedure
                            ResultItem.realPrice = Convert.ToDecimal(priceTaxService[12]); // Set Real Price from store procedure
                            stateService += ResultItem.serviceFees.Value; //count total state service fee.
                            stateTotal += ResultItem.priceSum.Value; //count state total price (with shipping cost).

                            foreach (var ResultItemlist in ResultItem.itemList) //Fine each result item's itemlist.
                            {
                                if (ResultItemlist.itemAttrID == 0)
                                {
                                    priceTaxService = itemTax.Value.Where(x => x.item_id == ResultItem.itemID && x.itemlist_id == ResultItemlist.itemID).Select(y => y.pricetaxdetail).FirstOrDefault().Split(','); //Combine stored procedure "getItemTaxDetail" and results.
                                }
                                else
                                {
                                    priceTaxService = itemTax.Value.Where(x => x.item_id == ResultItem.itemID && x.itemlist_id == ResultItemlist.itemAttrID).Select(y => y.pricetaxdetail).FirstOrDefault().Split(','); //Combine stored procedure "getItemTaxDetail" and results.
                                }
                                //statePrice += ResultItemlist.priceSum.Value;  //count state total price(without shipping cost).
                                if (priceTaxService[0] != "0") //if the local price have no price then set it to TWPrice
                                {
                                    ResultItemlist.itemLocalprice = priceTaxService[0];
                                }
                                else
                                {
                                    ResultItemlist.itemLocalprice = ResultItemlist.itemTWprice.Value.ToString();
                                }
                                ResultItemlist.itemTax = decimal.Parse(priceTaxService[2]) * ResultItemlist.itemQty; //plus qty *************it may not need plus...
                                ResultItemlist.priceSum += Convert.ToInt32(ResultItemlist.itemTax.Value);//plus tax and TWPrice to statePrice
                                statePrice += ResultItemlist.priceSum.Value; //count state total price(without shipping cost).
                                ResultItemlist.serviceFees = decimal.Parse(priceTaxService[3]) * ResultItemlist.itemQty; //plus qty ************* it may not need plus...

                                stateService += ResultItemlist.serviceFees.Value; //count total state service fee.
                                stateTotal += ResultItemlist.priceSum.Value;  //count state total price (with shipping cost).
                            }
                            break;
                    }



                }
                //stateTotal += (stateService + shippingCosts[itemTax.Key]);
                stateTotal += shippingCosts[itemTax.Key]; //Don't count service fee.

                foreach (var aItem in Result.Where(x => x.itemSellerID == int.Parse(itemTax.Key)).ToList())
                {
                    aItem.statesPrice = Convert.ToInt32(statePrice); //Set item's statesPrice.
                    aItem.serviceFees = stateService; //Set item's serviceFees.
                    aItem.statesPricesum = Convert.ToInt32(stateTotal); //Set item's statesPricesum.
                }
                /*
                ResultCart firstItem = new ResultCart();
                firstItem = Result.Where(x => x.itemSellerID == int.Parse(itemTax.Key)).ToList()[0]; //Find each first item in one sellerID.
                firstItem.statesPrice = Convert.ToInt32(statePrice); //Set first item's statesPrice.
                firstItem.serviceFees = stateService; //Set first item's serviceFees.
                firstItem.statesPricesum = Convert.ToInt32(stateTotal); //Set first item's statesPricesum.
                */
                totalService += stateService; //count Total all state service fee.
            }
            return totalService;
        }

        //Combine two stored procedure "getItemTaxDetail" and "getSalesOrdersBySONumber" and caculate total service fee, and it's for oversea products.
        public decimal SetResultsItem(List<ResultCart> Result, Dictionary<string, List<GetItemTaxDetail>> itemTaxDetail, Dictionary<string, decimal> shippingCosts, string isOverSea)
        {
            decimal totalService = new decimal();
            foreach (var itemTax in itemTaxDetail)
            {
                decimal statePrice = new decimal();
                decimal stateService = new int();
                decimal stateTotal = new decimal();
                foreach (var ResultItem in Result.Where(x => x.itemID == int.Parse(itemTax.Key))) //Find each result item which in same sellerid.
                {

                    string[] priceTaxService = itemTax.Value.Where(x => x.item_id == ResultItem.itemID && x.itemlist_id == ResultItem.itemAttrID).Select(y => y.pricetaxdetail).FirstOrDefault().Split(','); //Combine stored procedure "getItemTaxDetail" and results.
                    //ResultItem.itemTWprice -= decimal.Parse(priceTaxService[2]);//coz Rex insert the price with tax into salesorderitem, so here have to minus tax.
                    var delvType = db.Item.Where(x => x.ID == ResultItem.itemID).FirstOrDefault();
                    switch (delvType.DelvType)
                    {
                        case (int)Item.tradestatus.海外切貨:
                            ResultItem.itemLocalprice = "---";
                            ResultItem.itemTWprice = (delvType.PriceCash + delvType.Taxfee + delvType.PriceGlobalship);
                            ResultItem.priceSum = (delvType.PriceCash + delvType.Taxfee + delvType.PriceGlobalship) * ResultItem.itemQty;//plus pricesum after minus tax.
                            //ResultItem.itemTax = delvType.Taxfee * ResultItem.itemQty; //plus qty *************it may not need plus...
                            ResultItem.itemTax = 0; //plus qty *************it may not need plus...
                            //ResultItem.priceSum += Convert.ToInt32(ResultItem.itemTax.Value);//plus tax and TWPrice to statePrice
                            statePrice += ResultItem.priceSum.Value; //count state total price(without shipping cost).
                            ResultItem.serviceFees = 0;//decimal.Parse(priceTaxService[3]) * ResultItem.itemQty; //plus qty ************* it may not need plus...
                            ResultItem.shipping = 0;
                            ResultItem.discount = Convert.ToDecimal(priceTaxService[11]); // Set discount from store procedure
                            ResultItem.realPrice = Convert.ToDecimal(priceTaxService[12]); // Set Real Price from store procedure
                            stateService += 0; //count total state service fee.
                            stateTotal += ResultItem.priceSum.Value; //count state total price (with shipping cost).
                            ResultItem.itemOriTax = priceTaxService[3] + "," + priceTaxService[4] + "," + priceTaxService[5] + "," + priceTaxService[6] + "," + priceTaxService[7];//set tax detail four tax list price 
                            foreach (var ResultItemlist in ResultItem.itemList) //Fine each result item's itemlist.
                            {
                                if (ResultItemlist.itemAttrID == 0)
                                {
                                    priceTaxService = itemTax.Value.Where(x => x.item_id == ResultItem.itemID && x.itemlist_id == ResultItemlist.itemID).Select(y => y.pricetaxdetail).FirstOrDefault().Split(','); //Combine stored procedure "getItemTaxDetail" and results.
                                }
                                else
                                {
                                    priceTaxService = itemTax.Value.Where(x => x.item_id == ResultItem.itemID && x.itemlist_id == ResultItemlist.itemAttrID).Select(y => y.pricetaxdetail).FirstOrDefault().Split(','); //Combine stored procedure "getItemTaxDetail" and results.
                                }
                                //statePrice += ResultItemlist.priceSum.Value;  //count state total price(without shipping cost).
                                if (priceTaxService[0] != "0") //if the local price have no price then set it to TWPrice
                                {
                                    ResultItemlist.itemLocalprice = priceTaxService[0];
                                }
                                else
                                {
                                    ResultItemlist.itemLocalprice = ResultItemlist.itemTWprice.Value.ToString();
                                }
                                ResultItemlist.itemTax = decimal.Parse(priceTaxService[2]) * ResultItemlist.itemQty; //plus qty *************it may not need plus...
                                //ResultItemlist.priceSum += Convert.ToInt32(ResultItemlist.itemTax.Value);//plus tax and TWPrice to statePrice
                                statePrice += ResultItemlist.priceSum.Value; //count state total price(without shipping cost).
                                ResultItemlist.serviceFees = int.Parse(priceTaxService[3]) * ResultItemlist.itemQty; //plus qty ************* it may not need plus...
                                ResultItemlist.itemOriTax = priceTaxService[3] + "," + priceTaxService[4] + "," + priceTaxService[5] + "," + priceTaxService[6] + "," + priceTaxService[7];//set tax detail four tax list price 

                                stateService += ResultItemlist.serviceFees.Value; //count total state service fee.
                                stateTotal += ResultItemlist.priceSum.Value;  //count state total price (with shipping cost).
                            }
                            break;
                        default:
                            if (priceTaxService[0] != "0") //if the local price have no price then set it to TWPrice.
                            {
                                ResultItem.itemLocalprice = priceTaxService[0];
                            }
                            else
                            {
                                ResultItem.itemLocalprice = ResultItem.itemTWprice.Value.ToString();
                            }
                            ResultItem.itemTWprice = delvType.PriceCash;
                            ResultItem.priceSum = ResultItem.itemTWprice * ResultItem.itemQty;//plus pricesum after minus tax.
                            ResultItem.itemTax = ResultItem.itemTax * ResultItem.itemQty; //decimal.Parse(priceTaxService[2]) * ResultItem.itemQty; //plus qty *************it may not need plus...
                            //ResultItem.priceSum += Convert.ToInt32(ResultItem.itemTax.Value);//plus tax and TWPrice to statePrice
                            statePrice += ResultItem.priceSum.Value; //count state total price(without shipping cost).
                            ResultItem.serviceFees = delvType.ServicePrice * ResultItem.itemQty; //decimal.Parse(priceTaxService[3]) * ResultItem.itemQty; //plus qty ************* it may not need plus...
                            ResultItem.shipping = Convert.ToInt32(shippingCosts[ResultItem.itemID.ToString()]) + ResultItem.itemTax;
                            ResultItem.discount = Convert.ToDecimal(priceTaxService[11]); // Set discount from store procedure
                            ResultItem.realPrice = Convert.ToDecimal(priceTaxService[12]); // Set Real Price from store procedure
                            stateService += delvType.ServicePrice * ResultItem.itemQty; //count total state service fee.
                            stateTotal += ResultItem.priceSum.Value; //count state total price (with shipping cost).
                            ResultItem.itemOriTax = priceTaxService[3] + "," + priceTaxService[4] + "," + priceTaxService[5] + "," + priceTaxService[6] + "," + priceTaxService[7];//set tax detail four tax list price 
                            foreach (var ResultItemlist in ResultItem.itemList) //Fine each result item's itemlist.
                            {
                                if (ResultItemlist.itemAttrID == 0)
                                {
                                    priceTaxService = itemTax.Value.Where(x => x.item_id == ResultItem.itemID && x.itemlist_id == ResultItemlist.itemID).Select(y => y.pricetaxdetail).FirstOrDefault().Split(','); //Combine stored procedure "getItemTaxDetail" and results.
                                }
                                else
                                {
                                    priceTaxService = itemTax.Value.Where(x => x.item_id == ResultItem.itemID && x.itemlist_id == ResultItemlist.itemAttrID).Select(y => y.pricetaxdetail).FirstOrDefault().Split(','); //Combine stored procedure "getItemTaxDetail" and results.
                                }
                                //statePrice += ResultItemlist.priceSum.Value;  //count state total price(without shipping cost).
                                if (priceTaxService[0] != "0") //if the local price have no price then set it to TWPrice
                                {
                                    ResultItemlist.itemLocalprice = priceTaxService[0];
                                }
                                else
                                {
                                    ResultItemlist.itemLocalprice = ResultItemlist.itemTWprice.Value.ToString();
                                }
                                ResultItemlist.itemTax = decimal.Parse(priceTaxService[2]) * ResultItemlist.itemQty; //plus qty *************it may not need plus...
                                //ResultItemlist.priceSum += Convert.ToInt32(ResultItemlist.itemTax.Value);//plus tax and TWPrice to statePrice
                                statePrice += ResultItemlist.priceSum.Value; //count state total price(without shipping cost).
                                ResultItemlist.serviceFees = int.Parse(priceTaxService[3]) * ResultItemlist.itemQty; //plus qty ************* it may not need plus...
                                ResultItemlist.itemOriTax = priceTaxService[3] + "," + priceTaxService[4] + "," + priceTaxService[5] + "," + priceTaxService[6] + "," + priceTaxService[7];//set tax detail four tax list price 

                                stateService += ResultItemlist.serviceFees.Value; //count total state service fee.
                                stateTotal += ResultItemlist.priceSum.Value;  //count state total price (with shipping cost).
                            }
                            break;
                    }


                    stateTotal += ResultItem.shipping.Value;
                }
                //stateTotal += (stateService + shippingCosts[itemTax.Key]);
                //stateTotal += shippingCosts[itemTax.Key]; //Don't count service fee.

                foreach (var aItem in Result.Where(x => x.itemID == int.Parse(itemTax.Key)).ToList())
                {
                    aItem.statesPrice = Convert.ToInt32(statePrice); //Set item's statesPrice.
                    aItem.serviceFees = stateService; //Set item's serviceFees.
                    aItem.statesPricesum = Convert.ToInt32(stateTotal); //Set item's statesPricesum.
                }
                /*
                ResultCart firstItem = new ResultCart();
                firstItem = Result.Where(x => x.itemSellerID == int.Parse(itemTax.Key)).ToList()[0]; //Find each first item in one sellerID.
                firstItem.statesPrice = Convert.ToInt32(statePrice); //Set first item's statesPrice.
                firstItem.serviceFees = stateService; //Set first item's serviceFees.
                firstItem.statesPricesum = Convert.ToInt32(stateTotal); //Set first item's statesPricesum.
                */
                totalService += stateService; //count Total all state service fee.
            }
            return totalService;
        }

        public void SetInResults(List<InsertSalesOrdersBySellerOutput> ResultsTemp, List<ResultCart> Result, List<string> sameSellerCode, Seller seller, Dictionary<string, int> rowSpan, List<BuyingItems> ResultItems, List<Country> countries)
        {
            //DatabaseContext db = new DatabaseContext();
            foreach (var sellerCode in sameSellerCode) //Get all Result which had same sellerID and set into ResultCat.
            {
                var Item = ResultsTemp.Where(x => x.salesorder_code == sellerCode && x.salesorderitem_itemlistid == 0).ToList(); //Find where the item had in or not in ResultsTemp.
                var ItemList = ResultsTemp.Where(x => x.salesorder_code == sellerCode && x.salesorderitem_itemlistid != 0).ToList(); //find item's itemlist .
                var productID = Item[0].salesorderitem_productid; //Get product ID
                var productAll = db.Product.Where(x => x.ID == productID).FirstOrDefault(); //Find where the product is in the DB.
                decimal? ItemTotalCouponPrice = Item.Sum(x => x.salesorderitem_pricecoupon); // 算出每項Item使用了Coupon卷的金額總額
                decimal? ItemTotalApportionedAmount = Item.Sum(x => x.salesorderitem_apportionedamount); //calulate all apportionedamount price.
                var attrID = GetItemAttrID(Item[0].salesorderitem_itemid, Item[0].salesorderitem_productid, "itemAttr"); //if there had attrID then set it.
                ResultItems.Add(new BuyingItems { buyItemID_DelvType = (int)Item[0].salesorder_delivtype, buyItemID_Seller = seller.ID, buyItemID = Item[0].salesorderitem_itemid, buyingNumber = Item.Count, item_AttrID = attrID, buyItemLists = new List<BuyingItemList>() }); //Addd Item in ResultItem

                //Result.Add(SetAResultCart(productAll.DelvType.Value, sellerCode, Item, attrID, seller, countries, itemList, rowSpan, ResultItems, productAll));
                Result.Add(new ResultCart //Set date into Result.
                {
                    itemSONumber = sellerCode,
                    itemID = Item[0].salesorderitem_itemid,
                    itemAttrID = attrID,
                    itemAttribname = Item[0].salesorderitem_attribs,
                    itemName = Item[0].salesorderitem_name,
                    itemTWprice = Item[0].salesorderitem_price,
                    itemQty = Item.Count,
                    itemCountry = seller.CountryID.Value,
                    itemCountryName = countries.Where(x => x.ID == seller.CountryID.Value).FirstOrDefault().Name,
                    itemList = SetItemlist(ItemList, seller, rowSpan, ResultItems.Where(x => x.buyItemID == Item[0].salesorderitem_itemid).Select(y => y.buyItemLists).FirstOrDefault(), countries, sellerCode),
                    itemSeller = seller.Name,
                    itemSellerID = seller.ID,
                    priceSum = Item[0].salesorderitem_price * Item.Count,
                    itemLocalprice = ((productAll.Cost == null) ? "---" : productAll.Cost.Value.ToString()),
                    itemTax = Item[0].salesorderitem_tax,//productAll.TradeTax,
                    productid = productID,
                    Coupons = Item[0].salesorderitem_coupons,
                    Pricecoupon = ItemTotalCouponPrice,
                    apportionedAmount = ItemTotalApportionedAmount,
                    displayPrice = Item[0].salesorderitem_displayprice,
                    //discount = Item[0].salesorderitem_discountprice,
                    //below code have to read real data 
                    //shipping = Item[0].salesorderitem_price / 50, do at View(Results) page
                    itemPrice = Item[0].salesorderitem_price, //Set in function "SetResultsItem"
                    serviceFees = Item[0].salesorderitem_price / 200, //Set in function "SetResultsItem"
                    statesPrice = Item[0].salesorderitem_price / 4, //Set in function "SetResultsItem"
                    statesPricesum = Item[0].salesorderitem_price / 2, //Set in function "SetResultsItem"
                    InstallmentFee = Item.Sum(x => x.salesorderitem_installmentfee)
                });
                //Result[0].itemList = SetItemlist(itemList, seller, rowSpan, buyItemLists);
                if (rowSpan.ContainsKey(seller.Name)) //count how many row in same seller id that View to display table's td rowSpan.
                {
                    rowSpan[seller.Name]++;
                }
                else
                {
                    rowSpan.Add(seller.Name, 1);
                }


            }
            //return false;
            return;
        }
        private ResultCart SetAResultCart(int delvType, string sellerCode, List<InsertSalesOrdersBySellerOutput> Item, int attrID, Seller seller, List<Country> countries, List<InsertSalesOrdersBySellerOutput> ItemList, Dictionary<string, int> rowSpan, List<BuyingItems> ResultItems, Product productAll)
        {
            ResultCart aResultCart;
            switch (delvType)
            {
                case 4:
                    aResultCart = new ResultCart()
                    {
                        itemSONumber = sellerCode,
                        itemID = Item[0].salesorderitem_itemid,
                        itemAttrID = attrID,
                        itemAttribname = Item[0].salesorderitem_attribs,
                        itemName = Item[0].salesorderitem_name,
                        itemTWprice = Item[0].salesorderitem_price,
                        itemQty = Item.Count,
                        itemCountry = seller.CountryID.Value,
                        itemCountryName = countries.Where(x => x.ID == seller.CountryID.Value).FirstOrDefault().Name,
                        itemList = SetItemlist(ItemList, seller, rowSpan, ResultItems.Where(x => x.buyItemID == Item[0].salesorderitem_itemid).Select(y => y.buyItemLists).FirstOrDefault(), countries, sellerCode),
                        itemSeller = seller.Name,
                        itemSellerID = seller.ID,
                        priceSum = Item[0].salesorderitem_price * Item.Count,
                        itemLocalprice = ((productAll.Cost == null) ? "---" : productAll.Cost.Value.ToString()),
                        itemTax = productAll.TradeTax,
                        productid = productAll.ID,
                        //below code have to read real data 
                        //shipping = Item[0].salesorderitem_price / 50, do at View(Results) page
                        itemPrice = Item[0].salesorderitem_price, //Set in function "SetResultsItem"
                        serviceFees = Item[0].salesorderitem_price / 200, //Set in function "SetResultsItem"
                        statesPrice = Item[0].salesorderitem_price / 4, //Set in function "SetResultsItem"
                        statesPricesum = Item[0].salesorderitem_price / 2 //Set in function "SetResultsItem"
                    };
                    break;
                case 6:
                    aResultCart = new ResultCart()
                    {
                        itemSONumber = sellerCode,
                        itemID = Item[0].salesorderitem_itemid,
                        itemAttrID = attrID,
                        itemAttribname = Item[0].salesorderitem_attribs,
                        itemName = Item[0].salesorderitem_name,
                        itemTWprice = Item[0].salesorderitem_price,
                        itemQty = Item.Count,
                        itemCountry = seller.CountryID.Value,
                        itemCountryName = countries.Where(x => x.ID == seller.CountryID.Value).FirstOrDefault().Name,
                        itemList = SetItemlist(ItemList, seller, rowSpan, ResultItems.Where(x => x.buyItemID == Item[0].salesorderitem_itemid).Select(y => y.buyItemLists).FirstOrDefault(), countries, sellerCode),
                        itemSeller = seller.Name,
                        itemSellerID = seller.ID,
                        priceSum = Item[0].salesorderitem_price * Item.Count,
                        itemLocalprice = ((productAll.Cost == null) ? "---" : productAll.Cost.Value.ToString()),
                        itemTax = productAll.TradeTax,
                        productid = productAll.ID,
                        //below code have to read real data 
                        //shipping = Item[0].salesorderitem_price / 50, do at View(Results) page
                        itemPrice = Item[0].salesorderitem_price, //Set in function "SetResultsItem"
                        serviceFees = Item[0].salesorderitem_price / 200, //Set in function "SetResultsItem"
                        statesPrice = Item[0].salesorderitem_price / 4, //Set in function "SetResultsItem"
                        statesPricesum = Item[0].salesorderitem_price / 2 //Set in function "SetResultsItem"
                    };
                    break;
                default:
                    aResultCart = new ResultCart()
                    {
                        itemSONumber = sellerCode,
                        itemID = Item[0].salesorderitem_itemid,
                        itemAttrID = attrID,
                        itemAttribname = Item[0].salesorderitem_attribs,
                        itemName = Item[0].salesorderitem_name,
                        itemTWprice = Item[0].salesorderitem_price,
                        itemQty = Item.Count,
                        itemCountry = seller.CountryID.Value,
                        itemCountryName = countries.Where(x => x.ID == seller.CountryID.Value).FirstOrDefault().Name,
                        itemList = SetItemlist(ItemList, seller, rowSpan, ResultItems.Where(x => x.buyItemID == Item[0].salesorderitem_itemid).Select(y => y.buyItemLists).FirstOrDefault(), countries, sellerCode),
                        itemSeller = seller.Name,
                        itemSellerID = seller.ID,
                        priceSum = Item[0].salesorderitem_price * Item.Count,
                        itemLocalprice = ((productAll.Cost == null) ? "---" : productAll.Cost.Value.ToString()),
                        itemTax = productAll.TradeTax,
                        productid = productAll.ID,
                        //below code have to read real data 
                        //shipping = Item[0].salesorderitem_price / 50, do at View(Results) page
                        itemPrice = Item[0].salesorderitem_price, //Set in function "SetResultsItem"
                        serviceFees = Item[0].salesorderitem_price / 200, //Set in function "SetResultsItem"
                        statesPrice = Item[0].salesorderitem_price / 4, //Set in function "SetResultsItem"
                        statesPricesum = Item[0].salesorderitem_price / 2 //Set in function "SetResultsItem"
                    };
                    break;
            }
            return aResultCart;
        }
        private int GetItemAttrID(int itemID, int productID, string itemOrlist) //Find 
        {
            //DatabaseContext db = new DatabaseContext();
            int itemAttrID = new int();
            if (itemOrlist == "itemAttr")
            {
                //Find Item's AttrID.
                var Attr = db.ItemList.Where(x => x.ItemID == itemID && x.ItemlistProductID == productID).Select(y => y.ID).FirstOrDefault();
                if (Attr != null)
                {
                    itemAttrID = Attr;
                }
                else
                {
                    itemAttrID = 0;
                }
            }
            else if (itemOrlist == "itemlistAttr")
            {
                //Find Itemlist's AttrID.
                var Attr = db.ItemList.Where(x => x.ItemlistID == itemID && x.ItemlistProductID == productID).Select(y => y.ID).FirstOrDefault();
                if (Attr != null)
                {
                    itemAttrID = Attr;
                }
                else
                {
                    itemAttrID = 0;
                }
            }
            else
            {
                itemAttrID = 0;
            }
            return itemAttrID;
        }
        private List<ResultCart> SetItemlist(List<InsertSalesOrdersBySellerOutput> ResultsTemp, Seller seller, Dictionary<string, int> rowSpan, List<BuyingItemList> ResultItems, List<Country> countries, string salesOrderCode = null)
        {
            //DatabaseContext db = new DatabaseContext();
            List<ResultCart> ItemList = new List<ResultCart>();

            foreach (var itemlistid in ResultsTemp.Select(x => x.salesorderitem_itemlistid).Distinct()) //Set Itemlist in Result, almost same with function "SetInResults".
            {
                var Item = ResultsTemp.Where(x => x.salesorderitem_itemlistid == itemlistid).ToList();
                var productID = Item[0].salesorderitem_productlistid;
                var productAll = db.Product.Where(x => x.ID == productID).FirstOrDefault();
                var itemlistID = Item[0].salesorderitem_itemlistid;
                var itemlistAll = db.ItemList.Where(x => x.ID == itemlistid).FirstOrDefault();
                ResultItems.Add(new BuyingItemList { buyItemlistID = Item[0].salesorderitem_itemlistid, buyingNumber = Item.Count, item_AttrID = GetItemAttrID(Item[0].salesorderitem_itemlistid, Item[0].salesorderitem_productlistid, "itemlistAttr") });

                ItemList.Add(new ResultCart
                {
                    itemSONumber = salesOrderCode,
                    itemID = Item[0].salesorderitem_itemlistid,
                    itemAttribname = Item[0].salesorderitem_attribs,
                    itemName = itemlistAll.Name,
                    itemTWprice = Item[0].salesorderitem_price,
                    itemQty = Item.Count,
                    itemCountry = seller.CountryID.Value,
                    itemCountryName = countries.Where(x => x.ID == seller.CountryID.Value).FirstOrDefault().Name,
                    itemList = null,
                    itemSeller = seller.Name,
                    itemSellerID = seller.ID,
                    priceSum = Item[0].salesorderitem_price * Item.Count,
                    itemLocalprice = ((productAll.Cost == null) ? "---" : productAll.Cost.Value.ToString()),
                    itemTax = productAll.TradeTax,
                    itemType = GetItemListType(itemlistAll.ItemlistGroupID, itemlistAll.ItemID),
                    productid = productID,
                    displayPrice = Item[0].salesorderitem_displayprice,
                    //discount = Item[0].salesorderitem_discountprice,
                    //below code have to read real data 
                    //shipping = Item[0].salesorderitem_price / 50, do at View(Results) page
                    itemPrice = Item[0].salesorderitem_price, //No using
                    serviceFees = 0, //No using
                    statesPrice = 0, //No using
                    statesPricesum = 0 //No using
                });
                if (rowSpan.ContainsKey(seller.Name))
                {
                    rowSpan[seller.Name]++;
                }
                else
                {
                    rowSpan.Add(seller.Name, 1);
                }
            }


            return ItemList;
        }
        public bool GetIsOverSeaByDelvType(int? delvType)
        {
            bool isOverSea = false;

            if (isOverSea == null)
            {
                return isOverSea;
            }

            switch (delvType.Value)
            {
                case 0:
                    isOverSea = false;
                    break;
                case 1:
                    isOverSea = false;
                    break;
                case 2:
                    isOverSea = false;
                    break;
                case 3:
                    isOverSea = true;
                    break;
                case 4:
                    isOverSea = false;
                    break;
                case 5:
                    isOverSea = true;
                    break;
                case 6:
                    isOverSea = true;
                    break;
                default:
                    isOverSea = false;
                    break;
            }

            return isOverSea;
        }
        private string GetItemListType(int groupID, int itemID) //Get Item Type and return Chinese.
        {
            //DatabaseContext db = new DatabaseContext();
            string itemType = "";
            itemType = db.ItemListGroup.Where(x => x.ID == groupID && x.ItemID == itemID).Select(x => x.Type).FirstOrDefault().ToString();
            switch (itemType)
            {
                case "0":
                    itemType = "配件";
                    break;
                case "20":
                    itemType = "贈品";
                    break;
                default:
                    itemType = "配件";
                    break;
            }

            return itemType;
        }
        private List<InsertSalesOrdersBySellerOutput> GetSameOrderCode(string salesOrderCode, List<InsertSalesOrdersBySellerOutput> ResultsOri)//No using
        {
            return ResultsOri.Where(x => x.salesorder_code == salesOrderCode).ToList();
        }
        //Get Stored procedure from DB.
        public List<InsertSalesOrdersBySellerOutput> GetSalesOrders(string soNumber)
        {
            DbQuery nDb = null;
            //DbQuery iDb = null;
            SqlParameter[] NarySqlParameter = null;


            DataSet dsResult = null;


            nDb = new Models.DbQuery();


            NarySqlParameter = new SqlParameter[1]; // 如果要增加參數則SqlParameter[2]、SqlParameter[3]...
            NarySqlParameter[0] = new SqlParameter("@salesorder_code", SqlDbType.NVarChar);
            NarySqlParameter[0].Value = soNumber;

            string combine = "";
            combine = "exec dbo.[UP_EC_GetSalesOrdersBySONumberV4] '" + NarySqlParameter[0].Value + "'";

            dsResult = nDb.Query(combine, NarySqlParameter);

            //DataTable NdtItem = null; //getSalesOrderNumByDate
            //dsResult = oDb.Query("exec shoppingcart_getitem @account_id", arySqlParameter);
            //dsResult = nDb.Query(combine, arySqlParameter);

            DataTable dtItem = null;

            List<InsertSalesOrdersBySellerOutput> salesOrders = new List<InsertSalesOrdersBySellerOutput>();
            salesOrders.Clear();

            if (dsResult != null && dsResult.Tables.Count > 0)
            {
                dtItem = dsResult.Tables[0];

                int dsTcount = dsResult.Tables.Count;  // 計算DataSet中DataTable的個數
                int dsRcount = dsResult.Tables[0].Rows.Count; // 計算DataTable中Tables[0]的Rows的個數
                int _dsRcount = 0;
                //ViewBag.dsTcount = dsTcount;
                //ViewBag.dsRcount = dsRcount;

                InsertSalesOrdersBySellerOutput[] sp = new InsertSalesOrdersBySellerOutput[dsRcount];

                foreach (DataRow dr in dtItem.Rows)
                {
                    //----------public partial class salesorder----------//
                    sp[_dsRcount] = new InsertSalesOrdersBySellerOutput();
                    sp[_dsRcount].salesorder_code = Convert.ToString(dr["Code"]);
                    sp[_dsRcount].salesorder_salesordergroupid = Convert.ToInt32(dr["SalesOrderGroupID"]);
                    sp[_dsRcount].salesorder_idno = Convert.ToString(dr["IDNO"]);
                    sp[_dsRcount].salesorder_name = Convert.ToString(dr["Name"]);
                    sp[_dsRcount].salesorder_accountid = Convert.ToInt32(dr["AccountID"]);
                    sp[_dsRcount].salesorder_telday = Convert.ToString(dr["TelDay"]);
                    sp[_dsRcount].salesorder_telnight = Convert.ToString(dr["TelNight"]);
                    sp[_dsRcount].salesorder_mobile = Convert.ToString(dr["Mobile"]);
                    sp[_dsRcount].salesorder_email = Convert.ToString(dr["Email"]);
                    sp[_dsRcount].salesorder_paytype = Convert.ToInt16(dr["PayType"]);
                    sp[_dsRcount].salesorder_paytypeid = Convert.ToInt16(dr["PayTypeID"]);
                    //--------------------------------------//
                    DateTime dt = DateTime.Parse("1990/01/01");
                    try
                    {
                        dt = Convert.ToDateTime(dr["StarvlDate"]);
                    }
                    catch { }
                    sp[_dsRcount].salesorder_starvldate = dt;
                    //--------------------------------------//
                    sp[_dsRcount].salesorder_cardholder = Convert.ToString(dr["CardHolder"]);
                    sp[_dsRcount].salesorder_cardtelday = Convert.ToString(dr["CardTelDay"]);
                    sp[_dsRcount].salesorder_cardtelnight = Convert.ToString(dr["CardTelNight"]);
                    sp[_dsRcount].salesorder_cardmobile = Convert.ToString(dr["CardMobile"]);
                    sp[_dsRcount].salesorder_cardloc = Convert.ToString(dr["CardLOC"]);
                    sp[_dsRcount].salesorder_cardzip = Convert.ToString(dr["CardZip"]);
                    sp[_dsRcount].salesorder_cardaddr = Convert.ToString(dr["CardADDR"]);
                    sp[_dsRcount].salesorder_cardno = Convert.ToString(dr["CardNo"]);
                    sp[_dsRcount].salesorder_cardnochk = Convert.ToString(dr["CardNochk"]);
                    sp[_dsRcount].salesorder_cardtype = Convert.ToString(dr["CardType"]);
                    sp[_dsRcount].salesorder_cardbank = Convert.ToString(dr["CardBank"]);
                    sp[_dsRcount].salesorder_cardexpire = Convert.ToString(dr["CardExpire"]);
                    //--------------------------------------//
                    dt = DateTime.Parse("1990/01/01");
                    try
                    {
                        dt = Convert.ToDateTime(dr["CardBirthday"]);
                    }
                    catch { }
                    sp[_dsRcount].salesorder_cardbirthday = dt;
                    //--------------------------------------//
                    sp[_dsRcount].salesorder_invoreceiver = Convert.ToString(dr["InvoiceReceiver"]);
                    sp[_dsRcount].salesorder_invoid = Convert.ToString(dr["InvoiceID"]);
                    sp[_dsRcount].salesorder_invotitle = Convert.ToString(dr["InvoiceTitle"]);
                    sp[_dsRcount].salesorder_involoc = Convert.ToString(dr["InvoiceLoc"]);
                    sp[_dsRcount].salesorder_invozip = Convert.ToString(dr["InvoiceZip"]);
                    sp[_dsRcount].salesorder_invoaddr = Convert.ToString(dr["InvoiceAddr"]);
                    sp[_dsRcount].salesorder_recvname = Convert.ToString(dr["RecvName"]);
                    sp[_dsRcount].salesorder_recvengname = Convert.ToString(dr["RecvEngName"]);
                    sp[_dsRcount].salesorder_recvtelday = Convert.ToString(dr["RecvTelDay"]);
                    sp[_dsRcount].salesorder_recvtelnight = Convert.ToString(dr["RecvTelNight"]);
                    sp[_dsRcount].salesorder_recvmobile = Convert.ToString(dr["RecvMobile"]);
                    sp[_dsRcount].salesorder_delivtype = Convert.ToByte(dr["DelivType"]);
                    sp[_dsRcount].salesorder_delivdata = Convert.ToString(dr["DelivData"]);
                    sp[_dsRcount].salesorder_delivloc = Convert.ToString(dr["DelivLOC"]);
                    sp[_dsRcount].salesorder_delivzip = Convert.ToString(dr["DelivZip"]);
                    sp[_dsRcount].salesorder_delivaddr = Convert.ToString(dr["DelivADDR"]);
                    sp[_dsRcount].salesorder_delivengaddr = Convert.ToString(dr["DelivEngADDR"]);
                    sp[_dsRcount].salesorder_delivhitnote = Convert.ToString(dr["DelivHitNote"]);
                    //--------------------------------------//
                    dt = DateTime.Parse("1990/01/01");
                    try
                    {
                        dt = Convert.ToDateTime(dr["ConfirmDate"]);
                    }
                    catch { }
                    sp[_dsRcount].salesorder_confirmdate = dt;
                    //--------------------------------------//
                    sp[_dsRcount].salesorder_confirmnote = Convert.ToString(dr["ConfirmNote"]);
                    //--------------------------------------//
                    dt = DateTime.Parse("1990/01/01");
                    try
                    {
                        dt = Convert.ToDateTime(dr["AuthDate"]);
                    }
                    catch { }
                    sp[_dsRcount].salesorder_authdate = dt;
                    //--------------------------------------//
                    sp[_dsRcount].salesorder_authcode = Convert.ToString(dr["AuthCode"]);
                    sp[_dsRcount].salesorder_authnote = Convert.ToString(dr["AuthNote"]);
                    short dshort = 0;
                    try
                    {
                        dshort = Convert.ToInt16(dr["HpType"]);
                    }
                    catch { }
                    sp[_dsRcount].salesorder_hptype = dshort;
                    //--------------------------------------//
                    dt = DateTime.Parse("1990/01/01");
                    try
                    {
                        dt = Convert.ToDateTime(dr["RcptDate"]);
                    }
                    catch { }
                    sp[_dsRcount].salesorder_rcptdate = dt;
                    //--------------------------------------//
                    sp[_dsRcount].salesorder_rcptnote = Convert.ToString(dr["RcptNote"]);
                    //--------------------------------------//
                    dt = DateTime.Parse("1990/01/01");
                    try
                    {
                        dt = Convert.ToDateTime(dr["Expire"]);
                    }
                    catch { }
                    sp[_dsRcount].salesorder_expire = dt;
                    //--------------------------------------//
                    //--------------------------------------//
                    dt = DateTime.Parse("1990/01/01");
                    try
                    {
                        dt = Convert.ToDateTime(dr["DateDEL"]);
                    }
                    catch { }
                    sp[_dsRcount].salesorder_datedel = dt;
                    //--------------------------------------//
                    sp[_dsRcount].salesorder_coservername = Convert.ToString(dr["CoServerName"]);
                    sp[_dsRcount].salesorder_servername = Convert.ToString(dr["ServerName"]);
                    sp[_dsRcount].salesorder_actcode = Convert.ToString(dr["ActCode"]);
                    sp[_dsRcount].salesorder_status = Convert.ToByte(dr["Status"]);
                    sp[_dsRcount].salesorder_statusnote = Convert.ToString(dr["StatusNote"]);
                    sp[_dsRcount].salesorder_remoteip = Convert.ToString(dr["RemoteIP"]);
                    //--------------------------------------//
                    dt = DateTime.Parse("1990/01/01");
                    try
                    {
                        dt = Convert.ToDateTime(dr["Date"]);
                    }
                    catch { }
                    sp[_dsRcount].salesorder_date = dt;
                    //--------------------------------------//
                    sp[_dsRcount].salesorder_note = Convert.ToString(dr["Note"]);
                    sp[_dsRcount].salesorder_note2 = Convert.ToString(dr["Note2"]);
                    sp[_dsRcount].salesorder_createuser = Convert.ToString(dr["CreateUser"]);
                    //--------------------------------------//
                    dt = DateTime.Parse("1990/01/01");
                    try
                    {
                        dt = Convert.ToDateTime(dr["CreateDate"]);
                    }
                    catch { }
                    sp[_dsRcount].salesorder_createdate = dt;
                    //--------------------------------------//
                    sp[_dsRcount].salesorder_updated = Convert.ToByte(dr["Updated"]);
                    sp[_dsRcount].salesorder_updateuser = Convert.ToString(dr["UpdateUser"]);
                    //--------------------------------------//
                    dt = DateTime.Parse("1990/01/01");
                    try
                    {
                        dt = Convert.ToDateTime(dr["UpdateDate"]);
                    }
                    catch { }
                    sp[_dsRcount].salesorder_updatedate = dt;
                    //--------------------------------------//
                    //----------public partial class salesorderitem----------//
                    sp[_dsRcount].salesorderitem_code = Convert.ToString(dr["SalesorderItem_Code"]);

                    sp[_dsRcount].salesorderitem_salesordercode = Convert.ToString(dr["SalesorderItem_SalesorderCode"]);
                    sp[_dsRcount].salesorderitem_itemid = Convert.ToInt32(dr["SalesorderItem_ItemID"]);
                    sp[_dsRcount].salesorderitem_itemlistid = Convert.ToInt32(dr["SalesorderItem_ItemlistID"]);

                    sp[_dsRcount].salesorderitem_productid = Convert.ToInt32(dr["SalesorderItem_ProductID"]);
                    sp[_dsRcount].salesorderitem_productlistid = Convert.ToInt32(dr["SalesorderItem_ProductlistID"]);
                    sp[_dsRcount].salesorderitem_name = Convert.ToString(dr["SalesorderItem_Name"]);
                    sp[_dsRcount].salesorderitem_price = Convert.ToInt32(dr["SalesorderItem_Price"]);
                    sp[_dsRcount].salesorderitem_priceinst = Convert.ToDecimal(dr["SalesorderItem_Priceinst"]);
                    sp[_dsRcount].salesorderitem_qty = Convert.ToInt32(dr["SalesorderItem_Qty"]);
                    sp[_dsRcount].salesorderitem_pricecoupon = Convert.ToDecimal(dr["SalesorderItem_Pricecoupon"]);
                    sp[_dsRcount].salesorderitem_coupons = Convert.ToString(dr["SalesorderItem_Coupons"]);
                    sp[_dsRcount].salesorderitem_redmtkout = Convert.ToInt32(dr["SalesorderItem_RedmtkOut"]);
                    sp[_dsRcount].salesorderitem_redmbln = Convert.ToInt32(dr["SalesorderItem_RedmBLN"]);
                    sp[_dsRcount].salesorderitem_redmfdbck = Convert.ToInt32(dr["SalesorderItem_Redmfdbck"]);
                    byte dbyte = 0;
                    try
                    {
                        dbyte = Convert.ToByte(dr["SalesorderItem_Status"]);
                    }
                    catch { }
                    sp[_dsRcount].salesorderitem_status = dbyte;
                    sp[_dsRcount].salesorderitem_statusnote = Convert.ToString(dr["SalesorderItem_StatusNote"]);
                    //--------------------------------------//
                    dt = DateTime.Parse("1990/01/01");
                    try
                    {
                        dt = Convert.ToDateTime(dr["SalesorderItem_Date"]);
                    }
                    catch { }
                    sp[_dsRcount].salesorderitem_date = dt;
                    //--------------------------------------//
                    sp[_dsRcount].salesorderitem_attribs = Convert.ToString(dr["SalesorderItem_Attribs"]);
                    sp[_dsRcount].salesorderitem_note = Convert.ToString(dr["SalesorderItem_Note"]);
                    sp[_dsRcount].salesorderitem_wftkout = Convert.ToInt32(dr["SalesorderItem_WftkOut"]);
                    sp[_dsRcount].salesorderitem_wfbln = Convert.ToInt32(dr["SalesorderItem_WfBLN"]);
                    int dint = 0;
                    try
                    {
                        dint = Convert.ToInt32(dr["SalesorderItem_AdjPrice"]);
                    }
                    catch { }
                    sp[_dsRcount].salesorderitem_adjprice = dint;
                    sp[_dsRcount].salesorderitem_actid = Convert.ToString(dr["SalesorderItem_ActID"]);
                    sp[_dsRcount].salesorderitem_acttkout = Convert.ToInt32(dr["SalesorderItem_ActtkOut"]);
                    dint = 0;
                    try
                    {
                        dint = Convert.ToInt32(dr["SalesorderItem_ProdcutCostID"]);
                    }
                    catch { }
                    sp[_dsRcount].salesorderitem_prodcutcostid = dint;
                    sp[_dsRcount].salesorderitem_createuser = Convert.ToString(dr["SalesorderItem_CreateUser"]);
                    //--------------------------------------//
                    dt = DateTime.Parse("1990/01/01");
                    try
                    {
                        dt = Convert.ToDateTime(dr["SalesorderItem_CreateDate"]);
                    }
                    catch { }
                    sp[_dsRcount].salesorderitem_createdate = dt;
                    //--------------------------------------//
                    sp[_dsRcount].salesorderitem_updated = Convert.ToByte(dr["SalesorderItem_Updated"]);
                    //--------------------------------------//
                    dt = DateTime.Parse("1990/01/01");
                    try
                    {
                        dt = Convert.ToDateTime(dr["SalesorderItem_UpdateDate"]);
                    }
                    catch { }
                    sp[_dsRcount].salesorderitem_updatedate = dt;
                    //--------------------------------------//
                    sp[_dsRcount].salesorderitem_updateuser = Convert.ToString(dr["SalesorderItem_UpdateUser"]);
                    sp[_dsRcount].salesorderitem_shippingexpense = Convert.ToInt32(dr["SalesorderItem_ShippingExpense"]); // 部分運費
                    sp[_dsRcount].salesorderitem_serviceexpense = Convert.ToInt32(dr["SalesorderItem_ServiceExpense"]); // 部分服務費
                    sp[_dsRcount].salesorderitem_tax = Convert.ToInt32(dr["SalesorderItem_Tax"]); // 部分稅金
                    sp[_dsRcount].salesorderitem_displayprice = Convert.ToDecimal(dr["SalesorderItem_DisplayPrice"]);
                    sp[_dsRcount].salesorderitem_discountprice = Convert.ToDecimal(dr["SalesorderItem_DiscountPrice"]);
                    sp[_dsRcount].salesorderitem_itempricesum = Convert.ToInt32(dr["SalesorderItem_ItemPriceSum"]);
                    sp[_dsRcount].salesorderitem_installmentfee = Convert.ToDecimal(dr["SalesorderItem_InstallmentFee"]);
                    sp[_dsRcount].salesorderitem_isnew = Convert.ToString(dr["SalesorderItem_IsNew"]);
                    sp[_dsRcount].salesorderitem_apportionedamount = Convert.ToDecimal(dr["SalesorderItem_ApportionedAmount"]);
                    //----------public partial class salesorderitemext----------//
                    sp[_dsRcount].salesorderitemext_id = Convert.ToInt32(dr["SalesorderItemExt_ID"]);
                    sp[_dsRcount].salesorderitemext_salesorderitemcode = Convert.ToString(dr["SalesorderItemExt_SalesorderitemCode"]);
                    sp[_dsRcount].salesorderitemext_psproductid = Convert.ToString(dr["SalesorderItemExt_PsProductID"]);
                    sp[_dsRcount].salesorderitemext_psmproductid = Convert.ToString(dr["SalesorderItemExt_PsmProductID"]);
                    sp[_dsRcount].salesorderitemext_psoriprice = Convert.ToInt32(dr["SalesorderItemExt_PsoriPrice"]);
                    sp[_dsRcount].salesorderitemext_pssellcatid = Convert.ToString(dr["SalesorderItemExt_PsSellcatID"]);
                    sp[_dsRcount].salesorderitemext_psattribname = Convert.ToString(dr["SalesorderItemExt_PsAttribName"]);
                    sp[_dsRcount].salesorderitemext_psmodelno = Convert.ToString(dr["SalesorderItemExt_PsModelNO"]);
                    sp[_dsRcount].salesorderitemext_pscost = Convert.ToInt32(dr["SalesorderItemExt_PsCost"]);
                    sp[_dsRcount].salesorderitemext_psfvf = Convert.ToInt32(dr["SalesorderItemExt_Psfvf"]);

                    //ViewBag.sp = sp[_dsRcount];

                    salesOrders.Add(sp[_dsRcount]);
                    _dsRcount++;
                }//end foreach*/
            }//end if (dsResult != null && dsResult.Tables.Count > 0)

            return salesOrders;
        }



        public Dictionary<string, decimal> GetDiscountData(List<string> allSOItemCodes)
        {
            Dictionary<string, decimal> discountData = new Dictionary<string, decimal>();
            TWNewEgg.DB.TWSqlDBContext twSqlDB = new DB.TWSqlDBContext();
            var allPromotionRecords = twSqlDB.PromotionGiftRecords.Where(x => allSOItemCodes.Contains(x.SalesOrderItemCode)).ToList();
            var recoredPromotionBasicIDs = allPromotionRecords.Select(y => y.PromotionGiftBasicID).Distinct();
            var allPromotionBasicIDs = twSqlDB.PromotionGiftBasic.Where(x => recoredPromotionBasicIDs.Contains(x.ID)).ToList();
            foreach (var promationID in allPromotionBasicIDs.Where(x => x.Priority > 0).OrderBy(x => x.Priority))
            {
                discountData.Add(promationID.ShowDesc, allPromotionRecords.Where(x => x.PromotionGiftBasicID == promationID.ID).Sum(x => x.ApportionedAmount));
            }
            foreach (var promationID in allPromotionBasicIDs.Where(x => x.Priority == 0).OrderBy(x => x.StartDate))
            {
                discountData.Add(promationID.ShowDesc, allPromotionRecords.Where(x => x.PromotionGiftBasicID == promationID.ID).Sum(x => x.ApportionedAmount));
            }
            return discountData;
        }
    }
}