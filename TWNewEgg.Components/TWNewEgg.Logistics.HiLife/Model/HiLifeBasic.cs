using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel;
using System.Text;
using System.Xml;

namespace TWNewEgg.Logistics.HiLife.Model
{
    public enum HiLifeDbModelType
    {
        Unknown = 0,

        HiLifeF00 = 34,
        F00Head = 1,
        F00Body = 2,
        F00Tail = 3,

        HiLifeF01 = 35,
        F01Head = 4,
        F01Body = 5,
        F01Tail = 6,

        HiLifeF02 = 36,
        F02Head = 7,
        F02Body = 8,
        F02Tail = 9,

        HiLifeF03 = 37,
        F03Head = 10,
        F03Body = 11,
        F03Tail = 12,

        HiLifeF04 = 38,
        F04Head = 13,
        F04Body = 14,
        F04Tail = 15,

        HiLifeF05 = 39,
        F05Head = 16,
        F05Body = 17,
        F05Tail = 18,

        HiLifeF06 = 40,
        F06Head = 19,
        F06Body = 20,
        F06Tail = 21,

        HiLifeF07 = 41,
        F07Head = 22,
        F07Body = 23,
        F07Tail = 24,

        HiLifeF08 = 42,
        F08Head = 25,
        F08Body = 26,
        F08Tail = 27,

        HiLifeF09 = 43,
        F09Head = 28,
        F09Body = 29,
        F09Tail = 30,

        HiLifeTxt = 44,
        TxtHead = 31,
        TxtBody = 32,
        TxtTail = 33

    }

    public enum HiLifeFileType
    {
        F00 = 0,
        F01 = 1,
        F02 = 2,
        F03 = 3,
        F04 = 4,
        F05 = 5,
        F06 = 6,
        F08 = 8,
        F09 = 9,
        TXT = 10,

        Unknown = 99
    }
    public class HiLifeBasic
    {
        public enum Align
        {
            None = 0,
            Right = 1,
            Left = 2
        }

        public enum HiLifeFormat
        {
            /// <summary>
            /// 第一個Record
            /// </summary>
            Head = 1,

            /// <summary>
            /// 資料的Record, 一般是由第二列開始起算
            /// </summary>
            Body = 2,

            /// <summary>
            /// 最後一個Record
            /// </summary>
            Tail = 3
        }


        private Dictionary<string, List<HiLifeBasicFieldStruct>> m_objDictField = null;

        public Dictionary<string, List<HiLifeBasicFieldStruct>> DictFieldStruct
        {
            get
            {
                return this.m_objDictField;
            }
        }

        /// <summary>
        /// 將傳入字串依設定進行補齊或截斷
        /// </summary>
        /// <param name="argStrValue">傳入字串</param>
        /// <param name="argNumTargetLengh">規定長度</param>
        /// <param name="argPadSymbol">補齊符號</param>
        /// <param name="argAlign">對齊方式</param>
        /// <returns>正規格式的字串</returns>
        protected string ParseText(string argStrValue, int argNumTargetLengh, char argPadSymbol, HiLifeBasic.Align argAlign)
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
                    strRes = objEncoding.GetString(byteString, 0, argNumTargetLengh-1);
                }

                strResult = strRes;
            }
            else if (byteString.Length < argNumTargetLengh)
            {
                if (argAlign == Align.Left)
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
        /// 根據字串規定長度補齊或截斷
        /// </summary>
        /// <param name="argFormat">HiLifeFormat</param>
        /// <param name="argStrFieldName">Field Name</param>
        /// <param name="argStrFieldValue">Field Value</param>
        /// <returns></returns>
        protected string ParseText(HiLifeFormat argFormat, string argStrFieldName, string argStrFieldValue)
        {
            System.Text.Encoding objEncoding = System.Text.Encoding.GetEncoding("big5");
            byte[] byteString = objEncoding.GetBytes(argStrFieldValue);
            string strRes = "";
            string strResult = "";
            HiLifeBasicFieldStruct objFileStruct = null;

            //取得該欄位格式資訊
            objFileStruct = this.GetFieldStruct(argFormat, argStrFieldName);
            if (objFileStruct == null)
                return argStrFieldValue;

            strResult = argStrFieldValue;
            //先檢查是否有固定值
            if (objFileStruct.HasStaticValue)
                strResult = objFileStruct.StaticValue;

            //根據設定的字元長度開始做字串補齊或截取
            if (byteString.Length > objFileStruct.Length)
            {
                //若字串過長, 則進行截取
                strRes = objEncoding.GetString(byteString, 0, objFileStruct.Length);
                if (!objEncoding.GetString(byteString).StartsWith(strRes))
                {
                    //中文字截斷的判斷
                    strRes = objEncoding.GetString(byteString, 0, objFileStruct.Length - 1);
                }

                strResult = strRes;
            }
            else if (byteString.Length < objFileStruct.Length)
            {
                //若字串過短, 則依照設定進行補齊
                if (objFileStruct.Align == Align.Left || objFileStruct.Align == Align.None)
                {
                    strResult = strResult.PadRight(objFileStruct.Length, objFileStruct.Symbol);
                }
                else
                {
                    strResult = strResult.PadLeft(objFileStruct.Length, objFileStruct.Symbol);
                }
            }

            return strResult;
        }

        /// <summary>
        /// 根據傳入檔案路徑Map欄位名稱及長度、對齊方式及補齊符號
        /// </summary>
        /// <param name="argStrFilePath">檔案路徑</param>
        /// <param name="argFileType">檔案形態</param>
        /// <returns>正規格式的字串</returns>
        protected Dictionary<string, List<HiLifeBasicFieldStruct>> MapHiLifeField(string argStrFilePath, HiLifeFileType argFileType)
        {
            if (argStrFilePath == null || argStrFilePath.Length <= 0)
                return null;

            XmlDocument objXmlDoc = null;
            XmlNode objNodeFile = null;

            objXmlDoc = new XmlDocument();
            objXmlDoc.Load(argStrFilePath);
            if (objXmlDoc != null)
            {

                //根據檔案型態來決定節點
                switch (argFileType)
                {
                    case HiLifeFileType.F00:
                        objNodeFile = objXmlDoc.SelectSingleNode(".//File[@Type='F00']");
                        break;
                    case HiLifeFileType.F01:
                        objNodeFile = objXmlDoc.SelectSingleNode(".//File[@Type='F01']");
                        break;
                    case HiLifeFileType.F02:
                        objNodeFile = objXmlDoc.SelectSingleNode(".//File[@Type='F02']");
                        break;
                    case HiLifeFileType.F04:
                        objNodeFile = objXmlDoc.SelectSingleNode(".//File[@Type='F04']");
                        break;
                    case HiLifeFileType.F05:
                        objNodeFile = objXmlDoc.SelectSingleNode(".//File[@Type='F05']");
                        break;
                    case HiLifeFileType.F06:
                        objNodeFile = objXmlDoc.SelectSingleNode(".//File[@Type='F06']");
                        break;
                    case HiLifeFileType.F08:
                        objNodeFile = objXmlDoc.SelectSingleNode(".//File[@Type='F08']");
                        break;
                    case HiLifeFileType.F09:
                        objNodeFile = objXmlDoc.SelectSingleNode(".//File[@Type='F09']");
                        break;
                    case HiLifeFileType.TXT:
                        objNodeFile = objXmlDoc.SelectSingleNode(".//File[@Type='TXT']");
                        break;
                }//end switch

                if (objNodeFile != null)
                {
                    this.m_objDictField = new Dictionary<string, List<HiLifeBasicFieldStruct>>();

                    //Head
                    this.SetDictField(objNodeFile, "Head/Field", HiLifeFormat.Head);

                    //Body
                    this.SetDictField(objNodeFile, "Body/Field", HiLifeFormat.Body);

                    //Tail
                    this.SetDictField(objNodeFile, "Tail/Field", HiLifeFormat.Tail);

                }
            }

            return this.m_objDictField;
        }

        /// <summary>
        /// 設定基底Field的資料
        /// </summary>
        /// <param name="argNodeParent">欲尋找的Note節點</param>
        /// <param name="argStrXpath">xml search string</param>
        /// <param name="argFormat">HiLifeFormat</param>
        private void SetDictField(XmlNode argNodeParent, string argStrXpath, HiLifeFormat argFormat)
        {
            XmlNodeList objNodeListTemp = null;
            List<HiLifeBasicFieldStruct> listField = null;
            HiLifeBasicFieldStruct objField = null;

            //Head
            objNodeListTemp = argNodeParent.SelectNodes(argStrXpath);
            if (objNodeListTemp != null && objNodeListTemp.Count > 0)
            {
                //根據節點建立資訊
                listField = new List<HiLifeBasicFieldStruct>();
                foreach (XmlNode objSubNode in objNodeListTemp)
                {
                    objField = new HiLifeBasicFieldStruct();
                    objField.Name = objSubNode.Attributes["Name"].Value;
                    objField.Length = Convert.ToInt32(objSubNode.Attributes["Length"].Value);
                    if (objSubNode.Attributes["PadSymbol"].Value.Length == 1)
                        objField.Symbol = Convert.ToChar(objSubNode.Attributes["PadSymbol"].Value);
                    else if (objSubNode.Attributes["PadSymbol"].Value.Length == 0)
                        objField.Symbol = ' ';
                    else
                        objField.Symbol = Convert.ToChar(objSubNode.Attributes["PadSymbol"].Value.Substring(0, 1));

                    switch (objSubNode.Attributes["Align"].Value.ToUpper())
                    {
                        case "LEFT":
                            objField.Align = Align.Left;
                            break;
                        case "RIGHT":
                            objField.Align = Align.Right;
                            break;
                        default:
                            objField.Align = Align.None;
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
                    listField.Add(objField);
                }//end foreach
                this.m_objDictField.Add(argFormat.ToString(), listField);
            }
        }

        
        /// <summary>
        /// 根據HiLifeFormat及Field Name傳回該Field的資料
        /// </summary>
        /// <param name="argFormat">HiLifeFormat</param>
        /// <param name="argStrFieldName">Field Name</param>
        /// <returns>null or HiLifeBasicFieldStruct</returns>
        private HiLifeBasicFieldStruct GetFieldStruct(HiLifeFormat argFormat, string argStrFieldName)
        {
            HiLifeBasicFieldStruct objFieldStruct = null;
            List<HiLifeBasicFieldStruct> objListFieldStruct = null;

            if (this.m_objDictField != null && this.m_objDictField.ContainsKey(HiLifeFormat.Head.ToString()))
            {
                objListFieldStruct = this.m_objDictField[HiLifeFormat.Head.ToString()];
                if (objListFieldStruct != null)
                    objFieldStruct = objListFieldStruct.Where(x => x.Name == argStrFieldName).FirstOrDefault();
            }

            return objFieldStruct;
        }
    }
}
