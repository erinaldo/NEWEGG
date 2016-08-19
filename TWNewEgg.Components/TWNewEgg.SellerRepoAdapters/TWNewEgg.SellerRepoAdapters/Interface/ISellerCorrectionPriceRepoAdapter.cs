using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TWNewEgg.Models.DBModels.TWBACKENDDB;

namespace TWNewEgg.SellerRepoAdapters.Interface
{
    /// <summary>
    /// 依據 BSATW-232 供應商對帳單新增調整項目---------------add by bruce 20160719
    /// </summary>
    public interface ISellerCorrectionPriceRepoAdapter
    {

        /// <summary>
        /// 取得一筆
        /// </summary>
        /// <param name="id">系統流水編號</param>
        /// <returns></returns>
        SellerCorrectionPriceDB GetInfo(int? id);

        IQueryable<SellerCorrectionPriceDB> GetData(int sellerID, string settlementID, string user_name);

        IQueryable<SellerCorrectionPriceDB> GetAll();

        bool Update(SellerCorrectionPriceDB info);
        
        SellerCorrectionPriceDB Add(SellerCorrectionPriceDB info);
        
        bool Del(SellerCorrectionPriceDB info);
    }
}
