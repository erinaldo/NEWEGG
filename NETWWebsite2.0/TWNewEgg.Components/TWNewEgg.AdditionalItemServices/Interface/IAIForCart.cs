using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TWNewEgg.Models.DomainModels.AdditionalItem;

namespace TWNewEgg.AdditionalItemServices.Interface
{
    public interface IAIForCart
    {
        /// <summary>
        /// Get all additional items.
        /// </summary>
        /// <param name="cartType"></param>
        /// <returns></returns>
        List<AllAIForCart> GetAllAdditionalItemDetail();
        /// <summary>
        /// Get additional items by cart type.
        /// </summary>
        /// <param name="cartType"></param>
        /// <returns></returns>
        List<AllAIForCart> GetAdditionalItemDetailByCartType(int cartType);
        /// <summary>
        /// 前台頁面顯示加購商品
        /// </summary>
        /// <param name="cartType"></param>
        /// <returns></returns>
        List<AllAIForCart> GetAdditionalItemDetailforShopByCartType(int cartType);
        /// <summary>
        /// Get all additional item, simple version.
        /// </summary>
        /// <param name="cartType"></param>
        /// <returns></returns>
        List<AIForCartDM> GetAllAdditionalItemSimple();
        /// <summary>
        /// Get additional items by cart type, simple version.
        /// </summary>
        /// <param name="cartType"></param>
        /// <returns></returns>
        List<AIForCartDM> GetAllAdditionalItemsimpleByCartType(int cartType);
        /// <summary>
        /// Add or Update enable addition item to for cart.
        /// </summary>
        /// <param name="itemID"></param>
        /// <param name="cartType"></param>
        /// <param name="limitedPrice"></param>
        /// <param name="specificStatus"></param>
        /// <returns></returns>
        int EnableAdditionalItem(int itemID, int cartType, string updateUser, decimal? limitedPrice, int? specificStatus);
        /// <summary>
        /// Add or Update planning addition item to for cart.
        /// </summary>
        /// <param name="itemID"></param>
        /// <param name="cartType"></param>
        /// <param name="limitedPrice"></param>
        /// <param name="specificStatus"></param>
        /// <returns></returns>
        int PlanningAdditionalItem(int itemID, int cartType, string updateUser, decimal? limitedPrice, int? specificStatus);
        /// <summary>
        /// Disable addition item for cart
        /// </summary>
        /// <param name="itemID">Item ID</param>
        /// <param name="cartType">Cart Type</param>
        /// <returns></returns>
        int DisableAdditionalItem(int itemID, int cartType, string updateUser);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="AdditionCartInput"></param>
        /// <returns></returns>

        List<Models.DomainModels.AdditionalItem.AllAIForCart> QeuryGetAdditionalItemDetailByCartType(TWNewEgg.Models.DomainModels.AdditionalItem.AdditionCartInput AdditionCartInput);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="cartType"></param>
        /// <param name="ItemID"></param>
        /// <returns></returns>
        bool CheckExiteforCartType(int cartType, int ItemID);

        /// <summary>
        /// 更新加價購(目前只提供排序順序修改)
        /// </summary>
        /// <param name="inputData">更新資訊</param>
        /// <returns>回饋訊息</returns>
        TWNewEgg.Models.DomainModels.Message.ResponseMessage<string> UpdateAdditionalItem(Models.DomainModels.AdditionalItem.UpdateAdditionalItem inputData);
    }
}
