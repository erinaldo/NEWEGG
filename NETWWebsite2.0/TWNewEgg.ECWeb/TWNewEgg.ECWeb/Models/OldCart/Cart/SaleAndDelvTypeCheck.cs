using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TWNewEgg.Website.ECWeb.Models
{
    public class SaleAndDelvTypeCheck
    {
        public SaleAndDelvTypeCheck()
        {
            this.AddItemCheck = false;
            this.DelvTypeCheck = false;
            this.DelvTypeCheck = false;
        }
        public bool AddItemCheck { get; set; }
        public bool DelvTypeCheck { get; set; }
        public bool CashOnDelCheck { get; set; }
    }
}