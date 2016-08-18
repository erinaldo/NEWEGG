using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Cors;
using TWNewEgg.ECWeb.PrivilegeFilters.Api;
using TWNewEgg.ECWeb_Mobile.Utility;
using TWNewEgg.Framework.ServiceApi;

namespace TWNewEgg.ECWeb_Mobile.Controllers.Api
{
#if DEBUG
    [EnableCors(origins: "*", headers: "*", methods: "*")]
#endif
    [AllowNonSecures]
    [AllowAnonymous]
    public class FlashController : ApiController
    {

        // GET api/flash/5
        public List<TWNewEgg.Models.ViewModels.GroupBuy.GroupBuyViewInfo> Get(int GroupBuyID = 0, int pageSize = 3, int pageNumber = 1)
        {
            List<TWNewEgg.Models.ViewModels.GroupBuy.GroupBuyViewInfo> infoList = new List<TWNewEgg.Models.ViewModels.GroupBuy.GroupBuyViewInfo>();
            try
            {
                //GroupBuyService.Service.GroupBuyService gpbService = new GroupBuyService.Service.GroupBuyService();
                TWNewEgg.Models.DomainModels.GroupBuy.GroupBuyQueryCondition condition = new TWNewEgg.Models.DomainModels.GroupBuy.GroupBuyQueryCondition();
                condition.PageSize = pageSize;
                condition.PageNumber = pageNumber;
                condition.GroupBuyID = GroupBuyID;
                //infoList = gpbService.QueryViewInfo(condition);
                var result = Processor.Request<List<TWNewEgg.Models.ViewModels.GroupBuy.GroupBuyViewInfo>, List<TWNewEgg.Models.DomainModels.GroupBuy.GroupBuyViewInfo>>("GroupBuyService", "QueryViewInfo", condition, (condition.PageSize.ToString() + "_" + condition.PageNumber.ToString()));
                if (string.IsNullOrEmpty(result.error))
                {
                    infoList = result.results;
                    foreach (var singleInfo in infoList)
                    {
                        singleInfo.ImgUrl = ImageUtility.GetImagePath(singleInfo.ImgUrl);
                    }
                }
            }
            catch (Exception e)
            { }

            return infoList;
        }

        // POST api/flash
        public void Post([FromBody]string value)
        {
        }

        // PUT api/flash/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/flash/5
        public void Delete(int id)
        {
        }
    }
}
