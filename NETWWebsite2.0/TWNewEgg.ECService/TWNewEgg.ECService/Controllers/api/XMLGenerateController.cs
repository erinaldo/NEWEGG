using Autofac;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using TWNewEgg.Framework.Autofac;
using TWNewEgg.CacheGenerateServices.Interface;

namespace TWNewEgg.ECService.Controllers.api
{
    public class XMLGenerateController : ApiController
    {
        // GET api/xmlgenerate
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/xmlgenerate/5
        public string Get(string actionType, string jsonValue)
        {
            //var results = ChooseFunction(actionType, jsonValue);
            string results = string.Empty;
            using (var scope = AutofacConfig.Container.BeginLifetimeScope())
            {
                var generateXML = scope.Resolve<IXMLGenerate>();
                results = generateXML.ChooseFunction(actionType, jsonValue);
            }
            return results;
        }

        // POST api/xmlgenerate
        public void Post([FromBody]string value)
        {
        }

        // PUT api/xmlgenerate/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/xmlgenerate/5
        public void Delete(int id)
        {
        }
    }
}
