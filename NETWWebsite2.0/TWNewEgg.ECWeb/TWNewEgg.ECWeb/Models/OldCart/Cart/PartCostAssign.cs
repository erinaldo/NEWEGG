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
    public class PartCostAssign
    {
        public PartCostAssign()
        {
            this.PartPrice = 0;
            this.PartShippingCost = 0;
            this.PartServiceCost = 0;
            this.PartTaxCost = 0;
            this.DelvType = "";
            this.CorrectFlag = false;
        }

        public int ItemID { get; set; }
        public decimal PartPrice { get; set; }
        public decimal PartShippingCost { get; set; } // 各別運費
        public decimal PartServiceCost { get; set; }  // 各別服務費
        public decimal PartTaxCost { get; set; }      // 各別稅金
        public string DelvType { get; set; }
        public bool CorrectFlag { get; set; }
    }
}