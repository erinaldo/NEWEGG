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
    public class CategoryController : ApiController
    {
        // GET api/category
        public List<TWNewEgg.Models.ViewModels.Category.Category_TreeItem> Get(int cID, int lID)
        {
            List<TWNewEgg.Models.ViewModels.Category.Category_TreeItem> result = new List<Models.ViewModels.Category.Category_TreeItem>();

            //result = Processor.Request<List<TWNewEgg.Models.ViewModels.Category.Category_TreeItem>, List<TWNewEgg.Models.DomainModels.Category.Category_TreeItem>>("Service.BaseService", "XML2Tree", "XML\\Category_Menu_ALL.XML").results;

            var results = Processor.Request<Dictionary<string, List<TWNewEgg.Models.ViewModels.Category.Category_TreeItem>>, Dictionary<string, List<TWNewEgg.Models.DomainModels.Category.Category_TreeItem>>>("Service.BaseService", "GetCategory", cID, lID, null, null, null, null);

            if (string.IsNullOrEmpty(results.error))
            {
                if (results.results != null)
                {
                    result = results.results.FirstOrDefault().Value;
                }
            }


            return result;
        }

        // POST api/category
        public void Post([FromBody]string value)
        {
        }

        // PUT api/category/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/category/5
        public void Delete(int id)
        {
        }
    }
}
