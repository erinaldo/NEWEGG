using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Cors;
using TWNewEgg.ECWeb.PrivilegeFilters.Api;
using TWNewEgg.Framework.ServiceApi;
using TWNewEgg.Models.ViewModels.GreetingWords;

namespace TWNewEgg.ECWeb_Mobile.Controllers.Api
{
#if DEBUG
    [EnableCors(origins: "*", headers: "*", methods: "*")]
#endif
    public class GreetingWordsController : ApiController
    {
        [AllowNonSecures]
        [AllowAnonymous]
        [HttpGet]
        // GET api/greetingwords
        public HomeGreetingWordsVM Get(string type)
        {
            HomeGreetingWordsVM results = new HomeGreetingWordsVM();
            if (string.IsNullOrEmpty(type))
            {
                return results;
            }
            DateTime dateTimeNow = DateTime.UtcNow.AddHours(8);
            switch (type)
            {
                case "words":
                    results = GetGreetingWords(dateTimeNow);
                    break;
                case "cards":
                    results = GetGreetingCards(dateTimeNow);
                    break;
                default:
                    break;
            }
            return results;
        }


        // POST api/greetingwords
        public void Post([FromBody]string value)
        {
        }

        // PUT api/greetingwords/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/greetingwords/5
        public void Delete(int id)
        {
        }

        private HomeGreetingWordsVM GetGreetingWords(DateTime dateTimeNow)
        {
            HomeGreetingWordsVM results = new HomeGreetingWordsVM();
            var result = Processor.Request<List<TWNewEgg.Models.ViewModels.GreetingWords.HomeGreetingWordsVM>, List<TWNewEgg.Models.DomainModels.GreetingWords.HomeGreetingWordsDM>>("HomeGreetingWordsService", "GetShow", dateTimeNow);
            if (result.results != null)
            {
                if (DateTime.Compare(dateTimeNow, result.results[0].StartDate) >= 0 && DateTime.Compare(dateTimeNow, result.results[0].EndDate) < 0)
                {
                    //greetingWords = "Hi, {email}, <br>" + result.results[0].Description;
                    results = result.results[0];
                }
            }
            return results;
        }
        private HomeGreetingWordsVM GetGreetingCards(DateTime dateTimeNow)
        {
            HomeGreetingWordsVM results = new HomeGreetingWordsVM();
            var result = Processor.Request<List<TWNewEgg.Models.ViewModels.GreetingWords.HomeGreetingWordsVM>
                , List<TWNewEgg.Models.DomainModels.GreetingWords.HomeGreetingWordsDM>>("HolidayGreetingWordsService", "GetShow", dateTimeNow);

            if (result.results != null)
            {
                if (DateTime.Compare(dateTimeNow, result.results[0].StartDate) >= 0 && DateTime.Compare(dateTimeNow, result.results[0].EndDate) < 0)
                {
                    //greetingWords = result.results[0].ImageUrl;
                    results = result.results[0];
                }
            }
            return results;
        }
    }
}
