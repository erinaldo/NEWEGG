using log4net;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using TWNewEgg.API.Models;

using TWNewEgg.Models.DomainModels;

namespace TWNewEgg.API.Service
{
    public class Discard4Service
    {
        // 記錄訊息
        //private static ILog log = LogManager.GetLogger(typeof(Discard4Service));
        private static ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.FullName);

        /// <summary>
        /// InitData
        /// </summary>
        /// <param name="salesorderCode"></param>
        /// <param name="user_name"></param>
        /// <returns></returns>
        public static List<TWNewEgg.Models.DomainModels.Discard4.Discard4ItemDM> InitData(string salesorderCode, string user_name)
        {
            List<TWNewEgg.Models.DomainModels.Discard4.Discard4ItemDM> list_info = new List<TWNewEgg.Models.DomainModels.Discard4.Discard4ItemDM>();
            try
            {
                TWNewEgg.Models.DomainModels.Discard4.Discard4ItemDM info = new TWNewEgg.Models.DomainModels.Discard4.Discard4ItemDM();
                info.SalesorderCode = salesorderCode;
                info.CreateUser = user_name;

                var result = TWNewEgg.Framework.ServiceApi.Processor.Request<List<TWNewEgg.Models.DomainModels.Discard4.Discard4ItemDM>
                    , List<TWNewEgg.Models.DomainModels.Discard4.Discard4ItemDM>>("Discard4ItemService", "InitData", salesorderCode, user_name);
                list_info = result.results;
            }
            catch (Exception ex)
            {
                log.Error("預覽 Msg: " + ex.Message + ", Stacktrace: " + ex.StackTrace);
            }
            return list_info;
        }

        /// <summary>
        /// Save
        /// </summary>
        /// <param name="list_info"></param>
        /// <param name="user_name"></param>
        /// <returns></returns>
        public static List<bool> Save(List<TWNewEgg.Models.DomainModels.Discard4.Discard4ItemDM> list_info, string user_name)
        {
            List<bool> list_result = new List<bool>();
            try
            {
                //string user_name = string.Empty;
                foreach (var each_info in list_info)
                {
                    each_info.UpdateUser = user_name;
                    break;
                }

                var result = TWNewEgg.Framework.ServiceApi.Processor.Request<List<bool>
                   , List<bool>>("Discard4ItemService", "Save", list_info, user_name);

                if (result.results != null)
                    list_result = result.results;
                else
                {
                    foreach (var each_info in list_info)
                        list_result.Add(false);
                }


               // foreach (TWNewEgg.Models.DomainModels.Discard4.Discard4ItemDM each_info in list_info)
               // {
               //     string method_name = string.Empty;
               //     if (each_info.ID == 0) method_name = "Add";

               //     if (each_info.ID > 0) method_name = "Update";

               //     //method_name = "Save";

               //     var info_result = TWNewEgg.Framework.ServiceApi.Processor.Request<List<bool>
               //, List<bool>>("Discard4ItemService", method_name, each_info, user_name);

               //     if (info_result.results != null)
               //         list_result.Add(true);

               // }

            }
            catch (Exception ex)
            {
                log.Error("預覽 Msg: " + ex.Message + ", Stacktrace: " + ex.StackTrace);
            }
            return list_result;
        }

        /// <summary>
        /// InitData
        /// </summary>
        /// <param name="salesorderCode"></param>
        /// <param name="user_name"></param>
        /// <returns></returns>
        public static List<TWNewEgg.Models.DomainModels.Discard4.Discard4ItemDM> GetData(string salesorderCode, string user_name)
        {
            List<TWNewEgg.Models.DomainModels.Discard4.Discard4ItemDM> list_info = new List<TWNewEgg.Models.DomainModels.Discard4.Discard4ItemDM>();
            try
            {
                TWNewEgg.Models.DomainModels.Discard4.Discard4ItemDM info = new TWNewEgg.Models.DomainModels.Discard4.Discard4ItemDM();
                info.SalesorderCode = salesorderCode;
                info.CreateUser = user_name;

                var result = TWNewEgg.Framework.ServiceApi.Processor.Request<List<TWNewEgg.Models.DomainModels.Discard4.Discard4ItemDM>
                    , List<TWNewEgg.Models.DomainModels.Discard4.Discard4ItemDM>>("Discard4ItemService", "GetData", info);
                list_info = result.results;
            }
            catch (Exception ex)
            {
                log.Error("預覽 Msg: " + ex.Message + ", Stacktrace: " + ex.StackTrace);
            }

            return list_info;
        }

    }

}