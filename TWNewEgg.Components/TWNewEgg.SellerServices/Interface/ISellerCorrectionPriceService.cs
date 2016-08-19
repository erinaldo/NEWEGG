using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TWNewEgg.Framework.AutoMapper;

using TWNewEgg.Models.DomainModels.Seller;

namespace TWNewEgg.SellerServices.Interface
{
    /// <summary>
    /// 依據 BSATW-232 供應商對帳單新增調整項目---------------add by bruce 20160720
    /// </summary>
    public interface ISellerCorrectionPriceService
    {

        /// <summary>
        /// 儲存即有的或新的資料
        /// </summary>
        /// <param name="list_info"></param>
        /// <param name="user_name"></param>
        /// <returns></returns>
        List<bool> Save(List<SellerCorrectionPriceDM> list_info, string user_name);

        /// <summary>
        /// 儲存發票後執行
        /// </summary>
        /// <param name="input_info"></param>
        /// <param name="user_name"></param>
        /// <returns></returns>
        List<bool> Save1(SellerCorrectionPriceSearchDM input_info, string user_name);

        /// <summary>
        /// 取得一筆
        /// </summary>
        /// <param name="id">系統流水編號</param>
        /// <returns></returns>
        SellerCorrectionPriceDM GetInfo(int? id);



        /// <summary>
        /// 依條件取得資料
        /// </summary>
        /// <param name="input_info"></param>
        /// <returns></returns>
        List<SellerCorrectionPriceDM> GetGroupBy(SellerCorrectionPriceSearchDM input_info);


        /// <summary>
        /// 依條件取得資料
        /// </summary>
        /// <param name="input_info"></param>
        /// <returns></returns>
        List<SellerCorrectionPriceDM> GetData(SellerCorrectionPriceSearchDM input_info);

        /// <summary>
        /// 取得全部資料
        /// </summary>
        /// <returns></returns>
        List<SellerCorrectionPriceDM> GetAll();

        /// <summary>
        /// 修改資料
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        bool Update(SellerCorrectionPriceDM info, string user_name);

        /// <summary>
        /// 建立資料
        /// </summary>
        /// <param name="info">domain model</param>
        /// <returns></returns>
        SellerCorrectionPriceDM Add(SellerCorrectionPriceDM info, string user_name);

        /// <summary>
        /// 刪除
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        bool Del(int? id);

    }
}
