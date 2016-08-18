using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Web.UI.WebControls;

namespace TWNewEgg.Website.ECWeb.Models
{

    public class Bankinfo
    {
        public string Bank_id { get; set; }
        public string Bank_name { get; set; }
    }
}