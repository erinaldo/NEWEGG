using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TWNewEgg.Models.DBModels.TWSQLDB;
using TWNewEgg.Models.DomainModels.Cart;
using TWNewEgg.Models.DomainModels.Item;

namespace TWNewEgg.ItemServices.Interface
{
    public interface IItemGroupService
    {
        bool IsValid(List<int> itemIds);
        CartItemGroup BuildItemGroup(List<PostCartItemGroup> PostCartItemGroup);

        /// <summary>
        /// 根據GroupId取得所有的ItemMarketGroup
        /// </summary>
        /// <param name="numGroupId">Group Id</param>
        /// <returns>List of ItemMarketGroup</returns>
        List<ItemMarketGroup> GetItemMarketGroupByGroupId(int numGroupId);

        /// <summary>
        /// 根據ItemId取得與Item相關的所有ItemGroup
        /// </summary>
        /// <param name="numItemId">Item Id</param>
        /// <returns>Dictionary<GroupId, List of ItemMarketGroup></returns>
        Dictionary<int, List<ItemMarketGroup>> GetRelativeItemMarketGroupByItemId(int numItemId);

        /// <summary>
        /// 取得屬性物件
        /// </summary>
        /// <param name="numItemId"></param>
        /// <returns>Dictionary<int of ItemId, list of ItemMarketGroup></returns>
        Dictionary<int, List<ItemMarketGroup>> GetItemMarketGroupByItemId(int argNumItemId);

        /// <summary>
        /// 取得屬性物件
        /// </summary>
        /// <param name="argListItemId">List of ItemId</param>
        /// <param name="argFilterCancelItem">過濾掉下線的賣場, true:過濾, false:不過濾</param>
        /// <returns>null or Dictionary<int of ItemId, List of ItemMarketGroup></returns>
        Dictionary<int, List<ItemMarketGroup>> GetItemMarketGroupByItemId(List<int> argListItemId, bool argFilterCancelItem);

        /// <summary>
        /// 取得屬性名稱
        /// </summary>
        /// <param name="NumItemId">int of Item Id</param>
        /// <returns>string of GroupName, if not has GroupName then return empty</returns>
        string GetItemMarketGroupNameByItemId(int argNumItemId);

        /// <summary>
        /// 取得屬性名稱
        /// </summary>
        /// <param name="argListItemId">list of Item Id</param>
        /// <returns>Dictionary<int of ItemId, string of GroupName>, if not has GroupName then return empty</returns>
        Dictionary<int, string> GetItemMarketGroupNameByItemIds(List<int> argListItemId);

    }
}
