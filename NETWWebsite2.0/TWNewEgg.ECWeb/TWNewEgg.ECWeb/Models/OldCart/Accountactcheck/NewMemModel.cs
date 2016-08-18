using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TWNewEgg.Website.ECWeb.Models
{
    //client 端使用
    public class NewMemModel
    {
        public DateTime? StatusDate { get; set; }
        public string Status { get; set; }
        public string Phone { get; set; }
        public int FailCount { get; set; }
        public string SubmitButton { get; set; }
    }
}