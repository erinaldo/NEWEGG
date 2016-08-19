using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TWNewEgg.DB.TWSQLDB.Models.ExtModels
{
    public interface IOrderItems
    {
        string Code { get; set; }
        string SalesorderCode { get; set; }
        int ItemID { get; set; }
        int ItemlistID { get; set; }
        int ProductID { get; set; }
        int ProductlistID { get; set; }
        string Name { get; set; }
          decimal Price { get; set; }
        Nullable<decimal> Priceinst { get; set; }
        int Qty { get; set; }
        Nullable<decimal> Pricecoupon { get; set; }
        Nullable<int> RedmtkOut { get; set; }
        Nullable<int> RedmBLN { get; set; }
        Nullable<int> Redmfdbck { get; set; }
          Nullable<int> Status { get; set; }
        string StatusNote { get; set; }
        Nullable<System.DateTime> Date { get; set; }
        string Attribs { get; set; }
        string Note { get; set; }
        Nullable<int> WftkOut { get; set; }
        Nullable<int> WfBLN { get; set; }
        Nullable<int> AdjPrice { get; set; }
        string ActID { get; set; }
        Nullable<int> ActtkOut { get; set; }
        Nullable<int> ProdcutCostID { get; set; }
        string CreateUser { get; set; }
        System.DateTime CreateDate { get; set; }
          Nullable<int> Updated { get; set; }
        Nullable<System.DateTime> UpdateDate { get; set; }
        string UpdateUser { get; set; }
    }
}