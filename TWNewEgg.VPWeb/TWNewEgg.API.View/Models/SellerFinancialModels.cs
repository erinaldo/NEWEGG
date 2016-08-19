using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TWNewEgg.API.Models;
using TWNewEgg.DB.TWSELLERPORTALDB.Models;

namespace TWNewEgg.API.View
{
    public class SellerFinancialModels
    {
        public SellerFinancialModels()
        {
            this.SellerFinancial = new Seller_Financial();
            this.GetRegionListResultList = new List<GetRegionListResult>();
        }

        public Seller_Financial SellerFinancial { get; set; }
        public List<GetRegionListResult> GetRegionListResultList { get; set; }
    }
}