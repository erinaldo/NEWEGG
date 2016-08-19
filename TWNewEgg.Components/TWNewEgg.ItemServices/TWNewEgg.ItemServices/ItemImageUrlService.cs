using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TWNewEgg.ItemServices.Interface;
using TWNewEgg.Models.DomainModels.Item;
using TWNewEgg.ItemRepoAdapters;
using TWNewEgg.ItemRepoAdapters.Interface;
using TWNewEgg.Models.DBModels.TWSQLDB;
using TWNewEgg.Framework.AutoMapper;

namespace TWNewEgg.ItemServices
{
    public class ItemImageUrlService:IItemImageUrlService
    {
        private IImageUrlReferenceRepoAdapter _imageUrlReferenceRepoAdapter;
        private IItemRepoAdapter _itemRepoAdapter;
        public ItemImageUrlService(IImageUrlReferenceRepoAdapter imageUrlReferenceRepoAdapter, IItemRepoAdapter itemRepoAdapter)
        {
            this._imageUrlReferenceRepoAdapter = imageUrlReferenceRepoAdapter;
            this._itemRepoAdapter = itemRepoAdapter;
        }
        //取得一批商品的所有規格的圖檔
        public Dictionary<int, List<ImageUrlReferenceDM>> GetItemImagePath(List<int> itemIDs)
        {
            itemIDs = itemIDs.Distinct().ToList();
            var RTN = new Dictionary<int, List<ImageUrlReferenceDM>>();
            Dictionary<int, List<ImageUrlReference>> result = _imageUrlReferenceRepoAdapter.GetAllImagePath(itemIDs);
            int[] ImageSize = new int[] { 60, 125, 300, 640 };
            //轉成domain model回傳
            foreach (int itemId in itemIDs)
            {
                RTN.Add(itemId, new List<ImageUrlReferenceDM>());
                if (result.ContainsKey(itemId) && result[itemId].Count > 0)
                {
                    //ImageUrlReference表有資料,用表裡面的
                    foreach (ImageUrlReference img in result[itemId])
                    {
                        RTN[itemId].Add(new ImageUrlReferenceDM
                        {
                            ItemID = img.ItemID,
                            Size = img.Size,
                            SizeIndex = img.SizeIndex,
                            ImageUrl = img.ImageUrl,
                            CreateUser = img.CreateUser,
                            CreateDate = img.CreateDate,
                            Updated = img.Updated,
                            UpdateUser = img.UpdateUser,
                            UpdateDate = img.UpdateDate
                        });
                    }
                }
                else
                {
                    Models.DBModels.TWSQLDB.Item eachItem = _itemRepoAdapter.GetIfAvailable(itemId);
                    if (eachItem == null)
                    {
                        continue;
                    }
                    int minCount = eachItem.PicStart ?? 1, maxCount = eachItem.PicEnd ?? 1;

                    for (int j = minCount; j <= maxCount; j++)
                    {
                        //ImageUrlReference表沒有資料,系統拚字串
                        for (int i = 0; i < ImageSize.Length; i++)
                        {
                            RTN[itemId].Add(new ImageUrlReferenceDM
                            {
                                ItemID = itemId,
                                Size = ImageSize[i],
                                SizeIndex = 1,
                                ImageUrl = string.Format("/pic/{0}/{1}/{2}_{3}_{4}.{5}", "item", (itemId / 10000).ToString("0000"), (itemId % 10000).ToString("0000"), j, ImageSize[i], "jpg"),
                                CreateUser = "",
                                UpdateUser = ""
                            });
                        }
                    }
                }
            }
            return RTN;
        }

        //取得單一商品的所有規格的圖檔
        public List<ImageUrlReferenceDM> GetSingleItemImagePath(int itemID)
        {
            var tmp = this.GetItemImagePath(new List<int>(new int[] { itemID }));
            if (tmp.ContainsKey(itemID) == false) 
                return null;
            else
                return tmp[itemID];
        }
    }
}
