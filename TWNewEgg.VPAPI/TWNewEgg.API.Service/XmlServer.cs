using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace TWNewEgg.API.Service
{
    public class XmlServer
    {
        log4net.ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.FullName);
        public Models.ActionResponse<string> readXML()
        {
            Models.ActionResponse<string> result = new Models.ActionResponse<string>();
            logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.FullName);

            string _strXMLPath = System.Web.HttpContext.Current.Server.MapPath(@"../FileTempLocation/XML/Messageboard.xml");
            //string xmlContent = string.Empty;
            string xmlUpdateUserContent = string.Empty;
            string xmlMessageContent = string.Empty;
            string UpdatedNumber = string.Empty;
            if (System.IO.File.Exists(_strXMLPath) == false)
            {
                result.IsSuccess = false;
                result.Msg = "資料不存在";
                return result;
            }
            try
            {
                string xmlContentTemp = string.Empty;
                using (System.IO.StreamReader sr = new System.IO.StreamReader(_strXMLPath))
                {
                    xmlContentTemp = sr.ReadToEnd();
                    sr.Close();
                }
                int contentStart = xmlContentTemp.IndexOf("<Content>") + 18;
                int contentEnd = xmlContentTemp.IndexOf("</Content>");
                int contentLength = contentEnd - contentStart;
                xmlMessageContent = xmlContentTemp.Substring(contentStart, contentLength - 3);

                int UpdateUserStart = xmlContentTemp.IndexOf("<Updated>") + 9;
                int UpdateUserEnd = xmlContentTemp.IndexOf("</Updated>");
                int UpdateUserLength = UpdateUserEnd - UpdateUserStart;
                xmlUpdateUserContent = xmlContentTemp.Substring(UpdateUserStart, UpdateUserLength);

                //System.Xml.Linq.XDocument xdoc = System.Xml.Linq.XDocument.Load(_strXMLPath);
                //XCData cdata = xdoc.DescendantNodes().OfType<XCData>().Where(p => p.Parent.Name == "Content").ToList()[0];
                //xmlContent = cdata.Value;
                //var content = from p in xdoc.DescendantNodes()
                //              where p.NodeType == System.Xml.XmlNodeType.CDATA
                //              select p.Parent.Value.Trim();
                //var Updated = xdoc.Descendants().Where(n => n.Name == "Updated").FirstOrDefault();

                //UpdatedNumber = Updated.Value.ToString();
                UpdatedNumber = xmlUpdateUserContent;
                result.IsSuccess = true;
            }
            catch (Exception error)
            {
                logger.Error("/XmlServer/readXML error: " + error.Message);
                result.IsSuccess = false;
                result.Msg = "資料錯誤";
            }
            if (result.IsSuccess == false)
            {
                return result;
            }
            result.Body = xmlMessageContent;
            result.IsSuccess = true;
            result.Msg = UpdatedNumber;//借用Msg來回傳更新次數
            return result;
        }
        public Models.ActionResponse<string> writeXml(string xmlContent, int userid, int updateNumber)
        {
            logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.FullName);
            Models.ActionResponse<string> result = new Models.ActionResponse<string>();
            string _strXMLPath = System.Web.HttpContext.Current.Server.MapPath(@"../FileTempLocation/XML/Messageboard.xml");
            DateTime dateTimeMillisecond = new DateTime(0001, 01, 01, 01, 01, 01, 01);
            string timeString = dateTimeMillisecond.ToString("yyyy-MM-dd hh:mm:ss fff");
            dateTimeMillisecond = DateTime.Now;
            timeString = dateTimeMillisecond.ToString("yyyyMMddHHmmss");

            //file is not exist
            if (System.IO.File.Exists(_strXMLPath) == false)
            {
                result.IsSuccess = false;
                result.Msg = "檔案不存在";
                return result;
            }
            string _strSavePath = System.Web.HttpContext.Current.Server.MapPath(@"../FileTempLocation/XML/XmlFileTemp");
            string _xmlTempSave = System.Web.HttpContext.Current.Server.MapPath(@"../FileTempLocation/XML/XmlFileTemp/XmlTeml_" + timeString + ".xml");

            if (System.IO.Directory.Exists(_strSavePath) == false)
            {
                System.IO.Directory.CreateDirectory(_strSavePath);
            }
            try
            {

                string contenTemp = string.Empty;
                string updateNumberToCheck = string.Empty;
                using (System.IO.StreamReader sr = new System.IO.StreamReader(_strXMLPath))
                {
                    contenTemp = sr.ReadToEnd();
                    sr.Close();
                }

                int UpdateUserStart = contenTemp.IndexOf("<Updated>") + 9;
                int UpdateUserEnd = contenTemp.IndexOf("</Updated>");
                int UpdateUserLength = UpdateUserEnd - UpdateUserStart;
                updateNumberToCheck = contenTemp.Substring(UpdateUserStart, UpdateUserLength);
                int updateCheckNumber = Convert.ToInt32(updateNumberToCheck);
                if (updateCheckNumber != updateNumber)
                {
                    result.IsSuccess = false;
                    result.Msg = "已經有人使用，請先複製要修改的內容並重新整理";
                }
                string modufyContent = createXMLContent(xmlContent, userid, updateNumber);
                using (System.IO.StreamWriter sw = new System.IO.StreamWriter(_strXMLPath))
                {
                    sw.WriteLine(modufyContent);
                    sw.Close();
                }
                using (System.IO.StreamWriter sw = new System.IO.StreamWriter(_xmlTempSave))
                {
                    sw.WriteLine(modufyContent);
                    sw.Close();
                }
                result.IsSuccess = true;
                result.Msg = "修改成功";
                #region 避免檔案是唯獨不能寫入造成錯誤
                //string _strFileAttribute = System.Web.HttpContext.Current.Server.MapPath(@"../FileTempLocation/XML/Messageboard.xml");
                //logger.Info("XML PATH is: " + _strFileAttribute);
                //System.IO.FileInfo FileAttribute = new System.IO.FileInfo(_strFileAttribute);
                //FileAttribute.Attributes = System.IO.FileAttributes.Normal;
                #endregion
                #region Linq to XML
                //System.Xml.Linq.XDocument xdoc = System.Xml.Linq.XDocument.Load(_strXMLPath);
                //XElement dataNode = xdoc.Descendants("Content").First();
                //XElement root = new XElement("Message_board");
                //var Updated = xdoc.Descendants().Where(n => n.Name == "Updated").FirstOrDefault();
                //var updateuserid = xdoc.Descendants().Where(n => n.Name == "UpdateUser").FirstOrDefault();
                //var updateDate = xdoc.Descendants().Where(n => n.Name == "UpdateDate").FirstOrDefault();
                //int updatenumber = Convert.ToInt32(Updated.Value.Trim());

                //if (updateNumber != updatenumber)
                //{
                //    result.IsSuccess = false;
                //    result.Msg = "已經有人使用，請先複製要修改的內容並重新整理";
                //}
                //else
                //{
                //    //updatenumber = updatenumber + 1;
                //    //DateTime datenow = new DateTime();
                //    //datenow = DateTime.Now;

                //    //dataNode.ReplaceAll(new XCData(xmlContent));
                //    //updateuserid.ReplaceAll(userid.ToString());
                //    //Updated.ReplaceAll(updatenumber.ToString());
                //    //updateDate.ReplaceAll(datenow.ToString());
                //    //xdoc.Save(_strXMLPath);
                //    //xdoc.Save(_xmlTempSave);
                //    result.IsSuccess = true;
                //    result.Msg = "修改成功";
                //}
                #endregion
            }
            catch (Exception error)
            {
                logger.Error("/XmlServer/writeXml error:" + error.Message);
                result.IsSuccess = false;
                result.Msg = "資料錯誤";
            }
            return result;
        }

        public string createXMLContent(string xmlContent, int userid, int updateNumber)
        {
            string result = string.Empty;
            string _strTitle = "<?xml version=\"1.0\" encoding=\"utf-8\"?>";
            DateTime dateNow = new DateTime();
            dateNow = DateTime.Now;
            string dateString = dateNow.ToString();
            updateNumber++;
            result = _strTitle + "<Message_board>" + "<Content><![CDATA[" + xmlContent + "]]></Content><UpdateUser>" + userid + "</UpdateUser><UpdateDate>" + dateString + "</UpdateDate><Updated>" + updateNumber + "</Updated></Message_board>";
            return result;
        }
        #region
        //public Models.ActionResponse<string> readXML()
        //{
        //    Models.ActionResponse<string> result = new Models.ActionResponse<string>();
        //    logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.FullName);

        //    string _strXMLPath = System.Web.HttpContext.Current.Server.MapPath(@"../FileTempLocation/XML/Messageboard.xml");
        //    logger.Info("讀取XML檔案路徑: " + _strXMLPath);
        //    string xmlContent = string.Empty;
        //    string UpdatedNumber = string.Empty;
        //    if (System.IO.File.Exists(_strXMLPath) == false)
        //    {
        //        result.IsSuccess = false;
        //        result.Msg = "資料不存在";
        //        return result;
        //    }
        //    try
        //    {
        //        System.Xml.Linq.XDocument xdoc = System.Xml.Linq.XDocument.Load(_strXMLPath);
        //        XCData cdata = xdoc.DescendantNodes().OfType<XCData>().Where(p => p.Parent.Name == "Content").ToList()[0];
        //        xmlContent = cdata.Value;
        //        //var content = from p in xdoc.DescendantNodes()
        //        //              where p.NodeType == System.Xml.XmlNodeType.CDATA
        //        //              select p.Parent.Value.Trim();
        //        var Updated = xdoc.Descendants().Where(n => n.Name == "Updated").FirstOrDefault();
        //        UpdatedNumber = Updated.Value.ToString();
        //        result.IsSuccess = true;
        //    }
        //    catch (Exception error)
        //    {
        //        logger.Error("/XmlServer/readXML error: " + error.Message);
        //        result.IsSuccess = false;
        //        result.Msg = "資料錯誤";
        //    }
        //    if (result.IsSuccess == false)
        //    {
        //        return result;
        //    }
        //    result.Body = xmlContent;
        //    result.IsSuccess = true;
        //    result.Msg = UpdatedNumber;//借用Msg來回傳更新次數
        //    return result;
        //}
        //public Models.ActionResponse<string> writeXml(string xmlContent, int userid, int updateNumber)
        //{
        //    logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.FullName);
        //    Models.ActionResponse<string> result = new Models.ActionResponse<string>();
        //    string _strXMLPath = System.Web.HttpContext.Current.Server.MapPath(@"../FileTempLocation/XML/Messageboard.xml");
        //    DateTime dateTimeMillisecond = new DateTime(0001, 01, 01, 01, 01, 01, 01);
        //    string timeString = dateTimeMillisecond.ToString("yyyy-MM-dd hh:mm:ss fff");
        //    dateTimeMillisecond = DateTime.Now;
        //    timeString = dateTimeMillisecond.ToString("yyyyMMddHHmmss");
            
        //    //file is not exist
        //    if (System.IO.File.Exists(_strXMLPath) == false)
        //    {
        //        result.IsSuccess = false;
        //        result.Msg = "檔案不存在";
        //        return result;
        //    }
        //    string _strSavePath = System.Web.HttpContext.Current.Server.MapPath(@"../FileTempLocation/XML/XmlFileTemp");
        //    string _xmlTempSave = System.Web.HttpContext.Current.Server.MapPath(@"../FileTempLocation/XML/XmlFileTemp/XmlTeml_" + timeString + ".xml");
            
        //    if (System.IO.Directory.Exists(_strSavePath) == false)
        //    {
        //        logger.Info("System.IO.Directory.Exists(_strSavePath): " + _strSavePath);
        //        System.IO.Directory.CreateDirectory(_strSavePath);
        //    }
        //    try
        //    {
        //        #region 避免檔案是唯獨不能寫入造成錯誤
        //        string _strFileAttribute = System.Web.HttpContext.Current.Server.MapPath(@"../FileTempLocation/XML/Messageboard.xml");
        //        logger.Info("XML PATH is: " + _strXMLPath);
        //        //System.IO.FileInfo FileAttribute = new System.IO.FileInfo(_strFileAttribute);
        //        logger.Info("FileAttribute後");
        //        //FileAttribute.Attributes = System.IO.FileAttributes.Normal;
        //        logger.Info("System.IO.FileAttributes.Normal;後");
        //        #endregion
        //        logger.Info("xdoc 讀取:前");
        //        System.Xml.Linq.XDocument xdoc = System.Xml.Linq.XDocument.Load(_strXMLPath);
        //        logger.Info("xdoc 讀取:後");
        //        XElement dataNode = xdoc.Descendants("Content").First();
        //        XElement root = new XElement("Message_board");
        //        var Updated = xdoc.Descendants().Where(n => n.Name == "Updated").FirstOrDefault();
        //        var updateuserid = xdoc.Descendants().Where(n => n.Name == "UpdateUser").FirstOrDefault();
        //        var updateDate = xdoc.Descendants().Where(n => n.Name == "UpdateDate").FirstOrDefault();
        //        int updatenumber = Convert.ToInt32(Updated.Value.Trim());
        //        logger.Info("xdoc 讀取:後2");
        //        if (updateNumber != updatenumber)
        //        {
        //            result.IsSuccess = false;
        //            result.Msg = "已經有人使用，請先複製要修改的內容並重新整理";
        //        }
        //        else
        //        {
        //            updatenumber = updatenumber + 1;
        //            DateTime datenow = new DateTime();
        //            datenow = DateTime.Now;

        //            dataNode.ReplaceAll(new XCData(xmlContent));
        //            updateuserid.ReplaceAll(userid.ToString());
        //            Updated.ReplaceAll(updatenumber.ToString());
        //            updateDate.ReplaceAll(datenow.ToString());
        //            logger.Info("_strXMLPath存路徑: " + _strXMLPath);

        //            //xdoc.Save(_strXMLPath);
        //            logger.Info("_xmlTempSave存路徑: " + _xmlTempSave);
        //            xdoc.Save(_xmlTempSave);
        //            result.IsSuccess = true;
        //            result.Msg = "修改成功";
        //        }
        //    }
        //    catch (Exception error)
        //    {
        //        logger.Error("/XmlServer/writeXml error:" + error.Message);
        //        result.IsSuccess = false;
        //        result.Msg = "資料錯誤";
        //    }
        //    return result;
        //}
        #endregion
    }
    
}
