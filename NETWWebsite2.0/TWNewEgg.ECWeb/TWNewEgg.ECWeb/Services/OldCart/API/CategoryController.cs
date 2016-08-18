using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Configuration;
using TWNewEgg.Website.ECWeb.Models;
using TWNewEgg.DB.TWSQLDB.Models;
using TWNewEgg.DB.TWBACKENDDB.Models;
using TWNewEgg.Website.ECWeb.Service;
using TWNewEgg.CategoryService.Service;
using TWNewEgg.CategoryService.Models;
using TWNewEgg.DB;


namespace TWNewEgg.Website.ECWeb.Controllers.Api
{
    public class CategoryController : ApiController
    {
        private BaseService baseController = new BaseService();
        private string m_strServerImagePath = ConfigurationManager.AppSettings.Get("Images");
        private string m_strServerImageSSLPath = ConfigurationManager.AppSettings.Get("SSLImages");
        // GET api/category
        //public IEnumerable<Category_TreeItem> GetCategory(int? CategoryID, int? LayerID, int? CountryID, int? BrandID, int? SellerID)
        //Get category form XML, if it's don't need some variable then leave it empty. (ex api/GetCategory?CategoryID=1&LayerID=1?CountryID=&BrandID=&SellerID=) 
        public Dictionary<string, List<Category_TreeItem>> GetCategory(int? CategoryID, int? LayerID, int? CountryID, int? BrandID, int? SellerID, bool? isSecure = null)
        {
            List<Category_TreeItem> Home_Menu = new List<Category_TreeItem>();
            CheckSSL sslCheck = new CheckSSL();
            //string PathName = "\\XML\\";
            //BaseController baseController = new BaseController();
            string PathName;
            string CategoryPic = "";
            int Layer;
            bool CheckLayerFlag = Int32.TryParse(LayerID.ToString(), out Layer);
            if (isSecure == null)
            {
                isSecure = sslCheck.CheckUrlSSL();
            }
            if (isSecure.Value)
            {
                CategoryPic += this.m_strServerImageSSLPath;
            }
            else
            {
                CategoryPic += this.m_strServerImagePath;
            }
            //int Category;
            //bool CheckCategoryFlag = Int32.TryParse(CategoryID.Value.ToString(), out Category);
            if (CheckLayerFlag == true && CategoryID != null)
            {
                if (CountryID == null && BrandID == null && SellerID == null) //GLOBE
                {
                    if (Layer == 0 && CategoryID.Value == 0)
                    {
                        PathName = "\\XML\\" + "Category_Menu.XML";
                        CategoryPic += "/images/0/Category0_Layer0/";
                    }
                    else
                    {
                        PathName = "\\XML\\" + baseController.GetLayerName(Layer) + "\\id_" + CategoryID.Value + ".XML";
                        CategoryPic += "/images/0/Category" + CategoryID.ToString() + "_Layer" + (Layer + 1).ToString() + "/";
                    }
                    Home_Menu = baseController.XML2Tree(PathName);
                }
                else if (CountryID != null && BrandID == null && SellerID == null) //Have COUNTRY ID and Don't have BRAND ID
                {
                    if (Layer == 0 && CategoryID.Value == 0)
                    {
                        PathName = "\\XML_Country\\id_" + CountryID.Value + "\\Category_Menu.XML";
                        CategoryPic += "/images/" + CountryID.ToString() + "/Category0_Layer0/";
                    }
                    else
                    {
                        PathName = "\\XML_Country\\id_" + CountryID.Value + "\\" + baseController.GetLayerName(Layer) + "\\id_" + CategoryID.Value + ".XML";
                        CategoryPic += "/images/" + CountryID.ToString() + "/Category" + CategoryID.ToString() + "_Layer" + (Layer + 1).ToString() + "/";
                    }
                    Home_Menu = baseController.XML2Tree(PathName);
                }
                else if (CountryID != null && BrandID != null && SellerID == null) //Have COUNTRY ID and BRAND ID
                {
                    if (Layer == 0 && CategoryID.Value == 0)
                    {
                        PathName = "\\XML_Country\\id_" + CountryID.Value + "\\XML_BRAND\\id_" + BrandID.Value + "\\Category_Menu.XML";
                        CategoryPic += "/images/Brand/" + BrandID.ToString() + "/Category0_Layer0/";
                        //CategoryPic += "/images/0/Category0_Layer0/";
                    }
                    else
                    {
                        PathName = "\\XML_Country\\id_" + CountryID.Value + "\\XML_BRAND\\id_" + BrandID.Value + "\\" + baseController.GetLayerName(Layer) + "\\id_" + CategoryID.Value + ".XML";
                        CategoryPic += "/images/Brand/" + BrandID.ToString() + "/Category" + CategoryID.ToString() + "_Layer" + (Layer + 1).ToString() + "/";
                        //CategoryPic += "/images/0/Category" + CategoryID.ToString() + "_Layer" + (Layer + 1).ToString() + "/";
                    }
                    Home_Menu = baseController.XML2Tree(PathName);
                }
                else if (CountryID == null && BrandID != null && SellerID == null) //Have Brand ID
                {
                    if (Layer == 0 && CategoryID.Value == 0)
                    {
                        PathName = "\\XML_BRAND\\id_" + BrandID.Value + "\\Category_Menu.XML";
                        CategoryPic += "/images/Brand/" + BrandID.ToString() + "/Category0_Layer0/";
                        //CategoryPic += "/images/0/Category0_Layer0/";
                    }
                    else
                    {
                        PathName = "\\XML_BRAND\\id_" + BrandID.Value + "\\" + baseController.GetLayerName(Layer) + "\\id_" + CategoryID.Value + ".XML";
                        CategoryPic += "/images/Brand/" + BrandID.ToString() + "/Category" + CategoryID.ToString() + "_Layer" + (Layer + 1).ToString() + "/";
                        //CategoryPic += "/images/0/Category" + CategoryID.ToString() + "_Layer" + (Layer + 1).ToString() + "/";
                    }
                    Home_Menu = baseController.XML2Tree(PathName);
                }
                else if (CountryID == null && BrandID == null && SellerID != null) //Have SELLER ID and Don't have BRAND ID
                {
                    if (Layer == 0 && CategoryID.Value == 0)
                    {
                        PathName = "\\XML_SELLER\\id_" + CountryID.Value + "\\Category_Menu.XML";
                        CategoryPic += "/images/Brand/" + BrandID.ToString() + "/Category0_Layer0/"; // Lack seller's Category Pic Info
                        //CategoryPic += "/images/0/Category0_Layer0/";

                    }
                    else
                    {
                        PathName = "\\XML_SELLER\\id_" + CountryID.Value + "\\" + baseController.GetLayerName(Layer) + "\\id_" + CategoryID.Value + ".XML";
                        CategoryPic += "/images/Brand/" + BrandID.ToString() + "/Category" + CategoryID.ToString() + "_Layer" + (Layer + 1).ToString() + "/"; // Lack seller's Category Pic Info
                        //CategoryPic += "/images/0/Category" + CategoryID.ToString() + "_Layer" + (Layer + 1).ToString() + "/";
                    }
                    Home_Menu = baseController.XML2Tree(PathName);
                }
                else if (CountryID == null && BrandID != null && SellerID != null) //Have SELLER ID and BRAND ID
                {
                    if (Layer == 0 && CategoryID.Value == 0)
                    {
                        PathName = "\\XML_SELLER\\id_" + CountryID.Value + "\\XML_BRAND\\id_" + BrandID.Value + "\\Category_Menu.XML";
                        CategoryPic += "\\images\\Brand\\" + BrandID.ToString() + "\\Category0_Layer0\\";
                        //CategoryPic += "/images/0/Category0_Layer0/";
                    }
                    else
                    {
                        PathName = "\\XML_SELLER\\id_" + CountryID.Value + "\\XML_BRAND\\id_" + BrandID.Value + "\\" + baseController.GetLayerName(Layer) + "\\id_" + CategoryID.Value + ".XML";
                        CategoryPic += "/images/Brand/" + BrandID.ToString() + "/Category" + CategoryID.ToString() + "_Layer" + (Layer + 1).ToString() + "/";
                        //CategoryPic += "/images/0/Category" + CategoryID.ToString() + "_Layer" + (Layer + 1).ToString() + "/";
                    }
                    Home_Menu = baseController.XML2Tree(PathName);
                }
                //XML2Tree("Category.XML");//Start it in Global.asax and using Session_Start Function






                Dictionary<string, List<Category_TreeItem>> CategoryResult = new Dictionary<string, List<Category_TreeItem>>();

                CategoryResult.Add(CategoryPic, Home_Menu);


                //return View(Home_Menu[0].Nodes);
                if (Home_Menu.Count > 0)
                    return CategoryResult;
                //return Home_Menu;
                else
                    return null;
            }
            else
            {
                return null;
            }
        }
        //Get category's parents form XML, if it's don't need some variable then leave it empty. (ex api/GetParents?CategoryID=1&LayerID=1?CountryID=&BrandID=&SellerID=)
        public IEnumerable<MapPath> GetParents(int? CategoryID, int? LayerID, int? CountryID, int? BrandID, int? SellerID)
        {
            List<MapPath> MapPath = new List<MapPath>();
            //string PathName = "\\XML\\";
            //BaseController baseController = new BaseController();
            string PathName;
            int Layer;
            bool CheckLayerFlag = Int32.TryParse(LayerID.ToString(), out Layer);
            //int Category;
            //bool CheckCategoryFlag = Int32.TryParse(CategoryID.Value.ToString(), out Category);
            if (CheckLayerFlag == true && CategoryID != null)
            {
                if (CountryID == null && BrandID == null && SellerID == null) //GLOBE
                {
                    if (Layer == 0 && CategoryID.Value == 0)
                    {
                        PathName = "\\XML\\" + "Category_Menu_Parents.XML";
                    }
                    else
                    {
                        PathName = "\\XML\\" + baseController.GetLayerName(Layer) + "\\id_" + CategoryID.Value + "_Parents.XML";
                    }
                    baseController.SiteMap(MapPath, PathName);
                }
                else if (CountryID != null && BrandID == null && SellerID == null) //Have COUNTRY ID and Don't have BRAND ID
                {
                    if (Layer == 0 && CategoryID.Value == 0)
                    {
                        PathName = "\\XML_Country\\id_" + CountryID.Value + "\\Category_Menu_Parents.XML";
                    }
                    else
                    {
                        PathName = "\\XML_Country\\id_" + CountryID.Value + "\\" + baseController.GetLayerName(Layer) + "\\id_" + CategoryID.Value + "_Parents.XML";
                    }
                    baseController.SiteMap(MapPath, PathName);
                }
                else if (CountryID != null && BrandID != null && SellerID == null) //Have COUNTRY ID and BRAND ID
                {
                    if (Layer == 0 && CategoryID.Value == 0)
                    {
                        PathName = "\\XML_Country\\id_" + CountryID.Value + "\\XML_BRAND\\id_" + BrandID.Value + "\\Category_Menu_Parents.XML";
                    }
                    else
                    {
                        PathName = "\\XML_Country\\id_" + CountryID.Value + "\\XML_BRAND\\id_" + BrandID.Value + "\\" + baseController.GetLayerName(Layer) + "\\id_" + CategoryID.Value + "_Parents.XML";
                    }
                    baseController.SiteMap(MapPath, PathName);
                }
                else if (CountryID == null && BrandID != null && SellerID == null) //Have Brand ID
                {
                    if (Layer == 0 && CategoryID.Value == 0)
                    {
                        PathName = "\\XML_BRAND\\id_" + BrandID.Value + "\\Category_Menu_Parents.XML";
                    }
                    else
                    {
                        PathName = "\\XML_BRAND\\id_" + BrandID.Value + "\\" + baseController.GetLayerName(Layer) + "\\id_" + CategoryID.Value + "_Parents.XML";
                    }
                    baseController.SiteMap(MapPath, PathName);
                }
                else if (CountryID == null && BrandID == null && SellerID != null) //Have SELLER ID and Don't have BRAND ID
                {
                    if (Layer == 0 && CategoryID.Value == 0)
                    {
                        PathName = "\\XML_SELLER\\id_" + SellerID.Value + "\\Category_Menu_Parents.XML";
                    }
                    else
                    {
                        PathName = "\\XML_SELLER\\id_" + SellerID.Value + "\\" + baseController.GetLayerName(Layer) + "\\id_" + CategoryID.Value + "_Parents.XML";
                    }
                    baseController.SiteMap(MapPath, PathName);
                }
                else if (CountryID == null && BrandID != null && SellerID != null) //Have SELLER ID and BRAND ID
                {
                    if (Layer == 0 && CategoryID.Value == 0)
                    {
                        PathName = "\\XML_SELLER\\id_" + SellerID.Value + "\\XML_BRAND\\id_" + BrandID.Value + "\\Category_Menu_Parents.XML";
                    }
                    else
                    {
                        PathName = "\\XML_SELLER\\id_" + SellerID.Value + "\\XML_BRAND\\id_" + BrandID.Value + "\\" + baseController.GetLayerName(Layer) + "\\id_" + CategoryID.Value + "_Parents.XML";
                    }
                    baseController.SiteMap(MapPath, PathName);
                }
                //XML2Tree("Category.XML");//Start it in Global.asax and using Session_Start Function










                //return View(Home_Menu[0].Nodes);
                if (MapPath.Count > 0)
                {
                    return MapPath;
                }
                else
                {
                    MapPath.Add(new MapPath { category_id = "0", category_layer = "0", category_title = "None", category_showorder = 999, category_description = "None" });
                    return MapPath;
                }
            }
            else
            {
                MapPath.Add(new MapPath { category_id = "0", category_layer = "0", category_title = "None", category_showorder = 999, category_description = "None" });
                return MapPath;
            }
        }
        //Get category's brands form XML, if it's don't need some variable then leave it empty. (ex api/GetBrands?CategoryID=1&LayerID=1?CountryID=&BrandID=&SellerID=)
        public IEnumerable<BrandInfo> GetBrands(int? CategoryID, int? LayerID, int? CountryID, int? BrandID, int? SellerID)
        {
            List<BrandInfo> AllBrand = new List<BrandInfo>();
            //string PathName = "\\XML\\";
            //BaseController baseController = new BaseController();
            string PathName;
            int Layer;
            bool CheckLayerFlag = Int32.TryParse(LayerID.ToString(), out Layer);
            CheckSSL sslCheck = new CheckSSL();
            var isSecure = sslCheck.CheckUrlSSL();
            //var isSecure = Request.RequestUri.Scheme == Uri.UriSchemeHttps;
            //int Category;
            //bool CheckCategoryFlag = Int32.TryParse(CategoryID.Value.ToString(), out Category);
            if (CheckLayerFlag == true && CategoryID != null)
            {
                if (CountryID == null && BrandID == null && SellerID == null) //GLOBE
                {
                    if (Layer == 0 && CategoryID.Value == 0)
                    {
                        PathName = "\\XML\\" + "Category_Menu_Brands.XML";
                    }
                    else
                    {
                        PathName = "\\XML\\" + baseController.GetLayerName(Layer) + "\\id_" + CategoryID.Value + "_Brands.XML";
                    }
                    baseController.CategoryBrands(AllBrand, PathName, isSecure);
                }
                else if (CountryID != null && BrandID == null && SellerID == null) //Have COUNTRY ID and Don't have BRAND ID
                {
                    if (Layer == 0 && CategoryID.Value == 0)
                    {
                        PathName = "\\XML_Country\\id_" + CountryID.Value + "\\Category_Menu_Brands.XML";
                    }
                    else
                    {
                        PathName = "\\XML_Country\\id_" + CountryID.Value + "\\" + baseController.GetLayerName(Layer) + "\\id_" + CategoryID.Value + "_Brands.XML";
                    }
                    baseController.CategoryBrands(AllBrand, PathName, isSecure);
                }
                else if (CountryID != null && BrandID != null && SellerID == null) //Have COUNTRY ID and BRAND ID
                {
                    if (Layer == 0 && CategoryID.Value == 0)
                    {
                        PathName = "\\XML_Country\\id_" + CountryID.Value + "\\XML_BRAND\\id_" + BrandID.Value + "\\Category_Menu_Brands.XML";
                    }
                    else
                    {
                        PathName = "\\XML_Country\\id_" + CountryID.Value + "\\XML_BRAND\\id_" + BrandID.Value + "\\" + baseController.GetLayerName(Layer) + "\\id_" + CategoryID.Value + "_Brands.XML";
                    }
                    baseController.CategoryBrands(AllBrand, PathName, isSecure);
                }
                else if (CountryID == null && BrandID != null && SellerID == null) //Have Brand ID
                {
                    if (Layer == 0 && CategoryID.Value == 0)
                    {
                        PathName = "\\XML_BRAND\\id_" + BrandID.Value + "\\Category_Menu_Brands.XML";
                    }
                    else
                    {
                        PathName = "\\XML_BRAND\\id_" + BrandID.Value + "\\" + baseController.GetLayerName(Layer) + "\\id_" + CategoryID.Value + "_Brands.XML";
                    }
                    baseController.CategoryBrands(AllBrand, PathName, isSecure);
                }
                else if (CountryID == null && BrandID == null && SellerID != null) //Have SELLER ID and Don't have BRAND ID
                {
                    if (Layer == 0 && CategoryID.Value == 0)
                    {
                        PathName = "\\XML_SELLER\\id_" + SellerID.Value + "\\Category_Menu_Brands.XML";
                    }
                    else
                    {
                        PathName = "\\XML_SELLER\\id_" + SellerID.Value + "\\" + baseController.GetLayerName(Layer) + "\\id_" + CategoryID.Value + "_Brands.XML";
                    }
                    baseController.CategoryBrands(AllBrand, PathName, isSecure);
                }
                else if (CountryID == null && BrandID != null && SellerID != null) //Have SELLER ID and BRAND ID
                {
                    if (Layer == 0 && CategoryID.Value == 0)
                    {
                        PathName = "\\XML_SELLER\\id_" + SellerID.Value + "\\XML_BRAND\\id_" + BrandID.Value + "\\Category_Menu_Brands.XML";
                    }
                    else
                    {
                        PathName = "\\XML_SELLER\\id_" + SellerID.Value + "\\XML_BRAND\\id_" + BrandID.Value + "\\" + baseController.GetLayerName(Layer) + "\\id_" + CategoryID.Value + "_Brands.XML";
                    }
                    baseController.CategoryBrands(AllBrand, PathName, isSecure);
                }
                //XML2Tree("Category.XML");//Start it in Global.asax and using Session_Start Function








                //return View(Home_Menu[0].Nodes);
                if (AllBrand.Count > 0)
                {
                    return AllBrand;
                }
                else
                {
                    AllBrand.Add(new BrandInfo { category_brands = "None", category_brands_pic = "" });
                    return AllBrand;
                }
            }
            else
            {
                AllBrand.Add(new BrandInfo { category_brands = "None", category_brands_pic = "" });
                return AllBrand;
            }
        }


        //Generate category xml from DB
        [HttpGet]
        public Dictionary<string, string> XMLGenerate(int? password)
        {
            var classA = ((System.Web.HttpContextWrapper)Request.Properties["MS_HttpContext"]).Request.UserHostAddress.Split('.')[0].ToString();
            Dictionary<string, string> status = new Dictionary<string, string>();
            if (password != null)
            {
                //if (password.Value.ToString() == "123" && (classA == "::1" || classA == "172" || classA == "10" || classA == "192"))
                if (password.Value.ToString() == "123" && (classA == "::1" || classA == "172" || classA == "10" || classA == "192")) //this is only for ipv4
                {
                    GlobeCategory GlobeXML = new GlobeCategory();
                    BrandCategory BrandXML = new BrandCategory();
                    CountryCategory CountryXML = new CountryCategory();
                    SellerCategory SellerXML = new SellerCategory();


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
                        BrandXML.BrandGenerate();
                        status.Add("Brand", "Success");
                    }
                    catch (Exception e)
                    {
                        status.Add("Brand", "Failed");
                    }
                    try
                    {
                        CountryXML.CountryGenerate();
                        status.Add("Country", "Success");
                    }
                    catch (Exception e)
                    {
                        status.Add("Country", "Failed");
                    }
                    try
                    {
                        SellerXML.SellerGenerate();
                        status.Add("Seller", "Success");
                    }
                    catch (Exception e)
                    {
                        status.Add("Seller", "Failed");
                    }

                    return status;
                }
                else
                {
                    status.Add("NewEgg", "NewEgg");
                    return status;
                }
            }
            else
            {
                status.Add("NewEgg", "NewEgg");
                return status;
            }
        }

        [HttpGet]
        public Manufacture getManufactureById(int BrandID)
        {
            if (BrandID == null)
                return null;
            Manufacture oResult = null;
            TWSqlDBContext oDb = null;

            oDb = new TWSqlDBContext();
            oResult = (from x in oDb.Manufacture
                       where x.ID == BrandID
                       select x).FirstOrDefault();

            oDb.Dispose();
            oDb = null;
            return oResult;
        }//end getManufactureById

        /// <summary>
        /// 
        /// </summary>
        /// <param name="CategoryID"></param>
        /// <param name="Layer"></param>
        /// <param name="CountryID"></param>
        /// <param name="BrandID"></param>
        /// <param name="SellerID"></param>
        /// <returns></returns>
        [HttpGet]
        public List<PropertyGroup> GetCategoryItemProperties(int CategoryID, int Layer, int CountryID, int BrandID, int SellerID)
        {
            List<PropertyGroup> responseMessage = new List<PropertyGroup>();
            FindProductProperty propertyService = new FindProductProperty();
            Service.ItemService itemService = new Service.ItemService();
            if (CategoryID == 0 && Layer == 0)
            {
                return null;
            }
            var allProductIDs = itemService.FindProductIDsByCategoryID(CategoryID, Layer, CountryID, BrandID, SellerID);
            if (allProductIDs.Count > 0)
            {
                responseMessage = propertyService.FindAllGroupPropertyByProductIDs(allProductIDs);
            }

            return responseMessage;
        }

        [HttpGet]
        public List<PropertyGroup> GetCategoryItemProperties(int CategoryID)
        {
            List<PropertyGroup> responseMessage = new List<PropertyGroup>();
            FindProductProperty propertyService = new FindProductProperty();

            if (CategoryID == 0)
            {
                return null;
            }

            responseMessage = propertyService.FindAllGroupPropertyByCategoryID(CategoryID, new string[1] { "F" });
            return responseMessage;
        }
    }
}
