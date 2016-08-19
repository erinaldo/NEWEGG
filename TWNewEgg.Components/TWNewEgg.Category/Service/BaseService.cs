using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Threading.Tasks;
using System.Xml;
using System.IO;
using TWNewEgg.DB;
using TWNewEgg.DB.TWSQLDB.Models;
using System.Configuration;
using TWNewEgg.CategoryService.Models;
//using TWNewEgg.DB.TWBACKENDDB.Models;

namespace TWNewEgg.CategoryService.Service
{
    public class BaseService : IDisposable
    {
        //
        // GET: /Base/



        protected TWSqlDBContext db = new TWSqlDBContext();
        protected TWBackendDBContext mObjBackDb = new TWBackendDBContext();
        private IQueryable<TWNewEgg.DB.TWSQLDB.Models.Category> allCategorirsFromDB = null;
        private IQueryable<TWNewEgg.DB.TWBACKENDDB.Models.CategoryAssociatedWithPM> mListCategoryPm = null;
        protected int Total_Layer = new int();
        private string m_strServerImagePath = ConfigurationManager.AppSettings.Get("Images");
        private string m_strServerImageSSLPath = ConfigurationManager.AppSettings.Get("SSLImages");

        public void Dispose()
        {
            if (db != null)
            {
                db.Dispose();
                db = null;
            }

            if (mObjBackDb != null)
            {
                mObjBackDb.Dispose();
                mObjBackDb = null;
            }
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (db != null)
                {
                    db.Dispose();
                    db = null;
                }
                if (mObjBackDb != null)
                {
                    mObjBackDb.Dispose();
                    mObjBackDb = null;
                }
            }
        }

        public BaseService()
        {
            this.allCategorirsFromDB = db.Category;
            this.mListCategoryPm = this.mObjBackDb.CategoryAssociatedWithPM;
        }

        public void SiteMap(List<MapPath> MapPath, string FileName) //Find the Node and all of it's parents.
        {

            XmlDocument xmlDoc = new XmlDocument();
            try
            {
                if (!File.Exists(AppDomain.CurrentDomain.BaseDirectory + FileName))
                {
                    return;
                }
                xmlDoc.Load(AppDomain.CurrentDomain.BaseDirectory + FileName); //Find and load  Xml File
                if (xmlDoc.ChildNodes.Count == 0)//make no children under this node
                {
                    return;
                }
                else
                {
                    foreach (XmlNode node in xmlDoc.ChildNodes) //Search node children from XmlNode
                    {
                        if (node.ChildNodes.Count > 0)
                        {
                            if (node.NodeType == XmlNodeType.Element) //Set node's attribute in to List<Category_TreeItem>
                            {
                                foreach (var item in node.ChildNodes)
                                {
                                    try
                                    {
                                        XmlElement xe = item as XmlElement;
                                        MapPath Parent = new MapPath();
                                        string a = xe.GetAttribute("category_id");
                                        Parent.category_id = xe.GetAttribute("category_id");
                                        Parent.category_title = HttpUtility.UrlDecode(xe.GetAttribute("category_title")); //URL Decode !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!! may change in the future
                                        //Parent.category_layer = LayerName(Byte.Parse(xe.GetAttribute("category_layer")));
                                        Parent.category_layer = xe.GetAttribute("category_layer");
                                        Parent.category_showorder = ((xe.GetAttribute("category_showorder") != "") ? int.Parse(xe.GetAttribute("category_showorder")) : 500);
                                        Parent.category_description = HttpUtility.UrlDecode(xe.GetAttribute("category_description"));//URL Decode !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!! may change in the future
                                        MapPath.Add(Parent); //Add into List<Category_TreeItem>
                                    }
                                    catch (Exception ex)
                                    {
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                return;
            }
        }
        public void CategoryBrands(List<BrandInfo> AllBrand, string FileName, bool? SSLStatus = null) //Find the Node and all of it's parents.
        {
            string urlPath = "";
            if (SSLStatus != null && SSLStatus.Value == true)
            {
                urlPath = this.m_strServerImageSSLPath;
            }
            else if (SSLStatus != null && SSLStatus.Value == false)
            {
                urlPath = this.m_strServerImagePath;
            }
            XmlDocument xmlDoc = new XmlDocument();
            try
            {
                xmlDoc.Load(AppDomain.CurrentDomain.BaseDirectory + FileName); //Find and load  Xml File
                if (xmlDoc.ChildNodes.Count == 0)//make no children under this node
                {
                    return;
                }
                else
                {
                    foreach (XmlNode node in xmlDoc.ChildNodes) //Search node children from XmlNode
                    {
                        if (node.ChildNodes.Count > 0)
                        {
                            if (node.NodeType == XmlNodeType.Element) //Set node's attribute in to List<Category_TreeItem>
                            {
                                foreach (var item in node.ChildNodes)
                                {
                                    try
                                    {
                                        XmlElement xe = item as XmlElement;
                                        BrandInfo Parent = new BrandInfo();

                                        Parent.category_brands = xe.GetAttribute("category_brands");
                                        //Parent.category_brands_pic = manufactureImage(int.Parse(Parent.category_brands), "_1_60");
                                        //轉換Image網址為http的路徑, Updated from Lynn at 2013/08/12
                                        //Parent.category_brands_pic = urlPath + manufactureImage(int.Parse(Parent.category_brands), "_1_60").Replace("..", "").Replace("\\", "/");
                                        Parent.category_brands_pic = urlPath + manufactureImage(int.Parse(Parent.category_brands), "_1_60");
                                        //Parent.category_brands_pic = this.m_strServerImagePath;
                                        AllBrand.Add(Parent); //Add into List<Category_TreeItem>
                                    }
                                    catch (Exception ex)
                                    {
                                    }
                                }
                            }
                        }
                    }
                }
                /*
                foreach (var aBrand in AllBrand)
                {
                    StringBuilder urls = new StringBuilder();
                    var brandID = int.Parse(aBrand.category_brands);
                    //aBrand.category_brands_pic = this.m_strServerImagePath + manufactureImage(int.Parse(aBrand.category_brands), "_1_60");
                    urls.AppendFormat("{0}{1}{2}{3}{4}", this.m_strServerImagePath, "/pic/manufacture/", (brandID / 1000).ToString("0000") + "/", (brandID % 1000).ToString("0000") + "_1_60", ".jpg");
                    aBrand.category_brands_pic = urls.ToString();
                }*/
            }
            catch (Exception e)
            {
                return;
            }
        }
        public string manufactureImage(int id, string file1Picnum) //Copy From Steven's Class , I didn't use it since 2013/09/11!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
        {
            //Manufacture result = db.Manufacture.Find(id);
            //var manuImage = result.manufacture_id.ToString("00000000");
            var manuImage = id.ToString("00000000");
            //string manufacturepath = "..\\pic\\manufacture\\" + manuImage.Substring(0, 4) + "\\";
            string manufacturepath = "/pic/manufacture/" + manuImage.Substring(0, 4) + "/";
            string name = manuImage.Substring(4, 4) + file1Picnum + ".jpg";
            string savename = manufacturepath + name;
            //return new FilePathResult(savename, "image/jpeg");
            return savename;
        }
        public string GetLayerName(int Layer)
        {
            return Category_type(Layer);
        }
        private string LayerName(int Layer) //Set the item's Action Name in SiteMap_no using since 2103/12/17_Bill Wu
        {
            switch (Layer)
            {
                case 0:
                    return "Tab";
                case 1:
                    return "Category";
                case 2:
                    return "Subcategory";
                case 3:
                    return "Products";
                case 4:
                    return "Layer5";
                case 5:
                    return "Layer6";
                case 6:
                    return "Layer7";
                case 7:
                    return "Layer8";
                case 8:
                    return "Layer9";
                case 9:
                    return "Layer10";
                default:
                    return "index";
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
                        PathName = "\\XML\\" + GetLayerName(Layer) + "\\id_" + CategoryID.Value + "_Parents.XML";
                    }
                    SiteMap(MapPath, PathName);
                }
                else if (CountryID != null && BrandID == null && SellerID == null) //Have COUNTRY ID and Don't have BRAND ID
                {
                    if (Layer == 0 && CategoryID.Value == 0)
                    {
                        PathName = "\\XML_Country\\id_" + CountryID.Value + "\\Category_Menu_Parents.XML";
                    }
                    else
                    {
                        PathName = "\\XML_Country\\id_" + CountryID.Value + "\\" + GetLayerName(Layer) + "\\id_" + CategoryID.Value + "_Parents.XML";
                    }
                    SiteMap(MapPath, PathName);
                }
                else if (CountryID != null && BrandID != null && SellerID == null) //Have COUNTRY ID and BRAND ID
                {
                    if (Layer == 0 && CategoryID.Value == 0)
                    {
                        PathName = "\\XML_Country\\id_" + CountryID.Value + "\\XML_BRAND\\id_" + BrandID.Value + "\\Category_Menu_Parents.XML";
                    }
                    else
                    {
                        PathName = "\\XML_Country\\id_" + CountryID.Value + "\\XML_BRAND\\id_" + BrandID.Value + "\\" + GetLayerName(Layer) + "\\id_" + CategoryID.Value + "_Parents.XML";
                    }
                    SiteMap(MapPath, PathName);
                }
                else if (CountryID == null && BrandID != null && SellerID == null) //Have Brand ID
                {
                    if (Layer == 0 && CategoryID.Value == 0)
                    {
                        PathName = "\\XML_BRAND\\id_" + BrandID.Value + "\\Category_Menu_Parents.XML";
                    }
                    else
                    {
                        PathName = "\\XML_BRAND\\id_" + BrandID.Value + "\\" + GetLayerName(Layer) + "\\id_" + CategoryID.Value + "_Parents.XML";
                    }
                    SiteMap(MapPath, PathName);
                }
                else if (CountryID == null && BrandID == null && SellerID != null) //Have SELLER ID and Don't have BRAND ID
                {
                    if (Layer == 0 && CategoryID.Value == 0)
                    {
                        PathName = "\\XML_SELLER\\id_" + SellerID.Value + "\\Category_Menu_Parents.XML";
                    }
                    else
                    {
                        PathName = "\\XML_SELLER\\id_" + SellerID.Value + "\\" + GetLayerName(Layer) + "\\id_" + CategoryID.Value + "_Parents.XML";
                    }
                    SiteMap(MapPath, PathName);
                }
                else if (CountryID == null && BrandID != null && SellerID != null) //Have SELLER ID and BRAND ID
                {
                    if (Layer == 0 && CategoryID.Value == 0)
                    {
                        PathName = "\\XML_SELLER\\id_" + SellerID.Value + "\\XML_BRAND\\id_" + BrandID.Value + "\\Category_Menu_Parents.XML";
                    }
                    else
                    {
                        PathName = "\\XML_SELLER\\id_" + SellerID.Value + "\\XML_BRAND\\id_" + BrandID.Value + "\\" + GetLayerName(Layer) + "\\id_" + CategoryID.Value + "_Parents.XML";
                    }
                    SiteMap(MapPath, PathName);
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
        public Dictionary<string, List<Category_TreeItem>> GetCategory(int? CategoryID, int? LayerID, int? CountryID, int? BrandID, int? SellerID, bool? isSecure = null)
        {
            List<Category_TreeItem> Home_Menu = new List<Category_TreeItem>();
            //CheckSSL sslCheck = new CheckSSL();
            //string PathName = "\\XML\\";
            //BaseController baseController = new BaseController();
            string PathName;
            string CategoryPic = "";
            int Layer;
            bool CheckLayerFlag = Int32.TryParse(LayerID.ToString(), out Layer);
            if (isSecure == null)
            {
                //isSecure = sslCheck.CheckUrlSSL();
                isSecure = false;
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
                        PathName = "\\XML\\" + GetLayerName(Layer) + "\\id_" + CategoryID.Value + ".XML";
                        CategoryPic += "/images/0/Category" + CategoryID.ToString() + "_Layer" + (Layer + 1).ToString() + "/";
                    }
                    Home_Menu = XML2Tree(PathName);
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
                        PathName = "\\XML_Country\\id_" + CountryID.Value + "\\" + GetLayerName(Layer) + "\\id_" + CategoryID.Value + ".XML";
                        CategoryPic += "/images/" + CountryID.ToString() + "/Category" + CategoryID.ToString() + "_Layer" + (Layer + 1).ToString() + "/";
                    }
                    Home_Menu = XML2Tree(PathName);
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
                        PathName = "\\XML_Country\\id_" + CountryID.Value + "\\XML_BRAND\\id_" + BrandID.Value + "\\" + GetLayerName(Layer) + "\\id_" + CategoryID.Value + ".XML";
                        CategoryPic += "/images/Brand/" + BrandID.ToString() + "/Category" + CategoryID.ToString() + "_Layer" + (Layer + 1).ToString() + "/";
                        //CategoryPic += "/images/0/Category" + CategoryID.ToString() + "_Layer" + (Layer + 1).ToString() + "/";
                    }
                    Home_Menu = XML2Tree(PathName);
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
                        PathName = "\\XML_BRAND\\id_" + BrandID.Value + "\\" + GetLayerName(Layer) + "\\id_" + CategoryID.Value + ".XML";
                        CategoryPic += "/images/Brand/" + BrandID.ToString() + "/Category" + CategoryID.ToString() + "_Layer" + (Layer + 1).ToString() + "/";
                        //CategoryPic += "/images/0/Category" + CategoryID.ToString() + "_Layer" + (Layer + 1).ToString() + "/";
                    }
                    Home_Menu = XML2Tree(PathName);
                }
                else if (CountryID == null && BrandID == null && SellerID != null) //Have SELLER ID and Don't have BRAND ID
                {
                    if (Layer == 0 && CategoryID.Value == 0)
                    {
                        PathName = "\\XML_SELLER\\id_" + CountryID.Value + "\\Category_Menu.XML";
                        CategoryPic += "/images/Brand/" + BrandID.ToString() + "/Category0_Layer0/"; // Lack Seller's Category Pic Info
                        //CategoryPic += "/images/0/Category0_Layer0/";

                    }
                    else
                    {
                        PathName = "\\XML_SELLER\\id_" + CountryID.Value + "\\" + GetLayerName(Layer) + "\\id_" + CategoryID.Value + ".XML";
                        CategoryPic += "/images/Brand/" + BrandID.ToString() + "/Category" + CategoryID.ToString() + "_Layer" + (Layer + 1).ToString() + "/"; // Lack Seller's Category Pic Info
                        //CategoryPic += "/images/0/Category" + CategoryID.ToString() + "_Layer" + (Layer + 1).ToString() + "/";
                    }
                    Home_Menu = XML2Tree(PathName);
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
                        PathName = "\\XML_SELLER\\id_" + CountryID.Value + "\\XML_BRAND\\id_" + BrandID.Value + "\\" + GetLayerName(Layer) + "\\id_" + CategoryID.Value + ".XML";
                        CategoryPic += "/images/Brand/" + BrandID.ToString() + "/Category" + CategoryID.ToString() + "_Layer" + (Layer + 1).ToString() + "/";
                        //CategoryPic += "/images/0/Category" + CategoryID.ToString() + "_Layer" + (Layer + 1).ToString() + "/";
                    }
                    Home_Menu = XML2Tree(PathName);
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
        public List<Category_TreeItem> XML2Tree(string FileName) //Start it in Global.asax and using Session_Start Function
        {
            List<Category_TreeItem> Root = new List<Category_TreeItem>();
            try
            {
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.Load(AppDomain.CurrentDomain.BaseDirectory + FileName); //Find and load  Xml File
                Root.Clear();
                //SearchXMLDoc(xmlDoc, Root);
                Category_TreeItem RootNode = new Category_TreeItem();
                RootNode.category_id = 0;
                RootNode.category_title = "Root";
                RootNode.category_layer = 0;
                RootNode.Parents = null;
                RootNode.Nodes = new List<Category_TreeItem>();
                //Root.Add(RootNode);
                if (xmlDoc.ChildNodes.Count == 0)
                {
                    return new List<Category_TreeItem>();
                }
                else
                {
                    //List<Category_TreeItem> newNodes = new List<Category_TreeItem>();
                    foreach (XmlNode node in xmlDoc.ChildNodes) //Start to find this XML file and set them into List<Category_TreeItem>
                    {
                        if (node.NodeType == XmlNodeType.Element) //Set node's attribute in to List<Category_TreeItem>
                        {
                            XmlElement xe = node as XmlElement;
                            Category_TreeItem newNode = new Category_TreeItem();
                            newNode.category_id = Int32.Parse(xe.GetAttribute("category_id"));
                            newNode.category_description = HttpUtility.UrlDecode(xe.GetAttribute("category_description"));//URL Decode !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!! may change in the future
                            newNode.category_title = HttpUtility.UrlDecode(xe.GetAttribute("category_title"));//URL Decode !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!! may change in the future
                            newNode.category_layer = Byte.Parse(xe.GetAttribute("category_layer"));
                            newNode.category_parentid = Int32.Parse(xe.GetAttribute("category_parentid"));
                            newNode.category_categoryfromwsid = Int32.Parse(xe.GetAttribute("category_categoryfromwsid"));
                            newNode.category_showorder = Int32.Parse(xe.GetAttribute("category_showorder"));
                            newNode.category_sellerid = Int32.Parse(xe.GetAttribute("category_sellerid"));
                            newNode.category_deviceid = Byte.Parse(xe.GetAttribute("category_deviceid"));
                            newNode.category_showall = Byte.Parse(xe.GetAttribute("category_showall"));
                            newNode.category_version = Byte.Parse(xe.GetAttribute("category_version"));
                            newNode.ClassName = Convert.ToString(xe.GetAttribute("ClassName"));
                            newNode.ImagePath = Convert.ToString(xe.GetAttribute("ImagePath"));
                            newNode.ImageHref = Convert.ToString(xe.GetAttribute("ImageHref"));

                            //SearchXMLDoc(node, newNodes);
                            newNode.Nodes = new List<Category_TreeItem>();
                            //////////SearchXMLDoc(node, newNode.Nodes, newNode); //Call self and fine who is this node childrens.
                            //newNodes.Nodes = new List<Category_TreeItem>();
                            Root.Add(newNode); //Add into List<Category_TreeItem>
                            //Node.Add(item);
                            //Category_TreeItem PassNode = newNodes.Find(delegate(Category_TreeItem p) {return p.category_id == newNode.category_id; });
                            //SearchXMLDoc(node , PassNode);


                            /*
                            foreach (var item in Nodes)
                            {
                                item.Nodes = new List<Category_TreeItem>();
                                SearchXMLDoc(node, item.Nodes);

                            }
                            */
                            SearchXMLDoc(node, Root[0].Nodes, Root[0]);
                        }

                    }
                    //item.Nodes = new List<Category_TreeItem>();

                    return Root;
                }
                //int a = 0;
                /*
                foreach (XmlNode node in xmlDoc.DocumentElement.ChildNodes)
                {
                    XmlElement xe = node as XmlElement;
                    Category_TreeItem newNode = new Category_TreeItem();
                    newNode.category_id = Int32.Parse(xe.GetAttribute("category_id"));
                    newNode.category_description = xe.GetAttribute("category_description");
                    newNode.category_title = xe.GetAttribute("category_title");
                    newNode.category_layer = Byte.Parse(xe.GetAttribute("category_layer"));
                    newNode.category_parentid = Int32.Parse(xe.GetAttribute("category_parentid"));
                    newNode.category_categoryfromwsid = Int32.Parse(xe.GetAttribute("category_categoryfromwsid"));
                    newNode.category_showorder = Int32.Parse(xe.GetAttribute("category_showorder"));
                    newNode.category_sellerid = Int32.Parse(xe.GetAttribute("category_sellerid"));
                    newNode.category_deviceid = Byte.Parse(xe.GetAttribute("category_deviceid"));
                    newNode.category_showall = Byte.Parse(xe.GetAttribute("category_showall"));
                    newNode.category_version = Byte.Parse(xe.GetAttribute("category_version"));
                    //調用遞歸函數
                    SearchXMLDoc(node, newNode);
                    //添加子節點
                    //Root.Nodes.Add(newNode);
                }
                */
            }
            catch (Exception ex)
            {
                Root.Clear();
                //SearchXMLDoc(xmlDoc, Root);
                Category_TreeItem RootNode = new Category_TreeItem();
                RootNode.category_id = 0;
                RootNode.category_title = "Root";
                RootNode.category_layer = 0;
                RootNode.Parents = null;
                RootNode.Nodes = new List<Category_TreeItem>();
                Root.Add(RootNode); //Add into List<Category_TreeItem>
                return Root;
                //MessageBox.Show(ex.Message);
            }

        }
        private void SearchXMLDoc(XmlNode xmlNode, List<Category_TreeItem> Nodes, Category_TreeItem Parents)
        {
            if (xmlNode.ChildNodes.Count == 0)//make no children under this node
            {

                return;
            }
            else
            {
                //List<Category_TreeItem> newNodes = new List<Category_TreeItem>();
                foreach (XmlNode node in xmlNode.ChildNodes) //Search node children from XmlNode
                {

                    try
                    {
                        if (node.NodeType == XmlNodeType.Element) //Set node's attribute in to List<Category_TreeItem>
                        {
                            XmlElement xe = node as XmlElement;
                            Category_TreeItem newNode = new Category_TreeItem();
                            newNode.category_id = Int32.Parse(xe.GetAttribute("category_id"));
                            newNode.category_description = HttpUtility.UrlDecode(xe.GetAttribute("category_description"));//URL Decode !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!! may change in the future
                            newNode.category_title = HttpUtility.UrlDecode(xe.GetAttribute("category_title"));//URL Decode !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!! may change in the future
                            newNode.category_layer = Byte.Parse(xe.GetAttribute("category_layer"));
                            newNode.category_parentid = Int32.Parse(xe.GetAttribute("category_parentid"));
                            newNode.category_categoryfromwsid = Int32.Parse(xe.GetAttribute("category_categoryfromwsid"));
                            newNode.category_showorder = Int32.Parse(xe.GetAttribute("category_showorder"));
                            newNode.category_sellerid = Int32.Parse(xe.GetAttribute("category_sellerid"));
                            newNode.category_deviceid = Byte.Parse(xe.GetAttribute("category_deviceid"));
                            newNode.category_showall = Byte.Parse(xe.GetAttribute("category_showall"));
                            newNode.category_version = Byte.Parse(xe.GetAttribute("category_version"));
                            newNode.ClassName = Convert.ToString(xe.GetAttribute("ClassName"));
                            newNode.ImagePath = Convert.ToString(xe.GetAttribute("ImagePath"));
                            newNode.ImageHref = Convert.ToString(xe.GetAttribute("ImageHref"));
                            //newNode.Parents = Parents; //This is very IMPORTANT, CAN'T BE DELETE!!!!!!!!This can build a complete tree.
                            //SearchXMLDoc(node, newNodes);
                            newNode.Nodes = new List<Category_TreeItem>();
                            SearchXMLDoc(node, newNode.Nodes, newNode); //Call self and fine who is this node childrens.
                            //newNodes.Nodes = new List<Category_TreeItem>();
                            Nodes.Add(newNode); //Add into List<Category_TreeItem>
                            //Node.Add(item);
                            //Category_TreeItem PassNode = newNodes.Find(delegate(Category_TreeItem p) {return p.category_id == newNode.category_id; });
                            //SearchXMLDoc(node , PassNode);


                            /*
                            foreach (var item in Nodes)
                            {
                                item.Nodes = new List<Category_TreeItem>();
                                SearchXMLDoc(node, item.Nodes);

                            }
                            */

                        }
                    }
                    catch (Exception e)
                    {
                        //Console.WriteLine(e.ToString());
                    }
                }


            }
        }













        protected void SQL2XML(List<Category_TreeItem> Tree, string PathName, int? specifyBrandID = null) //Set XML File
        {
            List<Category_TreeItem> LayerTree = new List<Category_TreeItem>();
            Layer2XML(Tree[0], PathName, true, specifyBrandID); //Save ALL DB DATA in a XML
            Layer2XML(Tree[0], PathName, false, specifyBrandID);
            for (int i = 0; i <= Total_Layer; i++)
            {
                LayerTree.Clear();
                FindLayerNode(Tree[0].Nodes, LayerTree, i);
                foreach (var item in LayerTree)
                    Layer2XML(item, PathName, false, specifyBrandID);
            }

        }
        private void Layer2XML(Category_TreeItem Tree, string PathName, bool ALL, int? specifyBrandID)
        {
            //string test;
            string FileName, ParentFileName, BrandFileName;
            XmlDocument doc = new XmlDocument(); //Set XML File
            XmlDocument doc_Parents = new XmlDocument(); //Set XML File
            XmlDocument doc_Brands = new XmlDocument(); //Set XML File
            doc.AppendChild(doc.CreateXmlDeclaration("1.0", "UTF-8", "yes")); //Set XML Header
            doc_Parents.AppendChild(doc_Parents.CreateXmlDeclaration("1.0", "UTF-8", "yes")); //Set XML Header
            doc_Brands.AppendChild(doc_Brands.CreateXmlDeclaration("1.0", "UTF-8", "yes")); //Set XML Header
            //////XmlNode node = doc.CreateNode(XmlNodeType.Element, "Root", "");
            //if(Tree.category_id == 18)
            //    test = "a";

            if (Tree.category_id != 0)
            {
                FileName = Category_type(Tree.category_layer) + "\\id_" + Tree.category_id.ToString() + ".XML";
                ParentFileName = Category_type(Tree.category_layer) + "\\id_" + Tree.category_id.ToString() + "_Parents.XML";
                BrandFileName = Category_type(Tree.category_layer) + "\\id_" + Tree.category_id.ToString() + "_Brands.XML";
            }
            else
            {
                if (!ALL)
                {
                    FileName = "Category_Menu.XML";
                    ParentFileName = "Category_Menu_Parents.XML";
                    BrandFileName = "Category_Menu_Brands.XML";
                }
                else
                {
                    FileName = "Category_Menu_ALL.XML";
                    ParentFileName = "Category_Menu_Parents_ALL.XML";
                    BrandFileName = "Category_Menu_Brands.XML";
                }
            }

            //if (Tree.category_layer == 3)
            //    test = "a";
            List<Category_TreeItem> ParentNodes = new List<Category_TreeItem>();
            FindParentNodes(Tree, ParentNodes, Tree.category_layer, 0);//Get this node's all parents

            List<int> Brands = new List<int>();
            if (specifyBrandID == null)
            {
                FindNodeBrand(Tree, Brands); //Get this node's all brands
            }
            else
            {
                FindNodeBrand(Tree, Brands, specifyBrandID.Value); //Get this node's brands
            }
            //node.AppendChild(FindParentNode(Tree, doc));
            XmlElement node = doc.CreateElement(Category_type(Tree.category_layer) + "_" + Tree.category_id.ToString()); //Set First Node
            node.SetAttribute("category_id", Tree.category_id.ToString());
            node.SetAttribute("category_description", Tree.category_description);
            node.SetAttribute("category_title", Tree.category_title);
            node.SetAttribute("category_layer", Tree.category_layer.ToString());
            node.SetAttribute("category_parentid", Tree.category_parentid.ToString());
            node.SetAttribute("category_categoryfromwsid", Tree.category_categoryfromwsid.ToString());
            node.SetAttribute("category_showorder", Tree.category_showorder.ToString());
            node.SetAttribute("category_sellerid", Tree.category_sellerid.ToString());
            node.SetAttribute("category_deviceid", Tree.category_deviceid.ToString());
            node.SetAttribute("category_showall", Tree.category_showall.ToString());
            node.SetAttribute("category_version", Tree.category_version.ToString());
            node.SetAttribute("ClassName", Tree.ClassName);
            node.SetAttribute("ImagePath", Tree.ImagePath);
            node.SetAttribute("ImageHref", Tree.ImageHref);
            XmlElement node_Parents = doc_Parents.CreateElement(Category_type(Tree.category_layer) + "_" + Tree.category_id + "_Parents"); //Set First Node
            node_Parents.SetAttribute("category_id", Tree.category_id.ToString()); //Set First Node
            //node.AppendChild();

            XmlElement node_Brands = doc_Brands.CreateElement(Category_type(Tree.category_layer) + "_" + Tree.category_id + "_Brands"); //Set First Node
            node_Brands.SetAttribute("category_id", Tree.category_id.ToString()); //Set First Node

            foreach (var item in Tree.Nodes)
            {
                node.AppendChild(setXmlNode(item, doc, false, ALL)); //Set each node in List<Category_TreeItem> into XML Temp file
            }


            foreach (var item in ParentNodes)
            {
                node_Parents.AppendChild(setXmlNode(item, doc_Parents, true, ALL)); //Set each node in List<Category_TreeItem> into XML Temp file
            }


            foreach (var item in Brands)
            {
                node_Brands.AppendChild(setXmlNode(item, doc_Brands, true, ALL));
            }


            doc.AppendChild(node); //Add all temp file into XmlDocument
            doc_Parents.AppendChild(node_Parents); //Add all temp file into XmlDocument
            doc_Brands.AppendChild(node_Brands); //Add all temp file into XmlDocument
            try
            {
                BrandFileName = PathName + BrandFileName;
                ParentFileName = PathName + ParentFileName;
                PathName = PathName + FileName;
                doc.Save(AppDomain.CurrentDomain.BaseDirectory + PathName); //Write XML File into Computer
                doc_Parents.Save(AppDomain.CurrentDomain.BaseDirectory + ParentFileName);
                doc_Brands.Save(AppDomain.CurrentDomain.BaseDirectory + BrandFileName);
            }
            catch (Exception)
            {
                return;
            }

        }
        private XmlNode setParentNode(Category_TreeItem Nodes, Category_TreeItem Tree, XmlDocument doc, int layer, bool ALL) //Test Function (Write Node's Father to XML)(No use)
        {
            XmlElement node = doc.CreateElement(Category_type(Nodes.category_layer));
            if (layer == 0)
            {
                //XmlElement node = doc.CreateElement(Category_type(Nodes.category_layer));

                node.SetAttribute("category_id", Nodes.category_id.ToString());
                node.SetAttribute("category_description", Nodes.category_description);
                node.SetAttribute("category_title", Nodes.category_title);
                node.SetAttribute("category_layer", Nodes.category_layer.ToString());
                node.SetAttribute("category_parentid", Nodes.category_parentid.ToString());
                node.SetAttribute("category_categoryfromwsid", Nodes.category_categoryfromwsid.ToString());
                node.SetAttribute("category_showorder", Nodes.category_showorder.ToString());
                node.SetAttribute("category_sellerid", Nodes.category_sellerid.ToString());
                node.SetAttribute("category_deviceid", Nodes.category_deviceid.ToString());
                node.SetAttribute("category_showall", Nodes.category_showall.ToString());
                node.SetAttribute("category_version", Nodes.category_version.ToString());
                node.SetAttribute("ClassName", Nodes.ClassName);
                node.SetAttribute("ImagePath", Nodes.ImagePath);
                node.SetAttribute("ImageHref", Nodes.ImageHref);

                //foreach (var item in Tree.Nodes)
                //{
                node.AppendChild(setXmlNode(Tree, doc, false, ALL)); //Set each node in List<Category_TreeItem> into XML Temp file
                //}
            }
            else
            {
                node.SetAttribute("category_id", Nodes.Nodes[layer - 1].category_id.ToString());
                node.SetAttribute("category_description", Nodes.Nodes[layer - 1].category_description);
                node.SetAttribute("category_title", Nodes.Nodes[layer - 1].category_title);
                node.SetAttribute("category_layer", Nodes.Nodes[layer - 1].category_layer.ToString());
                node.SetAttribute("category_parentid", Nodes.Nodes[layer - 1].category_parentid.ToString());
                node.SetAttribute("category_categoryfromwsid", Nodes.Nodes[layer - 1].category_categoryfromwsid.ToString());
                node.SetAttribute("category_showorder", Nodes.Nodes[layer - 1].category_showorder.ToString());
                node.SetAttribute("category_sellerid", Nodes.Nodes[layer - 1].category_sellerid.ToString());
                node.SetAttribute("category_deviceid", Nodes.Nodes[layer - 1].category_deviceid.ToString());
                node.SetAttribute("category_showall", Nodes.Nodes[layer - 1].category_showall.ToString());
                node.SetAttribute("category_version", Nodes.Nodes[layer - 1].category_version.ToString());
                node.SetAttribute("ClassName", Nodes.Nodes[layer - 1].ClassName);
                node.SetAttribute("ImagePath", Nodes.Nodes[layer - 1].ImagePath);
                node.SetAttribute("ImageHref", Nodes.Nodes[layer - 1].ImageHref);
                node.AppendChild(setParentNode(Nodes, Tree, doc, (layer - 1), ALL));

            }
            return node;


        }
        private XmlNode setXmlNode(Category_TreeItem Nodes, XmlDocument doc, bool Break, bool ALL) //Write Into XML
        {
            //XmlNode node = doc.CreateNode(XmlNodeType.Element, Node.category_title, " ");
            XmlElement node = doc.CreateElement(Category_type(Nodes.category_layer));
            node.SetAttribute("category_id", Nodes.category_id.ToString());
            node.SetAttribute("category_description", Nodes.category_description);
            node.SetAttribute("category_title", Nodes.category_title);
            node.SetAttribute("category_layer", Nodes.category_layer.ToString());
            node.SetAttribute("category_parentid", Nodes.category_parentid.ToString());
            node.SetAttribute("category_categoryfromwsid", Nodes.category_categoryfromwsid.ToString());
            node.SetAttribute("category_showorder", Nodes.category_showorder.ToString());
            node.SetAttribute("category_sellerid", Nodes.category_sellerid.ToString());
            node.SetAttribute("category_deviceid", Nodes.category_deviceid.ToString());
            node.SetAttribute("category_showall", Nodes.category_showall.ToString());
            node.SetAttribute("category_version", Nodes.category_version.ToString());
            node.SetAttribute("ClassName", Nodes.ClassName);
            node.SetAttribute("ImagePath", Nodes.ImagePath);
            node.SetAttribute("ImageHref", Nodes.ImageHref);
            //node.SetAttribute("category_createuser", Nodes.category_createuser.ToString()); //Remain
            //node.SetAttribute("category_createdate", Nodes.category_createdate.ToString()); //Remain
            //node.SetAttribute("category_updated", Nodes.category_updated.ToString()); //Remain
            //node.SetAttribute("category_updatedate", Nodes.category_updatedate.ToString()); //Remain
            //node.SetAttribute("category_updateuser", Nodes.category_updateuser.ToString()); //Remain

            if (!Break)
            {
                foreach (var item in Nodes.Nodes) //Get all children nodes in this node
                {
                    if (!ALL)
                    {
                        node.AppendChild(setXmlNode(item, doc, true, ALL)); //Write Each Node to XML File
                    }
                    else
                    {
                        node.AppendChild(setXmlNode(item, doc, false, ALL)); //Write Each Node to XML File
                    }
                }
            }

            /* //Get all children nodes in this node
            foreach (var item in Nodes.Nodes) //Get all children nodes in this node
            {
                node.AppendChild(getXmlNode(item, doc)); //Write Each Node to XML File
            }*/
            return node;
        }
        private XmlNode setXmlNode(int Nodes, XmlDocument doc, bool Break, bool ALL) //Write Into XML
        {
            //XmlNode node = doc.CreateNode(XmlNodeType.Element, Node.category_title, " ");
            XmlElement node = doc.CreateElement("Brands");
            node.SetAttribute("category_brands", Nodes.ToString());

            if (!Break)
            {

            }

            /* //Get all children nodes in this node
            foreach (var item in Nodes.Nodes) //Get all children nodes in this node
            {
                node.AppendChild(getXmlNode(item, doc)); //Write Each Node to XML File
            }*/
            return node;
        }
        private void FindParentNodes(Category_TreeItem Nodes, List<Category_TreeItem> ParentsNodes, int Layer, int index) //Find the Node and all of it's parents.
        {
            if (index <= Layer)
            {
                Category_TreeItem Item = new Category_TreeItem();
                //Item.category_sn = Nodes.category_sn;
                Item.category_id = Nodes.category_id;
                Item.category_title = Nodes.category_title;
                Item.category_description = Nodes.category_description;
                Item.category_layer = Nodes.category_layer;
                Item.category_parentid = Nodes.category_parentid;
                Item.category_categoryfromwsid = Nodes.category_categoryfromwsid;
                Item.category_showorder = Nodes.category_showorder;
                Item.category_sellerid = Nodes.category_sellerid;
                Item.category_deviceid = Nodes.category_deviceid;
                Item.category_showall = Nodes.category_showall;
                Item.category_version = Nodes.category_version;
                Item.ClassName = Nodes.ClassName;
                Item.ImagePath = Nodes.ImagePath;
                Item.ImageHref = Nodes.ImageHref;

                ParentsNodes.Add(Item);
                FindParentNodes(Nodes.Parents, ParentsNodes, Layer, ++index);
            }
            return;
        }
        private void FindLayerNode(List<Category_TreeItem> Tree, List<Category_TreeItem> LayerTree, int Layer) //Search the id and the Layer in the Tree
        {
            foreach (Category_TreeItem node in Tree) //Find the Node in same layer
            {
                if (node.category_layer == Layer)
                {
                    LayerTree.Add(node);
                }
                else
                {
                    FindLayerNode(node.Nodes, LayerTree, Layer); //If there had childrens under the nodes, then find it
                    //return;
                }
            }
            return;
        }
        protected void FindNodeBrand(Category_TreeItem Node, List<int> Brands) //Search all brands in this node
        {

            List<int> nodes = new List<int>();

            if (Node.Nodes != null && Node.Nodes.Count == 0)
            {
                nodes.Add(Node.category_id);
            }
            else
            {
                FindBrandFromNodes(Node.Nodes, nodes);
            }
            //List<int> Manufatoryid = new List<int>();
            //foreach (var item in nodes)
            //{
            //    Manufatoryid.AddRange((from A in db.item where A.item_categoryid == item select A.item_manufactoryid).Distinct().ToList());
            //}

            //Manufatoryid = Manufatoryid.Distinct().ToList();
            //Brands.AddRange(Manufatoryid.Distinct().ToList());
            var dateTiemNow = DateTime.Now;
            //Brands.AddRange((from A in db.Item where nodes.Contains(A.item_categoryid) && A.item_status == 0 && A.item_showorder >= 0 && A.item_datestart < dateTiemNow && A.item_dateend > dateTiemNow select A.item_manufactureid).Distinct().ToList());
            var selectedBradn = (from A in db.Item where nodes.Contains(A.CategoryID) && A.Status == 0 && A.ShowOrder >= 0 && A.DateStart < dateTiemNow && A.DateEnd > dateTiemNow select A.ManufactureID).Distinct().ToList();
            Brands.AddRange(db.Manufacture.Where(x => x.Status == 0 && selectedBradn.Contains(x.ID)).Distinct().OrderBy(x => x.Showorder).Select(x => x.ID).ToList());

        }
        protected void FindNodeBrand(Category_TreeItem Node, List<int> Brands, int specifyBrandID) //Search the brand in these node
        {

            //List<int> nodes = new List<int>();

            //if (Node.Nodes != null && Node.Nodes.Count == 0)
            //{
            //    nodes.Add(Node.category_id);
            //}
            //else
            //{
            //    FindBrandFromNodes(Node.Nodes, nodes);
            //}
            //List<int> Manufatoryid = new List<int>();
            //foreach (var item in nodes)
            //{
            //    Manufatoryid.AddRange((from A in db.item where A.item_categoryid == item select A.item_manufactoryid).Distinct().ToList());
            //}

            //Manufatoryid = Manufatoryid.Distinct().ToList();
            //Brands.AddRange(Manufatoryid.Distinct().ToList());
            //var dateTiemNow = DateTime.Now;
            //Brands.AddRange((from A in db.Item where nodes.Contains(A.item_categoryid) && A.item_status == 0 && A.item_showorder >= 0 && A.item_datestart < dateTiemNow && A.item_dateend > dateTiemNow select A.item_manufactureid).Distinct().ToList());
            Brands.Add(specifyBrandID);


        }
        private void FindBrandFromNodes(List<Category_TreeItem> NodeTree, List<int> nodes)
        {
            foreach (var Node in NodeTree)
            {
                if (Node.Nodes != null && Node.Nodes.Count == 0)
                {
                    nodes.Add(Node.category_id);
                }
                else
                {
                    FindBrandFromNodes(Node.Nodes, nodes);
                }
            }
        }
        protected void CreateFolder(string FolderName)
        {
            string PathString = AppDomain.CurrentDomain.BaseDirectory + FolderName;
            try
            {
                if (Directory.Exists(PathString))
                {
                    return;
                }
                System.IO.Directory.CreateDirectory(PathString);
                return;

            }
            catch (Exception e)
            {
                return;
            }

        }
        protected int Get_Total_Layer()
        {
            try
            {
                int Layer = (from A in db.Category
                             select A.Layer).Max();
                return Layer;
            }
            catch (Exception e)
            {
                return 0;
            }
        }
        protected string Category_type(int type) //Set XML Label Name
        {
            string Type_Name;

            switch (type)
            {
                case 0:
                    Type_Name = "Tab";
                    break;
                case 1:
                    Type_Name = "Category";
                    break;
                case 2:
                    Type_Name = "Subcategory";
                    break;
                case 3:
                    Type_Name = "Products";
                    break;
                case 4:
                    Type_Name = "Layer5";
                    break;
                case 5:
                    Type_Name = "Layer6";
                    break;
                case 6:
                    Type_Name = "Layer7";
                    break;
                case 7:
                    Type_Name = "Layer8";
                    break;
                case 8:
                    Type_Name = "Layer9";
                    break;
                case 9:
                    Type_Name = "Layer10";
                    break;
                default:
                    Type_Name = type.ToString();
                    break;
            }




            return Type_Name;
        }


        private IQueryable<TWNewEgg.DB.TWSQLDB.Models.Category> GetAllCategories()
        {
            return db.Category;
        }

        public List<TWNewEgg.DB.TWSQLDB.Models.Category> FindBottomLayerCategory(List<int> categoryID)
        {
            List<TWNewEgg.DB.TWSQLDB.Models.Category> bottomLayerCategory = new List<TWNewEgg.DB.TWSQLDB.Models.Category>();

            allCategorirsFromDB = GetAllCategories();

            foreach (var aCategoryID in categoryID)
            {
                var parentNode = allCategorirsFromDB.Where(x => x.ID == aCategoryID).FirstOrDefault();
                FindBottomLayerCategories(bottomLayerCategory, parentNode, parentNode.Layer);
            }

            return bottomLayerCategory;
        }

        private void FindBottomLayerCategories(List<TWNewEgg.DB.TWSQLDB.Models.Category> bottomCategories, TWNewEgg.DB.TWSQLDB.Models.Category parentNode, int layer) //Find the children Node which parentid same with this Node's parent and Layer is over than 1
        {
            var nextLayerCategories = allCategorirsFromDB.Where(x => x.ParentID == parentNode.ID && x.Layer == (layer + 1)).ToList();
            if (nextLayerCategories.Count == 0)
            {
                bottomCategories.Add(parentNode);
                return;
            }
            else
            {
                foreach (var aNextLayerCategories in nextLayerCategories)
                {
                    var nextParentNode = allCategorirsFromDB.Where(x => x.ID == aNextLayerCategories.ID).FirstOrDefault();
                    FindBottomLayerCategories(bottomCategories, nextParentNode, nextParentNode.Layer);
                }
            }
        }

        /// <summary>
        /// 組好下層的所有Category, 回傳Tree, 此函式不需指定Layer, 此函式為遞迴
        /// </summary>
        /// <param name="argNumCategoryId"></param>
        /// <param name="numCountLayer">避免無窮迴圈的計數器, 第一次呼叫請放0</param>
        /// <returns>List of Category_TreeItem</returns>
        protected List<Category_TreeItem> GetNextLayerCategory(int argNumCategoryId, int numCountLayer)
        {
            List<Category_TreeItem> listResult = null;
            Category_TreeItem objTreeItem = null;
            List<Category> listCategory = null;
            int numNowLayer = numCountLayer + 1;//避免無窮迴圈的計數器
            TWNewEgg.DB.TWBACKENDDB.Models.CategoryAssociatedWithPM objManager = null;

            //若是計算層數大於10層為不合理, 故應直接跳出遞迴
            if (numCountLayer >= 10)
            {
                return null;
            }

            //取得該Category Id下的第一層目錄
            listCategory = this.allCategorirsFromDB.Where(x => x.ParentID == argNumCategoryId && x.ShowAll >= 0).OrderBy(x=>x.Showorder).ToList();
            if (listCategory != null && listCategory.Count > 0)
            {
                listResult = new List<Category_TreeItem>();
                foreach (Category objSubCategory in listCategory)
                {
                    objTreeItem = new Category_TreeItem();
                    objTreeItem.category_categoryfromwsid = objSubCategory.CategoryfromwsID;
                    objTreeItem.category_createdate = objSubCategory.CreateDate;
                    objTreeItem.category_createuser = objSubCategory.CreateUser;
                    objTreeItem.category_description = objSubCategory.Description;
                    objTreeItem.category_deviceid = objSubCategory.DeviceID;
                    objTreeItem.category_id = objSubCategory.ID;
                    objTreeItem.category_layer = objSubCategory.Layer;
                    objTreeItem.category_parentid = objSubCategory.ParentID;
                    objTreeItem.category_sellerid = objSubCategory.SellerID;
                    objTreeItem.category_showall = objSubCategory.ShowAll;
                    objTreeItem.category_showorder = objSubCategory.Showorder;
                    objTreeItem.category_title = objSubCategory.Title;
                    objTreeItem.category_translatecountryid = objSubCategory.TranslateCountryID;
                    objTreeItem.category_translateid = objSubCategory.TranslateID;
                    objTreeItem.category_updated = objSubCategory.Updated;
                    objTreeItem.category_updatedate = objSubCategory.UpdateDate;
                    objTreeItem.category_updateuser = objSubCategory.UpdateUser;
                    objTreeItem.category_version = objSubCategory.VerSion;
                    objTreeItem.ClassName = objSubCategory.ClassName ?? string.Empty;
                    objTreeItem.ImageHref = objSubCategory.ImageHref ?? string.Empty;
                    objTreeItem.ImagePath = objSubCategory.ImagePath ?? string.Empty;
                    //判斷有沒Manager
                    objManager = this.mListCategoryPm.FirstOrDefault(x => x.CategoryID == objTreeItem.category_id && x.ActiveStatus == 1 && x.Delvtype == "f");
                    if (objManager != null)
                    {
                        objTreeItem.category_manager = objManager.ManagerName;
                    }
                    else
                    {
                        objTreeItem.category_manager = "";
                    }

                    objTreeItem.Nodes = this.GetNextLayerCategory(objSubCategory.ID, numNowLayer);
                    listResult.Add(objTreeItem);
                }
            }

            return listResult;
        }

    }
}
