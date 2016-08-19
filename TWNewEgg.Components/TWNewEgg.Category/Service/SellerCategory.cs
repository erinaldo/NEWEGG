using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TWNewEgg.DB.TWSQLDB.Models;
using TWNewEgg.CategoryService.Models;

namespace TWNewEgg.CategoryService.Service
{
    public class SellerCategory : BaseService
    {
        private static DateTime dateTiemNow;
        private void BrandGenerate(string PathFolder, int SellerID)
        {
            List<int> BrandList = new List<int>();
            BrandList = (from A in db.Product                           //Search every parentid from catagory table and parentid same with parent node, and layer is more 1 than parent node
                         where A.SellerID == SellerID && A.Status == 0
                         select A.ManufactureID).Distinct().ToList();

            //ViewBag.BrandList = BrandList;


            foreach (var item in BrandList)
            {
                //var productid = (from A in db.product where A.product_manufactory == item select A);
                List<Category_TreeItem> BrandCategory = new List<Category_TreeItem>();
                var data = (from B in
                                (from A in db.Product where A.ManufactureID == item && A.SellerID == SellerID && A.Status == 0 select A)
                            join C in db.Item.Where(x => x.Status == 0 && x.ShowOrder >= 0 && x.DateStart < dateTiemNow && x.DateEnd > dateTiemNow)
                            on B.ID equals C.ProductID
                            join D in db.Category
                            on C.CategoryID equals D.ID
                            orderby D.Showorder, D.ID
                            select new Category_TreeItem
                            {
                                //B.product_id,
                                //C.item_id,
                                //D.category_id
                                category_id = D.ID,
                                category_title = D.Title,
                                category_description = D.Description,
                                category_layer = D.Layer,
                                category_parentid = D.ParentID,
                                category_categoryfromwsid = D.CategoryfromwsID,
                                category_showorder = D.Showorder,
                                category_sellerid = D.SellerID,
                                category_deviceid = D.DeviceID,
                                category_showall = D.ShowAll,
                                category_version = D.VerSion,
                                //category_createuser = D.category_createuser,
                                //category_createdate = D.category_createdate,
                                //category_updated = D.category_updated,
                                //category_updatedate = D.category_updatedate,
                                //category_updateuser = D.category_updateuser,
                            }).Distinct().ToList();
                BrandCategory.AddRange(data);

                Total_Layer = Get_Total_Layer(); //Get the Max Layer from DB category table
                string PathName = PathFolder + "\\XML_BRAND\\id_" + item + "\\";
                for (int i = 0; i <= Total_Layer; i++)
                {
                    PathName = PathName + Category_type(i);
                    CreateFolder(PathName); //CreateFolder to save Xml file
                    PathName = PathFolder + "\\XML_BRAND\\id_" + item + "\\";
                }

                foreach (var node in data)
                {

                    FindParentNodes(BrandCategory, node.category_layer, 0, node.category_parentid);
                    //FindParentNodes();

                }
                List<Category_TreeItem> BrandCateTree = new List<Category_TreeItem>();
                Category_TreeItem BrandNode = new Category_TreeItem();
                BrandNode.category_id = 0;
                BrandNode.category_title = "Root";
                BrandNode.category_layer = 0;
                BrandNode.Parents = null;
                BrandNode.Nodes = new List<Category_TreeItem>();

                BrandCateTree.Add(BrandNode);
                Sav2Tree(BrandCateTree[0].Nodes, 0, -1, BrandCateTree[0], BrandCategory); //Start set the data from DB into List<Category_TreeItem>, give parentid and layerid to build the tree

                //}





                SQL2XML(BrandCateTree, PathName); //Start save List<Category_TreeItem> into XML
                BrandCategory.Clear();


            }
        }
        public void SellerGenerate()
        {
            List<int> SellerList = new List<int>();
            //SellerList = (from A in db.product                           //Search every parentid from catagory table and parentid same with parent node, and layer is more 1 than parent node
            //              select A.product_sellerid).Distinct().ToList();
            SellerList = (from A in db.Seller select A.ID).Distinct().ToList();
            //ViewBag.BrandList = BrandList;
            dateTiemNow = DateTime.Now;

            foreach (var item in SellerList)
            {
                //var productid = (from A in db.product where A.product_manufactory == item select A);
                List<Category_TreeItem> BrandCategory = new List<Category_TreeItem>();
                var data = (from B in
                                (from A in db.Product where A.SellerID == item && A.Status == 0 select A)
                            join C in db.Item.Where(x => x.Status == 0 && x.DateEnd > dateTiemNow)
                            on B.ID equals C.ProductID
                            join D in db.Category
                            on C.CategoryID equals D.ID
                            orderby D.Showorder, D.ID
                            select new Category_TreeItem
                            {
                                //B.product_id,
                                //C.item_id,
                                //D.category_id
                                category_id = D.ID,
                                category_title = D.Title,
                                category_description = D.Description,
                                category_layer = D.Layer,
                                category_parentid = D.ParentID,
                                category_categoryfromwsid = D.CategoryfromwsID,
                                category_showorder = D.Showorder,
                                category_sellerid = D.SellerID,
                                category_deviceid = D.DeviceID,
                                category_showall = D.ShowAll,
                                category_version = D.VerSion,
                                //category_createuser = D.category_createuser,
                                //category_createdate = D.category_createdate,
                                //category_updated = D.category_updated,
                                //category_updatedate = D.category_updatedate,
                                //category_updateuser = D.category_updateuser,
                            }).Distinct().ToList();
                BrandCategory.AddRange(data);

                Total_Layer = Get_Total_Layer(); //Get the Max Layer from DB category table
                string PathName = "\\XML_SELLER\\id_" + item + "\\";
                for (int i = 0; i <= Total_Layer; i++)
                {
                    PathName = PathName + Category_type(i);
                    CreateFolder(PathName); //CreateFolder to save Xml file
                    PathName = "\\XML_SELLER\\id_" + item + "\\";
                }

                foreach (var node in data)
                {

                    FindParentNodes(BrandCategory, node.category_layer, 0, node.category_parentid);
                    //FindParentNodes();

                }
                List<Category_TreeItem> BrandCateTree = new List<Category_TreeItem>();
                Category_TreeItem BrandNode = new Category_TreeItem();
                BrandNode.category_id = 0;
                BrandNode.category_title = "Root";
                BrandNode.category_layer = 0;
                BrandNode.Parents = null;
                BrandNode.Nodes = new List<Category_TreeItem>();

                BrandCateTree.Add(BrandNode);
                Sav2Tree(BrandCateTree[0].Nodes, 0, -1, BrandCateTree[0], BrandCategory); //Start set the data from DB into List<Category_TreeItem>, give parentid and layerid to build the tree

                //}





                SQL2XML(BrandCateTree, PathName); //Start save List<Category_TreeItem> into XML
                BrandCategory.Clear();


                BrandGenerate(PathName, item);


            }
        }
        private void FindParentNodes(List<Category_TreeItem> BrandCategory, int Layer, int index, int Parentid) //Find the Node and all of it's parents.
        {
            if (index < Layer)
            {

                var data = (from A in db.Category
                            where A.ID == Parentid
                            orderby A.Showorder, A.ID
                            select new Category_TreeItem
                            {
                                //B.product_id,
                                //C.item_id,
                                //D.category_id
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
                                //category_createuser = D.category_createuser,
                                //category_createdate = D.category_createdate,
                                //category_updated = D.category_updated,
                                //category_updatedate = D.category_updatedate,
                                //category_updateuser = D.category_updateuser,
                            }).FirstOrDefault();
                //BrandCategory.AddRange(data);
                //if (BrandCategory.FindAll(x => x.category_id == data.category_id).Count == 0)
                if (data == null)
                {
                    return;
                }
                if (BrandCategory.FindAll(delegate(Category_TreeItem x) { return x.category_id == data.category_id; }).Count == 0)
                {
                    BrandCategory.Add(data);
                }
                FindParentNodes(BrandCategory, Layer, ++index, data.category_parentid);
            }
            return;
        }

        private void Sav2Tree(List<Category_TreeItem> BrandCateTree, int parentid, int Layer, Category_TreeItem ParentNode, List<Category_TreeItem> BrandCategory) //Find the children Node which parentid same with this Node's parent and Layer is over than 1
        {
            List<Category_TreeItem> nodes = new List<Category_TreeItem>();

            //nodes.AddRange(BrandCategory.FindAll(x => x.category_parentid == parentid && x.category_layer == (Layer + 1)));
            nodes.AddRange(BrandCategory.FindAll(delegate(Category_TreeItem x) { return x.category_parentid == parentid && x.category_layer == (Layer + 1); }));
            nodes = nodes.Where(x => x.category_showall == 1).OrderBy(x => x.category_showorder).ToList();
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
                    BrandCateTree.Add(item);
                    Sav2Tree(item.Nodes, item.category_id, item.category_layer, item, BrandCategory); //call self to find out which row data is this node's childrens.**********************************************************
                    //Sav2Tree(item.Nodes, item.category_sn); //call self to find out which row data is this node's childrens.************************************************************

                }
            }
        }
    }
}
