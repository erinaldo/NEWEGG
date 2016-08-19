using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TWNewEgg.DB;

namespace TWNewEgg.API.Models
{
    public class VM_ItemInventoryAlertInfo 
    {     
        public int ID { get; set; }
       
        public int SellerID { get; set; }
        
        public int ProductID { get; set; }
       
        public string Name { get; set; }
        
        public string NameTW { get; set; }
      
        public string UPC { get; set; }
       
        public string SellerProductID { get; set; }
        
        public int ManufactureID { get; set; }

        public string ManufactureName { get; set; }//頁面顯示製造商名稱
      
        public string ManufacturePartNum { get; set; }
        
        public int Condition { get; set; }

        public string ItemCondition { get; set; }
    
        public string ShipType { get; set; }
     
        public int Qty { get; set; }
      
        public int QtyReg { get; set; }
      
        public int SafeQty { get; set; }

        public string Status { get; set; }
   
        public int? InUserID { get; set; }
      
        public int? UpdateUserID { get; set; }

    
        public DateTime? UpdateDate { get; set; }

    
        public DateTime? InDate { get; set; }
     
    }

    public class DeleteItemInventory
    {
        public int ProductID { get; set; }

        public string SellerProductID { get; set; }

        public int ManufactureID { get; set; }

        public int Condition { get; set; } 

        public int SellerID { get; set; }

        public string ShipType { get; set; }
    }

    

    public class InventoryAlertSearchModel
    {     
        public string KeyWord { get; set; }
        public int SellerID { get; set; }
        public PageInfo PageInfo { get; set; }
        public string SortType { get; set; }
        public string SortField { get; set; }
    }

    public class ItemInventoryMailInfo
    {
        public string UserEmail { get; set; }

        public string UserName { get; set; }

        public string ProductName { get; set; }
    }

    

    /// <summary>
    /// Type of save error
    /// </summary>
    public enum ErrorCheckCode
    {
        /// <summary>
        /// Represents SafeQty
        /// </summary>
        SafeQty,

        /// <summary>
        /// Represents InUserID
        /// </summary>
        InUserID,

        /// <summary>
        /// Repressents ShipType
        /// </summary>
        ShipType,

        /// <summary>
        /// Respressent SellerID
        /// </summary>
        SellerID,

        /// <summary>
        /// Respressent ProductID
        /// </summary>
        ProductID,

        /// <summary>
        /// Respressent ManufactureID
        /// </summary>
        ManufactureID,

        /// <summary>
        /// Respressent Condition
        /// </summary>
        Condition,

        /// <summary>
        /// Respressent UpdateUserID
        /// </summary>
        UpdateUserID
    }
}
