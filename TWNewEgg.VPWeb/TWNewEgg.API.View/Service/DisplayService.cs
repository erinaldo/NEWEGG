using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TWNewEgg.API.Models;

namespace TWNewEgg.API.View.Service
{
    public class DisplayService
    {
        log4net.ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.FullName); 

        TWNewEgg.API.Models.Connector conn = new API.Models.Connector();

        public ActionResponse<List<DB.TWSQLDB.Models.Category>> GetCategoryList(int? categoryID, int? Layer, int? parentID)
        {
            ActionResponse<List<DB.TWSQLDB.Models.Category>> CategoryList = new ActionResponse<List<DB.TWSQLDB.Models.Category>>();

            CategoryList = GetAPICategoryList(categoryID, Layer, parentID);

            return CategoryList;
        }

        private ActionResponse<List<DB.TWSQLDB.Models.Category>> GetAPICategoryList(int? queryCategoryID, int? Layer, int? parentID)
        {
            ActionResponse<List<DB.TWSQLDB.Models.Category>> CategoryList = new ActionResponse<List<DB.TWSQLDB.Models.Category>>();

            try
            {
                if (Layer.HasValue && parentID.HasValue)
                {
                    CategoryList = conn.APIQueryCategory(null, null, Layer.Value, parentID.Value);


                }
                else
                {
                    CategoryList = conn.APIQueryCategory(null, null, null, null);
                }

                if (CategoryList.IsSuccess == true && CategoryList.Body != null)
                {
                    CategoryList.Body = CategoryList.Body.Where(x => x.ShowAll == 1).ToList();
                }

                //if (queryCategoryID != null || queryCategoryID != 0)
                //{
                //    CategoryList = CategoryList.Where(x => x.ID == queryCategoryID.Value).ToList();
                //}

            }
            catch (Exception error)
            {
                logger.Error("[Msg]: " + error.Message + " ;[StackTrace]: " + error.StackTrace);
            }

            return CategoryList;
        }
    }
}