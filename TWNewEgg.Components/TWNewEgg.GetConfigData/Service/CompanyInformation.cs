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
    public class CompanyInformation
    {
        public VENDORList GetCompanyInformationList(string FileNmne)
        {
            VENDORList VENDORList = new VENDORList();
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(VENDORList));

            string path = AppDomain.CurrentDomain.BaseDirectory + "Configurations\\" + FileNmne;
            //StreamReader readStream = new StreamReader(path);
            GetFileDataString GetFileDataString = new GetFileDataString();
            string xdata = GetFileDataString.GetFileDatainString(path);
            VENDORList = (VENDORList)xmlSerializer.Deserialize(new StringReader(xdata));
            //readStream.Close();
            return VENDORList;
        }

        public VENDORListVENDORDATA GetCompanyInformation(string TYPE,string FileNmne)
        {
            if (FileNmne == "" || FileNmne == null)
            {
                FileNmne = "CompanyInformation.config";
            }

            VENDORList VENDORList = GetCompanyInformationList(FileNmne);
            VENDORListVENDORDATA VENDORDATA = new VENDORListVENDORDATA();
            VENDORDATA = VENDORList.VENDORDATA.Where(x => x.TYPE == TYPE).FirstOrDefault();

            return VENDORDATA;
        }
    }
}