using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TWNewEgg.API.Models;
using TWNewEgg.API.Models.Models;
using TWNewEgg.DB.TWSELLERPORTALDB.Models;
using TWNewEgg.Framework.ServiceApi;

namespace TWNewEgg.API.View.ServiceAPI
{
    public class APIConnector
    {
        /// <summary>
        /// 待審明細編輯
        /// </summary>
        /// <param name="UpdateItemTemp"></param>
        /// <returns></returns>
        public TWNewEgg.API.Models.ActionResponse<List<string>> EditDetailTemp(TWNewEgg.API.Models.ItemSketch UpdateItemTemp)
        {
            var result = Processor.Request<TWNewEgg.API.Models.ActionResponse<List<string>>, TWNewEgg.API.Models.ActionResponse<List<string>>>("Controllers.ItemTempController", "EditDetailTemp", UpdateItemTemp);
            return result.results;
        }

        /// <summary>
        /// 待審搜尋
        /// </summary>
        /// <param name="condition">查詢條件</param>
        /// <returns></returns>
        public TWNewEgg.API.Models.ActionResponse<List<ItemSketch>> GetItemTempList(ItemSketchSearchCondition condition)
        {
            var result = Processor.Request<API.Models.ActionResponse<List<ItemSketch>>, API.Models.ActionResponse<List<ItemSketch>>>("Controllers.ItemTempController", "Search", condition);
            return result.results;
        }

        public TWNewEgg.API.Models.ActionResponse<List<TWNewEgg.API.Models.DomainModel.AutoCompleteModel>> GetSellerSearchAutoComplete()
        {
            var result = Processor.Request<TWNewEgg.API.Models.ActionResponse<List<TWNewEgg.API.Models.DomainModel.AutoCompleteModel>>, TWNewEgg.API.Models.ActionResponse<List<TWNewEgg.API.Models.DomainModel.AutoCompleteModel>>>("Controllers.SellerRelationshipManageController", "GetSeller_BasicInfos", null);
            return result.results;
        }

    }
}