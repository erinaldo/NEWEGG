using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TWNewEgg.API.Models;


namespace TWNewEgg.BulletinsMessageService.Interface
{
    public interface IBulletinsMessage
    {
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        ActionResponse<TWNewEgg.DB.TWSELLERPORTALDB.Models.Seller_BulletinsMessage> GetBulletinsMessage(string System);

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        ActionResponse<List<TWNewEgg.DB.TWSELLERPORTALDB.Models.Seller_BulletinsMessage>> GetBulletinsMessageList(string System);

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        ActionResponse<string> CreateBulletinsMessage(TWNewEgg.DB.TWSELLERPORTALDB.Models.Seller_BulletinsMessage CreateModel);

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        ActionResponse<string> UpdateBulletinsMessage(TWNewEgg.DB.TWSELLERPORTALDB.Models.Seller_BulletinsMessage UpdateModel);

        /// <summary>
        /// 啟用哪一個公佈欄，利用 ID、FromSystem 決定，一旦啟用該 System 其他公佈欄將會 Disable
        /// </summary>
        /// <returns></returns>
        ActionResponse<string> EnableBulletinsMessage(DB.TWSELLERPORTALDB.Models.Seller_BulletinsMessage EnableModel);

        /// <summary>
        /// 不啟用哪些公佈欄
        /// </summary>
        /// <returns></returns>
        ActionResponse<string> DisableBulletinsMessage(DB.TWSELLERPORTALDB.Models.Seller_BulletinsMessage DisableModel);
    }
}
