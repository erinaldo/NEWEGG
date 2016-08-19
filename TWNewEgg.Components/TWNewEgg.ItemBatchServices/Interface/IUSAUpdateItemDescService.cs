using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TWNewEgg.ItemBatchServices.Models;
using TWNewEgg.Models.DomainModels.Item;

namespace TWNewEgg.ItemBatchServices.Interface
{
    public interface IUSAUpdateItemDescService
    {
        ActionResponse<List<DomainResult>> DoWork(UpdateModel updateModel);
    }
}
