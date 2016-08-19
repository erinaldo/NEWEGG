using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel;

namespace TWNewEgg.DB.TWBACKENDDB.Models
{
    [Table("SalesRetgood")]
    public partial class SalesRetgood
    {
        public enum status : short
        {

        }

        public SalesRetgood()
        { }

        public SalesRetgood(int status, string SalesRetgoodCode, Models.ExtModels.ReturnDetail rd)
        {
            this.Code = SalesRetgoodCode;
            this.retgoodCode = rd.Retgood_Code;
            this.SupplierID = rd.Retgood_SupplierID;
            this.ProductID = rd.Retgood_ProductID;
            this.Date = rd.Retgood_Date;
            this.FinalDate = rd.Retgood_FinalDate;
            this.Status = rd.Retgood_Status;
            this.Price = rd.Retgood_Price;
            this.Qty = rd.Retgood_Qty;
            this.Cause = rd.Retgood_Cause;
            this.CauseNote = rd.Retgood_CauseNote;
            this.StockOutItemID = rd.Retgood_StockOutItemID;
            this.DealNote = rd.Retgood_DealNote;
            this.ProcessID = rd.Retgood_ProcessID;
            this.CartID = rd.Cart_ID;
            this.ToCompany = rd.Retgood_ToCompany;
            this.ToName = rd.Retgood_ToName;
            this.ToLocation = rd.Retgood_ToLocation;
            this.ToZipcode = rd.Retgood_ToZipcode;
            this.ToADDR = rd.Retgood_ToADDR;
            this.ToPhone = rd.Retgood_ToPhone;
            this.ScmenaBled = rd.Retgood_ScmenaBled;
            this.ScmNote = rd.Retgood_ScmNote;
            this.ShpCode = rd.Retgood_ShpCode;
            this.ShpEXPFile = rd.Retgood_ShpEXPFile;
            this.SysDate = rd.Retgood_SysDate;
            this.FreightalLocation = rd.Retgood_FreightalLocation;
            this.REShipping = rd.Retgood_REShipping;
            this.TaxCost = rd.Retgood_TaxCost;
            this.ASNNumber = rd.Retgood_ASNNumber;
            this.SendStatus = rd.Retgood_SendStatus;
            this.SendDate = rd.Retgood_SendDate;
            this.InventoryStatus = rd.Retgood_InventoryStatus;
            this.InventoryStatusDate = rd.Retgood_InventoryStatusDate;
            this.InventoryStatusUser = rd.Retgood_InventoryStatusUser;
            this.ProductStatus = rd.Retgood_ProductStatus;

        }

        [Key]
        public int ID { get; set; }
        public string Code { get; set; }
        public string retgoodCode { get; set; }
        public Nullable<int> SupplierID { get; set; }
        public Nullable<int> ProductID { get; set; }
        public Nullable<int> WarehouseID { get; set; }
        public Nullable<System.DateTime> Date { get; set; }
        public Nullable<System.DateTime> FinalDate { get; set; }
        public Nullable<int> Status { get; set; }
        public Nullable<decimal> Price { get; set; }
        public Nullable<int> Qty { get; set; }
        public Nullable<int> Cause { get; set; }
        public string CauseNote { get; set; }
        public Nullable<int> StockOutItemID { get; set; }
        public string DealNote { get; set; }
        public string ProcessID { get; set; }
        public string CartID { get; set; }
        public string ToCompany { get; set; }
        public string ToName { get; set; }
        public string ToLocation { get; set; }
        public string ToZipcode { get; set; }
        public string ToADDR { get; set; }
        public string ToPhone { get; set; }
        public Nullable<int> ScmenaBled { get; set; }
        public string ScmNote { get; set; }
        public string ShpCode { get; set; }
        public string ShpEXPFile { get; set; }
        public Nullable<System.DateTime> SysDate { get; set; }
        public Nullable<int> Updated { get; set; }
        public Nullable<System.DateTime> UpdatedDate { get; set; }
        public string UpdatedUser { get; set; }
        public Nullable<System.DateTime> OnReturnDate { get; set; }
        public Nullable<System.DateTime> FinReturnDate { get; set; }
        public Nullable<System.DateTime> ABNReturndate { get; set; }
        public Nullable<System.DateTime> CancelReturnDate { get; set; }
        public Nullable<int> Declined { get; set; }
        public Nullable<decimal> FreightalLocation { get; set; }
        public Nullable<decimal> REShipping { get; set; }
        public Nullable<decimal> TaxCost { get; set; }
        public Nullable<int> ASNNumber { get; set; }
        public string SendStatus { get; set; }
        public Nullable<System.DateTime> SendDate { get; set; }
        public Nullable<int> InventoryStatus { get; set; }
        public Nullable<System.DateTime> InventoryStatusDate { get; set; }
        public string InventoryStatusUser { get; set; }
        public string Note { get; set; }
        public Nullable<int> ProductStatus { get; set; }
        public Nullable<int> ToSAP { get; set; }
        public string ABNReturnReason { get; set; }
        public string CreateUser { get; set; }
    }
}