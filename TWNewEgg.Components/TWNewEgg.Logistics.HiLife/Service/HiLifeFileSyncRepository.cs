using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using System.IO;
using TWNewEgg.Logistics.HiLife.Model;
using TWNewEgg.DB.Models.Logistics.HiLife;

namespace TWNewEgg.Logistics.HiLife.Service
{
    /// <summary>
    /// HiLife檔案同步
    /// </summary>
    public class HiLifeFileSyncRepository : IHiLifeFileSync
    {
        private Dictionary<string, Dictionary<string, Dictionary<string, HiLifeBasicFieldStruct>>> mobjDictFieldStruct = null;
        private Dictionary<string, SyncTimes> mobjDictFileSync = null;
        private Dictionary<string, Thread> mdictThreadJob = null;
        private string mstrLogFilePath =  System.Configuration.ConfigurationManager.AppSettings["LogFilePath"];
        private FileStream mswLogMessage = null;
        private string mstrStructFilePath = System.Configuration.ConfigurationManager.AppSettings["StructFilePath"];

        /// <summary>
        /// 建構函式
        /// </summary>
        /// <param name="argStrConfigFilePath">結構檔的完整路徑及檔名</param>
        public HiLifeFileSyncRepository(string argStrConfigFilePath)
        {
            if (argStrConfigFilePath.Length >= 0)
            {
                this.LoadFileSyncSchedule(argStrConfigFilePath);
            }
        }

        public HiLifeFileSyncRepository()
        {
            
            this.LoadFileSyncSchedule(this.mstrStructFilePath);
        }
        
        /// <summary>
        /// 讀取同步設定檔並做相關的建置
        /// </summary>
        /// <param name="argStrConfigFilePath">結構檔的完整路徑及檔名</param>
        /// <returns>讀取成功並且建置成功回傳true, 否則回傳false</returns>
        public bool LoadFileSyncSchedule(string argStrConfigFilePath)
        {
            if (this.mobjDictFieldStruct == null)
            {
                this.mobjDictFieldStruct = new Dictionary<string, Dictionary<string, Dictionary<string, HiLifeBasicFieldStruct>>>();
            }
            else
            {
                this.mobjDictFieldStruct.Clear();
            }

            if (this.mobjDictFileSync == null)
            {
                this.mobjDictFileSync = new Dictionary<string, SyncTimes>();
            }
            else
            {
                this.mobjDictFileSync.Clear();
            }

            if (this.mdictThreadJob == null)
            {
                this.mdictThreadJob = new Dictionary<string, Thread>();
            }

            bool boolExec = true;
            XmlDocument objXmlDoc = null;
            XmlNodeList listFileNode = null;

            //檔案結構相關的變數
            SyncTimes.SyncFile objSyncFileInfo = null;
            SyncTimes objSyncTimes = null;
            List<SyncTimes.PerSyncTime> listSyncTimes = null;
            SyncTimes.PerSyncTime objPerTime = null;

            //欄位結構相關的變數
            Dictionary<string, Dictionary<string, HiLifeBasicFieldStruct>> objDictFileType = null;
            Dictionary<string, HiLifeBasicFieldStruct> objDictFieldStruct = null;

            //暫存用的變數
            XmlNode objTempNode = null;
            XmlNodeList objTempNodeList = null;
            string[] aryTempStr;

            //根據傳入路徑載入檔案
            objXmlDoc = new XmlDocument();
            objXmlDoc.Load(argStrConfigFilePath);
            if (objXmlDoc != null)
            {
                //取得所有File的節點
                listFileNode = objXmlDoc.SelectNodes(".//File");
                if (listFileNode != null)
                {
                    foreach (XmlNode objNodeFile in listFileNode)
                    {
                        try
                        {
                            //------ start of 解析同步檔案及時間 ------
                            #region 解析同步檔案及時間
                            objSyncTimes = new SyncTimes();

                            //讀取同步來源與目的地的檔案資訊
                            objSyncFileInfo = new SyncTimes.SyncFile();
                            objTempNode = objNodeFile.SelectSingleNode(".//FilePath/From");
                            if (objTempNode != null)
                            {
                                objSyncFileInfo.From = objTempNode.InnerText;
                            }

                            objTempNode = objNodeFile.SelectSingleNode(".//FilePath/To");
                            if (objTempNode != null)
                            {
                                objSyncFileInfo.To = objTempNode.InnerText;
                            }

                            objSyncTimes.SyncFileInfo = objSyncFileInfo;

                            //讀取同步檔案時間資訊
                            listSyncTimes = new List<SyncTimes.PerSyncTime>();
                            objTempNode = objNodeFile.SelectSingleNode(".//Sync/Times");
                            if (objTempNode != null)
                            {
                                //取得同步時間的節點
                                objTempNodeList = objTempNode.SelectNodes(".//Time");
                                if (objTempNodeList != null)
                                {
                                    //將時間節點的資料分析
                                    foreach (XmlNode objTimeNode in objTempNodeList)
                                    {
                                        objPerTime = new SyncTimes.PerSyncTime();
                                        aryTempStr = objTimeNode.InnerText.Split('-');
                                        //星期
                                        objPerTime.Week = (SyncTimes.SyncWeek)Enum.Parse(typeof(SyncTimes.SyncWeek), aryTempStr[0]);
                                        //時間
                                        aryTempStr = aryTempStr[1].Split(':');
                                        objPerTime.Hour = Convert.ToInt32(aryTempStr[0]);
                                        objPerTime.Minutes = Convert.ToInt32(aryTempStr[1]);

                                        listSyncTimes.Add(objPerTime);
                                    }
                                }
                            }

                            objSyncTimes.SyncTimeList = listSyncTimes;

                            //將資料存入結構裡
                            this.mobjDictFileSync.Add(objNodeFile.Attributes["Type"].Value, objSyncTimes);
                            #endregion
                            //------ end of 解析同步檔案及時間 ------

                            //------ start of 解析欄位結構 ------
                            #region 解析欄位結構
                            objDictFileType = new Dictionary<string, Dictionary<string, HiLifeBasicFieldStruct>>();

                            //Head
                            objTempNode = objNodeFile.SelectSingleNode(".//Head");
                            if (objTempNode != null)
                            {
                                objDictFieldStruct = this.GetListField(objTempNode);
                                objDictFileType.Add("Head", objDictFieldStruct);
                            }

                            //Body
                            objTempNode = objNodeFile.SelectSingleNode(".//Body");
                            if (objTempNode != null)
                            {
                                objDictFieldStruct = this.GetListField(objTempNode);
                                objDictFileType.Add("Body", objDictFieldStruct);
                            }

                            //Tail
                            objTempNode = objNodeFile.SelectSingleNode(".//Tail");
                            if (objTempNode != null)
                            {
                                objDictFieldStruct = this.GetListField(objTempNode);
                                objDictFileType.Add("Tail", objDictFieldStruct);
                            }

                            this.mobjDictFieldStruct.Add(objNodeFile.Attributes["Type"].Value, objDictFileType);
                            #endregion
                            ////------ end of 解析欄位結構 ------
                        }
                        catch (Exception ex)
                        {
                            boolExec = false;
                            string strMessage = "";
                            strMessage += "時間: " + DateTime.Now.ToString() + "\r\n";
                            strMessage += "Function: HiLifeFileSyncRepository.LoadFileSyncSchedule" + "\r\n";
                            strMessage += "錯誤訊息: " + ex.Message + "\r\n\r\n";
                            this.LogSyncMessage(strMessage);
                        }
                    }
                    //end foreach
                }

                //取得LogFile的Path
                objTempNode = objXmlDoc.SelectSingleNode(".//LogFilePath");
                if (objTempNode != null)
                {
                    this.mstrLogFilePath = objTempNode.InnerText;
                }
            }

            return boolExec;
        }


        /// <summary>
        /// 將傳入字串依設定進行正規補齊或截斷
        /// </summary>
        /// <param name="argStrValue">傳入字串</param>
        /// <param name="argNumTargetLengh">規定長度</param>
        /// <param name="argPadSymbol">補齊符號</param>
        /// <param name="argAlign">對齊方式</param>
        /// <returns>正規格式的字串</returns>
        public string ParseText(string argStrValue, int argNumTargetLengh, char argPadSymbol, HiLifeBasic.Align argAlign)
        {
            System.Text.Encoding objEncoding = System.Text.Encoding.GetEncoding("big5");
            byte[] byteString = objEncoding.GetBytes(argStrValue);
            string strRes = "";
            string strResult = "";

            strResult = argStrValue;
            if (byteString.Length > argNumTargetLengh)
            {
                strRes = objEncoding.GetString(byteString, 0, argNumTargetLengh);
                if (!objEncoding.GetString(byteString).StartsWith(strRes))
                {
                    //中文字截斷的判斷
                    strRes = objEncoding.GetString(byteString, 0, argNumTargetLengh - 1);
                }

                strResult = strRes;
            }
            else if (byteString.Length < argNumTargetLengh)
            {
                if (argAlign == HiLifeBasic.Align.Left)
                {
                    strResult = strResult.PadRight(argNumTargetLengh, argPadSymbol);
                }
                else
                {
                    strResult = strResult.PadLeft(argNumTargetLengh, argPadSymbol);
                }
            }

            return strResult;
        }
                
        /// <summary>
        /// Log Message
        /// </summary>
        /// <param name="argStrMessage">Message</param>
        public void LogSyncMessage(string argStrMessage)
        {
            DateTime objDate = DateTime.Now;
            string strFileName = "";
            byte[] byteMessage;

            strFileName = this.mstrLogFilePath + objDate.Year.ToString().PadLeft(4, '0') + objDate.Month.ToString().PadLeft(2, '0') + objDate.Day.ToString().PadLeft(2, '0') + ".FileSyncLog";

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
        /// 設定基底Field的資料
        /// </summary>
        /// <param name="argNodeParent">欲尋找的Note節點</param>
        /// <returns>Dictionary of Field Data</returns>
        private Dictionary<string, HiLifeBasicFieldStruct> GetListField(XmlNode argNodeParent)
        {
            XmlNodeList objNodeListField = null;
            Dictionary<string, HiLifeBasicFieldStruct> dictField = null;
            HiLifeBasicFieldStruct objField = null;

            //根據節點建立資訊
            dictField = new Dictionary<string, HiLifeBasicFieldStruct>();
            objNodeListField = argNodeParent.SelectNodes(".//Field");
            if (objNodeListField != null)
            {
                foreach (XmlNode objSubNode in objNodeListField)
                {
                    objField = new HiLifeBasicFieldStruct();
                    objField.Name = objSubNode.Attributes["Name"].Value;
                    objField.Length = Convert.ToInt32(objSubNode.Attributes["Length"].Value);
                    if (objSubNode.Attributes["PadSymbol"].Value.Length == 1)
                    {
                        objField.Symbol = Convert.ToChar(objSubNode.Attributes["PadSymbol"].Value);
                    }
                    else if (objSubNode.Attributes["PadSymbol"].Value.Length == 0)
                    {
                        objField.Symbol = ' ';
                    }
                    else
                    {
                        objField.Symbol = Convert.ToChar(objSubNode.Attributes["PadSymbol"].Value.Substring(0, 1));
                    }

                    switch (objSubNode.Attributes["Align"].Value.ToUpper())
                    {
                        case "LEFT":
                            objField.Align = HiLifeBasic.Align.Left;
                            break;
                        case "RIGHT":
                            objField.Align = HiLifeBasic.Align.Right;
                            break;
                        default:
                            objField.Align = HiLifeBasic.Align.None;
                            break;
                    }

                    //是否有固定值
                    if (objSubNode.Attributes["StaticValue"] != null)
                    {
                        objField.HasStaticValue = true;
                        objField.StaticValue = objSubNode.Attributes["StaticValue"].Value;
                    }
                    else
                    {
                        objField.HasStaticValue = false;
                    }

                    dictField.Add(objField.Name, objField);
                }
                //end foreach
            }

            return dictField;
        }

        /// <summary>
        /// 根據FileTime傳回離下次執行的時間有多少毫秒
        /// </summary>
        /// <param name="argFileType">HiLife File Type</param>
        /// <returns>-1 or micro seconds</returns>
        public int UntilNextTime(HiLifeFileType argFileType)
        {
            double numMicorSeconds = -1.0;
            DateTime objDateNow = DateTime.Now;
            SyncTimes objSyncTimes = null;
            SyncTimes.SyncWeek objNowWeek = SyncTimes.SyncWeek.Mon;
            List<SyncTimes.PerSyncTime> listPerSyncTime = null;
            SyncTimes.PerSyncTime objNextPerSyncTime = null;
            TimeSpan objNowSpan = new TimeSpan();
            TimeSpan objNextSpan = new TimeSpan();

            //若有任何錯誤或無資料無法判定時間, 就回傳-1
            //檢查同步檔案是否有這個資料型態
            if (this.mobjDictFileSync == null || this.mobjDictFileSync.Count <= 0)
            {
                return -1;
            }
            //檢查這個資料是否有File相關的資訊
            objSyncTimes = this.mobjDictFileSync[argFileType.ToString()];
            if (objSyncTimes == null)
            {
                return -1;
            }
            //檢查File相關的資訊裡, 是否有日期檔案
            listPerSyncTime = objSyncTimes.SyncTimeList;
            if (listPerSyncTime == null || listPerSyncTime.Count <= 0)
            {
                return -1;
            }

            //根據今日的時間去判斷執行時間
            switch (objDateNow.DayOfWeek)
            {
                case DayOfWeek.Monday:
                    objNowWeek = SyncTimes.SyncWeek.Mon;
                    break;
                case DayOfWeek.Tuesday:
                    objNowWeek = SyncTimes.SyncWeek.Tue;
                    break;
                case DayOfWeek.Wednesday:
                    objNowWeek = SyncTimes.SyncWeek.Wed;
                    break;
                case DayOfWeek.Thursday:
                    objNowWeek = SyncTimes.SyncWeek.Thu;
                    break;
                case DayOfWeek.Friday:
                    objNowWeek = SyncTimes.SyncWeek.Fri;
                    break;
                case DayOfWeek.Saturday:
                    objNowWeek = SyncTimes.SyncWeek.Sat;
                    break;
                case DayOfWeek.Sunday:
                    objNowWeek = SyncTimes.SyncWeek.Sun;
                    break;
            }

            //比對今日執行時間裡是否有符合的資料
            objNextPerSyncTime = listPerSyncTime.Where(x => Convert.ToInt32(x.Week) == Convert.ToInt32(objNowWeek) && (x.Hour > objDateNow.Hour || (x.Hour == objDateNow.Hour && x.Minutes > objDateNow.Minute))).FirstOrDefault();

            //若今日執行時間裡沒有符合的資料,就找與今日距離最近的符合資料
            if (objNextPerSyncTime == null)
            {
                foreach (SyncTimes.PerSyncTime objSubPerTime in listPerSyncTime)
                {
                    objNextPerSyncTime = listPerSyncTime.Where(x => Convert.ToInt32(x.Week) == (Convert.ToInt32(objNowWeek) + 1)).FirstOrDefault();
                    if (objNextPerSyncTime != null)
                    {
                        //若有找到下次距離最近的資料就跳出迴圈
                        break;
                    }
                }
            }

            //開始進行時間比對並進行計算
            if (objNextPerSyncTime != null)
            {
                //計算現在到下次DateTime是多久
                objNowSpan = new TimeSpan(Convert.ToInt32(objNowWeek), objDateNow.Hour, objDateNow.Minute, objNowSpan.Seconds);
                objNextSpan = new TimeSpan(Convert.ToInt32(objNextPerSyncTime.Week), objNextPerSyncTime.Hour, objNextPerSyncTime.Minutes, 0);

                numMicorSeconds = (objNextSpan - objNowSpan).TotalMilliseconds;
            }

            //釋放記憶體
            objSyncTimes = null;
            listPerSyncTime = null;
            objNextPerSyncTime = null;

            return Convert.ToInt32(numMicorSeconds);
        }

        /// <summary>
        /// 根據FileType尋找Xml的設定,根據Xml的設定Copy檔案
        /// </summary>
        /// <param name="argFileCopyControl">FileCopyControl的物件</param>
        public void CopyFile(object argFileCopyControl)
        {
            if (argFileCopyControl == null)
            {
                return;
            }

            FileCopyControl objFileControl = null;
            int numNextTime = 0;
            bool boolExec = false;
            string strFromPath = "";
            string strToPath = "";
            string strDate = "";
            string strToPathDirectory = "";
            bool boolAlreadyFirstSync = false;
            HiLifeAnalysisRepository objAnaService = null;

            //傳入參數轉型
            try
            {
                objFileControl = (FileCopyControl)argFileCopyControl;
                boolExec = true;
            }
            catch (Exception ex)
            {
                boolExec = false;
                string strMessage = "";
                strMessage += "時間: " + DateTime.Now.ToString() + "\r\n";
                strMessage += "Function: HiLifeFileSyncRepository.CopyFile" + "\r\n";
                strMessage += "FileType: FileCopyControl轉型錯誤\r\n";
                strMessage += "錯誤訊息: " + ex.Message + "\r\n\r\n";
                this.LogSyncMessage(strMessage);
            }

            //轉型成功
            if (boolExec)
            {
                numNextTime = this.UntilNextTime(objFileControl.FileType);
                strFromPath = this.mobjDictFileSync[objFileControl.FileType.ToString()].SyncFileInfo.From;
                strToPath = this.mobjDictFileSync[objFileControl.FileType.ToString()].SyncFileInfo.To;
                strToPathDirectory = this.mobjDictFileSync[objFileControl.FileType.ToString()].SyncFileInfo.To;

                while (true)
                {
                    strFromPath = this.mobjDictFileSync[objFileControl.FileType.ToString()].SyncFileInfo.From;
                    strToPath = this.mobjDictFileSync[objFileControl.FileType.ToString()].SyncFileInfo.To;
                    strToPathDirectory = this.mobjDictFileSync[objFileControl.FileType.ToString()].SyncFileInfo.To;

                    try
                    {
                        if (!objFileControl.SyncFirst && !boolAlreadyFirstSync)
                        {
                            Thread.Sleep(numNextTime);
                        }

                        //檔名組合日期
                        strDate = DateTime.Now.Year.ToString() + DateTime.Now.Month.ToString().PadLeft(2, '0') + DateTime.Now.Day.ToString().PadLeft(2, '0');
                        switch (objFileControl.FileType)
                        {
                            case HiLifeFileType.F00:
                                strFromPath = strFromPath + strDate + ".F00";
                                strToPath = strToPath + strDate + ".F00";
                                break;
                            case HiLifeFileType.F01:
                                strFromPath = strFromPath + strDate + ".F01";
                                strToPath = strToPath + strDate + ".F01";
                                break;
                            case HiLifeFileType.F02:
                                strFromPath = strFromPath + strDate + ".F02";
                                strToPath = strToPath + strDate + ".F02";
                                break;
                            case HiLifeFileType.F03:
                                strFromPath = strFromPath + strDate + ".F03";
                                strToPath = strToPath + strDate + ".F03";
                                break;
                            case HiLifeFileType.F04:
                                strFromPath = strFromPath + strDate + ".F04";
                                strToPath = strToPath + strDate + ".F04";
                                break;
                            case HiLifeFileType.F05:
                                strFromPath = strFromPath + strDate + ".F05";
                                strToPath = strToPath + strDate + ".F05";
                                break;
                            case HiLifeFileType.F06:
                                strFromPath = strFromPath + strDate + ".F06";
                                strToPath = strToPath + strDate + ".F06";
                                break;
                            case HiLifeFileType.F08:
                                strFromPath = strFromPath + strDate + ".F08";
                                strToPath = strToPath + strDate + ".F08";
                                break;
                            case HiLifeFileType.F09:
                                strFromPath = strFromPath + strDate + ".F09";
                                strToPath = strToPath + strDate + ".F09";
                                break;
                            case HiLifeFileType.TXT:
                                strFromPath = strFromPath + strDate + ".TXT";
                                strToPath = strToPath + strDate + ".TXT";
                                break;
                        }
                        if (File.Exists(strFromPath))
                        {
                            if (!Directory.Exists(strToPathDirectory))
                            {
                                Directory.CreateDirectory(strToPathDirectory);
                            }

                            File.Copy(strFromPath, strToPath, true);
                            //File.Move(strFromPath, strToPath);
                            objAnaService = new HiLifeAnalysisRepository();
                            objAnaService.ParseFileToDb(objFileControl.FileType, strToPath, Encoding.GetEncoding("big5"));
                            objAnaService = null;
                        }
                    }
                    catch (Exception ex)
                    {
                        string strMessage = "";
                        strMessage += "時間: " + DateTime.Now.ToString() + "\r\n";
                        strMessage += "Function: HiLifeFileSyncRepository.CopyFile" + "\r\n";
                        strMessage += "FileType: " + objFileControl.FileType.ToString() + "\r\n";
                        strMessage += "錯誤訊息: " + ex.Message + "\r\n\r\n";
                        this.LogSyncMessage(strMessage);
                    }

                    boolAlreadyFirstSync = true;
                    numNextTime = this.UntilNextTime(objFileControl.FileType);
                    Thread.Sleep(numNextTime);
                }
            }
        }

        /// <summary>
        /// 新增一個檔案排程
        /// </summary>
        /// <param name="argFileType">FileType</param>
        /// <param name="argBoolSyncFirst">true:無視時間先Copy資料再進入排程, false:依排程設定copy資料</param>
        /// <returns>設定排程成功:true, else return false</returns>
        public bool StartNewSyncSchedule(HiLifeFileType argFileType, bool argBoolSyncFirst)
        {
            bool boolExec = false;
            FileCopyControl objFileControl = null;
            object arguFileCopyControl = null;
            Thread objThread = null;
            ParameterizedThreadStart objThreadStart = null;

            try
            {
                if (this.mdictThreadJob.ContainsKey(argFileType.ToString()))
                {
                    objThread = this.mdictThreadJob[argFileType.ToString()];
                    if (objThread.ThreadState == ThreadState.Running)
                    {
                        objThread.Abort();
                    }

                    this.mdictThreadJob.Remove(argFileType.ToString());
                }

                objThreadStart = new ParameterizedThreadStart(this.CopyFile);
                objThread = new Thread(objThreadStart);
                this.mdictThreadJob.Add(argFileType.ToString(), objThread);

                objFileControl = new FileCopyControl();
                objFileControl.FileType = argFileType;
                objFileControl.SyncFirst = argBoolSyncFirst;
                arguFileCopyControl = (object)objFileControl;
                objThread.Start(arguFileCopyControl);

                boolExec = true;

                //釋放記憶體
                objFileControl = null;
            }
            catch (Exception ex)
            {
                boolExec = false;
                string strMessage = "";
                strMessage += "時間: " + DateTime.Now.ToString() + "\r\n";
                strMessage += "Function: HiLifeFileSyncRepository.StartNewSyncSchedule" + "\r\n";
                strMessage += "FileType: " + argFileType.ToString() + "\r\n";
                strMessage += "錯誤訊息: " + ex.Message + "\r\n\r\n";
                this.LogSyncMessage(strMessage);
            }

            return boolExec;
        }

        /// <summary>
        /// 啟動所有檔案同步服務
        /// </summary>
        /// <param name="argBoolSyncFirst">true:重啟時先同步所有檔案; false:重啟之後,檔案依排程時間再同步</param>
        /// <returns>true:啟動成功; false:啟動失敗</returns>
        public bool StartAllSchedule(bool argBoolSyncFirst)
        {
            if (this.mdictThreadJob == null)
            {
                return false;
            }

            FileCopyControl objFileControl = null;
            List<string> listThreadName = null;
            int numStartThreadCount = 0;
            int numTotalThreadCount = 0;
            Thread objThread = null;
            bool boolExec = true;

            //listThreadName = new List<string>(this.mdictThreadJob.Keys);
            listThreadName = new List<string>(this.mobjDictFieldStruct.Keys);

            if (listThreadName != null && listThreadName.Count > 0)
            {
                objFileControl = new FileCopyControl();
                objFileControl.SyncFirst = argBoolSyncFirst;
                numTotalThreadCount = listThreadName.Count;
                foreach (string strThreadName in listThreadName)
                {
                    try
                    {
                        //啟動服務
                        objFileControl.FileType = (HiLifeFileType)Enum.Parse(typeof(HiLifeFileType), strThreadName);
                        if (this.mdictThreadJob.ContainsKey(strThreadName))
                        {
                            objThread = this.mdictThreadJob[strThreadName];
                            objThread.Abort();
                            this.mdictThreadJob.Remove(strThreadName);
                        }

                        objThread = new Thread(new ParameterizedThreadStart(this.CopyFile));
                        this.mdictThreadJob.Add(strThreadName, objThread);
                        objThread.Start(objFileControl);

                        numStartThreadCount++;
                    }
                    catch (Exception ex)
                    {
                        boolExec = false;
                        string strMessage = "";
                        strMessage += "時間: " + DateTime.Now.ToString() + "\r\n";
                        strMessage += "Function: HiLifeFileSyncRepository.StartAllSchedule" + "\r\n";
                        strMessage += "錯誤訊息: " + ex.Message + "\r\n\r\n";
                        this.LogSyncMessage(strMessage);
                    }
                }
                //end foreach
            }

            //釋放記憶體
            objFileControl = null;
            listThreadName = null;

            return boolExec;
        }

        /// <summary>
        /// 停止所有檔案同步服務
        /// </summary>
        /// <returns>true: 停止成功; false:停止失敗</returns>
        public bool StopAllSchedule()
        {
            if (this.mdictThreadJob == null)
            {
                return false;
            }

            List<string> listThreadName = null;
            int numStartThreadCount = 0;
            int numTotalThreadCount = 0;
            Thread objThread = null;
            bool boolExec = true;

            listThreadName = new List<string>(this.mdictThreadJob.Keys);

            if (listThreadName != null && listThreadName.Count > 0)
            {
                numTotalThreadCount = listThreadName.Count;
                foreach (string strThreadName in listThreadName)
                {
                    try
                    {
                        //停止服務
                        objThread = this.mdictThreadJob[strThreadName];
                        if (objThread.ThreadState == ThreadState.Running)
                        {
                            objThread.Abort();
                        }

                        numStartThreadCount++;
                    }
                    catch (Exception ex)
                    {
                        boolExec = false;
                        string strMessage = "";
                        strMessage += "時間: " + DateTime.Now.ToString() + "\r\n";
                        strMessage += "Function: HiLifeFileSyncRepository.StopAllSchedule" + "\r\n";
                        strMessage += "錯誤訊息: " + ex.Message + "\r\n\r\n";
                        this.LogSyncMessage(strMessage);
                    }
                }
                //end foreach
            }

            //釋放記憶體
            listThreadName = null;

            return boolExec;
        }

        /// <summary>
        /// 暫停該檔的排程
        /// </summary>
        /// <param name="argFileType">HiLifeFileType</param>
        /// <returns>true:停止成功; false:停止失敗</returns>
        public bool StopSyncSchedule(HiLifeFileType argFileType)
        {
            if (this.mdictThreadJob == null)
            {
                return false;
            }

            Thread objThread = null;
            bool boolExec = false;
            string strThreadName = "";

            strThreadName = argFileType.ToString();
            if (!this.mdictThreadJob.ContainsKey(strThreadName))
            {
                return boolExec;
            }

            try
            {
                objThread = this.mdictThreadJob[strThreadName];
                //停止服務
                if (objThread != null)
                {
                    objThread.Abort();
                }

                boolExec = true;
            }
            catch (Exception ex)
            {
                boolExec = false;
                string strMessage = "";
                strMessage += "時間: " + DateTime.Now.ToString() + "\r\n";
                strMessage += "Function: HiLifeFileSyncRepository.StopSyncSchedule" + "\r\n";
                strMessage += "FileType: " + argFileType.ToString() + "\r\n";
                strMessage += "錯誤訊息: " + ex.Message + "\r\n\r\n";
                this.LogSyncMessage(strMessage);
            }

            return boolExec;
        }

        /// <summary>
        /// 取得欄位結構物件
        /// </summary>
        /// <returns>根據XML所解析所有FileType Field結構, 並存放在Dictionary</returns>
        public Dictionary<string, Dictionary<string, Dictionary<string, HiLifeBasicFieldStruct>>> GetFieldStruct()
        {
            return this.mobjDictFieldStruct;
        }

        /// <summary>
        /// 設定錯誤訊息Log檔案的目錄(僅目錄)
        /// </summary>
        /// <param name="argStrFilePath">錯誤訊息Log檔案的目錄</param>
        public void SetLogFilePath(string argStrFilePath)
        {
            if (argStrFilePath.Length <= 0)
            {
                return;
            }

            this.mstrLogFilePath = argStrFilePath.TrimEnd('\\') + "\\";
        }

        /// <summary>
        /// 取得檔案同步時間、同步路徑
        /// </summary>
        /// <returns>Dictionary<F00, 同步路徑物件></returns>
        public Dictionary<string, SyncTimes> GetFileSync()
        {
            return this.mobjDictFileSync;
        }
    }
    //end class
}
