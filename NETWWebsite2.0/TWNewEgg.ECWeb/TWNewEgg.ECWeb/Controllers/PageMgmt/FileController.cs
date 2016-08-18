using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TWNewEgg.StorageServices;
using TWNewEgg.StorageServices.Interface;
using TWNewEgg.FileServices.Interface;
using TWNewEgg.Models.ViewModels.FileUpload;

namespace TWNewEgg.ECWeb.Controllers.PageMgmt
{
    public class FileController : Controller
    {
        private IFileService _fileservice;
        private ICloudStorageAdapter _cloudStorageService;
        public FileController(IFileService fileservice, ICloudStorageAdapter cloudStorageService)
        {
            this._fileservice = fileservice;
            this._cloudStorageService = cloudStorageService;
        }

        [HttpPost]
        public ActionResult Upload(UploadModel model)
        {
            var filepath = model.subpath;
            model.subpath = Server.MapPath(filepath);
            model.file = Request.Files[0] ?? model.file;

            this._fileservice.fileUpload(model.file, model.chunk, model.name, model.subpath);
            this._cloudStorageService.Upload(model.subpath);

            return Content("ok");
        }
    }
}
