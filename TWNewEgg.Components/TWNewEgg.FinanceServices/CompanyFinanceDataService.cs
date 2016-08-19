using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TWNewEgg.FinanceServices.Interface;
using TWNewEgg.Models.DBModels.TWBACKENDDB;
using TWNewEgg.Models.DBModels.TWBACKENDDBExtModels;
using TWNewEgg.Models.DomainModels.Message;
using TWNewEgg.Models.DomainModels.Finance;
using TWNewEgg.FinanceRepoAdapters.Interface;
using System.Reflection;
using System.Xml;
using System.Xml.Serialization;
using System.IO;
using System.Web;

namespace TWNewEgg.FinanceServices
{
    public class CompanyFinanceDataService : ICompanyFinanceDataService
    {
        log4net.ILog _logger = log4net.LogManager.GetLogger(LoggerInfo.FinanceLog);

        public FinanceDataList GetAll()
        {
            FinanceDataList finanList = new FinanceDataList();
            StreamReader readStream = null;

            try
            {
                //讀取 Configurations/FinanceList.xml
                XmlSerializer xmlSerializer = new XmlSerializer(typeof(FinanceDataList));
                Assembly asm = Assembly.GetExecutingAssembly();

                string configPath = Path.Combine(HttpContext.Current.Server.MapPath("/"), System.Configuration.ConfigurationManager.AppSettings["FinanceConfig"]);

                readStream = new StreamReader(configPath);
                string xdata = readStream.ReadToEnd();
                finanList = (FinanceDataList)xmlSerializer.Deserialize(new StringReader(xdata));

                return finanList;
            }
            finally
            {
                if (readStream != null)
                    readStream.Close();
            }      
        }

        public FinanceDataListFinanceData Get(DateTime nowDate)
        {
            FinanceDataList finanList = this.GetAll();

            return Get(nowDate, finanList.FinanceData);
        }

        public FinanceDataListFinanceData Get(DateTime nowDate, List<FinanceDataListFinanceData> finanList)
        {
            try
            {
                //取得符合條件的資料
                IEnumerable<FinanceDataListFinanceData> finanDataList = finanList.Where(x => x.StartDate <= nowDate);

                if (finanDataList.Count() == 0) return null;

                //回傳日期最接近nowDate的那筆資料
                return finanDataList.OrderByDescending(x => x.StartDate).First();
            }
            catch (Exception ex)
            {
                return null;
            }
        }
    }
}
