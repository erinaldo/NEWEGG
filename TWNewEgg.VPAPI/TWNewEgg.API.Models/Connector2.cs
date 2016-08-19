using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TWNewEgg.API.Models
{
    public partial class Connector
    {
        /// <summary>
        /// 初始化癈四機回收四聯單
        /// </summary>
        /// <param name="auth"></param>
        /// <param name="token"></param>
        /// <param name="salesorderCode">LBO</param>
        /// <param name="user_name">建立者</param>
        /// <returns></returns>
        public TWNewEgg.API.Models.ActionResponse<List<TWNewEgg.Models.DomainModels.Discard4.Discard4ItemDM>> InitData_Discard4(string auth, string token, string salesorderCode, string user_name)
        {

            //TWNewEgg.Models.DomainModels.Discard4.Discard4ItemDM info = new TWNewEgg.Models.DomainModels.Discard4.Discard4ItemDM();
            //info.SalesorderCode = salesorderCode;
            //info.CreateUser = user_name;
            var result = Post<TWNewEgg.API.Models.ActionResponse<List<TWNewEgg.Models.DomainModels.Discard4.Discard4ItemDM>>>(APIHost + "/Discard4/InitData", auth, token, new { salesorderCode = salesorderCode, user_name = user_name });
            return result;
        }

        /// <summary>
        /// 儲存即有的或新的資料
        /// </summary>
        /// <param name="auth"></param>
        /// <param name="token"></param>
        /// <param name="list_info"></param>
        /// <param name="user_name"></param>
        /// <returns></returns>
        public TWNewEgg.API.Models.ActionResponse<List<bool>> Save_Discard4(string auth, string token, List<TWNewEgg.Models.DomainModels.Discard4.Discard4ItemDM> list_info, string user_name)
        {
            foreach (var info in list_info) info.UpdateUser = user_name;
            var result = Post<TWNewEgg.API.Models.ActionResponse<List<bool>>>(APIHost + "/Discard4/Save", auth, token, new { list_info = list_info, user_name = user_name });
            return result;
        }

        /// <summary>
        /// 取得資料
        /// </summary>
        /// <param name="auth"></param>
        /// <param name="token"></param>
        /// <param name="salesorderCode"></param>
        /// <param name="user_name"></param>
        /// <returns></returns>
        public TWNewEgg.API.Models.ActionResponse<List<TWNewEgg.Models.DomainModels.Discard4.Discard4ItemDM>> GetData_Discard4(string auth, string token, string salesorderCode, string user_name)
        {
            var result = Post<TWNewEgg.API.Models.ActionResponse<List<TWNewEgg.Models.DomainModels.Discard4.Discard4ItemDM>>>(APIHost + "/Discard4/GetData", auth, token, new { salesorderCode = salesorderCode, user_name = user_name });
            return result;
        }



        #region 依據 BSATW-232 供應商對帳單新增調整項目---------------add by bruce 20160721


        /// <summary>        
        /// 取得供應商對帳單新增調整項目
        /// </summary>
        /// <param name="auth"></param>
        /// <param name="token"></param>
        /// <param name="sellerID"></param>
        /// <param name="user_name"></param>
        /// <returns></returns>
        public TWNewEgg.API.Models.ActionResponse<List<TWNewEgg.Models.DomainModels.Seller.SellerCorrectionPriceDM>> Get_SellerCorrectionPrice_GroupBy(string auth, string token, string finanStatus, int sellerID, string settlementID,  string user_name)
        {
            var result = Post<TWNewEgg.API.Models.ActionResponse<List<TWNewEgg.Models.DomainModels.Seller.SellerCorrectionPriceDM>>>(APIHost + "/SellerCorrectionPrice/GetGroupBy", auth, token, new { finanStatus = finanStatus, sellerID = sellerID, settlementID = settlementID, user_name = user_name });
            return result;
        }


        /// <summary>
        /// 儲存供應商對帳單新增調整項目
        /// </summary>
        /// <param name="auth"></param>
        /// <param name="token"></param>
        /// <param name="sellerID"></param>
        /// <param name="settlementID"></param>
        /// <param name="user_name"></param>
        /// <returns></returns>
        public TWNewEgg.API.Models.ActionResponse<List<bool>> Save_SellerCorrectionPrice(string auth, string token,string finanStatus, int sellerID, string settlementID, string user_name)
        {
            var result = Post<TWNewEgg.API.Models.ActionResponse<List<bool>>>(APIHost + "/SellerCorrectionPrice/Save1", auth, token, new { finanStatus = finanStatus, sellerID = sellerID, settlementID = settlementID, user_name = user_name });
            return result;
        }

        #endregion



    }
}
