using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Net.Security;
using System.Web;
using System.Web.Script.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using System.Security.Cryptography.X509Certificates;
using TWNewEgg.GetConfigData.Models;

namespace TWNewEgg.GetConfigData.Service
{
    public class WebSiteInformation
    {
        public WebSiteList GetWebSiteList(string FileNmne)
        {
            WebSiteList WebSiteList = new WebSiteList();
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(WebSiteList));

            string path = AppDomain.CurrentDomain.BaseDirectory + "Configurations\\" + FileNmne;
            //StreamReader readStream = new StreamReader(path);
            GetFileDataString GetFileDataString = new GetFileDataString();
            string xdata = GetFileDataString.GetFileDatainString(path);
            WebSiteList = (WebSiteList)xmlSerializer.Deserialize(new StringReader(xdata));
            //readStream.Close();
            return WebSiteList;
        }

        public WebSiteListWebSiteData GetWebSiteData(int ID, string FileNmne)
        {
            if (FileNmne == "" || FileNmne == null)
            {
                FileNmne = "WebSiteInformation.config";
            }

            WebSiteList WebSiteList = GetWebSiteList(FileNmne);
            WebSiteListWebSiteData WebSiteData = new WebSiteListWebSiteData();
            WebSiteData = WebSiteList.WebSiteData.Where(x => x.ID == ID).FirstOrDefault();

            return WebSiteData;
        }
    }
}
