using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Cors;
using TWNewEgg.ECWeb.PrivilegeFilters.Api;
using TWNewEgg.ECWeb_Mobile.Auth;
using TWNewEgg.Framework.ServiceApi;
using TWNewEgg.Models.DomainModels.Cart;
using TWNewEgg.Models.ViewModels.Cart;

namespace TWNewEgg.ECWeb_Mobile.Controllers.Api
{
#if DEBUG
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    [AllowNonSecures]
    //[AllowAnonymous]
#endif
    public class CartController : ApiController
    {
        private const string DOMESTICCART = "domesCart";
        private const string INTERNATIONCART = "interCart";
        private const string CHOOSECART = "chooseCart";
        // GET api/cart/5
        public Dictionary<string, int> Get(string cType)
        {
            Dictionary<string, int> result = new Dictionary<string, int>();
            int domesticNumber = 0;
            int internationNumber = 0;
            int chooseNumber = 0;
            int accID = NEUser.ID;
            var results = Processor.Request<List<ShoppingCart_View>, List<ShoppingCartDM>>("ShoppingCartService", "GetCartAllList", accID);
            if (string.IsNullOrEmpty(results.error))
            {
                var domesticCart = results.results.Where(x => x.ID == (int)ShoppingCartDM.CartType.新蛋購物車).FirstOrDefault().CartItemClassList.FirstOrDefault();
                domesticNumber = domesticCart != null ? domesticCart.CartItemList.Count : 0;
                var internationCart = results.results.Where(x => x.ID == (int)ShoppingCartDM.CartType.海外購物車).FirstOrDefault().CartItemClassList.FirstOrDefault();
                internationNumber = internationCart != null ? internationCart.CartItemList.Count : 0;
                var chooseCart = results.results.Where(x => x.ID == (int)ShoppingCartDM.CartType.任選館購物車).FirstOrDefault().CartItemClassList.FirstOrDefault();
                chooseNumber = chooseCart != null ? chooseCart.CartItemList.Count : 0;
            }
            result.Add(DOMESTICCART, domesticNumber);
            result.Add(INTERNATIONCART, internationNumber);
            result.Add(CHOOSECART, chooseNumber);
            return result;
        }

        // POST api/cart
        public void Post([FromBody]string value)
        {
        }

        // PUT api/cart/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/cart/5
        public void Delete(int id)
        {
        }
    }
}
