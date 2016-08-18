using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TWNewEgg.Models.DomainModels.Category;
using TWNewEgg.Models.DomainModels.Message;

namespace TWNewEgg.CategoryServices.Interface
{
    public interface IAdLayer3Services
    {
        /// <summary>
        /// 查詢第三層分類廣告類型：banner
        /// </summary>
        /// <param name="SearchCondition"></param>
        /// <returns></returns>
        List<AdLayer3DM> GetAdLayer3(AdLayer3DM SearchCondition);

        /// <summary>
        /// 查詢第三層分類廣告類型：items
        /// </summary>
        /// <param name="SearchCondition"></param>
        /// <returns></returns>
        List<AdLayer3ItemDM> GetAdLayer3Item(AdLayer3ItemDM SearchCondition);

        /// <summary>
        /// 更新第三層分類廣告banner or items(單筆)
        /// </summary>
        /// <param name="NewData"></param>
        /// <returns></returns>
        ResponseMessage<AdLayer3DM> UpdateAdLayer3Data(AdLayer3DM NewData);

        /// <summary>
        /// 更新第三層分類廣告banner or items(多筆)
        /// </summary>
        /// <param name="NewDataList"></param>
        /// <returns></returns>
        List<ResponseMessage<AdLayer3DM>> UpdateAdLayer3List(List<AdLayer3DM> NewDataList);

        /// <summary>
        /// 查詢第三層分類廣告類型：banner and items
        /// </summary>
        /// <param name="SearchCondition"></param>
        /// <returns></returns>
        List<AdLayer3DM> GetAdLayer3List(AdLayer3DM SearchCondition);

        /// <summary>
        /// 回傳有效的itemID list
        /// </summary>
        /// <param name="itemIDList"></param>
        /// <returns></returns>
        List<int> GetAvailableAndVisibleItemID(List<int> itemIDList);
    }
}
