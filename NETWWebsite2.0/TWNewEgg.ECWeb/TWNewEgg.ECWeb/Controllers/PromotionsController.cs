//using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TWNewEgg.ECWeb.PrivilegeFilters;
using TWNewEgg.Framework.ServiceApi;

namespace TWNewEgg.ECWeb.Controllers
{
    [AllowNonSecures]
    [AllowAnonymous]
    public class PromotionsController : Controller
    {
        //
        // GET: /Promotions/

        public ActionResult Card()
        {
            return View();
        }

        public ActionResult CardInstallment()
        {
            return View();
        }
        /// <summary>
        /// 顯示信用卡紅利點數折抵
        /// </summary>
        /// <returns></returns>
        public ActionResult CardRedeem()
        {
            var getBankInfo = Processor.Request<TWNewEgg.Models.DomainModels.Message.ResponseMessage<List<TWNewEgg.Models.ViewModels.BankBonus.BankBonus_VM>>, TWNewEgg.Models.DomainModels.Message.ResponseMessage<List<TWNewEgg.Models.DomainModels.BankBonus.BankBonus_DM>>>("BankBonusService", "GetAllEffectiveBankBonus");
            //service 發生錯誤
            if (string.IsNullOrEmpty(getBankInfo.error) == false)
            {
                return View();
            }
            //沒有任何相關的銀行信用卡紅利資訊
            if (getBankInfo.results.Data == null)
            {
                return View();
            }
            //ImageUtility.GetImagePath
            getBankInfo.results.Data.ForEach(p => {
                p.PhotoName = TWNewEgg.ECWeb.Utility.ImageUtility.GetImagePath(p.PhotoName);
                //p.DescriptionFormat = Server.HtmlEncode(p.DescriptionFormat);
            });
            return View(getBankInfo.results.Data);
        }

        public ActionResult CardMore()
        {
            return View();
        }

    }
}
