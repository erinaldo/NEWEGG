using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TWNewEgg.Models.DomainModels.AdditionalItem
{
    public class AdditionCartInput
    {
       
            public enum SaerchKeystatus
            {
                賣場編號 = 0,
                商品編號 = 1,
                賣場名稱 = 2,
                商家商品編號 = 3,
                規格品編號 = 4
            };
            public enum ProductStatusID
            {
                已上架 = 0,
                未上架 = 1,
                保持下架 = 2,
                系統下架 = 3
            };
            /// <summary>
            /// Related to CartType
            /// </summary>
            public enum CartTypeStatus
            {
                全部 = -1,
                無定義 = 0,
                Domestic = 1,
                Internation = 2,
                ChooseAny = 3
            };
            public int? ItemID { get; set; }
            //供應商ID
            public string ManufactorID { get; set; }
            //類別ID
            public List<string> CategoryID { get; set; }
            //關鍵字類型
            public string SaerchKey { get; set; }
            //關鍵字
            public string SaerchWord { get; set; }
            //商品狀態
            public string ProductStatus { get; set; }
            //配送方式
            public string Delvtype { get; set; }
            //選擇加購的型態
            public string CartType { get; set; }

       
    }
}
