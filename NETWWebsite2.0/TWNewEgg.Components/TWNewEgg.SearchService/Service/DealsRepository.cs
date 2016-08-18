using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using System.Data.Entity.Infrastructure;
using System.Threading.Tasks;
using TWNewEgg.DB;
using TWNewEgg.DB.TWSQLDB.Models.ExtModels;
using TWNewEgg.CategoryService.Service;

namespace TWNewEgg.SearchService.Service
{
    public class DealsRepository : IDeals
    {
        public List<ItemPreview> getItemUnderCategory(int page, int showNumber, int showAll, int showZero, List<int> brandIds, List<int> categoryIds, string orderByType, string orderBy, decimal priceCash)
        {
            List<ItemPreview> allItemUnderCategory = new List<ItemPreview>();
            string brandIdString = "";
            string categoryIdString = "";
            if (brandIds.Count == 0)
            {
                brandIdString = "";
            }
            else
            {
                brandIdString = string.Join(",", brandIds.ToArray());
            }
            if (categoryIds.Count == 0)
            {
                return allItemUnderCategory;
            }
            BaseService CategoryData = new BaseService();
            var allCategoryIds = CategoryData.FindBottomLayerCategory(categoryIds).Select(x => x.ID).ToList();
            categoryIdString = string.Join(",", allCategoryIds.ToArray());
            allItemUnderCategory.AddRange(getItemUnderCategorySP(page, showNumber, showAll, showZero, brandIdString, categoryIdString, orderByType, orderBy, priceCash));
            return allItemUnderCategory;
        }
        /// <summary>
        /// This function Call Stored Procedure from db.
        /// </summary>
        /// <param name="page">Display which pages.</param>
        /// <param name="showNumber">Set item number on each page.</param>
        /// <param name="showAll">Display all item without paging, 0: display paging, 1: display all.</param>
        /// <param name="showZero">Display non-zero/zero item, 0: don't display qty 0 item, 1: display all</param>
        /// <param name="brandIds">Set brand IDs ex: '236,285'</param>
        /// <param name="categoryIds">Set category IDs ex: '84,85,86'</param>
        /// <param name="orderByType">Set order condition, 'Qty', 'Hit', 'Price', 'Review'</param>
        /// <param name="orderBy">Set second order condition, 'DESC' or 'ASC'</param>
        /// <param name="priceCash">Set price condition, type decimal</param>
        /// <returns></returns>
        private List<ItemPreview> getItemUnderCategorySP(int page, int showNumber, int showAll, int showZero, string brandIds, string categoryIds, string orderByType, string orderBy, decimal priceCash)
        {
            TWSqlDBContext TWSql = new TWSqlDBContext();
            List<ItemPreview> underThisCategory = new List<ItemPreview>();

            TWSql.Database.Initialize(force: false);
            var cmd = TWSql.Database.Connection.CreateCommand();

            SqlParameter paramOne = new SqlParameter();
            paramOne.ParameterName = "@Page";
            paramOne.Value = page;
            cmd.Parameters.Add(paramOne);

            SqlParameter paramTwo = new SqlParameter();
            paramTwo.ParameterName = "@ShowNumber";
            paramTwo.Value = showNumber;
            cmd.Parameters.Add(paramTwo);

            SqlParameter paramThree = new SqlParameter();
            paramThree.ParameterName = "@ShowAll";
            paramThree.Value = showAll;
            cmd.Parameters.Add(paramThree);

            SqlParameter paramFour = new SqlParameter();
            paramFour.ParameterName = "@ShowZero";
            paramFour.Value = showZero;
            cmd.Parameters.Add(paramFour);

            SqlParameter paramFive = new SqlParameter();
            paramFive.ParameterName = "@BrandIds";
            paramFive.Value = brandIds;
            cmd.Parameters.Add(paramFive);

            SqlParameter paramSix = new SqlParameter();
            paramSix.ParameterName = "@Categoryids";
            paramSix.Value = categoryIds;
            cmd.Parameters.Add(paramSix);

            SqlParameter paramSeven = new SqlParameter();
            paramSeven.ParameterName = "@OrderByType";
            paramSeven.Value = orderByType;
            cmd.Parameters.Add(paramSeven);

            SqlParameter paramEight = new SqlParameter();
            paramEight.ParameterName = "@OrderBy";
            paramEight.Value = orderBy;
            cmd.Parameters.Add(paramEight);

            SqlParameter paramNine = new SqlParameter();
            paramNine.ParameterName = "@PriceCondition";
            paramNine.Value = priceCash;
            cmd.Parameters.Add(paramNine);

            cmd.CommandText = "[dbo].[UP_EC_GetItemByQtyV2] @Page, @ShowNumber, @ShowAll, @ShowZero, @BrandIds, @Categoryids, @OrderByType, @OrderBy, @PriceCondition";

            TWSql.Database.Connection.Open();
            var reader = cmd.ExecuteReader();

            underThisCategory = ((IObjectContextAdapter)TWSql).ObjectContext.Translate<ItemPreview>(reader).ToList();
            TWSql.Database.Connection.Close();
            return underThisCategory;
        }
    }
}
