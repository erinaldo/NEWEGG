using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TWNewEgg.Framework.ServiceApi;
using TWNewEgg.Models.DBModels.TWSQLDB;
using TWNewEgg.Models.DomainModels.PromoActive;


namespace TWNewEgg.PromoActiveService.Interface
{
    public interface IPromoActiveService
    {
        List<PromoActiveDM> GetAllPromoActiveDM(int pageNumber = 1, int pageDataNumber = 10, string activityType = "Newest");
        //IPP Search
        //List<PromoActiveDM> GetIPPAllPromoActive(TWNewEgg.Models.DomainModels.PromoActive.SearchPromoModel searchPromoModel);
        

        // 搜尋行銷活動
        ResponsePacket<List<PromoActiveDM>> PromoActiveSearch(TWNewEgg.Models.DomainModels.PromoActive.SearchPromoModel searchPromoModel);

        //IPP Update and Create
        PromoActiveDM StorePromoActiveDetail(TWNewEgg.Models.DomainModels.PromoActive.PromoActiveDM promoActiveDM);

        /// <summary>
        /// 取得中獎名單頁的活動清單
        /// </summary>
        /// <param name="searchCondition">搜尋條件</param>
        /// <returns>中獎名單頁的活動清單</returns>
        ResponsePacket<TWNewEgg.Models.DomainModels.PromoActive.AwardDM> GetAwardList(TWNewEgg.Models.DomainModels.PromoActive.AwardListSearchConditionDM searchCondition);
    }
}
