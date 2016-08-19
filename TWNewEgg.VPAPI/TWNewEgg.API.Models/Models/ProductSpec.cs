using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TWNewEgg.API.Models.Models
{
    #region 存入Product.Spec
    public class ProductSpec
    {
        public List<HtmlSpec> HtmlList { get; set; }
    }

    public class HtmlSpec
    {
        public string SellerProductID { get; set; }
        public string Html { get; set; } 
    }
    #endregion

    #region 存入Model
    public class ProductSpec_Model
    {
        public List<HtmlSpec_Model> HtmlList_Model { get; set; }
    }

    public class HtmlSpec_Model
    {
        public string SellerProductID_Model { get; set; }
        public string Html_Model { get; set; }
    }
    #endregion

    #region 存入Spec
    public class ProductSpec_Spec
    {
        public List<HtmlSpec_Spec> HtmlList_Spec { get; set; }
    }

    public class HtmlSpec_Spec
    {
        public string SellerProductID_Spec { get; set; }
        public string Html_Spec { get; set; }
    }
    #endregion

    #region 存入Features
    public class ProductSpec_Features
    {
        public List<HtmlSpec_Features> HtmlList_Features { get; set; }
    }

    public class HtmlSpec_Features
    {
        public string SellerProductID_Features { get; set; }
        public string Html_Features { get; set; }
    }
    #endregion

    #region 存入Size
    public class ProductSpec_Size
    {
        public List<HtmlSpec_Size> HtmlList_Size { get; set; }
    }

    public class HtmlSpec_Size
    {
        public string SellerProductID_Size { get; set; }
        public string Html_Size { get; set; }
    }
    #endregion
}
