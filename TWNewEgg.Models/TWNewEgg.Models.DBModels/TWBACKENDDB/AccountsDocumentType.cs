using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TWNewEgg.Models.DBModels.TWBACKENDDB
{
    [Table("AccountsDocumentType")]
    public class AccountsDocumentType
    {
        public enum DocTypeEnum
        {
            XQ,
            XD,
            XI,
            XIRMA
        }

        //貨到付款代碼
        public const int PayOnDeliveryCode = 21;
        //海外切貨-美金
        public const int OverSeaBuyOutUSDCode = 32;
        
        [Key]
        public int Code { get; set; }
        public string DocType { get; set; }
        public string Description { get; set; }
    }
}
