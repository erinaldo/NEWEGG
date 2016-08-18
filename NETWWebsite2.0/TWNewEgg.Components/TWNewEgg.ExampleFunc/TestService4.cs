using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TWNewEgg.ExampleFunc
{
    public class TestService4 : Interface.ITest3, IDisposable
    {
        public string report3()
        {
            return "this is TestService444444444444";
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }
    }
}
