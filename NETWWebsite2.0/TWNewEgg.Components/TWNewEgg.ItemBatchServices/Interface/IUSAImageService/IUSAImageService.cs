using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TWNewEgg.ItemBatchServices.Models;

namespace TWNewEgg.ItemBatchServices.Interface
{
    public interface IUSAImageService
    {
        ActionResponse<List<DomainResult>> DivGetPictureFromNeweggUSAItem(UpdateModel UpdateModel);
    }
}
