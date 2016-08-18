using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TWNewEgg.Models.DomainModels.NeweggUSA
{
    /// <summary>
    /// New version for http://www.ows.newegg.com/Products.egg/{itemnumber}/Detail?ispremier=false
    /// </summary>
    public class ProductDetails
    {
        public ProductDetail Basic { get; set; }
    }

    /// <summary>
    /// This class's attribute include old and new version
    /// </summary>
    public class ProductDetail
    {
        public string FinalPrice { get; set; }
        /// <summary>
        /// Old version for http://www.ows.newegg.com/products.egg/{itemnumber}/ProductDetails
        /// </summary>
        public List<ImageGallery> imageGallery { get; set; }

        /// <summary>
        /// New version for http://www.ows.newegg.com/Products.egg/{itemnumber}/Detail?ispremier=false
        /// </summary>
        public List<ImageGallery> ItemImages { get; set; }

        /// <summary>
        /// 取得SellerInfo
        /// </summary>
        public SellerInfo SellerInfo { get; set; }
    }

    public class SellerInfo
    {
        public string SellerName { get; set; }
        public string SellerId { get; set; }
        public int Rating { get; set; }
        public int ReviewCount { get; set; }
        public string PaymentType { get; set; }
    }

    public class ImageGallery
    {
        public string PathSize60 { get; set; }
        public string PathSize125 { get; set; }
        public string PathSize300 { get; set; }
        public string PathSize640 { get; set; }
        public string FullPath { get; set; }
    }
}
