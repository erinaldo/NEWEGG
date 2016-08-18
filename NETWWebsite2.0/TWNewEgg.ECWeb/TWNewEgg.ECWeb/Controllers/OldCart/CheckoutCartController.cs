using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Cors;
using System.Web.Http.Filters;
using TWNewEgg.Website.ECWeb.Models;
using TWNewEgg.DB.TWSQLDB.Models;
using TWNewEgg.DB.TWSQLDB.Models.ExtModels;
using TWNewEgg.DB.TWBACKENDDB.Models;
using TWNewEgg.Website.ECWeb.Service;

namespace TWNewEgg.ECWeb.Controllers.Api
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class CheckoutCartController : ApiController
    {
        private ICarts repository = new CartsRepository();
        private string loginStatus = "1"; //0 = false, 1 = true
        //private OutputMessage OMessage = new OutputMessage();
        /**
         * Op number
         * 1  =  get buynow table
         * 3  =  get buynext table
         * 5  =  get tracknext table
         * 7  =  get oversea buynow table
         * 8  =  get oversea buynext table
         * 9  =  get oversea tracknext table
         * 21 =  add to Shopping cart from product page
         * 23 =  add to Shopping cart from Api
         * 31 =  del track
         * 33 =  del Trackitem
         * 35 =  del Trackitem's attrid
         */
        // GET api/checkoutcart
        //Do Nothing
        public IEnumerable<string> Get()
        {
            return new string[] { "Empty" };
        }


        // GET all itemlistids with itemid that user choose, (api/CheckoutCart?accid=...)
        //[RequireHttps]
        public Dictionary<int, List<int>> Get(string accid)
        {

            string[] plainText = repository.Decoder(accid, false); //Decoder accid, login and datetime  status

            if (plainText.Length < 3) //Make sure there had three variable, accid, login and datetime  status.
                return null;

            if (plainText[1] == loginStatus) //if loginstatus equal "True" then enter
            {
                int accountId;
                if (accid != null)
                {
                    bool check = Int32.TryParse(plainText[0], out accountId); //Get accid 
                    if (check == true && plainText[0] != null)
                    {
                        if (!repository.SetTrackAll(accountId, plainText[2])) //Set AccountID into this repository class
                        {
                            return null;
                        }
                        return repository.GetTrackItemAll(); //GET all itemlistids with itemid that user choose
                    }

                }
                return null;
            }
            else
            {
                return null;
            }
        }

        // GET all final shopping cart items from Stored procedure,  (api/CheckoutCart?accid=...&sort=...)
        //[RequireHttps]
        public IEnumerable<ShoppingCartItems> Get(string accid, int? sort)
        {
            string[] plainText = repository.Decoder(accid, false);//Decoder accid, login and datetime  status

            if (plainText.Length < 3)//Make sure there had three variable, accid, login and datetime  status.
                return null;

            if (plainText[1] == loginStatus) //if loginstatus equal "True" then enter
            {


                int accountId, sortCode;
                if (accid != null)
                {
                    bool check = Int32.TryParse(plainText[0], out accountId); //Get accid 
                    if (check == true && plainText[0] != null)
                    {
                        if (!repository.SetTrackAll(accountId, plainText[2]))//Set AccountID into this repository class
                        {
                            return null;
                        }
                        if (sort == null)
                        {
                            sortCode = 0;
                        }
                        else
                        {
                            sortCode = sort.Value; //set sort code
                        }
                        return repository.GetShoppingCart(sortCode, "False"); //GET all final shopping cart items from Stored procedure
                    }
                }
                return null;
            }
            else
            {
                return null;
            }
        }

        // GET all cart items from Stored procedure,  api/CheckoutCart?accit=...&op=...&sort=...
        //[RequireHttps]
        public IEnumerable<CartItems> Get(string accid, int? op, int? sort)
        {
            //string[] plainText = repository.Decoder(accid, false);//Decoder accid ,login and login datetime status

            //if (plainText.Length < 3)//Make sure there had three variable, accid and login status.
            //    return null;

            //if (plainText[1] == loginStatus) //if loginstatus equal "True" then enter
            //{
                int accountId, opCode, sortCode;
                if (op != null && accid != null)
                {
                    opCode = op.Value;
                    bool check = Int32.TryParse(accid, out accountId); //Get accid 
                    if (check == true && accid != null)
                    {
                        repository.SetTrackAll(accountId, DateTime.Now.ToString());
                        //if (!repository.SetTrackAll(accountId, plainText[2]))//Set AccountID into this repository class
                        //{
                        //    return null;
                        //}
                        if (sort == null)
                        {
                            sortCode = 0;
                        }
                        else
                        {
                            sortCode = sort.Value; //set sort code
                        }
                        return repository.GetTrackAll(opCode, sortCode);// GET all cart items from Stored procedure
                    }
                }
                return null;
            //}
            //else
            //{
            //    return null;
            //}
        }



        //1. Get total Shopping cart's items number. variable "postData" set "GetCartNumber"
        //2. Get Shopping cart's all items' shipping costs and group by sellerID
        //[RequireHttps]
        public Dictionary<string, decimal> Get(string accid, string postData)
        {
            var Request = System.Web.HttpContext.Current.Request;

            string[] plainText;
            if (postData == "GetCartNumber")
            {
                //if (!Request.IsSecureConnection)
                //{
                //    accid = accid.Substring(1, accid.Length - 2);
                //    plainText = repository.DecoderIE(accid, true);//Decoder accid, login and datetime status
                //}
                //else
                //{
                if (accid[0] == 'i' && accid[accid.Length - 1] == 'e')
                {
                    accid = accid.Substring(1, accid.Length - 2);
                    plainText = repository.DecoderIE(accid, true);//Decoder accid, login and datetime  status
                }
                else
                {
                    plainText = repository.Decoder(accid, true);//Decoder accid, login and datetime  status
                }
                //}
            }
            else
            {
                plainText = repository.Decoder(accid, false);//Don't decoder accid, login and datetime  status
            }

            if (plainText.Length < 3)//Make sure there had three variable, accid, login and datetime  status.
                return null;

            if (plainText[1] == loginStatus)//if loginstatus equal "True" then enter
            {

                int accountId;
                bool check = Int32.TryParse(plainText[0], out accountId);//Get accid 
                if (check == true && plainText[0] != null)
                {
                    if (!repository.SetTrackAll(accountId, plainText[2]))//Set AccountID into this repository class
                    {
                        return null;
                    }
                }
                else
                {
                    return null;
                }
                if (postData == "GetCartNumber")
                {
                    return repository.GetCartNumber("0"); //Get total Shopping cart's items number.
                }
                else
                {
                    //if (Request.IsSecureConnection)
                    //{
                    return repository.ShippingCosts(postData, "SellerID"); //Get Shopping cart's all items' shipping costs and group by sellerID.
                    //}
                    //else
                    //{
                    //    return null;
                    //}
                }
            }
            else
            {
                return null;
            }
        }



        // POST api/CheckoutCart
        //Add item or itemlist into DB 
        //[RequireHttps]
        public string Post([FromBody]TrackCart value)
        {
            var Request = System.Web.HttpContext.Current.Request;
            string[] plainText;
            //if (!Request.IsSecureConnection)
            //{
            //    value.accID = value.accID.Substring(1, value.accID.Length - 2);
            //    plainText = repository.DecoderIE(value.accID, true);//Decoder accid, login and datetime  status
            //}
            //else
            //{
            if (value.accID[0] == 'i' && value.accID[value.accID.Length - 1] == 'e')
            {
                value.accID = value.accID.Substring(1, value.accID.Length - 2);
                plainText = repository.DecoderIE(value.accID, true);//Decoder accid, login and datetime  status
            }
            else
            {
                plainText = repository.Decoder(value.accID, true);//Decoder accid, login and datetime  status
            }
            //}
            if (plainText.Length < 3)//Make sure there had three variable, accid, login and datetime  status.
                return OutputMessage.cookiesError;

            if (plainText[1] == loginStatus)//if loginstatus equal "True" then enter
            {


                int accountId;
                bool check = Int32.TryParse(plainText[0], out accountId);//Get accid 
                if (check == true && plainText[0] != null)
                {
                    if (!repository.SetTrackAll(accountId, plainText[2]))//Set AccountID into this repository class
                    {
                        return OutputMessage.accountError;
                    }
                }
                else
                {
                    return OutputMessage.accountNull;
                }
                if (value.opCode.ToString() == "21")// If opCode is 21, then add item
                {
                    return repository.AddTrack(value.itemID, value.itemlistID, value.trackStatus); //send itemid itemlistid and trackstatus
                    //return false;
                }
                else if (value.opCode.ToString() == "23")// If opCode is 23, then add itemlist
                {
                    return repository.AddTrackItem(value.itemID[0], value.itemlistID[0]);//send itemid itemlistid
                }
                else { return OutputMessage.wrongOP; }
            }
            else
            {
                return OutputMessage.loginError;
            }
        }


        // PUT api/CheckoutCart
        //Update item to shopping cart or wish list 
        //[RequireHttps]
        public string Put([FromBody]TrackCart value)
        {

            string[] plainText = repository.Decoder(value.accID, true);//Decoder accid, login and datetime  status

            if (plainText.Length < 3)//Make sure there had three variable, accid, login and datetime  status.
                return OutputMessage.cookiesError;

            if (plainText[1] == loginStatus)//if loginstatus equal "True" then enter
            {

                int accountId;
                bool check = Int32.TryParse(plainText[0], out accountId);//Get accid 
                if (check == true && plainText[0] != null)
                {
                    if (!repository.SetTrackAll(accountId, plainText[2]))//Set AccountID into this repository class
                    {
                        return OutputMessage.accountError;
                    }
                }
                else
                {
                    return OutputMessage.accountNull;
                }
                return repository.UpdateTrack(value.itemID, value.trackStatus, true); //send itemID and trackStatus
            }
            else
            {
                return OutputMessage.loginError;
            }
        }

        // DELETE api/CheckoutCart
        //Delete item or itemlist 
        //[RequireHttps]
        public string Delete([FromBody]TrackCart value)
        {
            string[] plainText = repository.Decoder(value.accID, true);//Decoder accid, login and datetime  status

            if (plainText.Length < 3)//Make sure there had three variable, accid, login and datetime  status.
                return OutputMessage.cookiesError;

            if (plainText[1] == loginStatus)//if loginstatus equal "True" then enter
            {

                int accountId;
                bool check = Int32.TryParse(plainText[0], out accountId);//Get accid 
                if (check == true && plainText[0] != null)
                {
                    if (!repository.SetTrackAll(accountId, plainText[2]))//Set AccountID into this repository class
                    {
                        return OutputMessage.accountError;
                    }
                }
                else
                {
                    return OutputMessage.accountNull;
                }
                if (value.opCode.ToString() == "31")//If opCode is 31, then delete all item
                {
                    return repository.RemoveTrack(value.itemID);
                }
                else if (value.opCode.ToString() == "33")//If opCode is 33, then delete a itemlist
                {
                    return repository.RemoveTrackItem(value.itemID[0], value.itemlistID[0]);
                }
                else if (value.opCode.ToString() == "35")// If opCode is 35, then delete select itemlists
                {
                    return repository.RemoveTrackItem(value.itemID[0], value.itemlistID);
                }
                else { return OutputMessage.wrongOP; }
            }
            else
            {
                return OutputMessage.loginError;
            }
        }

    }
    //public class RequireHttpsAttribute : AuthorizationFilterAttribute
    //{
    //    public override void OnAuthorization(HttpActionContext actionContext)
    //    {
    //        if (actionContext.Request.RequestUri.Scheme != Uri.UriSchemeHttps)
    //        {
    //            actionContext.Response = new HttpResponseMessage(System.Net.HttpStatusCode.Forbidden)
    //            {
    //                ReasonPhrase = "HTTPS Required"
    //            };
    //        }
    //        else
    //        {
    //            base.OnAuthorization(actionContext);
    //        }
    //    }
    //}
}
