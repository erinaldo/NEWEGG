using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TWNewEgg.CacheGenerateServices.Interface
{
    public interface IXMLGenerate
    {
        string ChooseFunction(string actionType, string jsonValue);
    }
}
