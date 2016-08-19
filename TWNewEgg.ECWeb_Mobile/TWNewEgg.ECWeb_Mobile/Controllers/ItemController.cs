using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TWNewEgg.ECWeb.PrivilegeFilters;
using TWNewEgg.Framework.ServiceApi;
using TWNewEgg.Models.DomainModels.Item;
using TWNewEgg.Models.ViewModels.Item;

namespace TWNewEgg.ECWeb_Mobile.Controllers
{
    [AllowNonSecures]
    [AllowAnonymous]
    public class ItemController : Controller
    {
        //
        // GET: /Item/

        public ActionResult Index(int ItemId)
        {
            //考慮到資料傳輸與組合的效能, 麵包屑及所有父階的取得作業, 改由Client利用Ajax呼叫Api來完成
            ItemBasic objItemBasic = null;
            Dictionary<int, ItemUrl> itemUrls = new Dictionary<int, ItemUrl>();

            objItemBasic = Processor.Request<ItemBasic, ItemDetail>("ItemDetailService", "GetItemDetail", ItemId, "on").results;
            if (objItemBasic == null)
            {
                return View();
            }

            Dictionary<int, List<ItemUrl>> itemUrlDictionary = Processor.Request<Dictionary<int, List<ItemUrl>>, Dictionary<int, List<ImageUrlReferenceDM>>>("ItemImageUrlService", "GetItemImagePath", new List<int> { ItemId }).results;

            var listImgUrl = new List<string>();
            foreach (ItemUrl singleImgUrl in itemUrlDictionary[ItemId].Where(x => x.Size == 640))
            {
                if (singleImgUrl.ImageUrl.IndexOf("newegg.com/") >= 0)
                {
                    listImgUrl.Add(singleImgUrl.ImageUrl);
                }
                else
                {
                    listImgUrl.Add(string.Format("{0}{1}", "https://ssl-images.newegg.com.tw", singleImgUrl.ImageUrl));
                }
            }
            objItemBasic.ImgUrlList = listImgUrl;
            return View(objItemBasic);
        }
    }
}
