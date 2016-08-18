using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel;

namespace TWNewEgg.DB.TWBACKENDDB.Models
{
    [Table("Sap_BapiAccDocument_DocHeader")]
    public class Sap_BapiAccDocument_DocHeader
    {
        public Sap_BapiAccDocument_DocHeader()
        {
            this.BUS_ACT = "RFBU";
            this.USERNAME = "SAPRFC01";
            this.COMP_CODE = "3101";
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int SN { get; set; }
        public string TransactionType { get; set; } // IPP 文档类型， 如普通PO是P0； 供应商发票是IV等等。
        public string TransactionID { get; set; } // 对应 IPP OrderNumber，普通PO 这里就是PO#+Batch#， 供应商发票这里就是发票流水号
        public string BUS_ACT { get; set; } // 对我们这里所有的Interface， 这个字段取值都是 “RFBU”
        public string USERNAME { get; set; } // SAP 用户名， 这里缺省都放 SAPRFC01
        public string HEADER_TXT { get; set; } // SAP 文档的标题栏， 通常我们会在这里说明文档的类型/对应IPP OrderNumber等等， 比如 “发票-(联强824/1781)”
        public string COMP_CODE { get; set; } // Company_Code 公司代碼
        public Nullable<System.DateTime> DOC_DATE { get; set; } // 文档日期， 除非特别指定， 通常这个字段和下面的PSTNG_DATE都是取OrderDate
        public Nullable<System.DateTime> PSTNG_DATE { get; set; } // 文档Posting 日期， 这个对应我们的OrderDate
        public Nullable<System.DateTime> TRANS_DATE { get; set; } // 文档传输日期， 这个字段目前留空。
        public string FISC_YEAR { get; set; } // 这个字段就是PSTNG_DATE 这个日期的4位年份
        public string FIS_PERIOD { get; set; } // 这个字段就是PSTNG_DATE 这个日期的月份
        public string DOC_TYPE { get; set; } // 这是SAP 系统内这个文档的文档类型， 比如供应商发票是XV；
        public string DOC_NUMBER { get; set; } // 这是SAP 系统内这个文档的文档类型， 比如供应商发票是XV；
        public string REF_DOC_NO { get; set; } // 这里放OrderNumber
        public string AC_DOC_NO { get; set; } // 留空， 这个字段对应SAP DocumentNo， Interface 在SAP 创建成功以后会回写这个字段
        public string C_STATUS { get; set; } // 缺省值为“”； 目前这个字段没有使用。
        public string P_STATUS { get; set; } // 文档状态； Interface 程序会抓取P_Status = "" 的文档送到SAP。
        public Nullable<System.DateTime> P_TIME { get; set; } // 处理时间， 这个字段有缺省值GetDate(), 不需要去显式的更新这个字段。
        public Nullable<int> TransactionNumber { get; set; } // Identity 字段， 不需要更新。
        // ACCOUNTRECEIVABLE
        public Nullable<int> Receivalbe_ItemNOACC { get; set; }
        public string Receivable_Customer { get; set; }
        public string Receivable_BusArea { get; set; }
        public string Receivable_Pmnttrms { get; set; }
        public string Receivable_AllocNmbr { get; set; }
        public string Receivable_ItemText { get; set; }
        public string Receivable_SPGLIND { get; set; }
        // ACCOUNTPAYABLE
        public Nullable<int> Payable_ItemNOACC { get; set; }
        public string Payable_VendorNO { get; set; }
        public string Payable_BusArea { get; set; }
        public string Payable_Pmnttrms { get; set; }
        public string Payable_AllocNmbr { get; set; }
        public string Payable_ItemText { get; set; }
        public string Payable_SPGLIND { get; set; }
    }
}
