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
    public class ShippingAssign
    {
        public ShippingAssign() {
            this.PartShippingCost = 0;
            this.DelvType = "";
            this.CorrectFlag = false;
        }

        public int ItemID { get; set; }
        public decimal PartShippingCost { get; set; }
        public string DelvType { get; set; }
        public bool CorrectFlag { get; set; }
    }
}