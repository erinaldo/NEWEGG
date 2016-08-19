using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TWNewEgg.Website.ECWeb.Models.Q4S
{
    public class Warehouse
    {
        public double AverageCost { get; set; }
        public string CurrencyCode{get;set;}
        public double LastCost{get;set;}
        public int Quantity{get;set;}
        public string WarehouseNumber { get; set; }
    }
}