using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TWNewEgg.BackendService.Interface;

namespace TWNewEgg.API.Controllers
{
    public class JobInterFaceController : Controller
    {
        private IAzureService _azureImageService;
        public JobInterFaceController(IAzureService azureImageService)
        {
            this._azureImageService = azureImageService;
        }
        public JsonResult AzureJobCopYImage(string FromFolderPath, string UpdateUser, string fromSystem = "VendorPortal/AzureController/FolderUploadToAzure")
        {
            List<TWNewEgg.BackendService.Models.ActionResponse<string>> result = new List<TWNewEgg.BackendService.Models.ActionResponse<string>>();
            result = this._azureImageService.FolderUploadToAzure(FromFolderPath, fromSystem, UpdateUser);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

    }
}
