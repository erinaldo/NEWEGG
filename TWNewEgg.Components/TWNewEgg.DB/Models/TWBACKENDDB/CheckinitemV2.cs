using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TWNewEgg.DB.TWBACKENDDB.Models
{
    [Table("CheckinitemV2")]
    public class CheckinitemV2
    {
        [Key]
        public int ID { get; set; }

        [ForeignKey("checkin")]
        public Nullable<int> CheckinID { get; set; }
        public virtual CheckinV2 checkin { get; set; }

        public string PoCode { get; set; }
        public string PoitemCode { get; set; }
        public Nullable<int> ProductID { get; set; }
        public string SpNO { get; set; }
        public string Attribs { get; set; }
        public int Qty { get; set; }
        public Nullable<int> QtyStckResv { get; set; }
        public Nullable<int> QtypaID { get; set; }
        public decimal Price { get; set; }
        public Nullable<int> TaxType { get; set; }
        public Nullable<int> Parent { get; set; }
        public string Frm { get; set; }
        public Nullable<int> Frmid { get; set; }
        public Nullable<int> AmntpaId { get; set; }
        public Nullable<int> RetID { get; set; }
        public DateTime? StckDate { get; set; }
        public string StckUser { get; set; }
        public DateTime? ScmretcFmDate { get; set; }
        public string DcmRetcfmUser { get; set; }
        public string ScmRetcfmNote { get; set; }
        public DateTime? EraseDate { get; set; }
        public Nullable<int> EraseCause { get; set; }
        public string EraseCauseNote { get; set; }
        public DateTime? CreateDate { get; set; }
        public string CreateUser { get; set; }
        public Nullable<int> Updated { get; set; }
        public DateTime? UpdateDate { get; set; }
        public string UpdateUser { get; set; }
        public string Note { get; set; }
        public string FileName { get; set; }
        public Nullable<decimal> ShippingFee { get; set; }      //2014/01/23 add by Bill
        public Nullable<decimal> TaxandDuty { get; set; }       //2014/01/23 add by Bill
        public Nullable<decimal> CustomsCharges { get; set; }   //2014/01/23 add by Bill
        public Nullable<decimal> TradeServiceCharges { get; set; }   //2014/01/23 add by Bill
    }
}