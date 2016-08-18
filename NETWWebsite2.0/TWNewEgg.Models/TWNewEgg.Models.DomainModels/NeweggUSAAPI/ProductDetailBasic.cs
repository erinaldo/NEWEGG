using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TWNewEgg.Models.DomainModels.NeweggUSAAPI
{
   

        public class ProductDetailBasic
        {
            public int ProductStockType { get; set; }
            public bool CanAddToCart { get; set; }
            public string AddToCartText { get; set; }
            public bool IsFeaturedItem { get; set; }
            public bool CanAddToWishList { get; set; }
            public bool IsPremierItem { get; set; }
            public bool IsFirstFromAsia { get; set; }
            public int ItemType { get; set; }
            public string ItemNumber { get; set; }
            public string NeweggItemNumber { get; set; }
            public string ParentItemNumber { get; set; }
            public string Title { get; set; }
            public string PromotionText { get; set; }
            public string OriginalPrice { get; set; }
            public string FinalPrice { get; set; }
            public string MappingFinalPrice { get; set; }
            public string SavingText { get; set; }
            public string ShippingText { get; set; }
            public bool IsFreeShipping { get; set; }
            public string RebateText { get; set; }
            public int ItemMapPriceMarkType { get; set; }
            public bool Instock { get; set; }
            public ReviewSummary ReviewSummary { get; set; }
            public bool IsHot { get; set; }
            public SellerInfo SellerInfo { get; set; }
            public ItemBrand ItemBrand { get; set; }
            public List<ItemImages> ItemImages { get; set; }
        }

        public class ReviewSummary
        {
            public string Rating { get; set; }
            public string TotalReviews { get; set; }
        }
        public class SellerInfo
        {
            public string SellerName { get; set; }
            public string SellerId { get; set; }
            public int Rating { get; set; }
            public int ReviewCount { get; set; }
        }
        public class ItemBrand
        {
            public int Code { get; set; }
            public int BrandId { get; set; }
            public string Description { get; set; }
            public string ManufactoryWeb { get; set; }
            public string WebSiteURL { get; set; }
            public bool HasManfactoryLogo { get; set; }
            public string BrandImage { get; set; }
        }
        public class ItemImages
        {
            public string ItemNumber { get; set; }
            public string Title { get; set; }
            public string FullPath { get; set; }
            public string ThumbnailImagePath { get; set; }
            public string SmallImagePath { get; set; }
            public string PathSize35 { get; set; }
            public string PathSize60 { get; set; }
            public string PathSize100 { get; set; }
            public string PathSize125 { get; set; }
            public string PathSize180 { get; set; }
            public string PathSize300 { get; set; }
            public string PathSize640 { get; set; }
        }
   
}
