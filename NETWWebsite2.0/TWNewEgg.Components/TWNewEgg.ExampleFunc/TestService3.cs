using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TWNewEgg.ExampleFunc
{
    public class TestService3 : Interface.ITest3, IDisposable
    {
        public string report3()
        {
            return "this is TestService333333333";
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }
    }
}
