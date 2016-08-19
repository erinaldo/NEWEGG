using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TWNewEgg.API.View
{
    public class JsonDataSkechModel
    {
        //商家商品編號, 廠商產品編號, 草稿ID, 產品名稱
        public int searchSketchProperty { get; set; }
        //查詢輸入條件
        public string searchTextSketchProperty { get; set; }
        //製造商
        public int MarkerNameSketchProperty { get; set; }
        //可售數量
        public int StockSketchProperty { get; set; }
        //創建日期
        public int DateSketchProperty { get; set; }
        //開始時間
        public string StartDataSketchProperty { get; set; }
        //結束時間
        public string EndDataSketchProperty { get; set; }
        //主分類
        public int ItemCategorySketchProperty1 { get; set; }
        //誇分類 1
        public int ItemCategorySketchProperty2 { get; set; }
        //誇分類 2
        public int ItemCategorySketchProperty3 { get; set; }
    }
    public class JsonDataSketchPropertyModel
    {
        //商家商品編號, 廠商產品編號, 草稿ID, 產品名稱
        public int searchRequestListProperty { get; set; }
        //查詢輸入條件
        public string searchTextListProperty { get; set; }
        //審核狀態
        public int CheckStatus { get; set; }
        //商品狀態
        public int GoodsStatus { get; set; }
        //製造商
        public int MarkerNameListProperty { get; set; }
        //可售數量
        public int StockListProperty { get; set; }
        //主分類
        public int ItemCategory1ListProperty { get; set; }
        //誇分類 1
        public int ItemCategory2ListProperty { get; set; }
        //誇分類 2
        public int ItemCategory3ListProperty { get; set; }
        //創建日期
        public int DateListProperty { get; set; }
        //開始時間
        public string StartDataListProperty { get; set; }
        //結束時間
        public string EndDataListProperty { get; set; }
        //加購商品
        public int? ShowOrder { get; set; }
    }
}