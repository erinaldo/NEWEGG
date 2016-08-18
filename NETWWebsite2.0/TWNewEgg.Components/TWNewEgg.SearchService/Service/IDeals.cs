using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TWNewEgg.DB.TWSQLDB.Models.ExtModels;

namespace TWNewEgg.SearchService.Service
{
    public interface IDeals
    {
        /// <summary>
        /// This function Call Stored Procedure from db.
        /// </summary>
        /// <param name="page">Display which pages.</param>
        /// <param name="showNumber">Set item number on each page.</param>
        /// <param name="showAll">Display all item without paging, 0: display paging, 1: display all.</param>
        /// <param name="showZero">Display non-zero/zero item, 0: don't display qty 0 item, 1: display all</param>
        /// <param name="brandIds">Set brand IDs</param>
        /// <param name="categoryIds">Set category IDs</param>
        /// <param name="orderByType">Set order condition, 'Qty', 'Hit', 'Price', 'Review'</param>
        /// <param name="orderBy">Set second order condition, 'DESC' or 'ASC'</param>
        /// <param name="priceCash">Set price condition, type decimal</param>
        /// <returns></returns>
        List<ItemPreview> getItemUnderCategory(int page, int showNumber, int showAll, int showZero, List<int> brandIds, List<int> categoryIds, string orderByType, string orderBy, decimal priceCash);
    }
}
