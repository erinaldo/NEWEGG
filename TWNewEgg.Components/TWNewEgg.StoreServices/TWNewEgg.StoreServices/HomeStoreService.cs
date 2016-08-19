using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TWNewEgg.Models.DBModels.TWSQLDB;
using TWNewEgg.Models.DomainModels.Home;
using TWNewEgg.Models.DomainModels.Store;
using TWNewEgg.StoreRepoAdapters.Interface;
using TWNewEgg.StoreServices.Const;
using TWNewEgg.StoreServices.Interface;
using TWNewEgg.StoreServices.InternalModel;

namespace TWNewEgg.StoreServices
{
    public class HomeStoreService : IHomeStoreService
    {
        private IStoreRepoAdapter _iStoreRepoAdapter;
        private const int HOMESTORE_CATEGORYID = 0; // 指定CategoryID為0就代表是首頁.
        private const int LOGO_IMAGESIZE = 60;

        private StoreService _storeService;

        public HomeStoreService(IStoreRepoAdapter iStoreRepoAdapter)
        {
            this._iStoreRepoAdapter = iStoreRepoAdapter;
            _storeService = new StoreService(_iStoreRepoAdapter);
        }

        #region Service Interface

        public HomeStoreInfo GetHomeStoreInfo(List<int> shopWindowIndexList)
        {
            HomeStoreInfo retStore = new HomeStoreInfo()
            {
                ID="0",
                ShopWindowList = GetHomeShopWindows(shopWindowIndexList)
            };

            return retStore;
        }

        public List<HomeShopWindow> GetHomeShopWindows(List<int> shopWindowIndexList)
        {
            // 先把首頁的所有櫥窗取出來.
            List<SubCategory_NormalStore> shopWindows = this._iStoreRepoAdapter.NormalStore_GetAll()
                .Where(x => x.CategoryID == HOMESTORE_CATEGORYID && x.ShowAll == ConstShowAll.Show)
                .OrderBy(x => x.Showorder).ToList();

            List<SubCategory_NormalStore> needShopWindows = new List<SubCategory_NormalStore>();
            List<int> needIndexList = new List<int>();
            for (int i = 0; i < shopWindows.Count; i++)
            {
                // 找出要取的櫥窗集合.
                if (shopWindowIndexList == null || !shopWindowIndexList.Any() || shopWindowIndexList.Contains(i))
                {
                    needShopWindows.Add(shopWindows[i]);
                    needIndexList.Add(i);
                }
            }

            // 為了節省db存取次數, 先把所有必要的資料一次撈回來.
            List<int> shopWindowIDs = needShopWindows.Select(t => t.ID).ToList();
            HomeStoreDbData dbData = GetHomeStoreDbData(shopWindowIDs);
            
            List<HomeShopWindow> retWindows = new List<HomeShopWindow>();
            for (int i = 0; i < needShopWindows.Count; i++)
            {
                SubCategory_NormalStore window = needShopWindows[i];
                List<StoreBanner> logoImages = ConvertToShopWindowLogoImages(window.ID, dbData.LogoImages);
                string cssType = window.StyleClass.ToString();
                HomeShopWindow homeShopWindow = new HomeShopWindow()
                {
                    ID = window.ID.ToString(),
                    Index = needIndexList[i],
                    Title = window.Title,
                    CssType = cssType,
                    IconType = cssType,
                    BeltBanner = ConvertToBeltBanner(window),
                    BlockList = GetShopWindowBlocks(window.ID, dbData),
                    ImageList = logoImages
                };

                retWindows.Add(homeShopWindow);
            }

            return retWindows;
        }

        #endregion Service Interface

        private HomeStoreDbData GetHomeStoreDbData(List<int> shopWindowIDs)
        {
            HomeStoreDbData dbData = new HomeStoreDbData()
            {
                Blocks = GetDbWindowBlocks(shopWindowIDs),
                Cells = GetDbBlockCells(shopWindowIDs),
                GroupLinks = GetDbGroupLinks(shopWindowIDs),
                LogoImages = GetDbLogoImages(shopWindowIDs),
            };
            dbData.Items = GetAllShopWindowItems(dbData.Cells);

            return dbData;
        }

        /// <summary>
        /// 獲取所有櫥窗內的Item集合.
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        private List<StoreItemCell> GetAllShopWindowItems(List<ItemAndSubCategoryMapping_NormalStore> cells)
        {
            // 找出所有的ItemID.
            List<int> itemIDs = cells.Where(t => t.StyleClass == ConstDbCellStyle.Item && t.ItemID > 0).Select(t => t.ItemID)
                .Distinct().ToList();

            // 利用先前寫好的Service.
            //StoreService _storeService = new StoreService(_iStoreRepoAdapter);
            return _storeService.GetStoreItems(itemIDs);
        }

        /// <summary>
        /// 獲取所有櫥窗底下的群組連結.
        /// </summary>
        /// <param name="shopWindowIDs"></param>
        /// <returns></returns>
        private List<Subcategorygroup> GetDbGroupLinks(List<int> shopWindowIDs)
        {
            // 先撈出櫥窗內的所有Cells, 之後再照櫥窗和Block分類.
            List<Subcategorygroup> groupLinks = _iStoreRepoAdapter.Subcategorygroup_GetAll()
                .Where(t => shopWindowIDs.Contains(t.SubCategoryID) && t.ShowAll == ConstShowAll.Show).ToList();

            return groupLinks;
        }

        /// <summary>
        /// 獲取所有櫥窗底下的所有Logo圖.
        /// </summary>
        /// <param name="shopWindowIDs"></param>
        /// <returns></returns>
        private List<Subcategorylogo> GetDbLogoImages(List<int> shopWindowIDs)
        {
            // 先撈出櫥窗內的所有Cells, 之後再照櫥窗和Block分類.
            List<Subcategorylogo> logoImages = _iStoreRepoAdapter.Subcategorylogo_GetAll()
                .Where(t => shopWindowIDs.Contains(t.SubCategoryID) && t.ShowAll == ConstShowAll.Show).ToList();

            return logoImages;
        }

        /// <summary>
        /// 獲取多個櫥窗底下的所有Cells
        /// </summary>
        /// <param name="shopWindowIDs"></param>
        /// <returns></returns>
        private List<ItemAndSubCategoryMapping_NormalStore> GetDbBlockCells(List<int> shopWindowIDs)
        {
            // 先撈出櫥窗內的所有Cells, 之後再照櫥窗和Block分類.
            List<ItemAndSubCategoryMapping_NormalStore> dbCells = _iStoreRepoAdapter.NormalStoreItem_GetAll()
                .Where(t => shopWindowIDs.Contains(t.SubCategoryID)).ToList();

            return dbCells;
        }

        /// <summary>
        /// 獲取指定的多個shopWindow的block集合.
        /// </summary>
        /// <param name="shopWindowIDs"></param>
        /// <returns></returns>
        private List<WindowBlocks> GetDbWindowBlocks(List<int> shopWindowIDs)
        {
            List<WindowBlocks> dbBlocks = _iStoreRepoAdapter.WindowBlocks_GetAll()
                .Where(t => shopWindowIDs.Contains(t.SubCategoryID)).ToList();

            return dbBlocks;
        }

        /// <summary>
        /// 轉換腰帶區的資料.
        /// </summary>
        /// <param name="window">櫥窗資料</param>
        /// <returns>回傳腰帶區的資料</returns>
        private StoreBanner ConvertToBeltBanner(SubCategory_NormalStore window)
        {
            return new StoreBanner()
            {
                ID = window.ID.ToString(),
                Image = window.LogoImageURL,
                Title = window.Title,
                Url = window.StoreImageLinkURL
            };
        }

        /// <summary>
        /// 獲取每個櫥窗的組合區塊(Block)資料.
        /// </summary>
        /// <param name="shopWindowID">櫥窗ID</param>
        /// <returns>回傳該櫥窗的Block集合</returns>
        private List<WindowBlock> GetShopWindowBlocks(int shopWindowID, HomeStoreDbData dbData)
        {
            // 撈出櫥窗內的所有Block(一般而言只有1左-2中-3右 三個)
            List<WindowBlocks> dbBlocks = dbData.Blocks.Where(t => t.SubCategoryID == shopWindowID)
                .OrderBy(t => t.ZoneID).ToList();

            // 先撈出櫥窗內的所有Cells, 之後再照Block分類.
            List<ItemAndSubCategoryMapping_NormalStore> dbCells = dbData.Cells.Where(t => t.SubCategoryID == shopWindowID).ToList();

            // 櫥窗的群組連結或Logo都只有一組固定的.
            List<GroupLink> groupLinks = ConvertToShopWindowGroupLinks(shopWindowID, dbData.GroupLinks);
            List<StoreBanner> logoImages = ConvertToShopWindowLogoImages(shopWindowID, dbData.LogoImages);

            List<WindowBlock> retBlocks = new List<WindowBlock>();
            foreach (WindowBlocks block in dbBlocks)
            {
                WindowBlock winBlock = new WindowBlock()
                {
                    ID = block.SubCategoryID+"_"+block.ZoneID,
                    LayoutNumber = block.ZoneStyle,
                    CellList = ConvertToBlockCellList(dbCells, block.ZoneID, groupLinks, logoImages, dbData.Items)
                };

                retBlocks.Add(winBlock);
            }

            return retBlocks;
        }

        private List<StoreBanner> ConvertToShopWindowLogoImages(int shopWindowID, List<Subcategorylogo> dbLogoImages)
        {
            List<StoreBanner> retList = new List<StoreBanner>();

            List<Subcategorylogo> shopLogoImages = dbLogoImages.Where(t => t.SubCategoryID == shopWindowID)
                .OrderBy(t => t.Showorder).ToList();

            foreach (Subcategorylogo dbLogo in shopLogoImages)
            {
                retList.Add(new StoreBanner()
                {
                    ID = dbLogo.SubCategoryID+"_"+dbLogo.ManufactureID,
                    Title = dbLogo.ManufactureID.ToString(),
                    Image = dbLogo.ImageUrl ?? GenerateLogoImageUrl(dbLogo.ManufactureID, LOGO_IMAGESIZE),
                    Url = dbLogo.Clickpath,
                });
            }

            return retList;
        }

        private string GenerateLogoImageUrl(int manufactureID, int imageSize)
        {
            return string.Format("/pic/manufacture/{0}/{1}_1_{2}.jpg",
                (manufactureID / 10000).ToString("0000"),
                (manufactureID % 10000).ToString("0000"),
                imageSize);
        }

        private List<GroupLink> ConvertToShopWindowGroupLinks(int shopWindowID, List<Subcategorygroup> dbGroupLinks)
        {
            List<GroupLink> retList = new List<GroupLink>();

            List<Subcategorygroup> shopGroupLinks = dbGroupLinks.Where(t => t.SubCategoryID == shopWindowID)
                .OrderBy(t => t.Showorder).ToList();

            foreach (Subcategorygroup dbGroupLink in shopGroupLinks)
            {
                retList.Add(new GroupLink()
                {
                    ID = dbGroupLink.ID.ToString(),
                    Title = dbGroupLink.Description,
                    Url = dbGroupLink.Clickpath
                });
            }

            return retList;
        }

        private List<BlockCell> ConvertToBlockCellList(List<ItemAndSubCategoryMapping_NormalStore> dbCells, int zoneID,
            List<GroupLink> shopGroupLinks, List<StoreBanner> shopLogoImages, List<StoreItemCell> allShopWindowItems)
        {
            // 把屬於該Block的Cell過濾出來.
            List<ItemAndSubCategoryMapping_NormalStore> zoneBlockCells = 
                dbCells.Where(t => t.Zone == zoneID).OrderBy(t => t.Showorder).ToList();

            List<BlockCell> retCells = new List<BlockCell>();
            foreach (ItemAndSubCategoryMapping_NormalStore dbCell in zoneBlockCells)
            {
                BlockCell cell = ConvertToBlockCell(dbCell, shopGroupLinks, shopLogoImages, allShopWindowItems);
                if (cell != null)
                    retCells.Add(cell);

            }

            return retCells;
        }

        private BlockCell ConvertToBlockCell(ItemAndSubCategoryMapping_NormalStore dbCell, List<GroupLink> shopGroupLinks,
            List<StoreBanner> shopLogoImages, List<StoreItemCell> allShopWindowItems)
        {
            BlockCell retCell = new BlockCell();
            
            // 將DB存放的數字, 轉換為可讀性較高的字串.
            retCell.ID = dbCell.ID.ToString();
            retCell.Type = ConvertToCellType(dbCell.StyleClass);
            switch (retCell.Type)
            {
                case ConstCellType.GroupList:
                    retCell.GroupLinkList = shopGroupLinks;
                    break;
                case ConstCellType.Item:
                    retCell.Item = ConvertToStoreItem(dbCell, allShopWindowItems);
                    break;
                case ConstCellType.LogoList:
                    retCell.ImageList = shopLogoImages;
                    break;
                case ConstCellType.SingleImage:
                    retCell.ImageList = ConvertToSingleImageList(dbCell);
                    break;
                case ConstCellType.Text:
                    retCell.Text = dbCell.Title;
                    break;
                default: // 未定義的型態, 直接丟棄這個Cell不處理.
                    return null;
            }

            return retCell;
        }

        /// <summary>
        /// 根據Cell目前的值, 和先前收集的Item資訊, 挑出需要的該筆.
        /// </summary>
        /// <param name="dbCell"></param>
        /// <param name="allShopWindowItems"></param>
        /// <returns></returns>
        private StoreItemCell ConvertToStoreItem(ItemAndSubCategoryMapping_NormalStore dbCell, List<StoreItemCell> allShopWindowItems)
        {
            StoreItemCell retCell = allShopWindowItems.FirstOrDefault(t => t.ItemID == dbCell.ItemID);
            
            // ImageURL有值時,Item的Image用ImageURL取代.
            if (retCell != null && !string.IsNullOrWhiteSpace(dbCell.ImageURL))
            {
                retCell.ItemImage = dbCell.ImageURL;
            }

            return retCell;
        }

        ///// <summary>
        ///// 這個還可以再優化, 先頂著用.
        ///// </summary>
        ///// <param name="itemID"></param>
        ///// <returns></returns>
        //private StoreItemCell GetStoreItem(int itemID)
        //{
        //    StoreService _storeService = new StoreService(_iStoreRepoAdapter);

        //    List<int> itemIDs = new List<int>();
        //    itemIDs.Add(itemID);
        //    List<StoreItemCell> storeItems = _storeService.GetStoreItems(itemIDs);

        //    return storeItems[0];
        //}

        private List<StoreBanner> ConvertToSingleImageList(ItemAndSubCategoryMapping_NormalStore dbCell)
        {
            List<StoreBanner> retList = new List<StoreBanner>();
            retList.Add(new StoreBanner()
            {
                Image = dbCell.ImageURL,
                Title = dbCell.Title,
                Url = dbCell.LinkURL
            });

            return retList;
        }

        private string ConvertToCellType(int cellStyle)
        {
            string retType;
            switch (cellStyle)
            {
                case ConstDbCellStyle.Item:
                    retType = ConstCellType.Item;
                    break;
                case ConstDbCellStyle.Image:
                    retType = ConstCellType.SingleImage;
                    break;
                case ConstDbCellStyle.Text:
                    retType = ConstCellType.Text;
                    break;
                case ConstDbCellStyle.Group:
                    retType = ConstCellType.GroupList;
                    break;
                case ConstDbCellStyle.Logo:
                    retType = ConstCellType.LogoList;
                    break;
                default:
                    retType = ConstCellType.Undefined;
                    break;
            }

            return retType;
        }

    }
}
