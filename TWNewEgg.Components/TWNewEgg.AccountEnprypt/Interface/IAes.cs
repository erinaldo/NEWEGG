using System;
using System.Collections.Generic;
using System.Linq;

namespace TWNewEgg.AccountEnprypt.Interface
{
    public interface IAes
    {
        string AESenprypt(string source);

        string AESdecrypt(string source);
    }
}
