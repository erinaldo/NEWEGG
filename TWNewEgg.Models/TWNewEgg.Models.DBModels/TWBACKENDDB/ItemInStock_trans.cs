using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TWNewEgg.Models.DBModels.TWBACKENDDB
{
    [Table("ItemInStock_trans")]
    public class ItemInStock_trans
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int ID { get; set; }
        public Nullable<int> CheckinID { get; set; }
        //[ForeignKey("iteminstock")]
        public int StckID { get; set; }
        //public virtual ItemInStock iteminstock { get; set; }        
        public Nullable<int> CheckoutID { get; set; }

        public string POCode { get; set; }
        public string SOCode { get; set; }
        public string PoItemCode { get; set; }
        public string SoItemCode { get; set; }
        public Nullable<int> ProductID { get; set; }
        public string SpNO { get; set; }
        public string Attribs { get; set; }

        public int Qty { get; set; }
        public Nullable<int> QtyStckresv { get; set; }
        public Nullable<int> QtypaID { get; set; }

        public decimal Price { get; set; }
        public Nullable<int> TaxType { get; set; }
        public Nullable<int> Parent { get; set; }
        public string FRM { get; set; }
        public Nullable<int> FRMID { get; set; }
        public Nullable<int> AmntpaID { get; set; }
        public Nullable<int> RetID { get; set; }

        public DateTime? StckDate { get; set; }
        public string StckUser { get; set; }
        public DateTime? ScmretcfmDate { get; set; }
        public string Scmretcfmuser { get; set; }
        public string ScmretcfmNote { get; set; }
        public DateTime? EraseDate { get; set; }
        public Nullable<int> EraseCause { get; set; }
        public string EraseCauseNote { get; set; }
        public DateTime? CreateDate { get; set; }
        public Nullable<int> Updated { get; set; }
        public DateTime? UpdateDate { get; set; }
        public string UpdateUser { get; set; }

        public int PreQty { get; set; }
        public decimal PreAVGCost { get; set; }
        public decimal PreTotalCost { get; set; }
        public int AfterQty { get; set; }
        public decimal AfterAVGCost { get; set; }
        public decimal AfterTotalCost { get; set; }       
        
        public decimal Cost { get; set; }        
        
        public Nullable<decimal> PreTotalShippingFee { get; set; }     //2014/01/28 add by Bill
        public Nullable<decimal> PreTotalTaxandDuty { get; set; }      //2014/01/28 add by Bill
        public Nullable<decimal> PreTotalCustomsCharges { get; set; }  //2014/01/28 add by Bill
        public Nullable<decimal> PreTotalServiceCharges { get; set; }  //2014/01/28 add by Bill
        public Nullable<decimal> AfterTotalShippingFee { get; set; }     //2014/01/28 add by Bill
        public Nullable<decimal> AfterTotalTaxandDuty { get; set; }      //2014/01/28 add by Bill
        public Nullable<decimal> AfterTotalCustomsCharges { get; set; }  //2014/01/28 add by Bill
        public Nullable<decimal> AfterTotalServiceCharges { get; set; }  //2014/01/28 add by Bill
    }
}
