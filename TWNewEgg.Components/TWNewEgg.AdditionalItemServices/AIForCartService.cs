using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TWNewEgg.Framework.AutoMapper;
using TWNewEgg.AdditionalItemServices.Interface;
using TWNewEgg.AdditionalItemRepoAdapters.Interface;
using TWNewEgg.ItemRepoAdapters.Interface;
using TWNewEgg.ItemServices.Interface;
using TWNewEgg.Models.DBModels.TWSQLDB;
using TWNewEgg.TrackRepoAdapters.Interface;
using System.Configuration;

namespace TWNewEgg.AdditionalItemServices
{
    public class AIForCartServiceOutput
    {
        public const int OK = 10000;
        public const int CANTFINDITEM = 10001;
        public const int WRONGCARTTYPE = 10002;
        public const int WRONGDELIVTYPE = 10003;
    }
    public class AIForCartService : IAIForCart
    {
        private IItemRepoAdapter _itemRepo;
        private IAIForCartRepoAdapter _aiForCartRepo;
        private IItemGroupService _itemGroupService;
        private IItemDetailService _itemDetailService;
        private IItemImageUrlService _itemImageUrlService;
        private ITrackRepoAdapter _trackRepo;

        public AIForCartService(IItemRepoAdapter itemRepo, IAIForCartRepoAdapter aiForCartRepo, IItemGroupService itemGroupService, IItemDetailService itemDetailService, IItemImageUrlService itemImageUrlService, ITrackRepoAdapter trackRepo)
        {
            this._itemRepo = itemRepo;
            this._trackRepo = trackRepo;
            this._aiForCartRepo = aiForCartRepo;
            this._itemGroupService = itemGroupService;
            this._itemDetailService = itemDetailService;
            this._itemImageUrlService = itemImageUrlService;
        }
        public List<Models.DomainModels.AdditionalItem.AllAIForCart> GetAllAdditionalItemDetail()
        {
            //query all additionalitem from table additionalitemforCart.
            //fetch all data about these additional item from database.
            throw new NotImplementedException();
        }

        public List<Models.DomainModels.AdditionalItem.AllAIForCart> GetAdditionalItemDetailByCartType(int cartType)
        {
            //Check cartType is valid or not.
            if (!Enum.IsDefined(typeof(AdditionalItemForCart.CartTypeStatus), cartType))
            {
                return null;
            }

            List<Models.DomainModels.AdditionalItem.AllAIForCart> results = new List<Models.DomainModels.AdditionalItem.AllAIForCart>();

            //query additionalitem by cartType from table additionalitemforCart.
            List<AdditionalItemForCart> aiForCartDB = this._aiForCartRepo.GetAll().Where(x => x.CartType == cartType).ToList();

            //fetch all data about these additional item from database.
            for (int i = 0; i < aiForCartDB.Count; i++)
            {
                Models.DomainModels.AdditionalItem.AllAIForCart result = new Models.DomainModels.AdditionalItem.AllAIForCart();

                result.itemDetail = this._itemDetailService.GetItemDetail(aiForCartDB[i].ItemID);

                if (aiForCartDB[i].ItemGroupID.HasValue)
                {
                    result.itemGroup = this._itemGroupService.GetRelativeItemMarketGroupByItemId(aiForCartDB[i].ItemID)[aiForCartDB[i].ItemGroupID.Value] ?? new List<Models.DomainModels.Item.ItemMarketGroup>();
                }
                else
                {
                    result.itemGroup = new List<Models.DomainModels.Item.ItemMarketGroup>();
                }

                result.additionalItemForCart = ModelConverter.ConvertTo<TWNewEgg.Models.DomainModels.AdditionalItem.AIForCartDM>(aiForCartDB[i]);

                // 新增商品排序功能 by Smoke 2016/03/11 
                result.Sequence = aiForCartDB[i].Sequence;

                results.Add(result);
            }

            return results;
        }

        public List<Models.DomainModels.AdditionalItem.AllAIForCart> QeuryGetAdditionalItemDetailByCartType(TWNewEgg.Models.DomainModels.AdditionalItem.AdditionCartInput AdditionCartInput)
        {
            List<Models.DomainModels.AdditionalItem.AllAIForCart> AllAIForCartList = new List<Models.DomainModels.AdditionalItem.AllAIForCart>();

            // 將加價購表中未排序的資料，排入目前已排序資料的最後方
            this.SetUnsortSequenceValue();

            var GetAdditionalItemDetailModel = GetAdditionalItemDetailByCartType(Int32.Parse(AdditionCartInput.CartType));
            IQueryable<Models.DomainModels.AdditionalItem.AllAIForCart> IAdditionalItemDetail;
            IAdditionalItemDetail = GetAdditionalItemDetailModel.Where(x => x.additionalItemForCart.Status == (int)AdditionalItemForCart.AdditionalItemStatus.Enable).AsQueryable();
            List<int> CategoryIDSdetail = new List<int>();

            if (AdditionCartInput.CategoryID != null && AdditionCartInput.CategoryID.Count() != 0)
            {

                foreach (var CategoryIDforstring in AdditionCartInput.CategoryID)
                {

                    int CategoryIDS = Int32.Parse(CategoryIDforstring);
                    CategoryIDSdetail.Add(CategoryIDS);
                }

                IAdditionalItemDetail = IAdditionalItemDetail.Where(x => CategoryIDSdetail.Contains(x.itemDetail.Main.ItemBase.CategoryID)).AsQueryable();
            }

            if (!string.IsNullOrEmpty(AdditionCartInput.ManufactorID))
            {
                int ManufactorID = Int32.Parse(AdditionCartInput.ManufactorID);
                IAdditionalItemDetail = IAdditionalItemDetail.Where(x => x.itemDetail.Main.ItemBase.SellerID == ManufactorID).AsQueryable();
            }
            if (!string.IsNullOrEmpty(AdditionCartInput.ProductStatus))
            {
                int ProductStatus = Int32.Parse(AdditionCartInput.ProductStatus);
                IAdditionalItemDetail = IAdditionalItemDetail.Where(x => x.itemDetail.Main.ItemBase.Status == ProductStatus).AsQueryable();
            }
            if (!string.IsNullOrEmpty(AdditionCartInput.SaerchKey) && !string.IsNullOrEmpty(AdditionCartInput.SaerchWord))
            {

                TWNewEgg.Models.DomainModels.AdditionalItem.AdditionCartInput.SaerchKeystatus searchKey = new Models.DomainModels.AdditionalItem.AdditionCartInput.SaerchKeystatus();
                int SearchCase = Int32.Parse(AdditionCartInput.SaerchKey);
                searchKey = (Models.DomainModels.AdditionalItem.AdditionCartInput.SaerchKeystatus)SearchCase;
                switch (searchKey)
                {
                    case TWNewEgg.Models.DomainModels.AdditionalItem.AdditionCartInput.SaerchKeystatus.賣場編號:
                        var itemID = Int32.Parse(AdditionCartInput.SaerchWord);
                        IAdditionalItemDetail = IAdditionalItemDetail.Where(x => x.itemDetail.Main.ItemBase.ID == itemID).AsQueryable();
                        break;
                    case TWNewEgg.Models.DomainModels.AdditionalItem.AdditionCartInput.SaerchKeystatus.商品編號:
                        var productID = Int32.Parse(AdditionCartInput.SaerchWord);
                        IAdditionalItemDetail = IAdditionalItemDetail.Where(x => x.itemDetail.Main.ItemBase.ProductID == productID).AsQueryable();
                        break;
                    case TWNewEgg.Models.DomainModels.AdditionalItem.AdditionCartInput.SaerchKeystatus.賣場名稱:
                        IAdditionalItemDetail = IAdditionalItemDetail.Where(x => x.itemDetail.Main.ItemBase.Name.IndexOf(AdditionCartInput.SaerchWord) != -1).AsQueryable();
                        break;
                    case TWNewEgg.Models.DomainModels.AdditionalItem.AdditionCartInput.SaerchKeystatus.商家商品編號:
                        IAdditionalItemDetail = IAdditionalItemDetail.Where(x => x.itemDetail.Main.ProductBase.SellerProductID.Equals(AdditionCartInput.SaerchWord)).AsQueryable();
                        break;
                    case TWNewEgg.Models.DomainModels.AdditionalItem.AdditionCartInput.SaerchKeystatus.規格品編號:
                        var GroupID = Int32.Parse(AdditionCartInput.SaerchWord);
                        IAdditionalItemDetail = IAdditionalItemDetail.Where(x => x.additionalItemForCart.ItemGroupID != null && x.additionalItemForCart.ItemGroupID.Value == GroupID).AsQueryable();
                        break;
                    default:
                        break;
                }

                // int ProductStatus = Int32.Parse(AdditionCartInput.ProductStatus);
                // IAdditionalItemDetail = GetAdditionalItemDetailModel.Where(x => x.itemDetail.Main.ItemBase.Status == ProductStatus).AsQueryable();
            }

            AllAIForCartList = IAdditionalItemDetail.OrderBy(x => x.Sequence).ToList();
            return AllAIForCartList;
        }

        /// <summary>
        /// 將加價購表中未排序的資料，排入目前已排序資料的最後方
        /// </summary>
        /// <returns>錯誤訊息</returns>
        private string SetUnsortSequenceValue()
        {
            string result = null;
            List<Models.DBModels.TWSQLDB.AdditionalItemForCart> additionalItemForCartCell = null;
            int maxSquence = 0;

            try
            {
                // 取得未排序的資料
                additionalItemForCartCell = this._aiForCartRepo.GetAll().Where(x => x.Sequence == 0).ToList();

                if (additionalItemForCartCell.Count == 0)
                {
                    throw new Exception("查無未排序資料。");
                }

                // 取得目前已排序的最大排序值
                maxSquence = this._aiForCartRepo.GetAll().Max(x => x.Sequence);

                // 將未排序的資料，一一排入目前已排序資料的最後方
                foreach(Models.DBModels.TWSQLDB.AdditionalItemForCart additionalItemForCart in additionalItemForCartCell)
                {
                    additionalItemForCart.Sequence = ++maxSquence;
                    this._aiForCartRepo.Update(additionalItemForCart);
                }

                additionalItemForCartCell = null;
            }
            catch (Exception ex)
            {
                result = ex.Message;
            }
            finally
            {
                if (additionalItemForCartCell != null)
                {
                    additionalItemForCartCell = null;
                }
            }

            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public List<Models.DomainModels.AdditionalItem.AllAIForCart> GetAdditionalItemDetailforItem(TWNewEgg.Models.DomainModels.AdditionalItem.AdditionCartInput AdditionCartInput)
        {
            List<int> CartType = new List<int>();
            // CartType.Add((int)TWNewEgg.Models.DomainModels.AdditionalItem.AIForCartDM.CartTypeStatus.ChooseAny);
            //CartType.Add((int)TWNewEgg.Models.DomainModels.AdditionalItem.AIForCartDM.CartTypeStatus.Domestic);
            // CartType.Add((int)TWNewEgg.Models.DomainModels.AdditionalItem.AIForCartDM.CartTypeStatus.Internation);

            //var GetAdditionalItemDetailModel = GetAdditionalItemDetailByCartType(Int32.Parse(AdditionCartInput.CartType));
            List<Models.DomainModels.AdditionalItem.AllAIForCart> AllAIForCartList = new List<Models.DomainModels.AdditionalItem.AllAIForCart>();
            List<int> ItemID = new List<int>();
            var ItemDetail = _itemRepo.GetAll().Where(x => x.ShowOrder == (int)TWNewEgg.Models.DBModels.TWSQLDB.Item.ShowOrderStatus.AddtionalItemForCart).Select(x => x.ID).ToList();
            List<Models.DomainModels.AdditionalItem.AllAIForCart> results = new List<Models.DomainModels.AdditionalItem.AllAIForCart>();
            foreach (var ItemDetailS in ItemDetail)
            {
                Models.DomainModels.AdditionalItem.AllAIForCart result = new Models.DomainModels.AdditionalItem.AllAIForCart();
                result.itemDetail = this._itemDetailService.GetItemDetail(ItemDetailS);

                Dictionary<int, List<Models.DomainModels.Item.ItemMarketGroup>> DIC = this._itemGroupService.GetRelativeItemMarketGroupByItemId(ItemDetailS);
                if (DIC != null)
                {
                    Models.DomainModels.AdditionalItem.AIForCartDM AIForCartDM = new Models.DomainModels.AdditionalItem.AIForCartDM();
                    //List<Models.DomainModels.Item.ItemMarketGroup> ItemMarketGroupList = new List<Models.DomainModels.Item.ItemMarketGroup>();
                    //ItemMarketGroupList.AddRange(DIC.SelectMany(x => x.Value).ToList());
                    //result.itemGroup = ItemMarketGroupList;
                    AIForCartDM.ItemID = ItemDetailS;
                    AIForCartDM.ItemGroupID = DIC.First().Key;
                    result.additionalItemForCart = new Models.DomainModels.AdditionalItem.AIForCartDM();
                    result.additionalItemForCart.ItemGroupID = new int();
                    result.additionalItemForCart.ItemGroupID = DIC.First().Key;
                    result.additionalItemForCart = AIForCartDM;
                    results.Add(result);
                }
                else
                {
                    result.itemGroup = new List<Models.DomainModels.Item.ItemMarketGroup>();
                    results.Add(result);
                }


            }

            //   (from x in ItemDetail)
            List<int> ItemAddID = new List<int>();

            // Models.DomainModels.AdditionalItem.AllAIForCart AllAIForCartInfo = new Models.DomainModels.AdditionalItem.AllAIForCart();
            //Models.DomainModels.Item.ItemBase ItemBase=new Models.DomainModels.Item.ItemBase();

            //ItemBase = ModelConverter.ConvertTo<Models.DomainModels.Item.ItemBase>(IAdditionalItemDetails);

            // AllAIForCartInfo.itemDetail.Main.ItemBase = ItemBase;

            List<int> CategoryIDSdetail = new List<int>();
            IQueryable<Models.DomainModels.AdditionalItem.AllAIForCart> IAdditionalItemDetail;
            IAdditionalItemDetail = results.AsQueryable();
            IAdditionalItemDetail = IAdditionalItemDetail.Where(x => x.itemDetail.Main.ItemBase.Status == (int)TWNewEgg.Models.DBModels.TWSQLDB.Item.status.已上架).AsQueryable();

            if (AdditionCartInput.CategoryID != null && AdditionCartInput.CategoryID.Count() != 0)
            {
                foreach (var CategoryIDforstring in AdditionCartInput.CategoryID)
                {

                    int CategoryIDS = Int32.Parse(CategoryIDforstring);
                    CategoryIDSdetail.Add(CategoryIDS);
                }

                IAdditionalItemDetail = IAdditionalItemDetail.Where(x => CategoryIDSdetail.Contains(x.itemDetail.Main.ItemBase.CategoryID)).AsQueryable();
            }
            if (!string.IsNullOrEmpty(AdditionCartInput.ManufactorID))
            {
                int ManufactorID = Int32.Parse(AdditionCartInput.ManufactorID);
                IAdditionalItemDetail = IAdditionalItemDetail.Where(x => x.itemDetail.Main.ItemBase.SellerID == ManufactorID).AsQueryable();
            }
            if (!string.IsNullOrEmpty(AdditionCartInput.ProductStatus))
            {
                int ProductStatus = Int32.Parse(AdditionCartInput.ProductStatus);
                IAdditionalItemDetail = IAdditionalItemDetail.Where(x => x.itemDetail.Main.ItemBase.Status == ProductStatus).AsQueryable();
            }
            if (!string.IsNullOrEmpty(AdditionCartInput.SaerchKey) && !string.IsNullOrEmpty(AdditionCartInput.SaerchWord))
            {

                TWNewEgg.Models.DomainModels.AdditionalItem.AdditionCartInput.SaerchKeystatus searchKey = new Models.DomainModels.AdditionalItem.AdditionCartInput.SaerchKeystatus();
                int SearchCase = Int32.Parse(AdditionCartInput.SaerchKey);
                searchKey = (Models.DomainModels.AdditionalItem.AdditionCartInput.SaerchKeystatus)SearchCase;
                switch (searchKey)
                {
                    case TWNewEgg.Models.DomainModels.AdditionalItem.AdditionCartInput.SaerchKeystatus.賣場編號:
                        var itemID = Int32.Parse(AdditionCartInput.SaerchWord);
                        IAdditionalItemDetail = IAdditionalItemDetail.Where(x => x.itemDetail.Main.ItemBase.ID == itemID).AsQueryable();
                        break;
                    case TWNewEgg.Models.DomainModels.AdditionalItem.AdditionCartInput.SaerchKeystatus.商品編號:
                        var productID = Int32.Parse(AdditionCartInput.SaerchWord);
                        IAdditionalItemDetail = IAdditionalItemDetail.Where(x => x.itemDetail.Main.ItemBase.ProductID == productID).AsQueryable();
                        break;
                    case TWNewEgg.Models.DomainModels.AdditionalItem.AdditionCartInput.SaerchKeystatus.賣場名稱:
                        IAdditionalItemDetail = IAdditionalItemDetail.Where(x => x.itemDetail.Main.ItemBase.Name.IndexOf(AdditionCartInput.SaerchWord) != -1).AsQueryable();
                        break;
                    case TWNewEgg.Models.DomainModels.AdditionalItem.AdditionCartInput.SaerchKeystatus.商家商品編號:
                        IAdditionalItemDetail = IAdditionalItemDetail.Where(x => x.itemDetail.Main.ProductBase.SellerProductID.Equals(AdditionCartInput.SaerchWord)).AsQueryable();
                        break;
                    case TWNewEgg.Models.DomainModels.AdditionalItem.AdditionCartInput.SaerchKeystatus.規格品編號:
                        var GroupID = Int32.Parse(AdditionCartInput.SaerchWord);
                        IAdditionalItemDetail = IAdditionalItemDetail.Where(x => x.additionalItemForCart != null && x.additionalItemForCart.ItemGroupID != null && x.additionalItemForCart.ItemGroupID.Value == GroupID).AsQueryable();
                        break;
                        break;
                    default:
                        break;
                }
            }
            if (IAdditionalItemDetail != null)
            {
                AllAIForCartList = IAdditionalItemDetail.ToList();
            }
            return AllAIForCartList;
        }

        public bool CheckExiteforCartType(int cartType, int ItemID)
        {

            List<Models.DomainModels.AdditionalItem.AllAIForCart> AllAIForCart = new List<Models.DomainModels.AdditionalItem.AllAIForCart>();
            List<AdditionalItemForCart> aiForCartDB = this._aiForCartRepo.GetAll().Where(x => x.CartType == cartType && x.ItemID == ItemID && x.Status == (int)AdditionalItemForCart.AdditionalItemStatus.Enable).ToList();
            if (aiForCartDB.Count() > 0)
            {
                return false;

            }
            else
            {
                return true;

            }

        }


        /// <summary>
        /// 前台頁面顯示加購商品
        /// </summary>
        /// <param name="cartType"></param>
        /// <returns></returns>
        public List<Models.DomainModels.AdditionalItem.AllAIForCart> GetAdditionalItemDetailforShopByCartType(int cartType)
        {
            // 排除賣場開賣日期限制,ShowOrder不為賣場加價購,賣場Status=0(正常),於加價購表內非開賣的商品
            List<Models.DomainModels.AdditionalItem.AllAIForCart> AllAIForCartList = GetAdditionalItemDetailByCartType(cartType).Where(
                x => (x.itemDetail.Main.ItemBase.DateStart < DateTime.Now) &&
                     (x.itemDetail.Main.ItemBase.DateEnd > DateTime.Now) &&
                     (x.itemDetail.Main.ItemBase.Status == 0) &&
                     (x.itemDetail.Main.ItemBase.ShowOrder == (int)TWNewEgg.Models.DBModels.TWSQLDB.Item.ShowOrderStatus.AddtionalItemForCart) &&
                     (x.additionalItemForCart.Status == (int)TWNewEgg.Models.DomainModels.AdditionalItem.AIForCartDM.AdditionalItemStatus.Enable)).ToList();

            // 符合加價構的ItemID
            List<int> AdditionalItemIDList = new List<int>();
            AdditionalItemIDList = AllAIForCartList.Select(x => x.additionalItemForCart.ItemID).ToList();

            // 撈出規格品,篩掉不符合加價購的規格品
            List<Models.DomainModels.AdditionalItem.AllAIForCart> AllAIForCartListresults = AllAIForCartList.Where(x => x.additionalItemForCart != null).Where(x => x.additionalItemForCart.ItemGroupID != null).GroupBy(x => x.additionalItemForCart.ItemGroupID).Select(x => x.FirstOrDefault()).ToList();
            foreach (var AllAIForCartListresultstemp in AllAIForCartListresults)
            {
                AllAIForCartListresultstemp.itemGroup = AllAIForCartListresultstemp.itemGroup.Where(x => AdditionalItemIDList.Contains(x.ItemId)).ToList();
            }

            // 將其他非規格品加入清單
            AllAIForCartListresults.AddRange(AllAIForCartList.Where(x => x.additionalItemForCart != null).Where(x => x.additionalItemForCart.ItemGroupID == null).ToList());

            List<int> itemIds = AllAIForCartList.Select(x => x.itemDetail.Main.ItemBase.ID).ToList();
            Dictionary<int, List<TWNewEgg.Models.DomainModels.Item.ImageUrlReferenceDM>> ImageUrlReferenceDMList = this._itemImageUrlService.GetItemImagePath(itemIds);

            foreach (var temp in AllAIForCartListresults)
            {
                if (temp.itemDetail.SellingQty <= 0)
                {
                    temp.itemDetail.SellingQty = -1;
                }
                else
                {
                    int delvType = temp.itemDetail.Main.ItemBase.DelvType;
                    if (temp.itemDetail.SellingQty > 10)
                    {
                        temp.itemDetail.SellingQty = 10;
                    }
                    if (delvType == (int)TWNewEgg.Models.DBModels.TWSQLDB.Item.tradestatus.三角 || delvType == (int)TWNewEgg.Models.DBModels.TWSQLDB.Item.tradestatus.海外切貨)
                    {
                        temp.itemDetail.SellingQty = 1;
                    }
                }

                if (temp.itemDetail.Main.ItemBase.imgPath == null || temp.itemDetail.Main.ItemBase.imgPath == "")
                {
                    if (ImageUrlReferenceDMList.Where(x => x.Key == temp.itemDetail.Main.ItemBase.ID).FirstOrDefault().Value.Count != 0)
                    {
                        temp.itemDetail.Main.ItemBase.imgPath = ImageUrlReferenceDMList[temp.itemDetail.Main.ItemBase.ID].Where(x => x.Size == 300).OrderBy(x => x.SizeIndex).Select(x => x.ImageUrl).FirstOrDefault();
                        if (temp.itemDetail.Main.ItemBase.imgPath == null || temp.itemDetail.Main.ItemBase.imgPath == "")
                        {
                            temp.itemDetail.Main.ItemBase.imgPath = ImageUrlReferenceDMList[temp.itemDetail.Main.ItemBase.ID].GroupBy(x => x.SizeIndex).FirstOrDefault().OrderByDescending(x => x.Size).Select(x => x.ImageUrl).FirstOrDefault();
                        }
                    }
                }
                // 填入圖片路徑
                if (temp.itemDetail.Main.ItemBase.imgPath != null && temp.itemDetail.Main.ItemBase.imgPath != "")
                {
                    if (temp.itemDetail.Main.ItemBase.imgPath.IndexOf("newegg.com/") >= 0)
                    {
                    }
                    else
                    {
                        string hostString = ConfigurationManager.AppSettings.Get("ECWebHttpsImgDomain");
                        temp.itemDetail.Main.ItemBase.imgPath = string.Format("{0}{1}", hostString, temp.itemDetail.Main.ItemBase.imgPath);
                    }
                }

                if (temp.itemGroup != null && temp.itemGroup.Count != 0)
                {
                    // 只撈出有量賣場
                    temp.itemGroup = temp.itemGroup.Where(x => x.SellingQty != 0).ToList();
                    foreach (var itemGrouptemp in temp.itemGroup)
                    {
                        itemGrouptemp.Status = 0;
                        if (AllAIForCartList.Where(x => x.itemDetail.Main.ItemBase.ID == itemGrouptemp.ItemId) != null && AllAIForCartList.Where(x => x.itemDetail.Main.ItemBase.ID == itemGrouptemp.ItemId).ToList().Count > 0)
                        {
                            itemGrouptemp.Status = AllAIForCartList.Where(x => x.itemDetail.Main.ItemBase.ID == itemGrouptemp.ItemId).Select(x => x.itemDetail.Main.ItemBase.Status).FirstOrDefault();
                            itemGrouptemp.SellingQty = AllAIForCartList.Where(x => x.itemDetail.Main.ItemBase.ID == itemGrouptemp.ItemId).Select(x => x.itemDetail.SellingQty).FirstOrDefault();
                            int delvType = AllAIForCartList.Where(x => x.itemDetail.Main.ItemBase.ID == itemGrouptemp.ItemId).Select(x => x.itemDetail.Main.ItemBase.DelvType).FirstOrDefault();

                            if (itemGrouptemp.SellingQty <= 0)
                            {
                                /// 關閉賣場
                                itemGrouptemp.Status = -1;
                            }
                            else
                            {
                                if (itemGrouptemp.SellingQty > 10)
                                {
                                    itemGrouptemp.SellingQty = 10;
                                }
                                if (delvType == (int)TWNewEgg.Models.DBModels.TWSQLDB.Item.tradestatus.三角 || delvType == (int)TWNewEgg.Models.DBModels.TWSQLDB.Item.tradestatus.海外切貨)
                                {
                                    itemGrouptemp.SellingQty = 1;
                                }
                            }
                        }
                    }
                    // 只撈出開賣賣場
                    temp.itemGroup = temp.itemGroup.Where(x => x.Status == 0).ToList();
                    if (temp.itemGroup.Count == 0)
                    {
                        temp.itemDetail.SellingQty = -1;
                    }
                }
                //else{
                //    if (temp.itemDetail.SellingQty <= 0)
                //    {
                //        temp.itemDetail.SellingQty = -1;
                //    }
                //    else {
                //        int delvType = temp.itemDetail.Main.ItemBase.DelvType;
                //        if (temp.itemDetail.SellingQty > 10)
                //        {
                //            temp.itemDetail.SellingQty = 10;
                //        }
                //        if (delvType == (int)TWNewEgg.Models.DBModels.TWSQLDB.Item.tradestatus.三角 || delvType == (int)TWNewEgg.Models.DBModels.TWSQLDB.Item.tradestatus.海外切貨)
                //        {
                //            temp.itemDetail.SellingQty = 1;
                //        }
                //    }
                //}            
            }
            AllAIForCartListresults = AllAIForCartListresults.Where(x => x.itemDetail.SellingQty != -1).OrderBy(X=>X.Sequence).ToList();
            return AllAIForCartListresults;
        }

        public List<Models.DomainModels.AdditionalItem.AIForCartDM> GetAllAdditionalItemSimple()
        {
            //query all additionalitem from table additionalitemforCart.
            throw new NotImplementedException();
        }

        public List<Models.DomainModels.AdditionalItem.AIForCartDM> GetAllAdditionalItemsimpleByCartType(int cartType)
        {
            //Check cartType is valid or not.
            if (!Enum.IsDefined(typeof(AdditionalItemForCart.CartTypeStatus), cartType))
            {
                return null;
            }
            List<Models.DomainModels.AdditionalItem.AIForCartDM> results = new List<Models.DomainModels.AdditionalItem.AIForCartDM>();
            //query additionalitem by cartType from table additionalitemforCart.
            List<AdditionalItemForCart> aiForCartDB = this._aiForCartRepo.GetAll().Where(x => x.CartType == cartType).ToList();
            //fetch all data about these additional item from database.
            for (int i = 0; i < aiForCartDB.Count; i++)
            {
                results.Add(ModelConverter.ConvertTo<TWNewEgg.Models.DomainModels.AdditionalItem.AIForCartDM>(aiForCartDB[i]));
            }
            return results;
        }

        public int EnableAdditionalItem(int itemID, int cartType, string updateUser, decimal? limitedPrice, int? specificStatus)
        {
            return SettingAdditionItem(itemID, (int)AdditionalItemForCart.AdditionalItemStatus.Enable, cartType, updateUser, limitedPrice, specificStatus);
        }

        public int PlanningAdditionalItem(int itemID, int cartType, string updateUser, decimal? limitedPrice, int? specificStatus)
        {
            return SettingAdditionItem(itemID, (int)AdditionalItemForCart.AdditionalItemStatus.Planning, cartType, updateUser, limitedPrice, specificStatus);
        }

        public int DisableAdditionalItem(int itemID, int cartType, string updateUser)
        {
            //Check is the Item exist.
            Item existItem = _itemRepo.GetIfAvailable(itemID);
            //if no item , then escape.
            if (existItem == null)
            {
                return AIForCartServiceOutput.CANTFINDITEM;
            }
            //if exist, then check item which is additional item or not, if yes, then change item to normal item.
            //if (existItem.ShowOrder == (int)Item.ShowOrderStatus.AddtionalItemForCart)
            //{
            //    existItem.ShowOrder = (int)Item.ShowOrderStatus.正常;
            //    try
            //    {
            //        _itemRepo.UpdateItem(existItem);
            //    }
            //    catch (Exception e)
            //    {
            //        throw e;
            //    }
            //}
            //Check itemid is in table additionalitem.
            AdditionalItemForCart additionalItem = _aiForCartRepo.GetAll().Where(x => x.ItemID == itemID && x.CartType == cartType).FirstOrDefault();
            //if exist, then update.
            //if not, then escape.
            DateTime dateNow = DateTime.UtcNow.AddHours(8);
            if (additionalItem != null)
            {
                additionalItem.Status = (int)AdditionalItemForCart.AdditionalItemStatus.Disable;
                additionalItem.UpdateDate = dateNow;
                additionalItem.UpdateUser = updateUser;
                try
                {
                    _aiForCartRepo.Update(additionalItem);
                }
                catch (Exception e)
                {
                    //existItem.ShowOrder = (int)Item.ShowOrderStatus.AddtionalItemForCart;
                    //_itemRepo.UpdateItem(existItem);
                    throw e;
                }
                List<Track> oldTrack = this._trackRepo.GetAll().Where(x => x.ItemID == itemID).ToList();
                this._trackRepo.DeleteTracks(oldTrack);
            }
            return AIForCartServiceOutput.OK;
        }

        private bool CheckDelivTpyeInCarType(int delivType, int carType)
        {
            if (carType == (int)TWNewEgg.Models.DomainModels.AdditionalItem.AIForCartDM.CartTypeStatus.Domestic ||
                carType == (int)TWNewEgg.Models.DomainModels.AdditionalItem.AIForCartDM.CartTypeStatus.ChooseAny)
            {
                if (delivType == (int)Item.tradestatus.間配 || delivType == (int)Item.tradestatus.三角)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
            else
            {
                return true;
            }
        }

        private int SettingAdditionItem(int itemID, int status, int cartType, string updateUser, decimal? limitedPrice, int? specificStatus)
        {
            //Check is the Item exist.
            int? itemOriShowOrder = null;
            Item existItem = _itemRepo.GetIfAvailable(itemID);
            //if no item , then escape.
            if (existItem == null)
            {
                return AIForCartServiceOutput.CANTFINDITEM;
            }
            if (!CheckDelivTpyeInCarType(existItem.DelvType, cartType))
            {
                return AIForCartServiceOutput.WRONGDELIVTYPE;
            }
            //if exist, then check item which is additional item or not, if not, then change item to additional item.
            if (existItem.ShowOrder != (int)Item.ShowOrderStatus.AddtionalItemForCart)
            {
                itemOriShowOrder = existItem.ShowOrder;
                existItem.ShowOrder = (int)Item.ShowOrderStatus.AddtionalItemForCart;
                try
                {
                    _itemRepo.UpdateItem(existItem);
                }
                catch (Exception e)
                {
                    throw e;
                }
            }
            //Check itemid is in table additionalitem.
            AdditionalItemForCart additionalItem = _aiForCartRepo.GetAll().Where(x => x.ItemID == itemID && x.CartType == cartType).FirstOrDefault();
            Dictionary<int, List<TWNewEgg.Models.DomainModels.Item.ItemMarketGroup>> itemGroupData = this._itemGroupService.GetRelativeItemMarketGroupByItemId(itemID);
            int? itemGroupID = null;
            if (itemGroupData != null && itemGroupData.Count > 0)
            {
                itemGroupID = itemGroupData.First().Key;
            }
            //if exist, then update.
            //if not, then create.
            DateTime dateNow = DateTime.UtcNow.AddHours(8);
            if (additionalItem != null)
            {
                additionalItem.Status = status;
                additionalItem.LimitedPrice = limitedPrice ?? 1;
                additionalItem.ItemGroupID = itemGroupID;
                additionalItem.Specific = specificStatus ?? (int)AdditionalItemForCart.SpecificStatus.AllAccount;
                additionalItem.CartType = cartType;
                additionalItem.UpdateDate = dateNow;
                additionalItem.UpdateUser = updateUser;

                try
                {
                    AddSequence();

                    // 重新加回的加價購排序為第一個
                    additionalItem.Sequence = 1;

                    _aiForCartRepo.Update(additionalItem);
                }
                catch (Exception e)
                {
                    if (itemOriShowOrder.HasValue)
                    {
                        existItem.ShowOrder = itemOriShowOrder.Value;
                        _itemRepo.UpdateItem(existItem);
                    }
                    throw e;
                }
            }
            else
            {
                additionalItem = new AdditionalItemForCart();
                additionalItem.ItemID = itemID;
                additionalItem.Status = status;
                additionalItem.LimitedPrice = limitedPrice ?? 1;
                additionalItem.ItemGroupID = itemGroupID;
                additionalItem.Specific = specificStatus ?? (int)AdditionalItemForCart.SpecificStatus.AllAccount;
                additionalItem.StartDate = dateNow;
                additionalItem.EndDate = dateNow.AddYears(50);
                additionalItem.CartType = cartType;
                additionalItem.CreateUser = updateUser;
                additionalItem.CreateDate = dateNow;

                // 新增的加價購排序為第一個
                additionalItem.Sequence = 1;

                try
                {
                    AddSequence();
                    _aiForCartRepo.Add(additionalItem);
                }
                catch (Exception e)
                {
                    if (itemOriShowOrder.HasValue)
                    {
                        existItem.ShowOrder = itemOriShowOrder.Value;
                        _itemRepo.UpdateItem(existItem);
                    }
                    throw e;
                }
            }
            return AIForCartServiceOutput.OK;
        }

        /// <summary>
        /// 將已排序的 Sequence 值 + 1
        /// </summary>
        /// <returns></returns>
        private string AddSequence()
        {
            string result = null;
            List<Models.DBModels.TWSQLDB.AdditionalItemForCart> additionalItemForCartCell = null;

            try
            {
                // 取得已排序的資料
                additionalItemForCartCell = this._aiForCartRepo.GetAll().Where(x => x.Sequence > 0).ToList();

                if (additionalItemForCartCell.Count == 0)
                {
                    throw new Exception("無已排序資料。");
                }

                // 將所有已排序的 Sequence 值 + 1
                foreach (Models.DBModels.TWSQLDB.AdditionalItemForCart additionalItemForCart in additionalItemForCartCell)
                {
                    additionalItemForCart.Sequence++;
                    this._aiForCartRepo.Update(additionalItemForCart);
                }

                additionalItemForCartCell = null;
            }
            catch (Exception ex)
            {
                result = ex.Message;
            }
            finally
            {
                if (additionalItemForCartCell != null)
                {
                    additionalItemForCartCell = null;
                }
            }

            return result;
        }

        /// <summary>
        /// 更新加價購(目前只提供排序順序修改)
        /// </summary>
        /// <param name="inputData">更新資訊</param>
        /// <returns>回饋訊息</returns>
        public TWNewEgg.Models.DomainModels.Message.ResponseMessage<string> UpdateAdditionalItem(Models.DomainModels.AdditionalItem.UpdateAdditionalItem inputData)
        {
            TWNewEgg.Models.DomainModels.Message.ResponseMessage<string> result = new Models.DomainModels.Message.ResponseMessage<string>();
            List<int> additionalNumCell = null;
            List<AdditionalItemForCart> updateData = null;
            DateTime updateDate = DateTime.Now;

            try
            {
                // 取出更新資料裡，所有的 ID
                additionalNumCell = inputData.AdditionalItemCell.Select(input => input.AdditionalNum).ToList();

                // 透過 ID 讀取要更新的資料
                updateData = _aiForCartRepo.GetAll().Where(x => additionalNumCell.Contains(x.ID)).ToList();

                additionalNumCell = null;

                // 更新資料
                foreach (AdditionalItemForCart additionalItem in updateData)
                {
                    additionalItem.Sequence = inputData.AdditionalItemCell.Where(input => input.AdditionalNum == additionalItem.ID).Select(input => input.Sequence).First();
                    additionalItem.UpdateUser = inputData.UpdateUser;
                    additionalItem.UpdateDate = updateDate;
                }

                // 儲存更新
                result = this.SaveUpdateDate(updateData);

                updateData = null;
            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.Data = null;
                result.Message = "更新失敗。";
                result.Error.Detail = ex.ToString();
            }
            finally
            {
                if (additionalNumCell != null)
                {
                    additionalNumCell = null;
                }

                if (updateData != null)
                {
                    updateData = null;
                }
            }

            return result;
        }

        /// <summary>
        /// 儲存更新
        /// </summary>
        /// <param name="updateData">更新資訊</param>
        /// <returns>回饋訊息</returns>
        private TWNewEgg.Models.DomainModels.Message.ResponseMessage<string> SaveUpdateDate(List<AdditionalItemForCart> updateData)
        {
            TWNewEgg.Models.DomainModels.Message.ResponseMessage<string> result = new Models.DomainModels.Message.ResponseMessage<string>();

            using(System.Transactions.TransactionScope transaction = new System.Transactions.TransactionScope())
            {
                try
                {
                    _aiForCartRepo.UpdateList(updateData);
                    transaction.Complete();

                    result.IsSuccess = true;
                    result.Message = "更新成功。";
                    result.Data = null;
                    result.Error = null;
                }
                catch(Exception ex)
                {
                    transaction.Dispose();

                    result.IsSuccess = false;
                    result.Message = "更新失敗。";
                    result.Data = null;
                    result.Error.Detail = ex.ToString();
                }
                finally
                {
                    if(transaction != null)
                    {
                        transaction.Dispose();
                    }
                }
            }

            return result;
        }
    }
}
