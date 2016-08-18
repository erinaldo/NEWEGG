using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TWNewEgg.ItemBatchServices.Models
{
    public class UpdateModel
    {
        public enum UpdateTypestatus
        {
            UpdateALL = 0,
            UpdateItemStock = 1,
            UpdateSpec = 2,
            UpdatePrice = 3,
            UpdateItemPicture = 4,
            UpdateNullDesc = 5

        }
        public enum UpdateListTypestatus
        {
            Itemlist = 0,
            ProductList = 1,
            AutoItemlist = 10,
            AutoProductList = 11
        }

        public UpdateModel()
        {
            this.Itemlist = new List<int>();
            this.ProductList = new List<int>();
            this.SellerProductIDsList = new List<string>();
            this.UpdateType = 0;
            this.ThreadingNum = 100;
        }

        public List<int> Itemlist { get; set; }
        public List<int> ProductList { get; set; }
        public List<string> SellerProductIDsList { get; set; }
        public int UpdateType { get; set; }
        public int? UpdateListType { get; set; }
        public int ThreadingNum { get; set; }
        public string UpdateUser { get; set; }

    }
}
