using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TWNewEgg.InternalSendMail.Service
{
    public class BodyGenerator
    {
        public string GenerateBody(string path, object model)
        {
            string templatePath = AppDomain.CurrentDomain.BaseDirectory + "\\" + path;
            StreamReader readStream = new StreamReader(templatePath);
            string template = readStream.ReadToEnd();
            string body = "";
            body = RazorEngine.Razor.Parse(template, model);
            return body;
        }
    }
}
