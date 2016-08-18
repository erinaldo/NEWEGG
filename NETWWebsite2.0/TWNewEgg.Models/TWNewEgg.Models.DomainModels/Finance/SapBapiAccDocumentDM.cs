using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//using TWNewEgg.Models.DBModels.TWBACKENDDB;

namespace TWNewEgg.Models.DomainModels.Finance
{
    public enum DocTypeEnum
    {
        XQ,
        XD,
        XI,
        XIRMA
    }

    public class SapBapiAccDocumentDM
    {
        public DocTypeEnum DocType { get; set; }
        public SapBapiAccDocumentDocHeaderDM DocHeader { get; set; }
        public IEnumerable<SapBapiAccDocumentDocDetailDM> DocDetail { get; set; }
    }

    public class SapBapiAccDocumentDocHeaderDM //: Sap_BapiAccDocument_DocHeader
    {
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

    public class SapBapiAccDocumentDocDetailDM //: Sap_BapiAccDocument_DocDetail
    {
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
