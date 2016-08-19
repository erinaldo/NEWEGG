using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TWNewEgg.ItemBatchServices.Interface;
using TWNewEgg.ItemBatchServices.Models;
using TWNewEgg.ItemServices.Interface;


namespace TWNewEgg.ItemBatchServices.Service
{
    public class USABatchService : IUSABatchService
    {
        private IUSAUpdateItemDescService _iUSAUpdateItemDescService;
        private IUSAUpdateItemPriceService _iUSAUpdateItemPriceService;
        private IUSAUpdateItemStockService _iUSAUpdateItemStockService;
        private IUSAImageService _IUSAImage;
        public USABatchService(IUSAUpdateItemDescService iUSAUpdateItemDescService, IUSAUpdateItemPriceService iUSAUpdateItemPriceService, IUSAUpdateItemStockService iUSAUpdateItemStockService, IUSAImageService IUSAImageService)
        {
            this._iUSAUpdateItemDescService = iUSAUpdateItemDescService;
            this._iUSAUpdateItemPriceService = iUSAUpdateItemPriceService;
            this._iUSAUpdateItemStockService = iUSAUpdateItemStockService;
            this._IUSAImage = IUSAImageService;
        }

        public ActionResponse<List<DomainResult>> SelectDoWork(UpdateModel updateModel)
        {
            ActionResponse<List<DomainResult>> result = new ActionResponse<List<DomainResult>>();
            switch (updateModel.UpdateType)
            {
                case (int)UpdateModel.UpdateTypestatus.UpdateALL:
                    result = this._iUSAUpdateItemStockService.DoWork(updateModel);
                    result.Body.AddRange(this._iUSAUpdateItemPriceService.DoWork(updateModel).Body);
                    result.Body.AddRange(this._iUSAUpdateItemDescService.DoWork(updateModel).Body);

                    break;
                case (int)UpdateModel.UpdateTypestatus.UpdateItemStock:
                    result = this._iUSAUpdateItemStockService.DoWork(updateModel);
                    break;
                case (int)UpdateModel.UpdateTypestatus.UpdateSpec:
                    result = this._iUSAUpdateItemDescService.DoWork(updateModel);
                    break;
                case (int)UpdateModel.UpdateTypestatus.UpdatePrice:
                    result = this._iUSAUpdateItemPriceService.DoWork(updateModel);
                    break;
                case (int)UpdateModel.UpdateTypestatus.UpdateItemPicture:
                    result = this._IUSAImage.DivGetPictureFromNeweggUSAItem(updateModel);
                    break;
                case (int)UpdateModel.UpdateTypestatus.UpdateNullDesc:
                    result = this._iUSAUpdateItemDescService.DoWork(updateModel);
                    break;
         
                default:
                    break;
            }
            return result;
        }
    }
}
