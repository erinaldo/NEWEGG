using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TWNewEgg.ItemBatchServices.Interface;
using TWNewEgg.ItemRepoAdapters.Interface;
using TWNewEgg.Models.DBModels.TWSQLDB;
using TWNewEgg.Framework.AutoMapper;
using TWNewEgg.ItemRepoAdapters;
using TWNewEgg.ItemBatchServices.Models;
using TWNewEgg.NeweggUSARequestServices.Interface;
using TWNewEgg.NeweggUSARequestServices.Models;
using TWNewEgg.Models.DBModels.TWSQLDB;

namespace TWNewEgg.ItemBatchServices.Service
{
    public class USAImageService : IUSAImageService
    {
        private IImageUrlReferenceForProductRepoAdapter _ImageUrlReferenceForProduct;
       private IImageUrlReferenceRepoAdapter _ImageUrlReferenceRepoAdapter;
       private IItemRepoAdapter _itemRepoAdapter;
       private IProductRepoAdapter _ProductRepoAdapter;
  
       public USAImageService(IImageUrlReferenceRepoAdapter ImageUrlReferenceRepoAdapter, IItemRepoAdapter itemRepoAdapter, IProductRepoAdapter ProductRepoAdapter)
       {
           this._ImageUrlReferenceRepoAdapter = ImageUrlReferenceRepoAdapter;
           this._itemRepoAdapter = itemRepoAdapter;
           this._ProductRepoAdapter = ProductRepoAdapter;
       
       }
       public ActionResponse<List<DomainResult>> DivGetPictureFromNeweggUSAItem(UpdateModel UpdateModel) 
       {
           List<int> Itemlists = new List<int>();
           List<int> Productlists = new List<int>();
           Itemlists = UpdateModel.Itemlist;
           Productlists = UpdateModel.ProductList;
                  int timenumber = 0;
                  int itemnumber = 0;
                  int skipnumber = 20;
                  if (UpdateModel.UpdateListType == (int)UpdateModel.UpdateListTypestatus.Itemlist)
                  {

                      itemnumber = Itemlists.Count;
                  }
                  else if (UpdateModel.UpdateListType == (int)UpdateModel.UpdateListTypestatus.ProductList)
                  {
                      itemnumber = Productlists.Count();
                  
                  }
             
                   ActionResponse<List<DomainResult>> result = new ActionResponse<List<DomainResult>>();
                   result.Body = new List<DomainResult>();
                   result.IsSuccess = false;
                   while ((timenumber * skipnumber) < itemnumber)
                   {
                       try
                       {
                           List<int> ItemlistsIDstemp = new List<int>();
                           UpdateModel UpdateModeltemp = new Models.UpdateModel();
                           if (UpdateModel.UpdateListType == (int)UpdateModel.UpdateListTypestatus.Itemlist)
                           {
                               ItemlistsIDstemp = Itemlists.Skip(timenumber * skipnumber).Take(skipnumber).ToList();
                               ItemlistsIDstemp = ItemlistsIDstemp.Distinct().ToList();
                               UpdateModeltemp.Itemlist = ItemlistsIDstemp;

                           }
                           else if (UpdateModel.UpdateListType == (int)UpdateModel.UpdateListTypestatus.ProductList)
                           {
                               ItemlistsIDstemp = Productlists.Skip(timenumber * skipnumber).Take(skipnumber).ToList();
                               ItemlistsIDstemp = ItemlistsIDstemp.Distinct().ToList();
                               UpdateModeltemp.ProductList = ItemlistsIDstemp;
                           }

                           UpdateModeltemp.ThreadingNum = UpdateModel.ThreadingNum;
                           UpdateModeltemp.UpdateListType = UpdateModel.UpdateListType;
                           UpdateModeltemp.UpdateType = UpdateModel.UpdateType;
                           UpdateModeltemp.UpdateUser = UpdateModel.UpdateUser;

                           if (UpdateModel.UpdateListType == (int)UpdateModel.UpdateListTypestatus.Itemlist && ItemlistsIDstemp.Count() > 0)
                           {
                               ActionResponse<List<DomainResult>> GetPictureFromNeweggUSA = GetPictureFromNeweggUSAItem(UpdateModeltemp);
                               result.Body.AddRange(GetPictureFromNeweggUSA.Body);
                               result.IsSuccess = true;
                               result.Msg += "執行成功";
                           }
                           else if (UpdateModel.UpdateListType == (int)UpdateModel.UpdateListTypestatus.ProductList && ItemlistsIDstemp.Count() > 0)
                           {
                               ActionResponse<List<DomainResult>> GetPictureFromNeweggUSA = GetPictureFromNeweggUSAItem(UpdateModeltemp);
                               result.Body.AddRange(GetPictureFromNeweggUSA.Body);
                               result.IsSuccess = true;
                               result.Msg += "執行成功";


                           }
                           else
                           {
                               result.IsSuccess = true;
                               result.Msg += "無需要執行資料";

                           }
                       }
                       catch (Exception e)
                       {
                           result.IsSuccess = false;
                           result.Msg += "Ex:" + GetExceptionMessage(e);
                          
                       
                       }
                       timenumber++;
                   }
                   return result;
       }
       public ActionResponse<List<DomainResult>> GetPictureFromNeweggUSAItem(UpdateModel UpdateModel)
       {
           ActionResponse<List<DomainResult>> result = new ActionResponse<List<DomainResult>>();
           result.Body = new List<DomainResult>();
           result.IsSuccess = false;
           List<int> ItemIDs = new List<int>();
           List<int> ProductID = new List<int>();
           List<DomainResult> DomainResultDetail = new List<DomainResult>();
           if (UpdateModel.UpdateListType == (int)UpdateModel.UpdateListTypestatus.Itemlist)
           {
               if (UpdateModel.Itemlist != null && UpdateModel.Itemlist.Count > 0)
               {





                   var Itemcount = 0;

                   foreach (var ItemlistDetail in UpdateModel.Itemlist)
                   {

                       Itemcount++;
                       // List<Product> Productlist = new List<TWNewEgg.Models.DBModels.TWSQLDB.Product>();
                       //List<Item> Itemlist = new List<TWNewEgg.Models.DBModels.TWSQLDB.Item>();
                       List<ImageUrlReference> ImageUrlReferenceDetail = new List<TWNewEgg.Models.DBModels.TWSQLDB.ImageUrlReference>();
                       DomainResult DomainResulttempforItem = new DomainResult();
                       /*只有不存在的才要新增*/
                       if (ExiteUSAImage(ItemlistDetail) == "false")
                       {
                           if (UpdateModel.UpdateUser == null || UpdateModel.UpdateUser == "")
                           {
                               UpdateModel.UpdateUser = "UpdateUser by ItemImageUpdate";

                           }




                           ProductDetail USAImage = new ProductDetail();
                           List<Item> resaults = _itemRepoAdapter.GetAll().Where(x => x.ID == ItemlistDetail).ToList();
                           INeweggRequest INeweggRequest = new TWNewEgg.NeweggUSARequestServices.Services.NeweggRequest();



                           foreach (var resault in resaults)
                           {
                               List<Product> Productlist = this._ProductRepoAdapter.GetAll().Where(x => x.ID == resault.ProductID).ToList();
                               if (Productlist != null)
                               {
                                   foreach (var resaultforproduct in Productlist)
                                   {
                                       if (resault.Itemnumber == null || resault.Itemnumber.Trim() == "")
                                       {

                                           try
                                           {

                                               USAImage = INeweggRequest.GetProductDetail(resaultforproduct.SellerProductID);

                                           }
                                           catch (Exception ex)
                                           {
                                               DomainResulttempforItem.Log += "Ex:" + GetExceptionMessage(ex);

                                           }
                                       }
                                       else
                                       {
                                           try
                                           {
                                               USAImage = INeweggRequest.GetProductDetail(resault.Itemnumber);

                                           }
                                           catch (Exception ex)
                                           {
                                               DomainResulttempforItem.Log += "Ex:" + GetExceptionMessage(ex);

                                           }

                                       }
                                       int i = 1;
                                       if (USAImage != null)
                                       {
                                           if (USAImage.imageGallery != null)
                                           {
                                               foreach (var USAImageDetail in USAImage.imageGallery)
                                               {
                                                   //resault.PicStart = 1;
                                                   //resaultforproduct.PicStart = 1;

                                                   //同步美蛋商品圖片時，TWSQLDB_PRD.dbo.item不記錄圖片數量
                                                   resault.PicStart = 0;
                                                   resaultforproduct.PicStart = 0;

                                                   Type USAImagetype = typeof(ImageGallery);
                                                   List<string> USAImageProps = USAImagetype.GetProperties().Select(p => p.Name).ToList();
                                                   Dictionary<string, string> ImageUrl = new Dictionary<string, string>();
                                                   ImageUrl.Add("PathSize125", USAImageDetail.PathSize125 ?? "");
                                                   ImageUrl.Add("PathSize300", USAImageDetail.PathSize300 ?? "");
                                                   ImageUrl.Add("PathSize60", USAImageDetail.PathSize60 ?? "");
                                                   ImageUrl.Add("PathSize640", USAImageDetail.PathSize640 ?? "");
                                                   foreach (var USAImageProp in USAImageProps)
                                                   {


                                                       string Size = USAImageProp.Replace("PathSize", "");
                                                       if (USAImageProp.Contains("FullPath") == true)
                                                       {

                                                       }

                                                       else
                                                       {
                                                           ImageUrlReference ImageUrlReference = new ImageUrlReference();

                                                           ImageUrlReference.ItemID = resault.ID;

                                                           ImageUrlReference.ImageUrl = ImageUrl[USAImageProp];
                                                           ImageUrlReference.SizeIndex = i;
                                                           ImageUrlReference.Size = Int32.Parse(Size);
                                                           ImageUrlReference.CreateDate = DateTime.Now;
                                                           ImageUrlReference.CreateUser = UpdateModel.UpdateUser;
                                                           ImageUrlReferenceDetail.Add(ImageUrlReference);


                                                       }


                                                   }
                                                   DomainResult DomainResulttemp = new DomainResult();
                                                   try
                                                   {
                                                       //resault.PicEnd = i;
                                                       //resaultforproduct.PicEnd = i;

                                                       //同步美蛋商品圖片時，TWSQLDB_PRD.dbo.item不記錄圖片數量
                                                       resault.PicEnd = 0;
                                                       resaultforproduct.PicEnd = 0;

                                                       DomainResulttemp.IsSuccess = true;
                                                       DomainResulttemp.ItemID = resault.ID;
                                                       DomainResulttemp.Log += resault.ID.ToString() + "更新成功";
                                                   }
                                                   catch (Exception exception)
                                                   {
                                                       DomainResulttemp.Log += "Ex:" + GetExceptionMessage(exception);

                                                   }
                                                   DomainResultDetail.Add(DomainResulttemp);
                                                   i++;
                                               }
                                               // Itemlist.Add(resault);
                                               // Productlist.Add(resaultforproduct);

                                               DomainResulttempforItem.IsSuccess = true;
                                               DomainResulttempforItem.ItemID = ItemlistDetail;
                                               DomainResulttempforItem.Log += "更新成功完畢";
                                           }
                                           else
                                           {
                                               DomainResulttempforItem.ItemID = ItemlistDetail;
                                               DomainResulttempforItem.Log += "美蛋無資料 imageGallery,無法新增";

                                           }
                                       }
                                       else
                                       {
                                           DomainResulttempforItem.ItemID = ItemlistDetail;
                                           DomainResulttempforItem.Log += "美蛋無資料,無法新增";

                                       }



                                   }
                                   if (Productlist != null && Productlist.Count() != 0)
                                   {
                                       try
                                       {
                                           _ProductRepoAdapter.UpdateMany(Productlist);
                                           System.Threading.Thread.Sleep(100);
                                       }
                                       catch (Exception exception)
                                       {
                                           DomainResulttempforItem.Log += "Ex:" + GetExceptionMessage(exception);
                                           DomainResulttempforItem.ItemID = ItemlistDetail;

                                       }
                                   }
                               }

                               //  _ImageUrlReferenceRepoAdapter.AddImageUrlReference(ItemlistDetail);
                               if (ImageUrlReferenceDetail != null && ImageUrlReferenceDetail.Count() != 0)
                               {
                                   try
                                   {
                                       _ImageUrlReferenceRepoAdapter.AddManyImageUrlReference(ImageUrlReferenceDetail);
                                       System.Threading.Thread.Sleep(100);
                                   }
                                   catch (Exception exception)
                                   {
                                       DomainResulttempforItem.Log += "Ex:" + GetExceptionMessage(exception);
                                       DomainResulttempforItem.ItemID = ItemlistDetail;

                                   }
                               }

                               if (resaults != null && resaults.Count() != 0)
                               {
                                   try
                                   {
                                       _itemRepoAdapter.UpdateItemList(resaults);
                                       System.Threading.Thread.Sleep(100);
                                   }
                                   catch (Exception exception)
                                   {
                                       DomainResulttempforItem.Log += "Ex:" + GetExceptionMessage(exception);
                                       DomainResulttempforItem.ItemID = ItemlistDetail;

                                   }
                               }
                           }
                       }
                       else
                       {
                           DomainResulttempforItem.Log += "Ex: ItemID:" + ItemlistDetail + "已存在";
                           DomainResulttempforItem.ItemID = ItemlistDetail;

                           DomainResultDetail.Add(DomainResulttempforItem);
                       }
                       while (Itemcount == 30)
                       {
                           Itemcount = 0;
                           System.Threading.Thread.Sleep(1000);
                       }
                       DomainResultDetail.Add(DomainResulttempforItem);

                   }
                   result.IsSuccess = true;
                   result.Body = DomainResultDetail;
                   result.Msg = "更新成功完畢";


               }
               else
               {
                   result.Msg = "Ex: ItemModel為空的";
                   result.Body = DomainResultDetail;

               }

           }
           else
           {
               if (UpdateModel.ProductList != null && UpdateModel.ProductList.Count > 0)
               {
                   var Itemcount = 0;
                   ProductID = _ProductRepoAdapter.GetAll().Where(x => UpdateModel.ProductList.Contains(x.ID)).Select(x => x.ID).Distinct().ToList();
                   foreach (var ProductlistDetail in ProductID)
                   {
                       Itemcount++;
                       List<ImageUrlReferenceForProduct> ImageUrlReferenceDetail = new List<TWNewEgg.Models.DBModels.TWSQLDB.ImageUrlReferenceForProduct>();
                       //List<Product> Productlist = new List<TWNewEgg.Models.DBModels.TWSQLDB.Product>();
                       DomainResult DomainResulttempforProduct = new DomainResult();
                       /*只有不存在的才要新增*/
                       if (ExiteUSAImagefroProduct(ProductlistDetail) == "false")
                       {

                           if (UpdateModel.UpdateUser == null || UpdateModel.UpdateUser == "")
                           {
                               UpdateModel.UpdateUser = "UpdateUser by ItemImageUpdateProduct";

                           }

                           ProductDetail USAImage = new ProductDetail();
                           List<Product> Productlist = _ProductRepoAdapter.GetAll().Where(x => x.ID == ProductlistDetail).ToList();
                           foreach (var resault in Productlist)
                           {
                               INeweggRequest INeweggRequest = new TWNewEgg.NeweggUSARequestServices.Services.NeweggRequest();
                               if (resault.SellerProductID == null || resault.SellerProductID.Trim() == "")
                               {
                                   DomainResulttempforProduct.Log += "Ex:無美蛋號碼";


                               }
                               else
                               {
                                   try
                                   {
                                       USAImage = INeweggRequest.GetProductDetail(resault.SellerProductID);

                                   }
                                   catch (Exception ex)
                                   {
                                       DomainResulttempforProduct.Log += "Ex:" + GetExceptionMessage(ex);

                                   }

                               }
                               int i = 1;
                               if (USAImage != null)
                               {
                                   if (USAImage.imageGallery != null)
                                   {
                                       foreach (var USAImageDetail in USAImage.imageGallery)
                                       {
                                           //resault.PicStart = 1;

                                           //同步美蛋商品圖片時，TWSQLDB_PRD.dbo.item不記錄圖片數量
                                           resault.PicStart = 0;

                                           Type USAImagetype = typeof(ImageGallery);
                                           List<string> USAImageProps = USAImagetype.GetProperties().Select(p => p.Name).ToList();
                                           Dictionary<string, string> ImageUrl = new Dictionary<string, string>();
                                           ImageUrl.Add("PathSize125", USAImageDetail.PathSize125 ?? "");
                                           ImageUrl.Add("PathSize300", USAImageDetail.PathSize300 ?? "");
                                           ImageUrl.Add("PathSize60", USAImageDetail.PathSize60 ?? "");
                                           ImageUrl.Add("PathSize640", USAImageDetail.PathSize640 ?? "");
                                           foreach (var USAImageProp in USAImageProps)
                                           {
                                               string Size = USAImageProp.Replace("PathSize", "");
                                               if (USAImageProp.Contains("FullPath") == true)
                                               {

                                               }
                                               else
                                               {
                                                   ImageUrlReferenceForProduct ImageUrlReferenceForProduct = new ImageUrlReferenceForProduct();

                                                   ImageUrlReferenceForProduct.ProductID = resault.ID;
                                                   ImageUrlReferenceForProduct.ImageUrl = ImageUrl[USAImageProp];
                                                   ImageUrlReferenceForProduct.SizeIndex = i;
                                                   ImageUrlReferenceForProduct.Size = Int32.Parse(Size);
                                                   ImageUrlReferenceForProduct.CreateDate = DateTime.Now;
                                                   ImageUrlReferenceForProduct.CreateUser = UpdateModel.UpdateUser;
                                                   ImageUrlReferenceDetail.Add(ImageUrlReferenceForProduct);


                                               }
                                           }
                                           DomainResult DomainResulttemp = new DomainResult();
                                           try
                                           {
                                               //resault.PicEnd = i;

                                               //同步美蛋商品圖片時，TWSQLDB_PRD.dbo.item不記錄圖片數量
                                               resault.PicEnd = 0;

                                               //Productlist.Add(resault);



                                               DomainResulttemp.IsSuccess = true;
                                               DomainResulttemp.ItemID = resault.ID;
                                               DomainResulttemp.Log += resault.ID.ToString() + "更新成功";
                                           }
                                           catch (Exception exception)
                                           {
                                               DomainResulttemp.ItemID = resault.ID;
                                               DomainResulttemp.Log += GetExceptionMessage(exception);
                                           }
                                           DomainResultDetail.Add(DomainResulttemp);
                                           i++;
                                       }

                                       //Productlist.Add(resault);
                                       DomainResulttempforProduct.IsSuccess = true;
                                       DomainResulttempforProduct.ProductID = ProductlistDetail;
                                       DomainResulttempforProduct.Log += "更新成功完畢";
                                       DomainResultDetail.Add(DomainResulttempforProduct);
                                   }
                                   else
                                   {
                                       DomainResulttempforProduct.ProductID = ProductlistDetail;
                                       DomainResulttempforProduct.Log += "美蛋無資料 imageGallery,無法新增";
                                   }
                               }
                               else
                               {
                                   DomainResulttempforProduct.ProductID = ProductlistDetail;
                                   DomainResulttempforProduct.Log += "美蛋無資料,無法新增";

                               }
                           }
                           if (ImageUrlReferenceDetail != null && ImageUrlReferenceDetail.Count() != 0)
                           {
                               try
                               {
                                   _ImageUrlReferenceForProduct.AddManyImageUrlReferenceForProduct(ImageUrlReferenceDetail);
                                   System.Threading.Thread.Sleep(100);
                               }
                               catch (Exception exception)
                               {
                                   DomainResulttempforProduct.Log += "Ex:" + GetExceptionMessage(exception);
                                   DomainResulttempforProduct.ProductID = ProductlistDetail;

                               }
                           }
                           if (Productlist != null && Productlist.Count() != 0)
                           {
                               try
                               {
                                   _ProductRepoAdapter.UpdateMany(Productlist);
                                   System.Threading.Thread.Sleep(100);
                               }
                               catch (Exception exception)
                               {
                                   DomainResulttempforProduct.Log += "Ex:" + GetExceptionMessage(exception);
                                   DomainResulttempforProduct.ProductID = ProductlistDetail;

                               }
                           }
                           //try
                           //{
                           //   _ImageUrlReferenceForProduct.AddManyImageUrlReferenceForProduct(ImageUrlReferenceDetail);
                           //}
                           //catch (Exception exception)
                           //{
                           //    DomainResulttempforProduct.Log += "Ex:" + GetExceptionMessage(exception);
                           //    DomainResulttempforProduct.ItemID = ProductlistDetail;

                           //}
                           //try
                           //{
                           //    _ProductRepoAdapter.UpdateMany(Productlist);
                           //}
                           //catch (Exception exception)
                           //{
                           //    DomainResulttempforProduct.Log += "Ex:" + GetExceptionMessage(exception);
                           //    DomainResulttempforProduct.ItemID = ProductlistDetail;


                           //}
                       }
                       else
                       {
                           DomainResulttempforProduct.Log += "Ex: ItemID:" + ProductlistDetail + "已存在";

                           DomainResulttempforProduct.ProductID = ProductlistDetail;

                           DomainResultDetail.Add(DomainResulttempforProduct);
                       }


                   }


                   result.IsSuccess = true;
                   result.Body = DomainResultDetail;
                   result.Msg = "";
               }
               else
               {
                   result.Msg = "Ex: ItemModel為空的";
                   result.Body = DomainResultDetail;
               }

           }

           return result;


       }
       private string GetExceptionMessage(System.Exception ex)
       {
           string errorMessage = string.Empty;

           if (ex.Message.IndexOf("See the inner exception for details.") != -1)
           {

               errorMessage = ex.InnerException.StackTrace;

               if (errorMessage.IndexOf("See the inner exception for details.") != -1)
               {
                   errorMessage = GetExceptionMessage(ex.InnerException);
               }
           }
           else
           {
               errorMessage = ex.Message;
               errorMessage += ex.StackTrace;
           }

           return errorMessage;
       }
       public void GetPictureFromNeweggUSA(UpdateModel UpdateModel) 
       {
       
       }
       public string ExiteUSAImage(int ItemID) 
       {

           try
           {
               var resault = _ImageUrlReferenceRepoAdapter.GetAll().Where(x => x.ItemID == ItemID);
               if (resault != null &&resault.Count()!=0)
               {
                   return "true";
               }
               return "false";
           }
           catch (Exception ex)
           {
               return "false";
              
           
           }
       
       }
       public string ExiteUSAImagefroProduct(int ProductID)
       {

           try
           {
               var resault = _ImageUrlReferenceForProduct.GetAll().Where(x => x.ProductID == ProductID);
               if (resault != null)
               {
                   return "true";
               }
               return "false";
           }
           catch (Exception ex)
           {
              
               return "false";
           }

       }
    }
}
