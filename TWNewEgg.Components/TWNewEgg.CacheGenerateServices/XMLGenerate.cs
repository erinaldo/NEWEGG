using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TWNewEgg.Framework.Autofac;
using TWNewEgg.CacheGenerateServices.Interface;

namespace TWNewEgg.CacheGenerateServices
{
    public class XMLGenerate : IXMLGenerate
    {
        public string ChooseFunction(string actionType, string jsonValue)
        {
            string results = string.Empty;
            Service.GenerateItem itemService = null;
            int numPerCount = 10000;
            switch (actionType.ToLower())
            {
                case "category":
                    Service.GenerateCategory categoryService = new Service.GenerateCategory();
                    results = TWNewEgg.Framework.Common.JSONSerialization.Serializer(categoryService.XMLGenerate());
                    break;
                case "findpricexml":
                    itemService = new Service.GenerateItem();
                    results = TWNewEgg.Framework.Common.JSONSerialization.Serializer(itemService.XMLGenerate("findprice"));
                    break;
                case "csvtoftp":
                    if (!String.IsNullOrEmpty(jsonValue))
                    {
                        Int32.TryParse(jsonValue, out numPerCount);
                    }
                    itemService = new Service.GenerateItem();
                    results = itemService.CSVGenerate(numPerCount);
                    break;
                case "csvtoftpday":
                    if (!String.IsNullOrEmpty(jsonValue))
                    {
                        Int32.TryParse(jsonValue, out numPerCount);
                    }
                    itemService = new Service.GenerateItem();
                    results = itemService.CSVGenerateDaily(jsonValue);
                    break;
                default:
                    results = string.Empty;
                    break;
            }
            return results;
        }
    }
}
