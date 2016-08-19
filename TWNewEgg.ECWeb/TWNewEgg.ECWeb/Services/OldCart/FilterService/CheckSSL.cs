using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using TWNewEgg.Website.ECWeb.Service;
using TWNewEgg.Website.ECWeb.Models;

namespace TWNewEgg.Website.ECWeb.Service
{
    public class CheckSSL
    {
        public bool CheckUrlSSL()
        {
            log4net.ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.FullName);

            bool flag = false;
            string www = System.Configuration.ConfigurationManager.AppSettings["ECDomain"];
            www = www.ToLower();
            int wwwPort = int.Parse(System.Configuration.ConfigurationManager.AppSettings["ECDomainPort"]);
            string ssl = System.Configuration.ConfigurationManager.AppSettings["ECSSLDomain"];
            ssl = ssl.ToLower();
            int sslPort = int.Parse(System.Configuration.ConfigurationManager.AppSettings["ECSSLDomainPort"]);

            /*logger.Info("Scheme:" + HttpContext.Current.Request.Url.Scheme.ToString());
            logger.Info("URL:" + HttpContext.Current.Request.Url.ToString().ToLower());
            logger.Info("Host:" + HttpContext.Current.Request.Url.Host);
            logger.Info("ToLower():" + HttpContext.Current.Request.Url.Host.ToString().ToLower());
            logger.Info("SSL:" + ssl);
            logger.Info("Port:" + HttpContext.Current.Request.Url.Port);
            logger.Info("SSL_PORT:" + sslPort);*/
            //因 IT 設定，會有以下的現象
            /* 
                INFO  2014/01/05 22:02:15 CheckSSL             CheckUrlSSL                 【Scheme:http】

                INFO  2014/01/05 22:02:15 CheckSSL             CheckUrlSSL                 【URL:http://secure.newegg.com.tw/home/index】

                INFO  2014/01/05 22:02:15 CheckSSL             CheckUrlSSL                 【Host:secure.newegg.com.tw】

                INFO  2014/01/05 22:02:15 CheckSSL             CheckUrlSSL                 【ToLower():secure.newegg.com.tw】

                INFO  2014/01/05 22:02:15 CheckSSL             CheckUrlSSL                 【SSL:secure.newegg.com.tw】

                INFO  2014/01/05 22:02:15 CheckSSL             CheckUrlSSL                 【Port:80】

                INFO  2014/01/05 22:02:15 CheckSSL             CheckUrlSSL                 【SSL_PORT:443】
            */
            if (wwwPort != 0)
            {
                if (HttpContext.Current.Request.Url.Host.ToString().ToLower().IndexOf(ssl) >= 0)
                {
                    flag = true;
                }
            }
            else
            {
                if (HttpContext.Current.Request.Url.Port == sslPort)
                {
                    flag = true;
                }
            }
            return flag;
        }
    }
}