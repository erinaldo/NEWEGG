using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.IO;

namespace TWNewEgg.ECService.Controllers
{
    public class EnumerableStreamResult : FileResult
    {
        public IEnumerable<string> Enumerable { get; private set; }
        public Encoding ContentEncoding { get; set; }
        public string FileName { get; private set; }

        const string txtContentType = "text/plain";

        public EnumerableStreamResult(string contentType = txtContentType)
            : base(contentType)
        {

        }

        public EnumerableStreamResult(IEnumerable<string> enumerable, string contentType, string fileName)
            : base(contentType)
        {
            if (enumerable == null)
                throw new ArgumentNullException("enumerable");
            else
                this.Enumerable = enumerable;

            if (string.IsNullOrWhiteSpace(fileName))
                throw new ArgumentNullException("fileName");
            else
                this.FileName = fileName;
        }

        protected override void WriteFile(HttpResponseBase response)
        {
            Stream outputStream = response.OutputStream;
            if (this.ContentEncoding != null)
            {
                response.ContentEncoding = this.ContentEncoding;
            }

            if (response.ContentType != null && this.Enumerable != null)
            {
                switch (response.ContentType.ToLower())
                {
                    case txtContentType:
                        //文字檔下載
                        response.AppendHeader("content-disposition", string.Format("attachment;filename={0}", this.FileName));
                        break;
                }
            }

            if (this.Enumerable != null)
            {
                foreach (var item in Enumerable)
                {
                    //do your stuff here 
                    response.Write(item + System.Environment.NewLine);
                }
            }
            else
                response.Write("檔案不存在。");
        }
    }
}
