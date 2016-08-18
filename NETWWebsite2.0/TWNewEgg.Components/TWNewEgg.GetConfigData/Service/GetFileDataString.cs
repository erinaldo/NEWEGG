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
using TWNewEgg.GetConfigData.Models.CompanyInformation;

namespace TWNewEgg.GetConfigData.Service
{
    public class GetFileDataString
    {
        public string GetFileDatainString(string path)
        {
            StreamReader readStream = new StreamReader(path);
            string xdata = readStream.ReadToEnd();
            readStream.Close();
            return xdata;
        }
    }
}