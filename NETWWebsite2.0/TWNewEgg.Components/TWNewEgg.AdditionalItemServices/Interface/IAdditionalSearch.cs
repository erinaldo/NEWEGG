using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TWNewEgg.AdditionalItemServices.Interface
{
    public interface IAdditionalSearch
    {
        /// <summary>
        /// 檢查 Item.ID 是否為加價購賣場
        /// </summary>
        /// <param name="argitemID"></param>
        /// <returns>key: ID, value: ShowOrder</returns>
        Dictionary<int,int> checkAdditionItem(int argitemID);

        /// <summary>
        /// 檢查 ItemTemp.ID 是否為加價購賣場
        /// </summary>
        /// <param name="argitemTempID"></param>
        /// <returns>key: ID, value: ShowOrder</returns>
        Dictionary<int, int> checkAdditionTemp(int argitemTempID);

        /// <summary>
        /// 檢查 ItemSketch.ID 是否為加價購賣場
        /// </summary>
        /// <param name="argitemItemSketchID"></param>
        /// <returns>key: ID, value: ShowOrder</returns>
        Dictionary<int, int> checkAdditionSketch(int argitemItemSketchID);

        /// <summary>
        /// 檢查此 List<Item.ID> 是否為加價購賣場
        /// </summary>
        /// <param name="argitemID"></param>
        /// <returns>key: ID, value: ShowOrder</returns>
        Dictionary<int, int> checkAdditionItems(List<int> argitemID);

        /// <summary>
        /// 檢查此 List<ItemTemp.ID> 是否為加價購賣場 
        /// </summary>
        /// <param name="argitemTempID"></param>
        /// <returns>key: ID, value: ShowOrder</returns>
        Dictionary<int, int> checkAdditionTemps(List<int> argitemTempID);

        /// <summary>
        /// 檢查此 List<ItemSketch.ID> 是否為加價購賣場 
        /// </summary>
        /// <param name="argitemItemSketchID"></param>
        /// <returns>key: ID, value: ShowOrder</returns>
        Dictionary<int, int> checkAdditionSketchs(List<int> argitemItemSketchID);
    }
}
