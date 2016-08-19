using System;
using System.Data;
using System.Data.SqlClient;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Xml;
using System.Net.Http;
using System.Web.Script.Serialization;
using TWNewEgg.DB;
using TWNewEgg.DB.Models.Logistics.HiLife;
using TWNewEgg.Logistics.HiLife.Model;
using TWNewEgg.Common;
using TWNewEgg.DB.TWSQLDB.Models;

namespace TWNewEgg.Logistics.HiLife.Service
{
    /// <summary>
    /// 新蛋使用萊爾富的Service接口
    /// </summary>
    public class HiLifeForNewEggAPI
    {
        private string mstrStructFilePath = System.Configuration.ConfigurationManager.AppSettings["StructFilePath"];
        private string mstrIppDomain = System.Configuration.ConfigurationManager.AppSettings["IppUrl"];
        private string mstrLogFilePath = System.Configuration.ConfigurationManager.AppSettings["LogFilePath"];
        private FileStream mswLogMessage = null;
        public HiLifeForNewEggAPI()
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public XmlDocument GetRoadMap()
        {
            XmlDocument objXml = null;
            HiLifeFileSyncRepository objFileService = null;
            Dictionary<string, Dictionary<string, Dictionary<string, HiLifeBasicFieldStruct>>> objDictFieldStruct = null;

            objFileService = new HiLifeFileSyncRepository();
            objDictFieldStruct = objFileService.GetFieldStruct();

            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="argTargetSavePath"></param>
        public void GetRoadMap(string argTargetSavePath)
        {
        }

        public void SetOrderArrivedTime()
        {
        }

        public void SetOrderPickupType()
        {
        }

        /// <summary>
        /// 讀取F04預計進店檔內容，呼叫IPP程式
        /// </summary>
        /// <param name="TransDate">預計進店檔傳送日期</param>
        public void SetOrderPredictTime(DateTime TransDate)
        {
            /* 從DB裡抓取進店檔內容
             * 轉換訂單編號(11碼->15碼)
             * 呼叫程式回寫資訊
             */
            HiLifeAnalysisRepository objHiLifeF04 = null;
            TWNewEgg.DB.TWSQLDB.Models.GenerateNumberMap objNewNumber = null;
            List<string> listOldnumber = null;
            List<F04Body> ListF04 = null;
            List<TWNewEgg.DB.TWSQLDB.Models.GenerateNumberMap> ListNewnumber = null;
            Dictionary<string, string> DictionaryF04 = null;

            // 取出DB內資料
            objHiLifeF04 = new HiLifeAnalysisRepository();
            ListF04 = objHiLifeF04.GetBodyByCreareDate<F04Body>(TransDate); //取得F04Body檔案

            // 將訂單編號存在List裡
            listOldnumber = ListF04.Select(x => x.Odno).DefaultIfEmpty().ToList();

            // 11碼 -> 15碼
            ListNewnumber = this.SearchNeweggOrderNumber(listOldnumber);
            if (ListNewnumber != null)
            {
                DictionaryF04 = new Dictionary<string, string>();
                foreach (F04Body subF04Body in ListF04)
                {
                    objNewNumber = ListNewnumber.FirstOrDefault(x => x.GenerateNumber.Equals(subF04Body.Odno));
                    if (objNewNumber != null && !DictionaryF04.ContainsKey(objNewNumber.SourceNumber))
                    {
                        // 將訂單編號與商品進店日存入Dictionary內
                        DictionaryF04.Add(objNewNumber.SourceNumber, subF04Body.Dcstdt.Substring(0,4) + "/" + subF04Body.Dcstdt.Substring(4,2) + "/" + subF04Body.Dcstdt.Substring(6,2));
                    }
                }
            }

            // 釋放記憶體
            if (ListF04 != null)
            {
                ListF04.Clear();
                ListF04 = null;
            }

            if (ListNewnumber != null)
            {
                ListNewnumber.Clear();
                ListNewnumber = null;
            }

            objNewNumber = null;

            //呼叫IPP後台傳Dictionary
            string strJson = "";
            System.Net.Http.HttpContent objContent = null;
            System.Net.Http.HttpClient objClient = null;
            HttpResponseMessage objWebRsp = null;

            strJson = new JavaScriptSerializer().Serialize(DictionaryF04);
            strJson = "{\"o2oDelivery\":" + strJson + "}";
            objContent = new StringContent(strJson);
            objContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");
            objClient = new System.Net.Http.HttpClient();
            objClient.DefaultRequestHeaders.Add("user-agent", "Mozilla/5.0 (compatible; MSIE 10.0; Windows NT 6.2; WOW64; Trident/6.0)");
            objWebRsp = objClient.PostAsync(this.mstrIppDomain.TrimEnd('/') + "/Cart/O2OToShop", objContent).Result;
            objContent.Dispose();
            //objClient.Dispose();
        }
        /// <summary>
        /// 讀取F05取貨完成檔內容, 呼叫IPP回寫訂單資訊
        /// </summary>
        /// <param name="argDate">訂單建立時間</param>
        /// <returns> Dictionary<新蛋訂單編號,訂單日期> </returns>
        public void SetOrderReturns(DateTime argDate)
        {
            HiLifeAnalysisRepository objAnalysisServer = null;
            List<F05Body> listF05Body = null;
            List<F02Body> listF02Body = null;
            F05Body objF05Body = null;
            List<TWNewEgg.DB.TWSQLDB.Models.GenerateNumberMap> listNumberMap = null;
            TWNewEgg.DB.TWSQLDB.Models.GenerateNumberMap objNewNumberMap = null;
            Dictionary<string, string> newDicF05 = null;
            List<string> list11Number = null;

            objAnalysisServer = new TWNewEgg.Logistics.HiLife.Service.HiLifeAnalysisRepository();

            //get F05 Body from DB
            listF05Body = objAnalysisServer.GetBodyByCreareDate<F05Body>(argDate);
            
            if(listF05Body != null)
            {
                //從F05的BC1與BC2, 查到F02的資料, 因F02Body才有訂單編號
                listF02Body = objAnalysisServer.GetF02BodyByBc1AndBc2<F05Body>(listF05Body);
                if (listF02Body != null)
                {
                    list11Number = new List<string>(listF02Body.Select(x => x.Odno));
                }

                listNumberMap = this.SearchNeweggOrderNumber(list11Number);
            }

            if (listNumberMap != null)
            {
                newDicF05 = new Dictionary<string, string>();
                foreach (F02Body objSubF02Body in listF02Body)
                {
                    objF05Body = listF05Body.FirstOrDefault(x => x.Bc1 == objSubF02Body.Bc1 && x.Bc2 == objSubF02Body.Bc2);
                    objNewNumberMap = listNumberMap.FirstOrDefault(x => x.GenerateNumber == objSubF02Body.Odno);
                    if (objF05Body != null && objNewNumberMap != null && !newDicF05.ContainsKey(objNewNumberMap.SourceNumber))
                    {
                        newDicF05.Add(objNewNumberMap.SourceNumber, objF05Body.Rtdt.Substring(0, 4) + "/" + objF05Body.Rtdt.Substring(4,2) + "/" + objF05Body.Rtdt.Substring(6,2));
                    }
                }
            }

            //呼叫IPP後台傳Dictionary
            string strJson = "";
            System.Net.Http.HttpContent objContent = null;
            System.Net.Http.HttpClient objClient = null;
            HttpResponseMessage objWebRsp = null;

            strJson = new JavaScriptSerializer().Serialize(newDicF05);
            strJson = "{\"cartlist\":" + strJson + "}";
            objContent = new StringContent(strJson);
            objContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");
            objClient = new System.Net.Http.HttpClient();
            objClient.DefaultRequestHeaders.Add("user-agent", "Mozilla/5.0 (compatible; MSIE 10.0; Windows NT 6.2; WOW64; Trident/6.0)");
            objWebRsp = objClient.PostAsync(this.mstrIppDomain.TrimEnd('/') + "/Cart/UpdateO2O", objContent).Result;

            objContent.Dispose();
            //objClient.Dispose();

            //釋放所有記憶體
            objAnalysisServer = null;
            listF05Body = null;
            listF02Body = null;
            objF05Body = null;
            listNumberMap = null;
            objNewNumberMap = null;
            newDicF05 = null;
            list11Number = null;

            /*listF05 = new List<F05Body>();
            listOldNumber = new List<string>();
            listF05 = objAnalysisServer.GetBodyByCreareDate<F05Body>(argDate);
            //將dictionary裡的Key值存在List
            foreach (F05Body subF05Body in listF05)
            {
                hiLifeNum = subF05Body.Bc1.Substring(3,3) + subF05Body.Bc2.Substring(0,8);
                listOldNumber.Add(hiLifeNum);
            }
             
            //轉碼，11(HiLife)碼轉15碼(NewEgg)
            //listNewNumberMap = new List<GenerateNumberMap>();
            listNewNumberMap = this.GetNeweggOrderNumber(listOldNumber);
            //objNewNumberMap=new GenerateNumberMap();
            if (listNewNumberMap != null)
            {
                foreach (F05Body subF05Body in listF05)
                {
                    objNewNumberMap = listNewNumberMap.FirstOrDefault(x => x.GenerateNumber.Equals(listOldNumber));
                    if (objNewNumberMap != null)
                    {
                        newDicF05.Add(objNewNumberMap.SourceNumber, Convert.ToDateTime(subF05Body.Rtdt.Substring(0,4) + "/" + subF05Body.Rtdt.Substring(4,2) + "/" + subF05Body.Rtdt.Substring(6,2)));
                    }
                }
            }

            if (listF05 != null)
            {
                listF05.Clear();
                listF05 = null;
            }
            if (listNewNumberMap != null)
            {
                listNewNumberMap.Clear();
                listNewNumberMap = null;
            }
            objAnalysisServer = null;
            */

            
        }

       

        #region
        /// <summary>
        /// 增加當日Hi-Life F02記錄
        /// </summary>
        /// <param name="argListF02Body"></param>
        /// <returns></returns>
        public bool AddShippingF02(List<F02Body> argListF02Body)
        {
            if (argListF02Body == null)
            {
                return false;
            }

            /*
             * 接到WMS來的出貨訂單F02Body的資料
             * 存到DB裡後, 轉存*.F02檔案
             */
            HiLifeAnalysisRepository objAnaService = null;
            HiLifeFileSyncRepository objFileService = null;
            Dictionary<string, SyncTimes> dictFileSync = null;
            SyncTimes.SyncFile objSyncFilePath = null;
            int numF02BodyCount = 0;
            bool boolExec = false;

            objAnaService = new HiLifeAnalysisRepository();
            objFileService = new HiLifeFileSyncRepository();

            //將接收的資料新增進DB
            foreach (F02Body objsubF02Body in argListF02Body)
            {
                try
                {
                    if (objAnaService.CreateBody<TWNewEgg.DB.Models.Logistics.HiLife.F02Body>(objsubF02Body) > 0)
                    {
                        numF02BodyCount++;
                    }

                }
                catch (Exception ex)
                {
                    boolExec = false;
                    string strMessage = "";
                    strMessage += "時間: " + DateTime.Now.ToString() + "\r\n";
                    strMessage += "Function: HiLifeFileSyncRepository.StartAllSchedule" + "\r\n";
                    strMessage += "錯誤訊息: " + ex.InnerException.Message + "\r\n\r\n";
                    objAnaService.LogMessage(strMessage);
                }
            }

            //取得路徑檔
            dictFileSync = objFileService.GetFileSync();
            // 判斷Key值是否存在
            if (dictFileSync.ContainsKey(HiLifeFileType.F02.ToString()) && dictFileSync[HiLifeFileType.F02.ToString()] != null)
            {
                objSyncFilePath = dictFileSync[HiLifeFileType.F02.ToString()].SyncFileInfo;
            }

            //產生F02的檔案
            boolExec = objAnaService.ParseDbToFile(HiLifeFileType.F02, DateTime.Now, DateTime.Now, objSyncFilePath.From);

            objAnaService = null;
            objFileService = null;

            return boolExec;
        }
        #endregion

         ///<summary>
         ///增加當日Hi-Life F06記錄(多筆)
         ///</summary>
         ///<param name="argListF06">退貨訂單F06Body的資料</param>
         ///<returns>從DB抓取資料轉成Txt檔成功:回傳true , 失敗回傳false</returns>
        public bool AddReturnF06Document(List<F06Body> argListF06)
        {
            /*
             * 接到Penny來的退貨訂單F06Body的資料
             * 存到DB裡後, 轉存*.F06檔案
             */
            HiLifeAnalysisRepository objAnalysisServer = null;
            //HiLifeFileSyncRepository objFileSync= null;
            Dictionary<string, Dictionary<string, Dictionary<string, HiLifeBasicFieldStruct>>> dictStruct = null;
            //Dictionary<string,SyncTimes> getFileSync=null;
            int successSave = 0;
            //取得路徑檔
            objAnalysisServer = new TWNewEgg.Logistics.HiLife.Service.HiLifeAnalysisRepository();
            dictStruct = objAnalysisServer.GetFieldStruct(mstrStructFilePath);
            List<TWNewEgg.DB.TWSQLDB.Models.GenerateNumberMap> listNewNumberMap = null;
            TWNewEgg.DB.TWSQLDB.Models.GenerateNumberMap objNewNumberMap = null;
            List<string> listOldNumber = null;
            int numHeadId = 0;
            bool boolExec = false;
            //SyncTimes.SyncFile objSyncFilePath = null;

            //先將List F06裡的訂單編號存在新的List
            listOldNumber = new List<string>(argListF06.Select(x => x.Odno).DefaultIfEmpty().ToList());
            //轉碼，15碼(NewEgg)轉11碼(HiLife)
            listNewNumberMap = this.SearchHiLifeNumber(listOldNumber);

            if (listNewNumberMap != null)
            {
                //將F06檔存進DB
                foreach (F06Body objSubBody in argListF06)
                {
                    objNewNumberMap = listNewNumberMap.FirstOrDefault(x => x.SourceNumber.Equals(objSubBody.Odno));
                    if (objNewNumberMap != null)
                    {
                        objSubBody.Odno = objNewNumberMap.GenerateNumber;
                        objSubBody.HeadId = numHeadId;
                        objAnalysisServer.CreateBody<TWNewEgg.DB.Models.Logistics.HiLife.F06Body>(objSubBody);
                    }
                }
                //判斷是否成功存進DB
                if (successSave != 0 || successSave != -1)
                {
                    boolExec = true;
                }
                //objFileSync=new HiLifeFileSyncRepository();
                //getFileSync=objFileSync.GetFileSync();
                ////判斷是否有F06這個KEY值
                //if(getFileSync.ContainsKey(HiLifeFileType.F06.ToString()) && getFileSync[HiLifeFileType.F06.ToString()] != null)
                //{
                //    //取得dictionary裡面Value欄位裡面的值
                //    objSyncFilePath = getFileSync[HiLifeFileType.F06.ToString()].SyncFileInfo;
                //}
                ////從DB抓取資料轉成Txt檔
                //boolExec = objAnalysisServer.ParseDbToFile(HiLifeFileType.F06, DateTime.Now, DateTime.Now, objSyncFilePath.From);
            }

            return boolExec;
        }

        /// <summary>
        /// 增加當日Hi-Life F06記錄(單筆)
        /// </summary>
        /// <param name="argF06Body">退貨訂單F06Body的資料</param>
        /// <returns>從DB抓取資料，成功回傳true，失敗回傳fales</returns>
        public bool AddReturnF06Document(F06Body argF06Body)
        {
            HiLifeAnalysisRepository objAnalysisServer = null;
            Dictionary<string, Dictionary<string, Dictionary<string, HiLifeBasicFieldStruct>>> dictStruct = null;
            TWNewEgg.DB.TWSQLDB.Models.GenerateNumberMap objNewNumberMap = null;
            objNewNumberMap=new GenerateNumberMap();
            F06Body newF06Body = null;
            newF06Body = new F06Body();
            int numHeadId = 0;
            int successSave = 0;
            bool boolExec = false;


            //取得路徑檔
            objAnalysisServer = new TWNewEgg.Logistics.HiLife.Service.HiLifeAnalysisRepository();
            dictStruct = objAnalysisServer.GetFieldStruct(mstrStructFilePath);

            //轉碼，15碼(NewEgg)轉11碼(HiLife)
            if (argF06Body != null)
            {
                objNewNumberMap = this.SearchHiLifeNumber(argF06Body.Odno);
            }

            //將F06檔存進DB
            if (objNewNumberMap != null)
            {
                newF06Body.Odno = objNewNumberMap.GenerateNumber;
                newF06Body.HeadId = numHeadId;
                successSave=objAnalysisServer.CreateBody<TWNewEgg.DB.Models.Logistics.HiLife.F06Body>(newF06Body);
            }
            //判斷是否成功存進DB
            if (successSave != 0 || successSave != -1)
            {
                boolExec = true;
            }

            return boolExec;
        }

        /// <summary>
        /// 從DB抓取F06資料，轉成TXT檔
        /// </summary>
        /// <param name="argDateTime">退貨訂單建立時間</param>
        /// <returns>轉檔成功回傳true , 失敗回傳fales</returns>
        public bool SaveToF06Document(DateTime argDateTime)
        {
            HiLifeFileSyncRepository objFileSync = null;
            SyncTimes.SyncFile objSyncFilePath = null;
            HiLifeAnalysisRepository objAnalysisServer = null;
            Dictionary<string,SyncTimes> getFileSync=null;
            bool boolExec = false;

            objAnalysisServer = new TWNewEgg.Logistics.HiLife.Service.HiLifeAnalysisRepository();
            objSyncFilePath = new SyncTimes.SyncFile();
            objFileSync=new HiLifeFileSyncRepository();
            getFileSync=objFileSync.GetFileSync();

            //判斷是否有F06這個KEY值
            if(getFileSync.ContainsKey(HiLifeFileType.F06.ToString()) && getFileSync[HiLifeFileType.F06.ToString()] != null)
            {
                //取得dictionary裡面Value欄位裡面的值
                objSyncFilePath = getFileSync[HiLifeFileType.F06.ToString()].SyncFileInfo;
            }
            //從DB抓取資料轉成Txt檔
            //boolExec = objAnalysisServer.ParseDbToFile(HiLifeFileType.F06, DateTime.Now, DateTime.Now, objSyncFilePath.From);
            boolExec = objAnalysisServer.ParseDbToFile(HiLifeFileType.F06, argDateTime, DateTime.Now, objSyncFilePath.From);
            
            return boolExec;
        }

        /// <summary>
        /// 取得Hi-Life的11碼訂單號碼
        /// </summary>
        /// <returns>true:產生實體檔案成功; false:產生實體檔案失敗</returns>
        public bool ParseF06Document()
        {
            return false;
        }

        /// <summary>
        /// 最得一個新的HiLifeNumber
        /// </summary>
        /// <param name="argStrSalesOrder"></param>
        /// <returns></returns>
        public string GetNewHiLifeNumber(string argStrSalesOrder)
        {
            /*
             * 原來傳入的argHiLifeOrderNumber為15碼
             * 舊碼1-3碼: 前綴字
             * 舊碼4-10碼: 日期
             * 舊碼11-15碼: 流水號
             * 
             * 新碼(共11碼)
             * 第1碼(共1碼):物流業者的代碼, 請參照NeweggCommonService.LogiticsPartner
             * 第2碼(共1碼): 年份
             * 第3-4碼(共2碼): 月份, 請參照NeweggcommonService.MonthCode
             * 第5-6碼(共2碼): 日期
             * 第7-11碼(共6碼): 流水號
             */

            NeweggCommonService.LogiticsPartner objLogiticsPartner = NeweggCommonService.LogiticsPartner.HiLife;
            string strDate = "";
            string strNumber = "";
            string strResult = "";
            int numNumber = 0;
            TWSqlDBContext objDb = null;
            SqlParameter objParam = null;
            System.Data.Common.DbCommand objDbCom = null;
            System.Data.Common.DbDataReader objDbReader = null;
            TWNewEgg.DB.TWSQLDB.Models.GenerateNumberMap objNumberMap = null;

            //取得最新的流水號
            objDb = new TWSqlDBContext();
            objDb.Database.Connection.Open();
            objDbCom = objDb.Database.Connection.CreateCommand();

            objParam = new SqlParameter();
            objParam.ParameterName = "@LogiticsCode";
            objParam.Value = Convert.ToChar(objLogiticsPartner);
            objDbCom.Parameters.Add(objParam);


            objDbCom.CommandText = "[dbo].[UP_EC_GetLogiticsNumber]@LogiticsCode";

            try
            {
                objDbReader = objDbCom.ExecuteReader();
                objDbCom.Parameters.Clear();
                if (objDbReader.HasRows)
                {
                    objDbReader.Read();
                    numNumber = objDbReader.GetInt32(0); //取得第一行第一列的資料
                }
            }
            catch (Exception ex)
            {
                string strMessage = "";
                strMessage += "時間: " + DateTime.Now.ToString() + "\r\n";
                strMessage += "Function: HiLifeForNewEggAPI.GetNewHiLifeOrderNumber" + "\r\n";
                strMessage += "錯誤訊息: " + ex.InnerException.Message + "\r\n\r\n";
                this.LogMessage(strMessage);
            }
            finally
            {
                //釋放DB相關的記憶體供下次使用
                if (objDbReader != null)
                {
                    objDbReader.Dispose();
                }

                if (objDbCom != null)
                {
                    objDbCom.Parameters.Clear();
                    objDbCom.Dispose();
                    objDbCom = null;
                }
                objDb.Database.Connection.Close();
            }

            //組合新的流水號碼
            if (numNumber > 0)
            {
                strNumber = numNumber.ToString().PadLeft(5, '0');

                //取得今日的日期編碼(共5碼)
                strDate = Convert.ToChar(Convert.ToInt32(DateTime.Now.Year.ToString().Substring(2, 2))+51).ToString();
                //strDate += ((NeweggCommonService.MonthCode)DateTime.Now.Month).ToString();
                strDate += DateTime.Now.Month.ToString().PadLeft(2, '0');
                strDate += DateTime.Now.Day.ToString().PadLeft(2, '0');

                //組合所有的號碼
                strResult = Convert.ToString(Convert.ToChar(objLogiticsPartner)) + strDate + strNumber;

                //寫入對應表裡
                objNumberMap = new DB.TWSQLDB.Models.GenerateNumberMap();
                objNumberMap.SourceNumber = argStrSalesOrder;
                objNumberMap.GenerateNumber= strResult;
                objDb.GenerateNumberMap.Add(objNumberMap);
                try
                {
                    objDb.SaveChanges();
                }
                catch (Exception ex)
                {
                    string strMessage = "";
                    strMessage += "時間: " + DateTime.Now.ToString() + "\r\n";
                    strMessage += "Function: HiLifeForNewEggAPI.GetNewHiLifeOrderNumber" + "\r\n";
                    strMessage += "錯誤訊息: " + ex.InnerException.Message + "\r\n\r\n";
                    this.LogMessage(strMessage);
                }
            }


            if (objDb != null)
            {
                objDb.Dispose();
                objDb = null;
            }


            return strResult;

        }

        /// <summary>
        /// 給HiLife編號, 回傳對應的新蛋訂單編號
        /// </summary>
        /// <param name="argListHiLifeNumber"></param>
        /// <returns></returns>
        public List<TWNewEgg.DB.TWSQLDB.Models.GenerateNumberMap> SearchNeweggOrderNumber(List<string> argListHiLifeNumber)
        {
            if (argListHiLifeNumber == null)
        {
            return null;
        }

            TWSqlDBContext objDb = null;
            List<GenerateNumberMap> listNumberMap = null;

            objDb = new TWSqlDBContext();
            listNumberMap = objDb.GenerateNumberMap.Where(x => argListHiLifeNumber.Contains(x.GenerateNumber)).DefaultIfEmpty().ToList();

            objDb.Dispose();

            return listNumberMap;
        }

        /// <summary>
        /// 根據HiLife編號, 回傳對應的新蛋訂單編號
        /// </summary>
        /// <param name="argStrOrderNumber">OrderNumber</param>
        /// <returns>HiLife Number</returns>
        public string SearchNeweggOrderNumber(string argStrOrderNumber)
        {
            TWSqlDBContext objDb = null;
            TWNewEgg.DB.TWSQLDB.Models.GenerateNumberMap objNumberMap = null;
            string strResult = "";

            objDb = new TWSqlDBContext();
            objNumberMap = objDb.GenerateNumberMap.SingleOrDefault(x => x.GenerateNumber.Equals(argStrOrderNumber));

            objDb.Dispose();

            if (objNumberMap != null)
            {
                strResult = objNumberMap.SourceNumber;
            }

            return strResult;
        }

        /// <summary>
        /// 依據新蛋訂單編號, 回傳對應的Hi-Life對應的編號
        /// </summary>
        /// <param name="argListNeweggNumber"></param>
        /// <returns></returns>
        public List<TWNewEgg.DB.TWSQLDB.Models.GenerateNumberMap> SearchHiLifeNumber(List<string> argListNeweggNumber)
        {
            if (argListNeweggNumber == null)
            {
            return null;
        }

            TWSqlDBContext objDb = null;
            List<GenerateNumberMap> listNumberMap = null;

            objDb = new TWSqlDBContext();
            listNumberMap = objDb.GenerateNumberMap.Where(x => argListNeweggNumber.Contains(x.SourceNumber)).DefaultIfEmpty().ToList();

            objDb.Dispose();

            return listNumberMap;
        }

        /// <summary>
        /// 根據Newegg訂單編號(15)碼查詢對應的Hi-Life編號(11碼)
        /// </summary>
        /// <param name="argStrNeweggNumber">Newegg訂單編號</param>
        /// <returns>GenerateNumberMap物件</returns>
        public TWNewEgg.DB.TWSQLDB.Models.GenerateNumberMap SearchHiLifeNumber(string argStrNeweggNumber)
        {
            TWSqlDBContext objDb = null;
            GenerateNumberMap objNumberMap = null;

            objDb = new TWSqlDBContext();
            objNumberMap = objDb.GenerateNumberMap.Where(x => x.SourceNumber == argStrNeweggNumber).FirstOrDefault();

            objDb.Dispose();

            return objNumberMap;
        }


        /// <summary>
        /// Log Message
        /// </summary>
        /// <param name="argStrMessage">Message</param>
        public void LogMessage(string argStrMessage)
        {
            DateTime objDate = DateTime.Now;
            string strFileName = "";
            byte[] byteMessage;

            strFileName = this.mstrLogFilePath + objDate.Year.ToString().PadLeft(4, '0') + objDate.Month.ToString().PadLeft(2, '0') + objDate.Day.ToString().PadLeft(2, '0') + ".APILog";

            try
            {
                if (this.mswLogMessage != null)
                {
                    if (!this.mswLogMessage.Name.ToUpper().Equals(strFileName.ToUpper()) || !this.mswLogMessage.CanWrite)
                    {
                        this.mswLogMessage.Close();
                        this.mswLogMessage = new FileStream(strFileName, FileMode.Append, FileAccess.Write);
                    }
                }
                else
                {
                    this.mswLogMessage = new FileStream(strFileName, FileMode.Append, FileAccess.Write);
                }

                byteMessage = System.Text.Encoding.GetEncoding("big5").GetBytes(argStrMessage);
                this.mswLogMessage.Write(byteMessage, 0, byteMessage.Length);
                this.mswLogMessage.Flush();
                this.mswLogMessage.Close();
                this.mswLogMessage.Dispose();
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return;
        }

        /// <summary>
        /// 取得流水編號
        /// </summary>
        /// <param name="argObjLogiticsPartner">Enum of NeweggCommonService.LogiticsPartner</param>
        /// <param name="argNumTotalLengh">流水號的總長度</param>
        /// <param name="argSymbol">前端填補的字元</param>
        /// <returns>流水編號</returns>
        public string GetSerialNumber(NeweggCommonService.LogiticsPartner argObjLogiticsPartner, int argNumTotalLengh, char argSymbol)
        {
            string strNumber = "";
            int numNumber = 0;
            TWSqlDBContext objDb = null;
            SqlParameter objParam = null;
            System.Data.Common.DbCommand objDbCom = null;
            System.Data.Common.DbDataReader objDbReader = null;

            //取得最新的流水號
            objDb = new TWSqlDBContext();
            objDb.Database.Connection.Open();
            objDbCom = objDb.Database.Connection.CreateCommand();

            objParam = new SqlParameter();
            objParam.ParameterName = "@LogiticsCode";
            objParam.Value = Convert.ToChar(argObjLogiticsPartner);
            objDbCom.Parameters.Add(objParam);


            objDbCom.CommandText = "[dbo].[UP_EC_GetLogiticsNumber]@LogiticsCode";

            try
            {
                objDbReader = objDbCom.ExecuteReader();
                objDbCom.Parameters.Clear();
                if (objDbReader.HasRows)
                {
                    objDbReader.Read();
                    numNumber = objDbReader.GetInt32(0); //取得第一行第一列的資料
                }
            }
            catch (Exception ex)
            {
                string strMessage = "";
                strMessage += "時間: " + DateTime.Now.ToString() + "\r\n";
                strMessage += "Function: HiLifeForNewEggAPI.GetSerialNumber" + "\r\n";
                strMessage += "錯誤訊息: " + ex.InnerException.Message + "\r\n\r\n";
                this.LogMessage(strMessage);
            }
            finally
            {
                //釋放DB相關的記憶體供下次使用
                if (objDbReader != null)
                {
                    objDbReader.Dispose();
                }

                if (objDbCom != null)
                {
                    objDbCom.Parameters.Clear();
                    objDbCom.Dispose();
                    objDbCom = null;
                }
                objDb.Database.Connection.Close();

                if (objDb != null)
                {
                    objDb.Dispose();
                    objDb = null;
                }
            }

            //組合新的流水號碼
            if (numNumber > 0)
            {
                strNumber = numNumber.ToString().PadLeft(argNumTotalLengh, argSymbol);
            }


            return strNumber;
        }
                
    }
}
