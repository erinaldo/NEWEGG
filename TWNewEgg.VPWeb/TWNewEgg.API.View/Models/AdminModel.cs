using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TWNewEgg.API.View
{
    public class AdminModel
    {
        public int CategotyID { get; set; }
        public string CategotyName { get; set; }
        public int FunctionID { get; set; }
        public string FunctionName { get; set; }
        public string Enable { get; set; }
        public string PurviewType { get; set; }
        public string UserEmail { get; set; }
    }
}