using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TWNewEgg.ItemServices.Interface
{
    public interface IItemDeliver
    {
        TWNewEgg.Models.DomainModels.Item.ItemDeliver getItemBalckAndWhite(List<int> itemid, List<int> payTypeid);
    }
}
