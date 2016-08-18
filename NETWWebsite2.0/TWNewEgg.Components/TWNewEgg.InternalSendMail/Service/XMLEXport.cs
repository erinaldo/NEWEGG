using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.Net.Security;
using System.Web;
using System.Threading.Tasks;
using System.Xml.Serialization;
using TWNewEgg.InternalSendMail.Model.SendMailModel;

namespace TWNewEgg.InternalSendMail.Service
{
    public class XMLEXport
    {
        public MailList GenerateXsdFromXml()
        {
            //XmlSerializer xmlSerializer = new XmlSerializer(typeof(MailList));
            //MailList mailList = new MailList();
            //mailList.MailGroup = new MailGroup();
            //mailList.MailGroup.MailGroupList = new List<MailGroupList>();
            //var mg1 = new MailGroupList();
            //mg1.MailGroup="G1";
            //mg1.Name="N1";
            //var mg2 = new MailGroupList();
            //mg2.MailGroup="G2";
            //mg2.Name="N2";
            //mailList.MailGroup.MailGroupList.Add(mg1);
            //mailList.MailGroup.MailGroupList.Add(mg2);
            //mailList.Retgood = new Retgood();
            //mailList.Retgood.GroupList = new List<GroupList>();
            //var gl1 = new GroupList();
            //gl1.DelivType = "1";
            //gl1.MailGroup = new List<string>();
            //gl1.MailGroup.Add("mg1");
            //var gl2 = new GroupList();
            //gl2.DelivType = "2";
            //gl2.MailGroup = new List<string>();
            //gl2.MailGroup.Add("mg2");
            //mailList.Retgood.GroupList.Add(gl1);
            //mailList.Retgood.GroupList.Add(gl2);

            //TextWriter tw = new StreamWriter("D:\\xml01.txt");
            //xmlSerializer.Serialize(tw,mailList);

            MailList Mail = new MailList();
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(MailList));

            string path = AppDomain.CurrentDomain.BaseDirectory + "Configurations\\Maillist.xml";
            StreamReader readStream = new StreamReader(path);
            string xdata = readStream.ReadToEnd();
            Mail = (MailList)xmlSerializer.Deserialize(new StringReader(xdata));
            readStream.Close();
            return Mail;

        }
    }


}
