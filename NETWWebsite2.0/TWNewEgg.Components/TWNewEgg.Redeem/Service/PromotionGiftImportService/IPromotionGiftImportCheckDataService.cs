using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TWNewEgg.Models.DomainModels.Redeem;

namespace TWNewEgg.Redeem.Service.PromotionGiftExportService
{
    public interface IPromotionGiftImportCheckDataService
    {
        ActionResponse<List<PromotionGiftImportCheckData>> CheckImportDate(List<PromotionGiftImportCheckData> dataList);
    }
}
