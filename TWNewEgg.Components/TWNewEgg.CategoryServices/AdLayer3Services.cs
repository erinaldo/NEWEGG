using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TWNewEgg.CategoryServices.Interface;
using TWNewEgg.CategoryRepoAdapters.Interface;
using TWNewEgg.ItemRepoAdapters.Interface;
using TWNewEgg.Models.DBModels.TWSQLDB;
using TWNewEgg.Framework.AutoMapper;
using TWNewEgg.Models.DomainModels.Category;
using TWNewEgg.Models.DomainModels.Message;
using System.IO;

namespace TWNewEgg.CategoryServices
{
    public class AdLayer3Services : IAdLayer3Services
    {
        private IAdLayer3RepoAdapter _adLayer3SerAdp;
        private IItemRepoAdapter _adItemSerAdp;
        public AdLayer3Services(IAdLayer3RepoAdapter adLayer3SerAdp, IItemRepoAdapter adItemSerAdp)
        {
            this._adLayer3SerAdp = adLayer3SerAdp;
            this._adItemSerAdp = adItemSerAdp;
        }

        public List<AdLayer3DM> GetAdLayer3(AdLayer3DM SearchCondition)
        {
            IQueryable<AdLayer3> SearchData = _adLayer3SerAdp.AdLayer3_GetAll();

            List<AdLayer3DM> ReturnSearchData = new List<AdLayer3DM>();

            if (SearchCondition.ID != null)
            {
                SearchData = SearchData.Where(x => x.ID == SearchCondition.ID);
            }
            if (SearchCondition.CategoryID != null)
            {
                SearchData = SearchData.Where(x => x.CategoryID == SearchCondition.CategoryID);
            }
            if (SearchCondition.AdType != null)
            {
                SearchData = SearchData.Where(x => x.AdType == SearchCondition.AdType.GetValueOrDefault((int)AdLayer3DM.adType.image));
            }
            if (SearchCondition.ImagePath != null)
            {
                SearchData = SearchData.Where(x => x.ImagePath == SearchCondition.ImagePath);
            }
            if (SearchCondition.ImageLink != null)
            {
                SearchData = SearchData.Where(x => x.ImageLink == SearchCondition.ImageLink);
            }
            if (SearchCondition.ShowAll != null)
            {
                SearchData = SearchData.Where(x => x.ShowAll == SearchCondition.ShowAll);
            }
            if (SearchCondition.Showorder != null)
            {
                SearchData = SearchData.Where(x => x.Showorder == SearchCondition.Showorder);
            }
            if (!string.IsNullOrEmpty(SearchCondition.UpdateUser))
            {
                SearchData = SearchData.Where(x => x.UpdateUser == SearchCondition.UpdateUser);
            }
            if (SearchCondition.UpdateDate != null)
            {
                SearchData = SearchData.Where(x => x.UpdateDate == SearchCondition.UpdateDate);
            }
            if (!string.IsNullOrEmpty(SearchCondition.CreateUser))
            {
                SearchData = SearchData.Where(x => x.CreateUser == SearchCondition.CreateUser);
            }
            if (SearchCondition.CreateDate != null)
            {
                SearchData = SearchData.Where(x => x.CreateDate == SearchCondition.CreateDate);
            }

            if ((SearchData != null) && (SearchData.Count() > 0))
            {
                SearchData = SearchData.OrderBy(x => x.Showorder);
                ReturnSearchData = ModelConverter.ConvertTo<List<AdLayer3DM>>(SearchData.ToList());
            }

            return ReturnSearchData;
        }

        public List<AdLayer3ItemDM> GetAdLayer3Item(AdLayer3ItemDM SearchCondition)
        {
            IQueryable<AdLayer3Item> SearchData = _adLayer3SerAdp.AdLayer3Item_GetAll().AsQueryable();

            List<AdLayer3ItemDM> ReturnSearchData = new List<AdLayer3ItemDM>();

            //AdLayer3廣告主單資料
            if (SearchCondition.AdLayer3ID != null)
            {
                SearchData = SearchData.Where(x => x.AdLayer3ID == SearchCondition.AdLayer3ID);
            }
            if (SearchCondition.ShowAll != null)
            {
                SearchData = SearchData.Where(x => x.ShowAll == SearchCondition.ShowAll);
            }
            if (SearchCondition.Showorder != null)
            {
                SearchData = SearchData.Where(x => x.Showorder == SearchCondition.Showorder);
            }
            if (!string.IsNullOrEmpty(SearchCondition.UpdateUser))
            {
                SearchData = SearchData.Where(x => x.UpdateUser == SearchCondition.UpdateUser);
            }

            if ((SearchData != null) && (SearchData.Count() > 0))
            {
                SearchData = SearchData.OrderBy(x => x.Showorder);
                ReturnSearchData = ModelConverter.ConvertTo<List<AdLayer3ItemDM>>(SearchData.ToList());
            }

            return ReturnSearchData;
        }

        public List<AdLayer3DM> GetAdLayer3List(AdLayer3DM SearchCondition)
        {
            List<AdLayer3DM> AdLayer3List = new List<AdLayer3DM>();
            //IQueryable<AdLayer3DM> AdLayer3Query = null;
            
            AdLayer3ItemDM AdLayer3Item = new AdLayer3ItemDM();
            AdLayer3List = GetAdLayer3(SearchCondition);

            if ((AdLayer3List != null) && (AdLayer3List.Count() > 0))
            {
                foreach (AdLayer3DM banner in AdLayer3List)
                {
                    if (banner.AdType == (int)AdLayer3DM.adType.item)
                    {
                        List<AdLayer3ItemDM> AdLayer3ItemList = new List<AdLayer3ItemDM>();
                        AdLayer3Item.AdLayer3ID = banner.ID;
                        AdLayer3Item.ShowAll = SearchCondition.ShowAll;
                        AdLayer3ItemList = GetAdLayer3Item(AdLayer3Item);
                        banner.ItemList = AdLayer3ItemList;
                    }
                    else
                        banner.ItemList = null;
                }
            }

            return AdLayer3List;
        }

        public ResponseMessage<AdLayer3DM> UpdateAdLayer3Data(AdLayer3DM NewData)
        {
            DateTime dt = DateTime.UtcNow.AddHours(8);
            dt.GetDateTimeFormats('r')[0].ToString();
            Convert.ToDateTime(dt);

            AdLayer3DM ReData = new AdLayer3DM();
            
            TWNewEgg.Models.DBModels.TWSQLDB.AdLayer3 Info = new TWNewEgg.Models.DBModels.TWSQLDB.AdLayer3();
            TWNewEgg.Models.DBModels.TWSQLDB.AdLayer3 OldInfo = new TWNewEgg.Models.DBModels.TWSQLDB.AdLayer3();
            TWNewEgg.Models.DBModels.TWSQLDB.AdLayer3 ReInfo = new TWNewEgg.Models.DBModels.TWSQLDB.AdLayer3();
            ResponseMessage<AdLayer3DM> Result = new ResponseMessage<AdLayer3DM>();
            List<AdLayer3Item> ReItemInfo = new List<AdLayer3Item>();
            List<AdLayer3ItemDM> ReItemData = new List<AdLayer3ItemDM>();
            List<int> avaItemList = new List<int>();
            List<int> bannerItemList = new List<int>();
            List<int> noExitItem = new List<int>();

            try
            {
                if (NewData.ID.GetValueOrDefault() > 0)
                {
                    OldInfo = _adLayer3SerAdp.AdLayer3_GetAll().Where(x => x.ID == NewData.ID).SingleOrDefault();

                    //修改清單中更新時間小於DB更新時間，表示有人更新，不可修改此筆資料
                    if ((OldInfo.UpdateDate != null) && (NewData.UpdateDate < OldInfo.UpdateDate))
                    {
                        Result.IsSuccess = false;
                        Result.Message = string.Format("CategoryID：{0}，此筆資料，{1}此人於{2}同時更新，無法修改，請重新進入修改畫面!!\r\n", NewData.CategoryID, Info.UpdateUser, Info.UpdateDate);
                        throw new Exception("次分類頁banner更新失敗!!");
                        //return Result;
                    }
                }


                bannerItemList = NewData.ItemList.Select(x => x.ItemID.GetValueOrDefault()).ToList();
                avaItemList = GetAvailableAndVisibleItemID(bannerItemList);
                //判斷設定的item是否為有效的item
                if (bannerItemList.Count() != avaItemList.Count())
                {
                    string strNoExit = "";
                    noExitItem = (from old in bannerItemList
                                  where !(from pro in avaItemList
                                          select pro)
                                          .Contains(old)
                                  select old).ToList();

                    for (int i = 0; i < noExitItem.Count(); i++)
                    {
                        strNoExit = strNoExit + noExitItem[i].ToString();
                        if ((i + 1) < noExitItem.Count())
                            strNoExit = strNoExit + ", ";
                    }

                    Result.IsSuccess = false;
                    Result.Message = string.Format("CategoryID：{0}，櫥窗名稱：{1}，有無效的Item：{2}!!", NewData.CategoryID, NewData.Title, strNoExit);
                    throw new Exception(Result.Message);
                }


                Info = AdLayer3Data(NewData);
                ReInfo = _adLayer3SerAdp.UpdateAdLayer3Data(Info);
                if (ReInfo != null)
                {
                    ReData = ModelConverter.ConvertTo<AdLayer3DM>(ReInfo);
                    if (Info.AdType == (int)AdLayer3DM.adType.item)
                    {
                        if (!string.IsNullOrEmpty(ReInfo.ID.ToString()) && (ReInfo.ID > 0) && (NewData.ItemList != null))
                        {
                            
                            NewData.ID = ReInfo.ID;
                            ReItemInfo = UpdateAdLayer3ItemData(NewData);

                            ReItemData = ModelConverter.ConvertTo<List<AdLayer3ItemDM>>(ReItemInfo);
                            ReData.ItemList = ReItemData;
                        }
                    }
                    else
                    {
                        ReData.ItemList = null;
                    }

                    Result.Data = ReData;
                    Result.IsSuccess = true;
                    Result.Message = string.Format("categoryID:{0}{1}，更新成功^^", NewData.ID, NewData.Title);
                }
                else
                {
                    Result.Data = null;
                    Result.IsSuccess = false;
                    Result.Message = string.Format("categoryID:{0}{1}，更新失敗!!", NewData.ID, NewData.Title);
                    throw new Exception("次分類頁banner更新失敗!!");
                }
            }
            catch (Exception ex)
            {
                //throw ex;
                Result.Message = ex.Message;
                return Result;
            }
            
            return Result;
        }

        private AdLayer3 AdLayer3Data(AdLayer3DM NewData)
        {
            DateTime dt = DateTime.UtcNow.AddHours(8);
            dt.GetDateTimeFormats('r')[0].ToString();
            Convert.ToDateTime(dt);

            string strfileName = System.Configuration.ConfigurationManager.AppSettings["HttpCategoryImagePath"];

            AdLayer3DM ReData = new AdLayer3DM();
            TWNewEgg.Models.DBModels.TWSQLDB.AdLayer3 Info = new TWNewEgg.Models.DBModels.TWSQLDB.AdLayer3();
            ResponseMessage<AdLayer3DM> Result = new ResponseMessage<AdLayer3DM>();

            Info = _adLayer3SerAdp.AdLayer3_GetAll().Where(x => x.ID == NewData.ID).SingleOrDefault();

            if (Info == null)
            {
                Info = new TWNewEgg.Models.DBModels.TWSQLDB.AdLayer3();
                Info.CreateDate = dt;
                Info.CreateUser = NewData.UpdateUser;
            }
            else
            {
                Info.UpdateDate = dt;
            }

            ModelConverter.ConvertTo<AdLayer3DM, AdLayer3>(NewData, Info);

            if (!string.IsNullOrEmpty(NewData.ImagePath) && (NewData.AdType == (int)AdLayer3DM.adType.image))
            {
                //Info.ImagePath = strfileName + NewData.CategoryID + "/" + NewData.Title + ".jpg";
                //圖檔更改檔名存置資料庫
                Info.ImagePath = string.Format("{0}{1}/{2}{3}", strfileName, NewData.CategoryID, NewData.Title, Path.GetExtension(NewData.ImagePath));  //副檔名改為user傳進的副檔名
            }

            return Info;
        }

        private List<AdLayer3Item> UpdateAdLayer3ItemData(AdLayer3DM NewData)
        {
            DateTime dt = DateTime.UtcNow.AddHours(8);
            dt.GetDateTimeFormats('r')[0].ToString();
            Convert.ToDateTime(dt);

            List<AdLayer3Item> ReData = new List<AdLayer3Item>();

            if (_adLayer3SerAdp.AdLayer3Item_GetAll().Where(x => x.AdLayer3ID == NewData.ID).Any())
            {
                _adLayer3SerAdp.DeleteAdLayer3Items(NewData.ID.GetValueOrDefault(0));
            }

            foreach (AdLayer3ItemDM item in NewData.ItemList)
            {
                item.AdLayer3ID = NewData.ID;
                AdLayer3Item iteminfo = new AdLayer3Item();
                ModelConverter.ConvertTo<AdLayer3ItemDM, AdLayer3Item>(item, iteminfo);

                iteminfo = _adLayer3SerAdp.UpdateAdLayer3ItemData(iteminfo);
                ReData.Add(iteminfo);
            }

            return ReData;
        }

        public List<ResponseMessage<AdLayer3DM>> UpdateAdLayer3List(List<AdLayer3DM> NewDataList)
        {
            List<ResponseMessage<AdLayer3DM>> Result = new List<ResponseMessage<AdLayer3DM>>();

            foreach (AdLayer3DM NewData in NewDataList)
            {
                ResponseMessage<AdLayer3DM> ReData = new ResponseMessage<AdLayer3DM>();

                ReData = UpdateAdLayer3Data(NewData);

                Result.Add(ReData);
            }

            return Result;
        }

        public List<int> GetAvailableAndVisibleItemID(List<int> itemIDList)
        {
            List<int> Result = new List<int>();

            Result = _adItemSerAdp.GetAvailableAndVisibleItemList(itemIDList).Select(x => x.ID).ToList();

            return Result;
        }

    }
}
