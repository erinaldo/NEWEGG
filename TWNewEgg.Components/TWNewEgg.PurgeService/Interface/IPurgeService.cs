using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TWNewEgg.PurgeService.Interface
{
    public interface IPurgeService
    {
        string AzurePurge(int purgeNumber = 50);
    }
}
