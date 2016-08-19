using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TWNewEgg.TrackServices.Interface;
using TWNewEgg.ItemServices.Interface;
using TWNewEgg.TrackRepoAdapters.Interface;
using TWNewEgg.Models.DomainModels.Track;
using TWNewEgg.Models.DBModels.TWSQLDB;
using TWNewEgg.Models.DomainModels.Item;
using TWNewEgg.AdditionalItemRepoAdapters.Interface;

namespace TWNewEgg.TrackServices
{
    public class TrackService : ITrackService
    {
        private ITrackRepoAdapter _trackRepoAdapter;
        private IItemDetailService _itemDetailService;
        private IAIForCartRepoAdapter _aiForCartRepo;
        private const string ALLNAME = "all";
        private const string CARTNAME = "cart";
        private const string WISHNAME = "wish";
        private const string CARTADDITIONALNAME = "cartadditional";
        private const string CHOOSEADDITIONALNAME = "chooseadditional";
        private const string INTERADDITIONALNAME = "interadditional";
        private const string CARTCHINAME = "購物";
        private const string WISHCHINAME = "追蹤";
        private const string ADDSUCCESS = "新增成功";
        private const string UPDATESUCCESS = "更新成功";
        private const string DELETESUCCESS = "刪除成功";
        private const string ADDEXSIST = "已存在清單中";
        private const string UPDATEDELETEFAIL = "無此商品編號";

        public TrackService(ITrackRepoAdapter trackRepoAdapter, IItemDetailService itemDetailService, IAIForCartRepoAdapter aiForCartRepo)
        {
            this._trackRepoAdapter = trackRepoAdapter;
            this._itemDetailService = itemDetailService;
            this._aiForCartRepo = aiForCartRepo;
        }


        public bool CleanOldAndUpdateTracks(int accountID, DateTime beforeDate)
        {
            List<Track> dbOriTracks = this._trackRepoAdapter.ReadTracks(accountID, null, beforeDate, null).ToList();
            if (dbOriTracks.Count > 0)
            {
                this._trackRepoAdapter.DeleteTracks(dbOriTracks);
            }
            DateTime dateNow = DateTime.UtcNow.AddHours(8);
            dbOriTracks = this._trackRepoAdapter.ReadTracks(accountID, null, null, null).ToList();
            for (int i = 0; i < dbOriTracks.Count; i++)
            {
                var itemDetail = this._itemDetailService.GetItemDetail(dbOriTracks[i].ItemID);
                if (itemDetail == null)
                {
                    List<Track> deleteTracks = new List<Track>();
                    deleteTracks.Add(dbOriTracks[i]);
                    this._trackRepoAdapter.DeleteTracks(deleteTracks);
                }
                if (itemDetail.SellingQty == 0 || itemDetail.Main.ItemBase.Status != 0 || itemDetail.Main.ItemBase.DateStart > dateNow || itemDetail.Main.ItemBase.DateEnd < dateNow)
                {
                    if (IsAddtionalItem(itemDetail.Main.ItemBase.ShowOrder))
                    {
                        List<Track> deleteTracks = new List<Track>();
                        deleteTracks.Add(dbOriTracks[i]);
                        this._trackRepoAdapter.DeleteTracks(deleteTracks);                        
                    }
                    else
                    {
                        this._trackRepoAdapter.UpdateTracks(ConvertUpdateToDBTrack(new TrackDM() { Status = 1, CategoryID = dbOriTracks[i].CategoryID, CategoryType = dbOriTracks[i].CategoryType }, dbOriTracks[i]), false);
                    }
                    continue;
                }
                // 刪除追蹤清單內加價購 但item商品ShowOrder狀態錯誤的商品
                else if(!(IsAddtionalItem(itemDetail.Main.ItemBase.ShowOrder)) && (UpdateToTracksItemStatus(dbOriTracks[i].Status) && dbOriTracks[i].Status != (int)TWNewEgg.Models.DBModels.TWSQLDB.Track.TrackStatus.購物車)){
                        List<Track> deleteTracks = new List<Track>();
                        deleteTracks.Add(dbOriTracks[i]);
                        this._trackRepoAdapter.DeleteTracks(deleteTracks);        
                        continue;
                }
            }

            List<int> AdditionTrackStatus = new List<int>();
            AdditionTrackStatus.Add((int)TWNewEgg.Models.DBModels.TWSQLDB.Track.TrackStatus.任選館購物車加價商品);
            AdditionTrackStatus.Add((int)TWNewEgg.Models.DBModels.TWSQLDB.Track.TrackStatus.海外購物車加價商品);
            AdditionTrackStatus.Add((int)TWNewEgg.Models.DBModels.TWSQLDB.Track.TrackStatus.國內購物車加價商品);
            dbOriTracks = dbOriTracks.Where(x => AdditionTrackStatus.Contains(x.Status)).ToList();

            // 加價購商品檢察
            if (dbOriTracks.Count > 0)
            {
                List<int> AdditionItemID = dbOriTracks.Select(x => x.ItemID).ToList();
                List<AdditionalItemForCart> aiForCartDB = this._aiForCartRepo.GetAll().Where(x => AdditionItemID.Contains(x.ItemID)).ToList();
                foreach (var temp in dbOriTracks) {
                    int CartTypetemp = 0;
                    switch(temp.Status)
                    {
                        case (int)TWNewEgg.Models.DBModels.TWSQLDB.Track.TrackStatus.國內購物車加價商品:
                            CartTypetemp = (int)TWNewEgg.Models.DBModels.TWSQLDB.AdditionalItemForCart.CartTypeStatus.Domestic;
                            break;
                        case (int)TWNewEgg.Models.DBModels.TWSQLDB.Track.TrackStatus.海外購物車加價商品:
                            CartTypetemp = (int)TWNewEgg.Models.DBModels.TWSQLDB.AdditionalItemForCart.CartTypeStatus.Internation;
                            break;
                        case (int)TWNewEgg.Models.DBModels.TWSQLDB.Track.TrackStatus.任選館購物車加價商品:
                            CartTypetemp = (int)TWNewEgg.Models.DBModels.TWSQLDB.AdditionalItemForCart.CartTypeStatus.ChooseAny;
                            break;
                        default:
                            CartTypetemp = (int)TWNewEgg.Models.DBModels.TWSQLDB.AdditionalItemForCart.CartTypeStatus.無定義;
                            break;
                    }
                    if (aiForCartDB.Where(x => x.ItemID == temp.ItemID && x.CartType == CartTypetemp && x.Status != (int)TWNewEgg.Models.DBModels.TWSQLDB.AdditionalItemForCart.AdditionalItemStatus.Enable).Count() > 0)
                    {
                        List<Track> deleteTracks = new List<Track>();
                        deleteTracks.Add(temp);
                        this._trackRepoAdapter.DeleteTracks(deleteTracks);
                    }
                }
            }


            return true;
        }

        public List<string> AddToTracks(int accountID, List<TrackDM> newTracks)
        {
            List<string> results = new List<string>();
            if (newTracks == null)
            {
                return null;
            }
            newTracks = newTracks.Distinct().ToList();
            DateTime dateNow = DateTime.UtcNow.AddHours(8);
            List<Track> dbAddTracks = new List<Track>();
            List<Track> dbOriTracks = this._trackRepoAdapter.ReadTracks(accountID, null, null, null).ToList();
            for (int i = 0; i < newTracks.Count; i++)
            {
                var itemDetail = this._itemDetailService.GetItemDetail(newTracks[i].ItemID);
                if (itemDetail == null || !Enum.IsDefined(typeof(Track.TrackStatus), newTracks[i].Status))
                {
                    continue;
                }
                if (itemDetail.SellingQty == 0 || itemDetail.Main.ItemBase.Status != 0 || itemDetail.Main.ItemBase.DateStart > dateNow || itemDetail.Main.ItemBase.DateEnd < dateNow)
                {
                    // 沒量不做
                    newTracks[i].Status = 1;
                }
                var oriTracks = dbOriTracks.Where(x => x.ACCID == accountID && x.ItemID == newTracks[i].ItemID && x.Status == newTracks[i].Status).FirstOrDefault();
                if (oriTracks == null)
                {
                    if (UpdateToTracksItemStatus(newTracks[i].Status) && newTracks[i].Status != (int)TWNewEgg.Models.DBModels.TWSQLDB.Track.TrackStatus.購物車)
                    {
                        int EnableVal = (int)TWNewEgg.Models.DBModels.TWSQLDB.AdditionalItemForCart.AdditionalItemStatus.Enable;
                        int ItemIDTemp = newTracks[i].ItemID;
                        int StatusTemp = TrackStatusToCartType(newTracks[i].Status);
                        List<AdditionalItemForCart> aiForCartDB = this._aiForCartRepo.GetAll().Where(x => x.ItemID == ItemIDTemp && x.Status == EnableVal && x.CartType == StatusTemp).ToList();
                        if (aiForCartDB != null && aiForCartDB.Count > 0)
                        {
                            this._trackRepoAdapter.AddTracks(ConvertNewToDBTrack(accountID, newTracks[i]));
                            results.Add(string.Format(ADDSUCCESS, newTracks[i].ItemID));
                        }
                        else {
                            results.Add(string.Format(UPDATEDELETEFAIL, newTracks[i].ItemID));
                        }
                    }
                    else
                    {
                        //add to track
                        this._trackRepoAdapter.AddTracks(ConvertNewToDBTrack(accountID, newTracks[i]));
                        results.Add(string.Format(ADDSUCCESS, newTracks[i].ItemID));
                    }
                }
                else
                {
                    if (UpdateToTracksItemStatus(newTracks[i].Status) && newTracks[i].Status != (int)TWNewEgg.Models.DBModels.TWSQLDB.Track.TrackStatus.購物車)
                    {
                        int EnableVal = (int)TWNewEgg.Models.DBModels.TWSQLDB.AdditionalItemForCart.AdditionalItemStatus.Enable;
                        int ItemIDTemp = newTracks[i].ItemID;
                        int StatusTemp = TrackStatusToCartType(newTracks[i].Status);
                        List<AdditionalItemForCart> aiForCartDB = this._aiForCartRepo.GetAll().Where(x => x.ItemID == ItemIDTemp && x.Status == EnableVal && x.CartType == StatusTemp).ToList();
                        if (aiForCartDB != null && aiForCartDB.Count > 0)
                        {
                            if (oriTracks.Status == newTracks[i].Status && oriTracks.Qty == newTracks[i].Qty && oriTracks.CategoryID == newTracks[i].CategoryID && oriTracks.CategoryType == newTracks[i].CategoryType)
                            {
                                //return message;
                                results.Add(string.Format(ADDEXSIST, newTracks[i].ItemID, (UpdateToTracksItemStatus(newTracks[i].Status)) ? CARTCHINAME : WISHCHINAME));
                            }
                            else
                            {
                                //update
                                this._trackRepoAdapter.UpdateTracks(ConvertUpdateToDBTrack(newTracks[i], oriTracks), true);
                                results.Add(string.Format(ADDSUCCESS, newTracks[i].ItemID));
                            }
                        }
                        else
                        {
                            List<Track> deleteTracks = new List<Track>();
                            deleteTracks.Add(oriTracks);
                            this._trackRepoAdapter.DeleteTracks(deleteTracks);
                            results.Add(string.Format(DELETESUCCESS, newTracks[i].ItemID));
                        }
                    }
                    else
                    {
                        if (oriTracks.Status == newTracks[i].Status && oriTracks.Qty == newTracks[i].Qty && oriTracks.CategoryID == newTracks[i].CategoryID && oriTracks.CategoryType == newTracks[i].CategoryType)
                        {
                            //return message;
                            results.Add(string.Format(ADDEXSIST, newTracks[i].ItemID, (UpdateToTracksItemStatus(newTracks[i].Status)) ? CARTCHINAME : WISHCHINAME));
                        }
                        else
                        {
                            //update
                            this._trackRepoAdapter.UpdateTracks(ConvertUpdateToDBTrack(newTracks[i], oriTracks), true);
                            results.Add(string.Format(ADDSUCCESS, newTracks[i].ItemID));
                        }
                    }

                }
            }

            return results;
        }

        public List<string> UpdateToTracks(int accountID, List<TrackDM> updateTracks)
        {
            List<string> results = new List<string>();
            if (updateTracks == null)
            {
                return null;
            }
            updateTracks = updateTracks.Distinct().ToList();
            DateTime dateNow = DateTime.UtcNow.AddHours(8);
            List<Track> dbAddTracks = new List<Track>();
            List<Track> dbOriTracks = this._trackRepoAdapter.ReadTracks(accountID, null, null, null).ToList();
            for (int i = 0; i < updateTracks.Count; i++)
            {
                var itemDetail = this._itemDetailService.GetItemDetail(updateTracks[i].ItemID);
                if (itemDetail == null || !Enum.IsDefined(typeof(Track.TrackStatus), updateTracks[i].Status))
                {
                    continue;
                }
                if (itemDetail.SellingQty == 0 || itemDetail.Main.ItemBase.Status != 0 || itemDetail.Main.ItemBase.DateStart > dateNow || itemDetail.Main.ItemBase.DateEnd < dateNow)
                {
                    updateTracks[i].Status = 1;
                }
                Track oriTracks = dbOriTracks.Where(x => x.ACCID == accountID && x.ItemID == updateTracks[i].ItemID && x.Status == updateTracks[i].Status).FirstOrDefault();
                if (oriTracks == null)
                {
                    oriTracks = dbOriTracks.Where(x => x.ACCID == accountID && x.ItemID == updateTracks[i].ItemID).FirstOrDefault();
                }

                if (oriTracks == null)
                {
                    results.Add(string.Format(UPDATEDELETEFAIL));
                }
                else
                {
                    this._trackRepoAdapter.UpdateTracks(ConvertUpdateToDBTrack(updateTracks[i], oriTracks), true);
                    results.Add(string.Format(UPDATESUCCESS, updateTracks[i].ItemID));
                }
            }

            return results;
        }

        public List<string> DeleteFromTracks(int accountID, List<TrackDM> deleteTracks)
        {
            List<string> results = new List<string>();
            if (deleteTracks == null)
            {
                return null;
            }
            deleteTracks = deleteTracks.Distinct().ToList();
            List<Track> dbOriTracks = this._trackRepoAdapter.ReadTracks(accountID, null, null, null).ToList();
            for (int i = 0; i < deleteTracks.Count; i++)
            {
                Track oriTrack = new Track();
                if (UpdateToTracksItemStatus(deleteTracks[i].Status) && deleteTracks[i].Status != (int)TWNewEgg.Models.DBModels.TWSQLDB.Track.TrackStatus.購物車)
                {
                    oriTrack = dbOriTracks.Where(x => x.ACCID == accountID && x.ItemID == deleteTracks[i].ItemID && x.Status == deleteTracks[i].Status).FirstOrDefault();
                }
                else {
                    oriTrack = dbOriTracks.Where(x => x.ACCID == accountID && x.ItemID == deleteTracks[i].ItemID).FirstOrDefault();
                }
                if (oriTrack == null)
                {
                    results.Add(string.Format(UPDATEDELETEFAIL, deleteTracks[i].ItemID));
                }
                else
                {
                    List<Track> deletedbTracks = new List<Track>();
                    deletedbTracks.Add(oriTrack);
                    this._trackRepoAdapter.DeleteTracks(deletedbTracks);
                    results.Add(string.Format(DELETESUCCESS, deleteTracks[i].ItemID));
                }
            }

            return results;
        }

        private List<Track> ConvertNewToDBTrack(int accountID, TrackDM trackDM)
        {
            List<Track> newTrackList = new List<Track>();
            Track newTrack = new Track();
            newTrack.ACCID = accountID;
            newTrack.CategoryID = trackDM.CategoryID;
            newTrack.CategoryType = trackDM.CategoryType;
            newTrack.ItemID = trackDM.ItemID;
            newTrack.Status = trackDM.Status;
            newTrack.Qty = trackDM.Qty;
            newTrackList.Add(newTrack);
            return newTrackList;
        }
        private Track ConvertUpdateToDBTrack(TrackDM trackDM, Track oriTrack)
        {
            oriTrack.CategoryID = trackDM.CategoryID;
            oriTrack.CategoryType = trackDM.CategoryType;
            oriTrack.Status = trackDM.Status;
            oriTrack.Qty = trackDM.Qty;
            return oriTrack;
        }


        public Dictionary<string, List<TrackItem>> GetTracksStatus(int accountID, int? categoryID = null)
        {
            Dictionary<string, List<TrackItem>> results = new Dictionary<string, List<TrackItem>>();
            IQueryable<Track> readDBTracks = this._trackRepoAdapter.ReadTracks(accountID, null, null, null);
            if (categoryID != null)
            {
                readDBTracks = readDBTracks.Where(x => x.CategoryID == categoryID);
            }
            List<Track> dbOriTracks = readDBTracks.ToList();
            List<Track> cartTracks = dbOriTracks.Where(x => x.Status == 0).ToList();
            List<Track> wishTracks = dbOriTracks.Where(x => x.Status == 1).ToList();

            List<TrackItem> all = new List<TrackItem>();
            List<TrackItem> cart = new List<TrackItem>();
            List<TrackItem> wish = new List<TrackItem>();
            List<TrackItem> chooseadditional = new List<TrackItem>();
            List<TrackItem> cartadditional = new List<TrackItem>();
            List<TrackItem> interadditional = new List<TrackItem>();

            for (int i = 0; i < dbOriTracks.Count; i++)
            {
                all.Add(new TrackItem() { ItemID = dbOriTracks[i].ItemID, CategoryID = dbOriTracks[i].CategoryID, CategoryType = dbOriTracks[i].CategoryType, ItemQty = dbOriTracks[i].Qty ?? 1, CreateDate = dbOriTracks[i].CreateDate });
                InsertIntoAdditionalCart(ref cartadditional, ref interadditional, ref chooseadditional, dbOriTracks[i]);
            }
            for (int i = 0; i < cartTracks.Count; i++)
            {
                cart.Add(new TrackItem() { ItemID = cartTracks[i].ItemID, CategoryID = cartTracks[i].CategoryID, CategoryType = cartTracks[i].CategoryType, ItemQty = cartTracks[i].Qty ?? 1, CreateDate = cartTracks[i].CreateDate });
            }
            for (int i = 0; i < wishTracks.Count; i++)
            {
                wish.Add(new TrackItem() { ItemID = wishTracks[i].ItemID, CategoryID = wishTracks[i].CategoryID, CategoryType = wishTracks[i].CategoryType, ItemQty = wishTracks[i].Qty ?? 1, CreateDate = wishTracks[i].CreateDate });
            }

            results.Add(ALLNAME, all);
            results.Add(CARTNAME, cart);
            results.Add(WISHNAME, wish);
            results.Add(CHOOSEADDITIONALNAME, chooseadditional);
            results.Add(CARTADDITIONALNAME, cartadditional);
            results.Add(INTERADDITIONALNAME, interadditional);

            return results;
        }

        public Dictionary<string, List<TrackItem>> GetTracksDetial(int accountID)
        {
            Dictionary<string, List<TrackItem>> results = new Dictionary<string, List<TrackItem>>();
            List<Track> dbOriTracks = this._trackRepoAdapter.ReadTracks(accountID, null, null, null).ToList();

            List<ItemDetail> allItems = new List<ItemDetail>();
            for (int i = 0; i < dbOriTracks.Count; i++)
            {
                var item = this._itemDetailService.GetItemDetail(dbOriTracks[i].ItemID);
                if (item == null)
                {
                    continue;
                }
                allItems.Add(item);
            }

            TrackPacket cart = new TrackPacket();
            cart.Name = CARTNAME;
            cart.Items = new List<TrackItem>();
            TrackPacket wish = new TrackPacket();
            wish.Name = WISHNAME;
            wish.Items = new List<TrackItem>();
            var cartTracks = dbOriTracks.Where(x => x.Status == 0).ToList();
            var wishTracks = dbOriTracks.Where(x => x.Status == 1).ToList();
            for (int i = 0; i < cartTracks.Count; i++)
            {
                var item = allItems.Where(x => x.Main.ItemBase.ID == cartTracks[i].ItemID).FirstOrDefault();
                if (item != null)
                {
                    cart.Items.Add(new TrackItem() { ItemID = item.Main.ItemBase.ID, ItemName = item.Main.ItemBase.Name, ItemPrice = item.Price.DisplayPrice, ItemQty = cartTracks[i].Qty ?? 1, CategoryID = cartTracks[i].CategoryID, CategoryType = cartTracks[i].CategoryType, CreateDate = cartTracks[i].CreateDate });
                }
            }
            for (int i = 0; i < wishTracks.Count; i++)
            {
                var item = allItems.Where(x => x.Main.ItemBase.ID == wishTracks[i].ItemID).FirstOrDefault();
                if (item != null)
                {
                    wish.Items.Add(new TrackItem() { ItemID = item.Main.ItemBase.ID, ItemName = item.Main.ItemBase.Name, ItemPrice = item.Price.DisplayPrice, ItemQty = GetWishCartSellingQty(item), CategoryID = wishTracks[i].CategoryID, CategoryType = wishTracks[i].CategoryType, CreateDate = wishTracks[i].CreateDate });
                }
            }
            results.Add(cart.Name, cart.Items);
            results.Add(wish.Name, wish.Items);
            return results;
        }

        private void InsertIntoAdditionalCart(ref List<TrackItem> cartAdditionCart, ref List<TrackItem> interAdditionCart, ref List<TrackItem> chooseAdditionCart, Track dbTrack)
        {
            switch (dbTrack.Status)
            {
                case (int)Track.TrackStatus.國內購物車加價商品:
                    cartAdditionCart.Add(new TrackItem() { ItemID = dbTrack.ItemID, CategoryID = dbTrack.CategoryID, CategoryType = dbTrack.CategoryType, ItemQty = dbTrack.Qty ?? 1, CreateDate = dbTrack.CreateDate });
                    break;
                case (int)Track.TrackStatus.海外購物車加價商品:
                    interAdditionCart.Add(new TrackItem() { ItemID = dbTrack.ItemID, CategoryID = dbTrack.CategoryID, CategoryType = dbTrack.CategoryType, ItemQty = dbTrack.Qty ?? 1, CreateDate = dbTrack.CreateDate });
                    break;
                case (int)Track.TrackStatus.任選館購物車加價商品:
                    chooseAdditionCart.Add(new TrackItem() { ItemID = dbTrack.ItemID, CategoryID = dbTrack.CategoryID, CategoryType = dbTrack.CategoryType, ItemQty = dbTrack.Qty ?? 1, CreateDate = dbTrack.CreateDate });
                    break;
                default:
                    break;
            }
        }

        private int GetWishCartSellingQty(ItemDetail itemDetail)
        {
            int sellingQty = itemDetail.SellingQty;

            if (itemDetail.Main.ItemBase.DateEnd < DateTime.UtcNow.AddHours(8) || itemDetail.Main.ItemBase.DateStart > DateTime.UtcNow.AddHours(8))
            {
                sellingQty = 0;
            }
            if (itemDetail.Main.ItemBase.Status != (int)TWNewEgg.Models.DBModels.TWSQLDB.Item.status.已上架)
            {
                sellingQty = 0;
            }

            return sellingQty;
        }

        public Dictionary<string, List<TrackItem>> GetAllTracksItemQty(int accountID)
        {
            Dictionary<string, List<TrackItem>> results = new Dictionary<string, List<TrackItem>>();

            List<TrackItem> TrackItemList = new List<TrackItem>();
            List<Track> dbOriTracks = this._trackRepoAdapter.ReadTracks(accountID, null, null, null).ToList();

            foreach (var temp in dbOriTracks) {
                TrackItem TrackItem = new TrackItem();
                TrackItem.ItemID = temp.ItemID;
                TrackItem.CategoryID = temp.CategoryID;
                TrackItem.CategoryType = temp.CategoryType;
                TrackItem.ItemQty = temp.Qty ?? 1;
                TrackItem.CreateDate = temp.CreateDate;
                TrackItemList.Add(TrackItem);
            }

            results.Add("All", TrackItemList);

            return results;
        }

        private bool UpdateToTracksItemStatus(int? CartTypeStatus)
        {
            bool resultStatus = false;
            switch (CartTypeStatus)
            {
                case (int)TWNewEgg.Models.DBModels.TWSQLDB.Track.TrackStatus.購物車:
                    resultStatus = true;
                    break;
                case (int)TWNewEgg.Models.DBModels.TWSQLDB.Track.TrackStatus.國內購物車加價商品:
                    resultStatus = true;
                    break;
                case (int)TWNewEgg.Models.DBModels.TWSQLDB.Track.TrackStatus.海外購物車加價商品:
                    resultStatus = true;
                    break;
                case (int)TWNewEgg.Models.DBModels.TWSQLDB.Track.TrackStatus.任選館購物車加價商品:
                    resultStatus = true;
                    break;
                default:
                    resultStatus = false;
                    break;
            }
            return resultStatus;
        }

        private bool IsAddtionalItem(int? TrickStatus)
        {
            bool resultStatus = false;
            switch (TrickStatus)
            {
                case (int)TWNewEgg.Models.DBModels.TWSQLDB.Item.ShowOrderStatus.AdditionalItem:
                    resultStatus = true;
                    break;
                case (int)TWNewEgg.Models.DBModels.TWSQLDB.Item.ShowOrderStatus.AddtionalItemForCart:
                    resultStatus = true;
                    break;
                case (int)TWNewEgg.Models.DBModels.TWSQLDB.Item.ShowOrderStatus.AddtionalItemForItem:
                    resultStatus = true;
                    break;
                default:
                    resultStatus = false;
                    break;
            }
            return resultStatus;
        }

        private int TrackStatusToCartType(int TrickStatus)
        {
            int resultCartType = -1;
            switch (TrickStatus)
            {
                case (int)TWNewEgg.Models.DBModels.TWSQLDB.Track.TrackStatus.國內購物車加價商品:
                    resultCartType = (int)TWNewEgg.Models.DBModels.TWSQLDB.AdditionalItemForCart.CartTypeStatus.Domestic;
                    break;
                case (int)TWNewEgg.Models.DBModels.TWSQLDB.Track.TrackStatus.海外購物車加價商品:
                    resultCartType = (int)TWNewEgg.Models.DBModels.TWSQLDB.AdditionalItemForCart.CartTypeStatus.Internation;
                    break;
                case (int)TWNewEgg.Models.DBModels.TWSQLDB.Track.TrackStatus.任選館購物車加價商品:
                    resultCartType = (int)TWNewEgg.Models.DBModels.TWSQLDB.AdditionalItemForCart.CartTypeStatus.ChooseAny;
                    break;
                default:
                    break;
            }
            return resultCartType;
        }
    }
}
