using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using TWNewEgg.Framework.BaseController;
using TWNewEgg.Framework.AOP;
using TWNewEgg.Framework.ServiceApi;
using TWNewEgg.Framework.AutoMapper;
using TWNewEgg.Models.DomainModels.Item;
using TWNewEgg.Models.ViewModels.Item;
using TWNewEgg.Models.DomainModels.VotingActivity;
using TWNewEgg.Models.DomainModels.Property;
using TWNewEgg.ECWeb.PrivilegeFilters;
using TWNewEgg.ECWeb.Auth;
using TWNewEgg.ECWeb.Utility;

namespace TWNewEgg.ECWeb.Controllers
{
    [AllowNonSecures]
    [AllowAnonymous]
    public class VotingActivityController : BaseController
    {
        public ActionResult Index()
        {
            return View();
        }

        [AllowNonSecures]
        [AllowAnonymous]
        public ActionResult ShareGift_0807()
        {
            int numGroupId = 1; //此次活動用的GroupId
            TWNewEgg.Models.ViewModels.VotingActivity.VotingActivityGroup objGroup = null;
            List<TWNewEgg.Models.ViewModels.VotingActivity.VotingActivityItems> listVotingItems = null;
            TWNewEgg.Models.ViewModels.VotingActivity.VotingActivityItems objVotingItem = null;
            Dictionary<int, ItemInfo> dictItemInfos = null;
            Dictionary<int, List<ItemUrl>> dictItemImgUrl = null;
            Dictionary<int, string> dictItemImgUrlResult = null;
            List<int> listItemIds = null;
            List<string> listImgUrl = null;
            ItemUrl objImgUrl = null;
            DateTime dateNow = DateTime.Now;
            string strServiceName= "VotingActivityService";
            Dictionary<int, object> dictResult = null;

            objGroup = Processor.Request<TWNewEgg.Models.ViewModels.VotingActivity.VotingActivityGroup, TWNewEgg.Models.DomainModels.VotingActivity.VotingActivityGroup>(strServiceName, "GetVotingGroupById", numGroupId).results;

            //檢查時間是否有效
            if (objGroup != null && objGroup.OnlineStatus == 1 && objGroup.DisplayStartDate <= dateNow && objGroup.DisplayEndDate >= dateNow)
            {
                listVotingItems = Processor.Request<List<TWNewEgg.Models.ViewModels.VotingActivity.VotingActivityItems>, List<TWNewEgg.Models.DomainModels.VotingActivity.VotingActivityItems>>(strServiceName, "GetActiveVotingItemsByGroupId", numGroupId).results;
                if (listVotingItems != null && listVotingItems.Count > 0)
                {
                    listItemIds = listVotingItems.Select(x => x.ItemId).ToList();

                    //取得商品資訊
                    dictItemInfos = Processor.Request<Dictionary<int, ItemInfo>, Dictionary<int, ItemInfo>>("ItemInfoService", "GetItemInfoList", listItemIds).results;

                    //取得商品圖片資訊
                    dictItemImgUrl = Processor.Request<Dictionary<int, List<ItemUrl>>, Dictionary<int, List<ImageUrlReferenceDM>>>("ItemImageUrlService", "GetItemImagePath", listItemIds).results;
                    if (dictItemImgUrl != null)
                    {
                        dictItemImgUrlResult = new Dictionary<int, string>();

                        listImgUrl = new List<string>();
                        foreach (int numSubItemId in listItemIds)
                        {
                            objImgUrl = ((List<ItemUrl>) dictItemImgUrl[numSubItemId]).Where(x => x.Size == 300 || x.Size == 640).OrderBy(x=>x.Size).FirstOrDefault();
                            if (objImgUrl != null && objImgUrl.ImageUrl.IndexOf("newegg.com/") >= 0)
                            {
                                dictItemImgUrlResult.Add(numSubItemId, objImgUrl.ImageUrl);
                            }
                            else if (objImgUrl != null)
                            {
                                dictItemImgUrlResult.Add(numSubItemId, ImageUtility.GetImagePath(objImgUrl.ImageUrl));
                            }
                        }
                    }
                    

                    //整理全部的資料
                    dictResult = new Dictionary<int, object>();
                    dictResult.Add(0, objGroup);
                    dictResult.Add(1, listVotingItems);
                    dictResult.Add(2, dictItemInfos);
                    dictResult.Add(3, dictItemImgUrlResult);
                }
            }


            return View(dictResult);
        }


        [AllowNonSecures]
        [AllowAnonymous]
        [ActionName("20151216_brandCombi")]
        public ActionResult voting20151216()
        {
            int numGroupId = 2; //此次活動用的GroupId
            TWNewEgg.Models.ViewModels.VotingActivity.VotingActivityRec objTodayVotingRec = null;
            TWNewEgg.Models.ViewModels.VotingActivity.VotingActivityGroup objGroup = null;
            List<TWNewEgg.Models.ViewModels.VotingActivity.VotingActivityItems> listVotingItems = null;
            DateTime dateNow = DateTime.Now;
            string strServiceName = "VotingActivityService";
            Dictionary<int, object> dictResult = null;
            string strAccountId = "";
            string strAccountSource = "";
            string strAccountEmail = "";
            bool boolCanVote = true;


            objGroup = Processor.Request<TWNewEgg.Models.ViewModels.VotingActivity.VotingActivityGroup, TWNewEgg.Models.DomainModels.VotingActivity.VotingActivityGroup>(strServiceName, "GetVotingGroupById", numGroupId).results;

            //檢查時間是否有效
            if (objGroup != null && objGroup.OnlineStatus == 1 && objGroup.DisplayStartDate <= dateNow && objGroup.DisplayEndDate >= dateNow)
            {
                listVotingItems = Processor.Request<List<TWNewEgg.Models.ViewModels.VotingActivity.VotingActivityItems>, List<TWNewEgg.Models.DomainModels.VotingActivity.VotingActivityItems>>(strServiceName, "GetActiveVotingItemsByGroupId", numGroupId).results;
                if (listVotingItems != null && listVotingItems.Count > 0)
                {
                    //整理全部的資料
                    dictResult = new Dictionary<int, object>();
                    dictResult.Add(0, objGroup);
                    dictResult.Add(1, listVotingItems);

                    //檢查使用者登入狀態, 若有登入即要判斷是否可投票, 若無登入則預設可投票(為顯示用)
                    if (NEUser.ID > 0 && objGroup != null)
                    {
                        //從登入者取得資訊
                        strAccountSource = TWNewEgg.Models.ViewModels.VotingActivity.VotingActivityGroup.RestrictAccountOption.Newegg.ToString();
                        strAccountId = NEUser.ID.ToString();
                        strAccountEmail = NEUser.Email;

                        //取得該User當日投該Group的所有記錄
                        objTodayVotingRec = Processor.Request<TWNewEgg.Models.ViewModels.VotingActivity.VotingActivityRec, TWNewEgg.Models.DomainModels.VotingActivity.VotingActivityRec>("VotingActivityService", "GetVotingRecByGroupIdAndAccountAndDate", numGroupId, strAccountId, strAccountSource, String.Format("{0:yyyy/MM/dd}", DateTime.Now)).results;
                        //投票限制每日一票, 已投過
                        if (objTodayVotingRec != null &&  objGroup.RestrictType == (int)TWNewEgg.Models.ViewModels.VotingActivity.VotingActivityGroup.RestrictTypeOption.EveryDay)
                        {
                            boolCanVote = false;
                        }
                    }

                    //是否可投票
                    dictResult.Add(2, boolCanVote);
                }
            }
            else
            {
                boolCanVote = false;
            }

            

            return View("voting20151216", dictResult);
        }
    }
}