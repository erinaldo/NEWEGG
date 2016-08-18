using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.Entity;
using System.Threading.Tasks;
using TWNewEgg.DB.TWSQLDB.Models;
using TWNewEgg.CategoryService.Models;
using TWNewEgg.DB.TWBACKENDDB.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace TWNewEgg.CategoryService.Service
{
    public class GlobeCategory : BaseService
    {

        private static DateTime dateTimeNow;
        public void XMLGenerate() //CALL DB 2 XML
        {
            //Stopwatch Timer = new Stopwatch();
            //Timer.Reset();
            //Timer.Start();
            string PathName = "\\XML\\";
            dateTimeNow = DateTime.Now;
            //int a;
            //a = Sav2Tree(Root, 0, 0);
            List<Category_TreeItem> Root = new List<Category_TreeItem>();
            Category_TreeItem RootNode = new Category_TreeItem();
            RootNode.category_id = 0;
            RootNode.category_title = "Root";
            RootNode.category_layer = 0;
            RootNode.Parents = null;
            RootNode.Nodes = new List<Category_TreeItem>();

            Root.Add(RootNode);
            //Root.Add(new Category_TreeItem { category_id = 0, category_title = "Home" });
            //var data_layer = (from A in db.category select A.category_layer).Max(); 
            //int Maxlayer = data_layer;
            //for (int layer = 0; layer <= a; layer++)
            //{


            //Make sure the category had item 
            List<int> itemIDs = (from A in db.Item where A.Status == 0 && A.ShowOrder >= 0 && A.DateEnd > dateTimeNow && A.DateStart < dateTimeNow select A.CategoryID).Distinct().ToList();
            List<TWNewEgg.DB.TWSQLDB.Models.Category> totalCategory = (from A in db.Category where itemIDs.Contains(A.ID) select A).ToList();

            int maxCategoryLayer = totalCategory.Select(x => x.Layer).Max();
            for (int i = maxCategoryLayer; i > -1; i--)
            {
                List<int> CID = totalCategory.Select(x => x.ParentID).ToList();
                totalCategory.AddRange((from A in db.Category where CID.Contains(A.ID) select A).Distinct().ToList());
                //totalCategory.AddRange(db.Category.Where(x => totalCategory.Select(y => y.category_id).Contains(x.category_parentid) && x.category_layer == i).ToList());
            }
            totalCategory.RemoveAll(x => x.ShowAll == 0);
            List<TWNewEgg.DB.TWSQLDB.Models.Category> newtotalCategory = totalCategory.Distinct().ToList();

            Sav2Tree(Root[0].Nodes, 0, -1, Root[0], newtotalCategory); //Start set the data from DB into List<Category_TreeItem>, give parentid and layerid to build the tree


            Total_Layer = Get_Total_Layer(); //Get the Max Layer from DB category table
            //}
            for (int i = 0; i <= Total_Layer; i++)
            {
                PathName = PathName + Category_type(i);
                CreateFolder(PathName); //CreateFolder to save Xml file
                PathName = "\\XML\\";
            }



            //Timer.Stop();


            //ViewBag.Timer = Timer.ElapsedMilliseconds.ToString();



            SQL2XML(Root, PathName); //Start save List<Category_TreeItem> into XML




            //int b = Root[0].Nodes[0].category_id;

        }

        private void Sav2Tree(List<Category_TreeItem> Node, int parentid, int Layer, Category_TreeItem ParentNode, List<TWNewEgg.DB.TWSQLDB.Models.Category> TotalTree) //Find the children Node which parentid same with this Node's parent and Layer is over than 1
        {
            List<Category_TreeItem> nodes = new List<Category_TreeItem>();
            //var data_parentid = (from A in db.category where A.category_layer == layer select A.category_parentid).Distinct().ToList();

            nodes = (from A in TotalTree                           //Search every parentid from catagory table and parentid same with parent node, and layer is more 1 than parent node
                     where A.ParentID == parentid && A.Layer == (Layer + 1)
                     orderby A.Showorder, A.ID
                     select new Category_TreeItem
                     {
                         //category_sn = A.category_sn,
                         category_id = A.ID,
                         category_title = A.Title,
                         category_description = A.Description,
                         category_layer = A.Layer,
                         category_parentid = A.ParentID,
                         category_categoryfromwsid = A.CategoryfromwsID,
                         category_showorder = A.Showorder,
                         category_sellerid = A.SellerID,
                         category_deviceid = A.DeviceID,
                         category_showall = A.ShowAll,
                         category_version = A.VerSion,
                         category_createuser = A.CreateUser,
                         category_createdate = A.CreateDate,
                         category_updated = A.Updated,
                         category_updatedate = A.UpdateDate,
                         category_updateuser = A.UpdateUser,
                         ClassName = A.ClassName ?? string.Empty,
                         ImagePath = A.ImagePath ?? string.Empty,
                         ImageHref = A.ImageHref ?? string.Empty
                         //Nodes = new List<Category_TreeItem>()
                     }).ToList();
            foreach (var item in nodes)
            {
                item.Parents = ParentNode;
            }
            //var data_layer = (from A in db.category select A.category_layer).Max();
            if (nodes.Count() > 0)
            {

                foreach (var item in nodes) //start write into List<Category_TreeItem>, also find all row data who row parentid is same with in the nodes
                {
                    item.Nodes = new List<Category_TreeItem>();
                    Node.Add(item);
                    Sav2Tree(item.Nodes, item.category_id, item.category_layer, item, TotalTree); //call self to find out which row data is this node's childrens.**********************************************************
                    //Sav2Tree(item.Nodes, item.category_sn); //call self to find out which row data is this node's childrens.************************************************************

                }
            }


            /*foreach (var item in data_parentid)
            {






                var data = (from A in db.category
                            where A.category_layer == layer && A.category_parentid == item
                            select new Category_TreeItem
                            {
                                category_sn = A.category_sn,
                                category_id = A.category_id,
                                category_title = A.category_title,
                                category_description = A.category_description,
                                category_layer = A.category_layer,
                                category_parentid = A.category_parentid,
                                category_categoryfromwsid = A.category_categoryfromwsid,
                                category_showorder = A.category_showorder,
                                category_sellerid = A.category_sellerid,
                                category_deviceid = A.category_deviceid,
                                category_showall = A.category_showall,
                                category_version = A.category_version,
                                category_createuser = A.category_createuser,
                                category_createdate = A.category_createdate,
                                category_updated = A.category_updated,
                                category_updatedate = A.category_updatedate,
                                category_updateuser = A.category_updateuser,
                                //Nodes = new List<Category_TreeItem>()
                            });
                try
                {*/
            //Category_TreeItem Nodes = new Category_TreeItem();
            //CategoryTree[1].Nodes = new List<Category_TreeItem>();
            //Nodes = data.ToList();
            //CategoryTree[0].Nodes = new List<Category_TreeItem>();
            //Nodes.Nodes = new List<Category_TreeItem>();
            //Nodes.Nodes = data.ToList();


            /*CategoryTree.Nodes = new List<Category_TreeItem>();
            CategoryTree.Nodes = data.ToList();
            CategoryTree.Nodes[0].Nodes = new List<Category_TreeItem>();
            CategoryTree.Nodes[0].Nodes = data.ToList();
            CategoryTree.Nodes[0].Nodes[0].Nodes = new List<Category_TreeItem>();
            CategoryTree.Nodes[0].Nodes[0].Nodes = data.ToList();*/
            //List<Category_TreeItem> AddTree = new List<Category_TreeItem>();
            //AddTree = data.ToList();
            //nodes.Find(delegate(Category_TreeItem p) { return (p.category_id == parentid); }).Nodes.AddRange(data.ToList());
            /*foreach (Category_TreeItem p in AddTree)
            {
                if (p.category_id == item)
                    p.Nodes = AddTree;
            }

            if (nodes != null)
            {
                TempData["alertMessage"] = "No Data";
                //return View(CategoryTree);
            }
            else
            {
                TempData["alertMessage"] = "No Data";
                //return View();
            }
        }
        catch (Exception ex)
        {
            TempData["alertMessage"] = ex.ToString();
            //return View();
        }
        //Sav2Tree()


    }
    layer++;
    //var NN = (from tt in prenodes select tt).ToList();
    //prenodes.AddRange(Sav2Tree(nodes, nodes, layer, Maxlayer, parentid));
    return new List<Category_TreeItem>();
            
    */

            //List<Category_TreeItem> CategoryTree = new List<Category_TreeItem>();

            /*Category_TreeItem CategoryTree = new Category_TreeItem();*/

            //Category_TreeItem a;
            //a = CategoryTree[0].Nodes.Find(delegate(Category_TreeItem p){return (p.category_id == 3);});
            //Category_TreeItem b = new Category_TreeItem();
            //b.category_id = 1;
            //CategoryTree[0].Nodes.Find(delegate(Category_TreeItem p) { return (p.category_id == 3); }).Nodes.Add(b);

            /*var data = (from A in db.category
                        where A.category_layer == layer && A.category_parentid == 1
                        select new Category_TreeItem
                        {
                            category_sn = A.category_sn,
                            category_id = A.category_id,
                            category_title = A.category_title,
                            category_description = A.category_description,
                            category_layer = A.category_layer,
                            category_parentid = A.category_parentid,
                            category_categoryfromwsid = A.category_categoryfromwsid,
                            category_showorder = A.category_showorder,
                            category_sellerid = A.category_sellerid,
                            category_deviceid = A.category_deviceid,
                            category_showall = A.category_showall,
                            category_version = A.category_version,
                            category_createuser = A.category_createuser,
                            category_createdate = A.category_createdate,
                            category_updated = A.category_updated,
                            category_updatedate = A.category_updatedate,
                            category_updateuser = A.category_updateuser,
                            //Nodes = new List<Category_TreeItem>()
                        });*/
            //try
            //{
            //Category_TreeItem Nodes = new Category_TreeItem();
            //CategoryTree[1].Nodes = new List<Category_TreeItem>();
            //Nodes = data.ToList();
            //CategoryTree[0].Nodes = new List<Category_TreeItem>();
            //Nodes.Nodes = new List<Category_TreeItem>();
            //Nodes.Nodes = data.ToList();


            /*CategoryTree.Nodes = new List<Category_TreeItem>();
            CategoryTree.Nodes = data.ToList();
            CategoryTree.Nodes[0].Nodes = new List<Category_TreeItem>();
            CategoryTree.Nodes[0].Nodes = data.ToList();
            CategoryTree.Nodes[0].Nodes[0].Nodes = new List<Category_TreeItem>();
            CategoryTree.Nodes[0].Nodes[0].Nodes = data.ToList();*/
            /*
                nodes.Nodes = data.ToList();


                if (nodes.Nodes != null)
                {
                    TempData["alertMessage"] = "No Data";
                    //return View(CategoryTree);
                }
                else
                {
                    TempData["alertMessage"] = "No Data";
                    //return View();
                }
            }
            catch (Exception ex)
            {
                TempData["alertMessage"] = ex.ToString();
                //return View();
            }*/

        }



        /* Dump All Tree Data to XML File
        public void SQL2XML(List<Category_TreeItem> Tree, string PathName) //Set XML File
        {
            string FileName;
            XmlDocument doc = new XmlDocument(); //Set XML File
            doc.AppendChild(doc.CreateXmlDeclaration("1.0", "UTF-8", "yes")); //Set XML Header
            //XmlNode node = doc.CreateNode(XmlNodeType.Element, "Root", "");
            XmlElement node = doc.CreateElement(Tree[0].category_title.ToString()); //Set First Node
            node.SetAttribute("category_id", Tree[0].category_id.ToString()); //Set First Node
            FileName = Category_type(Tree[0].category_layer) + "\\id_" + Tree[0].category_id.ToString() + ".XML";
            foreach (var item in Tree[0].Nodes)
            {
                node.AppendChild(getXmlNode(item, doc)); //Set each node in List<Category_TreeItem> into XML Temp file
            }
            doc.AppendChild(node); //Add all temp file into XmlDocument
            try
            {
                PathName = PathName + FileName;
                doc.Save(AppDomain.CurrentDomain.BaseDirectory + PathName); //Write XML File into Computer
            }
            catch (Exception)
            {
                return;
            }

        }*/
        
        /// <summary>
        /// 取得Category Tree
        /// </summary>
        /// <returns>Category Tree</returns>
        public List<Category_TreeItem> GetCategoryTreeMap()
        {
            List<Category_TreeItem> listResult = new List<Category_TreeItem>();
            listResult = base.GetNextLayerCategory(0, 0);

            return listResult;
        }

        /// <summary>
        /// 取得Category Tree
        /// </summary>
        /// <param name="numCategoryId"></param>
        /// <returns>Category Tree</returns>
        public List<Category_TreeItem> GetCategoryTreeMap(int numCategoryId)
        {
            List<Category_TreeItem> listResult = new List<Category_TreeItem>();
            listResult = base.GetNextLayerCategory(numCategoryId, 0);

            return listResult;
        }


        /// <summary>
        /// 取得後台所有User的列表
        /// </summary>
        /// <param name="argNumRoleId">if 0 return all users</param>
        /// <returns>use list</returns>
        public List<User> GetUserList(int argNumRoleId)
        {
            TWNewEgg.DB.TWBackendDBContext objBackDb = null;
            List<User> listUser = null;

            objBackDb = new DB.TWBackendDBContext();

            if (argNumRoleId == 0)
            {
                listUser = objBackDb.User.ToList<User>();
            }
            else
            {
                listUser = objBackDb.User.Where(x => x.RoleID == argNumRoleId).ToList<User>();
            }
            objBackDb.Dispose();
            objBackDb = null;

            return listUser;
        }

        /// <summary>
        /// 根據User Id取得Category
        /// </summary>
        /// <param name="argStrUserName">User Name</param>
        /// <returns>Category Id or null</returns>
        public List<int> GetActiveCategoryVerifyByUserName(string argStrUserName)
        {
            TWNewEgg.DB.TWBackendDBContext objBackDb = null;
            List<int> listCategoryId = null;

            objBackDb = new DB.TWBackendDBContext();
            listCategoryId = objBackDb.CategoryAssociatedWithPM.Where(x => x.ManagerName == argStrUserName && x.ActiveStatus == 1).Select(x => x.CategoryID).ToList<int>();

            objBackDb.Dispose();
            objBackDb = null;

            return listCategoryId;
        }


        /// <summary>
        /// 移除該User所有的權限
        /// </summary>
        /// <param name="argNumUserId">User Id</param>
        /// <returns></returns>
        public bool RemoveAllCategoryVerifyByUser(int argNumUserId)
        {
            return false;
        }

        /// <summary>
        /// 移除該User管理此Category的權限
        /// </summary>
        /// <param name="argNumCategoryId"></param>
        /// <param name="argNumUserId"></param>
        /// <returns></returns>
        public bool RemoveCategoryVerify(int argNumCategoryId, int argNumUserId)
        {
            return false;
        }

        /// <summary>
        /// 設定該User可編輯的Category
        /// </summary>
        /// <param name="argStrCategoryMap">string of CategoryMap</param>
        /// <param name="argNumUserId">User Id</param>
        /// <returns>updated(or created) success return true, else return false</returns>
        public bool AddCategoryVerifyByUser(string argStrCategoryMap, int argNumUserId)
        {
            return false;
        }

        /// <summary>
        /// 設定該User可管理此Category
        /// </summary>
        /// <param name="argNumCategoryId">Category Id</param>
        /// <param name="argNumUserId">User Id</param>
        /// <returns></returns>
        public bool AddCategoryVerifyByUser(int argNumCategoryId, int argNumUserId)
        {
            return false;
        }

        /// <summary>
        /// 根據Category Id取得具有權限的User列表
        /// </summary>
        /// <param name="argNumCategoryId">Category Id</param>
        /// <returns>User List or null</returns>
        public List<TWNewEgg.DB.TWBACKENDDB.Models.CategoryAssociatedWithPM> GetVerifyUserListByCategoryId(int argNumCategoryId)
        {
            List<TWNewEgg.DB.TWBACKENDDB.Models.CategoryAssociatedWithPM> listUser = null;

            listUser = (from CategoryPm in base.mObjBackDb.CategoryAssociatedWithPM
                        join x in base.mObjBackDb.User
                        on CategoryPm.ManagerName equals x.Name
                        where CategoryPm.CategoryID == argNumCategoryId && CategoryPm.ActiveStatus == 1
                        select CategoryPm).ToList<TWNewEgg.DB.TWBACKENDDB.Models.CategoryAssociatedWithPM>();

            return listUser;
        }


        /// <summary>
        /// 修改Category的管理者
        /// </summary>
        /// <param name="argObjCategoryPm">Category Object</param>
        /// <returns></returns>
        public bool UpdateCategoryManager(CategoryAssociatedWithPM argObjCategoryPm)
        {
            bool boolExec = false;
            int numId = -1;
            List<CategoryAssociatedWithPM> listCategoryOtherPm = null;
            List<CategoryAssociatedWithPM> listCategoryNewPm = null;
            string strJson = "";
            TWNewEgg.DB.TWSQLDB.Models.CategoryUpdatedHistory objHistory = null;

            if (argObjCategoryPm == null)
            {
                return boolExec;
            }

            //先取得是否有此筆資料
            listCategoryNewPm = base.mObjBackDb.CategoryAssociatedWithPM.Where(x => x.CategoryID == argObjCategoryPm.CategoryID && x.ManagerName == argObjCategoryPm.ManagerName && x.Delvtype == "f").ToList < TWNewEgg.DB.TWBACKENDDB.Models.CategoryAssociatedWithPM>();
            listCategoryOtherPm = base.mObjBackDb.CategoryAssociatedWithPM.Where(x => x.CategoryID == argObjCategoryPm.CategoryID && (x.ManagerName != argObjCategoryPm.ManagerName || (x.ManagerName == argObjCategoryPm.ManagerName && x.Delvtype != "f"))).ToList < TWNewEgg.DB.TWBACKENDDB.Models.CategoryAssociatedWithPM>();
            

            //若NewPM的資料已存在, 設定會ActiveStatus=1, 其他PM的資料則要下線
            if (listCategoryNewPm != null && listCategoryNewPm.Count > 0)
            {
                foreach (CategoryAssociatedWithPM objSubPm in listCategoryNewPm)
                {
                    //記錄History
                    if (objSubPm.ActiveStatus == 1)
                    {
                        strJson += JsonConvert.SerializeObject(objSubPm);
                    }

                    objSubPm.ActiveStatus = 1;
                    objSubPm.UpdateUser = argObjCategoryPm.UpdateUser;
                    objSubPm.UpdateDate = DateTime.Now;
                    objSubPm.Updated += 1;
                }
                numId = 1;
            }
            else
            {
                argObjCategoryPm.CreateUser = argObjCategoryPm.UpdateUser;
                argObjCategoryPm.ProductMananeger = argObjCategoryPm.ManagerName;
                argObjCategoryPm.UpdateUser = null;
                numId = this.AddCategoryManager(argObjCategoryPm);
            }

            if (listCategoryOtherPm != null && listCategoryOtherPm.Count > 0)
            {
                foreach (CategoryAssociatedWithPM objSubPm in listCategoryOtherPm)
                {
                    //記錄History
                    if (objSubPm.ActiveStatus == 1)
                    {
                        strJson += JsonConvert.SerializeObject(objSubPm);
                    }

                    objSubPm.UpdateUser = argObjCategoryPm.UpdateUser;
                    objSubPm.UpdateDate = DateTime.Now;
                    objSubPm.Updated += 1;
                    objSubPm.ActiveStatus = 0;
                }
            }

            //History
            objHistory = new CategoryUpdatedHistory();
            objHistory.CategoryId = argObjCategoryPm.CategoryID;
            objHistory.BeforeUpdatedData = strJson;
            if (argObjCategoryPm.UpdateUser == null)
            {
                objHistory.CreateUser = argObjCategoryPm.CreateUser;
            }
            else
            {
                objHistory.CreateUser = argObjCategoryPm.UpdateUser;
            }
            objHistory.UpdatedType = (int)CategoryUpdatedHistory.CategoryUpdatedType.UpdatedManager;

            try
            {
                base.mObjBackDb.SaveChanges();
                base.db.Entry<CategoryUpdatedHistory>(objHistory).State = EntityState.Added;
                base.db.SaveChanges();
                boolExec = true;
            }
            catch (Exception ex)
            {
                throw (ex);
            }

            if (numId > 0 && boolExec)
            {
                return true;
            }
            else
            {
                return false;
            }

        }

        /// <summary>
        /// 更新Category
        /// </summary>
        /// <param name="argCategory">Category Object</param>
        /// <returns>update success return true, else return false</returns>
        public List<TWNewEgg.DB.TWSQLDB.Models.Category> UpdateCategory(List<TWNewEgg.DB.TWSQLDB.Models.Category> argListCategory)
        {
            List<TWNewEgg.DB.TWSQLDB.Models.Category> listError = null;
            List<TWNewEgg.DB.TWSQLDB.Models.Category> listOldCategory = null;
            List<int> listIntCategoryId = null;
            TWNewEgg.DB.TWSQLDB.Models.Category objOldCategory = null;
            TWNewEgg.DB.TWSQLDB.Models.CategoryUpdatedHistory objHistory = null;
            string strJson = "";
            TWNewEgg.DB.TWSqlDBContext objSearchDb = null;

            if (argListCategory == null || argListCategory.Count <= 0)
            {
                return null;
            }

            //先找出舊的資料
            listIntCategoryId = argListCategory.Select(x => x.ID).ToList();
            if (listIntCategoryId == null || listIntCategoryId.Count <= 0)
            {
                return argListCategory;
            }
            //因Update & Search沒辦法用同一個映象Object, 為了讓Entry能順利運作, 故再另開SearchDb的接口
            objSearchDb = new DB.TWSqlDBContext();
            listOldCategory = objSearchDb.Category.Where(x => listIntCategoryId.Contains(x.ID)).ToList();
            if (listOldCategory == null || listOldCategory.Count <= 0)
            {
                objSearchDb.Dispose();
                return argListCategory;
            }

            //進行資枓儲存
            listError = new List<Category>();
            foreach (TWNewEgg.DB.TWSQLDB.Models.Category objSubCategory in argListCategory)
            {
                strJson = "";
                objSubCategory.UpdateDate = DateTime.Now;
                objSubCategory.Updated = objSubCategory.Updated + 1;

                //在修改之前, 要先記錄至History 
                objOldCategory = listOldCategory.Where(x => x.ID == objSubCategory.ID).FirstOrDefault();
                if (objOldCategory == null)
                {
                    listError.Add(objSubCategory);
                    continue;
                }
                strJson = JsonConvert.SerializeObject(objOldCategory);
                objHistory = new CategoryUpdatedHistory();
                objHistory.CategoryId = objSubCategory.ID;
                objHistory.BeforeUpdatedData = strJson;
                objHistory.CreateUser = objSubCategory.UpdateUser;
                objHistory.UpdatedType = (int)TWNewEgg.DB.TWSQLDB.Models.CategoryUpdatedHistory.CategoryUpdatedType.UpdatedBasicData;
                base.db.Entry<TWNewEgg.DB.TWSQLDB.Models.CategoryUpdatedHistory>(objHistory).State = System.Data.EntityState.Added;

                base.db.Entry<TWNewEgg.DB.TWSQLDB.Models.Category>(objSubCategory).State = System.Data.EntityState.Modified;
                try
                {
                    db.SaveChanges();
                }
                catch (Exception ex)
                {
                    listError.Add(objSubCategory);
                }
            }

            listOldCategory = null;
            listIntCategoryId = null;
            if (objSearchDb != null)
            {
                objSearchDb.Dispose();

            }

            if (listError.Count <= 0)
                return null;
            else
                return listError;
        }

        public bool UpdateCategory(TWNewEgg.DB.TWSQLDB.Models.Category argCategory)
        {
            if (argCategory == null)
            {
                return false;
            }

            bool boolExec = false;
            TWNewEgg.DB.TWSqlDBContext objDb = null;
            TWNewEgg.DB.TWSqlDBContext objSearchDb = null;
            TWNewEgg.DB.TWSQLDB.Models.CategoryUpdatedHistory objHistory = null;
            TWNewEgg.DB.TWSQLDB.Models.Category objOldCategory = null;
            string strJson = "";

            argCategory.UpdateDate = DateTime.Now;
            argCategory.Updated = argCategory.Updated + 1;

            objDb = new DB.TWSqlDBContext();
            objSearchDb = new DB.TWSqlDBContext();

            //在修改之前, 要先記錄至History (因Update & Search沒辦法用同一個映象Object, 為了讓Entry能順利運作, 故再另開SearchDb的接口
            objOldCategory = objSearchDb.Category.Where(x => x.ID == argCategory.ID).FirstOrDefault();
            if (objOldCategory == null)
            {
                objSearchDb.Dispose();
                objSearchDb = null;
                objDb.Dispose();
                objDb = null;
                return false;
            }
            strJson = JsonConvert.SerializeObject(objOldCategory);
            objHistory = new CategoryUpdatedHistory();
            objHistory.CategoryId = argCategory.ID;
            objHistory.BeforeUpdatedData = strJson;
            objHistory.CreateUser = argCategory.UpdateUser;
            objHistory.UpdatedType = (int)TWNewEgg.DB.TWSQLDB.Models.CategoryUpdatedHistory.CategoryUpdatedType.UpdatedBasicData;
            objDb.Entry<TWNewEgg.DB.TWSQLDB.Models.CategoryUpdatedHistory>(objHistory).State = System.Data.EntityState.Added;

            //設定新值
            objDb.Entry<TWNewEgg.DB.TWSQLDB.Models.Category>(argCategory).State = System.Data.EntityState.Modified;

            try
            {
                objDb.SaveChanges();
                boolExec = true;
            }
            catch (Exception ex)
            {
                boolExec = false;
            }

            objDb.Dispose();
            objSearchDb.Dispose();

            return boolExec;
        }

        public List<TWNewEgg.DB.TWSQLDB.Models.Category> GetAllCategoris()
        {
            List<TWNewEgg.DB.TWSQLDB.Models.Category> listCategory = null;

            listCategory = base.db.Category.Where(x=>x.ShowAll >=0).ToList<Category>();

            return listCategory;
        }

        public int AddCategory(TWNewEgg.DB.TWSQLDB.Models.Category argCategory)
        {
            if (argCategory == null)
            {
                return -1;
            }

            int numId = -1;
            TWNewEgg.DB.TWSqlDBContext objDb = null;

            argCategory.CreateDate = DateTime.Now;

            objDb = new DB.TWSqlDBContext();
            objDb.Category.Add(argCategory);
            try
            {
                objDb.SaveChanges();
                numId = argCategory.ID;
            }
            catch (Exception ex)
            {
                numId = -1;
            }

            objDb.Dispose();
            return numId;
        }

        /// <summary>
        /// 新增Category的管理權限
        /// </summary>
        /// <param name="argObjCategoryPm">Object of CategoryAssociatedWithPM</param>
        /// <returns>append success return Id, else return -1</returns>
        private int AddCategoryManager(CategoryAssociatedWithPM argObjCategoryPm)
        {
            int numId = -1;
            List<CategoryAssociatedWithPM> listCategoryNewPm = null;
            TWNewEgg.DB.TWBackendDBContext objBackDb = null;

            objBackDb = new DB.TWBackendDBContext();
            listCategoryNewPm = objBackDb.CategoryAssociatedWithPM.Where(x => x.CategoryID == argObjCategoryPm.CategoryID && x.ManagerName == argObjCategoryPm.ManagerName && x.Delvtype == "f").ToList();

            if (listCategoryNewPm == null || listCategoryNewPm.Count <= 0)
            {
                try
                {
                    argObjCategoryPm.ProductMananeger = argObjCategoryPm.ManagerName;
                    argObjCategoryPm.Delvtype = "f";
                    objBackDb.CategoryAssociatedWithPM.Add(argObjCategoryPm);
                    objBackDb.SaveChanges();
                    numId = argObjCategoryPm.ID;
                }
                catch (Exception ex)
                {
                    numId = -1;
                }
            }

            objBackDb.Dispose();
            objBackDb = null;
            listCategoryNewPm = null;

            return numId;
        }

        /*
        /// <summary>
        /// 新增Category Updated History的記錄
        /// </summary>
        /// <param name="argListHistory">list of CategoryUpdatedHistory objects</param>
        /// <returns>append success return true, else return false</returns>
        public bool AddCategoryUpdatedHistory(List<TWNewEgg.DB.TWSQLDB.Models.CategoryUpdatedHistory> argListHistory)
        {
            bool boolExec = false;
            TWNewEgg.DB.TWSqlDBContext objDb = null;

            if (argListHistory == null || argListHistory.Count <= 0)
            {
                return false;
            }

            objDb = new DB.TWSqlDBContext();
            foreach (TWNewEgg.DB.TWSQLDB.Models.CategoryUpdatedHistory objHistory in argListHistory)
            {
                objDb.CategoryUpdatedHistory.Add(objHistory);
            }

            try
            {
                objDb.SaveChanges();
                boolExec = true;
            }
            catch (Exception ex)
            {
                boolExec = false;
            }
            finally
            {
                objDb.Dispose();
                objDb = null;
            }

            return boolExec;
        }
        */

    }
}
