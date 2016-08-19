using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Newegg.Mobile.MvcApplication.Models
{
    public class UIRegisterResultInfo
    {
        public int Code { get; set; }
        public string Description { get; set; }
        public UILoginResultInfo Body { get; set; }
    }
}