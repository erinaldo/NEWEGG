using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TWNewEgg.API.Models
{
    /*---------- add by thisway ----------*/
    /// <summary>
    /// Detail Info (APIModel)
    /// <para>Website Page:Create Items / Detail Info</para>
    /// </summary>
    public class DetailInfo
    {
        #region SellerInfo
        ///// <summary>
        ///// Seller Part #
        ///// <para>DB Form:TWSQLDB.dbo.product</para>
        ///// </summary>
        //public string SellerProductID { get; set; }

        ///// <summary>
        ///// Manufacturer Part # / ISBN
        ///// <para>DB Form:TWSQLDB.dbo.product</para>
        ///// </summary>
        //public string MenufacturePartNum { get; set; }

        ///// <summary>
        ///// UPC
        ///// <para>DB Form:TWSQLDB.dbo.product</para>
        ///// </summary>
        //public string UPC { get; set; }
        #endregion

        #region Post List Data
        /// <summary>
        /// Product Spec's Model Data(SpecItem + Title + Description)
        /// <para>DB Form:No DB</para>
        /// </summary>
        public List<string> ProductSpec_Model { get; set; }

        /// <summary>
        /// Product Spec's Spec Data(SpecItem + Title + Description)
        /// <para>DB Form:No DB</para>
        /// </summary>
        public List<string> ProductSpec_Spec { get; set; }

        /// <summary>
        /// Product Spec's Features Data(SpecItem + Title + Description)
        /// <para>DB Form:No DB</para>
        /// </summary>
        public List<string> ProductSpec_Features { get; set; }

        /// <summary>
        /// Product Spec's Size Data(SpecItem + Title + Description)
        /// <para>DB Form:No DB</para>
        /// </summary>
        public List<string> ProductSpec_Size { get; set; }
        #endregion

        #region Post single Data
        /// <summary>
        /// 名稱  ：UserID(使用者ID)
        /// DB位置
        /// 說明  ：
        /// </summary>
        public int UserID { get; set; }
        #endregion

        #region Mark_Real DB Data
        ///// <summary>
        ///// Product's ID(item's ID)
        ///// <para>DB Form:TWSELLERPORTALDB.dbo.Seller_Charge</para>
        ///// </summary>
        //public int Product_id { get; set; }

        ///// <summary>
        ///// Product's Spec(Model = 1, Spec = 2, Features = 3, Size = 4)
        ///// <para>DB Form:TWSELLERPORTALDB.dbo.Seller_Charge</para>
        ///// </summary>
        //public int SpecItem { get; set; }

        ///// <summary>
        ///// Product Spec's Title
        ///// <para>DB Form:TWSELLERPORTALDB.dbo.Seller_Charge</para>
        ///// </summary>
        //public string Title { get; set; }

        ///// <summary>
        ///// Product Spec's content
        ///// <para>DB Form:TWSELLERPORTALDB.dbo.Seller_Charge</para>
        ///// </summary>
        //public string Description { get; set; }
        #endregion

        #region 存入Product.Spec
        public TWNewEgg.API.Models.Models.ProductSpec productSpecList { get; set; }
        #endregion
    }
    /*---------- end by thisway ----------*/
}
