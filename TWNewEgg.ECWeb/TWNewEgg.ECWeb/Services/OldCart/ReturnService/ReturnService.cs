using Newegg.Mobile.MvcApplication.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using TWNewEgg.Website.ECWeb.Models;
using TWNewEgg.DB.TWSQLDB.Models;
using TWNewEgg.DB.TWSQLDB.Models.ExtModels;
using TWNewEgg.DB.TWBACKENDDB.Models;
using TWNewEgg.ItemService.Models;
using TWNewEgg.Website.ECWeb.Service;
using TWNewEgg.Redeem.Service;
using TWNewEgg.InternalSendMail.Service;
using System.Data;
using System.Data.SqlClient;
using System.Web.Util;
using TWNewEgg.GetConfigData.Service;
using TWNewEgg.DB;
using TWNewEgg.Framework.ServiceApi;
using TWNewEgg.Models.ViewModels.Redeem;
namespace TWNewEgg.ECWeb.Services.OldCart.ReturnService
{
    public class ReturnService
    {
        private TWSqlDBContext dbbefore = new TWSqlDBContext();
        private TWBackendDBContext dbafter = new TWBackendDBContext();
        public  TWNewEgg.Models.ViewModels.Redeem.ActionResponse<TWNewEgg.Models.ViewModels.MyAccount.ReturnPost> Return(string SoCode, int accID) 
        {
            TWNewEgg.Models.ViewModels.Redeem.ActionResponse<TWNewEgg.Models.ViewModels.MyAccount.ReturnPost> ActionResponse = new TWNewEgg.Models.ViewModels.Redeem.ActionResponse<TWNewEgg.Models.ViewModels.MyAccount.ReturnPost>();
            TWNewEgg.Models.ViewModels.MyAccount.ReturnPost ReturnPost = new TWNewEgg.Models.ViewModels.MyAccount.ReturnPost();
       

                TWNewEgg.DB.TWBACKENDDB.Models.Cart CartList = dbafter.Cart.Where(x => x.ID == SoCode).FirstOrDefault();
                if (CartList != null)
                {
                    var payTypes = dbbefore.PayType.ToList();
                    int Paytype = dbbefore.SalesOrder.Where(x => x.Code == CartList.ID).Select(x => (int)x.PayType).FirstOrDefault();
                    int PaytypeID = dbbefore.SalesOrder.Where(x => x.Code == CartList.ID).Select(x => (int)x.PayTypeID).FirstOrDefault();
                    bool Paytypeboolen = ((payTypes.Where(x => x.PayType0rateNum == Paytype).FirstOrDefault() == payTypes.Where(x => x.PayType0rateNum == (int)TWNewEgg.DB.TWSQLDB.Models.PayType.nPayType.網路ATM).FirstOrDefault()) && (payTypes.Where(x => x.PayType0rateNum == Paytype).FirstOrDefault() == payTypes.Where(x => x.PayType0rateNum == (int)TWNewEgg.DB.TWSQLDB.Models.PayType.nPayType.貨到付款).FirstOrDefault()) && (payTypes.Where(x => x.PayType0rateNum == Paytype).FirstOrDefault() == payTypes.Where(x => x.PayType0rateNum == (int)TWNewEgg.DB.TWSQLDB.Models.PayType.nPayType.實體ATM).FirstOrDefault()));
                    // 訂單帳戶編號
                    int cartACCID = int.Parse(CartList.UserID);
                    Dictionary<bool, string> dicstatus = new Dictionary<bool, string>();
                   dicstatus= returnstatus(accID, cartACCID,(int) CartList.DelvStatus);
                   if (dicstatus.Select(x => x.Key).FirstOrDefault() != false)
                   {
                       AutoMapper.Mapper.Map(CartList, ReturnPost);
                       TWNewEgg.DB.TWSQLDB.Models.Member Member= dbbefore.Member.Where(x => x.AccID == cartACCID).FirstOrDefault();
                       ReturnPost.Lastname = Member.Lastname;
                       ReturnPost.Firstname = Member.Firstname;
                       ReturnPost.Paytypeboolen = Paytypeboolen;

                       if (Member.Sex == null)
                           ReturnPost.Sex = 9;
                       else
                           ReturnPost.Sex = (int)Member.Sex;

                       List<TWNewEgg.DB.TWBACKENDDB.Models.Process> ProcessList = dbafter.Process.Where(x => x.CartID == SoCode).ToList();
                       ReturnPost.processlist = new List<TWNewEgg.Models.ViewModels.MyAccount.Process>();
                       string addr = "";
                       string address = "";
                         string[] tel = ReturnPost.Phone.Split(new char[] { '(', ')', '#' }).Where(s => !string.IsNullOrEmpty(s)).ToArray();
                         ReturnPost.TelZip = tel.Length > 0 ? tel[0] : "";
                         ReturnPost.TelDay2 = tel.Length > 1 ? tel[1] : "";
                         ReturnPost.TelExtension = tel.Length > 2 ? tel[2] : "";
              
                       TWNewEgg.Models.ViewModels.MyAccount.AddressQuery addrQuery2 = AddressAnalyse(true, CartList.Location, CartList.ADDR, CartList.Zipcode.Substring(0, 3));
                       if (addrQuery2.FindCity)
                       {
                           addr = addrQuery2.City;
                           address = addrQuery2.Addr;
                       }
                       else
                       {
                           addr = "";
                           address = CartList.ADDR;
                       }
                       ReturnPost.Code = CartList.ID;
                 
                       ReturnPost.addr = addr;
                       ReturnPost.address = address;
                       foreach (var ProcessLists in ProcessList)
                       {
                           TWNewEgg.Models.ViewModels.MyAccount.Process Process = new TWNewEgg.Models.ViewModels.MyAccount.Process();
                           TWNewEgg.DB.TWSQLDB.Models.Item Item = dbbefore.Item.Where(x => x.ID == ProcessLists.StoreID).FirstOrDefault();
                           ReturnPost.processlist = new List<TWNewEgg.Models.ViewModels.MyAccount.Process>();
                           Process.ProductName = Regex.Replace(Item.Name, @"[\r\n]+\s{0,}[\r\n]+", " ");
                           Process.PriceCash = (int)ProcessLists.DisplayPrice;
                           Process.code = ProcessLists.ID;
                           ReturnPost.processlist.Add(Process);

                       }
                       ActionResponse.IsSuccess = true;
                       ActionResponse.Body = ReturnPost;
                   }
                   else
                   {
                       string MSG = "";
                        MSG=dicstatus.Select(x => x.Value).FirstOrDefault();
                       ActionResponse.IsSuccess = false;
                       ActionResponse.Msg = MSG;
                      
                   }
               
                  

              
            }
            else 
            {

                ActionResponse.IsSuccess = false;
                ActionResponse.Msg = "return_form";
            }
            return ActionResponse;
        }



        public TWNewEgg.Models.ViewModels.Redeem.ActionResponse<TWNewEgg.Models.ViewModels.MyAccount.ReturnPost> Refund(string SoCode, int accID)
        {
            TWNewEgg.Models.ViewModels.Redeem.ActionResponse<TWNewEgg.Models.ViewModels.MyAccount.ReturnPost> ActionResponse = new TWNewEgg.Models.ViewModels.Redeem.ActionResponse<TWNewEgg.Models.ViewModels.MyAccount.ReturnPost>();
            TWNewEgg.Models.ViewModels.MyAccount.ReturnPost ReturnPost = new TWNewEgg.Models.ViewModels.MyAccount.ReturnPost();


            TWNewEgg.DB.TWBACKENDDB.Models.Cart CartList = dbafter.Cart.Where(x => x.ID == SoCode).FirstOrDefault();
            if (CartList != null)
            {
                //payTypes所有資料
                var payTypes = dbbefore.PayType.ToList();
                int Paytype = dbbefore.SalesOrder.Where(x => x.Code == CartList.ID).Select(x => (int)x.PayType).FirstOrDefault();
                int PaytypeID = dbbefore.SalesOrder.Where(x => x.Code == CartList.ID).Select(x => (int)x.PayTypeID).FirstOrDefault();
                bool Paytypeboolen = ((payTypes.Where(x => x.PayType0rateNum == Paytype).FirstOrDefault() == payTypes.Where(x => x.PayType0rateNum == (int)TWNewEgg.DB.TWSQLDB.Models.PayType.nPayType.網路ATM).FirstOrDefault()) || (payTypes.Where(x => x.PayType0rateNum == Paytype).FirstOrDefault() == payTypes.Where(x => x.PayType0rateNum == (int)TWNewEgg.DB.TWSQLDB.Models.PayType.nPayType.貨到付款).FirstOrDefault()) || (payTypes.Where(x => x.PayType0rateNum == Paytype).FirstOrDefault() == payTypes.Where(x => x.PayType0rateNum == (int)TWNewEgg.DB.TWSQLDB.Models.PayType.nPayType.實體ATM).FirstOrDefault()));
                // 訂單帳戶編號
                int cartACCID = int.Parse(CartList.UserID);
                Dictionary<bool, string> dicstatus = new Dictionary<bool, string>();
                dicstatus = refundstatus(accID, cartACCID, (int)CartList.DelvStatus, (int)CartList.Status);
                if (dicstatus.Select(x => x.Key).FirstOrDefault() != false)
                {
                    AutoMapper.Mapper.Map(CartList, ReturnPost);
                    TWNewEgg.DB.TWSQLDB.Models.Member Member = dbbefore.Member.Where(x => x.AccID == cartACCID).FirstOrDefault();
                    ReturnPost.Lastname = Member.Lastname;
                    ReturnPost.Firstname = Member.Firstname;
                    ReturnPost.Paytypeboolen = Paytypeboolen;
                    
                    if (Member.Sex == null)
                        ReturnPost.Sex = 9;
                    else
                        ReturnPost.Sex = (int)Member.Sex;

                    List<TWNewEgg.DB.TWBACKENDDB.Models.Process> ProcessList = dbafter.Process.Where(x => x.CartID == SoCode).ToList();
                    ReturnPost.processlist = new List<TWNewEgg.Models.ViewModels.MyAccount.Process>();
                    string addr = "";
                    string address = "";
                    string[] tel = ReturnPost.Phone.Split(new char[] { '(', ')', '#' }).Where(s => !string.IsNullOrEmpty(s)).ToArray();
                    ReturnPost.TelZip = tel.Length > 0 ? tel[0] : "";
                    ReturnPost.TelDay2 = tel.Length > 1 ? tel[1] : "";
                    ReturnPost.TelExtension = tel.Length > 2 ? tel[2] : "";
                     TWNewEgg.Models.ViewModels.MyAccount.AddressQuery addrQuery2 = AddressAnalyse(true, CartList.Location, CartList.ADDR, CartList.Zipcode.Substring(0, 3));
                    if (addrQuery2.FindCity)
                    {
                        addr = addrQuery2.City;
                        address = addrQuery2.Addr;
                    }
                    else
                    {
                        addr = "";
                        address = CartList.ADDR;
                    }
                    ReturnPost.Code = CartList.ID;

                    ReturnPost.addr = addr;
                    ReturnPost.address = address;
                    string ProcessListsstring="";
                    foreach (var ProcessLists in ProcessList)
                    {
                        TWNewEgg.Models.ViewModels.MyAccount.Process Process = new TWNewEgg.Models.ViewModels.MyAccount.Process();
                        TWNewEgg.DB.TWSQLDB.Models.Item Item = dbbefore.Item.Where(x => x.ID == ProcessLists.StoreID).FirstOrDefault();
                        ReturnPost.processlist = new List<TWNewEgg.Models.ViewModels.MyAccount.Process>();
                        Process.ProductName = Regex.Replace(Item.Name, @"[\r\n]+\s{0,}[\r\n]+", " ");
                        Process.PriceCash = (int)ProcessLists.DisplayPrice;
                        Process.code = ProcessLists.ID;
                
                        ReturnPost.processlist.Add(Process);

                    }
                   List<string> Salceorder= dbafter.Cart.Where(x => x.SalesorderGroupID == CartList.SalesorderGroupID).Select(x=>x.ID).ToList();
                   foreach (string Salceorderitem in Salceorder) 
                   {

                       ProcessListsstring += (ProcessListsstring.Trim() == "" ? "" : ",") + Salceorderitem;
                   }
                    ReturnPost.SalceorderCodeList=ProcessListsstring;
                    ActionResponse.IsSuccess = true;
                    ActionResponse.Body = ReturnPost;
                }
                else
                {
                    string MSG = "";
                    MSG = dicstatus.Select(x => x.Value).FirstOrDefault();
                    ActionResponse.IsSuccess = false;
                    ActionResponse.Msg = MSG;

                }




            }
            else
            {

                ActionResponse.IsSuccess = false;
                ActionResponse.Msg = "return_form";
            }
            return ActionResponse;
        }

        #region -- AddressAnalyse --
        /// <summary>
        /// 鄉鎮市區與街道等解析
        /// </summary>
        /// <param name="address"></param>
        /// <returns></returns>
        public static TWNewEgg.Models.ViewModels.MyAccount.AddressQuery AddressAnalyse(bool recv, string county, string address, string zipCode)
        {
            bool successFlag = false;
            char KeyWord = new char();
            TWNewEgg.Models.ViewModels.MyAccount.AddressQuery addressQuery = new TWNewEgg.Models.ViewModels.MyAccount.AddressQuery();

            string str = "鄉鎮市區";
            char item = new char();
            for (var inti = 0; inti < str.Length; inti++)
            {
                item = char.Parse(str.Substring(inti, 1));
                if (CityCheck(recv, county, address.Split(item)[0] + item, zipCode))
                {
                    KeyWord = item;
                    successFlag = true;
                    break;
                }
            }

            // 若查詢的資料是存在的
            if (successFlag)
            {
                addressQuery.FindCity = true;
                addressQuery.County = county;
                addressQuery.City = address.Split(KeyWord)[0] + KeyWord;
                if (address.Split(KeyWord).Length == 2)
                {
                    addressQuery.Addr = address.Split(KeyWord)[1];
                }
                else
                {
                    for (int subAddr = 1; subAddr < address.Split(KeyWord).Length - 1; subAddr++)
                    {
                        addressQuery.Addr += address.Split(KeyWord)[subAddr] + KeyWord;
                    }
                    addressQuery.Addr += address.Split(KeyWord)[address.Split(KeyWord).Length - 1];
                }
                addressQuery.ZipCode = zipCode;
                //return Json(new { AddressQuery = addressQuery });
            }

            // 若查詢的資料不存在，FindCity = false(預設)
            //return Json(new { AddressQuery = addressQuery });

            return addressQuery;
        }

        /// <summary>
        /// 驗證該鄉鎮市區是否存在
        /// </summary>
        /// <param name="recv"></param>
        /// <param name="county"></param>
        /// <param name="city"></param>
        /// <param name="zipCode"></param>
        /// <returns>若存在則回傳true，不存在則回傳false</returns>
        public static bool CityCheck(bool recv, string county, string city, string zipCode)
        {
            string[][] twAddrData = null;
            if (!recv)
            {
                twAddrData = new string[][]
                {
                    new string[] {"基隆市", "200:仁愛區", "201:信義區", "202:中正區", "203:中山區", "204:安樂區", "205:暖暖區", "206:七堵區"},
                    new string[] {"台北市", "100:中正區", "103:大同區", "104:中山區", "105:松山區", "106:大安區", "108:萬華區", "110:信義區", "111:士林區", "112:北投區", "114:內湖區", "115:南港區", "116:文山區"},
                    new string[] {"新北市", "207:萬里區", "208:金山區", "220:板橋區", "221:汐止區", "222:深坑區", "223:石碇區", "224:瑞芳區", "226:平溪區", "227:雙溪區", "228:貢寮區", "231:新店區", "232:坪林區", "233:烏來區", "234:永和區", "235:中和區", "236:土城區", "237:三峽區", "238:樹林區", "239:鶯歌區", "241:三重區", "242:新莊區", "243:泰山區", "244:林口區", "247:蘆洲區", "248:五股區", "249:八里區", "251:淡水區", "252:三芝區", "253:石門區"},
                    new string[] {"桃園縣", "320:中壢市", "324:平鎮市", "325:龍潭鄉", "326:楊梅市", "327:新屋鄉", "328:觀音鄉", "330:桃園市", "333:龜山鄉", "334:八德市", "335:大溪鎮", "336:復興鄉", "337:大園鄉", "338:蘆竹鄉"},
                    new string[] {"新竹市", "300:東區", "300:北區", "300:香山區"},
                    new string[] {"新竹縣", "302:竹北市", "303:湖口鄉", "304:新豐鄉", "305:新埔鎮", "306:關西鎮", "307:芎林鄉", "308:寶山鄉", "310:竹東鎮", "311:五峰鄉", "312:橫山鄉", "313:尖石鄉", "314:北埔鄉", "315:峨眉鄉"},
                    new string[] {"苗栗縣", "350:竹南鎮", "351:頭份鎮", "352:三灣鄉", "353:南庄鄉", "354:獅潭鄉", "356:後龍鎮", "357:通霄鎮", "358:苑裡鎮", "360:苗栗市", "361:造橋鄉", "362:頭屋鄉", "363:公館鄉", "364:大湖鄉", "365:泰安鄉", "366:銅鑼鄉", "367:三義鄉", "368:西湖鄉", "369:卓蘭鎮"},
                    new string[] {"台中市", "400:中區", "401:東區", "402:南區", "403:西區", "404:北區", "406:北屯區", "407:西屯區", "408:南屯區", "411:太平區", "412:大里區", "413:霧峰區", "414:烏日區", "420:豐原區", "421:后里區", "422:石岡區", "423:東勢區", "424:和平區", "426:新社區", "427:潭子區", "428:大雅區", "429:神岡區", "432:大肚區", "433:沙鹿區", "434:龍井區", "435:梧棲區", "436:清水區", "437:大甲區", "438:外埔區", "439:大安區"},
                    new string[] {"彰化縣", "500:彰化市", "502:芬園鄉", "503:花壇鄉", "504:秀水鄉", "505:鹿港鎮", "506:福興鄉", "507:線西鄉", "508:和美鎮", "509:伸港鄉", "510:員林鎮", "511:社頭鄉", "512:永靖鄉", "513:埔心鄉", "514:溪湖鎮", "515:大村鄉", "516:埔鹽鄉", "520:田中鎮", "521:北斗鎮", "522:田尾鄉", "523:埤頭鄉", "524:溪州鄉", "525:竹塘鄉", "526:二林鎮", "527:大城鄉", "528:芳苑鄉", "530:二水鄉"},
                    new string[] {"南投縣", "540:南投市", "541:中寮鄉", "542:草屯鎮", "544:國姓鄉", "545:埔里鎮", "546:仁愛鄉", "551:名間鄉", "552:集集鎮", "553:水里鄉", "555:魚池鄉", "556:信義鄉", "557:竹山鎮", "558:鹿谷鄉"},
                    new string[] {"雲林縣", "630:斗南鎮", "631:大埤鄉", "632:虎尾鎮", "633:土庫鎮", "635:東勢鄉", "634:褒忠鄉", "636:台西鄉", "637:崙背鄉", "638:麥寮鄉", "640:斗六市", "643:林內鄉", "646:古坑鄉", "647:莿桐鄉", "648:西螺鎮", "649:二崙鄉", "651:北港鎮", "652:水林鄉", "653:口湖鄉", "654:四湖鄉", "655:元長鄉"},
                    new string[] {"嘉義市", "600:東區", "600:西區"},
                    new string[] {"嘉義縣", "602:番路鄉", "603:梅山鄉", "604:竹崎鄉", "605:阿里山鄉", "606:中埔鄉", "607:大埔鄉", "608:水上鄉", "611:鹿草鄉", "612:太保市", "613:朴子市", "614:東石鄉", "615:六腳鄉", "616:新港鄉", "621:民雄鄉", "622:大林鎮", "623:溪口鄉", "624:義竹鄉", "625:布袋鎮"},
                    new string[] {"台南市", "700:中西區", "701:東區", "702:南區", "704:北區", "708:安平區", "709:安南區", "710:永康區", "711:歸仁區", "712:新化區", "713:左鎮區", "714:玉井區", "715:楠西區", "717:仁德區", "718:關廟區", "716:南化區", "719:龍崎區", "720:官田區", "721:麻豆區", "722:佳里區", "723:西港區", "724:七股區", "725:將軍區", "726:學甲區", "727:北門區", "730:新營區", "731:後壁區", "732:白河區", "733:東山區", "734:六甲區", "735:下營區", "736:柳營區", "737:鹽水區", "741:善化區", "742:大內區", "744:新市區", "745:安定區", "743:山上區"},
                    new string[] {"高雄市", "800:新興區", "801:前金區", "802:苓雅區", "803:鹽埕區", "804:鼓山區", "805:旗津區", "806:前鎮區", "807:三民區", "811:楠梓區", "812:小港區", "813:左營區", "814:仁武區", "815:大社區", "817:東沙群島", "820:岡山區", "821:路竹區", "819:南沙群島", "822:阿蓮區", "823:田寮區", "824:燕巢區", "825:橋頭區", "826:梓官區", "827:彌陀區", "829:湖內區", "828:永安區", "830:鳳山區", "831:大寮區", "832:林園區", "833:鳥松區", "840:大樹區", "842:旗山區", "843:美濃區", "844:六龜區", "845:內門區", "847:甲仙區", "846:杉林區", "848:桃源區", "852:茄萣區", "851:茂林區", "849:那瑪夏區"},
                    new string[] {"屏東縣", "900:屏東市", "901:三地門鄉", "902:霧台鄉", "903:瑪家鄉", "904:九如鄉", "905:里港鄉", "906:高樹鄉", "907:鹽埔鄉", "908:長治鄉", "909:麟洛鄉", "911:竹田鄉", "912:內埔鄉", "913:萬丹鄉", "920:潮州鎮", "921:泰武鄉", "923:萬巒鄉", "922:來義鄉", "924:崁頂鄉", "925:新埤鄉", "926:南州鄉", "927:林邊鄉", "928:東港鎮", "929:琉球鄉", "931:佳冬鄉", "932:新園鄉", "940:枋寮鄉", "941:枋山鄉", "943:獅子鄉", "944:車城鄉", "945:牡丹鄉", "946:恆春鎮", "947:滿州鄉", "942:春日鄉"},
                    new string[] {"台東縣", "950:台東市", "951:綠島鄉", "952:蘭嶼鄉", "954:卑南鄉", "955:鹿野鄉", "956:關山鎮", "957:海端鄉", "958:池上鄉", "953:延平鄉", "959:東河鄉", "961:成功鎮", "962:長濱鄉", "963:太麻里鄉", "965:大武鄉", "966:達仁鄉", "964:金峰鄉"},
                    new string[] {"花蓮縣", "970:花蓮市", "971:新城鄉", "972:秀林鄉", "973:吉安鄉", "974:壽豐鄉", "975:鳳林鎮", "976:光復鄉", "977:豐濱鄉", "978:瑞穗鄉", "981:玉里鎮", "979:萬榮鄉", "983:富里鄉", "982:卓溪鄉"},
                    new string[] {"宜蘭縣", "260:宜蘭市", "261:頭城鎮", "262:礁溪鄉", "263:壯圍鄉", "264:員山鄉", "265:羅東鎮", "266:三星鄉", "267:大同鄉", "268:五結鄉", "269:冬山鄉", "270:蘇澳鎮", "272:南澳鄉", "290:釣魚台"},
                    new string[] {"澎湖縣", "880:馬公市", "881:西嶼鄉", "882:望安鄉", "885:湖西鄉", "883:七美鄉", "884:白沙鄉"},
                    new string[] {"金門縣", "890:金沙鎮", "891:金湖鎮", "892:金寧鄉", "893:金城鎮", "894:烈嶼鄉", "896:烏坵鄉"},
                    new string[] {"連江縣", "209:南竿鄉", "210:北竿鄉", "211:莒光鄉", "212:東引鄉"},
                };
            }
            else
            {
                twAddrData = new string[][]
                {
                    new string[] {"基隆市", "200:仁愛區", "201:信義區", "202:中正區", "203:中山區", "204:安樂區", "205:暖暖區", "206:七堵區"},
                    new string[] {"台北市", "100:中正區", "103:大同區", "104:中山區", "105:松山區", "106:大安區", "108:萬華區", "110:信義區", "111:士林區", "112:北投區", "114:內湖區", "115:南港區", "116:文山區"},
                    new string[] {"新北市", "207:萬里區", "208:金山區", "220:板橋區", "221:汐止區", "222:深坑區", "223:石碇區", "224:瑞芳區", "226:平溪區", "227:雙溪區", "228:貢寮區", "231:新店區", "232:坪林區", "233:烏來區", "234:永和區", "235:中和區", "236:土城區", "237:三峽區", "238:樹林區", "239:鶯歌區", "241:三重區", "242:新莊區", "243:泰山區", "244:林口區", "247:蘆洲區", "248:五股區", "249:八里區", "251:淡水區", "252:三芝區", "253:石門區"},
                    new string[] {"桃園縣", "320:中壢市", "324:平鎮市", "325:龍潭鄉", "326:楊梅市", "327:新屋鄉", "328:觀音鄉", "330:桃園市", "333:龜山鄉", "334:八德市", "335:大溪鎮", "336:復興鄉", "337:大園鄉", "338:蘆竹鄉"},
                    new string[] {"新竹市", "300:東區", "300:北區", "300:香山區"},
                    new string[] {"新竹縣", "302:竹北市", "303:湖口鄉", "304:新豐鄉", "305:新埔鎮", "306:關西鎮", "307:芎林鄉", "308:寶山鄉", "310:竹東鎮", "311:五峰鄉", "312:橫山鄉", "313:尖石鄉", "314:北埔鄉", "315:峨眉鄉"},
                    new string[] {"苗栗縣", "350:竹南鎮", "351:頭份鎮", "352:三灣鄉", "353:南庄鄉", "354:獅潭鄉", "356:後龍鎮", "357:通霄鎮", "358:苑裡鎮", "360:苗栗市", "361:造橋鄉", "362:頭屋鄉", "363:公館鄉", "364:大湖鄉", "365:泰安鄉", "366:銅鑼鄉", "367:三義鄉", "368:西湖鄉", "369:卓蘭鎮"},
                    new string[] {"台中市", "400:中區", "401:東區", "402:南區", "403:西區", "404:北區", "406:北屯區", "407:西屯區", "408:南屯區", "411:太平區", "412:大里區", "413:霧峰區", "414:烏日區", "420:豐原區", "421:后里區", "422:石岡區", "423:東勢區", "424:和平區", "426:新社區", "427:潭子區", "428:大雅區", "429:神岡區", "432:大肚區", "433:沙鹿區", "434:龍井區", "435:梧棲區", "436:清水區", "437:大甲區", "438:外埔區", "439:大安區"},
                    new string[] {"彰化縣", "500:彰化市", "502:芬園鄉", "503:花壇鄉", "504:秀水鄉", "505:鹿港鎮", "506:福興鄉", "507:線西鄉", "508:和美鎮", "509:伸港鄉", "510:員林鎮", "511:社頭鄉", "512:永靖鄉", "513:埔心鄉", "514:溪湖鎮", "515:大村鄉", "516:埔鹽鄉", "520:田中鎮", "521:北斗鎮", "522:田尾鄉", "523:埤頭鄉", "524:溪州鄉", "525:竹塘鄉", "526:二林鎮", "527:大城鄉", "528:芳苑鄉", "530:二水鄉"},
                    new string[] {"南投縣", "540:南投市", "541:中寮鄉", "542:草屯鎮", "544:國姓鄉", "545:埔里鎮", "546:仁愛鄉", "551:名間鄉", "552:集集鎮", "553:水里鄉", "555:魚池鄉", "556:信義鄉", "557:竹山鎮", "558:鹿谷鄉"},
                    new string[] {"雲林縣", "630:斗南鎮", "631:大埤鄉", "632:虎尾鎮", "633:土庫鎮", "635:東勢鄉", "634:褒忠鄉", "636:台西鄉", "637:崙背鄉", "638:麥寮鄉", "640:斗六市", "643:林內鄉", "646:古坑鄉", "647:莿桐鄉", "648:西螺鎮", "649:二崙鄉", "651:北港鎮", "652:水林鄉", "653:口湖鄉", "654:四湖鄉", "655:元長鄉"},
                    new string[] {"嘉義市", "600:東區", "600:西區"},
                    new string[] {"嘉義縣", "602:番路鄉", "603:梅山鄉", "604:竹崎鄉", "605:阿里山鄉", "606:中埔鄉", "607:大埔鄉", "608:水上鄉", "611:鹿草鄉", "612:太保市", "613:朴子市", "614:東石鄉", "615:六腳鄉", "616:新港鄉", "621:民雄鄉", "622:大林鎮", "623:溪口鄉", "624:義竹鄉", "625:布袋鎮"},
                    new string[] {"台南市", "700:中西區", "701:東區", "702:南區", "704:北區", "708:安平區", "709:安南區", "710:永康區", "711:歸仁區", "712:新化區", "713:左鎮區", "714:玉井區", "715:楠西區", "717:仁德區", "718:關廟區", "716:南化區", "719:龍崎區", "720:官田區", "721:麻豆區", "722:佳里區", "723:西港區", "724:七股區", "725:將軍區", "726:學甲區", "727:北門區", "730:新營區", "731:後壁區", "732:白河區", "733:東山區", "734:六甲區", "735:下營區", "736:柳營區", "737:鹽水區", "741:善化區", "742:大內區", "744:新市區", "745:安定區", "743:山上區"},
                    new string[] {"高雄市", "800:新興區", "801:前金區", "802:苓雅區", "803:鹽埕區", "804:鼓山區", "805:旗津區", "806:前鎮區", "807:三民區", "811:楠梓區", "812:小港區", "813:左營區", "814:仁武區", "815:大社區"/*, "817:東沙群島"*/, "820:岡山區", "821:路竹區"/*, "819:南沙群島"*/, "822:阿蓮區", "823:田寮區", "824:燕巢區", "825:橋頭區", "826:梓官區", "827:彌陀區", "829:湖內區", "828:永安區", "830:鳳山區", "831:大寮區", "832:林園區", "833:鳥松區", "840:大樹區", "842:旗山區", "843:美濃區", "844:六龜區", "845:內門區", "847:甲仙區", "846:杉林區", "848:桃源區", "852:茄萣區", "851:茂林區", "849:那瑪夏區"},
                    new string[] {"屏東縣", "900:屏東市", "901:三地門鄉", "902:霧台鄉", "903:瑪家鄉", "904:九如鄉", "905:里港鄉", "906:高樹鄉", "907:鹽埔鄉", "908:長治鄉", "909:麟洛鄉", "911:竹田鄉", "912:內埔鄉", "913:萬丹鄉", "920:潮州鎮", "921:泰武鄉", "923:萬巒鄉", "922:來義鄉", "924:崁頂鄉", "925:新埤鄉", "926:南州鄉", "927:林邊鄉", "928:東港鎮"/*, "929:琉球鄉"*/, "931:佳冬鄉", "932:新園鄉", "940:枋寮鄉", "941:枋山鄉", "943:獅子鄉", "944:車城鄉", "945:牡丹鄉", "946:恆春鎮", "947:滿州鄉", "942:春日鄉"},
                    new string[] {"台東縣", "950:台東市", /*"951:綠島鄉", "952:蘭嶼鄉",*/ "954:卑南鄉", "955:鹿野鄉", "956:關山鎮", "957:海端鄉", "958:池上鄉", "953:延平鄉", "959:東河鄉", "961:成功鎮", "962:長濱鄉", "963:太麻里鄉", "965:大武鄉", "966:達仁鄉", "964:金峰鄉"},
                    new string[] {"花蓮縣", "970:花蓮市", "971:新城鄉", "972:秀林鄉", "973:吉安鄉", "974:壽豐鄉", "975:鳳林鎮", "976:光復鄉", "977:豐濱鄉", "978:瑞穗鄉", "981:玉里鎮", "979:萬榮鄉", "983:富里鄉", "982:卓溪鄉"},
                    new string[] {"宜蘭縣", "260:宜蘭市", "261:頭城鎮", "262:礁溪鄉", "263:壯圍鄉", "264:員山鄉", "265:羅東鎮", "266:三星鄉", "267:大同鄉", "268:五結鄉", "269:冬山鄉", "270:蘇澳鎮", "272:南澳鄉"/*, "290:釣魚台"*/},
                    new string[] {"澎湖縣", "880:馬公市", "881:西嶼鄉", "882:望安鄉", "885:湖西鄉", "883:七美鄉", "884:白沙鄉"},
                    new string[] {"金門縣", "890:金沙鎮", "891:金湖鎮", "892:金寧鄉", "893:金城鎮", "894:烈嶼鄉", "896:烏坵鄉"},
                    new string[] {"連江縣", "209:南竿鄉", "210:北竿鄉", "211:莒光鄉", "212:東引鄉"},
                };
            }

            int countyNumber = 0;
            // 找出縣市位置
            for (int countyi = 0; countyi < 22; countyi++)
            {
                if (twAddrData[countyi][0] == county)
                {
                    countyNumber = countyi;
                    break;
                }
            }

            // 驗證該鄉鎮市區是否存在
            for (int cityi = 0; cityi < twAddrData[countyNumber].Length; cityi++)
            {
                if (twAddrData[countyNumber][cityi] == zipCode + ":" + city)
                {
                    twAddrData = null;
                    // 若存在則回傳true
                    return true;
                }
            }

            twAddrData = null;
            // 若不存在則回傳false
            return false;
        }
        #endregion
        public Dictionary<bool,string> returnstatus(int accID,int CartUserID,int Delvstatus) 
        {
            Dictionary<bool, string> dicstatus = new Dictionary<bool, string>();
             // 判斷登入帳戶是否與訂單帳戶相同,若不同返回首頁
            if (accID != CartUserID)
            {

                dicstatus.Add(false, "Home");

            }
            else
            {
                // 判斷定單是否配達,沒有配達則返回首頁
                if (Delvstatus == (int)TWNewEgg.DB.TWBACKENDDB.Models.Cart.cartstatus.配達)
                {
                    dicstatus.Add(true, "");
                }
                else
                {
                    dicstatus.Add(false, "Home");
                }
            
            }
            return dicstatus;
        }
        public Dictionary<bool, string> refundstatus(int accID, int CartUserID, int Delvstatus,int CartStatus)
        {
            Dictionary<bool, string> dicstatus = new Dictionary<bool, string>();
            // 判斷登入帳戶是否與訂單帳戶相同,若不同返回首頁
            if (accID != CartUserID)
            {

                dicstatus.Add(false, "Home");

            }
            else
            {
                // 判斷定單是否配達,沒有配達則返回首頁
                if (CartStatus == 0 && (Delvstatus == (int)TWNewEgg.DB.TWBACKENDDB.Models.Cart.cartstatus.初始狀態 || Delvstatus == (int)TWNewEgg.DB.TWBACKENDDB.Models.Cart.cartstatus.已成立))
                {
                    dicstatus.Add(true, "");
                }
                else
                {
                    dicstatus.Add(false, "Home");
                }

            }
            return dicstatus;
        }

             /// <summary>
        /// 取消訂單用API
        /// </summary>
        /// <param name="salesorder_code">訂單編號</param>
        /// <param name="reset_reasonval">取消原因</param>
        /// <param name="reset_reasontext">取消原因描述</param>
        /// <param name="bankid">銀行ID</param>
        /// <param name="branches">銀行分行</param>
        /// <param name="bankaccount">銀行帳號</param>
        /// <param name="passive">??</param>
        /// <param name="password">密碼輸入??</param>
        /// <returns>Json成功失敗</returns>
        [AcceptVerbs(HttpVerbs.Post)]
        public TWNewEgg.Models.ViewModels.Redeem.ActionResponse<TWNewEgg.Models.ViewModels.MyAccount.ReturnPost> Refundpost(TWNewEgg.Models.ViewModels.MyAccount.ReturnPost ReturnPost, int? return_reasonval, string return_reasontext, int accid)
        {
            Thread.Sleep(1000);
            log4net.ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.FullName);
            TWNewEgg.Models.ViewModels.Redeem.ActionResponse<TWNewEgg.Models.ViewModels.MyAccount.ReturnPost> ReturnPostAction = new TWNewEgg.Models.ViewModels.Redeem.ActionResponse<TWNewEgg.Models.ViewModels.MyAccount.ReturnPost>();
            // 判斷字串取消原因描述是否過長,若過長則自動截斷
            if (return_reasontext.Length > 240)
            {
                return_reasontext = return_reasontext.Substring(0, 239);
            }

            // 判斷是否登入,登入則回AccountID,否則回0

            // 若登入則繼續,沒登入則Json回饋上味登入訊息
            if (accid > 0)
            {
                // 撈取取消單資料
                List<SalesOrderCancel> salesOrderCancellist = dbbefore.SalesOrderCancel.Where(x => x.SalesorderCode == ReturnPost.Code).ToList();
                // 撈取SO資料
                TWNewEgg.DB.TWBACKENDDB.Models.Cart cartTemp = dbafter.Cart.Where(x => x.ID == ReturnPost.Code).FirstOrDefault();
                // 撈取Group底下SO狀態為正常且成立,
                List<TWNewEgg.DB.TWBACKENDDB.Models.Cart> cartList = dbafter.Cart.Where(x => x.SalesorderGroupID == (cartTemp.SalesorderGroupID ?? 0) && x.Status == 0 && (x.DelvStatus == (int)TWNewEgg.DB.TWBACKENDDB.Models.Cart.cartstatus.初始狀態 || x.DelvStatus == (int)TWNewEgg.DB.TWBACKENDDB.Models.Cart.cartstatus.已成立)).ToList();

                // 判斷cartList是否有訂單可以取消
                if (cartList.Count == 0)
                {
                    ReturnPostAction.IsSuccess = false;
                    ReturnPostAction.Msg = "error";
                    return ReturnPostAction;
                }
                // 判斷取消訂單與登入帳戶為同一人
                if (accid != int.Parse(cartList.FirstOrDefault().UserID))
                {
                    ReturnPostAction.IsSuccess = false;
                    ReturnPostAction.Msg = "AccidError";
                    return ReturnPostAction;
                  
                }
                // 判斷訂單是否已經取消過在salesOrderCancellist有資料
                if (salesOrderCancellist.Count > 0)
                {
                    ReturnPostAction.IsSuccess = false;
                    ReturnPostAction.Msg = "canceled";
                    return ReturnPostAction;
              
                }
                //// 前台SO資料
                //SalesOrder salesOrder = dbbefore.SalesOrder.Single(x => x.Code == salesorder_code);
                // 訂單帳戶資料
                TWNewEgg.DB.TWSQLDB.Models.Account account = dbbefore.Account.Where(x => x.ID == accid).FirstOrDefault();
                // Logger取消訂單開始
                logger.Debug("start grepping data from db, salesorder " + ReturnPost.Code);

                // SOGroupID
                int? sogroup = cartTemp.SalesorderGroupID;
                // SOGroupID底下前台SO訂單
                List<SalesOrder> salesOrders = dbbefore.SalesOrder.Where(s => s.SalesOrderGroupID == sogroup).ToList();
                // SalesorderCodeList SOGroupID底下前台SOIDList
                List<string> salesorderCodes = salesOrders.Select(s => s.Code).ToList();
                // SalesOrderItemList SOGroupID底下f前台SOitemList
                List<SalesOrderItem> salesOrderItems = dbbefore.SalesOrderItem.Where(p => salesorderCodes.Contains(p.SalesorderCode)).ToList();
                // SalesorderCodeList SOGroupID底下前台SOListIDList
                List<string> salesOrderItemsIDstring = salesOrderItems.Select(s => s.Code).ToList();
                // CattList SOGroupID底下後台SO訂單
                List<TWNewEgg.DB.TWBACKENDDB.Models.Cart> carts = dbafter.Cart.Where(x => x.SalesorderGroupID == sogroup).ToList();
                // CattIDList SOGroupID底下後台SOIDList
                List<string> cartsIDs = carts.Select(c => c.ID).ToList();
                // POCodesList SOGroupID底下後台POList
                List<string> pocodes = dbbefore.PurchaseOrder.Where(p => salesorderCodes.Contains(p.SalesorderCode)).Select(p => p.Code).ToList();
                // POItemsList SOGroupID底下後台POitemList
                List<PurchaseOrderItem> poitems = dbbefore.PurchaseOrderItem.Where(p => pocodes.Contains(p.PurchaseorderCode) && p.SellerOrderCode != null && p.SellerOrderCode.Trim() != "").ToList();
                // processesList SOGroupID底下後台SOitemList
                List<Process> processes = dbafter.Process.Where(p => cartsIDs.Contains(p.CartID)).ToList();
                // CouponsList
                List<TWNewEgg.DB.TWSQLDB.Models.Coupon> coupons;
                // CouponIDList
                List<string> couponIDListString;
                // eachCouponIDList
                List<string> eachcouponIDListString;
                // CouponIDList to Int
                List<int> couponIDListInt = new List<int>();

                // 判斷SOitemList數量(是否轉單)
                if (processes.Count > 0)
                {
                    couponIDListString = processes.Select(x => x.Coupons).ToList();
                }
                else
                {
                    couponIDListString = salesOrderItems.Select(x => x.Coupons).ToList();
                }

                // 為改變coupons狀態為使用取消,將使用過的coupons撈出
                foreach (var couponIDListStringtemp in couponIDListString)
                {
                    // coupons可以為兩張以上,使用','分開
                    eachcouponIDListString = couponIDListStringtemp.Split(',').ToList();
                    foreach (var eachCouponIDListStringtemp in eachcouponIDListString)
                    {
                        // 判斷是否有coupons存在
                        if (eachCouponIDListStringtemp != null && eachCouponIDListStringtemp.Trim() != "")
                        {
                            couponIDListInt.Add((Int32.Parse(eachCouponIDListStringtemp)));
                        }
                    }
                }
                // 將所有使用的coupons撈出
                coupons = dbbefore.Coupon.Where(x => couponIDListInt.Contains(x.id)).ToList();
                //coupons = Processor.Request<List<TWNewEgg.Models.ViewModels.Redeem.Coupon>, List<TWNewEgg.Models.DomainModels.Redeem.Coupon>>("Service.CouponService.CouponServiceRepository", "GetCouponByIdList", couponIDListInt).results;

                logger.Debug("end of data query from db, salesorder " + ReturnPost.Code);

                // 在正式環境對美蛋拋送訂單取消
                if (System.Configuration.ConfigurationManager.AppSettings["Environment"] == "PRD")
                {
                    //NeweggRequestCancel(salesorder_code, logger, poitems);
                }

                // 取消單生成退款單 SO Group 為單位
                if (carts != null && carts.Count() > 0)
                {
                    logger.Debug("start generating refund2c, salesorder " + ReturnPost.Code);
                    // 判斷是否產生退款單,貨到付款取消不產生
                    if (salesOrders[0].PayType != 31)
                    {
                        // 退款單
                        refund2c refund2c = new refund2c();
                        // 信用卡付款資料
                        CreditAuth creditAuth = new CreditAuth();
                        // SOID
                        string salesOrdersIDtemp = salesOrders[0].Code;
                        // 將信用卡刷卡資料撈出
                        creditAuth = dbafter.CreditAuth.Where(x => x.OrderNO == salesOrdersIDtemp || x.SalesOrderGroupID == (sogroup ?? 0)).FirstOrDefault();

                        // 若信用卡資訊存在
                        if (creditAuth != null)
                        {
                            // 判斷是否付款成功
                            if (creditAuth.SuccessFlag == "1")
                            {
                                // 產生退款單
                                refund2c = Creatrefund2c(carts, processes, ReturnPost.bankid, ReturnPost.bankbranch, ReturnPost.Bankaccount, "取消-" + return_reasontext,return_reasonval);
                            }
                        }
                        else
                        {
                            // 若信用卡資訊不存,產生退款單
                            refund2c = Creatrefund2c(carts, processes, ReturnPost.bankid, ReturnPost.bankbranch, ReturnPost.Bankaccount, "取消-" + return_reasontext,return_reasonval);
                        }

                    }
                }

                // 產生取消單
                List<SalesOrderCancel> CreatSalesOrderList = CreatSalesOrderCancel(carts, processes, ReturnPost.bankid, ReturnPost.bankbranch, ReturnPost.Bankaccount, return_reasontext);
                //List<TWNewEgg.Models.ViewModels.Redeem.PromotionGiftRecords> PromotionGiftRecordsList = dbbefore.PromotionGiftRecords.Where(x => pocodes.Contains(x.SalesOrderItemCode)).ToList();
                //List<TWNewEgg.Models.ViewModels.Redeem.PromotionGiftRecords> PromotionGiftRecordsList = Processor.Request<List<TWNewEgg.Models.ViewModels.Redeem.PromotionGiftRecords>, List<TWNewEgg.Models.DomainModels.Redeem.PromotionGiftRecords>>("Service.PromotionGiftService.PromotionGiftRecordRepository", "GetGiftRecordsByListSalesOrderItemCode", pocodes).results;

                //Avoid the difference time cause database error,check again
                carts = dbafter.Cart.Where(x => x.SalesorderGroupID == sogroup).ToList();
                // 將前後台主單取消
                foreach (var tempitem in carts)
                {
                    tempitem.Status = (int)TWNewEgg.DB.TWSQLDB.Models.SalesOrder.status.取消;
                }
                foreach (var tempitem in salesOrders)
                {
                    tempitem.Status = (int)TWNewEgg.DB.TWSQLDB.Models.SalesOrder.status.取消;
                }

                // 取消coupons使用狀態
                UpDataCoupon(salesOrderItemsIDstring, accid);
                try
                {
                    dbafter.SaveChanges();
                    dbbefore.SaveChanges();
                    // 回傳成功
                    ReturnPostAction.IsSuccess = true;
                    ReturnPostAction.Msg = "1";


                }
                catch (Exception e)
                {
                    ReturnPostAction.IsSuccess = false;
                    ReturnPostAction.Msg = "error";


                }
                // 送sellerprotal取消信
                string Environment = System.Configuration.ConfigurationManager.AppSettings["Environment"];
                if (System.Configuration.ConfigurationManager.AppSettings["Environment"] == "PRD")
                {
                    try
                    {
                        string sellerprotalIP = "";
                        try
                        {
                            sellerprotalIP = System.Configuration.ConfigurationManager.AppSettings["SellerProtalIP_" + Environment];
                        }
                        catch
                        {

                        }
                        TWNewEgg.Website.ECWeb.Service.NeweggRequest ProcessVoidOrderNumMailurlReq = new TWNewEgg.Website.ECWeb.Service.NeweggRequest();
                        string OrderNum = ReturnPost.Code;
                        string ProcessVoidOrderNumMailurlReqtemp = ProcessVoidOrderNumMailurlReq.Post<string>(sellerprotalIP + "shiftOrderNum/ProcessVoidOrderNumMail?OrderNum=" + ReturnPost.Code, null);
                    }
                    catch { }
                }
                // 計算退款通知信寄出嘗試次數
                int count = 0;
                // 給客服人員的訂單取消通知信
                // PO拋單異常造成失敗
                logger.Info("In t0 Parsing.....GeneraterViewPage：" + CreatSalesOrderList.FirstOrDefault().SalesorderCode);
                if (false)
                {
                    var TestXMLEXport = new TWNewEgg.InternalSendMail.Service.GeneratorView();
                    TestXMLEXport.GeneraterViewPage(CreatSalesOrderList, "InnerCancelSO", carts, salesOrders, "拋單失敗異常訂單取消",null);
                    //while (!Mail_InnerCancelSO(salesOrders, "拋單失敗異常訂單取消", bankid, branches, bankaccount))
                    //{
                    //    count++;
                    //    if (count >= 3)
                    //    {
                    //        logger.Error("Mail_InnerCancelSO not sent, salesorder " + salesorder_code);
                    //        break;
                    //    }
                    //}
                }
                else
                {
                    var TestXMLEXport = new TWNewEgg.InternalSendMail.Service.GeneratorView();
                    var GeneraterViewPageResult = TestXMLEXport.GeneraterViewPage(CreatSalesOrderList, "InnerCancelSO", carts, salesOrders, return_reasontext, return_reasonval);
                    TestXMLEXport.GeneraterViewPage(CreatSalesOrderList, "CancelSO", carts, salesOrders, return_reasontext, return_reasonval);
                    //// 使用者取消訂單
                    //List<SalesOrder> SalesOrdertemp = salesOrders.Where(x => x.DelivType == 2 || x.DelivType == 7 || x.DelivType == 8 || x.DelivType == 9).ToList();
                    //// 給SellerVendor客服的訂單取消通知信
                    //if (SalesOrdertemp.Count > 0 && SalesOrdertemp != null)
                    //{
                    //    while (!Mail_VendorCancelSO(SalesOrdertemp, reset_reasontext, bankid, branches, bankaccount))
                    //    {
                    //        count++;
                    //        if (count >= 3)
                    //        {
                    //            logger.Error("Mail_InnerCancelSO not sent, salesorder " + salesorder_code);
                    //            break;
                    //        }
                    //    }
                    //}
                    //// 給客服的訂單取消通知信
                    //while (!Mail_InnerCancelSO(salesOrders, reset_reasontext, bankid, branches, bankaccount))
                    //{
                    //    count++;
                    //    if (count >= 3)
                    //    {
                    //        logger.Error("Mail_InnerCancelSO not sent, salesorder " + salesorder_code);
                    //        break;
                    //    }
                    //}
                    //// 給使用者的訂單取消通知信
                    //while (!Mail_CancelSO(salesOrders))
                    //{
                    //    count++;
                    //    if (count >= 3)
                    //    {
                    //        logger.Error("Mail_CancelSO not sent, salesorder " + salesorder_code);
                    //        break;
                    //    }
                    //}
                }

                logger.Debug("Emails have been sent, salesorder " + ReturnPost.Code);

                //if (coupons!= null && coupons.Count() > 0)
                //{
                //    foreach (var Couponstemp in coupons)
                //    {
                //        Couponstemp.usestatus = (int)Coupon.CouponUsedStatusOption.UsedButCancel;
                //        Couponstemp.updatedate = DateTime.Now;
                //        Couponstemp.updateuser = Couponstemp.accountid;
                //        Couponstemp.note = Couponstemp.note + " " + DateTime.Now.ToString("yyyyMMdd-HHmmss") + "客戶取消訂單折價券作廢;";
                //    }
                //}
                //if (PromotionGiftRecordsList != null && PromotionGiftRecordsList.Count() > 0)
                //{
                //    foreach (var PromotionGiftRecordtemp in PromotionGiftRecordsList)
                //    {
                //        PromotionGiftRecordtemp.UsedStatus = (int)PromotionGiftRecords.UsedStatusOption.CancelUsed;
                //        PromotionGiftRecordtemp.UpdateDate = DateTime.Now;
                //        PromotionGiftRecordtemp.UpdateUser = accID.ToString();
                //    }
                //}
            }
         
            return ReturnPostAction;
        
        }
        /// <summary>
        /// 訂單退貨Post, status = 5 for the return
        /// </summary>
        /// <param name="salesorder_code">訂單編碼</param>
        /// <param name="return_reasonval">退貨原因</param>
        /// <param name="return_reasontext">退貨描述</param>
        /// <param name="item_status">商品狀態</param>
        /// <param name="name">商品名稱</param>
        /// <param name="email">訂購人email</param>
        /// <param name="address1">訂購人郵遞區號</param>
        /// <param name="address2">訂購人縣市</param>
        /// <param name="address3">訂購人地址</param>
        /// <param name="tel">訂購人電話</param>
        /// <param name="mobile">訂購人手機</param>
        /// <param name="bankid">退款銀行ID</param>
        /// <param name="branches">退款銀行分行</param>
        /// <param name="bankaccount">退款銀行帳號</param>
        /// <returns>Json回傳執行結果</returns>
        [AcceptVerbs(HttpVerbs.Post)]
        public TWNewEgg.Models.ViewModels.Redeem.ActionResponse<TWNewEgg.Models.ViewModels.MyAccount.ReturnPost> Returnpost(TWNewEgg.Models.ViewModels.MyAccount.ReturnPost ReturnPost, int? return_reasonval, string return_reasontext,int accid)
        {

            TWNewEgg.Models.ViewModels.Redeem.ActionResponse<TWNewEgg.Models.ViewModels.MyAccount.ReturnPost> ReturnPostActionResponse = new TWNewEgg.Models.ViewModels.Redeem.ActionResponse<TWNewEgg.Models.ViewModels.MyAccount.ReturnPost>();
            Thread.Sleep(500);
            log4net.ILog logger;
            logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.FullName);
            //aesenc = new AesCookies();

            // retgoodList
            List<Retgood> retgoodList = new List<Retgood>();
            // 判斷是否登入,沒登入返回登入頁面
            //int accID = CheckAccount();
            // RetgoodMODEL
            Retgood retgood = new Retgood();
            // SalesOrderMODEL
            SalesOrder salesOrder = dbbefore.SalesOrder.Where(x => x.Code == ReturnPost.Code).FirstOrDefault();
            // SalesOrderitemMODEL
            Process salesOrderitem = dbafter.Process.Where(x => x.CartID == ReturnPost.Code).FirstOrDefault();
            // CartMODEL
            TWNewEgg.DB.TWBACKENDDB.Models.Cart cart = dbafter.Cart.Where(x => x.ID == ReturnPost.Code).FirstOrDefault();
            // CartMODEL
            List<TWNewEgg.DB.TWBACKENDDB.Models.Cart> CartList = new List<TWNewEgg.DB.TWBACKENDDB.Models.Cart>();
            List<SalesOrder> SalesOrderList = new List<SalesOrder>();
            CartList.Add(cart);
            // SO子單List
            List<Process> ProcessList = dbafter.Process.Where(x => x.CartID == ReturnPost.Code && x.ProductID != 13189 && x.ProductID != 13190).ToList();
            // 退貨單
            retgood = dbafter.Retgood.Where(x => x.ProcessID == salesOrderitem.ID).FirstOrDefault();
            // 判斷此訂單是否退訂過
            string salesorder_codeold = dbafter.Retgood.Where(x => x.CartID == ReturnPost.Code).Select(x => x.CartID).FirstOrDefault();
            int count = 0;
            ReturnPost.Receiver = salesOrder.RecvName;
            // 判斷此退貨單是否存在
            if (retgood != null || salesorder_codeold != null)

            {
                ReturnPostActionResponse.IsSuccess = false;
                ReturnPostActionResponse.Msg = "Has_returned";
                return ReturnPostActionResponse;
            }

            //// 判斷登入帳號是否正確
            //if (salesOrder.AccountID != accID)
            //{
            //    return Json("AccidError");
            //}

            // 是否配達
            if (cart.DelvStatus != 2)
            {

                ReturnPostActionResponse.IsSuccess = false;
                ReturnPostActionResponse.Msg = "error";
                return ReturnPostActionResponse;
          
            }
            else
            {
                if (ProcessList.Sum(x => x.ApportionedAmount) == 0m)
                {
                    // 判斷訂單是否存在,若存在,將狀態改為退貨
                    if (salesOrder != null)
                    {
                        salesOrder.Status = (int)TWNewEgg.DB.TWSQLDB.Models.SalesOrder.status.退貨;
                    }

                    // 判斷後台訂單是否存在,若存在,將狀態改為退貨
                    if (cart != null)
                    {
                        cart.Status = (int)TWNewEgg.DB.TWBACKENDDB.Models.Cart.status.退貨;
                    }
                    // 產生退貨單
                    retgoodList = CreatRetgood(ReturnPost, return_reasonval, return_reasontext);
                    Send_retgood(ReturnPost, retgoodList, CartList, SalesOrderList, return_reasontext);
                }
                else
                {
                    int SalesorderGroupID = cart.SalesorderGroupID ?? 0;
                    CartList = dbafter.Cart.Where(x => x.SalesorderGroupID == SalesorderGroupID).ToList();
                    SalesOrderList = dbbefore.SalesOrder.Where(x => x.SalesOrderGroupID == SalesorderGroupID).ToList();
                    List<string> CartCodeList = CartList.Select(x => x.ID).ToList();
                    ProcessList = dbafter.Process.Where(x => CartCodeList.Contains(x.CartID)).ToList();
                    foreach (var Cartitem in CartList)
                    {
                        List<Retgood> retgoodListtemp = new List<Retgood>();
                        List<Process> ProcessListtemp = ProcessList.Where(x => x.CartID == Cartitem.ID).ToList();
                        List<TWNewEgg.DB.TWBACKENDDB.Models.Cart> CartListtemp = new List<TWNewEgg.DB.TWBACKENDDB.Models.Cart>();
                        CartListtemp.Add(Cartitem);
                        if (ProcessList.Sum(x => x.ApportionedAmount) != 0m)
                        {
                            if (Cartitem.Status != (int)TWNewEgg.DB.TWBACKENDDB.Models.Cart.status.退貨 && (Cartitem.DelvStatus == 1 || Cartitem.DelvStatus == 2 || Cartitem.DelvStatus == 5))
                            {
                                ReturnPost.Code = Cartitem.ID;
                                retgoodListtemp = CreatRetgood(ReturnPost, return_reasonval, return_reasontext);
                                Cartitem.Status = (int)TWNewEgg.DB.TWBACKENDDB.Models.Cart.status.退貨;
                                SalesOrderList.Where(x => x.Code == Cartitem.ID).FirstOrDefault().Status = (int)TWNewEgg.DB.TWBACKENDDB.Models.Cart.status.退貨;
                             
                                retgoodList.Add(retgoodListtemp.FirstOrDefault());
                            }
                            else if (Cartitem.Status != (int)TWNewEgg.DB.TWBACKENDDB.Models.Cart.status.退貨 && Cartitem.Status != (int)TWNewEgg.DB.TWBACKENDDB.Models.Cart.status.取消)
                            {
                                string CartitemCode = Cartitem.ID;
                                List<SalesOrder> SalesOrderListtemp = dbbefore.SalesOrder.Where(x => x.Code == CartitemCode).ToList();
                                List<SalesOrderItem> SalesOrderItemListtemp = dbbefore.SalesOrderItem.Where(x => x.SalesorderCode == CartitemCode).ToList();
                                CreatCancel(CartListtemp, ProcessListtemp, SalesOrderListtemp, SalesOrderItemListtemp, "其它共同活動參與商品退貨-" + return_reasontext,return_reasonval, ReturnPost.bankid, ReturnPost.bankbranch, ReturnPost.Bankaccount);
                                Cartitem.Status = (int)TWNewEgg.DB.TWBACKENDDB.Models.Cart.status.取消;
                                SalesOrderList.Where(x => x.Code == Cartitem.ID).FirstOrDefault().Status = (int)TWNewEgg.DB.TWBACKENDDB.Models.Cart.status.取消;
                            }
                        }
                        dbafter.SaveChanges();
                        List<TWNewEgg.DB.TWBACKENDDB.Models.Cart> CartInfo = new List<DB.TWBACKENDDB.Models.Cart>();
                        CartInfo.Add(Cartitem);
                        List<SalesOrder> SalesOrderforCart = SalesOrderList.Where(x => x.Code == Cartitem.ID).ToList();

                        Send_retgood(ReturnPost, retgoodListtemp, CartInfo, SalesOrderforCart, return_reasontext);
                    }
                }

                List<string> ProcessListString = ProcessList.Select(x => x.ID).ToList();
                UpDataCoupon(ProcessListString, accid);

                Retgood retgoodmain = new Retgood();
                retgoodmain = retgoodList.FirstOrDefault();

                try
                {
                    dbbefore.SaveChanges();
                    dbafter.SaveChanges();
                   
                    ReturnPostActionResponse.IsSuccess = true;                                  
                }
                catch (InvalidCastException e)
                {
                    ReturnPostActionResponse.IsSuccess = false;
                    ReturnPostActionResponse.Msg = e.InnerException.StackTrace;                              
                }

                return ReturnPostActionResponse;

            }
        }
        /// <summary>
        /// 折價券狀態改變
        /// </summary>
        /// <param name="processesList"></param>
        /// <returns></returns>
        public void UpDataCoupon(List<string> processesList, int accID)
        {
            List<TWNewEgg.DB.TWSQLDB.Models.Coupon> CouponList = dbbefore.Coupon.Where(x => processesList.Contains(x.ordcode)).ToList();
            //List<TWNewEgg.Models.ViewModels.Redeem.Coupon> CouponList = Processor.Request<List<TWNewEgg.Models.ViewModels.Redeem.Coupon>, List<TWNewEgg.Models.DomainModels.Redeem.Coupon>>("Service.CouponService.CouponServiceRepository", "GetCouponByOrdCode", processesList).results;
            List<TWNewEgg.DB.TWSQLDB.Models.PromotionGiftRecords> PromotionGiftRecordsList = dbbefore.PromotionGiftRecords.Where(x => processesList.Contains(x.SalesOrderItemCode)).ToList();
            //List<TWNewEgg.Models.ViewModels.Redeem.PromotionGiftRecords> PromotionGiftRecordsList = Processor.Request<List<TWNewEgg.Models.ViewModels.Redeem.PromotionGiftRecords>, List<TWNewEgg.Models.DomainModels.Redeem.PromotionGiftRecords>>("Service.PromotionGiftService.PromotionGiftRecordRepository", "GetGiftRecordsByListSalesOrderItemCode", processesList).results;

            if (CouponList != null && CouponList.Count() > 0)
            {
                foreach (var Couponstemp in CouponList)
                {
                    Couponstemp.usestatus = (int)TWNewEgg.Models.ViewModels.Redeem.Coupon.CouponUsedStatusOption.UsedButCancel;
                    Couponstemp.updatedate = DateTime.UtcNow.AddHours(8);
                    Couponstemp.updateuser = Couponstemp.accountid;
                    Couponstemp.note = Couponstemp.note + " " + DateTime.UtcNow.AddHours(8).ToString("yyyyMMdd-HHmmss") + "客戶取消訂單折價券作廢;";
                }
            }
            if (PromotionGiftRecordsList != null && PromotionGiftRecordsList.Count() > 0)
            {
                foreach (var PromotionGiftRecordtemp in PromotionGiftRecordsList)
                {
                    PromotionGiftRecordtemp.UsedStatus = (int)TWNewEgg.Models.ViewModels.Redeem.PromotionGiftRecords.UsedStatusOption.CancelUsed;
                    PromotionGiftRecordtemp.UpdateDate = DateTime.UtcNow.AddHours(8);
                    PromotionGiftRecordtemp.UpdateUser = accID.ToString();
                }
            }
            dbbefore.SaveChanges();
        }

        /// <summary>
        /// 建立退貨單
        /// </summary>
        /// <param name="carts"></param>
        /// <param name="Processes"></param>
        /// <param name="bankid"></param>
        /// <param name="branches"></param>
        /// <param name="bankaccount"></param>
        /// <param name="reset_reasontext"></param>
        /// <returns></returns>
        public List<Retgood> CreatRetgood(TWNewEgg.Models.ViewModels.MyAccount.ReturnPost ReturnPost, int? return_reasonval, string return_reasontext) 
        {

            List< TWNewEgg.DB.TWBACKENDDB.Models.Cart> carts = dbafter.Cart.Where(x => x.ID == ReturnPost.Code).ToList();
            List<Process> Process = dbafter.Process.Where(x => x.CartID == ReturnPost.Code).ToList();
            List<Retgood> retgoodList = new List<Retgood>();

            Retgood retgoodmain = new Retgood();
            // 退貨人
            retgoodmain.AccountName = ReturnPost.Username;
            // 退貨原因編碼
            retgoodmain.Cause = (int)return_reasonval;
            // 退貨原因
            retgoodmain.CauseNote = return_reasontext;
            // 商品狀態
            retgoodmain.DealNote = "無";
            // 商品屬性0一般, 10屬性, 20贈品
            retgoodmain.RetgoodType = 0;
            // 收件人
            retgoodmain.FrmName = ReturnPost.Receiver;
            // 收件人email
            retgoodmain.FrmEmail = ReturnPost.Email;
            // 收件人電話
            retgoodmain.FrmPhone = ReturnPost.Phone;
            retgoodmain.FrmMobile = ReturnPost.RecvMobile;
            // 收件人地址
            retgoodmain.FrmLocation = ReturnPost.Location;
            retgoodmain.FrmZipcode = ReturnPost.Zipcode.Split(' ')[0];
            retgoodmain.FrmADDR = ReturnPost.Zipcode+" "+ ReturnPost.ADDR;
            // 退貨單號
            retgoodmain.Code = GetRegoodNO("Regood");
            retgoodmain.Price = Convert.ToDecimal(CalculationAmount(carts, Process));
            // 退貨品項
            retgoodmain.ProductID = Process.Where(x => x.ProductID != 13189 && x.ProductID != 13190 && x.CartID == ReturnPost.Code).FirstOrDefault().ProductID;
            // 退貨SO
            retgoodmain.CartID = ReturnPost.Code;
            // 退貨SOitem
            retgoodmain.ProcessID = Process.Where(x => x.ProductID != 13189 && x.ProductID != 13190 && x.CartID == ReturnPost.Code).FirstOrDefault().ID;
            // 退貨數量
            retgoodmain.Qty = Process.Where(x => x.ProductID != 13189 && x.ProductID != 13190 && x.CartID == ReturnPost.Code).Select(x => x.Qty ?? 0).Sum();
            // 退貨庫存代號
            retgoodmain.StockOutItemID = dbbefore.ItemStock.Where(x => x.ProductID == retgoodmain.ProductID).Select(x => x.ID).FirstOrDefault();
            // 退貨seller
            retgoodmain.SupplierID = dbbefore.Product.Where(x => x.ID == retgoodmain.ProductID).FirstOrDefault().SellerID;
            retgoodmain.Date = DateTime.UtcNow.AddHours(8);
            // 退貨初始狀態
            retgoodmain.Status = 0;
            // 退款銀行代號
            retgoodmain.BankName = ReturnPost.bankid.Replace("請選擇","");
            // 退款分行
            retgoodmain.BankBranch = ReturnPost.bankbranch;
            // 退款帳號
            retgoodmain.AccountNO = ReturnPost.Bankaccount.ToString();
            retgoodmain.UpdateNote = DateTime.UtcNow.AddHours(8) + "　退貨單成立　;<br>";
            retgoodList.Add(retgoodmain);
            dbafter.Retgood.Add(retgoodmain);
            dbafter.SaveChanges();
            Processor.Request<int, int>("Controllers.ShiftOrderNumController","ProcessRMANumMail", ReturnPost.Code);
            return retgoodList;
        }
        public string GetRegoodNO(string Retype)
        {
            //List<GetItemTaxDetail> ItemTaxDetail = new List<GetItemTaxDetail>();
            TWSqlDBContext db_SP = new TWSqlDBContext();


            db_SP.Database.Initialize(force: false);
            var cmd = db_SP.Database.Connection.CreateCommand();

            if (Retype == "Regood")
            {
                cmd.CommandText = "exec [dbo].[UP_EC_RegoodGetNumber]";
            }
            else if (Retype == "Refund2c")
            {
                cmd.CommandText = "exec [dbo].[UP_EC_Refund2cGetNumber]";
            }

            try
            {
                db_SP.Database.Connection.Open();
                var reader = cmd.ExecuteReader();

                TWNewEgg.Website.ECWeb.Models.DbQuery nDb = null;
                DataSet NdsResult = null; //getSalesOrderNumByDate
                nDb = new TWNewEgg.Website.ECWeb.Models.DbQuery();

                NdsResult = nDb.Query(cmd.CommandText);

                DataTable NdtItem = null; //getSalesOrderNumByDate
                string RetypeNO = null;
                if (NdsResult != null && NdsResult.Tables.Count > 0)
                {
                    NdtItem = NdsResult.Tables[0];

                    RetypeNO = Convert.ToString(NdtItem.Rows[0][0]);
                    /*foreach (DataRow dr in NdtItem.Rows)
                    {
                        RetypeNO = Convert.ToString(dr[0]);
                    }//end foreach*/
                }//end if (dsResult != null && dsResult.Tables.Count > 0)

                return RetypeNO;

            }
            catch (Exception e)
            {
                return null;
            }
        }

        /// <summary>
        /// 計算退款金額
        /// </summary>
        /// <param name="carts">cartsList</param>
        /// <param name="Processes">processesList</param>
        /// <returns>退款金額</returns>
        public int CalculationAmount(List<TWNewEgg.DB.TWBACKENDDB.Models.Cart> carts, List<Process> processes)
        {
            int Amount = 0;
            for (int i = 0; i < carts.Count(); i++)
            {
                //if (!new List<int>() { 31, 32 }.Contains(carts[i].PayType ?? 0))
                //{
                List<Process> Processlist = processes.Where(x => x.CartID == carts[i].ID).ToList();
                foreach (var proc in Processlist)
                {
                    if (proc.ProductID != 13189 && proc.ProductID != 13190)
                    {
                        Amount += (int)(proc.Price ?? 0);
                        Amount += (int)(proc.ShippingExpense ?? 0);
                        Amount += (int)(proc.ServiceExpense ?? 0);
                        Amount += (int)(proc.InstallmentFee);
                        Amount -= (int)(Math.Abs(proc.Pricecoupon ?? 0));
                        Amount -= (int)(Math.Abs(proc.ApportionedAmount));
                    }
                }
                //}
            }
            return Amount;
        }
        /// <summary>
        /// 取消訂單用API
        /// </summary>
        /// <param name="salesorder_code">訂單編號</param>
        /// <param name="reset_reasontext">取消原因描述</param>
        /// <param name="bankid">銀行ID</param>
        /// <param name="branches">銀行分行</param>
        /// <param name="bankaccount">銀行帳號</param>
        /// <param name="passive">??</param>
        /// <param name="password">密碼輸入??</param>
        /// <returns>Json成功失敗</returns>
        public int CreatCancel(List<TWNewEgg.DB.TWBACKENDDB.Models.Cart> CartList, List<Process> Processes, List<SalesOrder> SalesOrders, List<SalesOrderItem> SalesOrderItems, string reset_reasontext,int? return_reasonval, string bankid, string branches, string bankaccount)
        {
            log4net.ILog logger;
            logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.FullName);
            List<string> ProcessesIDstringList = Processes.Select(x => x.ID).ToList();
            refund2c refund2ctemp = dbafter.refund2c.Where(x => ProcessesIDstringList.Contains(x.ProcessID)).FirstOrDefault();

            if (refund2ctemp == null)
            {
                // 判斷字串取消原因描述是否過長,若過長則自動截斷
                if (reset_reasontext.Length > 240)
                {
                    reset_reasontext = reset_reasontext.Substring(0, 239);
                }

                // 帳號
                string AccountID = CartList.FirstOrDefault().UserID;
                // 撈取取消單資料
                string SalesorderCodetemp = CartList.FirstOrDefault().ID;
                List<SalesOrderCancel> salesOrderCancellist = dbbefore.SalesOrderCancel.Where(x => x.SalesorderCode == SalesorderCodetemp).ToList();
                // SOGroupID
                int? sogroup = CartList.FirstOrDefault().SalesorderGroupID;
                // POCodesList SOGroupID底下後台POList
                List<string> pocodes = dbbefore.PurchaseOrder.Where(p => p.SalesorderCode == SalesorderCodetemp).Select(p => p.Code).ToList();
                // POItemsList SOGroupID底下後台POitemList
                List<PurchaseOrderItem> poitems = dbbefore.PurchaseOrderItem.Where(p => pocodes.Contains(p.PurchaseorderCode) && p.SellerOrderCode != null && p.SellerOrderCode.Trim() != "").ToList();
                // SalesorderCodeList SOGroupID底下前台SOIDList
                List<string> salesorderCodes = SalesOrders.Select(s => s.Code).ToList();
                // CouponsList
                List<TWNewEgg.DB.TWSQLDB.Models.Coupon> coupons;
                // CouponIDList
                List<string> couponIDListString;
                // eachCouponIDList
                List<string> eachcouponIDListString;
                // CouponIDList to Int
                List<int> couponIDListInt = new List<int>();

                // 判斷SOitemList數量(是否轉單)
                if (Processes.Count > 0)
                {
                    couponIDListString = Processes.Select(x => x.Coupons).ToList();
                }
                else
                {
                    couponIDListString = SalesOrderItems.Select(x => x.Coupons).ToList();
                }

                // 為改變coupons狀態為使用取消,將使用過的coupons撈出
                foreach (var couponIDListStringtemp in couponIDListString)
                {
                    // coupons可以為兩張以上,使用','分開
                    eachcouponIDListString = couponIDListStringtemp.Split(',').ToList();
                    foreach (var eachCouponIDListStringtemp in eachcouponIDListString)
                    {
                        // 判斷是否有coupons存在
                        if (eachCouponIDListStringtemp != null && eachCouponIDListStringtemp.Trim() != "")
                        {
                            couponIDListInt.Add((Int32.Parse(eachCouponIDListStringtemp)));
                        }
                    }
                }
                // 將所有使用的coupons撈出
                coupons = dbbefore.Coupon.Where(x => couponIDListInt.Contains(x.id)).ToList();
                //coupons = Processor.Request<List<TWNewEgg.Models.ViewModels.Redeem.Coupon>, List<TWNewEgg.Models.DomainModels.Redeem.Coupon>>("Service.CouponService.CouponServiceRepository", "GetCouponByIdList", couponIDListInt).results;

                logger.Debug("end of data query from db, salesorder " + SalesorderCodetemp);

                // 在正式環境對美蛋拋送訂單取消
                if (System.Configuration.ConfigurationManager.AppSettings["Environment"] == "PRD")
                {
                    NeweggRequestCancel(SalesorderCodetemp, logger, poitems);
                }

                // 取消單生成退款單 SO Group 為單位
                if (CartList != null && CartList.Count() > 0)
                {
                    logger.Debug("start generating refund2c, salesorder " + SalesorderCodetemp);
                    // 判斷是否產生退款單,貨到付款取消不產生
                    if (SalesOrders[0].PayType != 31)
                    {
                        // 退款單
                        refund2c refund2c = new refund2c();
                        // 信用卡付款資料
                        CreditAuth creditAuth = new CreditAuth();
                        // SOID
                        string salesOrdersIDtemp = SalesOrders[0].Code;
                        // 將信用卡刷卡資料撈出
                        creditAuth = dbafter.CreditAuth.Where(x => x.OrderNO == salesOrdersIDtemp || x.SalesOrderGroupID == (sogroup ?? 0)).FirstOrDefault();

                        // 若信用卡資訊存在
                        if (creditAuth != null)
                        {
                            // 判斷是否付款成功
                            if (creditAuth.SuccessFlag == "1")
                            {
                                // 產生退款單
                                refund2c = Creatrefund2c(CartList, Processes, bankid, branches, bankaccount, "取消-" + reset_reasontext,return_reasonval);
                            }
                        }
                        else
                        {
                            // 若信用卡資訊不存,產生退款單
                            refund2c = Creatrefund2c(CartList, Processes, bankid, branches, bankaccount, "取消-" + reset_reasontext, return_reasonval);
                        }

                    }
                }

                // 產生取消單
                List<TWNewEgg.DB.TWSQLDB.Models.SalesOrderCancel> CreatSalesOrderList = CreatSalesOrderCancel(CartList, Processes, bankid, branches, bankaccount, reset_reasontext);
                List<TWNewEgg.DB.TWSQLDB.Models.PromotionGiftRecords> PromotionGiftRecordsList = dbbefore.PromotionGiftRecords.Where(x => pocodes.Contains(x.SalesOrderItemCode)).ToList();
                //List<TWNewEgg.Models.ViewModels.Redeem.PromotionGiftRecords> PromotionGiftRecordsList = Processor.Request<List<TWNewEgg.Models.ViewModels.Redeem.PromotionGiftRecords>, List<TWNewEgg.Models.DomainModels.Redeem.PromotionGiftRecords>>("Service.PromotionGiftService.PromotionGiftRecordRepository", "GetGiftRecordsByListSalesOrderItemCode", pocodes).results;

                // 計算退款通知信寄出嘗試次數
                int count = 0;
                List<SalesOrder> SalesOrdertemp = SalesOrders.Where(x => x.DelivType == 2 || x.DelivType == 7 || x.DelivType == 8 || x.DelivType == 9).ToList();
                var TestXMLEXport = new TWNewEgg.InternalSendMail.Service.GeneratorView();
                TestXMLEXport.GeneraterViewPage(CreatSalesOrderList, "InnerCancelSO", CartList, SalesOrders, reset_reasontext, return_reasonval);
                TestXMLEXport.GeneraterViewPage(CreatSalesOrderList, "CancelSO", CartList, SalesOrders, reset_reasontext,return_reasonval);

                //// 給SellerVendor客服的訂單取消通知信
                //if (SalesOrdertemp.Count > 0 && SalesOrdertemp != null)
                //{
                //    while (!Mail_VendorCancelSO(SalesOrdertemp, reset_reasontext, bankid, branches, bankaccount))
                //    {
                //        count++;
                //        if (count >= 3)
                //        {
                //            logger.Error("Mail_InnerCancelSO not sent, salesorder " + SalesorderCodetemp);
                //            break;
                //        }
                //    }
                //}
                //// 給客服的訂單取消通知信
                //while (!Mail_InnerCancelSO(SalesOrders, reset_reasontext, bankid, branches, bankaccount))
                //{
                //    count++;
                //    if (count >= 3)
                //    {
                //        logger.Error("Mail_InnerCancelSO not sent, salesorder " + SalesorderCodetemp);
                //        break;
                //    }
                //}
                //// 給使用者的訂單取消通知信
                //while (!Mail_CancelSO(SalesOrders))
                //{
                //    count++;
                //    if (count >= 3)
                //    {
                //        logger.Error("Mail_CancelSO not sent, salesorder " + SalesorderCodetemp);
                //        break;
                //    }
                //}

                logger.Debug("Emails have been sent, salesorder " + SalesorderCodetemp);



                // 將前後台主單取消
                foreach (var tempitem in CartList)
                {
                    tempitem.Status = (int)TWNewEgg.DB.TWSQLDB.Models.SalesOrder.status.取消;
                }
                foreach (var tempitem in SalesOrders)
                {
                    tempitem.Status = (int)TWNewEgg.DB.TWSQLDB.Models.SalesOrder.status.取消;
                }
                // 取消coupons使用狀態
                if (coupons != null && coupons.Count() > 0)
                {
                    foreach (var Couponstemp in coupons)
                    {
                        Couponstemp.usestatus = (int)TWNewEgg.Models.ViewModels.Redeem.Coupon.CouponUsedStatusOption.UsedButCancel;
                        Couponstemp.updatedate = DateTime.UtcNow.AddHours(8);
                        Couponstemp.updateuser = Couponstemp.accountid;
                        Couponstemp.note = Couponstemp.note + " " + DateTime.UtcNow.AddHours(8).ToString("yyyyMMdd-HHmmss") + "客戶取消訂單折價券作廢;";
                    }
                }
                if (PromotionGiftRecordsList != null && PromotionGiftRecordsList.Count() > 0)
                {
                    foreach (var PromotionGiftRecordtemp in PromotionGiftRecordsList)
                    {
                        PromotionGiftRecordtemp.UsedStatus = (int)TWNewEgg.Models.ViewModels.Redeem.PromotionGiftRecords.UsedStatusOption.CancelUsed;
                        PromotionGiftRecordtemp.UpdateDate = DateTime.UtcNow.AddHours(8);
                        PromotionGiftRecordtemp.UpdateUser = AccountID.ToString();
                    }
                }

                try
                {
                    dbafter.SaveChanges();
                    dbbefore.SaveChanges();
                    // 回傳成功
                    return 1;
                }
                catch (Exception e)
                {
                    logger.Error("exception caught", e);
                    return 0;
                }
            }
            else
            {
                logger.Error("exception caught:退款單已產生過");
                return 0;
            }
        }
        /// <summary>
        /// 建立退款單
        /// </summary>
        /// <param name="carts">cartsList</param>
        /// <param name="Processes">processesList</param>
        /// <param name="bankid">銀行ID</param>
        /// <param name="branches">分行ID</param>
        /// <param name="bankaccount">退款帳號</param>
        /// <param name="reset_reasontext">取消原因</param>
        /// <returns>refund2c model</returns>
        public refund2c Creatrefund2c(List<TWNewEgg.DB.TWBACKENDDB.Models.Cart> carts, List<Process> processes, string bankid, string branches, string bankaccount, string reset_reasontext,int? return_reasonval)
        {
            refund2c refund2c = new refund2c();
            List<string> cartsIDs = carts.Select(x => x.ID).ToList();

            // 將子單金額加總
            refund2c.Amount = CalculationAmount(carts, processes);
            // 退款單編號
            refund2c.Code = GetRegoodNO("Refund2c");
            refund2c.AccountName = carts[0].Username;
            refund2c.BankID = bankid;
            refund2c.BankName = bankid;
            refund2c.SubBankName = branches;
            refund2c.AccountNO = bankaccount;
            refund2c.PayDate = carts[0].CreateDate;
            refund2c.Status = 98;
            refund2c.StatusDate = DateTime.UtcNow.AddHours(8);
            refund2c.Cause = return_reasonval;
            refund2c.CauseNote = reset_reasontext;
            refund2c.ApplyDate = DateTime.UtcNow.AddHours(8);
            refund2c.CreateDate = DateTime.UtcNow.AddHours(8);
            refund2c.Date = DateTime.UtcNow.AddHours(8);
            refund2c.UpdateNote = DateTime.UtcNow.AddHours(8) + "　退款單成立　;<br>";
            refund2c.ProcessID = processes.OrderBy(p => p.ID).First().ID;
            refund2c.CartID = carts[0].ID;
            // 判斷是否開立發票
            if (carts.FirstOrDefault().InvoiceNO == null || carts.FirstOrDefault().InvoiceNO == "")
            {
                refund2c.InvoiceResult = (int)TWNewEgg.DB.TWBACKENDDB.Models.refund2c.InvoiceResult_Status.未開立發票;
            }

            // 判斷金額大於0產生退款單
            if (refund2c.Amount > 0)
            {
                List<refund2c> refund2ctemp = dbafter.refund2c.Where(x => cartsIDs.Contains(x.CartID)).ToList();
                // 判斷使否產生過退款單
                if (refund2ctemp.Count == 0)
                {
                    dbafter.refund2c.Add(refund2c);
                    dbafter.SaveChanges();
                }
                else
                {
                    refund2c = null;
                }
            }
            return refund2c;
        }
        /// <summary>
        /// 拋送訂單取消
        /// </summary>
        /// <param name="salesorder_code"></param>
        /// <param name="logger"></param>
        /// <param name="POItems"></param>
        private void NeweggRequestCancel(string salesorder_code, log4net.ILog logger, List<PurchaseOrderItem> POItems)
        {
            try
            {
                if (System.Configuration.ConfigurationManager.AppSettings["Environment"] == "PRD")
                {
                    NeweggRequest neweggRequest = new NeweggRequest();
                    neweggRequest.Login();
                    foreach (var poi in POItems)
                    {
                        Message<UIOrderHistoryContent> OrderHistory;
                        logger.Debug("start taking OrderHistory of purchaseorderitem " + poi.Code + ", salesorder " + salesorder_code);
                        try
                        {
                            OrderHistory = neweggRequest.GetOrderHistory(poi.SellerOrderCode, 1);
                            if (OrderHistory == null || OrderHistory.Body == null || OrderHistory.Body.OrderSummaryList == null)
                            {
                                logger.Debug("can't find " + poi.Code + " with NeweggRequest, who's sellerordercode is " + poi.SellerOrderCode);
                                continue;
                            }
                        }
                        catch (NullReferenceException e)
                        {
                            logger.Error("error when taking OrderHistroy with NeweggRequest, Salesordercode = " + salesorder_code + ", sellerordercode is " + poi.SellerOrderCode, e);
                            throw new Exception("error when taking OrderHistroy with NeweggRequest, Salesordercode = " + salesorder_code);
                        }

                        var order = OrderHistory.Body.OrderSummaryList.SingleOrDefault(s => s.SONumber == Convert.ToInt32(poi.SellerOrderCode));
                        if (order == null)
                        {
                            logger.Debug("can't find " + poi.Code + " with NeweggRequest, who's sellerordercode is " + poi.SellerOrderCode);
                        }
                        else
                        {
                            logger.Debug("got ordersumarryList of " + poi.Code);
                            //不做是否可取消的預先判斷，一律拋送取消請求，交由API判斷取消是否成功
                            /*if (order.IsShowCancelButton)
                            {*/
                            logger.Debug("start sending CancelOrder request of " + poi.Code + " with NeweggRequest");
                            var result = neweggRequest.CancelOrder(Convert.ToInt32(poi.SellerOrderCode));
                            if (result != null)
                            {
                                //PO中寫入註記 (時間+ResponseCode)
                                try
                                {
                                    //update TWSQLDB
                                    DB.TWSqlDBContext db = new DB.TWSqlDBContext();
                                    var po = db.PurchaseOrder.Where(x => x.Code == poi.PurchaseorderCode).FirstOrDefault();
                                    po.Note2 += DateTime.UtcNow.AddHours(8).ToString() + "+" + result.Code;
                                    db.SaveChanges();
                                }
                                catch (Exception e)
                                {
                                    logger.Error(e.Message);
                                }
                                try
                                {
                                    //update TWBACKENDDB
                                    DB.TWBackendDBContext backendDB = new DB.TWBackendDBContext();
                                    var po = backendDB.PurchaseOrderTWBACK.Where(x => x.Code == poi.PurchaseorderCode).FirstOrDefault();
                                    po.Note2 += DateTime.UtcNow.AddHours(8).ToString() + "+" + result.Code;
                                    backendDB.SaveChanges();
                                }
                                catch (Exception e)
                                {
                                    logger.Error(e.Message);
                                }

                                //判斷是否取消成功
                                if (result.Code != "000")
                                {
                                    logger.Debug("Purchaseorderitem_code " + poi.Code + " NeweggRequest CancelOrder " + result.Code);
                                }
                                else
                                {
                                    logger.Debug("Purchaseorderitem_code " + poi.Code + " NeweggRequest CancelOrder 000");
                                    //取消成功_更新PO狀態
                                    try
                                    {
                                        //update TWSQLDB
                                        DB.TWSqlDBContext db = new DB.TWSqlDBContext();
                                        var po = db.PurchaseOrder.Where(x => x.Code == poi.PurchaseorderCode).FirstOrDefault();
                                        if (po != null)
                                        {
                                            po.Status = (int)DB.TWSQLDB.Models.PurchaseOrder.status.取消;
                                        }
                                        var tempPOI = db.PurchaseOrderItem.Where(x => x.Code == poi.Code).FirstOrDefault();
                                        if (tempPOI != null)
                                        {
                                            tempPOI.Status = (int)DB.TWSQLDB.Models.PurchaseOrderItem.status.取消;
                                        }
                                        db.SaveChanges();
                                    }
                                    catch (Exception e)
                                    {
                                        logger.Error(e.Message);
                                    }
                                    try
                                    {
                                        //update TWBACKENDDB
                                        DB.TWBackendDBContext backendDB = new DB.TWBackendDBContext();
                                        var po = backendDB.PurchaseOrderTWBACK.Where(x => x.Code == poi.PurchaseorderCode).FirstOrDefault();
                                        if (po != null)
                                        {
                                            po.Status = (int)DB.TWBACKENDDB.Models.PurchaseOrderTWBACK.status.取消;
                                        }
                                        var tempPOI = backendDB.PurchaseOrderitemTWBACK.Where(x => x.Code == poi.Code).FirstOrDefault();
                                        if (tempPOI != null)
                                        {
                                            tempPOI.Status = (int)DB.TWBACKENDDB.Models.PurchaseOrderitemTWBACK.status.取消;
                                        }
                                        backendDB.SaveChanges();
                                    }
                                    catch (Exception e)
                                    {
                                        logger.Error(e.Message);
                                    }
                                }
                            }
                            /*}
                            else
                            {
                                logger.Debug("Purchaseorderitem_code " + poi.Code + " doesn' show Cancel Button");
                            }*/
                        }
                    }
                }

                if (POItems.Count() == 0)
                {
                    logger.Debug("no purchaseorderitem cancelOrder requests have been sent, salesorder " + salesorder_code);
                }
                else
                {
                    logger.Debug("all the purchaseorderitem cancelOrder requests have been sent, salesorder " + salesorder_code);
                }
            }
            catch (Exception e)
            {
                logger.Debug("NeweggRequestCancel_Bad");
            }
        }
        public string Send_retgood(TWNewEgg.Models.ViewModels.MyAccount.ReturnPost ReturnPost, List<Retgood> retgoodList, List<TWNewEgg.DB.TWBACKENDDB.Models.Cart> CartList, List<TWNewEgg.DB.TWSQLDB.Models.SalesOrder>SalesOrderList,string return_reasontext) 
        {

            try
            {
                // 收件人
                string Recipient = "";
                Recipient = ReturnPost.Email;

                // 判斷交易模式寄信給物流
                if (ReturnPost.ShipType == 2 || ReturnPost.ShipType == 7 || ReturnPost.ShipType == 8 || ReturnPost.ShipType == 9)
                {
                    Recipient = "Gretchen.H.Yeh@newegg.com, Teresa.S.Li@newegg.com, Dolcee.J.Chang@newegg.com, Joyce.H.Hsiao@newegg.com, gp.team.mkpl.sourcing.tw@newegg.com";
                }
                else
                {
                    Recipient = "Gretchen.H.Yeh@newegg.com, Grace.c.hsiao@newegg.com, Steven.c.mao@newegg.com, Jasmine.C.Hsieh@newegg.com, Jessie.Y.Tseng@newegg.com, Dolcee.J.Chang@newegg.com, Joyce.H.Hsiao@newegg.com, gp.team.mkpl.sourcing.tw@newegg.com";
                }

                var TestXMLEXport = new TWNewEgg.InternalSendMail.Service.GeneratorView();
                TestXMLEXport.GeneraterViewPage(retgoodList, "retgood", CartList, SalesOrderList, return_reasontext, null);
                return "true";
            }
            catch 
            {
                return "false";
            
            }
        }
        /// <summary>
        /// 產生取消單
        /// </summary>
        /// <param name="carts">cartsList</param>
        /// <param name="Processes">processesList</param>
        /// <param name="bankid">銀行ID</param>
        /// <param name="branches">分行ID</param>
        /// <param name="bankaccount">退款帳號</param>
        /// <param name="reset_reasontext">取消原因</param>
        /// <returns>SalesOrderCancel model</returns>
        public List<SalesOrderCancel> CreatSalesOrderCancel(List<TWNewEgg.DB.TWBACKENDDB.Models.Cart> carts, List<TWNewEgg.DB.TWBACKENDDB.Models.Process> processes, string bankid, string branches, string bankaccount, string reset_reasontext)
        {
            // 取消單List
            List<TWNewEgg.DB.TWSQLDB.Models.SalesOrderCancel> SalesOrderCancelList = new List<TWNewEgg.DB.TWSQLDB.Models.SalesOrderCancel>();
            // 產生取消單
            for (int i = 0; i < carts.Count(); i++)
            {
                int itemid = (processes.Where(s => s.CartID == carts[i].ID).Select(s => s.StoreID).FirstOrDefault() ?? 0);
                TWNewEgg.DB.TWSQLDB.Models.SalesOrderCancel SalesOrderCancel = new TWNewEgg.DB.TWSQLDB.Models.SalesOrderCancel()
                {
                    // CartID
                    SalesorderCode = carts[i].ID,
                    // 賣場ID
                    ItemID = itemid,
                    // 取消原因
                    CauseNote = reset_reasontext,
                    // 使用者名稱
                    AccountName = carts[i].Username,
                    // 銀行ID
                    BankID = bankid,
                    // 分行ID
                    BankBranch = branches,
                    // 退款帳戶
                    AccountNO = bankaccount,
                    // 產生時間
                    CreateDate = DateTime.UtcNow.AddHours(8)
                };

                dbbefore.SalesOrderCancel.Add(SalesOrderCancel);
                SalesOrderCancelList.Add(SalesOrderCancel);
            }
            dbbefore.SaveChanges();
            return SalesOrderCancelList;
        }

    }
}