using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TWNewEgg.PublicApiModels;

namespace TWNewEgg.PublicAPI.BatchUpdata.Interface
{
    public interface IBatchTempOfficial
    {
        BatchResponse EditBatchUpdate(List<ItemSketchEdit> modelItemEdit, int userid);
    }
}
