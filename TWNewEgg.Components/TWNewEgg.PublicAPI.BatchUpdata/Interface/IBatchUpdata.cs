using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TWNewEgg.PublicApiModels;

namespace TWNewEgg.PublicAPI.BatchUpdata.Interface
{
    public interface IBatchUpdata
    {
        ActionResponse<List<ItemSketchEdit>> UpdateCheck(string emailAccount, string fromIP, string JsonStr, string AuthToken);
        List<ItemSketchEdit> hasItemIDRepeatAddWithSeller(List<ItemSketchEdit> editModel, int sellerid);
        List<ItemSketchEdit> categoryFieldCheck(List<ItemSketchEdit> checkItemSketchModel);
    }
}
