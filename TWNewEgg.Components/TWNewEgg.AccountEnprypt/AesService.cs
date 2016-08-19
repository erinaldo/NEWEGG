using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TWNewEgg.AccountEnprypt.Core;
using TWNewEgg.AccountEnprypt.Interface;

namespace TWNewEgg.AccountEnprypt
{
    public class AesService : IAesService
    {
        private IAes _aesCode;

        public AesService(IAes aesCode)
        {
            _aesCode = aesCode;
        }

        public string Enprypt(string source)
        {
            return _aesCode.AESenprypt(source);
        }

        public string Decrypt(string source)
        {
            return _aesCode.AESdecrypt(source);
        }

        public List<string> Enprypts(List<string> sources)
        {
            List<string> enprypts = new List<string>();

            for (int i = 0; i < sources.Count; i++)
            {
                enprypts.Add(_aesCode.AESenprypt(sources[i]));
            }

            return enprypts;
        }

        public List<string> Decrypts(List<string> sources)
        {
            List<string> decrypts = new List<string>();

            for (int i = 0; i < sources.Count; i++)
            {
                decrypts.Add(_aesCode.AESdecrypt(sources[i]));
            }

            return decrypts;
        }
    }
}
