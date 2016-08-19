using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Threading.Tasks;
using System.IO;
using System.Diagnostics;
using TWNewEgg.DB;
using TWNewEgg.DB.TWSQLDB.Models;
using TWNewEgg.SearchService.Models;
using TWNewEgg.CategoryService.Service;
using TWNewEgg.CategoryService.Models;
using TWNewEgg.DB.TWSQLDB.Models.ExtModels;
using System.Data.Entity.Infrastructure;

namespace TWNewEgg.SearchService.Service
{
    public class SearchService : IDisposable
    {
        private BaseService oCategoryController = new BaseService();
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                oCategoryController.Dispose();
                //oCategoryController = null;
            }
        }
        
        private readonly int DEFAULTTAKENUMBER = 10000;
        private readonly string SEARCHITEMSTART = "itemid:{";
        private readonly string SEARCHITEMEND = "}";

        private Dictionary<string, List<int?>> KeyWordandCategory = new Dictionary<string, List<int?>>();
        private Dictionary<string, List<int?>> KeyWordandItemID = new Dictionary<string, List<int?>>();
        private Dictionary<string, List<int?>> KeyWordOnlyItemID = new Dictionary<string, List<int?>>();

        private void SetKeyWordShowing() //All key words must use lower case
        {
            //KeyWordandCategory.Add("pandora", new List<int?> { 338 }); //If Key word contains pandora, then show category id 338's item firstly.
        }
        private void SetKeyWordOnlyItemID() //All key words must use lower case
        {
            //KeyWordOnlyItemID.Add("beats studio", new List<int?> { 39305, 7949 }); //If Key word contains pandora, then show category id 338's item firstly.
        }

        public enum OrderCondition
        {
            PriceHigh2Low = 0,
            PriceLow2High = 1,
            MostRelate = 2,
            MostPolular = 3,
            StockHigh2Low = 4,
            CreateNew2Old = 5,
            CreateOld2New = 6
        };

        public List<ItemSearch> SearchResult(string SearchWord, string SrchIn, int? Order, int? Cat, int? LID, int? Cty, int? BID, int? SID, int? minPrice, int? maxPrice, int PageSize, int? Page, string Mode, string Submit)
        {
            TWSqlDBContext db = new TWSqlDBContext();
            /// <summary>
            /// Search item ,then sort it,
            /// </summary>
            /// <param name="1">Cookie name</param>
            /// <returns>URL</returns>
            List<int?> isInKeyWordCatrgory = new List<int?>();
            List<int?> isInKeyWordOnlyItem = new List<int?>();
            SetKeyWordShowing();
            SetKeyWordOnlyItemID();
            //if (Page == null)
            //    Page = 0;
            //Page += 1;
            //int pagenumber = PageSize * (int)Page;
            if (SearchWord == null)
            {
                SearchWord = "";
            }
            char[] delimiterChars = { ' ' };
            Searchitem Is = new Searchitem();
            //全部的東西
            List<ItemSearch> AllIs = new List<ItemSearch>();
            List<ItemSearch> AllIsTemp = new List<ItemSearch>();
            List<ItemSearch> AllIsAccu = new List<ItemSearch>();
            List<ItemSearch> AllIsTotalAccu = new List<ItemSearch>();
            List<ItemSearch> AllInDictionary = new List<ItemSearch>();
            List<ItemSearch> Count = new List<ItemSearch>();

            //計算不同欄位的數量
            List<ItemSearch> title = new List<ItemSearch>();
            List<ItemSearch> manufacture = new List<ItemSearch>();
            List<ItemSearch> ItemName = new List<ItemSearch>();
            List<ItemSearch> productName = new List<ItemSearch>();
            List<ItemSearch> sellerProductID = new List<ItemSearch>();
            //List<ItemSearch> productModel = new List<ItemSearch>();
            //List<ItemSearch> itemDesc = new List<ItemSearch>();

            List<ItemSearch> titleAccu = new List<ItemSearch>();
            List<ItemSearch> manufactureAccu = new List<ItemSearch>();
            List<ItemSearch> ItemNameAccu = new List<ItemSearch>();
            List<ItemSearch> productNameAccu = new List<ItemSearch>();
            List<ItemSearch> sellerProductIDAccu = new List<ItemSearch>();

            string[] keyword = DelEmpty(SearchWord.Split(delimiterChars)).ToArray();
            List<string> srchInkeyword = new List<string>();
            if (SrchIn != null)
            {
                srchInkeyword = DelEmpty(SrchIn.Split(delimiterChars));
                //srchInkeyword = DelEmptyAndPrecise(SrchIn.Split(delimiterChars)).ToList();
            }
            string totalKeyWord = SearchWord.Trim();
            //var test = db.ItemSearch.Select(x => x.Name.ToLower()).Where(x => keyword.All(y => x.Contains(y))).ToList();// test function
            //Find all of child categories in this category
            List<int?> allChildCategory = new List<int?>();
            if (Cat != null && LID != null)
            {
                if (!GetChildCategory(allChildCategory, Cat, LID, Cty, BID, SID))
                {
                    allChildCategory.Clear();
                }
            }

            if (KeyWordOnlyItemID.ContainsKey(totalKeyWord.ToLower()))
            {
                isInKeyWordOnlyItem.AddRange(KeyWordOnlyItemID[totalKeyWord.ToLower()]);
                AllIs.AddRange(db.ItemSearch.Where(x => isInKeyWordOnlyItem.Contains(x.ID)).ToList());
                Is.ISnumber = AllIs.Count;
            }
            else
            {

                //分別從 item.item_name , category.category_title , manufacture.manufacture_name , ProductName , SellerProductID N欄比對搜尋
                foreach (string a in keyword)
                {
                    if (minPrice == null & maxPrice == null)
                    {
                        if (allChildCategory.Count > 0)
                        {
                            /*AllIs = db.ItemSearch.Where(x => (x.category_title.Contains(a) || x.manufacture_name.Contains(a) || x.item_name.Contains(a) || x.product_model.Contains(a) || x.item_sdesc.Contains(a))
                                                           && allChildCategory.Contains(x.category_id)  ).ToList<ItemSearch>();*/
                            /*title = db.ItemSearch.Where(x => x.category_title.Contains(a) && allChildCategory.Contains(x.category_id)).ToList<ItemSearch>();
                            manufacture = db.ItemSearch.Where(x => x.manufacture_name.Contains(a) && allChildCategory.Contains(x.category_id)).ToList<ItemSearch>();
                            ItemName = db.ItemSearch.Where(x => x.item_name.Contains(a) && allChildCategory.Contains(x.category_id)).ToList<ItemSearch>();
                            productModel = db.ItemSearch.Where(x => x.product_model.Contains(a) && allChildCategory.Contains(x.category_id)).ToList<ItemSearch>();
                            itemDesc = db.ItemSearch.Where(x => x.item_sdesc.Contains(a) && allChildCategory.Contains(x.category_id)).ToList<ItemSearch>();
                            */
                            title.AddRange(db.ItemSearch.Where(x => x.CategoryTitle.Contains(a) && allChildCategory.Contains(x.CategoryID)).ToList<ItemSearch>());
                            manufacture.AddRange(db.ItemSearch.Where(x => x.ManufactureName.Contains(a) && allChildCategory.Contains(x.CategoryID)).ToList<ItemSearch>());
                            ItemName.AddRange(db.ItemSearch.Where(x => x.Name.Contains(a) && allChildCategory.Contains(x.CategoryID)).ToList<ItemSearch>());
                            productName.AddRange(db.ItemSearch.Where(x => x.ProductName.Contains(a) && allChildCategory.Contains(x.CategoryID)).ToList<ItemSearch>());
                            //productModel.AddRange(db.ItemSearch.Where(x => x.product_model.Contains(a) && allChildCategory.Contains(x.category_id)).ToList<ItemSearch>());
                            //itemDesc.AddRange(db.ItemSearch.Where(x => x.item_sdesc.Contains(a) && allChildCategory.Contains(x.category_id)).ToList<ItemSearch>());
                        }
                        else
                        {
                            //AllIs = db.ItemSearch.Where(x => x.category_title.Contains(a) || x.manufacture_name.Contains(a) || x.item_name.Contains(a) || x.product_model.Contains(a) || x.item_sdesc.Contains(a)).ToList<ItemSearch>();
                            /*title = db.ItemSearch.Where(x => x.category_title.Contains(a)).ToList<ItemSearch>();
                            manufacture = db.ItemSearch.Where(x => x.manufacture_name.Contains(a)).ToList<ItemSearch>();
                            ItemName = db.ItemSearch.Where(x => x.item_name.Contains(a)).ToList<ItemSearch>();
                            productModel = db.ItemSearch.Where(x => x.product_model.Contains(a)).ToList<ItemSearch>();
                            itemDesc = db.ItemSearch.Where(x => x.item_sdesc.Contains(a)).ToList<ItemSearch>();*/
                            title.AddRange(db.ItemSearch.Where(x => x.CategoryTitle.Contains(a)).ToList<ItemSearch>());
                            manufacture.AddRange(db.ItemSearch.Where(x => x.ManufactureName.Contains(a)).ToList<ItemSearch>());
                            ItemName.AddRange(db.ItemSearch.Where(x => x.Name.Contains(a)).ToList<ItemSearch>());
                            productName.AddRange(db.ItemSearch.Where(x => x.ProductName.Contains(a)).ToList<ItemSearch>());
                            //productModel.AddRange(db.ItemSearch.Where(x => x.product_model.Contains(a)).ToList<ItemSearch>());
                            //itemDesc.AddRange(db.ItemSearch.Where(x => x.item_sdesc.Contains(a)).ToList<ItemSearch>());
                        }
                    }
                    else if (minPrice != null & maxPrice != null)
                    {
                        if (allChildCategory.Count > 0)
                        {
                            /*AllIs = db.ItemSearch.Where( x => (x.category_title.Contains(a) || x.manufacture_name.Contains(a) || x.item_name.Contains(a) || x.product_model.Contains(a) || x.item_sdesc.Contains(a))
                                                          && x.item_pricecash >= minPrice.Value && x.item_pricecash <= maxPrice.Value && allChildCategory.Contains(x.category_id)   ).ToList<ItemSearch>();*/
                            /*title = db.ItemSearch.Where(x => x.category_title.Contains(a) && x.item_pricecash >= minPrice.Value && x.item_pricecash <= maxPrice.Value && allChildCategory.Contains(x.category_id)).ToList<ItemSearch>();
                            manufacture = db.ItemSearch.Where(x => x.manufacture_name.Contains(a) && x.item_pricecash >= minPrice.Value && x.item_pricecash <= maxPrice.Value && allChildCategory.Contains(x.category_id)).ToList<ItemSearch>();
                            ItemName = db.ItemSearch.Where(x => x.item_name.Contains(a) && x.item_pricecash >= minPrice.Value && x.item_pricecash <= maxPrice.Value && allChildCategory.Contains(x.category_id)).ToList<ItemSearch>();
                            productModel = db.ItemSearch.Where(x => x.product_model.Contains(a) && x.item_pricecash >= minPrice.Value && x.item_pricecash <= maxPrice.Value && allChildCategory.Contains(x.category_id)).ToList<ItemSearch>();
                            itemDesc = db.ItemSearch.Where(x => x.item_sdesc.Contains(a) && x.item_pricecash >= minPrice.Value && x.item_pricecash <= maxPrice.Value && allChildCategory.Contains(x.category_id)).ToList<ItemSearch>();*/
                            title.AddRange(db.ItemSearch.Where(x => x.CategoryTitle.Contains(a) && x.Pricecash >= minPrice.Value && x.Pricecash <= maxPrice.Value && allChildCategory.Contains(x.CategoryID)).ToList<ItemSearch>());
                            manufacture.AddRange(db.ItemSearch.Where(x => x.ManufactureName.Contains(a) && x.Pricecash >= minPrice.Value && x.Pricecash <= maxPrice.Value && allChildCategory.Contains(x.CategoryID)).ToList<ItemSearch>());
                            ItemName.AddRange(db.ItemSearch.Where(x => x.Name.Contains(a) && x.Pricecash >= minPrice.Value && x.Pricecash <= maxPrice.Value && allChildCategory.Contains(x.CategoryID)).ToList<ItemSearch>());
                            productName.AddRange(db.ItemSearch.Where(x => x.ProductName.Contains(a) && x.Pricecash >= minPrice.Value && x.Pricecash <= maxPrice.Value && allChildCategory.Contains(x.CategoryID)).ToList<ItemSearch>());
                            //productModel.AddRange(db.ItemSearch.Where(x => x.product_model.Contains(a) && x.item_pricecash >= minPrice.Value && x.item_pricecash <= maxPrice.Value && allChildCategory.Contains(x.category_id)).ToList<ItemSearch>());
                            //itemDesc.AddRange(db.ItemSearch.Where(x => x.item_sdesc.Contains(a) && x.item_pricecash >= minPrice.Value && x.item_pricecash <= maxPrice.Value && allChildCategory.Contains(x.category_id)).ToList<ItemSearch>());
                        }
                        else
                        {
                            /*AllIs = db.ItemSearch.Where(x => (x.category_title.Contains(a) || x.manufacture_name.Contains(a) || x.item_name.Contains(a) || x.product_model.Contains(a) || x.item_sdesc.Contains(a))
                                                          && x.item_pricecash >= minPrice.Value && x.item_pricecash <= maxPrice.Value  ).ToList<ItemSearch>();*/
                            /*title = db.ItemSearch.Where(x => x.category_title.Contains(a) && x.item_pricecash >= minPrice.Value && x.item_pricecash <= maxPrice.Value).ToList<ItemSearch>();
                            manufacture = db.ItemSearch.Where(x => x.manufacture_name.Contains(a) && x.item_pricecash >= minPrice.Value && x.item_pricecash <= maxPrice.Value).ToList<ItemSearch>();
                            ItemName = db.ItemSearch.Where(x => x.item_name.Contains(a) && x.item_pricecash >= minPrice.Value && x.item_pricecash <= maxPrice.Value).ToList<ItemSearch>();
                            productModel = db.ItemSearch.Where(x => x.product_model.Contains(a) && x.item_pricecash >= minPrice.Value && x.item_pricecash <= maxPrice.Value).ToList<ItemSearch>();
                            itemDesc = db.ItemSearch.Where(x => x.item_sdesc.Contains(a) && x.item_pricecash >= minPrice.Value && x.item_pricecash <= maxPrice.Value).ToList<ItemSearch>();*/
                            title.AddRange(db.ItemSearch.Where(x => x.CategoryTitle.Contains(a) && x.Pricecash >= minPrice.Value && x.Pricecash <= maxPrice.Value).ToList<ItemSearch>());
                            manufacture.AddRange(db.ItemSearch.Where(x => x.ManufactureName.Contains(a) && x.Pricecash >= minPrice.Value && x.Pricecash <= maxPrice.Value).ToList<ItemSearch>());
                            ItemName.AddRange(db.ItemSearch.Where(x => x.Name.Contains(a) && x.Pricecash >= minPrice.Value && x.Pricecash <= maxPrice.Value).ToList<ItemSearch>());
                            productName.AddRange(db.ItemSearch.Where(x => x.ProductName.Contains(a) && x.Pricecash >= minPrice.Value && x.Pricecash <= maxPrice.Value).ToList<ItemSearch>());
                            //productModel.AddRange(db.ItemSearch.Where(x => x.product_model.Contains(a) && x.item_pricecash >= minPrice.Value && x.item_pricecash <= maxPrice.Value).ToList<ItemSearch>());
                            //itemDesc.AddRange(db.ItemSearch.Where(x => x.item_sdesc.Contains(a) && x.item_pricecash >= minPrice.Value && x.item_pricecash <= maxPrice.Value).ToList<ItemSearch>());
                        }
                    }
                    titleAccu.AddRange(title.Where(x => SearchKey(x.CategoryTitle, a) == true).ToList());
                    manufactureAccu.AddRange(manufacture.Where(x => SearchKey(x.ManufactureName, a) == true).ToList());
                    ItemNameAccu.AddRange(ItemName.Where(x => SearchKey(x.Name, a) == true).ToList());
                    productNameAccu.AddRange(productName.Where(x => SearchKey(x.ProductName, a) == true).ToList());
                    if (KeyWordandCategory.ContainsKey(a.ToLower()))
                    {
                        isInKeyWordCatrgory.AddRange(KeyWordandCategory[a.ToLower()]);
                    }
                }
                //sellerProductID.AddRange(db.ItemSearch.Where(x => x.SellerProductID.Contains(totalKeyWord)));
                sellerProductID.AddRange(db.ItemSearch.Where(x => x.SellerProductID == totalKeyWord));

                var tttt = db.ItemSearch.Where(x => keyword.Contains(x.Name)).ToList();

                List<int> TMIPID = new List<int>();
                List<int> TMIPAccuID = new List<int>();
                if (keyword.Count() > 1)
                {
                    var TMIPTitle = title.Select(x => new { ID = x.ID, NAME = x.CategoryTitle + " " + x.Name + " " + x.ProductName + " " + x.ManufactureName }).ToList();
                    TMIPTitle = TMIPTitle.Where(x => keyword.All(y => x.NAME.ToLower().Contains(y))).ToList();
                    TMIPID = TMIPTitle.Select(x => x.ID).ToList();
                    title = title.Where(x => TMIPID.Contains(x.ID)).ToList();

                    var TMIPManufacture = manufacture.Select(x => new { ID = x.ID, NAME = x.CategoryTitle + " " + x.Name + " " + x.ProductName + " " + x.ManufactureName }).ToList();
                    TMIPManufacture = TMIPManufacture.Where(x => keyword.All(y => x.NAME.ToLower().Contains(y))).ToList();
                    TMIPID = TMIPManufacture.Select(x => x.ID).ToList();
                    manufacture = manufacture.Where(x => TMIPID.Contains(x.ID)).ToList();

                    var TMIPItemName = ItemName.Select(x => new { ID = x.ID, NAME = x.CategoryTitle + " " + x.Name + " " + x.ProductName + " " + x.ManufactureName }).ToList();
                    TMIPItemName = TMIPItemName.Where(x => keyword.All(y => x.NAME.ToLower().Contains(y))).ToList();
                    TMIPID = TMIPItemName.Select(x => x.ID).ToList();
                    ItemName = ItemName.Where(x => TMIPID.Contains(x.ID)).ToList();

                    var TMIPProductName = productName.Select(x => new { ID = x.ID, NAME = x.CategoryTitle + " " + x.Name + " " + x.ProductName + " " + x.ManufactureName }).ToList();
                    TMIPProductName = TMIPProductName.Where(x => keyword.All(y => x.NAME.ToLower().Contains(y))).ToList();
                    TMIPID = TMIPProductName.Select(x => x.ID).ToList();
                    productName = productName.Where(x => TMIPID.Contains(x.ID)).ToList();



                    //TMIPID.AddRange(TMIPList.Select(x => x.ID).ToList());
                    var TMIPItemNameAccu = ItemNameAccu.Select(x => new { ID = x.ID, NAME = x.CategoryTitle + " " + x.Name + " " + x.ProductName + " " + x.ManufactureName }).ToList();
                    TMIPItemNameAccu = TMIPItemNameAccu.Where(x => keyword.All(y => x.NAME.ToLower().Contains(y))).ToList();
                    TMIPID = TMIPItemNameAccu.Select(x => x.ID).ToList();
                    ItemNameAccu = ItemNameAccu.Where(x => TMIPID.Contains(x.ID)).ToList();

                    var TMIPProductNameAccu = productNameAccu.Select(x => new { ID = x.ID, NAME = x.CategoryTitle + " " + x.Name + " " + x.ProductName + " " + x.ManufactureName }).ToList();
                    TMIPProductNameAccu = TMIPProductNameAccu.Where(x => keyword.All(y => x.NAME.ToLower().Contains(y))).ToList();
                    TMIPID = TMIPProductNameAccu.Select(x => x.ID).ToList();
                    productNameAccu = productNameAccu.Where(x => TMIPID.Contains(x.ID)).ToList();

                    var TMIPManufactureAccu = manufactureAccu.Select(x => new { ID = x.ID, NAME = x.CategoryTitle + " " + x.Name + " " + x.ProductName + " " + x.ManufactureName }).ToList();
                    TMIPManufactureAccu = TMIPManufactureAccu.Where(x => keyword.All(y => x.NAME.ToLower().Contains(y))).ToList();
                    TMIPID = TMIPManufactureAccu.Select(x => x.ID).ToList();
                    manufactureAccu = manufactureAccu.Where(x => TMIPID.Contains(x.ID)).ToList();

                    var TMIPTitleAccu = titleAccu.Select(x => new { ID = x.ID, NAME = x.CategoryTitle + " " + x.Name + " " + x.ProductName + " " + x.ManufactureName }).ToList();
                    TMIPTitleAccu = TMIPTitleAccu.Where(x => keyword.All(y => x.NAME.ToLower().Contains(y))).ToList();
                    TMIPID = TMIPTitleAccu.Select(x => x.ID).ToList();
                    titleAccu = titleAccu.Where(x => TMIPID.Contains(x.ID)).ToList();

                }


                /*
                List<int> TMIPID = new List<int>();
                List<int> TMIPAccuID = new List<int>();

                    var TMIP = title.Select(x => new { ID = x.ID, NAME = x.CategoryTitle + " " + x.Name + " " + x.ProductName + " " + x.ManufactureName }).ToList();
                    TMIP.AddRange(manufacture.Select(x => new { ID = x.ID, NAME = x.CategoryTitle + " " + x.Name + " " + x.ProductName + " " + x.ManufactureName }).ToList());
                    TMIP.AddRange(ItemName.Select(x => new { ID = x.ID, NAME = x.CategoryTitle + " " + x.Name + " " + x.ProductName + " " + x.ManufactureName }).ToList());
                    TMIP.AddRange(productName.Select(x => new { ID = x.ID, NAME = x.CategoryTitle + " " + x.Name + " " + x.ProductName + " " + x.ManufactureName }).ToList());

                    TMIP = TMIP.Where(x => srchInkeyword.All(y => x.NAME.ToLower().Contains(y))).ToList();
                    TMIPID.AddRange(TMIP.Select(x => x.ID).ToList());

                    var TMIPAccu = ItemNameAccu.Select(x => new { ID = x.ID, NAME = x.CategoryTitle + " " + x.Name + " " + x.ProductName + " " + x.ManufactureName }).ToList();
                    TMIPAccu.AddRange(productNameAccu.Select(x => new { ID = x.ID, NAME = x.CategoryTitle + " " + x.Name + " " + x.ProductName + " " + x.ManufactureName }).ToList());
                    TMIPAccu.AddRange(manufactureAccu.Select(x => new { ID = x.ID, NAME = x.CategoryTitle + " " + x.Name + " " + x.ProductName + " " + x.ManufactureName }).ToList());
                    TMIPAccu.AddRange(titleAccu.Select(x => new { ID = x.ID, NAME = x.CategoryTitle + " " + x.Name + " " + x.ProductName + " " + x.ManufactureName }).ToList());

                    TMIPAccu = TMIPAccu.Where(x => srchInkeyword.All(y => x.NAME.ToLower().Contains(y))).ToList();
                    TMIPAccuID.AddRange(TMIPAccu.Select(x => x.ID).ToList());

                    AllIs = AllIs.Where(x => TMIPID.Contains(x.ID)).ToList();
                    AllIsAccu = AllIsAccu.Where(x => TMIPAccuID.Contains(x.ID)).ToList();
                */


                AllIs.AddRange(title);
                AllIs.AddRange(manufacture);
                AllIs.AddRange(ItemName);
                AllIs.AddRange(productName);
                AllIs.AddRange(sellerProductID);
                //AllIs.AddRange(productModel);
                //AllIs.AddRange(itemDesc);
                AllIsAccu.AddRange(ItemNameAccu);
                AllIsAccu.AddRange(productNameAccu);
                AllIsAccu.AddRange(manufactureAccu);
                AllIsAccu.AddRange(titleAccu);


                List<int> SrchInTMIPID = new List<int>();
                List<int> SrchInTMIPAccuID = new List<int>();
                if (srchInkeyword.Count > 0)
                {
                    var SrchInTMIP = title.Select(x => new { ID = x.ID, NAME = x.CategoryTitle + " " + x.Name + " " + x.ProductName + " " + x.ManufactureName }).ToList();
                    SrchInTMIP.AddRange(manufacture.Select(x => new { ID = x.ID, NAME = x.CategoryTitle + " " + x.Name + " " + x.ProductName + " " + x.ManufactureName }).ToList());
                    SrchInTMIP.AddRange(ItemName.Select(x => new { ID = x.ID, NAME = x.CategoryTitle + " " + x.Name + " " + x.ProductName + " " + x.ManufactureName }).ToList());
                    SrchInTMIP.AddRange(productName.Select(x => new { ID = x.ID, NAME = x.CategoryTitle + " " + x.Name + " " + x.ProductName + " " + x.ManufactureName }).ToList());

                    SrchInTMIP = SrchInTMIP.Where(x => srchInkeyword.All(y => x.NAME.ToLower().Contains(y))).ToList();
                    SrchInTMIPID.AddRange(SrchInTMIP.Select(x => x.ID).ToList());

                    var SrchInTMIPAccu = ItemNameAccu.Select(x => new { ID = x.ID, NAME = x.CategoryTitle + " " + x.Name + " " + x.ProductName + " " + x.ManufactureName }).ToList();
                    SrchInTMIPAccu.AddRange(productNameAccu.Select(x => new { ID = x.ID, NAME = x.CategoryTitle + " " + x.Name + " " + x.ProductName + " " + x.ManufactureName }).ToList());
                    SrchInTMIPAccu.AddRange(manufactureAccu.Select(x => new { ID = x.ID, NAME = x.CategoryTitle + " " + x.Name + " " + x.ProductName + " " + x.ManufactureName }).ToList());
                    SrchInTMIPAccu.AddRange(titleAccu.Select(x => new { ID = x.ID, NAME = x.CategoryTitle + " " + x.Name + " " + x.ProductName + " " + x.ManufactureName }).ToList());

                    SrchInTMIPAccu = SrchInTMIPAccu.Where(x => srchInkeyword.All(y => x.NAME.ToLower().Contains(y))).ToList();
                    SrchInTMIPAccuID.AddRange(SrchInTMIPAccu.Select(x => x.ID).ToList());

                    AllIs = AllIs.Where(x => SrchInTMIPID.Contains(x.ID)).ToList();
                    AllIsAccu = AllIsAccu.Where(x => SrchInTMIPAccuID.Contains(x.ID)).ToList();
                }



                //AllIsAccu.AddRange(titleAccu); //different order 
                //AllIsAccu.AddRange(manufactureAccu);
                //AllIsAccu.AddRange(ItemNameAccu);
                AllInDictionary.AddRange(AllIsAccu.Where(x => isInKeyWordCatrgory.Contains(x.CategoryID)).ToList());

                AllIs = AllIs.Distinct().ToList();
                if (keyword.Count() > 1)
                {
                    AllIsTotalAccu.AddRange(AllIs.Where(x => SearchKey(x.Name, totalKeyWord, true) == true).ToList());
                    AllIsTotalAccu.AddRange(AllIs.Where(x => SearchKey(x.ProductName, totalKeyWord, true) == true).ToList());
                }
                else
                {
                    AllIsTotalAccu.AddRange(AllIs.Where(x => SearchKey(x.Name, totalKeyWord) == true).ToList());
                    AllIsTotalAccu.AddRange(AllIs.Where(x => SearchKey(x.ProductName, totalKeyWord) == true).ToList());
                    AllIsTotalAccu.AddRange(AllIs.Where(x => SearchKey(x.SellerProductID, totalKeyWord) == true).ToList());
                }
                AllInDictionary = AllInDictionary.Distinct().OrderByDescending(x => x.Qtyreg).ToList();
                List<int> AllInDictionaryID = AllInDictionary.Select(x => x.ID).ToList();
                AllIsTotalAccu.RemoveAll(x => AllInDictionaryID.Contains(x.ID));
                AllIsTotalAccu = AllIsTotalAccu.Distinct().OrderByDescending(x => x.Qtyreg).ToList();
                List<int> AllIsTotalIDAccu = AllIsTotalAccu.Select(x => x.ID).ToList();
                //AllIsAccu = AllIsAccu.Distinct().OrderByDescending(x => x.item_qtyreg).ToList(); // don' order by selling qty
                AllIsAccu = AllIsAccu.Distinct().ToList();
                AllIsAccu.RemoveAll(x => AllIsTotalIDAccu.Contains(x.ID));
                List<int> AllIsIDAccu = AllIsAccu.Select(x => x.ID).ToList();
                AllIsIDAccu.AddRange(AllIsTotalIDAccu);

                Is.ISnumber = AllIs.Count;
                AllIs.RemoveAll(x => AllIsIDAccu.Contains(x.ID));
                AllIsTemp.AddRange(AllIs);
                AllIs.Clear();
                AllIs.AddRange(AllInDictionary);
                AllIs.AddRange(AllIsTotalAccu);
                AllIs.AddRange(AllIsAccu);
                AllIs.AddRange(AllIsTemp);
                AllIsTemp.Clear();
            }
            //2.排序
            switch (Order)
            {
                case 0:
                    //價格由低到高
                    AllIs = AllIs.OrderBy(x => x.Pricecash).ToList<ItemSearch>();
                    break;
                case 1:
                    //價格由高到低
                    AllIs = AllIs.OrderBy(x => -x.Pricecash).ToList<ItemSearch>();
                    break;
                case 2:
                    //最相關(新增至AllIs時，就已經是做最相關排序，所以這邊不做任何動作_Bill) //搜尋預設值是最相關
                    /*foreach (string b in keyword)
                    {
                        title = AllIs.Where(x => x.manufacture_name != null && x.category_title.IndexOf(b, StringComparison.OrdinalIgnoreCase) > -1).Count();
                        manufacture = AllIs.Where(x => x.manufacture_name != null && x.manufacture_name.IndexOf(b, StringComparison.OrdinalIgnoreCase) > -1).Count();
                        ItemName = AllIs.Where(x => x.item_name != null && x.item_name.IndexOf(b, StringComparison.OrdinalIgnoreCase) > -1).Count();
                        productModel = AllIs.Where(x => x.product_model != null && x.product_model.IndexOf(b, StringComparison.OrdinalIgnoreCase) > -1).Count();
                        itemDesc = AllIs.Where(x => x.item_sdesc != null && x.item_sdesc.IndexOf(b, StringComparison.OrdinalIgnoreCase) > -1).Count();
                    }*/

                    /*
                    if (title.Count >= manufacture.Count)
                    {
                        if (manufacture.Count >= ItemName.Count)
                        {
                            AllIs = AllIs.OrderByDescending(x => x.item_name.Contains(SearchWord) || x.manufacture_name.Contains(SearchWord) || x.category_title.Contains(SearchWord)).ThenBy(x => x.manufacture_name.Contains(keyword[0])).ThenBy(x => x.category_title.Contains(keyword[0])).ToList<ItemSearch>();
                            
                        }
                        else if (ItemName.Count >= title.Count)
                        {
                            AllIs = AllIs.OrderByDescending(x => x.item_name.Contains(SearchWord) || x.manufacture_name.Contains(SearchWord) || x.category_title.Contains(SearchWord)).ThenBy(x => x.item_name.Contains(keyword[0])).ThenBy(x => x.category_title.Contains(keyword[0])).ToList<ItemSearch>();
                        }
                        else
                        {
                            AllIs = AllIs.OrderByDescending(x => x.item_name.Contains(SearchWord) || x.manufacture_name.Contains(SearchWord) || x.category_title.Contains(SearchWord)).ThenBy(x => x.category_title.Contains(keyword[0])).ThenBy(x => x.item_name.Contains(keyword[0])).ToList<ItemSearch>();
                        }
                    }
                    else
                    {
                        if (title.Count >= ItemName.Count)
                        {
                            AllIs = AllIs.OrderByDescending(x => x.item_name.Contains(SearchWord) || x.manufacture_name.Contains(SearchWord) || x.category_title.Contains(SearchWord)).ThenBy(x => x.manufacture_name.Contains(keyword[0])).ThenBy(x => x.category_title.Contains(keyword[0])).ToList<ItemSearch>();
                        }
                        else if (ItemName.Count >= manufacture.Count)
                        {
                            AllIs = AllIs.OrderByDescending(x => x.item_name.Contains(SearchWord) || x.manufacture_name.Contains(SearchWord) || x.category_title.Contains(SearchWord)).ThenBy(x => x.item_name.Contains(keyword[0])).ThenBy(x => x.manufacture_name.Contains(keyword[0])).ToList<ItemSearch>();
                        }
                        else
                        {
                            AllIs = AllIs.OrderByDescending(x => x.item_name.Contains(SearchWord) || x.manufacture_name.Contains(SearchWord) || x.category_title.Contains(SearchWord)).ThenBy(x => x.manufacture_name.Contains(keyword[0])).ThenBy(x => x.item_name.Contains(keyword[0])).ToList<ItemSearch>();
                        }
                    }
                    */
                    //Is.ItemSearch = Is.ItemSearch.OrderBy(x => x.).ToList<ItemSearch>();
                    break;
                case 3:
                    //最有人氣，參考方式(銷售量?)
                    AllIs = AllIs.OrderByDescending(x => x.Qtyreg).ToList<ItemSearch>();
                    break;

            }


            //所有商品回傳
            return AllIs;
            /*
            allSearchResults.ItemSearch = AllIs;
            if (AllIs.Count < (PageSize * (int)Page))
            {
                pagenumber = AllIs.Count;
            }

            for (int i = PageSize * ((int)Page - 1); i < pagenumber; i++)
            {
                Count.Add(AllIs[i]);
            }

            Is.ItemSearch = Count;

            ICarts TestItemSellorNot = new CartsRepository();
            List<int> SellOrNot = new List<int>();

            foreach (ItemSearch s in Is.ItemSearch)
            {
                int sellingQTY = TestItemSellorNot.GetSellingQty((int)s.ID, "Item");
                ViewBag.Qty = Math.Min(20, sellingQTY);
                if (sellingQTY > 0 && !TestItemSellorNot.CheckDateTimeOut((int)s.ID, "Item"))
                {
                    SellOrNot.Add(1);
                }
                else
                {
                    SellOrNot.Add(0);
                    s.ItemSpechead = "此商品暫無販售";
                }

            }

            Is.SellOrNot = SellOrNot;
            return Is;
            */
        }

        private string GenerateMD5Hash(string input)
        {
            System.Security.Cryptography.MD5 md5Hash = System.Security.Cryptography.MD5.Create();
            // Convert the input string to a byte array and compute the hash. 
            byte[] data = md5Hash.ComputeHash(Encoding.UTF8.GetBytes(input));

            // Create a new Stringbuilder to collect the bytes 
            // and create a string.
            StringBuilder sBuilder = new StringBuilder();

            // Loop through each byte of the hashed data  
            // and format each one as a hexadecimal string. 
            for (int i = 0; i < data.Length; i++)
            {
                sBuilder.Append(data[i].ToString("x2"));
            }
            md5Hash.Dispose();
            // Return the hexadecimal string. 
            return sBuilder.ToString();
        }
        private bool CheckCacheIsInLifeTime(string fullPath, int mins)
        {
            bool isInLifeTime = false;
            DateTime cacheFileDate = TWNewEgg.Framework.Common.FileUtility.GetLastWriteTime(fullPath).AddMinutes(mins);
            DateTime datetimeNow = DateTime.UtcNow.AddHours(8);
            if (DateTime.Compare(cacheFileDate, datetimeNow) >= 0)
            {
                isInLifeTime = true;
            }
            return isInLifeTime;
        }

        public SearchResults TotalSearch(SearchApiModel searchCondition)
        {
            SearchResults returnResults = new SearchResults();
            List<ItemSearch> searchResults;

            searchResults = GetSearchResults(searchCondition);
            returnResults.resultCount = searchResults.Count;

            List<SearchCategory> categoryNumber = new List<SearchCategory>(); //for saved all item's category
            Dictionary<string, SearchCategory> hotCategory = new Dictionary<string, SearchCategory>();//for saved upper level category
            if (searchResults.Count > 0)
            {
                categoryNumber = GetCategoryNumber(searchResults, searchCondition.Cat, searchCondition.LID, searchCondition.Cty, searchCondition.BID, searchCondition.SID); //get all items' cateogry
                categoryNumber.RemoveAll(x => x.categoryDescr == "None" && x.categoryName == "None" && x.categoryShowOrder == 999);
                categoryNumber = categoryNumber.OrderBy(x => x.categoryShowOrder).ThenByDescending(x => x.number).ToList();
            }
            hotCategory = GetHotCategoryNumber(categoryNumber, searchCondition.LID); //combine all results category to tree struct
            //ViewBag.hotCategory = hotCategory;
            if (categoryNumber.Count > 0 && categoryNumber.Count < 2) //if search results items's category only one, then check search condotion, if there have no condition then show it, if yes then clear it, coz don't need it anymore
            {
                if (searchCondition.Cat != null && searchCondition.LID != null && categoryNumber[0].categoryID == searchCondition.Cat.Value.ToString() && categoryNumber[0].layer == searchCondition.LID.Value.ToString())
                {
                    categoryNumber.Clear();
                }
            }

            Dictionary<string, List<int>> searchPrice = new Dictionary<string, List<int>>(); //First int is number, second int is low price, third int is high price
            if (searchResults.Count > 0)
            {
                decimal maximunPrice = searchResults.Max(x => x.Pricecash).Value;
                searchPrice = CountNumberPrice(maximunPrice, searchResults);
            }

            searchResults = GetPageSearchItem(searchCondition, searchResults);

            returnResults.searchResults = searchResults;
            returnResults.hotCategory = hotCategory;
            returnResults.searchResultCategory = categoryNumber;
            returnResults.searchResultsPrice = searchPrice;
            return returnResults;
        }

        private List<ItemSearch> GetPageSearchItem(SearchApiModel searchCondition, List<ItemSearch> searchResults)
        {
            List<ItemSearch> pageSearchItem = new List<ItemSearch>();
            if (searchCondition.Page == null)
                searchCondition.Page = 0;
            searchCondition.Page += 1;
            int pagenumber = searchCondition.PageSize * (int)searchCondition.Page;

            if (searchResults.Count < pagenumber)
            {
                pagenumber = searchResults.Count;
            }

            for (int i = searchCondition.PageSize * ((int)searchCondition.Page - 1); i < pagenumber; i++)
            {
                pageSearchItem.Add(searchResults[i]);
            }

            return pageSearchItem;
        }

        private List<ItemSearch> GetSearchResults(SearchApiModel searchCondition)
        {
            List<ItemSearch> searchResults;
            string serverPath = AppDomain.CurrentDomain.BaseDirectory;
            string cacheName = string.Format("{0}_{1}_{2}",
                (searchCondition.SearchWord == null) ? "0000" : searchCondition.SearchWord.Length.ToString("D4"), 
                (searchCondition.Order == null) ? "2" : (searchCondition.Order.Value).ToString(), 
                GenerateMD5Hash(string.Format("{0}_{1}_{2}_{3}_{4}_{5}", 
                    searchCondition.SearchWord ?? "", 
                    (searchCondition.minPrice == null) ? "" : searchCondition.minPrice.Value.ToString(), 
                    (searchCondition.maxPrice == null) ? "" : searchCondition.maxPrice.Value.ToString(), 
                    searchCondition.orderCats ?? "",
                    searchCondition.SrchIn ?? "",
                    (searchCondition.Cat == null) ? "" : searchCondition.Cat.Value.ToString()
                    )));

            if (searchCondition.SearchWord == null || searchCondition.SearchWord.Length == 0)
            {
                searchResults = new List<ItemSearch>();
            }
            else if (!string.IsNullOrEmpty(searchCondition.SearchWord) && searchCondition.SearchWord.Trim().StartsWith(SEARCHITEMSTART) && searchCondition.SearchWord.Trim().EndsWith(SEARCHITEMEND))
            {
                string itemIDsString = searchCondition.SearchWord.Trim().Substring(SEARCHITEMSTART.Length);
                itemIDsString = itemIDsString.Substring(0, itemIDsString.Length - SEARCHITEMEND.Length);
                searchResults = SearchResultByItemIDs(itemIDsString);
            }
            else
            {
                string fullPath = serverPath + @"XMLSearch\" + cacheName;
                if (CheckCacheIsInLifeTime(fullPath, 150) && searchCondition.Mode != "all")
                {
                    searchResults = TWNewEgg.Framework.Common.XMLSerialization.Deserialize<List<ItemSearch>>(fullPath);
                }
                else
                {
                    searchResults = SearchResultV2(searchCondition.SearchWord, searchCondition.SrchIn, searchCondition.Order, searchCondition.Cat, searchCondition.LID, searchCondition.Cty, searchCondition.BID, searchCondition.SID, searchCondition.minPrice, searchCondition.maxPrice, searchCondition.PageSize, searchCondition.Page, searchCondition.Mode, searchCondition.Submit, searchCondition.orderCats);
                    if (searchResults != null && searchResults.Count > 0 && searchCondition.Mode != "all")
                    {
                        string xmlString = TWNewEgg.Framework.Common.XMLSerialization.Serializer(searchResults);
                        TWNewEgg.Framework.Common.FileUtility.WriteFileByPath("", "XMLSearch", cacheName, xmlString, true);
                    }
                }
            }
            return searchResults;
        }

        public List<ItemSearch> SearchResultByItemIDs(string itemIDsString)
        {
            if (string.IsNullOrEmpty(itemIDsString))
            {
                return new List<ItemSearch>();
            }
            TWSqlDBContext db = new TWSqlDBContext();
            List<ItemSearch> results = new List<ItemSearch>();
            List<int> itemIDs = new List<int>();
            var splitString = new string[] { "," };
            string[] orderCatIDsText = itemIDsString.Split(splitString, StringSplitOptions.RemoveEmptyEntries);
            foreach (var orderCatIDText in orderCatIDsText)
            {
                int orderCatID = 0;
                bool flag = int.TryParse(orderCatIDText, out orderCatID);
                if (flag)
                {
                    itemIDs.Add(orderCatID);
                }
            }
            results = db.ItemSearch.Where(x => itemIDs.Contains(x.ID)).ToList();
            return results;
        }

        public List<ItemSearch> SearchResultV2(string SearchWord, string SrchIn, int? Order, int? Cat, int? LID, int? Cty, int? BID, int? SID, int? minPrice, int? maxPrice, int PageSize, int? Page, string Mode, string Submit, string orderCats = "")
        {
#if DEBUG
            Stopwatch sw = new Stopwatch();
            long swtl = 0;
#endif
            TWSqlDBContext db = new TWSqlDBContext();
            ((IObjectContextAdapter)db).ObjectContext.CommandTimeout = 45;
            int defaultTakeNumber = DEFAULTTAKENUMBER;
            if (Mode == "all")
            {
                defaultTakeNumber = Int32.MaxValue;
            }
            /// <summary>
            /// Search item ,then sort it,
            /// </summary>
            /// <param name="1">Cookie name</param>
            /// <returns>URL</returns>
            List<int?> isInKeyWordCatrgory = new List<int?>();
            List<int?> isInKeyWordOnlyItem = new List<int?>();
            SetKeyWordShowing();
            SetKeyWordOnlyItemID();
            //if (Page == null)
            //    Page = 0;
            //Page += 1;
            //int pagenumber = PageSize * (int)Page;
            if (SearchWord == null)
            {
                SearchWord = "";
            }
            if (SrchIn == null)
            {
                SrchIn = "";
            }
            char[] delimiterChars = { ' ' };
            Searchitem SI = new Searchitem();
            //全部的東西
            List<ItemSearch> allIs = new List<ItemSearch>();
            List<ItemSearch> allIsTemp = new List<ItemSearch>();
            List<ItemSearch> allIsAccu = new List<ItemSearch>();
            List<ItemSearch> allIsTotalAccu = new List<ItemSearch>();
            List<ItemSearch> allInDictionary = new List<ItemSearch>();
            List<ItemSearch> allIsTempOrder = new List<ItemSearch>();

            //計算不同欄位的數量
            List<ItemSearch> title = new List<ItemSearch>();
            List<ItemSearch> manufacture = new List<ItemSearch>();
            List<ItemSearch> itemName = new List<ItemSearch>();
            List<ItemSearch> productName = new List<ItemSearch>();
            List<ItemSearch> sellerProductID = new List<ItemSearch>();
            //List<ItemSearch> productModel = new List<ItemSearch>();
            //List<ItemSearch> itemDesc = new List<ItemSearch>();

            List<ItemSearch> titleAccu = new List<ItemSearch>();
            List<ItemSearch> manufactureAccu = new List<ItemSearch>();
            List<ItemSearch> itemNameAccu = new List<ItemSearch>();
            List<ItemSearch> productNameAccu = new List<ItemSearch>();
            //List<ItemSearch> sellerProductIDAccu = new List<ItemSearch>();

#if DEBUG
            swtl = 0;
            sw.Reset();
            sw = Stopwatch.StartNew();
#endif
            List<string> srchInKeyword = new List<string>();
            string[] keyword = DelEmpty(SearchWord.Split(delimiterChars)).ToArray();
            //string[] preciseKeyword = DelEmptyAndPrecise(SearchWord.Split(delimiterChars)).ToArray();
            //List<string> srchInKeyword = new List<string>();
            //List<string> srchInPreciseKeyword = new List<string>();
            if (SrchIn != null)
            {
                srchInKeyword = DelEmpty(SrchIn.Split(delimiterChars));
                //srchInPreciseKeyword = DelEmptyAndPrecise(SrchIn.Split(delimiterChars));
            }
            string totalKeyWord = SearchWord.Trim();
            string totalSrchIn = SrchIn.Trim();
            //var test = db.ItemSearch.Select(x => x.Name.ToLower()).Where(x => keyword.All(y => x.Contains(y))).ToList();// test function
            //Find all of child categories in this category
            List<int?> allChildCategory = new List<int?>();
            if (Cat != null && LID != null)
            {
                if (!GetChildCategory(allChildCategory, Cat, LID, Cty, BID, SID))
                {
                    allChildCategory.Clear();
                }
            }
            if (orderCats != null && orderCats != "")//////////////////////////////////////////find order category IDs 
            {
                var splitString = new string[] { "," };
                string[] orderCatIDsText = orderCats.Split(splitString, StringSplitOptions.RemoveEmptyEntries);
                List<int> orderCategoryIDs = new List<int>();
                List<int> bottomCategoryIDs = new List<int>();
                foreach (var orderCatIDText in orderCatIDsText)
                {
                    int orderCatID = 0;
                    bool flag = int.TryParse(orderCatIDText, out orderCatID);
                    if (flag)
                    {
                        orderCategoryIDs.Add(orderCatID);
                        //allChildCategory.Add(orderCatID);
                        
                    }
                }
                bottomCategoryIDs = GetBottomChildCategory(orderCategoryIDs);
                for (int i = 0; i < bottomCategoryIDs.Count; i++)
                {
                    allChildCategory.Add(bottomCategoryIDs[i]);
                }
            }
            allChildCategory = allChildCategory.Distinct().ToList();
#if DEBUG
            sw.Stop();
            swtl += sw.ElapsedMilliseconds;
            sw.Reset();
            sw = Stopwatch.StartNew();
#endif
            if (KeyWordOnlyItemID.ContainsKey(totalKeyWord.ToLower()))
            {
                isInKeyWordOnlyItem.AddRange(KeyWordOnlyItemID[totalKeyWord.ToLower()]);
                allIs.AddRange(db.ItemSearch.Where(x => isInKeyWordOnlyItem.Contains(x.ID)).ToList());
                SI.ISnumber = allIs.Count;
            }
            else
            {
                int keywordCount = 1;
                //分別從 item.item_name , category.category_title , manufacture.manufacture_name , ProductName , SellerProductID N欄比對搜尋
                foreach (string a in keyword)
                {
                    if (keywordCount > 4)
                    {
                        break;
                    }
                    if (a != null && a.Length < 2)
                    {
                        continue;
                    }
                    if (minPrice == null & maxPrice == null)
                    {
                        if (allChildCategory.Count > 0)
                        {
                            /*AllIs = db.ItemSearch.Where(x => (x.category_title.Contains(a) || x.manufacture_name.Contains(a) || x.item_name.Contains(a) || x.product_model.Contains(a) || x.item_sdesc.Contains(a))
                                                           && allChildCategory.Contains(x.category_id)  ).ToList<ItemSearch>();*/
                            /*title = db.ItemSearch.Where(x => x.category_title.Contains(a) && allChildCategory.Contains(x.category_id)).ToList<ItemSearch>();
                            manufacture = db.ItemSearch.Where(x => x.manufacture_name.Contains(a) && allChildCategory.Contains(x.category_id)).ToList<ItemSearch>();
                            ItemName = db.ItemSearch.Where(x => x.item_name.Contains(a) && allChildCategory.Contains(x.category_id)).ToList<ItemSearch>();
                            productModel = db.ItemSearch.Where(x => x.product_model.Contains(a) && allChildCategory.Contains(x.category_id)).ToList<ItemSearch>();
                            itemDesc = db.ItemSearch.Where(x => x.item_sdesc.Contains(a) && allChildCategory.Contains(x.category_id)).ToList<ItemSearch>();
                            */
                            manufacture.AddRange(db.ItemSearch.Where(x => x.ManufactureName == a && allChildCategory.Contains(x.CategoryID)).OrderByDescending(x => x.ItemUpdateDate).Take(defaultTakeNumber).ToList<ItemSearch>());
                            if (!isContainsUnicodeCharacter(a))
                            {
                                productName.AddRange(db.ItemSearch.Where(x => x.ProductName.Contains(a) && allChildCategory.Contains(x.CategoryID)).OrderByDescending(x => x.ItemUpdateDate).Take(defaultTakeNumber).ToList<ItemSearch>());
                            }
                            else
                            {
                                itemName.AddRange(db.ItemSearch.Where(x => x.Name.Contains(a) && allChildCategory.Contains(x.CategoryID)).OrderByDescending(x => x.ItemUpdateDate).Take(defaultTakeNumber).ToList<ItemSearch>());
                            }
                            title.AddRange(db.ItemSearch.Where(x => x.CategoryTitle == a && allChildCategory.Contains(x.CategoryID)).OrderByDescending(x => x.ItemUpdateDate).Take(defaultTakeNumber).ToList<ItemSearch>());
                            //productModel.AddRange(db.ItemSearch.Where(x => x.product_model.Contains(a) && allChildCategory.Contains(x.category_id)).ToList<ItemSearch>());
                            //itemDesc.AddRange(db.ItemSearch.Where(x => x.item_sdesc.Contains(a) && allChildCategory.Contains(x.category_id)).ToList<ItemSearch>());
                        }
                        else
                        {
                            //AllIs = db.ItemSearch.Where(x => x.category_title.Contains(a) || x.manufacture_name.Contains(a) || x.item_name.Contains(a) || x.product_model.Contains(a) || x.item_sdesc.Contains(a)).ToList<ItemSearch>();
                            /*title = db.ItemSearch.Where(x => x.category_title.Contains(a)).ToList<ItemSearch>();
                            manufacture = db.ItemSearch.Where(x => x.manufacture_name.Contains(a)).ToList<ItemSearch>();
                            ItemName = db.ItemSearch.Where(x => x.item_name.Contains(a)).ToList<ItemSearch>();
                            productModel = db.ItemSearch.Where(x => x.product_model.Contains(a)).ToList<ItemSearch>();
                            itemDesc = db.ItemSearch.Where(x => x.item_sdesc.Contains(a)).ToList<ItemSearch>();*/
                            manufacture.AddRange(db.ItemSearch.Where(x => x.ManufactureName == a).OrderByDescending(x => x.ItemUpdateDate).Take(defaultTakeNumber).ToList<ItemSearch>());
                            if (!isContainsUnicodeCharacter(a))
                            {
                                productName.AddRange(db.ItemSearch.Where(x => x.ProductName.Contains(a)).OrderByDescending(x => x.ItemUpdateDate).Take(defaultTakeNumber).ToList<ItemSearch>());
                            }
                            else
                            {
                                itemName.AddRange(db.ItemSearch.Where(x => x.Name.Contains(a)).OrderByDescending(x => x.ItemUpdateDate).Take(defaultTakeNumber).ToList<ItemSearch>());
                            }
                            title.AddRange(db.ItemSearch.Where(x => x.CategoryTitle == a).OrderByDescending(x => x.ItemUpdateDate).Take(defaultTakeNumber).ToList<ItemSearch>());
                            //productModel.AddRange(db.ItemSearch.Where(x => x.product_model.Contains(a)).ToList<ItemSearch>());
                            //itemDesc.AddRange(db.ItemSearch.Where(x => x.item_sdesc.Contains(a)).ToList<ItemSearch>());
                        }
                    }
                    else if (minPrice != null & maxPrice != null)
                    {
                        if (allChildCategory.Count > 0)
                        {
                            /*AllIs = db.ItemSearch.Where( x => (x.category_title.Contains(a) || x.manufacture_name.Contains(a) || x.item_name.Contains(a) || x.product_model.Contains(a) || x.item_sdesc.Contains(a))
                                                          && x.item_pricecash >= minPrice.Value && x.item_pricecash <= maxPrice.Value && allChildCategory.Contains(x.category_id)   ).ToList<ItemSearch>();*/
                            /*title = db.ItemSearch.Where(x => x.category_title.Contains(a) && x.item_pricecash >= minPrice.Value && x.item_pricecash <= maxPrice.Value && allChildCategory.Contains(x.category_id)).ToList<ItemSearch>();
                            manufacture = db.ItemSearch.Where(x => x.manufacture_name.Contains(a) && x.item_pricecash >= minPrice.Value && x.item_pricecash <= maxPrice.Value && allChildCategory.Contains(x.category_id)).ToList<ItemSearch>();
                            ItemName = db.ItemSearch.Where(x => x.item_name.Contains(a) && x.item_pricecash >= minPrice.Value && x.item_pricecash <= maxPrice.Value && allChildCategory.Contains(x.category_id)).ToList<ItemSearch>();
                            productModel = db.ItemSearch.Where(x => x.product_model.Contains(a) && x.item_pricecash >= minPrice.Value && x.item_pricecash <= maxPrice.Value && allChildCategory.Contains(x.category_id)).ToList<ItemSearch>();
                            itemDesc = db.ItemSearch.Where(x => x.item_sdesc.Contains(a) && x.item_pricecash >= minPrice.Value && x.item_pricecash <= maxPrice.Value && allChildCategory.Contains(x.category_id)).ToList<ItemSearch>();*/
                            manufacture.AddRange(db.ItemSearch.Where(x => x.ManufactureName == a && x.Pricecash >= minPrice.Value && x.Pricecash <= maxPrice.Value && allChildCategory.Contains(x.CategoryID)).OrderByDescending(x => x.ItemUpdateDate).Take(defaultTakeNumber).ToList<ItemSearch>());
                            if (!isContainsUnicodeCharacter(a))
                            {
                                productName.AddRange(db.ItemSearch.Where(x => x.ProductName.Contains(a) && x.Pricecash >= minPrice.Value && x.Pricecash <= maxPrice.Value && allChildCategory.Contains(x.CategoryID)).OrderByDescending(x => x.ItemUpdateDate).Take(defaultTakeNumber).ToList<ItemSearch>());
                            }
                            else
                            {
                                itemName.AddRange(db.ItemSearch.Where(x => x.Name.Contains(a) && x.Pricecash >= minPrice.Value && x.Pricecash <= maxPrice.Value && allChildCategory.Contains(x.CategoryID)).OrderByDescending(x => x.ItemUpdateDate).Take(defaultTakeNumber).ToList<ItemSearch>());
                            }
                            title.AddRange(db.ItemSearch.Where(x => x.CategoryTitle == a && x.Pricecash >= minPrice.Value && x.Pricecash <= maxPrice.Value && allChildCategory.Contains(x.CategoryID)).OrderByDescending(x => x.ItemUpdateDate).Take(defaultTakeNumber).ToList<ItemSearch>());
                            //productModel.AddRange(db.ItemSearch.Where(x => x.product_model.Contains(a) && x.item_pricecash >= minPrice.Value && x.item_pricecash <= maxPrice.Value && allChildCategory.Contains(x.category_id)).ToList<ItemSearch>());
                            //itemDesc.AddRange(db.ItemSearch.Where(x => x.item_sdesc.Contains(a) && x.item_pricecash >= minPrice.Value && x.item_pricecash <= maxPrice.Value && allChildCategory.Contains(x.category_id)).ToList<ItemSearch>());
                        }
                        else
                        {
                            /*AllIs = db.ItemSearch.Where(x => (x.category_title.Contains(a) || x.manufacture_name.Contains(a) || x.item_name.Contains(a) || x.product_model.Contains(a) || x.item_sdesc.Contains(a))
                                                          && x.item_pricecash >= minPrice.Value && x.item_pricecash <= maxPrice.Value  ).ToList<ItemSearch>();*/
                            /*title = db.ItemSearch.Where(x => x.category_title.Contains(a) && x.item_pricecash >= minPrice.Value && x.item_pricecash <= maxPrice.Value).ToList<ItemSearch>();
                            manufacture = db.ItemSearch.Where(x => x.manufacture_name.Contains(a) && x.item_pricecash >= minPrice.Value && x.item_pricecash <= maxPrice.Value).ToList<ItemSearch>();
                            ItemName = db.ItemSearch.Where(x => x.item_name.Contains(a) && x.item_pricecash >= minPrice.Value && x.item_pricecash <= maxPrice.Value).ToList<ItemSearch>();
                            productModel = db.ItemSearch.Where(x => x.product_model.Contains(a) && x.item_pricecash >= minPrice.Value && x.item_pricecash <= maxPrice.Value).ToList<ItemSearch>();
                            itemDesc = db.ItemSearch.Where(x => x.item_sdesc.Contains(a) && x.item_pricecash >= minPrice.Value && x.item_pricecash <= maxPrice.Value).ToList<ItemSearch>();*/
                            manufacture.AddRange(db.ItemSearch.Where(x => x.ManufactureName == a && x.Pricecash >= minPrice.Value && x.Pricecash <= maxPrice.Value).OrderByDescending(x => x.ItemUpdateDate).Take(defaultTakeNumber).ToList<ItemSearch>());
                            if (!isContainsUnicodeCharacter(a))
                            {
                                productName.AddRange(db.ItemSearch.Where(x => x.ProductName.Contains(a) && x.Pricecash >= minPrice.Value && x.Pricecash <= maxPrice.Value).OrderByDescending(x => x.ItemUpdateDate).Take(defaultTakeNumber).ToList<ItemSearch>());
                            }
                            else
                            {
                                itemName.AddRange(db.ItemSearch.Where(x => x.Name.Contains(a) && x.Pricecash >= minPrice.Value && x.Pricecash <= maxPrice.Value).OrderByDescending(x => x.ItemUpdateDate).Take(defaultTakeNumber).ToList<ItemSearch>());
                            }
                            title.AddRange(db.ItemSearch.Where(x => x.CategoryTitle == a && x.Pricecash >= minPrice.Value && x.Pricecash <= maxPrice.Value).OrderByDescending(x => x.ItemUpdateDate).Take(defaultTakeNumber).ToList<ItemSearch>());
                            //productModel.AddRange(db.ItemSearch.Where(x => x.product_model.Contains(a) && x.item_pricecash >= minPrice.Value && x.item_pricecash <= maxPrice.Value).ToList<ItemSearch>());
                            //itemDesc.AddRange(db.ItemSearch.Where(x => x.item_sdesc.Contains(a) && x.item_pricecash >= minPrice.Value && x.item_pricecash <= maxPrice.Value).ToList<ItemSearch>());
                        }
                    }
                    //titleAccu.AddRange(title.Where(x => SearchKey(x.CategoryTitle, a) == true).ToList());
                    //manufactureAccu.AddRange(manufacture.Where(x => SearchKey(x.ManufactureName, a) == true).ToList());
                    //ItemNameAccu.AddRange(ItemName.Where(x => SearchKey(x.Name, a) == true).ToList());
                    //productNameAccu.AddRange(productName.Where(x => SearchKey(x.ProductName, a) == true).ToList());
                    if (KeyWordandCategory.ContainsKey(a.ToLower()))
                    {
                        isInKeyWordCatrgory.AddRange(KeyWordandCategory[a.ToLower()]);
                    }
                    keywordCount++;
                }
#if DEBUG
                sw.Stop();
                swtl += sw.ElapsedMilliseconds;
                sw.Reset();
                sw = Stopwatch.StartNew();
#endif
                //sellerProductID.AddRange(db.ItemSearch.Where(x => x.SellerProductID.Contains(totalKeyWord)));
                sellerProductID.AddRange(db.ItemSearch.Where(x => x.SellerProductID == totalKeyWord));
                db.Dispose();
                allIsTemp.AddRange(manufacture);
                allIsTemp.AddRange(itemName);
                allIsTemp.AddRange(productName);
                allIsTemp.AddRange(title);
                if (KeyWordandCategory.ContainsKey(totalKeyWord.ToLower()))
                {
                    isInKeyWordCatrgory.AddRange(KeyWordandCategory[totalKeyWord.ToLower()]);
                }
                //if (keyword.Count() > 1 && preciseKeyword.Count() > 1)
                //{
                //    foreach (var aKeyword in keyword)
                //    {
                //        manufactureAccu.AddRange(manufacture.Where(x => SearchKey(x.ManufactureName, aKeyword, true) == true).OrderBy(x => x.DateEnd).ToList());
                //    }
                //}
#if DEBUG
                sw.Stop();
                swtl += sw.ElapsedMilliseconds;
                sw.Reset();
                sw = Stopwatch.StartNew();
#endif
                manufactureAccu.AddRange(manufacture.Where(x => SearchWholeKey(x.ManufactureName, totalKeyWord) == true).OrderBy(x => x.DateEnd).ToList());
                itemNameAccu.AddRange(itemName.Where(x => SearchWholeKey(x.Name, totalKeyWord) == true).OrderBy(x => x.DateEnd).ToList());
                productNameAccu.AddRange(productName.Where(x => SearchWholeKey(x.ProductName, totalKeyWord) == true).OrderBy(x => x.DateEnd).ToList());
                titleAccu.AddRange(title.Where(x => SearchWholeKey(x.CategoryTitle, totalKeyWord) == true).OrderBy(x => x.DateEnd).ToList());

#if DEBUG
                sw.Stop();
                swtl += sw.ElapsedMilliseconds;
                sw.Reset();
                sw = Stopwatch.StartNew();
#endif
                var MIPTName = manufacture.Select(x => new { ID = x.ID, NAME = x.CategoryTitle + " " + x.Name + " " + x.ProductName + " " + x.ManufactureName }).ToList();
                MIPTName.AddRange(itemName.Select(x => new { ID = x.ID, NAME = x.CategoryTitle + " " + x.Name + " " + x.ProductName + " " + x.ManufactureName }).ToList());
                MIPTName.AddRange(productName.Select(x => new { ID = x.ID, NAME = x.CategoryTitle + " " + x.Name + " " + x.ProductName + " " + x.ManufactureName }).ToList());
                MIPTName.AddRange(title.Select(x => new { ID = x.ID, NAME = x.CategoryTitle + " " + x.Name + " " + x.ProductName + " " + x.ManufactureName }).ToList());

#if DEBUG
                sw.Stop();
                swtl += sw.ElapsedMilliseconds;
                sw.Reset();
                sw = Stopwatch.StartNew();
#endif
                //var MIPTNameAccu = MIPTName.Where(x => keyword.All(y => x.NAME.ToLower().Contains(y))).Distinct().ToList(); //search all serach keywords with AND condition.
                //var MIPTNameAccu = MIPTName.Where(x => keyword.Any(y => x.NAME.ToLower().Contains(y))).Distinct().ToList();//search all serach keywords with OR condition.

                var MIPTNameAccu = MIPTName.Where(x => keyword.All(y => x.NAME.ToLower().Contains(y))).Distinct().ToList(); //search all serach keywords with AND condition.
                var accuID = new HashSet<int>(MIPTNameAccu.Select(y => y.ID).ToList());
                MIPTName.RemoveAll(x => accuID.Contains(x.ID));
                MIPTNameAccu.AddRange(MIPTName.Where(x => keyword.Any(y => x.NAME.ToLower().Contains(y))).Distinct().ToList()); //search all serach keywords with OR condition.

#if DEBUG
                sw.Stop();
                swtl += sw.ElapsedMilliseconds;
                sw.Reset();
                sw = Stopwatch.StartNew();
#endif
                //allInDictionary.AddRange(allIsTemp.Where(x => isInKeyWordCatrgory.Contains(x.CategoryID)).ToList());

                //List<int> manufactureSrchIn = new List<int>();
                //List<int> itemNameSrchIn = new List<int>();
                //List<int> productNameSrchIn = new List<int>();
                //List<int> titleSrchIn = new List<int>();
                //if (srchInKeyword.Count > 0 && srchInPreciseKeyword.Count > 0)
                if (srchInKeyword.Count > 0)
                {

                    //manufactureAccu = manufactureAccu.Where(x => srchInKeyword.All(y => (x.ManufactureName + " " + x.Name + " " + x.ProductName + " " + x.CategoryTitle).ToLower().Contains(y))).ToList();
                    //itemNameAccu = itemNameAccu.Where(x => srchInKeyword.All(y => (x.ManufactureName + " " + x.Name + " " + x.ProductName + " " + x.CategoryTitle).ToLower().Contains(y))).ToList();
                    //productNameAccu = productNameAccu.Where(x => srchInKeyword.All(y => (x.ManufactureName + " " + x.Name + " " + x.ProductName + " " + x.CategoryTitle).ToLower().Contains(y))).ToList();
                    //titleAccu = titleAccu.Where(x => srchInKeyword.All(y => (x.ManufactureName + " " + x.Name + " " + x.ProductName + " " + x.CategoryTitle).ToLower().Contains(y))).ToList();
                    //MIPTNameAccu = MIPTNameAccu.Where(x => srchInKeyword.All(y => x.NAME.ToLower().Contains(y))).ToList();
                    //manufacture.Clear();
                    //itemName.Clear();
                    //productName.Clear();
                    //title.Clear();

                    var manufactureAccuSrchInT = manufactureAccu.Where(x => SearchKey(x.ManufactureName, totalSrchIn, true) == true).OrderBy(x => x.DateEnd).ToList();
                    var manufactureAccuSrchInAll = manufactureAccu.Where(x => srchInKeyword.All(y => (x.ManufactureName + " " + x.Name + " " + x.ProductName + " " + x.CategoryTitle).ToLower().Contains(y))).ToList();
                    manufactureAccu.Clear();
                    manufactureAccu.AddRange(manufactureAccuSrchInT);
                    manufactureAccu.AddRange(manufactureAccuSrchInAll);
                    var itemNameAccuSrchInT = itemNameAccu.Where(x => SearchKey(x.Name, totalSrchIn, true) == true).OrderBy(x => x.DateEnd).ToList();
                    var itemNameAccuSrchInAll = itemNameAccu.Where(x => srchInKeyword.All(y => (x.ManufactureName + " " + x.Name + " " + x.ProductName + " " + x.CategoryTitle).ToLower().Contains(y))).ToList();
                    itemNameAccu.Clear();
                    itemNameAccu.AddRange(itemNameAccuSrchInT);
                    itemNameAccu.AddRange(itemNameAccuSrchInAll);
                    var productNameAccuSrchInT = productNameAccu.Where(x => SearchKey(x.ProductName, totalSrchIn, true) == true).OrderBy(x => x.DateEnd).ToList();
                    var productNameAccuSrchInAll = productNameAccu.Where(x => srchInKeyword.All(y => (x.ManufactureName + " " + x.Name + " " + x.ProductName + " " + x.CategoryTitle).ToLower().Contains(y))).ToList();
                    productNameAccu.Clear();
                    productNameAccu.AddRange(productNameAccuSrchInT);
                    productNameAccu.AddRange(productNameAccuSrchInAll);
                    var titleAccuSrchInT = titleAccu.Where(x => SearchKey(x.CategoryTitle, totalSrchIn, true) == true).OrderBy(x => x.DateEnd).ToList();
                    var titleAccuSrchInAll = titleAccu.Where(x => srchInKeyword.All(y => (x.ManufactureName + " " + x.Name + " " + x.ProductName + " " + x.CategoryTitle).ToLower().Contains(y))).ToList();
                    titleAccu.Clear();
                    titleAccu.AddRange(titleAccuSrchInT);
                    titleAccu.AddRange(titleAccuSrchInAll);
                    var MIPTNameAccuSrchInT = MIPTNameAccu.Where(x => SearchKey(x.NAME, totalSrchIn, true) == true).ToList();
                    var MIPTNameAccuSrchInAll = MIPTNameAccu.Where(x => srchInKeyword.All(y => x.NAME.ToLower().Contains(y))).ToList();
                    //MIPTNameAccu.Clear();
                    //MIPTNameAccu.AddRange(MIPTNameAccuSrchInT);
                    //MIPTNameAccu.AddRange(MIPTNameAccuSrchInAll);
                    var MIPTNameAccuSrchInTID = new HashSet<int>(MIPTNameAccuSrchInT.Select(y => y.ID).ToList());
                    allIsTotalAccu = allIsTemp.Where(x => MIPTNameAccuSrchInTID.Contains(x.ID)).ToList();
                    var MIPTNameAccuSrchInAllID = new HashSet<int>(MIPTNameAccuSrchInAll.Select(y => y.ID).ToList());
                    allIsAccu = allIsTemp.Where(x => MIPTNameAccuSrchInAllID.Contains(x.ID)).ToList();
                    manufacture.Clear();
                    itemName.Clear();
                    productName.Clear();
                    title.Clear();
                    MIPTNameAccu.Clear();
                }
#if DEBUG
                sw.Stop();
                swtl += sw.ElapsedMilliseconds;
                sw.Reset();
                sw = Stopwatch.StartNew();
#endif
                var divide = 20;
                if (MIPTNameAccu.Count > 10000)
                {
                    divide = 128;
                }
                else if (MIPTNameAccu.Count > 5000)
                {
                    divide = 128;
                }
                else if (MIPTNameAccu.Count > 1000)
                {
                    divide = 64;
                }
                else if (MIPTNameAccu.Count > 500)
                {
                    divide = 32;
                }
                else if (MIPTNameAccu.Count > 100)
                {
                    divide = 8;
                }
                else
                {
                    divide = 4;
                }

                for (int i = 0; i <= (MIPTNameAccu.Count / divide); i++)
                {
                    var hashID = new HashSet<int>(MIPTNameAccu.Select(y => y.ID).Skip(i * divide).Take(divide));
                    allIsTempOrder.AddRange(allIsTemp.Where(x => hashID.Contains(x.ID)));
                }
#if DEBUG
                sw.Stop();
                swtl += sw.ElapsedMilliseconds;
                sw.Reset();
                sw = Stopwatch.StartNew();
#endif
                //var MIPTAccuID = MIPTNameAccu.Select(y => y.ID).ToList();
                //allIsTemp = allIsTemp.Where(x => MIPTAccuID.Contains(x.ID)).ToList();
                allInDictionary.AddRange(allIsTempOrder.Where(x => isInKeyWordCatrgory.Contains(x.CategoryID)).ToList());
                allIs.AddRange(allInDictionary);
                allIs.AddRange(manufactureAccu);
                allIs.AddRange(itemNameAccu);
                allIs.AddRange(productNameAccu);
                allIs.AddRange(titleAccu);
                allIs.AddRange(allIsTotalAccu);
                allIs.AddRange(allIsAccu);
                allIs.AddRange(sellerProductID);
                allIs.AddRange(allIsTempOrder);
                allIs = allIs.Distinct().ToList();
                SI.ISnumber = allIs.Count;
#if DEBUG
                sw.Stop();
                swtl += sw.ElapsedMilliseconds;
                sw.Reset();
                sw = Stopwatch.StartNew();
#endif
            }
            //2.排序
            switch (Order)
            {
                case (int)OrderCondition.PriceLow2High:
                    //價格由低到高
                    allIs = allIs.OrderBy(x => x.Pricecash).ToList<ItemSearch>();
                    break;
                case (int)OrderCondition.PriceHigh2Low:
                    //價格由高到低
                    allIs = allIs.OrderByDescending(x => x.Pricecash).ToList<ItemSearch>();
                    break;
                case (int)OrderCondition.MostRelate:
                    //最相關(新增至AllIs時，就已經是做最相關排序，所以這邊不做任何動作_Bill) //搜尋預設值是最相關
                    /*foreach (string b in keyword)
                    {
                        title = AllIs.Where(x => x.manufacture_name != null && x.category_title.IndexOf(b, StringComparison.OrdinalIgnoreCase) > -1).Count();
                        manufacture = AllIs.Where(x => x.manufacture_name != null && x.manufacture_name.IndexOf(b, StringComparison.OrdinalIgnoreCase) > -1).Count();
                        ItemName = AllIs.Where(x => x.item_name != null && x.item_name.IndexOf(b, StringComparison.OrdinalIgnoreCase) > -1).Count();
                        productModel = AllIs.Where(x => x.product_model != null && x.product_model.IndexOf(b, StringComparison.OrdinalIgnoreCase) > -1).Count();
                        itemDesc = AllIs.Where(x => x.item_sdesc != null && x.item_sdesc.IndexOf(b, StringComparison.OrdinalIgnoreCase) > -1).Count();
                    }*/

                    /*
                    if (title.Count >= manufacture.Count)
                    {
                        if (manufacture.Count >= ItemName.Count)
                        {
                            AllIs = AllIs.OrderByDescending(x => x.item_name.Contains(SearchWord) || x.manufacture_name.Contains(SearchWord) || x.category_title.Contains(SearchWord)).ThenBy(x => x.manufacture_name.Contains(keyword[0])).ThenBy(x => x.category_title.Contains(keyword[0])).ToList<ItemSearch>();
                            
                        }
                        else if (ItemName.Count >= title.Count)
                        {
                            AllIs = AllIs.OrderByDescending(x => x.item_name.Contains(SearchWord) || x.manufacture_name.Contains(SearchWord) || x.category_title.Contains(SearchWord)).ThenBy(x => x.item_name.Contains(keyword[0])).ThenBy(x => x.category_title.Contains(keyword[0])).ToList<ItemSearch>();
                        }
                        else
                        {
                            AllIs = AllIs.OrderByDescending(x => x.item_name.Contains(SearchWord) || x.manufacture_name.Contains(SearchWord) || x.category_title.Contains(SearchWord)).ThenBy(x => x.category_title.Contains(keyword[0])).ThenBy(x => x.item_name.Contains(keyword[0])).ToList<ItemSearch>();
                        }
                    }
                    else
                    {
                        if (title.Count >= ItemName.Count)
                        {
                            AllIs = AllIs.OrderByDescending(x => x.item_name.Contains(SearchWord) || x.manufacture_name.Contains(SearchWord) || x.category_title.Contains(SearchWord)).ThenBy(x => x.manufacture_name.Contains(keyword[0])).ThenBy(x => x.category_title.Contains(keyword[0])).ToList<ItemSearch>();
                        }
                        else if (ItemName.Count >= manufacture.Count)
                        {
                            AllIs = AllIs.OrderByDescending(x => x.item_name.Contains(SearchWord) || x.manufacture_name.Contains(SearchWord) || x.category_title.Contains(SearchWord)).ThenBy(x => x.item_name.Contains(keyword[0])).ThenBy(x => x.manufacture_name.Contains(keyword[0])).ToList<ItemSearch>();
                        }
                        else
                        {
                            AllIs = AllIs.OrderByDescending(x => x.item_name.Contains(SearchWord) || x.manufacture_name.Contains(SearchWord) || x.category_title.Contains(SearchWord)).ThenBy(x => x.manufacture_name.Contains(keyword[0])).ThenBy(x => x.item_name.Contains(keyword[0])).ToList<ItemSearch>();
                        }
                    }
                    */
                    //Is.ItemSearch = Is.ItemSearch.OrderBy(x => x.).ToList<ItemSearch>();
                    break;
                case (int)OrderCondition.MostPolular:
                    //最有人氣，參考方式(銷售量?)
                    allIs = allIs.OrderByDescending(x => x.Qtyreg).ToList<ItemSearch>();
                    break;
                case (int)OrderCondition.StockHigh2Low:
                    //庫存量從多到少，參考方式(銷售量?)
                    allIs = allIs.OrderByDescending(x => x.SellingQty).ToList<ItemSearch>();
                    break;
                case (int)OrderCondition.CreateNew2Old:
                    //新到舊
                    allIs = allIs.OrderByDescending(x => x.ItemUpdateDate).ToList<ItemSearch>();
                    break;
                case (int)OrderCondition.CreateOld2New:
                    //舊到新
                    allIs = allIs.OrderBy(x => x.ItemUpdateDate).ToList<ItemSearch>();
                    break;
                default:
                    break;

            }
#if DEBUG
            sw.Stop();
            swtl += sw.ElapsedMilliseconds;
            sw.Reset();
            sw = Stopwatch.StartNew();
#endif
            //所有商品回傳
            KeyWordandCategory.Clear();
            KeyWordandItemID.Clear();
            KeyWordOnlyItemID.Clear();
            
            return allIs;
            /*
            allSearchResults.ItemSearch = AllIs;
            if (AllIs.Count < (PageSize * (int)Page))
            {
                pagenumber = AllIs.Count;
            }

            for (int i = PageSize * ((int)Page - 1); i < pagenumber; i++)
            {
                Count.Add(AllIs[i]);
            }

            Is.ItemSearch = Count;

            ICarts TestItemSellorNot = new CartsRepository();
            List<int> SellOrNot = new List<int>();

            foreach (ItemSearch s in Is.ItemSearch)
            {
                int sellingQTY = TestItemSellorNot.GetSellingQty((int)s.ID, "Item");
                ViewBag.Qty = Math.Min(20, sellingQTY);
                if (sellingQTY > 0 && !TestItemSellorNot.CheckDateTimeOut((int)s.ID, "Item"))
                {
                    SellOrNot.Add(1);
                }
                else
                {
                    SellOrNot.Add(0);
                    s.ItemSpechead = "此商品暫無販售";
                }

            }

            Is.SellOrNot = SellOrNot;
            return Is;
            */
        }



        //private DatabaseContext db = new DatabaseContext();
        /// <summary>
        /// Set price range, and make sure the range in all results price's range
        /// </summary>
        /// <param name="maxPrice"></param> Input Maximun price to this function
        /// <param name="Is"></param> Send all SearchItems to this function
        /// <returns></returns> It returns a dictionary, string for price range, and list<int> [0] is count number, [1] low bound price, [2] high bound price
        // maxPrice double 改 decimal
        public Dictionary<string, List<int>> CountNumberPrice(decimal maxPrice, List<ItemSearch> AllResults)
        {
            Dictionary<string, List<int>> countNumberPrice = new Dictionary<string, List<int>>();
            for (int i = 0; i < maxPrice; )
            {
                List<int> itemData = new List<int>();
                string priceCategory = "";
                int initPrice = i;
                if (i == 0)
                {
                    i += 500;
                }
                else
                {
                    i *= 2;
                }

                priceCategory = "$" + initPrice.ToString() + " ~ $" + i.ToString();
                int itemNumber = AllResults.Where(x => x.Pricecash >= initPrice && x.Pricecash <= i).Count();
                itemData.Add(itemNumber);
                itemData.Add(initPrice);
                itemData.Add(i);
                countNumberPrice.Add(priceCategory, itemData);

            }

            return countNumberPrice;
        }

        public Dictionary<string, List<int>> CountNumberPrice(decimal maxPrice, List<ItemPreview> AllResults)
        {
            Dictionary<string, List<int>> countNumberPrice = new Dictionary<string, List<int>>();
            for (int i = 0; i < maxPrice; )
            {
                List<int> itemData = new List<int>();
                string priceCategory = "";
                int initPrice = i;
                if (i == 0)
                {
                    i += 500;
                }
                else
                {
                    i *= 2;
                }

                priceCategory = "$" + initPrice.ToString() + " ~ $" + i.ToString();
                int itemNumber = AllResults.Where(x => x.PriceCash >= initPrice && x.PriceCash <= i).Count();
                itemData.Add(itemNumber);
                itemData.Add(initPrice);
                itemData.Add(i);
                countNumberPrice.Add(priceCategory, itemData);

            }

            return countNumberPrice;
        }

        /// <summary>
        /// nothing XD 
        /// </summary>
        /// <param name="categoryID"></param> Send category ID to this function
        /// <param name="categoeyLayer"></param> Send category layer to this function
        /// <param name="countryID"></param> Send country ID to this function, if need
        /// <param name="brandID"></param> Send brand ID to this function, if need
        /// <param name="sellerID"></param> Send seller ID to this function, if need
        /// <returns></returns> it'll return all category ID that under the category.
        public List<int> GetChildCategory(int categoryID, int categoeyLayer, int? countryID, int? brandID, int sellerID)
        {
            List<int> ChildCategory = new List<int>();


            return ChildCategory;
        }
        /// <summary>
        /// Get all category with these layer's items from category xml
        /// </summary>
        /// <param name="AllResults"></param>
        /// <param name="Cat"></param>
        /// <param name="LID"></param>
        /// <param name="Cty"></param>
        /// <param name="BID"></param>
        /// <param name="SID"></param>
        /// <returns></returns>
        public List<SearchCategory> GetCategoryNumber(List<ItemSearch> AllResults, int? Cat, int? LID, int? Cty, int? BID, int? SID)
        {
            List<SearchCategory> categoryNumber = new List<SearchCategory>();

            var allCategoryID = AllResults.Select(x => new { category_id = x.CategoryID, category_layer = x.CategoryLayer }).GroupBy(x => new { x.category_id, x.category_layer }).ToList();

            foreach (var acategory in allCategoryID)
            {
                List<SearchCategory> parentCategory = new List<SearchCategory>();
                parentCategory = GetParentCategory(acategory.Key.category_id, acategory.Key.category_layer, Cty, BID, SID); //Get parents category
                categoryNumber.Add(new SearchCategory { categoryID = acategory.Key.category_id.ToString(), layer = acategory.Key.category_layer.ToString(), number = acategory.Count(), categoryName = parentCategory[0].categoryName, categoryDescr = parentCategory[0].categoryDescr, categoryShowOrder = parentCategory[0].categoryShowOrder, parentCategory = parentCategory });// set all category in this veriable
            }

            return categoryNumber;
        }
        /// <summary>
        /// get parent category from this layer's category
        /// </summary>
        /// <param name="AllResults"></param>
        /// <param name="LID"></param>
        /// <returns></returns>
        public Dictionary<string, SearchCategory> GetHotCategoryNumber(List<SearchCategory> AllResults, int? LID)
        {
            List<SearchCategory> hotCategoryNumber = new List<SearchCategory>();
            Dictionary<string, SearchCategory> hotCategory = new Dictionary<string, SearchCategory>();
            Dictionary<string, SearchCategory> hotSubCategory = new Dictionary<string, SearchCategory>();
            int layer = new int();

            if (LID != null) //make sure layer id has value
            {
                if (LID.Value != 0 && LID.Value > 0)
                {
                    layer = LID.Value; //if had value and bigger than 0 then using this LID value
                }
                else if (LID.Value == 0)
                {
                    layer = 0; //if equal 0 then set to 0
                }
            }
            else
            {
                layer = -1; //if LID equal null thn set it to -1 
            }


            //find all results category's parents category
            foreach (var results in AllResults)
            {
                int categoryNum = results.parentCategory.Count;
                SearchCategory parentCategory = new SearchCategory();
                int layerCategory = new int();
                if (layer > 0)
                {
                    layerCategory = categoryNum - layer - 2;
                }
                else if (layer == 0)
                {
                    layerCategory = categoryNum - 2;
                }
                else if (layer < 0)
                {
                    layerCategory = categoryNum - 1;
                }
                if (layerCategory < 0)
                {
                    parentCategory = null;
                }
                else
                {
                    if (layerCategory == 0)
                    {
                        parentCategory = results;
                    }
                    else
                    {
                        parentCategory = results.parentCategory[layerCategory];
                    }
                }

                if (parentCategory != null)
                {
                    if (!hotCategory.ContainsKey(parentCategory.categoryID.ToString()))//check if the category not in the hotCategory, if yes then add category in it, if no then only add results number in it
                    {
                        hotCategory.Add(parentCategory.categoryID.ToString(), parentCategory); //add parent category into hotCategory
                        hotCategory[parentCategory.categoryID.ToString()].number = results.number; //count how many search item results in this category
                    }
                    else
                    {
                        hotCategory[parentCategory.categoryID.ToString()].number += results.number; //count how many search item results in this category
                    }

                }




                if (layer > 0)
                {
                    layerCategory = categoryNum - layer - 3;
                }
                else if (layer == 0)
                {
                    layerCategory = categoryNum - 3;
                }
                else if (layer < 0)
                {
                    layerCategory = categoryNum - 2;
                }
                if (layerCategory < 0)
                {
                    parentCategory = null;
                }
                else
                {
                    if (layerCategory == 0)
                    {
                        parentCategory = results;
                    }
                    else
                    {
                        parentCategory = results.parentCategory[layerCategory];
                    }
                }

                if (parentCategory != null)
                {
                    if (!hotSubCategory.ContainsKey(parentCategory.categoryID.ToString())) //check if the category not in the hotSubCategory, if yes then add category in it, if no then only add results number in it
                    {
                        hotSubCategory.Add(parentCategory.categoryID.ToString(), parentCategory); //add category into child category
                        hotSubCategory[parentCategory.categoryID.ToString()].number = results.number;  //count how many search item results in this category
                        hotSubCategory[parentCategory.categoryID.ToString()].parentCategory.AddRange(CreateNewCategory(results.parentCategory));//add parent category into child category
                    }
                    else
                    {
                        hotSubCategory[parentCategory.categoryID.ToString()].number += results.number;  //count how many search item results in this category
                    }

                }




            }

            ArrangeCategory(hotCategory, hotSubCategory, layer); //combine hotCategory and hotSubCateogry

            return hotCategory;
        }

        private List<SearchCategory> CreateNewCategory(List<SearchCategory> oriCategory)
        {
            List<SearchCategory> newCategory = new List<SearchCategory>();
            for (int i = 0; i < oriCategory.Count; i++)
            {
                SearchCategory singleCategory = CreateNewSingleCategory(oriCategory[i]);
                newCategory.Add(singleCategory);
            }
            return newCategory;
        }

        private SearchCategory CreateNewSingleCategory(SearchCategory searchCategory)
        {
            SearchCategory newSearchCategory = new SearchCategory();
            newSearchCategory.categoryDescr = searchCategory.categoryDescr;
            newSearchCategory.categoryID = searchCategory.categoryID;
            newSearchCategory.categoryName = searchCategory.categoryName;
            newSearchCategory.categoryShowOrder = searchCategory.categoryShowOrder;
            newSearchCategory.layer = searchCategory.layer;
            newSearchCategory.number = searchCategory.number;
            newSearchCategory.parentCategory = CreateNewCategory(searchCategory.parentCategory);
            return newSearchCategory;
        }
        /// <summary>
        /// add child category into parents category, and sort both dictionary
        /// </summary>
        /// <param name="hotCategory"></param>
        /// <param name="hotSubCategory"></param>
        /// <param name="layer"></param>
        private void ArrangeCategory(Dictionary<string, SearchCategory> hotCategory, Dictionary<string, SearchCategory> hotSubCategory, int layer)
        {
            List<KeyValuePair<string, SearchCategory>> hotCategorySort = hotCategory.OrderBy(x => x.Value.categoryShowOrder).ThenByDescending(x => x.Value.number).ToList();
            hotCategory.Clear();
            foreach (var aHotCategorySort in hotCategorySort)
            {
                hotCategory.Add(aHotCategorySort.Key, aHotCategorySort.Value);
            }
            List<KeyValuePair<string, SearchCategory>> hotSubCategorySort = hotSubCategory.OrderBy(x => x.Value.categoryShowOrder).ThenByDescending(x => x.Value.number).ToList();
            hotSubCategory.Clear();
            foreach (var aHotSubCategorySort in hotSubCategorySort)
            {
                hotSubCategory.Add(aHotSubCategorySort.Key, aHotSubCategorySort.Value);
            }
            foreach (var subCategory in hotSubCategory)
            {
                int categoryNum = subCategory.Value.parentCategory.Count;
                int layerCategory = new int();
                if (layer > 0)
                {
                    layerCategory = categoryNum - layer - 2;
                }
                else if (layer == 0)
                {
                    layerCategory = categoryNum - 2;
                }
                else if (layer < 0)
                {
                    layerCategory = categoryNum - 1;
                }


                if (hotCategory.ContainsKey(subCategory.Value.parentCategory[layerCategory].categoryID.ToString())) //if child categpry's parents cateogry not null
                {
                    hotCategory[subCategory.Value.parentCategory[layerCategory].categoryID.ToString()].parentCategory.Add(CreateNewSingleCategory(subCategory.Value)); //add child category into parent category
                }


            }
        }

        /// <summary>
        /// Get item's all parents category from category xml
        /// </summary>
        /// <param name="Cat"></param>
        /// <param name="LID"></param>
        /// <param name="Cty"></param>
        /// <param name="BID"></param>
        /// <param name="SID"></param>
        /// <returns></returns>
        private List<SearchCategory> GetParentCategory(int? Cat, int? LID, int? Cty, int? BID, int? SID)
        {
            var categoryDetail = oCategoryController.GetParents(Cat, LID, Cty, BID, SID).ToList(); //Load parents category from category xml
            List<SearchCategory> parentCategory = new List<SearchCategory>();

            foreach (var acategoryDetail in categoryDetail)
            {
                parentCategory.Add(new SearchCategory { categoryID = acategoryDetail.category_id, layer = acategoryDetail.category_layer, categoryName = acategoryDetail.category_title, categoryDescr = acategoryDetail.category_description, categoryShowOrder = acategoryDetail.category_showorder, number = 0, parentCategory = new List<SearchCategory>() });
            }
            
            return parentCategory;
        }
        /// <summary>
        /// get category's child
        /// </summary>
        /// <param name="childCategory"></param>
        /// <param name="categoryID"></param>
        /// <param name="layer"></param>
        /// <param name="Cty"></param>
        /// <param name="BID"></param>
        /// <param name="SID"></param>
        /// <returns></returns>
        public bool GetChildCategory(List<int?> childCategory, int? categoryID, int? layer, int? Cty, int? BID, int? SID)
        {
            //CheckSSL sslCheck = new CheckSSL();
            //var isSecure = sslCheck.CheckUrlSSL();
            var isSecure = false;
            //var isSecure = HttpContext.Current.Request.IsSecureConnection;
            Dictionary<string, List<Category_TreeItem>> categoryTree = new Dictionary<string, List<Category_TreeItem>>();
            try
            {
                categoryTree = oCategoryController.GetCategory(categoryID, layer, Cty, BID, SID, isSecure);
            }
            catch (Exception e)
            {
                return false;
            }

            foreach (var treeDetail in categoryTree)
            {
                foreach (var aNode in treeDetail.Value)
                {
                    childCategory.Add(aNode.category_id);
                    if (aNode.Nodes.Count > 0)
                    {
                        foreach (var nodes in aNode.Nodes)
                        {
                            GetChildCategory(childCategory, nodes.category_id, nodes.category_layer, Cty, BID, SID);
                        }
                    }
                }
            }

            return true;
        }
        public List<int> GetBottomChildCategory(List<int> categoryIDs)
        {
            TWSqlDBContext db = new TWSqlDBContext();
            List<int> allCateogryIDs = new List<int>();
            allCateogryIDs.AddRange(oCategoryController.FindBottomLayerCategory(categoryIDs).Select(x => x.ID).ToList());
            db.Dispose();
            return allCateogryIDs.Distinct().ToList();
        }
        public List<string> DelEmpty(string[] searchWord)
        {
            List<string> keyWord = new List<string>();
            foreach (var aWord in searchWord)
            {
                if (aWord != "")
                {
                    keyWord.Add(aWord.ToLower());
                }
            }

            return keyWord;
        }
        public List<string> DelEmptyAndPrecise(string[] searchWord)
        {
            List<string> keyWord = new List<string>();
            foreach (var aWord in searchWord)
            {
                if (aWord != "")
                {
                    keyWord.Add(" " + aWord.ToLower() + " ");
                }
            }

            return keyWord;
        }
        public bool SearchKey(string oriString, string keyWord)
        {
            if (oriString == null || oriString == "")
            {
                return false;
            }
            int start, end;
            if (oriString.IndexOf(keyWord, StringComparison.OrdinalIgnoreCase) > -1)
            {
                start = oriString.IndexOf(keyWord, 0, StringComparison.OrdinalIgnoreCase) + keyWord.Length;
                if (keyWord.Length == oriString.Length)
                {
                    return true;
                }
                else if (start == oriString.Length && oriString[start - keyWord.Length - 1] == ' ')
                {
                    return true;
                }
                else if ((start - keyWord.Length) == 0 && oriString[start] == ' ')
                {
                    return true;
                }
                else if ((start - keyWord.Length) > 0 && start < oriString.Length && oriString[start - keyWord.Length - 1] == ' ' && oriString[start] == ' ')
                {
                    return true;
                }
                else
                {
                    return false;
                }

                //if (start != keyWord.Length)
                //{
                //    end = oriString.IndexOf(" ", start, 1,  StringComparison.OrdinalIgnoreCase);
                //    return true;
                //}
                //else
                //{
                //    return true;
                //}

            }
            else
            {
                return false;
            }
        }
        public bool SearchKey(string oriString, string keyWord, bool wholeWord)
        {
            if (oriString == null || oriString == "")
            {
                return false;
            }
            int start, end;
            if (oriString.IndexOf(keyWord, StringComparison.OrdinalIgnoreCase) > -1)
            {
                start = oriString.IndexOf(keyWord, 0, StringComparison.OrdinalIgnoreCase) + keyWord.Length;
                if (keyWord.Length == oriString.Length)
                {
                    return true;
                }
                else if (start == oriString.Length && (oriString[start - keyWord.Length - 1] == ' ' || oriString[start - keyWord.Length - 1] != ' '))
                {
                    return true;
                }
                else if ((start - keyWord.Length) == 0 && (oriString[start] == ' ' || oriString[start] != ' '))
                {
                    return true;
                }
                else if ((start - keyWord.Length) > 0 && start < oriString.Length && (oriString[start - keyWord.Length - 1] == ' ' || oriString[start - keyWord.Length - 1] != ' ') && (oriString[start] == ' ' || oriString[start] != ' '))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }
        public bool SearchWholeKey(string oriString, string keyWord)
        {
            if (oriString == null || oriString == "")
            {
                return false;
            }
            int start, end;
            if (oriString.IndexOf(keyWord, StringComparison.OrdinalIgnoreCase) > -1)
            {
                start = oriString.IndexOf(keyWord, 0, StringComparison.OrdinalIgnoreCase) + keyWord.Length;
                if (keyWord.Length == oriString.Length)
                {
                    return true;
                }
                else if (start == oriString.Length && oriString[start - keyWord.Length - 1] == ' ')
                {
                    return true;
                }
                else if ((start - keyWord.Length) == 0 && oriString[start] == ' ')
                {
                    return true;
                }
                else if ((start - keyWord.Length) > 0 && start < oriString.Length && oriString[start - keyWord.Length - 1] == ' ' && oriString[start] == ' ')
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }
        public bool isContainsUnicodeCharacter(string input)
        {
            const int MaxAnsiCode = 255;
            return input.Any(c => c > MaxAnsiCode);
        }
        public bool GenerateSearchHintsFromDB()
        {
            bool flag = false;
            //List<ItemSearch> allItemSearchs = new List<ItemSearch>();
            string searchWordFolder = "Scripts\\TWEC";
            string searchWordFileName = "SearchWordList.js";
            TWSqlDBContext db = new TWSqlDBContext();
            var allItemSearchs = db.ItemSearch.Where(x => x.SellingQty > 0).OrderByDescending(x => x.Qtyreg).ThenByDescending(x => x.SellingQty).Select(x => new { x.Name, x.ManufactureName }).Distinct().Take(8000).OrderBy(x => x.Name).ToList();
            var allManufactures = allItemSearchs.Select(x => x.ManufactureName).GroupBy(x => x).Select(g => new { manufactureName = g.Key, manufactureCount = g.Count() }).OrderByDescending(y => y.manufactureCount).Take(500).OrderBy(y => y.manufactureName);
            string folderName = CreateFolder(searchWordFolder);
            if (folderName != "")
            {
                string jsContent = "var availableTags = [\"";

                jsContent += string.Join("\", \"", allManufactures.Select(x => HttpUtility.JavaScriptStringEncode(x.manufactureName)));
                jsContent += "\", \"";
                jsContent += string.Join("\", \"", allItemSearchs.Select(x => HttpUtility.JavaScriptStringEncode(x.Name)));
                jsContent += "\"];";

                flag = OutPut2JS(folderName, searchWordFileName, jsContent);
            }
            return flag;
        }
        private bool OutPut2JS(string folder, string fileName, string jsContent)
        {
            bool flag = false;
            StreamWriter sw = null;
            try
            {
                sw = new StreamWriter(folder + "\\" + fileName, false, System.Text.Encoding.GetEncoding("utf-8"));
                sw.Write(jsContent);
                flag = true;
            }
            catch (Exception e)
            {

            }
            sw.Close();
            return flag;
        }
        private string CreateFolder(string FolderName)
        {
            string PathString = AppDomain.CurrentDomain.BaseDirectory + FolderName + "\\";
            try
            {
                if (Directory.Exists(PathString))
                {
                    return PathString;
                }
                System.IO.Directory.CreateDirectory(PathString);
                return PathString;

            }
            catch (Exception e)
            {
                return "";
            }
        }
    }
}
