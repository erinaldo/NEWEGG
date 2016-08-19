using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TWNewEgg.NewEggUSGateway
{
    public static class NeweggConfiguration
    {
        public static string WWW { get; set; }
        public static string SSL { get; set; }

        static NeweggConfiguration()
        {
            WWW = GetHostWWW();
            SSL = GetHostSSL();
        }

        private static string GetHostWWW()
        {
            string url = "";
            try
            {
                System.Xml.XmlDocument doc = new System.Xml.XmlDocument();
                doc.Load(AppDomain.CurrentDomain.BaseDirectory + System.Configuration.ConfigurationManager.AppSettings["RestServiceConfigFile"]);
                for (int i = 0; i < doc.GetElementsByTagName("HostURL").Count; i++)
                {
                    if (doc.GetElementsByTagName("HostURL")[i].ParentNode.ParentNode.OuterXml.IndexOf("id=\"LocalServiceHostWWW\"") >= 0)
                    {
                        url = doc.GetElementsByTagName("HostURL")[i].ParentNode.InnerText;
                    }
                }
            }
            catch { }

            return url;
        }

        private static string GetHostSSL()
        {
            string url = "";
            try
            {
                System.Xml.XmlDocument doc = new System.Xml.XmlDocument();
                doc.Load(AppDomain.CurrentDomain.BaseDirectory + System.Configuration.ConfigurationManager.AppSettings["RestServiceConfigFile"]);
                for (int i = 0; i < doc.GetElementsByTagName("HostURL").Count; i++)
                {
                    if (doc.GetElementsByTagName("HostURL")[i].ParentNode.ParentNode.OuterXml.IndexOf("id=\"LocalServiceHostSSL\"") >= 0)
                    {
                        url = doc.GetElementsByTagName("HostURL")[i].ParentNode.InnerText;
                    }
                }
            }
            catch { }

            return url;
        }
    }
}
