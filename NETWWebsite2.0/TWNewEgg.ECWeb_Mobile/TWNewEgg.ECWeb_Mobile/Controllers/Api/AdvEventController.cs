using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Cors;
using TWNewEgg.ECWeb.PrivilegeFilters.Api;
using TWNewEgg.Framework.ServiceApi;

namespace TWNewEgg.ECWeb_Mobile.Controllers.Api
{
#if DEBUG
    [EnableCors(origins: "*", headers: "*", methods: "*")]
#endif
    [AllowNonSecures]
    [AllowAnonymous]
    public class AdvEventController : ApiController
    {

        public class AdvEventCollection
        {
            public TWNewEgg.Models.ViewModels.Advertising.AdvEventType advEventType { get; set; }
            public List<TWNewEgg.Models.ViewModels.Advertising.AdvEventDisplay> advEventList { get; set; }

        }

        // GET api/advevent/5
        public Dictionary<int, AdvEventCollection> Get(int advEventID)
        {
            List<TWNewEgg.Models.ViewModels.Advertising.AdvEventType> listAdvEventType = null;
            List<TWNewEgg.Models.ViewModels.Advertising.AdvEventDisplay> listAdvEventDisplay = null;
            Dictionary<int, AdvEventCollection> objDictResult = new Dictionary<int, AdvEventCollection>();
            int numSort = 0;

            //根據AdvTypeCode取得所有AdvType的列表
            listAdvEventType = Processor.Request<List<TWNewEgg.Models.ViewModels.Advertising.AdvEventType>, List<TWNewEgg.Models.DomainModels.Advertising.AdvEventType>>("Service.AdvEventTypeReposity", "GetActiveAdvEventTypeListByAdvType", advEventID).results;
            if (listAdvEventType != null)
            {
                //將AdvEventType以Country欄位作排序
                listAdvEventType = listAdvEventType.OrderBy(x => x.Country).ToList();

                //取得每一個AdvEventType下的AdvEvent, 並組好放到Dictionary
                //objDictResult = new Dictionary<int, Dictionary<TWNewEgg.Models.ViewModels.Advertising.AdvEventType, List<TWNewEgg.Models.ViewModels.Advertising.AdvEventDisplay>>>();
                numSort = 1;
                foreach (TWNewEgg.Models.ViewModels.Advertising.AdvEventType objAdvType in listAdvEventType)
                {
                    AdvEventCollection objDictItem = new AdvEventCollection();
                    //根據AdvTypeCode取得所有AdvType的列表, 並以Country欄位作為排序
                    listAdvEventDisplay = Processor.Request<List<TWNewEgg.Models.ViewModels.Advertising.AdvEventDisplay>, List<TWNewEgg.Models.DomainModels.Advertising.AdvEventDisplay>>("Service.AdvEventItemService", "GetActiveAdvEventDisplayListByAdvEventTypeId", objAdvType.ID).results;
                    //若此AdvEventType下有廣告,再存入Dictionary
                    if (listAdvEventDisplay != null)
                    {
                        //objDictItem = new Dictionary<TWNewEgg.Models.ViewModels.Advertising.AdvEventType, List<TWNewEgg.Models.ViewModels.Advertising.AdvEventDisplay>>();
                        objDictItem.advEventType = objAdvType;
                        objDictItem.advEventList = listAdvEventDisplay;
                        objDictResult.Add(numSort, objDictItem);
                        numSort++;
                    }
                }
            }
            return objDictResult;
        }

        // POST api/advevent
        public void Post([FromBody]string value)
        {
        }

        // PUT api/advevent/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/advevent/5
        public void Delete(int id)
        {
        }
    }
}
