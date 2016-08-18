using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel;

namespace TWNewEgg.DB.TWBACKENDDB.Models
{
    [Table("Sap_BapiAccDocument_DocDetail")]
    public class Sap_BapiAccDocument_DocDetail
    {
        public Sap_BapiAccDocument_DocDetail()
        {
            this.CURRENCY = "TWD";
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int SN { get; set; }
        public string TransactionType { get; set; } // 同Sap_BapiAccDocument_DocHeader的TransactionType
        public string TransactionID { get; set; } // 同Sap_BapiAccDocument_DocHeader的TransactionID， 通常Interface 会用TransactionType+TransactionID 来Join Header表
        public Nullable<int> ITEMNO_ACC { get; set; } // Item 流水号，按照TransactionType+TransactionID 分组，从1开始编号。
        public string ACCT_TYPE { get; set; } // Item的Accounting Type， Mapping 规则会定义这个字段取什么值
        public string GL_ACCOUNT { get; set; } // GL Account Number， Mapping 规则会定义这个字段取什么值,会计科目
        public string CUSTOMER_ID { get; set; } // Customer Number， 仅用于AR类型的数据， 比如SO/RO， PO/供应商发票不写这个字段
        public string VENDOR_ID { get; set; } // VENODR_ID， 仅用于AP类型的数据， 比如PO/供应商发票， SO/RO不写这个字段
        public string ALLOC_NMBR { get; set; } // 这里就是写OrderNumber
        public string ITEM_TEXT { get; set; } // Item的描述性文字， 比如“发票/票未到-(威帆1118/146678-01) ”
        public string BUS_AREA { get; set; } // 这个字段对应Warehouse， Mapping 规则会定义这个字段取什么值
        public string ORDERID { get; set; } // 缺省值为“”； 目前这个字段没有使用。
        public string COSTCENTER { get; set; } // 缺省值为“”； 目前这个字段没有使用。
        public string PROFIT_CTR { get; set; } // 缺省值为“”； 目前这个字段没有使用。
        public Nullable<System.DateTime> VALUE_DATE { get; set; } // 除非特别指定， 就是放OrderDate
        public string PMNTTREMS { get; set; } // pay Terms。缺省值为“”； 目前这个字段没有使用。
        public Nullable<System.DateTime> BLINE_DATE { get; set; } // 除非特别指定， 就是放OrderDate
        public string PYMT_METH { get; set; } // 支付方式，缺省值为“”； 目前这个字段没有使用。
        public string PMNT_BLOCK { get; set; } // 缺省值为“”； 目前这个字段没有使用。
        public string SP_GL_IND { get; set; } // Special GL 标志， 取值为“N” 或者“”，Mapping 规则会定义这个字段取什么值
        public string CURR_TYPE { get; set; } // 缺省值为“”； 目前这个字段没有使用。
        public string CURRENCY { get; set; } // 币种， 目前应该都是写“RMB”
        public Nullable<decimal> AMT_DOCCUR { get; set; } // 金额， Mapping 规则会定义这个字段取什么值
        public Nullable<decimal> EXCH_RATE { get; set; } // 汇率， 缺省值为NULL； 目前这个字段没有使用。
        public string COST_CTR { get; set; } // 支付方式，缺省值为“”； 目前这个字段没有使用。
    }
}
