using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TWNewEgg.ItemServices.Interface;
using TWNewEgg.SearchServices.Model;

namespace TWNewEgg.SearchServices
{
    public class CommonService
    {
        protected IItemDetailService _itemDetailService;
        private TWNewEgg.CategoryService.Service.BaseService oCategoryController = new TWNewEgg.CategoryService.Service.BaseService();

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
        
        public CommonService(IItemDetailService itemDetailService)
        {
            _itemDetailService = itemDetailService;
        }

        public virtual bool isContainsUnicodeCharacter(string input)
        {
            const int MaxAnsiCode = 255;
            return input.Any(c => c > MaxAnsiCode);
        }

        public virtual List<int> GetChildCategory(string orderCats)
        {
            List<int> allChildCategory = new List<int>();
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
                }
            }
            bottomCategoryIDs = GetBottomChildCategory(orderCategoryIDs);
            for (int i = 0; i < bottomCategoryIDs.Count; i++)
            {
                allChildCategory.Add(bottomCategoryIDs[i]);
            }
            return allChildCategory;
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
        public virtual List<SearchCategory> GetCategoryNumber(List<ItemSearch> AllResults, int? Cat, int? LID, int? Cty, int? BID, int? SID)
        {
            List<SearchCategory> categoryNumber = new List<SearchCategory>();

            var allCategoryID = AllResults.Select(x => new { category_id = x.CategoryID, category_layer = x.Layer }).GroupBy(x => new { x.category_id, x.category_layer }).ToList();

            foreach (var acategory in allCategoryID)
            {
                List<SearchCategory> parentCategory = new List<SearchCategory>();
                parentCategory = GetParentCategory(acategory.Key.category_id, acategory.Key.category_layer, Cty, BID, SID); //Get parents category
                categoryNumber.Add(new SearchCategory { categoryID = acategory.Key.category_id.ToString(), layer = acategory.Key.category_layer.ToString(), number = acategory.Count(), categoryName = parentCategory[0].categoryName, categoryDescr = parentCategory[0].categoryDescr, categoryShowOrder = parentCategory[0].categoryShowOrder, parentCategory = parentCategory });// set all category in this veriable
            }

            return categoryNumber;
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
        /// get parent category from this layer's category
        /// </summary>
        /// <param name="AllResults"></param>
        /// <param name="LID"></param>
        /// <returns></returns>
        public virtual Dictionary<string, SearchCategory> GetHotCategoryNumber(List<SearchCategory> AllResults, int? LID)
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


        //private DatabaseContext db = new DatabaseContext();
        /// <summary>
        /// Set price range, and make sure the range in all results price's range
        /// </summary>
        /// <param name="maxPrice"></param> Input Maximun price to this function
        /// <param name="Is"></param> Send all SearchItems to this function
        /// <returns></returns> It returns a dictionary, string for price range, and list<int> [0] is count number, [1] low bound price, [2] high bound price
        // maxPrice double 改 decimal
        public virtual Dictionary<string, List<int>> CountNumberPrice(decimal maxPrice, List<ItemSearch> AllResults)
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

        private List<int> GetBottomChildCategory(List<int> categoryIDs)
        {
            List<int> allCateogryIDs = new List<int>();
            allCateogryIDs.AddRange(oCategoryController.FindBottomLayerCategory(categoryIDs).Select(x => x.ID).ToList());
            return allCateogryIDs.Distinct().ToList();
        }
    }
}
