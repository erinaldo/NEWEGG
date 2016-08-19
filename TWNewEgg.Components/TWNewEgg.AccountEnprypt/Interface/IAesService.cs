using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TWNewEgg.AccountEnprypt.Interface
{
    public interface IAesService
    {
        string Enprypt(string source);
        string Decrypt(string source);
        List<string> Enprypts(List<string> sources);
        List<string> Decrypts(List<string> sources);
    }
}
