namespace TWNewEgg.AdvService.Service
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using TWNewEgg.AdvService.Models;
    using TWNewEgg.DB.TWSQLDB.Models;
    using TWNewEgg.DB.TWSQLDB.Models.ExtModels;
    using TWNewEgg.ItemService.Service;
    using TWNewEgg.SearchService.Service;

    /// <summary>
    /// AdvEventTypeReposity
    /// </summary>
    public class AdvEventTypeReposity : IAdvEventType
    {
        /// <summary>
        /// 新增AdvEvent
        /// </summary>
        /// <param name="arg_oAdvEventType">新增的AdvEvent</param>
        /// <returns>新增AdvEvent的ID;0:新增失敗</returns>
        public int AddAdvEventType(AdvEventType arg_oAdvEventType)
        {
            int nId = 0;
            AdvEventDBService objAdvTypeService = null;

            objAdvTypeService = new AdvEventDBService();
            nId = objAdvTypeService.AddAdvEventType(arg_oAdvEventType);
            objAdvTypeService = null;

            return nId;
        }

        /// <summary>
        /// 取得所有的AdvEventType列表
        /// </summary>
        /// <returns>null或是AdvEventType的列表</returns>
        public List<AdvEventType> GetAdvEventTypeList()
        {
            List<AdvEventType> listAdvEventType = null;
            AdvEventDBService objAdvTypeService = null;

            objAdvTypeService = new AdvEventDBService();
            listAdvEventType = objAdvTypeService.GetAdvEventTypeList();
            objAdvTypeService = null;

            return listAdvEventType;
        }
 
        /// <summary>
        /// 查詢Name含有關鍵字的AdvEventType
        /// </summary>
        /// <param name="arg_strKeyword">關鍵字</param>
        /// <returns>null或是AdvEventType的列表</returns>
        public List<AdvEventType> GetAdvEventTypeListByName(string arg_strKeyword)
        {
            List<AdvEventType> listAdvEventType = null;
            AdvEventDBService objAdvTypeService = null;

            objAdvTypeService = new AdvEventDBService();
            listAdvEventType = objAdvTypeService.GetAdvEventTypeListByName(arg_strKeyword);
            objAdvTypeService = null;

            return listAdvEventType;
        }

        /// <summary>
        /// 查詢CSS含有關鍵字的AdvEventType
        /// </summary>
        /// <param name="arg_strCSS">關鍵字</param>
        /// <returns>null或是AdvEventType的列表</returns>
        public List<AdvEventType> GetAdvEventTypeListByCSS(string arg_strCSS)
        {
            List<AdvEventType> listAdvEventType = null;
            AdvEventDBService objAdvTypeService = null;

            objAdvTypeService = new AdvEventDBService();
            listAdvEventType = objAdvTypeService.GetAdvEventTypeListByCSS(arg_strCSS);
            objAdvTypeService = null;

            return listAdvEventType;
        }

        /// <summary>
        /// 根據ID取得AdvEventType物件
        /// </summary>
        /// <param name="arg_nAdvEventType">ID</param>
        /// <returns>null或AdvEventType物件</returns>
        public AdvEventType GetAdvEventTypeById(int arg_nAdvEventType)
        {
            AdvEventType objAdvEventType = null;
            AdvEventDBService objAdvTypeService = null;

            objAdvTypeService = new AdvEventDBService();
            objAdvEventType = objAdvTypeService.GetAdvEventTypeById(arg_nAdvEventType);
            objAdvTypeService = null;

            return objAdvEventType;
        }

        /// <summary>
        /// 根據ID取得Active AdvEventType物件
        /// </summary>
        /// <param name="arg_nAdvEventType">ID</param>
        /// <returns>null或AdvEventType物件</returns>
        public AdvEventType GetActiveAdvEventTypeById(int arg_nAdvEventType)
        {
            AdvEventType objAdvEventType = null;
            AdvEventDBService objAdvTypeService = null;
            DateTime objDateNow = DateTime.Now;
            bool boolActive = false;

            objAdvTypeService = new AdvEventDBService();
            objAdvEventType = objAdvTypeService.GetAdvEventTypeById(arg_nAdvEventType);
            objAdvTypeService = null;

            if (objAdvEventType != null)
            {
                if (objAdvEventType.StartDate <= objDateNow && objAdvEventType.EndDate >= objDateNow)
                {
                    boolActive = true;
                }
            }

            if (boolActive)
            { 
                return objAdvEventType; 
            }
            else
            {
                return null; 
            }
        }

        /// <summary>
        /// 修改AdvEventType
        /// </summary>
        /// <param name="arg_oAdvEventType">修改的AdvEventType物件</param>
        /// <returns>true:修改成功, false:修改失敗</returns>
        public bool UpdateAdvEventType(AdvEventType arg_oAdvEventType)
        {
            bool boolExec = false;
            AdvEventDBService objAdvTypeService = null;

            objAdvTypeService = new AdvEventDBService();
            boolExec = objAdvTypeService.UpdateAdvEventType(arg_oAdvEventType);
            objAdvTypeService = null;

            return boolExec;
        }

        /// <summary>
        /// 根據Country Code及AdvType取得AdvEventType的列表
        /// </summary>
        /// <param name="arg_numCountryId">Country ID</param>
        /// <param name="arg_numAdvType">AdvType</param>
        /// <returns>arg_numAdvType列表</returns>
        public List<AdvEventType> GetAdvEventTypeListByCountryAndAdvType(int arg_numCountryId, int arg_numAdvType)
        {
            List<AdvEventType> listAdvEventType = null;
            AdvEventDBService objAdvTypeService = null;

            objAdvTypeService = new AdvEventDBService();
            listAdvEventType = objAdvTypeService.GetAdvEventTypeListByCountryAndAdvType(arg_numCountryId, arg_numAdvType);

            objAdvTypeService = null;
                
            return listAdvEventType;
        }

        /// <summary>
        /// 根據Country Code及AdvType取得有效期間內的AdvEventType的列表
        /// </summary>
        /// <param name="arg_numCountryId">國家ID</param>
        /// <param name="arg_numAdvType">AdvType</param>
        /// <returns>AdvEventType列表</returns>
        public List<AdvEventType> GetActiveAdvEventTypeListByCountryAndAdvType(int arg_numCountryId, int arg_numAdvType)
        {
            List<AdvEventType> listAdvEventType = null;
            DateTime objDateNow = DateTime.Now;
            AdvEventDBService objAdvTypeService = null;

            objAdvTypeService = new AdvEventDBService();
            listAdvEventType = objAdvTypeService.GetActiveAdvEventTypeListByCountryAndAdvType(arg_numCountryId, arg_numAdvType);

            objAdvTypeService = null;

            return listAdvEventType;
        }

        /// <summary>
        /// 根據Country Code及AdvType取得有效期間內的AdvEventType的列表
        /// </summary>
        /// <param name="arg_numAdvType">AdvType</param>
        /// <returns>AdvEventType列表</returns>
        public List<AdvEventType> GetActiveAdvEventTypeListByAdvType(int arg_numAdvType)
        {
            List<AdvEventType> listAdvEventType = null;
            DateTime objDateNow = DateTime.Now;
            AdvEventDBService objAdvTypeService = null;

            objAdvTypeService = new AdvEventDBService();
            listAdvEventType = objAdvTypeService.GetActiveAdvEventTypeListByAdvType(arg_numAdvType);

            objAdvTypeService = null;

            return listAdvEventType;
        }
    }
}
