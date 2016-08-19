using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TWNewEgg.CacheGenerateServices.Service
{
    public class GenerateCategory
    {
        public Dictionary<string, string> XMLGenerate()
        {
            Dictionary<string, string> status = new Dictionary<string, string>();
            TWNewEgg.CategoryService.Service.GlobeCategory GlobeXML = new TWNewEgg.CategoryService.Service.GlobeCategory();
            try
            {
                GlobeXML.XMLGenerate();
                status.Add("Global", "Success");
            }
            catch (Exception e)
            {
                status.Add("Global", "Failed");
            }
            try
            {
                //BrandXML.BrandGenerate();
                status.Add("Brand", "None");
            }
            catch (Exception e)
            {
                status.Add("Brand", "Failed");
            }
            try
            {
                //CountryXML.CountryGenerate();
                status.Add("Country", "None");
            }
            catch (Exception e)
            {
                status.Add("Country", "Failed");
            }
            try
            {
                //SellerXML.SellerGenerate();
                status.Add("Seller", "None");
            }
            catch (Exception e)
            {
                status.Add("Seller", "Failed");
            }

            return status;
        }
    }
}
