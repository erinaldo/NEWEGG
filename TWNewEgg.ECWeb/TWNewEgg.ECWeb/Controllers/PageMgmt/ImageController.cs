using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TWNewEgg.Models.DBModels.TWSQLDB.PageMgmt;

namespace TWNewEgg.ECWeb.Controllers.PageMgmt
{
    public class ImageController : Controller
    {
        //[HttpPost]
        //public ActionResult getAllData(int? topage)
        //{
        //    int page = topage ?? 1;
        //    int take = 20;
        //    int skip = (page - 1) * take;
        //    List<Photo> photos = new List<Photo>();
        //    if (IsAvailable)
        //    {
        //        List<AlbumPhotoPair> albums = _albumDBUtil.getAll("PageMgmt");
        //        photos = _photoDBUtil.getAll(albums[0].album.AlbumID).ToList();
        //    }

        //    int Count = photos.Count();
        //    return Json(new { photos = photos.Skip(skip).Take(take).ToList(), Count = Count, take = take, skip = skip });
        //}

        /// <summary>
        /// 上傳圖片，儲存在圖片的table
        /// </summary>
        /// <param name="img">圖片物件</param>
        /// <returns>所有頁面編輯用的圖片</returns>
        //[HttpPost]
        //public ActionResult uploadImg(ImageObject img)
        //{
        //    ActionResult result = Redirect_To_Login;
        //    List<AlbumPhotoPair> albums = _albumDBUtil.getAll("PageMgmt");
        //    Photo photo = new Photo()
        //    {
        //        AccountName = "PageMgmt",
        //        AlbumID = albums[0].album.AlbumID,
        //        FileName = img.FileName,
        //        Title = img.Title,
        //        Type = 0
        //    };
        //    _photoDBUtil.addPhoto(photo);
        //    List<Photo> photos = _photoDBUtil.getAll(albums[0].album.AlbumID).ToList();
        //    result = Json(photos);

        //    return result;
        //}
    }
}
