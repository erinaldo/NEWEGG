using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Xml;
using TWNewEgg.DB;
using TWNewEgg.DB.Models.Logistics.HiLife;
using TWNewEgg.Logistics.HiLife.Model;

namespace TWNewEgg.Logistics.HiLife.Service
{
    /// <summary>
    /// HiLife Data Analysis Model
    /// </summary>
    public class HiLifeAnalysisRepository : IHiLifeAnalysis
    {
        private Dictionary<string, Dictionary<string, Dictionary<string, HiLifeBasicFieldStruct>>> mdictFieldStruct = null;
        private HiLifeFileSyncRepository mobjFileService = null;
        private string mstrLogFilePath = System.Configuration.ConfigurationManager.AppSettings["LogFilePath"];
        private FileStream mswLogMessage = null;

        public HiLifeAnalysisRepository()
        {
            this.mobjFileService = new HiLifeFileSyncRepository();
            mdictFieldStruct = this.mobjFileService.GetFieldStruct();
        }

        /// <summary>
        /// 根據HeadId取得整份文件
        /// </summary>
        /// <typeparam name="T">HiLifeF00, HiLifeF01, HiLifeF02....HiLifeF09, HiLifeTxt</typeparam>
        /// <param name="argNumHeadId">HeadId</param>
        /// <returns>HiLifeF00, HiLifeF01, HiLifeF02....HiLifeF09, HiLifeTxt的資料</returns>
        public T GetHiLifeDocument<T>(int argNumHeadId)
        {
            if (argNumHeadId <= 0)
            {
                return default(T);
            }

            object objHiLife = null;

            HiLifeDbModelType objHiLifeModelType = HiLifeDbModelType.Unknown;

            // 判定T的Model名稱,轉為列舉
            objHiLifeModelType = (HiLifeDbModelType)Enum.Parse(typeof(HiLifeDbModelType), typeof(T).Name);

            switch (objHiLifeModelType)
            {
                case HiLifeDbModelType.HiLifeF00:
                    // HiLifeF00只有Body
                    HiLifeF00 objHiLifeF00 = new HiLifeF00();
                    List<F00Body> listF00Body = new List<F00Body>();
                    F00Body objF00Body = this.GetBody<F00Body>(argNumHeadId);
                    if (objHiLifeF00 != null)
                    {
                        listF00Body = new List<F00Body>();
                        listF00Body.Add(objF00Body);
                        objHiLifeF00.body = listF00Body;
                        objHiLife = objHiLifeF00;
                    }
                    else
                    {
                        objHiLife = null;
                    }

                    break;
                case HiLifeDbModelType.HiLifeF01:
                    HiLifeF01 objHiLifeF01 = new HiLifeF01();
                    objHiLifeF01.head = this.GetHead<TWNewEgg.DB.Models.Logistics.HiLife.F01Head>(argNumHeadId);
                    objHiLifeF01.body = this.GetBodyByHeadId<TWNewEgg.DB.Models.Logistics.HiLife.F01Body>(argNumHeadId);
                    objHiLifeF01.tail = this.GetTailByHeadId<TWNewEgg.DB.Models.Logistics.HiLife.F01Tail>(argNumHeadId);
                    objHiLife = objHiLifeF01;
                    break;
                case HiLifeDbModelType.HiLifeF02:
                    HiLifeF02 objHiLifeF02 = new HiLifeF02();
                    objHiLifeF02.head = this.GetHead<TWNewEgg.DB.Models.Logistics.HiLife.F02Head>(argNumHeadId);
                    objHiLifeF02.body = this.GetBodyByHeadId<TWNewEgg.DB.Models.Logistics.HiLife.F02Body>(argNumHeadId);
                    objHiLifeF02.tail = this.GetTailByHeadId<TWNewEgg.DB.Models.Logistics.HiLife.F02Tail>(argNumHeadId);
                    objHiLife = objHiLifeF02;
                    break;
                case HiLifeDbModelType.HiLifeF03:
                    HiLifeF03 objHiLifeF03 = new HiLifeF03();
                    objHiLifeF03.head = this.GetHead<TWNewEgg.DB.Models.Logistics.HiLife.F03Head>(argNumHeadId);
                    objHiLifeF03.body = this.GetBodyByHeadId<TWNewEgg.DB.Models.Logistics.HiLife.F03Body>(argNumHeadId);
                    objHiLifeF03.tail = this.GetTailByHeadId<TWNewEgg.DB.Models.Logistics.HiLife.F03Tail>(argNumHeadId);
                    objHiLife = objHiLifeF03;
                    break;
                case HiLifeDbModelType.HiLifeF04:
                    HiLifeF04 objHiLifeF04 = new HiLifeF04();
                    objHiLifeF04.head = this.GetHead<TWNewEgg.DB.Models.Logistics.HiLife.F04Head>(argNumHeadId);
                    objHiLifeF04.body = this.GetBodyByHeadId<TWNewEgg.DB.Models.Logistics.HiLife.F04Body>(argNumHeadId);
                    objHiLifeF04.tail = this.GetTailByHeadId<TWNewEgg.DB.Models.Logistics.HiLife.F04Tail>(argNumHeadId);
                    objHiLife = objHiLifeF04;
                    break;
                case HiLifeDbModelType.HiLifeF05:
                    HiLifeF05 objHiLifeF05 = new HiLifeF05();
                    objHiLifeF05.head = this.GetHead<TWNewEgg.DB.Models.Logistics.HiLife.F05Head>(argNumHeadId);
                    objHiLifeF05.body = this.GetBodyByHeadId<TWNewEgg.DB.Models.Logistics.HiLife.F05Body>(argNumHeadId);
                    objHiLifeF05.tail = this.GetTailByHeadId<TWNewEgg.DB.Models.Logistics.HiLife.F05Tail>(argNumHeadId);
                    objHiLife = objHiLifeF05;
                    break;
                case HiLifeDbModelType.HiLifeF06:
                    HiLifeF06 objHiLifeF06 = new HiLifeF06();
                    objHiLifeF06.head = this.GetHead<TWNewEgg.DB.Models.Logistics.HiLife.F06Head>(argNumHeadId);
                    objHiLifeF06.body = this.GetBodyByHeadId<TWNewEgg.DB.Models.Logistics.HiLife.F06Body>(argNumHeadId);
                    objHiLifeF06.tail = this.GetTailByHeadId<TWNewEgg.DB.Models.Logistics.HiLife.F06Tail>(argNumHeadId);
                    objHiLife = objHiLifeF06;
                    break;
                case HiLifeDbModelType.HiLifeF08:
                    var objHiLifeF08 = new HiLifeF08();
                    objHiLifeF08.head = this.GetHead<TWNewEgg.DB.Models.Logistics.HiLife.F08Head>(argNumHeadId);
                    objHiLifeF08.body = this.GetBodyByHeadId<TWNewEgg.DB.Models.Logistics.HiLife.F08Body>(argNumHeadId);
                    objHiLifeF08.tail = this.GetTailByHeadId<TWNewEgg.DB.Models.Logistics.HiLife.F08Tail>(argNumHeadId);
                    objHiLife = objHiLifeF08;
                    break;
                case HiLifeDbModelType.HiLifeF09:
                    var objHiLifeF09 = new HiLifeF09();
                    objHiLifeF09.head = this.GetHead<TWNewEgg.DB.Models.Logistics.HiLife.F09Head>(argNumHeadId);
                    objHiLifeF09.body = this.GetBodyByHeadId<TWNewEgg.DB.Models.Logistics.HiLife.F09Body>(argNumHeadId);
                    objHiLifeF09.tail = this.GetTailByHeadId<TWNewEgg.DB.Models.Logistics.HiLife.F09Tail>(argNumHeadId);
                    objHiLife = objHiLifeF09;
                    break;
                case HiLifeDbModelType.HiLifeTxt:
                    // HiLifeTxt只有Body
                    HiLifeTxt objHiLifeTxt = new HiLifeTxt();
                    List<TxtBody> listTxtBody = null;
                    TxtBody objTxtBody = this.GetBody<TxtBody>(argNumHeadId);
                    if (objTxtBody != null)
                    {
                        listTxtBody = new List<TxtBody>();
                        listTxtBody.Add(objTxtBody);
                        objHiLifeTxt.body = listTxtBody;
                        objHiLife = objHiLifeTxt;
                    }
                    else
                    {
                        objHiLife = null;
                    }

                    break;
            }

            return (T)objHiLife;
        }

        /// <summary>
        /// 根據Head的建立日期取得整份文件
        /// </summary>
        /// <typeparam name="T">HiLifeF00, HiLifeF01, HiLifeF02....HiLifeF09, HiLifeTxt</typeparam>
        /// <param name="argDateTime">建立日期, 僅比對Year, Month, Day的資料</param>
        /// <returns>日期內符合的資料</returns>
        public List<T> GetHiLifeDocument<T>(DateTime argDateTime)
        {
            TWNewEgg.DB.TWSqlDBContext objDb = null;
            objDb = new TWSqlDBContext();
            object objHead = null;
            List<T> listDocument = new List<T>();
            HiLifeDbModelType objHiLifeModelType = HiLifeDbModelType.Unknown;
            //判定T的Model名稱,轉為列舉
            objHiLifeModelType = (HiLifeDbModelType)Enum.Parse(typeof(HiLifeDbModelType), typeof(T).Name);
           //利用文件ID來查詢對應的table
            switch (objHiLifeModelType)
            {
                case HiLifeDbModelType.HiLifeF00:
                    //F00只有Body

                    /* Iris寫的:
                    var objF00= new HiLifeF00(); 
                    objF00.body = objBody as List<F00Body>;
                    List<F00Body> listBody00 = null;
                    listBody00 = this.GetBodyByCreareDate<TWNewEgg.DB.Models.Logistics.HiLife.F00Body>(argDateTime);
                    objF00.body = listBody00;
                    document.Add((T)(object)objF00);
                     */
                    #region F00
                    List<HiLifeF00> listF00 = null;
                    List<F00Body> listBody = null;
                    HiLifeF00 objF00 = null;

                    listBody = this.GetBodyByCreareDate<F00Body>(argDateTime);
                    if (listBody != null)
                    {
                        listF00 = new List<HiLifeF00>();
                        foreach (F00Body objSubBody in listBody)
                        {
                            objF00 = new HiLifeF00();
                            objF00.body = new List<F00Body>();
                            objF00.body.Add(objSubBody);

                            listF00.Add(objF00);
                        }

                        //進行轉型
                        listDocument = listF00.Cast<T>().ToList();
                    }
                    else
                    {
                        listDocument = null;
                    }
                    #endregion
                    break;
                case HiLifeDbModelType.HiLifeF01:
                    #region F01
                    HiLifeF01 objF01 = new HiLifeF01();
                    List<F01Head> listHead01 = null;
                    objF01.head = objHead as F01Head;
                    listHead01 = this.GetHead<TWNewEgg.DB.Models.Logistics.HiLife.F01Head>(argDateTime);
                    if (listHead01 != null && listHead01.Count > 0)
                    {
                        foreach (F01Head objSubHead in listHead01)
                        {
                            objF01 = this.GetHiLifeDocument<TWNewEgg.DB.Models.Logistics.HiLife.HiLifeF01>(objSubHead.Id);
                            listDocument.Add((T)(object)objF01);
                            //objF01 = new HiLifeF01();
                        }
                    }
                    #endregion
                    break;
                case HiLifeDbModelType.HiLifeF02:
                    #region F02
                    HiLifeF02 objF02 = new HiLifeF02();
                    List<F02Head> listHead02 = null;
                    objF02.head = objHead as F02Head;
                    listHead02 = this.GetHead<TWNewEgg.DB.Models.Logistics.HiLife.F02Head>(argDateTime);
                    if (listHead02 != null && listHead02.Count > 0)
                    {
                        foreach (F02Head objSubHead in listHead02)
                        {
                            objF02 = this.GetHiLifeDocument<TWNewEgg.DB.Models.Logistics.HiLife.HiLifeF02>(objSubHead.Id);
                            listDocument.Add((T)(object)objF02);
                        }
                    }
                    #endregion
                    break;
                case HiLifeDbModelType.HiLifeF03:
                    #region F03
                    HiLifeF03 objF03 = new HiLifeF03();
                    List<F03Head> listHead03 = null;
                    objF03.head = objHead as F03Head;
                    listHead03 = this.GetHead<TWNewEgg.DB.Models.Logistics.HiLife.F03Head>(argDateTime);
                    if (listHead03 != null && listHead03.Count > 0)
                    {
                        foreach (F03Head objSubHead in listHead03)
                        {
                            objF03 = this.GetHiLifeDocument<TWNewEgg.DB.Models.Logistics.HiLife.HiLifeF03>(objSubHead.Id);
                            listDocument.Add((T)(object)objF03);
                        }
                    }
                    #endregion
                    break;
                case HiLifeDbModelType.HiLifeF04:
                    #region F04
                    HiLifeF04 objF04 = new HiLifeF04();
                    List<F04Head> listHead04 = null;
                    objF04.head = objHead as F04Head;
                    listHead04 = this.GetHead<TWNewEgg.DB.Models.Logistics.HiLife.F04Head>(argDateTime);
                    if (listHead04 != null && listHead04.Count > 0)
                    {
                        foreach (F04Head objSubHead in listHead04)
                        {
                            objF04 = this.GetHiLifeDocument<TWNewEgg.DB.Models.Logistics.HiLife.HiLifeF04>(objSubHead.Id);
                            listDocument.Add((T)(object)objF04);
                        }
                    }
                    #endregion
                    break;
                case HiLifeDbModelType.HiLifeF05:
                    #region F05
                    HiLifeF05 objF05 = new HiLifeF05();
                    List<F05Head> listHead05 = null;
                    objF05.head = objHead as F05Head;
                    listHead05 = this.GetHead<TWNewEgg.DB.Models.Logistics.HiLife.F05Head>(argDateTime);
                    if (listHead05 != null && listHead05.Count > 0)
                    {
                        foreach (F05Head objSubHead in listHead05)
                        {
                            objF05 = this.GetHiLifeDocument<TWNewEgg.DB.Models.Logistics.HiLife.HiLifeF05>(objSubHead.Id);
                            listDocument.Add((T)(object)objF05);
                        }
                    }
                    #endregion
                    break;
                case HiLifeDbModelType.HiLifeF06:
                    #region F06
                    HiLifeF06 objF06 = new HiLifeF06();
                    List<F06Head> listHead06 = null;
                    objF06.head = objHead as F06Head;
                    listHead06 = this.GetHead<TWNewEgg.DB.Models.Logistics.HiLife.F06Head>(argDateTime);
                    if (listHead06 != null && listHead06.Count > 0)
                    {
                        foreach (F06Head objSubHead in listHead06)
                        {
                            objF06 = this.GetHiLifeDocument<TWNewEgg.DB.Models.Logistics.HiLife.HiLifeF06>(objSubHead.Id);
                            listDocument.Add((T)(object)objF06);
                        }
                    }
                    #endregion
                    break;
                case HiLifeDbModelType.HiLifeF08:
                    #region F08
                    HiLifeF08 objF08 = new HiLifeF08();
                    List<F08Head> listHead08 = null;
                    objF08.head = objHead as F08Head;
                    listHead08 = this.GetHead<TWNewEgg.DB.Models.Logistics.HiLife.F08Head>(argDateTime);
                    if (listHead08 != null && listHead08.Count > 0)
                    {
                        foreach (F08Head objSubHead in listHead08)
                        {
                            objF08 = this.GetHiLifeDocument<TWNewEgg.DB.Models.Logistics.HiLife.HiLifeF08>(objSubHead.Id);
                            listDocument.Add((T)(object)objF08);
                        }
                    }
                    #endregion
                    break;
                case HiLifeDbModelType.HiLifeF09:
                    #region F09
                    HiLifeF09 objF09 = new HiLifeF09();
                    List<F09Head> listHead09 = null;
                    objF09.head = objHead as F09Head;
                    listHead09 = this.GetHead<TWNewEgg.DB.Models.Logistics.HiLife.F09Head>(argDateTime);
                    if (listHead09 != null && listHead09.Count > 0)
                    {
                        foreach (F09Head objSubHead in listHead09)
                        {
                            objF09 = this.GetHiLifeDocument<TWNewEgg.DB.Models.Logistics.HiLife.HiLifeF09>(objSubHead.Id);
                            listDocument.Add((T)(object)objF09);
                            objF09 = new HiLifeF09();
                        }
                    }
                    #endregion
                    break;
                case HiLifeDbModelType.HiLifeTxt:
                    #region Txt
                    List<HiLifeTxt> listTxt = null;
                    List<TxtBody> listTxtBody = null;
                    HiLifeTxt objTxt = null;

                    listTxtBody = this.GetBodyByCreareDate<TxtBody>(argDateTime);
                    if (listTxtBody != null)
                    {
                        listTxt = new List<HiLifeTxt>();
                        foreach (TxtBody objSubBody in listTxtBody)
                        {
                            objTxt = new HiLifeTxt();
                            objTxt.body = new List<TxtBody>();
                            objTxt.body.Add(objSubBody);

                            listTxt.Add(objTxt);
                        }

                        //進行轉型
                        listDocument = listTxt.Cast<T>().ToList();
                    }
                    else
                    {
                        listDocument = null;
                    }
                    #endregion
                    break;
            }

            objDb.Dispose();
            objDb = null;

            return listDocument;
        }

        /// <summary>
        /// 新增一筆Head物件
        /// </summary>
        /// <typeparam name="T">F01Head, F02Head ... F09Head</typeparam>
        /// <param name="argObjHead">欲增加的Head物件</param>
        /// <returns>新增成功回傳Head Id, 新增失敗回傳0或-1</returns>
        public int CreateHead<T>(T argObjHead)
        {
            TWNewEgg.DB.TWSqlDBContext objDb = null;
            int numHeadId = 0;
            object objHead = null;

            HiLifeDbModelType objHiLifeModelType = HiLifeDbModelType.Unknown;

            //判定T的Model名稱,轉為列舉
            objHiLifeModelType = (HiLifeDbModelType)Enum.Parse(typeof(HiLifeDbModelType), typeof(T).Name);

            objDb = new TWSqlDBContext();

            //利用列舉來查詢對應的table
            switch (objHiLifeModelType)
            {
                case HiLifeDbModelType.F01Head:
                    objHead = argObjHead as F01Head;
                    objDb.HiLifeF01Head.Add((F01Head)objHead);
                    try
                    {
                        objDb.SaveChanges();
                        numHeadId = (int)((F01Head)objHead).Id;
                    }
                    catch (Exception ex)
                    {
                        numHeadId = 0;
                        string strMessage = "";
                        strMessage += "時間: " + DateTime.Now.ToString() + "\r\n";
                        strMessage += "Function: HiLifeAnalysisRepository.CreateHead" + "\r\n";
                        strMessage += "FileType: " + objHiLifeModelType.ToString() + "\r\n";
                        strMessage += "錯誤訊息: " + ex.Message + "\r\n\r\n";
                        this.LogMessage(strMessage);
                    }

                    break;
                case HiLifeDbModelType.F02Head:
                    objHead = argObjHead as F02Head;
                    objDb.HiLifeF02Head.Add((F02Head)objHead);
                    try
                    {
                        objDb.SaveChanges();
                        numHeadId = (int)((F02Head)objHead).Id;
                    }
                    catch (Exception ex)
                    {
                        numHeadId = 0;
                        string strMessage = "";
                        strMessage += "時間: " + DateTime.Now.ToString() + "\r\n";
                        strMessage += "Function: HiLifeAnalysisRepository.CreateHead" + "\r\n";
                        strMessage += "FileType: " + objHiLifeModelType.ToString() + "\r\n";
                        strMessage += "錯誤訊息: " + ex.Message + "\r\n\r\n";
                        this.LogMessage(strMessage);
                    }

                    break;
                case HiLifeDbModelType.F03Head:
                    objHead = argObjHead as F03Head;
                    objDb.HiLifeF03Head.Add((F03Head)objHead);
                    try
                    {
                        objDb.SaveChanges();
                        numHeadId = (int)((F03Head)objHead).Id;
                    }
                    catch (Exception ex)
                    {
                        numHeadId = 0;
                        string strMessage = "";
                        strMessage += "時間: " + DateTime.Now.ToString() + "\r\n";
                        strMessage += "Function: HiLifeAnalysisRepository.CreateHead" + "\r\n";
                        strMessage += "FileType: " + objHiLifeModelType.ToString() + "\r\n";
                        strMessage += "錯誤訊息: " + ex.Message + "\r\n\r\n";
                        this.LogMessage(strMessage);
                    }

                    break;
                case HiLifeDbModelType.F04Head:
                    objHead = argObjHead as F04Head;
                    objDb.HiLifeF04Head.Add((F04Head)objHead);
                    try
                    {
                        objDb.SaveChanges();
                        numHeadId = (int)((F04Head)objHead).Id;
                    }
                    catch (Exception ex)
                    {
                        numHeadId = 0;
                        string strMessage = "";
                        strMessage += "時間: " + DateTime.Now.ToString() + "\r\n";
                        strMessage += "Function: HiLifeAnalysisRepository.CreateHead" + "\r\n";
                        strMessage += "FileType: " + objHiLifeModelType.ToString() + "\r\n";
                        strMessage += "錯誤訊息: " + ex.Message + "\r\n\r\n";
                        this.LogMessage(strMessage);
                    }

                    break;
                case HiLifeDbModelType.F05Head:
                    objHead = argObjHead as F05Head;
                    objDb.HiLifeF05Head.Add((F05Head)objHead);
                    try
                    {
                        objDb.SaveChanges();
                        numHeadId = (int)((F05Head)objHead).Id;
                    }
                    catch (Exception ex)
                    {
                        numHeadId = 0;
                        string strMessage = "";
                        strMessage += "時間: " + DateTime.Now.ToString() + "\r\n";
                        strMessage += "Function: HiLifeAnalysisRepository.CreateHead" + "\r\n";
                        strMessage += "FileType: " + objHiLifeModelType.ToString() + "\r\n";
                        strMessage += "錯誤訊息: " + ex.Message + "\r\n\r\n";
                        this.LogMessage(strMessage);
                    }

                    break;
                case HiLifeDbModelType.F06Head:
                    objHead = argObjHead as F06Head;
                    objDb.HiLifeF06Head.Add((F06Head)objHead);
                    try
                    {
                        objDb.SaveChanges();
                        numHeadId = (int)((F06Head)objHead).Id;
                    }
                    catch (Exception ex)
                    {
                        numHeadId = 0;
                        string strMessage = "";
                        strMessage += "時間: " + DateTime.Now.ToString() + "\r\n";
                        strMessage += "Function: HiLifeAnalysisRepository.CreateHead" + "\r\n";
                        strMessage += "FileType: " + objHiLifeModelType.ToString() + "\r\n";
                        strMessage += "錯誤訊息: " + ex.Message + "\r\n\r\n";
                        this.LogMessage(strMessage);
                    }

                    break;
                case HiLifeDbModelType.F08Head:
                    objHead = argObjHead as F08Head;
                    objDb.HiLifeF08Head.Add((F08Head)objHead);
                    try
                    {
                        objDb.SaveChanges();
                        numHeadId = (int)((F08Head)objHead).Id;
                    }
                    catch (Exception ex)
                    {
                        numHeadId = 0;
                        string strMessage = "";
                        strMessage += "時間: " + DateTime.Now.ToString() + "\r\n";
                        strMessage += "Function: HiLifeAnalysisRepository.CreateHead" + "\r\n";
                        strMessage += "FileType: " + objHiLifeModelType.ToString() + "\r\n";
                        strMessage += "錯誤訊息: " + ex.Message + "\r\n\r\n";
                        this.LogMessage(strMessage);
                    }

                    break;
                case HiLifeDbModelType.F09Head:
                    objHead = argObjHead as F09Head;
                    objDb.HiLifeF09Head.Add((F09Head)objHead);
                    try
                    {
                        objDb.SaveChanges();
                        numHeadId = (int)((F09Head)objHead).Id;
                    }
                    catch (Exception ex)
                    {
                        numHeadId = 0;
                        string strMessage = "";
                        strMessage += "時間: " + DateTime.Now.ToString() + "\r\n";
                        strMessage += "Function: HiLifeAnalysisRepository.CreateHead" + "\r\n";
                        strMessage += "FileType: " + objHiLifeModelType.ToString() + "\r\n";
                        strMessage += "錯誤訊息: " + ex.Message + "\r\n\r\n";
                        this.LogMessage(strMessage);
                    }

                    break;
            }

            return numHeadId;
        }

        /// <summary>
        /// 修改一筆Head物件
        /// </summary>
        /// <typeparam name="T">F01Head, F02Head ... F09Head</typeparam>
        /// <param name="argObjHead">欲修改的Head物件</param>
        /// <returns>修改成功回傳true, 修改失敗回傳false</returns>
        public bool UpdateHead<T>(T argObjHead)
        {
            bool boolExec = false;
            TWNewEgg.DB.TWSqlDBContext objDb = null;

            object objHead = null;

            HiLifeDbModelType objHiLifeModelType = HiLifeDbModelType.Unknown;

            //判定T的Model名稱,轉為列舉
            objHiLifeModelType = (HiLifeDbModelType)Enum.Parse(typeof(HiLifeDbModelType), typeof(T).Name);

            objDb = new TWSqlDBContext();

            //利用列舉來查詢對應的table
            switch (objHiLifeModelType)
            {
                case HiLifeDbModelType.F01Head:
                    objHead = argObjHead as F01Head;
                    objDb.Entry<F01Head>((F01Head)objHead).State = System.Data.EntityState.Modified;
                    break;
                case HiLifeDbModelType.F02Head:
                    objHead = argObjHead as F02Head;
                    objDb.Entry<F02Head>((F02Head)objHead).State = System.Data.EntityState.Modified;
                    break;
                case HiLifeDbModelType.F03Head:
                    objHead = argObjHead as F03Head;
                    objDb.Entry<F03Head>((F03Head)objHead).State = System.Data.EntityState.Modified;
                    break;
                case HiLifeDbModelType.F04Head:
                    objHead = argObjHead as F04Head;
                    objDb.Entry<F04Head>((F04Head)objHead).State = System.Data.EntityState.Modified;
                    break;
                case HiLifeDbModelType.F05Head:
                    objHead = argObjHead as F05Head;
                    objDb.Entry<F05Head>((F05Head)objHead).State = System.Data.EntityState.Modified;
                    break;
                case HiLifeDbModelType.F06Head:
                    objHead = argObjHead as F06Head;
                    objDb.Entry<F06Head>((F06Head)objHead).State = System.Data.EntityState.Modified;
                    break;
                case HiLifeDbModelType.F08Head:
                    objHead = argObjHead as F08Head;
                    objDb.Entry<F08Head>((F08Head)objHead).State = System.Data.EntityState.Modified;
                    break;
                case HiLifeDbModelType.F09Head:
                    objHead = argObjHead as F09Head;
                    objDb.Entry<F09Head>((F09Head)objHead).State = System.Data.EntityState.Modified;
                    break;
            }

            try
            {
                objDb.SaveChanges();
                boolExec = true;
            }
            catch (Exception ex)
            {
                string strMessage = "";
                strMessage += "時間: " + DateTime.Now.ToString() + "\r\n";
                strMessage += "Function: HiLifeAnalysisRepository.UpdateHead" + "\r\n";
                strMessage += "FileType: " + objHiLifeModelType.ToString() + "\r\n";
                strMessage += "錯誤訊息: " + ex.Message + "\r\n\r\n";
                this.LogMessage(strMessage);
            }

            return boolExec;
        }

        /// <summary>
        /// 根據Head Id取得Head
        /// </summary>
        /// <typeparam name="T">F01Head, F02Head ... F09Head</typeparam>
        /// <param name="argNumHeadId">Head Id</param>
        /// <returns>有資料傳回對應T型態的Head物件, 無資料回傳null</returns>
        public T GetHead<T>(int argNumHeadId)
        {
            if (argNumHeadId <= 0)
            {
                return default(T);
            }

            TWNewEgg.DB.TWSqlDBContext objDb = null;
            object objHead = null;
            HiLifeDbModelType objHiLifeModelType = HiLifeDbModelType.Unknown;

            //判定T的Model名稱,轉為列舉
            objHiLifeModelType = (HiLifeDbModelType)Enum.Parse(typeof(HiLifeDbModelType), typeof(T).Name);

            objDb = new TWSqlDBContext();

            //利用列舉來查詢對應的table
            switch (objHiLifeModelType)
            {
                case HiLifeDbModelType.F01Head:
                    objHead = objDb.HiLifeF01Head.Where(x => x.Id == argNumHeadId).FirstOrDefault();
                    break;
                case HiLifeDbModelType.F02Head:
                    objHead = objDb.HiLifeF02Head.Where(x => x.Id == argNumHeadId).FirstOrDefault();
                    break;
                case HiLifeDbModelType.F03Head:
                    objHead = objDb.HiLifeF03Head.Where(x => x.Id == argNumHeadId).FirstOrDefault();
                    break;
                case HiLifeDbModelType.F04Head:
                    objHead = objDb.HiLifeF04Head.Where(x => x.Id == argNumHeadId).FirstOrDefault();
                    break;
                case HiLifeDbModelType.F05Head:
                    objHead = objDb.HiLifeF05Head.Where(x => x.Id == argNumHeadId).FirstOrDefault();
                    break;
                case HiLifeDbModelType.F06Head:
                    objHead = objDb.HiLifeF06Head.Where(x => x.Id == argNumHeadId).FirstOrDefault();
                    break;
                case HiLifeDbModelType.F08Head:
                    objHead = objDb.HiLifeF08Head.Where(x => x.Id == argNumHeadId).FirstOrDefault();
                    break;
                case HiLifeDbModelType.F09Head:
                    objHead = objDb.HiLifeF09Head.Where(x => x.Id == argNumHeadId).FirstOrDefault();
                    break;
            }

            objDb.Dispose();
            objDb = null;

            return (T)objHead;
        }

        /// <summary>
        /// 根據日期取得Head列表
        /// </summary>
        /// <typeparam name="T">F01Head, F02Head ... F09Head</typeparam>
        /// <param name="argDateTime">查詢日期, 僅比對Year, Month, Day</param>
        /// <returns>有資料傳回對應T型態的List物件, 無資料回傳null</returns>
        public List<T> GetHead<T>(DateTime argDateTime)
        {
            if (argDateTime == null)
            {
                return null;
            }
            
            List<object> listHead = null;
            //List<T> listTHead = null;
            List<T> listTHead = null;
            TWSqlDBContext objDb = null;
            string strDate = "";
            HiLifeDbModelType objHiLifeModelType = HiLifeDbModelType.Unknown;

            //欲查詢的日期格式轉成yyyymmdd的字串
            strDate = argDateTime.Year.ToString() + argDateTime.Month.ToString() + argDateTime.Day.ToString();

            //判定T的Model名稱,轉為列舉
            objHiLifeModelType = (HiLifeDbModelType)Enum.Parse(typeof(HiLifeDbModelType), typeof(T).Name);
            objDb = new TWSqlDBContext();

            //利用列舉來查詢對應的table
            switch (objHiLifeModelType)
            {
                case HiLifeDbModelType.F01Head:
                    listHead = objDb.HiLifeF01Head.Where(x => x.Prdt == strDate).ToList<object>();
                    break;
                case HiLifeDbModelType.F02Head:
                    listHead = objDb.HiLifeF02Head.Where(x => x.Prdt == strDate).ToList<object>();
                    break;
                case HiLifeDbModelType.F03Head:
                    listHead = objDb.HiLifeF03Head.Where(x => x.Prdt == strDate).ToList<object>();
                    break;
                case HiLifeDbModelType.F04Head:
                    listHead = objDb.HiLifeF04Head.Where(x => x.Prdt == strDate).ToList<object>();
                    break;
                case HiLifeDbModelType.F05Head:
                    listHead = objDb.HiLifeF05Head.Where(x => x.Prdt == strDate).ToList<object>();
                    break;
                case HiLifeDbModelType.F06Head:
                    listHead = objDb.HiLifeF06Head.Where(x => x.Prdt == strDate).ToList<object>();
                    break;
                case HiLifeDbModelType.F08Head:
                    listHead = objDb.HiLifeF08Head.Where(x => x.Prdt == strDate).ToList<object>();
                    break;
                case HiLifeDbModelType.F09Head:
                    listHead = objDb.HiLifeF09Head.Where(x => x.Prdt == strDate).ToList<object>();
                    break;
            }

            objDb.Dispose();
            objDb = null;

            //進行泛型轉型
            if (listHead != null && listHead.Count > 0)
            {
                listTHead = listHead.Cast<T>().ToList();
            }

            listHead = null;

            return listTHead;

            /*
             *  public IEnumerable<T> GetTable<T>(string tableName)
                {
                TWSqlDBContext dbo = new TWSqlDBContext();

                    DateTime dateTime = DateTime.Today.AddMonths(-2);
                    if (tableName == "Product")
                {
                    var dbProductData = dbo.Product.Where(x => x.CreateDate > dateTime).ToList();
                    return (IEnumerable<T>)dbProductData.ToList();
                }
                return null;
        }

             */
        }

        /// <summary>
        /// 新增一筆Body物件
        /// </summary>
        /// <typeparam name="T">F00Body, F01Body, ... F09Body, TxtBody</typeparam>
        /// <param name="argObjBody">欲新增的Body物件</param>
        /// <returns>新增成功回傳Body Id, 新增失敗回傳0或-1</returns>
        public int CreateBody<T>(T argObjBody)
        {
            TWNewEgg.DB.TWSqlDBContext objDb = null;
            int numBodyId = 0;
            object objBody = null;

            HiLifeDbModelType objHiLifeModelType = HiLifeDbModelType.Unknown;

            //判定T的Model名稱,轉為列舉
            objHiLifeModelType = (HiLifeDbModelType)Enum.Parse(typeof(HiLifeDbModelType), typeof(T).Name);

            objDb = new TWSqlDBContext();

            //利用列舉來查詢對應的table
            switch (objHiLifeModelType)
            {
                case HiLifeDbModelType.F00Body:
                    objBody = argObjBody as F00Body;
                    objDb.HiLifeF00Body.Add((F00Body)objBody);
                    try
                    {
                        objDb.SaveChanges();
                        numBodyId = (int)((F00Body)objBody).Id;
                    }
                    catch (Exception ex)
                    {
                        numBodyId = 0;
                        string strMessage = "";
                        strMessage += "時間: " + DateTime.Now.ToString() + "\r\n";
                        strMessage += "Function: HiLifeAnalysisRepository.CreateBody" + "\r\n";
                        strMessage += "FileType: " + objHiLifeModelType.ToString() + "\r\n";
                        strMessage += "錯誤訊息: " + ex.Message + "\r\n\r\n";
                        this.LogMessage(strMessage);
                    }

                    break;
                case HiLifeDbModelType.F01Body:
                    objBody = argObjBody as F01Body;
                    objDb.HiLifeF01Body.Add((F01Body)objBody);
                    try
                    {
                        objDb.SaveChanges();
                        numBodyId = (int)((F01Body)objBody).Id;
                    }
                    catch (Exception ex)
                    {
                        numBodyId = 0;
                        string strMessage = "";
                        strMessage += "時間: " + DateTime.Now.ToString() + "\r\n";
                        strMessage += "Function: HiLifeAnalysisRepository.CreateBody" + "\r\n";
                        strMessage += "FileType: " + objHiLifeModelType.ToString() + "\r\n";
                        strMessage += "錯誤訊息: " + ex.Message + "\r\n\r\n";
                        this.LogMessage(strMessage);
                    }

                    break;
                case HiLifeDbModelType.F02Body:
                    objBody = argObjBody as F02Body;
                    objDb.HiLifeF02Body.Add((F02Body)objBody);
                    try
                    {
                        objDb.SaveChanges();
                        numBodyId = (int)((F02Body)objBody).Id;
                    }
                    catch (Exception ex)
                    {
                        numBodyId = 0;
                        string strMessage = "";
                        strMessage += "時間: " + DateTime.Now.ToString() + "\r\n";
                        strMessage += "Function: HiLifeAnalysisRepository.CreateBody" + "\r\n";
                        strMessage += "FileType: " + objHiLifeModelType.ToString() + "\r\n";
                        strMessage += "錯誤訊息: " + ex.Message + "\r\n\r\n";
                        this.LogMessage(strMessage);
                    }

                    break;
                case HiLifeDbModelType.F03Body:
                    objBody = argObjBody as F03Body;
                    objDb.HiLifeF03Body.Add((F03Body)objBody);
                    try
                    {
                        objDb.SaveChanges();
                        numBodyId = (int)((F03Body)objBody).Id;
                    }
                    catch (Exception ex)
                    {
                        numBodyId = 0;
                        string strMessage = "";
                        strMessage += "時間: " + DateTime.Now.ToString() + "\r\n";
                        strMessage += "Function: HiLifeAnalysisRepository.CreateBody" + "\r\n";
                        strMessage += "FileType: " + objHiLifeModelType.ToString() + "\r\n";
                        strMessage += "錯誤訊息: " + ex.Message + "\r\n\r\n";
                        this.LogMessage(strMessage);
                    }

                    break;
                case HiLifeDbModelType.F04Body:
                    objBody = argObjBody as F04Body;
                    objDb.HiLifeF04Body.Add((F04Body)objBody);
                    try
                    {
                        objDb.SaveChanges();
                        numBodyId = (int)((F04Body)objBody).Id;
                    }
                    catch (Exception ex)
                    {
                        numBodyId = 0;
                        string strMessage = "";
                        strMessage += "時間: " + DateTime.Now.ToString() + "\r\n";
                        strMessage += "Function: HiLifeAnalysisRepository.CreateBody" + "\r\n";
                        strMessage += "FileType: " + objHiLifeModelType.ToString() + "\r\n";
                        strMessage += "錯誤訊息: " + ex.Message + "\r\n\r\n";
                        this.LogMessage(strMessage);
                    }

                    break;
                case HiLifeDbModelType.F05Body:
                    objBody = argObjBody as F05Body;
                    objDb.HiLifeF05Body.Add((F05Body)objBody);
                    try
                    {
                        objDb.SaveChanges();
                        numBodyId = (int)((F05Body)objBody).Id;
                    }
                    catch (Exception ex)
                    {
                        numBodyId = 0;
                        string strMessage = "";
                        strMessage += "時間: " + DateTime.Now.ToString() + "\r\n";
                        strMessage += "Function: HiLifeAnalysisRepository.CreateBody" + "\r\n";
                        strMessage += "FileType: " + objHiLifeModelType.ToString() + "\r\n";
                        strMessage += "錯誤訊息: " + ex.Message + "\r\n\r\n";
                        this.LogMessage(strMessage);
                    }

                    break;
                case HiLifeDbModelType.F06Body:
                    objBody = argObjBody as F06Body;
                    objDb.HiLifeF06Body.Add((F06Body)objBody);
                    try
                    {
                        objDb.SaveChanges();
                        numBodyId = (int)((F06Body)objBody).Id;
                    }
                    catch (Exception ex)
                    {
                        numBodyId = 0;
                        string strMessage = "";
                        strMessage += "時間: " + DateTime.Now.ToString() + "\r\n";
                        strMessage += "Function: HiLifeAnalysisRepository.CreateBody" + "\r\n";
                        strMessage += "FileType: " + objHiLifeModelType.ToString() + "\r\n";
                        strMessage += "錯誤訊息: " + ex.Message + "\r\n\r\n";
                        this.LogMessage(strMessage);
                    }

                    break;
                case HiLifeDbModelType.F08Body:
                    objBody = argObjBody as F08Body;
                    objDb.HiLifeF08Body.Add((F08Body)objBody);
                    try
                    {
                        objDb.SaveChanges();
                        numBodyId = (int)((F08Body)objBody).Id;
                    }
                    catch (Exception ex)
                    {
                        numBodyId = 0;
                        string strMessage = "";
                        strMessage += "時間: " + DateTime.Now.ToString() + "\r\n";
                        strMessage += "Function: HiLifeAnalysisRepository.CreateBody" + "\r\n";
                        strMessage += "FileType: " + objHiLifeModelType.ToString() + "\r\n";
                        strMessage += "錯誤訊息: " + ex.Message + "\r\n\r\n";
                        this.LogMessage(strMessage);
                    }

                    break;
                case HiLifeDbModelType.F09Body:
                    objBody = argObjBody as F09Body;
                    objDb.HiLifeF09Body.Add((F09Body)objBody);
                    try
                    {
                        objDb.SaveChanges();
                        numBodyId = (int)((F09Body)objBody).Id;
                    }
                    catch (Exception ex)
                    {
                        numBodyId = 0;
                        string strMessage = "";
                        strMessage += "時間: " + DateTime.Now.ToString() + "\r\n";
                        strMessage += "Function: HiLifeAnalysisRepository.CreateBody" + "\r\n";
                        strMessage += "FileType: " + objHiLifeModelType.ToString() + "\r\n";
                        strMessage += "錯誤訊息: " + ex.Message + "\r\n\r\n";
                        this.LogMessage(strMessage);
                    }

                    break;
                case HiLifeDbModelType.TxtBody:
                    objBody = argObjBody as TxtBody;
                    objDb.HiLifeTxtBody.Add((TxtBody)objBody);
                    try
                    {
                        objDb.SaveChanges();
                        numBodyId = (int)((TxtBody)objBody).Id;
                    }
                    catch (Exception ex)
                    {
                        numBodyId = 0;
                        string strMessage = "";
                        strMessage += "時間: " + DateTime.Now.ToString() + "\r\n";
                        strMessage += "Function: HiLifeAnalysisRepository.CreateBody" + "\r\n";
                        strMessage += "FileType: " + objHiLifeModelType.ToString() + "\r\n";
                        strMessage += "錯誤訊息: " + ex.Message + "\r\n\r\n";
                        this.LogMessage(strMessage);
                    }

                    break;
            }

            return numBodyId;
        }

        /// <summary>
        /// 修改一筆Body物件
        /// </summary>
        /// <typeparam name="T">F00Body, F01Body, ... F09Body, TxtBody</typeparam>
        /// <param name="argObjBody">欲修改的Body物件</param>
        /// <returns>修改成功回傳true, 修改失敗回傳false</returns>
        public bool UpdateBody<T>(T argObjBody)
        {
            bool boolExec = false;
            TWNewEgg.DB.TWSqlDBContext objDb = null;

            object objBody = null;

            HiLifeDbModelType objHiLifeModelType = HiLifeDbModelType.Unknown;

            //判定T的Model名稱,轉為列舉
            objHiLifeModelType = (HiLifeDbModelType)Enum.Parse(typeof(HiLifeDbModelType), typeof(T).Name);

            objDb = new TWSqlDBContext();

            //利用列舉來查詢對應的table
            switch (objHiLifeModelType)
            {
                case HiLifeDbModelType.F00Body:
                    objBody = argObjBody as F00Body;
                    objDb.Entry<F00Body>((F00Body)objBody).State = System.Data.EntityState.Modified;
                    break;

                case HiLifeDbModelType.F01Body:
                    objBody = argObjBody as F01Body;
                    objDb.Entry<F01Body>((F01Body)objBody).State = System.Data.EntityState.Modified;
                    break;
                case HiLifeDbModelType.F02Body:
                    objBody = argObjBody as F02Body;
                    objDb.Entry<F02Body>((F02Body)objBody).State = System.Data.EntityState.Modified;
                    break;
                case HiLifeDbModelType.F03Body:
                    objBody = argObjBody as F03Body;
                    objDb.Entry<F03Body>((F03Body)objBody).State = System.Data.EntityState.Modified;
                    break;
                case HiLifeDbModelType.F04Body:
                    objBody = argObjBody as F04Body;
                    objDb.Entry<F04Body>((F04Body)objBody).State = System.Data.EntityState.Modified;
                    break;
                case HiLifeDbModelType.F05Body:
                    objBody = argObjBody as F05Body;
                    objDb.Entry<F05Body>((F05Body)objBody).State = System.Data.EntityState.Modified;
                    break;
                case HiLifeDbModelType.F06Body:
                    objBody = argObjBody as F06Body;
                    objDb.Entry<F06Body>((F06Body)objBody).State = System.Data.EntityState.Modified;
                    break;
                case HiLifeDbModelType.F08Body:
                    objBody = argObjBody as F08Body;
                    objDb.Entry<F08Body>((F08Body)objBody).State = System.Data.EntityState.Modified;
                    break;
                case HiLifeDbModelType.F09Body:
                    objBody = argObjBody as F09Body;
                    objDb.Entry<F09Body>((F09Body)objBody).State = System.Data.EntityState.Modified;
                    break;
                case HiLifeDbModelType.TxtBody:
                    objBody = argObjBody as TxtBody;
                    objDb.Entry<TxtBody>((TxtBody)objBody).State = System.Data.EntityState.Modified;
                    break;
            }

            try
            {
                objDb.SaveChanges();
                boolExec = true;
            }
            catch (Exception ex)
            {
                string strMessage = "";
                strMessage += "時間: " + DateTime.Now.ToString() + "\r\n";
                strMessage += "Function: HiLifeAnalysisRepository.UpdateBody" + "\r\n";
                strMessage += "FileType: " + objHiLifeModelType.ToString() + "\r\n";
                strMessage += "錯誤訊息: " + ex.Message + "\r\n\r\n";
                this.LogMessage(strMessage);
            }

            return boolExec;
        }

        /// <summary>
        /// 修改List內的所有Body
        /// </summary>
        /// <typeparam name="T">F00Body, F01Body, ... F09Body, TxtBody</typeparam>
        /// <param name="argListBody">要修改的Body列表</param>
        /// <returns>修改成功回傳true, 修改失敗回傳false</returns>
        public bool UpdateBody<T>(List<T> argListBody)
        {
            bool boolExec = false;
            TWNewEgg.DB.TWSqlDBContext objDb = null;
            int i = 0;
            object objBody = null;

            HiLifeDbModelType objHiLifeModelType = HiLifeDbModelType.Unknown;

            //判定T的Model名稱,轉為列舉
            objHiLifeModelType = (HiLifeDbModelType)Enum.Parse(typeof(HiLifeDbModelType), typeof(T).Name);

            objDb = new TWSqlDBContext();

            //利用列舉來查詢對應的table
            switch (objHiLifeModelType)
            {
                case HiLifeDbModelType.F00Body:
                    for (i = 0; i < argListBody.Count; i++)
                    {
                        objBody = argListBody[i] as F00Body;
                        objDb.Entry<F00Body>((F00Body)objBody).State = System.Data.EntityState.Modified;
                    }

                    break;

                case HiLifeDbModelType.F01Body:
                    for (i = 0; i < argListBody.Count; i++)
                    {
                        objBody = argListBody[i] as F01Body;
                        objDb.Entry<F01Body>((F01Body)objBody).State = System.Data.EntityState.Modified;
                    }

                    break;
                case HiLifeDbModelType.F02Body:
                    for (i = 0; i < argListBody.Count; i++)
                    {
                        objBody = argListBody[i] as F02Body;
                        objDb.Entry<F02Body>((F02Body)objBody).State = System.Data.EntityState.Modified;
                    }

                    break;
                case HiLifeDbModelType.F03Body:
                    for (i = 0; i < argListBody.Count; i++)
                    {
                        objBody = argListBody[i] as F03Body;
                        objDb.Entry<F03Body>((F03Body)objBody).State = System.Data.EntityState.Modified;
                    }

                    break;
                case HiLifeDbModelType.F04Body:
                    for (i = 0; i < argListBody.Count; i++)
                    {
                        objBody = argListBody[i] as F04Body;
                        objDb.Entry<F04Body>((F04Body)objBody).State = System.Data.EntityState.Modified;
                    }

                    break;
                case HiLifeDbModelType.F05Body:
                    for (i = 0; i < argListBody.Count; i++)
                    {
                        objBody = argListBody[i] as F05Body;
                        objDb.Entry<F05Body>((F05Body)objBody).State = System.Data.EntityState.Modified;
                    }

                    break;
                case HiLifeDbModelType.F06Body:
                    for (i = 0; i < argListBody.Count; i++)
                    {
                        objBody = argListBody[i] as F06Body;
                        objDb.Entry<F06Body>((F06Body)objBody).State = System.Data.EntityState.Modified;
                    }

                    break;
                case HiLifeDbModelType.F08Body:
                    for (i = 0; i < argListBody.Count; i++)
                    {
                        objBody = argListBody[i] as F08Body;
                        objDb.Entry<F08Body>((F08Body)objBody).State = System.Data.EntityState.Modified;
                    }

                    break;
                case HiLifeDbModelType.F09Body:
                    for (i = 0; i < argListBody.Count; i++)
                    {
                        objBody = argListBody[i] as F09Body;
                        objDb.Entry<F09Body>((F09Body)objBody).State = System.Data.EntityState.Modified;
                    }

                    break;
                case HiLifeDbModelType.TxtBody:
                    for (i = 0; i < argListBody.Count; i++)
                    {
                        objBody = argListBody[i] as TxtBody;
                        objDb.Entry<TxtBody>((TxtBody)objBody).State = System.Data.EntityState.Modified;
                    }

                    break;
            }

            try
            {
                objDb.SaveChanges();
                boolExec = true;
            }
            catch (Exception ex)
            {
                string strMessage = "";
                strMessage += "時間: " + DateTime.Now.ToString() + "\r\n";
                strMessage += "Function: HiLifeAnalysisRepository.UpdateBody<T>(List<T> argListBody)" + "\r\n";
                strMessage += "FileType: " + objHiLifeModelType.ToString() + "\r\n";
                strMessage += "錯誤訊息: " + ex.Message + "\r\n\r\n";
                this.LogMessage(strMessage);
            }

            return boolExec;
        }

        /// <summary>
        /// 根據Body Id取得Body物件
        /// </summary>
        /// <typeparam name="T">F00Body, F01Body, ... F09Body, TxtBody</typeparam>
        /// <param name="argNumBodyId">Body Id</param>
        /// <returns>有資料傳回對應T型態的Body物件, 無資料回傳null</returns>
        public T GetBody<T>(int argNumBodyId)
        {
            if (argNumBodyId <= 0)
            {
                return default(T);
            }

            TWNewEgg.DB.TWSqlDBContext objDb = null;
            object objBody = null;
            HiLifeDbModelType objHiLifeModelType = HiLifeDbModelType.Unknown;

            //判定T的Model名稱,轉為列舉
            objHiLifeModelType = (HiLifeDbModelType)Enum.Parse(typeof(HiLifeDbModelType), typeof(T).Name);

            objDb = new TWSqlDBContext();

            //利用列舉來查詢對應的table
            switch (objHiLifeModelType)
            {
                case HiLifeDbModelType.F00Body:
                    objBody = objDb.HiLifeF00Body.Where(x => x.Id == argNumBodyId).FirstOrDefault();
                    break;
                case HiLifeDbModelType.F01Body:
                    objBody = objDb.HiLifeF01Body.Where(x => x.Id == argNumBodyId).FirstOrDefault();
                    break;
                case HiLifeDbModelType.F02Body:
                    objBody = objDb.HiLifeF02Body.Where(x => x.Id == argNumBodyId).FirstOrDefault();
                    break;
                case HiLifeDbModelType.F03Body:
                    objBody = objDb.HiLifeF03Body.Where(x => x.Id == argNumBodyId).FirstOrDefault();
                    break;
                case HiLifeDbModelType.F04Body:
                    objBody = objDb.HiLifeF04Body.Where(x => x.Id == argNumBodyId).FirstOrDefault();
                    break;
                case HiLifeDbModelType.F05Body:
                    objBody = objDb.HiLifeF05Body.Where(x => x.Id == argNumBodyId).FirstOrDefault();
                    break;
                case HiLifeDbModelType.F06Body:
                    objBody = objDb.HiLifeF06Body.Where(x => x.Id == argNumBodyId).FirstOrDefault();
                    break;
                case HiLifeDbModelType.F08Body:
                    objBody = objDb.HiLifeF08Body.Where(x => x.Id == argNumBodyId).FirstOrDefault();
                    break;
                case HiLifeDbModelType.F09Body:
                    objBody = objDb.HiLifeF09Body.Where(x => x.Id == argNumBodyId).FirstOrDefault();
                    break;
                case HiLifeDbModelType.TxtBody:
                    objBody = objDb.HiLifeTxtBody.Where(x => x.Id == argNumBodyId).FirstOrDefault();
                    break;
            }

            objDb.Dispose();
            objDb = null;

            return (T)objBody;
        }

        /// <summary>
        /// 根據Head Id回傳相對應的所有Body物件
        /// </summary>
        /// <typeparam name="T">F00Body, F01Body, ... F09Body, TxtBody</typeparam>
        /// <param name="argNumHeadId">Head Id</param>
        /// <returns>有資料傳回對應T型態的Body列表, 無資料回傳null</returns>
        public List<T> GetBodyByHeadId<T>(int argNumHeadId)
        {
            if (argNumHeadId <= 0)
            {
                return null;
            }

            List<object> listBody = null;
            //List<T> listTBody = null;
            List<T> listTBody = null;
            TWSqlDBContext objDb = null;
            HiLifeDbModelType objHiLifeModelType = HiLifeDbModelType.Unknown;

            //判定T的Model名稱,轉為列舉
            objHiLifeModelType = (HiLifeDbModelType)Enum.Parse(typeof(HiLifeDbModelType), typeof(T).Name);
            objDb = new TWSqlDBContext();

            //利用列舉來查詢對應的table
            switch (objHiLifeModelType)
            {
                case HiLifeDbModelType.F00Body:
                    listBody = objDb.HiLifeF00Body.Where(x => x.Id == argNumHeadId).ToList<object>();
                    break;
                case HiLifeDbModelType.F01Body:
                    listBody = objDb.HiLifeF01Body.Where(x => x.HeadId == argNumHeadId).ToList<object>();
                    break;
                case HiLifeDbModelType.F02Body:
                    listBody = objDb.HiLifeF02Body.Where(x => x.HeadId == argNumHeadId).ToList<object>();
                    break;
                case HiLifeDbModelType.F03Body:
                    listBody = objDb.HiLifeF03Body.Where(x => x.HeadId == argNumHeadId).ToList<object>();
                    break;
                case HiLifeDbModelType.F04Body:
                    listBody = objDb.HiLifeF04Body.Where(x => x.HeadId == argNumHeadId).ToList<object>();
                    break;
                case HiLifeDbModelType.F05Body:
                    listBody = objDb.HiLifeF05Body.Where(x => x.HeadId == argNumHeadId).ToList<object>();
                    break;
                case HiLifeDbModelType.F06Body:
                    listBody = objDb.HiLifeF06Body.Where(x => x.HeadId == argNumHeadId).ToList<object>();
                    break;
                case HiLifeDbModelType.F08Body:
                    listBody = objDb.HiLifeF08Body.Where(x => x.HeadId == argNumHeadId).ToList<object>();
                    break;
                case HiLifeDbModelType.F09Body:
                    listBody = objDb.HiLifeF09Body.Where(x => x.HeadId == argNumHeadId).ToList<object>();
                    break;
                case HiLifeDbModelType.TxtBody:
                    listBody = objDb.HiLifeTxtBody.Where(x => x.Id == argNumHeadId).ToList<object>();
                    break;
            }

            objDb.Dispose();
            objDb = null;

            //進行泛型轉型
            if (listBody != null && listBody.Count > 0)
            {
                listTBody = listBody.Cast<T>().ToList();
            }

            listBody = null;

            return listTBody;
        }

        /// <summary>
        /// 新增Tail(表尾)物件
        /// </summary>
        /// <typeparam name="T">F01Tail, F02Tail ... F09Tail</typeparam>
        /// <param name="argObjTail">欲新增的Tail物件</param>
        /// <returns>>新增成功回傳Tail Id, 新增失敗回傳0或-1</returns>
        public int CreateTail<T>(T argObjTail)
        {
            TWNewEgg.DB.TWSqlDBContext objDb = null;
            int numTailId = 0;
            object objTail = null;

            HiLifeDbModelType objHiLifeModelType = HiLifeDbModelType.Unknown;

            //判定T的Model名稱,轉為列舉
            objHiLifeModelType = (HiLifeDbModelType)Enum.Parse(typeof(HiLifeDbModelType), typeof(T).Name);

            objDb = new TWSqlDBContext();

            //利用列舉來查詢對應的table
            switch (objHiLifeModelType)
            {
                case HiLifeDbModelType.F01Tail:
                    objTail = argObjTail as F01Tail;
                    objDb.HiLifeF01Tail.Add((F01Tail)objTail);
                    try
                    {
                        objDb.SaveChanges();
                        numTailId = (int)((F01Tail)objTail).Id;
                    }
                    catch (Exception ex)
                    {
                        numTailId = 0;

                        string strMessage = "";
                        strMessage += "時間: " + DateTime.Now.ToString() + "\r\n";
                        strMessage += "Function: HiLifeAnalysisRepository.CreateTail" + "\r\n";
                        strMessage += "FileType: " + objHiLifeModelType.ToString() + "\r\n";
                        strMessage += "錯誤訊息: " + ex.Message + "\r\n\r\n";
                        this.LogMessage(strMessage);
                    }

                    break;
                case HiLifeDbModelType.F02Tail:
                    objTail = argObjTail as F02Tail;
                    objDb.HiLifeF02Tail.Add((F02Tail)objTail);
                    try
                    {
                        objDb.SaveChanges();
                        numTailId = (int)((F02Tail)objTail).Id;
                    }
                    catch (Exception ex)
                    {
                        numTailId = 0;
                        string strMessage = "";
                        strMessage += "時間: " + DateTime.Now.ToString() + "\r\n";
                        strMessage += "Function: HiLifeAnalysisRepository.CreateTail" + "\r\n";
                        strMessage += "FileType: " + objHiLifeModelType.ToString() + "\r\n";
                        strMessage += "錯誤訊息: " + ex.Message + "\r\n\r\n";
                        this.LogMessage(strMessage);
                    }

                    break;
                case HiLifeDbModelType.F03Tail:
                    objTail = argObjTail as F03Tail;
                    objDb.HiLifeF03Tail.Add((F03Tail)objTail);
                    try
                    {
                        objDb.SaveChanges();
                        numTailId = (int)((F03Tail)objTail).Id;
                    }
                    catch (Exception ex)
                    {
                        numTailId = 0;
                        string strMessage = "";
                        strMessage += "時間: " + DateTime.Now.ToString() + "\r\n";
                        strMessage += "Function: HiLifeAnalysisRepository.CreateTail" + "\r\n";
                        strMessage += "FileType: " + objHiLifeModelType.ToString() + "\r\n";
                        strMessage += "錯誤訊息: " + ex.Message + "\r\n\r\n";
                        this.LogMessage(strMessage);
                    }

                    break;
                case HiLifeDbModelType.F04Tail:
                    objTail = argObjTail as F04Tail;
                    objDb.HiLifeF04Tail.Add((F04Tail)objTail);
                    try
                    {
                        objDb.SaveChanges();
                        numTailId = (int)((F04Tail)objTail).Id;
                    }
                    catch (Exception ex)
                    {
                        numTailId = 0;
                        string strMessage = "";
                        strMessage += "時間: " + DateTime.Now.ToString() + "\r\n";
                        strMessage += "Function: HiLifeAnalysisRepository.CreateTail" + "\r\n";
                        strMessage += "FileType: " + objHiLifeModelType.ToString() + "\r\n";
                        strMessage += "錯誤訊息: " + ex.Message + "\r\n\r\n";
                        this.LogMessage(strMessage);
                    }

                    break;
                case HiLifeDbModelType.F05Tail:
                    objTail = argObjTail as F05Tail;
                    objDb.HiLifeF05Tail.Add((F05Tail)objTail);
                    try
                    {
                        objDb.SaveChanges();
                        numTailId = (int)((F05Tail)objTail).Id;
                    }
                    catch (Exception ex)
                    {
                        numTailId = 0;
                        string strMessage = "";
                        strMessage += "時間: " + DateTime.Now.ToString() + "\r\n";
                        strMessage += "Function: HiLifeAnalysisRepository.CreateTail" + "\r\n";
                        strMessage += "FileType: " + objHiLifeModelType.ToString() + "\r\n";
                        strMessage += "錯誤訊息: " + ex.Message + "\r\n\r\n";
                        this.LogMessage(strMessage);
                    }

                    break;
                case HiLifeDbModelType.F06Tail:
                    objTail = argObjTail as F06Tail;
                    objDb.HiLifeF06Tail.Add((F06Tail)objTail);
                    try
                    {
                        objDb.SaveChanges();
                        numTailId = (int)((F06Tail)objTail).Id;
                    }
                    catch (Exception ex)
                    {
                        numTailId = 0;
                        string strMessage = "";
                        strMessage += "時間: " + DateTime.Now.ToString() + "\r\n";
                        strMessage += "Function: HiLifeAnalysisRepository.CreateTail" + "\r\n";
                        strMessage += "FileType: " + objHiLifeModelType.ToString() + "\r\n";
                        strMessage += "錯誤訊息: " + ex.Message + "\r\n\r\n";
                        this.LogMessage(strMessage);
                    }

                    break;
                case HiLifeDbModelType.F08Tail:
                    objTail = argObjTail as F08Tail;
                    objDb.HiLifeF08Tail.Add((F08Tail)objTail);
                    try
                    {
                        objDb.SaveChanges();
                        numTailId = (int)((F08Tail)objTail).Id;
                    }
                    catch (Exception ex)
                    {
                        numTailId = 0;
                        string strMessage = "";
                        strMessage += "時間: " + DateTime.Now.ToString() + "\r\n";
                        strMessage += "Function: HiLifeAnalysisRepository.CreateTail" + "\r\n";
                        strMessage += "FileType: " + objHiLifeModelType.ToString() + "\r\n";
                        strMessage += "錯誤訊息: " + ex.Message + "\r\n\r\n";
                        this.LogMessage(strMessage);
                    }

                    break;
                case HiLifeDbModelType.F09Tail:
                    objTail = argObjTail as F09Tail;
                    objDb.HiLifeF09Tail.Add((F09Tail)objTail);
                    try
                    {
                        objDb.SaveChanges();
                        numTailId = (int)((F09Tail)objTail).Id;
                    }
                    catch (Exception ex)
                    {
                        numTailId = 0;
                        string strMessage = "";
                        strMessage += "時間: " + DateTime.Now.ToString() + "\r\n";
                        strMessage += "Function: HiLifeAnalysisRepository.CreateTail" + "\r\n";
                        strMessage += "FileType: " + objHiLifeModelType.ToString() + "\r\n";
                        strMessage += "錯誤訊息: " + ex.Message + "\r\n\r\n";
                        this.LogMessage(strMessage);
                    }

                    break;
            }

            return numTailId;
        }

        /// <summary>
        /// 修改Tail(表尾)物件
        /// </summary>
        /// <typeparam name="T">F01Tail, F02Tail ... F09Tail</typeparam>
        /// <param name="argObjTail">欲修改的Tail物件</param>
        /// <returns>修改成功回傳true, 修改失敗回傳false</returns>
        public bool UpdateTail<T>(T argObjTail)
        {
            bool boolExec = false;
            TWNewEgg.DB.TWSqlDBContext objDb = null;

            object objTail = null;

            HiLifeDbModelType objHiLifeModelType = HiLifeDbModelType.Unknown;

            //判定T的Model名稱,轉為列舉
            objHiLifeModelType = (HiLifeDbModelType)Enum.Parse(typeof(HiLifeDbModelType), typeof(T).Name);

            objDb = new TWSqlDBContext();

            //利用列舉來查詢對應的table
            switch (objHiLifeModelType)
            {
                case HiLifeDbModelType.F01Tail:
                    objTail = argObjTail as F01Tail;
                    objDb.Entry<F01Tail>((F01Tail)objTail).State = System.Data.EntityState.Modified;
                    break;
                case HiLifeDbModelType.F02Tail:
                    objTail = argObjTail as F02Tail;
                    objDb.Entry<F02Tail>((F02Tail)objTail).State = System.Data.EntityState.Modified;
                    break;
                case HiLifeDbModelType.F03Tail:
                    objTail = argObjTail as F03Tail;
                    objDb.Entry<F03Tail>((F03Tail)objTail).State = System.Data.EntityState.Modified;
                    break;
                case HiLifeDbModelType.F04Tail:
                    objTail = argObjTail as F04Tail;
                    objDb.Entry<F04Tail>((F04Tail)objTail).State = System.Data.EntityState.Modified;
                    break;
                case HiLifeDbModelType.F05Tail:
                    objTail = argObjTail as F05Tail;
                    objDb.Entry<F05Tail>((F05Tail)objTail).State = System.Data.EntityState.Modified;
                    break;
                case HiLifeDbModelType.F06Tail:
                    objTail = argObjTail as F06Tail;
                    objDb.Entry<F06Tail>((F06Tail)objTail).State = System.Data.EntityState.Modified;
                    break;
                case HiLifeDbModelType.F08Tail:
                    objTail = argObjTail as F08Tail;
                    objDb.Entry<F08Tail>((F08Tail)objTail).State = System.Data.EntityState.Modified;
                    break;
                case HiLifeDbModelType.F09Tail:
                    objTail = argObjTail as F09Tail;
                    objDb.Entry<F09Tail>((F09Tail)objTail).State = System.Data.EntityState.Modified;
                    break;
            }

            try
            {
                objDb.SaveChanges();
                boolExec = true;
            }
            catch (Exception ex)
            {
                string strMessage = "";
                strMessage += "時間: " + DateTime.Now.ToString() + "\r\n";
                strMessage += "Function: HiLifeAnalysisRepository.UpdateTail" + "\r\n";
                strMessage += "FileType: " + objHiLifeModelType.ToString() + "\r\n";
                strMessage += "錯誤訊息: " + ex.Message + "\r\n\r\n";
                this.LogMessage(strMessage);
            }

            return boolExec;
        }

        /// <summary>
        /// 根據Tail Id取得Tail物件
        /// </summary>
        /// <typeparam name="T">F01Tail, F02Tail ... F09Tail</typeparam>
        /// <param name="argNumTailId">Tail Id</param>
        /// <returns>有資料傳回對應T型態的Tail物件, 無資料回傳null</returns>
        public T GetTail<T>(int argNumTailId)
        {
            if (argNumTailId <= 0)
            {
                return default(T);
            }

            TWNewEgg.DB.TWSqlDBContext objDb = null;
            object objTail = null;
            HiLifeDbModelType objHiLifeModelType = HiLifeDbModelType.Unknown;

            //判定T的Model名稱,轉為列舉
            objHiLifeModelType = (HiLifeDbModelType)Enum.Parse(typeof(HiLifeDbModelType), typeof(T).Name);

            objDb = new TWSqlDBContext();

            //利用列舉來查詢對應的table
            switch (objHiLifeModelType)
            {
                case HiLifeDbModelType.F01Tail:
                    objTail = objDb.HiLifeF01Tail.Where(x => x.Id == argNumTailId).FirstOrDefault();
                    break;
                case HiLifeDbModelType.F02Tail:
                    objTail = objDb.HiLifeF02Tail.Where(x => x.Id == argNumTailId).FirstOrDefault();
                    break;
                case HiLifeDbModelType.F03Tail:
                    objTail = objDb.HiLifeF03Tail.Where(x => x.Id == argNumTailId).FirstOrDefault();
                    break;
                case HiLifeDbModelType.F04Tail:
                    objTail = objDb.HiLifeF04Tail.Where(x => x.Id == argNumTailId).FirstOrDefault();
                    break;
                case HiLifeDbModelType.F05Tail:
                    objTail = objDb.HiLifeF05Tail.Where(x => x.Id == argNumTailId).FirstOrDefault();
                    break;
                case HiLifeDbModelType.F06Tail:
                    objTail = objDb.HiLifeF06Tail.Where(x => x.Id == argNumTailId).FirstOrDefault();
                    break;
                case HiLifeDbModelType.F08Tail:
                    objTail = objDb.HiLifeF08Tail.Where(x => x.Id == argNumTailId).FirstOrDefault();
                    break;
                case HiLifeDbModelType.F09Tail:
                    objTail = objDb.HiLifeF09Tail.Where(x => x.Id == argNumTailId).FirstOrDefault();
                    break;
            }

            objDb.Dispose();
            objDb = null;

            return (T)objTail;
        }

        /// <summary>
        /// 根據Head Id取得Tail物件
        /// </summary>
        /// <typeparam name="T">F01Tail, F02Tail ... F09Tail</typeparam>
        /// <param name="argNumHeadId">Head Id</param>
        /// <returns>有資料傳回對應T型態的Tail物件, 無資料回傳null</returns>
        public T GetTailByHeadId<T>(int argNumHeadId)
        {
            if (argNumHeadId <= 0)
            {
                return default(T);
            }

            TWSqlDBContext objDb = null;
            object objTail = null;
            HiLifeDbModelType objHiLifeModelType = HiLifeDbModelType.Unknown;

            //判定T的Model名稱,轉為列舉
            objHiLifeModelType = (HiLifeDbModelType)Enum.Parse(typeof(HiLifeDbModelType), typeof(T).Name);
            objDb = new TWSqlDBContext();

            //利用列舉來查詢對應的table
            switch (objHiLifeModelType)
            {
                case HiLifeDbModelType.F01Tail:
                    objTail = objDb.HiLifeF01Tail.Where(x => x.HeadId == argNumHeadId).FirstOrDefault();
                    break;
                case HiLifeDbModelType.F02Tail:
                    objTail = objDb.HiLifeF02Tail.Where(x => x.HeadId == argNumHeadId).FirstOrDefault();
                    break;
                case HiLifeDbModelType.F03Tail:
                    objTail = objDb.HiLifeF03Tail.Where(x => x.HeadId == argNumHeadId).FirstOrDefault();
                    break;
                case HiLifeDbModelType.F04Tail:
                    objTail = objDb.HiLifeF04Tail.Where(x => x.HeadId == argNumHeadId).FirstOrDefault();
                    break;
                case HiLifeDbModelType.F05Tail:
                    objTail = objDb.HiLifeF05Tail.Where(x => x.HeadId == argNumHeadId).FirstOrDefault();
                    break;
                case HiLifeDbModelType.F06Tail:
                    objTail = objDb.HiLifeF06Tail.Where(x => x.HeadId == argNumHeadId).FirstOrDefault();
                    break;
                case HiLifeDbModelType.F08Tail:
                    objTail = objDb.HiLifeF08Tail.Where(x => x.HeadId == argNumHeadId).FirstOrDefault();
                    break;
                case HiLifeDbModelType.F09Tail:
                    objTail = objDb.HiLifeF09Tail.Where(x => x.HeadId == argNumHeadId).FirstOrDefault();
                    break;
            }

            objDb.Dispose();
            objDb = null;
            
            return (T)objTail;
        }

        /// <summary>
        /// 根據建立時間取得Body列表
        /// </summary>
        /// <typeparam name="T">F00Body, F01Body, ... F09Body, TxtBody</typeparam>
        /// <param name="argCreateDate">建立時間, 僅比對Year, Month, Day</param>
        /// <returns>有資料傳回對應T型態的List of Body物件, 無資料回傳null</returns>
        public List<T> GetBodyByCreareDate<T>(DateTime argCreateDate)
        {
            List<object> listBody = null;
            //List<T> listTBody = null;
            List<T> listTBody = null;
            TWSqlDBContext objDb = null;
            HiLifeDbModelType objHiLifeModelType = HiLifeDbModelType.Unknown;
            
            DateTime objDateStart;
            DateTime objDateEnd;
            string strDate = argCreateDate.Year.ToString().PadLeft(4, ' ') + argCreateDate.Month.ToString().PadLeft(2, '0') + argCreateDate.Day.ToString().PadLeft(2, '0');

            objDateStart = Convert.ToDateTime(argCreateDate.Year.ToString() + "-" + argCreateDate.Month.ToString() + "-" + argCreateDate.Day.ToString());
            objDateEnd = objDateStart.AddDays(1);

            //判定T的Model名稱,轉為列舉
            objHiLifeModelType = (HiLifeDbModelType)Enum.Parse(typeof(HiLifeDbModelType), typeof(T).Name);
            objDb = new TWSqlDBContext();

            //利用列舉來查詢對應的table
            switch (objHiLifeModelType)
            {
                case HiLifeDbModelType.F00Body:
                    listBody = objDb.HiLifeF00Body.Where(x => x.CreateDate >= objDateStart && x.CreateDate <= objDateEnd).DefaultIfEmpty().ToList<object>();
                    break;
                case HiLifeDbModelType.F01Body:
                    listBody = objDb.HiLifeF01Body.Where(x => x.CreateDate >= objDateStart && x.CreateDate <= objDateEnd).DefaultIfEmpty().ToList<object>();
                    break;
                case HiLifeDbModelType.F02Body:
                    listBody = objDb.HiLifeF02Body.Where(x => x.CreateDate >= objDateStart && x.CreateDate <= objDateEnd).DefaultIfEmpty().ToList<object>();
                    break;
                case HiLifeDbModelType.F04Body:
                    listBody = objDb.HiLifeF04Body.Where(x => x.CreateDate >= objDateStart && x.CreateDate <= objDateEnd).DefaultIfEmpty().ToList<object>();
                    break;
                case HiLifeDbModelType.F05Body:
                    listBody = objDb.HiLifeF05Body.Where(x => x.CreateDate >= objDateStart && x.CreateDate <= objDateEnd).DefaultIfEmpty().ToList<object>();
                    break;
                case HiLifeDbModelType.F06Body:
                    listBody = objDb.HiLifeF06Body.Where(x => x.CreateDate >= objDateStart && x.CreateDate <= objDateEnd).DefaultIfEmpty().ToList<object>();
                    break;
                case HiLifeDbModelType.F08Body:
                    listBody = objDb.HiLifeF08Body.Where(x => x.CreateDate >= objDateStart && x.CreateDate <= objDateEnd).DefaultIfEmpty().ToList<object>();
                    break;
                case HiLifeDbModelType.F09Body:
                    listBody = objDb.HiLifeF09Body.Where(x => x.CreateDate >= objDateStart && x.CreateDate <= objDateEnd).DefaultIfEmpty().ToList<object>();
                    break;
                case HiLifeDbModelType.TxtBody:
                    listBody = objDb.HiLifeTxtBody.Where(x => (x.CreateDate >= objDateStart && x.CreateDate <= objDateEnd)).DefaultIfEmpty().ToList<object>();
                    break;
            }

            objDb.Dispose();
            objDb = null;

            //進行泛型轉型
            if (listBody != null && listBody.Count > 0)
            {
                listTBody = listBody.Cast<T>().ToList();
            }

            listBody = null;
            return listTBody;
        }

        /// <summary>
        /// 根據檔案Type開始進行資料轉換並存進DB
        /// </summary>
        /// <param name="argFileType">HiLifeFileType</param>
        /// <param name="argStrFilePath">來源完整檔案名稱(包含路徑及檔名)</param>
        /// <param name="argEncodingType">Encoding</param>
        /// <returns>轉換筆數, 若回傳-1請先進行GetFieldStruct進行結構檔設定</returns>
        public int ParseFileToDb(HiLifeFileType argFileType, string argStrFilePath, Encoding argEncodingType)
        {
            if (this.mdictFieldStruct == null)
            {
                return -1;
            }

            if (argStrFilePath.Length <= 0)
            {
                return 0;
            }

            int numAddRecordsAmount = 0;
            StreamReader objSr = null;
            string strOneRecord = "";

            object objHead = null;
            object objBody = null;
            object objTail = null;
            int numId = 0;
            int numRecordIndex = 0;
            int numHeadId = 0;
            string strFieldName = "";
            Dictionary<string, HiLifeBasicFieldStruct> objDictFieldStruct = null;
            HiLifeBasic.HiLifeFormat objRecordFormat = HiLifeBasic.HiLifeFormat.Head;

            byte[] byteOneRecord;

            objSr = new StreamReader(argStrFilePath, argEncodingType);
            while (objSr.Peek() > -1)
            {
                strOneRecord = objSr.ReadLine();
                byteOneRecord = argEncodingType.GetBytes(strOneRecord);
                numRecordIndex = 0;

                //根據第一個字元決定是表頭、表身、或表尾
                if (argFileType == HiLifeFileType.F00 || argFileType == HiLifeFileType.TXT)
                {
                    objRecordFormat = HiLifeBasic.HiLifeFormat.Body;
                }
                else
                {
                    objRecordFormat = (HiLifeBasic.HiLifeFormat)Enum.Parse(typeof(HiLifeBasic.HiLifeFormat), strOneRecord.Substring(0, 1));
                }

                //依照表頭、表身、表尾進行不同的操作
                if (objRecordFormat.Equals(HiLifeBasic.HiLifeFormat.Head))
                {
                    //表頭
                    #region Head
                    objDictFieldStruct = ((this.mdictFieldStruct[argFileType.ToString()])["Head"]);

                    switch (argFileType)
                    {
                        case HiLifeFileType.F00:
                            //F00只有Body
                            break;

                        case HiLifeFileType.F01:
                            objHead = new F01Head();

                            strFieldName = "RDFMT";
                            ((F01Head)objHead).Rdfmt = argEncodingType.GetString(byteOneRecord, numRecordIndex, objDictFieldStruct[strFieldName].Length);
                            numRecordIndex += objDictFieldStruct[strFieldName].Length;

                            strFieldName = "SNCD";
                            ((F01Head)objHead).Sncd = argEncodingType.GetString(byteOneRecord, numRecordIndex, objDictFieldStruct[strFieldName].Length); 
                            numRecordIndex += objDictFieldStruct[strFieldName].Length;

                            strFieldName = "PRDT";
                            ((F01Head)objHead).Prdt = argEncodingType.GetString(byteOneRecord, numRecordIndex, objDictFieldStruct[strFieldName].Length); 
                            numRecordIndex += objDictFieldStruct[strFieldName].Length;

                            strFieldName = "FIL";
                            ((F01Head)objHead).Fil = argEncodingType.GetString(byteOneRecord, numRecordIndex, objDictFieldStruct[strFieldName].Length); 
                            numRecordIndex += objDictFieldStruct[strFieldName].Length;

                            numId = this.CreateHead<F01Head>((F01Head)objHead);
                            if (numId > 0)
                            {
                                numAddRecordsAmount++;
                                numHeadId = numId;
                            }
                            else
                            {
                                numHeadId = 0;
                            }

                            break;

                        case HiLifeFileType.F02:
                            objHead = new F02Head();

                            strFieldName = "RDFMT";
                            ((F02Head)objHead).Rdfmt = argEncodingType.GetString(byteOneRecord, numRecordIndex, objDictFieldStruct[strFieldName].Length);
                            numRecordIndex += objDictFieldStruct[strFieldName].Length;

                            strFieldName = "SNCD";
                            ((F02Head)objHead).Sncd = argEncodingType.GetString(byteOneRecord, numRecordIndex, objDictFieldStruct[strFieldName].Length); 
                            numRecordIndex += objDictFieldStruct[strFieldName].Length;

                            strFieldName = "PRDT";
                            ((F02Head)objHead).Prdt = argEncodingType.GetString(byteOneRecord, numRecordIndex, objDictFieldStruct[strFieldName].Length);
                            numRecordIndex += objDictFieldStruct[strFieldName].Length;

                            strFieldName = "FIL";
                            ((F02Head)objHead).Fil = argEncodingType.GetString(byteOneRecord, numRecordIndex, objDictFieldStruct[strFieldName].Length);

                            numId = this.CreateHead<F02Head>((F02Head)objHead);
                            if (numId > 0)
                            {
                                numAddRecordsAmount++;
                                numHeadId = numId;
                            }
                            else
                            {
                                numHeadId = 0;
                            }

                            break;

                        case HiLifeFileType.F04:
                            objHead = new F04Head();

                            strFieldName = "RDFMT";
                            ((F04Head)objHead).Rdfmt = argEncodingType.GetString(byteOneRecord, numRecordIndex, objDictFieldStruct[strFieldName].Length);
                            numRecordIndex += objDictFieldStruct[strFieldName].Length;

                            strFieldName = "SNCD";
                            ((F04Head)objHead).Sncd = argEncodingType.GetString(byteOneRecord, numRecordIndex, objDictFieldStruct[strFieldName].Length);
                            numRecordIndex += objDictFieldStruct[strFieldName].Length;

                            strFieldName = "PRDT";
                            ((F04Head)objHead).Prdt = argEncodingType.GetString(byteOneRecord, numRecordIndex, objDictFieldStruct[strFieldName].Length);
                            numRecordIndex += objDictFieldStruct[strFieldName].Length;

                            strFieldName = "FIL";
                            ((F04Head)objHead).Fil = argEncodingType.GetString(byteOneRecord, numRecordIndex, objDictFieldStruct[strFieldName].Length);

                            numId = this.CreateHead<F04Head>((F04Head)objHead);
                            if (numId > 0)
                            {
                                numAddRecordsAmount++;
                                numHeadId = numId;
                            }
                            else
                            {
                                numHeadId = 0;
                            }

                            break;

                        case HiLifeFileType.F05:
                            objHead = new F05Head();

                            strFieldName = "RDFMT";
                            ((F05Head)objHead).Rdfmt = argEncodingType.GetString(byteOneRecord, numRecordIndex, objDictFieldStruct[strFieldName].Length);
                            numRecordIndex += objDictFieldStruct[strFieldName].Length;

                            strFieldName = "SNCD";
                            ((F05Head)objHead).Sncd = argEncodingType.GetString(byteOneRecord, numRecordIndex, objDictFieldStruct[strFieldName].Length);
                            numRecordIndex += objDictFieldStruct[strFieldName].Length;

                            strFieldName = "PRDT";
                            ((F05Head)objHead).Prdt = argEncodingType.GetString(byteOneRecord, numRecordIndex, objDictFieldStruct[strFieldName].Length);
                            numRecordIndex += objDictFieldStruct[strFieldName].Length;

                            strFieldName = "FIL";
                            ((F05Head)objHead).Fil = argEncodingType.GetString(byteOneRecord, numRecordIndex, objDictFieldStruct[strFieldName].Length);

                            numId = this.CreateHead<F05Head>((F05Head)objHead);
                            if (numId > 0)
                            {
                                numAddRecordsAmount++;
                                numHeadId = numId;
                            }
                            else
                            {
                                numHeadId = 0;
                            }

                            break;

                        case HiLifeFileType.F06:
                            objHead = new F06Head();

                            strFieldName = "RDFMT";
                            ((F06Head)objHead).Rdfmt = argEncodingType.GetString(byteOneRecord, numRecordIndex, objDictFieldStruct[strFieldName].Length);
                            numRecordIndex += objDictFieldStruct[strFieldName].Length;

                            strFieldName = "SNCD";
                            ((F06Head)objHead).Sncd = argEncodingType.GetString(byteOneRecord, numRecordIndex, objDictFieldStruct[strFieldName].Length);
                            numRecordIndex += objDictFieldStruct[strFieldName].Length;

                            strFieldName = "PRDT";
                            ((F06Head)objHead).Prdt = argEncodingType.GetString(byteOneRecord, numRecordIndex, objDictFieldStruct[strFieldName].Length);
                            numRecordIndex += objDictFieldStruct[strFieldName].Length;

                            strFieldName = "FIL";
                            ((F06Head)objHead).Fil = argEncodingType.GetString(byteOneRecord, numRecordIndex, objDictFieldStruct[strFieldName].Length);

                            numId = this.CreateHead<F06Head>((F06Head)objHead);
                            if (numId > 0)
                            {
                                numAddRecordsAmount++;
                                numHeadId = numId;
                            }
                            else
                            {
                                numHeadId = 0;
                            }

                            break;

                        case HiLifeFileType.F08:
                            objHead = new F08Head();

                            strFieldName = "RDFMT";
                            ((F08Head)objHead).Rdfmt = argEncodingType.GetString(byteOneRecord, numRecordIndex, objDictFieldStruct[strFieldName].Length);
                            numRecordIndex += objDictFieldStruct[strFieldName].Length;

                            strFieldName = "SNCD";
                            ((F08Head)objHead).Sncd = argEncodingType.GetString(byteOneRecord, numRecordIndex, objDictFieldStruct[strFieldName].Length);
                            numRecordIndex += objDictFieldStruct[strFieldName].Length;

                            strFieldName = "PRDT";
                            ((F08Head)objHead).Prdt = argEncodingType.GetString(byteOneRecord, numRecordIndex, objDictFieldStruct[strFieldName].Length);
                            numRecordIndex += objDictFieldStruct[strFieldName].Length;

                            strFieldName = "FIL";
                            ((F08Head)objHead).Fil = argEncodingType.GetString(byteOneRecord, numRecordIndex, objDictFieldStruct[strFieldName].Length);

                            numId = this.CreateHead<F08Head>((F08Head)objHead);
                            if (numId > 0)
                            {
                                numAddRecordsAmount++;
                                numHeadId = numId;
                            }
                            else
                            {
                                numHeadId = 0;
                            }

                            break;
                        case HiLifeFileType.F09:
                            objHead = new F09Head();

                            strFieldName = "RDFMT";
                            ((F09Head)objHead).Rdfmt = argEncodingType.GetString(byteOneRecord, numRecordIndex, objDictFieldStruct[strFieldName].Length);
                            numRecordIndex += objDictFieldStruct[strFieldName].Length;

                            strFieldName = "SNCD";
                            ((F09Head)objHead).Sncd = argEncodingType.GetString(byteOneRecord, numRecordIndex, objDictFieldStruct[strFieldName].Length);
                            numRecordIndex += objDictFieldStruct[strFieldName].Length;

                            strFieldName = "PRDT";
                            ((F09Head)objHead).Prdt = argEncodingType.GetString(byteOneRecord, numRecordIndex, objDictFieldStruct[strFieldName].Length);
                            numRecordIndex += objDictFieldStruct[strFieldName].Length;

                            strFieldName = "FIL";
                            ((F09Head)objHead).Fil = argEncodingType.GetString(byteOneRecord, numRecordIndex, objDictFieldStruct[strFieldName].Length);
                            numId = this.CreateHead<F09Head>((F09Head)objHead);
                            if (numId > 0)
                            {
                                numAddRecordsAmount++;
                                numHeadId = numId;
                            }
                            else
                            {
                                numHeadId = 0;
                            }

                            break;

                        case HiLifeFileType.TXT:
                            //TXT只有表身
                            break;
                    }
                    #endregion
                }
                else if (objRecordFormat.Equals(HiLifeBasic.HiLifeFormat.Body))
                {
                    //表身
                    #region Body
                    objDictFieldStruct = ((this.mdictFieldStruct[argFileType.ToString()])["Body"]);
                    switch (argFileType)
                    {
                        case HiLifeFileType.F00:
                            objBody = new F00Body();

                            strFieldName = "STNO";
                            ((F00Body)objBody).Stno = argEncodingType.GetString(byteOneRecord, numRecordIndex, objDictFieldStruct[strFieldName].Length);
                            numRecordIndex += objDictFieldStruct[strFieldName].Length;

                            strFieldName = "NEW_STNO";
                            ((F00Body)objBody).New_Stno = argEncodingType.GetString(byteOneRecord, numRecordIndex, objDictFieldStruct[strFieldName].Length);
                            numRecordIndex += objDictFieldStruct[strFieldName].Length;

                            strFieldName = "DCRONO";
                            ((F00Body)objBody).Dcrono = argEncodingType.GetString(byteOneRecord, numRecordIndex, objDictFieldStruct[strFieldName].Length);

                            numId = this.CreateBody<F00Body>((F00Body)objBody);
                            if (numId > 0)
                            {
                                numAddRecordsAmount++;
                            }

                            break;

                        case HiLifeFileType.F01:
                            objBody = new F01Body();

                            strFieldName = "RDFMT";
                            ((F01Body)objBody).Rdfmt = argEncodingType.GetString(byteOneRecord, numRecordIndex, objDictFieldStruct[strFieldName].Length); 
                            numRecordIndex += objDictFieldStruct[strFieldName].Length;

                            strFieldName = "STNO";
                            ((F01Body)objBody).Stno = argEncodingType.GetString(byteOneRecord, numRecordIndex, objDictFieldStruct[strFieldName].Length); 
                            numRecordIndex += objDictFieldStruct[strFieldName].Length;

                            strFieldName = "STNM";
                            ((F01Body)objBody).Stnm = argEncodingType.GetString(byteOneRecord, numRecordIndex, objDictFieldStruct[strFieldName].Length); 
                            numRecordIndex += objDictFieldStruct[strFieldName].Length;

                            strFieldName = "STTEL";
                            ((F01Body)objBody).Sttel = argEncodingType.GetString(byteOneRecord, numRecordIndex, objDictFieldStruct[strFieldName].Length); 
                            numRecordIndex += objDictFieldStruct[strFieldName].Length;

                            strFieldName = "STCITY";
                            ((F01Body)objBody).Stcity = argEncodingType.GetString(byteOneRecord, numRecordIndex, objDictFieldStruct[strFieldName].Length); 
                            numRecordIndex += objDictFieldStruct[strFieldName].Length;

                            strFieldName = "STCNTRY";
                            ((F01Body)objBody).Stcntry = argEncodingType.GetString(byteOneRecord, numRecordIndex, objDictFieldStruct[strFieldName].Length); 
                            numRecordIndex += objDictFieldStruct[strFieldName].Length;

                            strFieldName = "STADR";
                            ((F01Body)objBody).Stadr = argEncodingType.GetString(byteOneRecord, numRecordIndex, objDictFieldStruct[strFieldName].Length); 
                            numRecordIndex += objDictFieldStruct[strFieldName].Length;

                            strFieldName = "ZIPCD";
                            ((F01Body)objBody).Zipcd = argEncodingType.GetString(byteOneRecord, numRecordIndex, objDictFieldStruct[strFieldName].Length); 
                            numRecordIndex += objDictFieldStruct[strFieldName].Length;

                            strFieldName = "DCRONO";
                            ((F01Body)objBody).Dcrono = argEncodingType.GetString(byteOneRecord, numRecordIndex, objDictFieldStruct[strFieldName].Length); 
                            numRecordIndex += objDictFieldStruct[strFieldName].Length;

                            strFieldName = "SDATE";
                            ((F01Body)objBody).Sdate = argEncodingType.GetString(byteOneRecord, numRecordIndex, objDictFieldStruct[strFieldName].Length); 
                            numRecordIndex += objDictFieldStruct[strFieldName].Length;

                            strFieldName = "EDATE";
                            ((F01Body)objBody).Edate = argEncodingType.GetString(byteOneRecord, numRecordIndex, objDictFieldStruct[strFieldName].Length);

                            ((F01Body)objBody).HeadId = numHeadId;
                            
                            numId = this.CreateBody<F01Body>((F01Body)objBody);
                            if (numId > 0)
                            {
                                numAddRecordsAmount++;
                            }

                            break;
                        case HiLifeFileType.F02:
                            objBody = new F02Body();

                            strFieldName = "RDFMT";
                            ((F02Body)objBody).Rdfmt = argEncodingType.GetString(byteOneRecord, numRecordIndex, objDictFieldStruct[strFieldName].Length);
                            numRecordIndex += objDictFieldStruct[strFieldName].Length;

                            strFieldName = "ECNO";
                            ((F02Body)objBody).Ecno = argEncodingType.GetString(byteOneRecord, numRecordIndex, objDictFieldStruct[strFieldName].Length);
                            numRecordIndex += objDictFieldStruct[strFieldName].Length;

                            strFieldName = "STNO";
                            ((F02Body)objBody).Stno = argEncodingType.GetString(byteOneRecord, numRecordIndex, objDictFieldStruct[strFieldName].Length);
                            numRecordIndex += objDictFieldStruct[strFieldName].Length;

                            strFieldName = "STNM";
                            ((F02Body)objBody).Stnm = argEncodingType.GetString(byteOneRecord, numRecordIndex, objDictFieldStruct[strFieldName].Length);
                            numRecordIndex += objDictFieldStruct[strFieldName].Length;

                            strFieldName = "ODNO";
                            ((F02Body)objBody).Odno = argEncodingType.GetString(byteOneRecord, numRecordIndex, objDictFieldStruct[strFieldName].Length);
                            numRecordIndex += objDictFieldStruct[strFieldName].Length;

                            strFieldName = "CUTKNM";
                            ((F02Body)objBody).Cutknm = argEncodingType.GetString(byteOneRecord, numRecordIndex, objDictFieldStruct[strFieldName].Length);
                            numRecordIndex += objDictFieldStruct[strFieldName].Length;

                            strFieldName = "DCRONO";
                            ((F02Body)objBody).Dcrono = argEncodingType.GetString(byteOneRecord, numRecordIndex, objDictFieldStruct[strFieldName].Length);
                            numRecordIndex += objDictFieldStruct[strFieldName].Length;

                            strFieldName = "EDCNO";
                            ((F02Body)objBody).Edcno = argEncodingType.GetString(byteOneRecord, numRecordIndex, objDictFieldStruct[strFieldName].Length);
                            numRecordIndex += objDictFieldStruct[strFieldName].Length;

                            strFieldName = "BC1";
                            ((F02Body)objBody).Bc1 = argEncodingType.GetString(byteOneRecord, numRecordIndex, objDictFieldStruct[strFieldName].Length);
                            numRecordIndex += objDictFieldStruct[strFieldName].Length;

                            strFieldName = "BC2";
                            ((F02Body)objBody).Bc2 = argEncodingType.GetString(byteOneRecord, numRecordIndex, objDictFieldStruct[strFieldName].Length);
                            numRecordIndex += objDictFieldStruct[strFieldName].Length;

                            strFieldName = "REALAMT";
                            ((F02Body)objBody).Realamt = argEncodingType.GetString(byteOneRecord, numRecordIndex, objDictFieldStruct[strFieldName].Length);
                            numRecordIndex += objDictFieldStruct[strFieldName].Length;

                            strFieldName = "PRODNM";
                            ((F02Body)objBody).Prodnm = argEncodingType.GetString(byteOneRecord, numRecordIndex, objDictFieldStruct[strFieldName].Length);

                            ((F02Body)objBody).HeadId = numHeadId;
                            
                            numId = this.CreateBody<F02Body>((F02Body)objBody);
                            if (numId > 0)
                            {
                                numAddRecordsAmount++;
                            }

                            break;
                        case HiLifeFileType.F04:
                            objBody = new F04Body();

                            strFieldName = "RDFMT";
                            ((F04Body)objBody).Rdfmt = argEncodingType.GetString(byteOneRecord, numRecordIndex, objDictFieldStruct[strFieldName].Length);
                            numRecordIndex += objDictFieldStruct[strFieldName].Length;

                            strFieldName = "ECNO";
                            ((F04Body)objBody).Ecno = argEncodingType.GetString(byteOneRecord, numRecordIndex, objDictFieldStruct[strFieldName].Length);
                            numRecordIndex += objDictFieldStruct[strFieldName].Length;

                            strFieldName = "STNO";
                            ((F04Body)objBody).Stno = argEncodingType.GetString(byteOneRecord, numRecordIndex, objDictFieldStruct[strFieldName].Length);
                            numRecordIndex += objDictFieldStruct[strFieldName].Length;

                            strFieldName = "ODNO";
                            ((F04Body)objBody).Odno = argEncodingType.GetString(byteOneRecord, numRecordIndex, objDictFieldStruct[strFieldName].Length);
                            numRecordIndex += objDictFieldStruct[strFieldName].Length;

                            strFieldName = "DCSTDT";
                            ((F04Body)objBody).Dcstdt = argEncodingType.GetString(byteOneRecord, numRecordIndex, objDictFieldStruct[strFieldName].Length);

                            ((F04Body)objBody).HeadId = numHeadId;

                            numId = this.CreateBody<F04Body>((F04Body)objBody);
                            if (numId > 0)
                            {
                                numAddRecordsAmount++;
                            }

                            break;
                        case HiLifeFileType.F05:
                            objBody = new F05Body();

                            strFieldName = "RDFMT";
                            ((F05Body)objBody).Rdfmt = argEncodingType.GetString(byteOneRecord, numRecordIndex, objDictFieldStruct[strFieldName].Length);
                            numRecordIndex += objDictFieldStruct[strFieldName].Length;

                            strFieldName = "CNNO";
                            ((F05Body)objBody).Cnno = argEncodingType.GetString(byteOneRecord, numRecordIndex, objDictFieldStruct[strFieldName].Length);
                            numRecordIndex += objDictFieldStruct[strFieldName].Length;

                            strFieldName = "BC1";
                            ((F05Body)objBody).Bc1 = argEncodingType.GetString(byteOneRecord, numRecordIndex, objDictFieldStruct[strFieldName].Length);
                            numRecordIndex += objDictFieldStruct[strFieldName].Length;

                            strFieldName = "BC2";
                            ((F05Body)objBody).Bc2 = argEncodingType.GetString(byteOneRecord, numRecordIndex, objDictFieldStruct[strFieldName].Length);
                            numRecordIndex += objDictFieldStruct[strFieldName].Length;

                            strFieldName = "STNO";
                            ((F05Body)objBody).Stno = argEncodingType.GetString(byteOneRecord, numRecordIndex, objDictFieldStruct[strFieldName].Length);
                            numRecordIndex += objDictFieldStruct[strFieldName].Length;

                            strFieldName = "RTDT";
                            ((F05Body)objBody).Rtdt = argEncodingType.GetString(byteOneRecord, numRecordIndex, objDictFieldStruct[strFieldName].Length);
                            numRecordIndex += objDictFieldStruct[strFieldName].Length;

                            strFieldName = "REALAMT";
                            ((F05Body)objBody).Realamt = argEncodingType.GetString(byteOneRecord, numRecordIndex, objDictFieldStruct[strFieldName].Length);

                            ((F05Body)objBody).HeadId = numHeadId;

                            numId = this.CreateBody<F05Body>((F05Body)objBody);
                            if (numId > 0)
                            {
                                numAddRecordsAmount++;
                            }

                            break;
                        case HiLifeFileType.F06:
                            objBody = new F06Body();

                            strFieldName = "RDFMT";
                            ((F06Body)objBody).Rdfmt = argEncodingType.GetString(byteOneRecord, numRecordIndex, objDictFieldStruct[strFieldName].Length);
                            numRecordIndex += objDictFieldStruct[strFieldName].Length;

                            strFieldName = "RETYPE";
                            ((F06Body)objBody).Retype = argEncodingType.GetString(byteOneRecord, numRecordIndex, objDictFieldStruct[strFieldName].Length);
                            numRecordIndex += objDictFieldStruct[strFieldName].Length;

                            strFieldName = "ECNO";
                            ((F06Body)objBody).Ecno = argEncodingType.GetString(byteOneRecord, numRecordIndex, objDictFieldStruct[strFieldName].Length);
                            numRecordIndex += objDictFieldStruct[strFieldName].Length;

                            strFieldName = "STNO";
                            ((F06Body)objBody).Stno = argEncodingType.GetString(byteOneRecord, numRecordIndex, objDictFieldStruct[strFieldName].Length);
                            numRecordIndex += objDictFieldStruct[strFieldName].Length;

                            strFieldName = "ODNO";
                            ((F06Body)objBody).Odno = argEncodingType.GetString(byteOneRecord, numRecordIndex, objDictFieldStruct[strFieldName].Length);
                            numRecordIndex += objDictFieldStruct[strFieldName].Length;

                            strFieldName = "SCRTDT";
                            ((F06Body)objBody).Scrtdt = argEncodingType.GetString(byteOneRecord, numRecordIndex, objDictFieldStruct[strFieldName].Length);
                            numRecordIndex += objDictFieldStruct[strFieldName].Length;

                            strFieldName = "REALAMT";
                            ((F06Body)objBody).Realamt = argEncodingType.GetString(byteOneRecord, numRecordIndex, objDictFieldStruct[strFieldName].Length);

                            ((F06Body)objBody).HeadId = numHeadId;

                            numId = this.CreateBody<F06Body>((F06Body)objBody);
                            if (numId > 0)
                            {
                                numAddRecordsAmount++;
                            }

                            break;

                        case HiLifeFileType.F08:
                            objBody = new F08Body();

                            strFieldName = "RDFMT";
                            ((F08Body)objBody).Rdfmt = argEncodingType.GetString(byteOneRecord, numRecordIndex, objDictFieldStruct[strFieldName].Length);
                            numRecordIndex += objDictFieldStruct[strFieldName].Length;

                            strFieldName = "RET_M";
                            ((F08Body)objBody).Ret_m = argEncodingType.GetString(byteOneRecord, numRecordIndex, objDictFieldStruct[strFieldName].Length);
                            numRecordIndex += objDictFieldStruct[strFieldName].Length;

                            strFieldName = "ECNO";
                            ((F08Body)objBody).Ecno = argEncodingType.GetString(byteOneRecord, numRecordIndex, objDictFieldStruct[strFieldName].Length);
                            numRecordIndex += objDictFieldStruct[strFieldName].Length;

                            strFieldName = "STNO";
                            ((F08Body)objBody).Stno = argEncodingType.GetString(byteOneRecord, numRecordIndex, objDictFieldStruct[strFieldName].Length);
                            numRecordIndex += objDictFieldStruct[strFieldName].Length;

                            strFieldName = "ODNO";
                            ((F08Body)objBody).Odno = argEncodingType.GetString(byteOneRecord, numRecordIndex, objDictFieldStruct[strFieldName].Length);
                            numRecordIndex += objDictFieldStruct[strFieldName].Length;

                            ((F08Body)objBody).HeadId = numHeadId;

                            numId = this.CreateBody<F08Body>((F08Body)objBody);
                            if (numId > 0)
                            {
                                numAddRecordsAmount++;
                            }

                            break;
                        case HiLifeFileType.F09:
                            objBody = new F09Body();

                            strFieldName = "RDFMT";
                            ((F09Body)objBody).Rdfmt = argEncodingType.GetString(byteOneRecord, numRecordIndex, objDictFieldStruct[strFieldName].Length);
                            numRecordIndex += objDictFieldStruct[strFieldName].Length;

                            strFieldName = "RET_M";
                            ((F09Body)objBody).Ret_m = argEncodingType.GetString(byteOneRecord, numRecordIndex, objDictFieldStruct[strFieldName].Length);
                            numRecordIndex += objDictFieldStruct[strFieldName].Length;

                            strFieldName = "ECNO";
                            ((F09Body)objBody).Ecno = argEncodingType.GetString(byteOneRecord, numRecordIndex, objDictFieldStruct[strFieldName].Length);
                            numRecordIndex += objDictFieldStruct[strFieldName].Length;

                            strFieldName = "ODNO";
                            ((F09Body)objBody).Odno = argEncodingType.GetString(byteOneRecord, numRecordIndex, objDictFieldStruct[strFieldName].Length);
                            numRecordIndex += objDictFieldStruct[strFieldName].Length;

                            ((F09Body)objBody).HeadId = numHeadId;

                            numId = this.CreateBody<F09Body>((F09Body)objBody);
                            if (numId > 0)
                            {
                                numAddRecordsAmount++;
                            }

                            break;
                        case HiLifeFileType.TXT:
                            objBody = new TxtBody();

                            strFieldName = "STNO";
                            ((TxtBody)objBody).Stno = argEncodingType.GetString(byteOneRecord, numRecordIndex, objDictFieldStruct[strFieldName].Length);
                            numRecordIndex += objDictFieldStruct[strFieldName].Length;

                            numId = this.CreateBody<TxtBody>((TxtBody)objBody);
                            if (numId > 0)
                            {
                                numAddRecordsAmount++;
                            }

                            break;
                    }
                    #endregion
                }
                else if (objRecordFormat.Equals(HiLifeBasic.HiLifeFormat.Tail))
                {
                    //表尾
                    #region Tail
                    objDictFieldStruct = ((this.mdictFieldStruct[argFileType.ToString()])["Tail"]);
                    switch (argFileType)
                    {
                        case HiLifeFileType.F00:
                            //F00只有表身
                            break;

                        case HiLifeFileType.F01:
                            objTail = new F01Tail();

                            strFieldName = "RDFMT";
                            ((F01Tail)objTail).Rdfmt = argEncodingType.GetString(byteOneRecord, numRecordIndex, objDictFieldStruct[strFieldName].Length); 
                            numRecordIndex += objDictFieldStruct[strFieldName].Length;

                            strFieldName = "RDCNT";
                            ((F01Tail)objTail).Rdcnt = argEncodingType.GetString(byteOneRecord, numRecordIndex, objDictFieldStruct[strFieldName].Length); 
                            numRecordIndex += objDictFieldStruct[strFieldName].Length;

                            strFieldName = "FIL2";
                            ((F01Tail)objTail).Fil2 = argEncodingType.GetString(byteOneRecord, numRecordIndex, objDictFieldStruct[strFieldName].Length);

                            ((F01Tail)objTail).HeadId = numHeadId;

                            numId = this.CreateTail<F01Tail>((F01Tail)objTail);
                            if (numId > 0)
                            {
                                numAddRecordsAmount++;
                            }

                            break;
                        case HiLifeFileType.F02:
                            objTail = new F02Tail();

                            strFieldName = "RDFMT";
                            ((F02Tail)objTail).Rdfmt = argEncodingType.GetString(byteOneRecord, numRecordIndex, objDictFieldStruct[strFieldName].Length);
                            numRecordIndex += objDictFieldStruct[strFieldName].Length;

                            strFieldName = "RDCNT";
                            ((F02Tail)objTail).Rdcnt = argEncodingType.GetString(byteOneRecord, numRecordIndex, objDictFieldStruct[strFieldName].Length);
                            numRecordIndex += objDictFieldStruct[strFieldName].Length;

                            strFieldName = "REALAMT";
                            ((F02Tail)objTail).Realamt = argEncodingType.GetString(byteOneRecord, numRecordIndex, objDictFieldStruct[strFieldName].Length);
                            numRecordIndex += objDictFieldStruct[strFieldName].Length;

                            strFieldName = "FIL2";
                            ((F02Tail)objTail).Fil2 = argEncodingType.GetString(byteOneRecord, numRecordIndex, objDictFieldStruct[strFieldName].Length);

                            ((F02Tail)objTail).HeadId = numHeadId;

                            numId = this.CreateTail<F02Tail>((F02Tail)objTail);
                            if (numId > 0)
                            {
                                numAddRecordsAmount++;
                            }

                            break;

                        case HiLifeFileType.F04:
                            objTail = new F04Tail();

                            strFieldName = "RDFMT";
                            ((F04Tail)objTail).Rdfmt = argEncodingType.GetString(byteOneRecord, numRecordIndex, objDictFieldStruct[strFieldName].Length);
                            numRecordIndex += objDictFieldStruct[strFieldName].Length;

                            strFieldName = "RDCNT";
                            ((F04Tail)objTail).Rdcnt = argEncodingType.GetString(byteOneRecord, numRecordIndex, objDictFieldStruct[strFieldName].Length);
                            numRecordIndex += objDictFieldStruct[strFieldName].Length;

                            strFieldName = "FIL2";
                            ((F04Tail)objTail).Fil2 = argEncodingType.GetString(byteOneRecord, numRecordIndex, objDictFieldStruct[strFieldName].Length);

                            ((F04Tail)objTail).HeadId = numHeadId;

                            numId = this.CreateTail<F04Tail>((F04Tail)objTail);
                            if (numId > 0)
                            {
                                numAddRecordsAmount++;
                            }

                            break;
                        case HiLifeFileType.F05:
                            objTail = new F05Tail();

                            strFieldName = "RDFMT";
                            ((F05Tail)objTail).Rdfmt = argEncodingType.GetString(byteOneRecord, numRecordIndex, objDictFieldStruct[strFieldName].Length);
                            numRecordIndex += objDictFieldStruct[strFieldName].Length;

                            strFieldName = "RDCNT";
                            ((F05Tail)objTail).Rdcnt = argEncodingType.GetString(byteOneRecord, numRecordIndex, objDictFieldStruct[strFieldName].Length);
                            numRecordIndex += objDictFieldStruct[strFieldName].Length;

                            strFieldName = "AMT";
                            ((F05Tail)objTail).Amt = argEncodingType.GetString(byteOneRecord, numRecordIndex, objDictFieldStruct[strFieldName].Length);
                            numRecordIndex += objDictFieldStruct[strFieldName].Length;

                            strFieldName = "FIL2";
                            ((F05Tail)objTail).Fil2 = argEncodingType.GetString(byteOneRecord, numRecordIndex, objDictFieldStruct[strFieldName].Length);

                            ((F05Tail)objTail).HeadId = numHeadId;

                            numId = this.CreateTail<F05Tail>((F05Tail)objTail);
                            if (numId > 0)
                            {
                                numAddRecordsAmount++;
                            }

                            break;
                        case HiLifeFileType.F06:
                            objTail = new F06Tail();

                            strFieldName = "RDFMT";
                            ((F06Tail)objTail).Rdfmt = argEncodingType.GetString(byteOneRecord, numRecordIndex, objDictFieldStruct[strFieldName].Length);
                            numRecordIndex += objDictFieldStruct[strFieldName].Length;

                            strFieldName = "RDCNT";
                            ((F06Tail)objTail).Rdcnt = argEncodingType.GetString(byteOneRecord, numRecordIndex, objDictFieldStruct[strFieldName].Length);
                            numRecordIndex += objDictFieldStruct[strFieldName].Length;

                            strFieldName = "REALAMT";
                            ((F06Tail)objTail).Realamt = argEncodingType.GetString(byteOneRecord, numRecordIndex, objDictFieldStruct[strFieldName].Length);
                            numRecordIndex += objDictFieldStruct[strFieldName].Length;

                            strFieldName = "FIL2";
                            ((F06Tail)objTail).Fil2 = argEncodingType.GetString(byteOneRecord, numRecordIndex, objDictFieldStruct[strFieldName].Length);

                            ((F06Tail)objTail).HeadId = numHeadId;

                            numId = this.CreateTail<F06Tail>((F06Tail)objTail);
                            if (numId > 0)
                            {
                                numAddRecordsAmount++;
                            }

                            break;
                        case HiLifeFileType.F08:
                            objTail = new F08Tail();

                            strFieldName = "RDFMT";
                            ((F08Tail)objTail).Rdfmt = argEncodingType.GetString(byteOneRecord, numRecordIndex, objDictFieldStruct[strFieldName].Length);
                            numRecordIndex += objDictFieldStruct[strFieldName].Length;

                            strFieldName = "RDCNT";
                            ((F08Tail)objTail).Rdcnt = argEncodingType.GetString(byteOneRecord, numRecordIndex, objDictFieldStruct[strFieldName].Length);
                            numRecordIndex += objDictFieldStruct[strFieldName].Length;

                            strFieldName = "FIL2";
                            ((F08Tail)objTail).Fil2 = argEncodingType.GetString(byteOneRecord, numRecordIndex, objDictFieldStruct[strFieldName].Length);

                            ((F08Tail)objTail).HeadId = numHeadId;

                            numId = this.CreateTail<F08Tail>((F08Tail)objTail);
                            if (numId > 0)
                            {
                                numAddRecordsAmount++;
                            }

                            break;
                        case HiLifeFileType.F09:
                            objTail = new F09Tail();

                            strFieldName = "RDFMT";
                            ((F09Tail)objTail).Rdfmt = argEncodingType.GetString(byteOneRecord, numRecordIndex, objDictFieldStruct[strFieldName].Length);
                            numRecordIndex += objDictFieldStruct[strFieldName].Length;

                            strFieldName = "RDCNT";
                            ((F09Tail)objTail).Rdcnt = argEncodingType.GetString(byteOneRecord, numRecordIndex, objDictFieldStruct[strFieldName].Length);
                            numRecordIndex += objDictFieldStruct[strFieldName].Length;

                            strFieldName = "FIL2";
                            ((F09Tail)objTail).Fil2 = argEncodingType.GetString(byteOneRecord, numRecordIndex, objDictFieldStruct[strFieldName].Length);

                            ((F09Tail)objTail).HeadId = numHeadId;

                            numId = this.CreateTail<F09Tail>((F09Tail)objTail);
                            if (numId > 0)
                            {
                                numAddRecordsAmount++;
                            }

                            break;
                        case HiLifeFileType.TXT:
                            //TXT只有表身
                            break;
                    }
                    #endregion
                }
                ////end if
            }
            ////end while

            objSr.Close();
            objSr.Dispose();
            objSr = null;

            return numAddRecordsAmount;
        }

        /// <summary>
        /// 產生F02或F06的傳輸檔案
        /// </summary>
        /// <param name="argFileType">HiLifeFileType.F02或HiLifeFileType.F06</param>
        /// <param name="argSearchDate">從資料庫抓取哪一天的交易資料(僅比對Year, Month, Day)</param>
        /// <param name="argOutputDate">產生F02及F06的檔案日期</param>
        /// <param name="argStrTargetPath">產生的檔案最後要輸出至哪一個目錄</param>
        /// <returns>產生成功回傳true, 失敗回傳false</returns>
        public bool ParseDbToFile(HiLifeFileType argFileType, DateTime argSearchDate, DateTime argOutputDate, string argStrTargetPath)
        {
            if (argFileType != HiLifeFileType.F02 && argFileType != HiLifeFileType.F06)
            {
                return false;
            }

            bool boolExec = false;
            StreamWriter objSw = null;
            int numBodyCount = 0;
            int numBodyPriceAmount = 0;
            int numHeadId = 0;

            if (this.mdictFieldStruct == null)
            {
                return false;
            }

            if (argStrTargetPath.Length <= 0)
            {
                return false;
            }

            string strOneRecord = "";
            string strDate = "";

            F02Head objF02Head = null;
            List<F02Body> listF02Body = null;
            F02Tail objF02Tail = null;

            F06Head objF06Head = null;
            List<F06Body> listF06Body = null;
            F06Tail objF06Tail = null;
            
            strDate = argOutputDate.Year.ToString().PadLeft(4, '0') + argOutputDate.Month.ToString().PadLeft(2, '0') + argOutputDate.Day.ToString().PadLeft(2, '0');
            switch (argFileType)
            {
                case HiLifeFileType.F02:
                    //先建Head
                    #region F02-Head
                    objSw = new StreamWriter(argStrTargetPath + strDate + ".F02");

                    strOneRecord = "";
                    objF02Head = new F02Head();
                    objF02Head.Rdfmt = this.ParseText(argFileType, HiLifeBasic.HiLifeFormat.Head, "RDFMT", "1");
                    objF02Head.Sncd = this.ParseText(argFileType, HiLifeBasic.HiLifeFormat.Head, "SNCD", "CVS");
                    objF02Head.Prdt = this.ParseText(argFileType, HiLifeBasic.HiLifeFormat.Head, "PRDT", strDate);
                    objF02Head.Fil = this.ParseText(argFileType, HiLifeBasic.HiLifeFormat.Head, "FIL", "");

                    try
                    {
                        strOneRecord += objF02Head.Rdfmt + objF02Head.Sncd + objF02Head.Prdt + objF02Head.Fil;
                        objSw.WriteLine(strOneRecord);

                        numHeadId = this.CreateHead<F02Head>(objF02Head);
                    }
                    catch (Exception ex)
                    {
                        boolExec = false;
                        string strMessage = "";
                        strMessage += "時間: " + DateTime.Now.ToString() + "\r\n";
                        strMessage += "Function: HiLifeFileSyncRepository.StartAllSchedule" + "\r\n";
                        strMessage += "錯誤訊息: " + ex.Message + "\r\n\r\n";
                        this.LogMessage(strMessage);
                    }
                    #endregion
                
                    //再建Body
                    #region F02-Body
                    listF02Body = this.GetBodyByCreareDate<F02Body>(argSearchDate);
                    if (listF02Body != null)
                    {
                        numBodyPriceAmount = 0;
                        foreach (F02Body objSubF02Body in listF02Body)
                        {
                            try
                            {
                                strOneRecord = "";
                                objSubF02Body.Rdfmt = this.ParseText(argFileType, HiLifeBasic.HiLifeFormat.Body, "RDFMT", objSubF02Body.Rdfmt);
                                objSubF02Body.Ecno = this.ParseText(argFileType, HiLifeBasic.HiLifeFormat.Body, "ECNO", objSubF02Body.Ecno);
                                objSubF02Body.Stno = this.ParseText(argFileType, HiLifeBasic.HiLifeFormat.Body, "STNO", objSubF02Body.Stno);
                                objSubF02Body.Stnm = this.ParseText(argFileType, HiLifeBasic.HiLifeFormat.Body, "STNM", objSubF02Body.Stnm);
                                objSubF02Body.Odno = this.ParseText(argFileType, HiLifeBasic.HiLifeFormat.Body, "ODNO", objSubF02Body.Odno);
                                objSubF02Body.Cutknm = this.ParseText(argFileType, HiLifeBasic.HiLifeFormat.Body, "CUTKNM", objSubF02Body.Cutknm);
                                objSubF02Body.Dcrono = this.ParseText(argFileType, HiLifeBasic.HiLifeFormat.Body, "DCRONO", objSubF02Body.Dcrono);
                                objSubF02Body.Edcno = this.ParseText(argFileType, HiLifeBasic.HiLifeFormat.Body, "EDCNO", objSubF02Body.Edcno);
                                objSubF02Body.Bc1 = this.ParseText(argFileType, HiLifeBasic.HiLifeFormat.Body, "BC1", objSubF02Body.Bc1);
                                objSubF02Body.Bc2 = this.ParseText(argFileType, HiLifeBasic.HiLifeFormat.Body, "BC2", objSubF02Body.Bc2);
                                objSubF02Body.Realamt = this.ParseText(argFileType, HiLifeBasic.HiLifeFormat.Body, "REALAMT", objSubF02Body.Realamt);
                                objSubF02Body.Prodnm = this.ParseText(argFileType, HiLifeBasic.HiLifeFormat.Body, "PRODNM", objSubF02Body.Prodnm);

                                strOneRecord = objSubF02Body.Rdfmt + objSubF02Body.Ecno + objSubF02Body.Stno + objSubF02Body.Stnm + objSubF02Body.Odno + objSubF02Body.Cutknm + objSubF02Body.Dcrono + objSubF02Body.Edcno + objSubF02Body.Bc1 + objSubF02Body.Bc2 + objSubF02Body.Realamt + objSubF02Body.Prodnm;
                                objSw.WriteLine(strOneRecord);

                                numBodyPriceAmount += Convert.ToInt32(objSubF02Body.Realamt);

                                objSubF02Body.HeadId = numHeadId;

                                numBodyCount++;
                            }
                            catch (Exception ex)
                            {
                                boolExec = false;
                                string strMessage = "";
                                strMessage += "時間: " + DateTime.Now.ToString() + "\r\n";
                                strMessage += "Function: HiLifeFileSyncRepository.StartAllSchedule" + "\r\n";
                                strMessage += "錯誤訊息: " + ex.Message + "\r\n\r\n";
                                this.LogMessage(strMessage);
                            }
                        }
                        
                        //修改這些F02Body的HeadId
                        this.UpdateBody<F02Body>(listF02Body);
                    }
                    #endregion

                    //最後建Tail
                    #region F02-Tail
                    strOneRecord = "";
                    objF02Tail = new F02Tail();
                    objF02Tail.Rdfmt = this.ParseText(argFileType, HiLifeBasic.HiLifeFormat.Tail, "RDFMT", "3");
                    objF02Tail.Rdcnt = this.ParseText(argFileType, HiLifeBasic.HiLifeFormat.Tail, "RDCNT", numBodyCount.ToString());
                    objF02Tail.Realamt = this.ParseText(argFileType, HiLifeBasic.HiLifeFormat.Tail, "REALAMT", numBodyPriceAmount.ToString());
                    objF02Tail.Fil2 = this.ParseText(argFileType, HiLifeBasic.HiLifeFormat.Tail, "FIL2", "0");

                    try
                    {
                        strOneRecord = objF02Tail.Rdfmt + objF02Tail.Rdcnt + objF02Tail.Realamt + objF02Tail.Fil2;
                        objSw.WriteLine(strOneRecord);

                        objF02Tail.HeadId = numHeadId;
                        this.CreateTail<F02Tail>(objF02Tail);
                    }
                    catch (Exception ex)
                    {
                        boolExec = false;
                        string strMessage = "";
                        strMessage += "時間: " + DateTime.Now.ToString() + "\r\n";
                        strMessage += "Function: HiLifeFileSyncRepository.StartAllSchedule" + "\r\n";
                        strMessage += "錯誤訊息: " + ex.Message + "\r\n\r\n";
                        this.LogMessage(strMessage);
                    }
                    #endregion

                    if (objSw != null)
                    {
                        objSw.Dispose();
                        objSw = null;
                        boolExec = true;
                    } 
                    
                    break;
                case HiLifeFileType.F06:
                    //建立Head
                    #region F06-Head
                    objSw = new StreamWriter(argStrTargetPath + strDate + ".F06");

                    strOneRecord = "";
                    objF06Head = new F06Head();
                    objF06Head.Rdfmt = this.ParseText(argFileType, HiLifeBasic.HiLifeFormat.Head, "RDFMT", "1");
                    objF06Head.Sncd = this.ParseText(argFileType, HiLifeBasic.HiLifeFormat.Head, "SNCD", "XXX");
                    objF06Head.Prdt = this.ParseText(argFileType, HiLifeBasic.HiLifeFormat.Head, "PRDT", strDate);
                    objF06Head.Fil = this.ParseText(argFileType, HiLifeBasic.HiLifeFormat.Head, "FIL", "");

                    try
                    {
                        strOneRecord += objF06Head.Rdfmt + objF06Head.Sncd + objF06Head.Prdt + objF06Head.Fil;
                        objSw.WriteLine(strOneRecord);

                        numHeadId = this.CreateHead<F06Head>(objF06Head);
                    }
                    catch (Exception ex)
                    {
                        boolExec = false;
                        string strMessage = "";
                        strMessage += "時間: " + DateTime.Now.ToString() + "\r\n";
                        strMessage += "Function: HiLifeFileSyncRepository.StartAllSchedule" + "\r\n";
                        strMessage += "錯誤訊息: " + ex.Message + "\r\n\r\n";
                        this.LogMessage(strMessage);
                    }
                    #endregion

                    //再建Body
                    #region F06-Body
                    listF06Body = this.GetBodyByCreareDate<F06Body>(argSearchDate);
                    if (listF06Body != null)
                    {
                        numBodyPriceAmount = 0;
                        foreach (F06Body objSubF06Body in listF06Body)
                        {
                            try
                            {
                                strOneRecord = "";
                                objSubF06Body.Rdfmt = this.ParseText(argFileType, HiLifeBasic.HiLifeFormat.Body, "RDFMT", objSubF06Body.Rdfmt);
                                objSubF06Body.Retype = this.ParseText(argFileType, HiLifeBasic.HiLifeFormat.Body, "RETYPE", objSubF06Body.Retype);
                                objSubF06Body.Ecno = this.ParseText(argFileType, HiLifeBasic.HiLifeFormat.Body, "ECNO", objSubF06Body.Ecno);
                                objSubF06Body.Stno = this.ParseText(argFileType, HiLifeBasic.HiLifeFormat.Body, "STNO", objSubF06Body.Stno);
                                objSubF06Body.Odno = this.ParseText(argFileType, HiLifeBasic.HiLifeFormat.Body, "ODNO", objSubF06Body.Odno);
                                objSubF06Body.Scrtdt = this.ParseText(argFileType, HiLifeBasic.HiLifeFormat.Body, "SCRTDT", objSubF06Body.Scrtdt);
                                objSubF06Body.Realamt = this.ParseText(argFileType, HiLifeBasic.HiLifeFormat.Body, "REALAMT", objSubF06Body.Realamt);

                                strOneRecord = objSubF06Body.Rdfmt + objSubF06Body.Retype + objSubF06Body.Ecno + objSubF06Body.Stno + objSubF06Body.Odno + objSubF06Body.Scrtdt + objSubF06Body.Realamt;
                                objSw.WriteLine(strOneRecord);

                                numBodyPriceAmount += Convert.ToInt32(objSubF06Body.Realamt);

                                objSubF06Body.HeadId = numHeadId;

                                numBodyCount++;
                            }
                            catch (Exception ex)
                            {
                                boolExec = false;
                                string strMessage = "";
                                strMessage += "時間: " + DateTime.Now.ToString() + "\r\n";
                                strMessage += "Function: HiLifeFileSyncRepository.LoadFileSyncSchedule" + "\r\n";
                                strMessage += "錯誤訊息: " + ex.Message + "\r\n\r\n";
                                this.LogMessage(strMessage);
                            }
                        }

                        // 修改這些F06Body的HeadId
                        this.UpdateBody<F06Body>(listF06Body);
                    }
                    #endregion

                    //最後建Tail
                    #region F06-Tail
                    objF06Tail = new F06Tail();

                    objF06Tail.Rdfmt = this.ParseText(argFileType, HiLifeBasic.HiLifeFormat.Tail, "RDFMT", "3");
                    objF06Tail.Rdcnt = this.ParseText(argFileType, HiLifeBasic.HiLifeFormat.Tail, "RDCNT", numBodyCount.ToString());
                    objF06Tail.Realamt = this.ParseText(argFileType, HiLifeBasic.HiLifeFormat.Tail, "REALAMT", numBodyPriceAmount.ToString());
                    objF06Tail.Fil2 = this.ParseText(argFileType, HiLifeBasic.HiLifeFormat.Tail, "FIL2", "0");

                    try
                    {
                        strOneRecord = objF06Tail.Rdfmt + objF06Tail.Rdcnt + objF06Tail.Realamt + objF06Tail.Fil2;
                        objSw.WriteLine(strOneRecord);

                        objF06Tail.HeadId = numHeadId;
                        this.CreateTail<F06Tail>(objF06Tail);
                    }
                    catch (Exception ex)
                    {
                        boolExec = false;
                        string strMessage = "";
                        strMessage += "時間: " + DateTime.Now.ToString() + "\r\n";
                        strMessage += "Function: HiLifeFileSyncRepository.StartAllSchedule" + "\r\n";
                        strMessage += "錯誤訊息: " + ex.Message + "\r\n\r\n";
                        this.LogMessage(strMessage);
                    }
                    #endregion

                    if (objSw != null)
                    {
                        objSw.Dispose();
                        objSw = null;
                        boolExec = true;
                    }

                    break;
            }
            ////end switch

            return boolExec;
        }

        /// <summary>
        /// 根據結構檔進行欄位長度、對齊方式等等的設定
        /// </summary>
        /// <param name="argStrConfigFilePath">結構檔的完整路徑及檔案</param>
        /// <returns>結構欄位的物件</returns>
        public Dictionary<string, Dictionary<string, Dictionary<string, HiLifeBasicFieldStruct>>> GetFieldStruct(string argStrConfigFilePath)
        {
            if (argStrConfigFilePath.Length <= 0)
            {
                return null;
            }

            this.mobjFileService = new HiLifeFileSyncRepository(argStrConfigFilePath);
            this.mdictFieldStruct = this.mobjFileService.GetFieldStruct();

            return mdictFieldStruct;
        }

        /// <summary>
        /// 將傳入字串依結構檔的規則,進行補齊或截斷
        /// </summary>
        /// <param name="argFileType">列舉HiLifeFileType</param>
        /// <param name="argFormatType">列舉HiLifeBasic.HiLifeFormat</param>
        /// <param name="argStrFieldName">結構檔的欄位名稱</param>
        /// <param name="argStrFieldValue">要修整的原始值</param>
        /// <returns>修整過後的值</returns>
        private string ParseText(HiLifeFileType argFileType, HiLifeBasic.HiLifeFormat argFormatType, string argStrFieldName, string argStrFieldValue)
        {
            if (this.mdictFieldStruct == null)
            {
                return argStrFieldValue;
            }

            Dictionary<string, Dictionary<string, HiLifeBasicFieldStruct>> objDictFieldStruct = null;
            HiLifeBasicFieldStruct objFieldStruct = null;

            System.Text.Encoding objEncoding = System.Text.Encoding.GetEncoding("big5");
            byte[] byteString = objEncoding.GetBytes(argStrFieldValue);
            string strRes = "";
            string strResult = "";

            objDictFieldStruct = this.mdictFieldStruct[argFileType.ToString()];
            if (objDictFieldStruct == null)
            {
                return argStrFieldValue;
            }

            objFieldStruct = objDictFieldStruct[argFormatType.ToString()][argStrFieldName];
            if (objFieldStruct == null)
            {
                return argStrFieldValue;
            }

            objDictFieldStruct = this.mdictFieldStruct[argFileType.ToString()];
            if (objDictFieldStruct == null)
            {
                return argStrFieldValue;
            }

            //若有固定值, 就使用固定值, 若無才使用傳入參數
            if (objFieldStruct.HasStaticValue)
            {
                strResult = objFieldStruct.StaticValue;
            }
            else
            {
                strResult = argStrFieldValue;
            }

            //進行字數長度截斷或補齊
            if (byteString.Length > objFieldStruct.Length)
            {
                //字數截斷
                strRes = objEncoding.GetString(byteString, 0, objFieldStruct.Length);
                if (!objEncoding.GetString(byteString).StartsWith(strRes))
                {
                    //中文字截斷的判斷
                    strRes = objEncoding.GetString(byteString, 0, objFieldStruct.Length - 1);
                }

                strResult = strRes;
            }
            else if (byteString.Length < objFieldStruct.Length)
            {
                //字數補齊
                if (objFieldStruct.Align == HiLifeBasic.Align.Right)
                {
                    //靠右對齊=左方補符號
                    strResult = strResult.PadLeft(objFieldStruct.Length, objFieldStruct.Symbol);
                }
                else
                {
                    strResult = strResult.PadRight(objFieldStruct.Length, objFieldStruct.Symbol);
                }
            }

            objFieldStruct = null;

            return strResult;
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

            if (!Directory.Exists(this.mstrLogFilePath))
            {
                Directory.CreateDirectory(this.mstrLogFilePath);
            }

            strFileName = this.mstrLogFilePath + "\\" + objDate.Year.ToString().PadLeft(4, '0') + objDate.Month.ToString().PadLeft(2, '0') + objDate.Day.ToString().PadLeft(2, '0') + ".AnalysisLog";

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
        /// 設定錯誤訊息Log檔案的目錄(僅目錄)
        /// </summary>
        /// <param name="argStrFilePath">錯誤訊息Log檔案的目錄(僅目錄)</param>
        public void SetLogFilePath(string argStrFilePath)
        {
            if (argStrFilePath.Length <= 0)
            {
                return;
            }

            this.mstrLogFilePath = argStrFilePath.TrimEnd('\\') + "\\";
        }

        /// <summary>
        /// 根據Bc1與Bc2查到F02訂單出貨資訊
        /// </summary>
        /// <typeparam name="T">HiLife Model</typeparam>
        /// <param name="argT">欲查詢的HiLifeModel List</param>
        /// <returns>null或是F02Body列表</returns>
        public List<F02Body> GetF02BodyByBc1AndBc2<T>(List<T> argListT)
        {
            HiLifeDbModelType objHiLifeModelType = HiLifeDbModelType.Unknown;
            TWSqlDBContext objDb = null;
            List<F02Body> listF02Body = null;
            F02Body objF02Body = null;

            //判定T的Model名稱,轉為列舉
            objHiLifeModelType = (HiLifeDbModelType)Enum.Parse(typeof(HiLifeDbModelType), typeof(T).Name);
            objDb = new TWSqlDBContext();
            
            //利用列舉來查詢對應的table
            switch (objHiLifeModelType)
            {
                  case HiLifeDbModelType.F05Body:
                    //進行泛型轉型 listBody.Cast<T>().ToList();
                    List<F05Body> listF05Body = argListT.Cast<F05Body>().ToList();
                    if(listF05Body != null)
                    {
                        listF02Body = new List<F02Body>();
                        foreach(F05Body objSubF05Body in listF05Body)
                        {
                            objF02Body = objDb.HiLifeF02Body.Where(x => x.Bc1 == objSubF05Body.Bc1 && x.Bc2 == objSubF05Body.Bc2).FirstOrDefault();
                            if (objF02Body != null)
                            {
                                listF02Body.Add(objF02Body);
                            }
                        }
                    }

                    break;

            }

            objDb.Dispose();
            objDb = null;

            return listF02Body;
        }
    }
}
