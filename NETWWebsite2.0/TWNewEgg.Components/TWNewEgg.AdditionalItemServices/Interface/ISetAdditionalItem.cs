using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TWNewEgg.AdditionalItemServices.Interface
{
    public interface ISetAdditionalItem
    {
        /// <summary>
        /// 設定 Item.ID 賣場為加價購賣場
        /// </summary>
        /// <param name="argItemID"></param>
        /// <param name="updateUser"></param>
        /// <returns></returns>
        bool EnableAdditionItemforItem(int argItemID, string updateUser);

        /// <summary>
        /// 設定 ItemTemp.ID 賣場為加價購賣場
        /// </summary>
        /// <param name="argItemTempID"></param>
        /// <param name="updateUser"></param>
        /// <returns></returns>
        bool EnableAdditionItemforItemTemp(int argItemTempID, string updateUser);
        
        /// <summary>
        /// 設定 ItemSketch.ID 賣場為加價購賣場
        /// </summary>
        /// <param name="argItemSketchID"></param>
        /// <param name="updateUser"></param>
        /// <returns></returns>
        bool EnableAdditionItemforItemSketch(int argItemSketchID, string updateUser);
        
        /// <summary>
        /// 取消設定 Item.ID 賣場為加價購賣場
        /// </summary>
        /// <param name="argItemID"></param>
        /// <param name="updateUser"></param>
        /// <returns></returns>
        bool DisableAdditionItem(int argItemID, string updateUser);
        
        /// <summary>
        /// 取消設定 ItemTemp.ID 賣場為加價購賣場
        /// </summary>
        /// <param name="argItemTempID"></param>
        /// <param name="updateUser"></param>
        /// <returns></returns>
        bool DisableAdditionItemTemp(int argItemTempID, string updateUser);
        
        /// <summary>
        /// 取消設定 ItemSketch.ID 賣場為加價購賣場
        /// </summary>
        /// <param name="argItemSketchID"></param>
        /// <param name="updateUser"></param>
        /// <returns></returns>
        bool DisableAdditionItemSketch(int argItemSketchID, string updateUser);
    }
}
