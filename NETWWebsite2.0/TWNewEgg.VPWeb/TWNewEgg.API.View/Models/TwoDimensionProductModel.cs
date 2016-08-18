using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TWNewEgg.API.View
{
    /// <summary>
    /// 規格商品屬性
    /// </summary>
    public class TwoDimensionProductProperty
    {
        /// <summary>
        /// 主項目ID
        /// </summary>
        public int? MainPropertyID { get; set; }

        /// <summary>
        /// 主項目名稱
        /// </summary>
        public string MainPropertyName { get; set; }

        /// <summary>
        /// 次項目ID
        /// </summary>
        public int? SecondPropertyID { get; set; }

        /// <summary>
        /// 次項目名稱
        /// </summary>
        public string SecondPropertyName { get; set; }

        /// <summary>
        /// 主項目內容
        /// </summary>
        public List<MainPropertyValue> MainPropertyValueCell { get; set; }

        public TwoDimensionProductProperty()
        {
            MainPropertyID = null;
            MainPropertyName = null;

            SecondPropertyID = null;
            SecondPropertyName = null;

            MainPropertyValueCell = new List<MainPropertyValue>();
        }
    }

    /// <summary>
    /// 主項目內容
    /// </summary>
    public class MainPropertyValue
    {
        /// <summary>
        /// 自訂屬性別名
        /// </summary>
        public string InputValue { get; set; }

        /// <summary>
        /// 主項目值ID
        /// </summary>
        public int MainPropertyValueID { get; set; }

        /// <summary>
        /// 主項目值名稱
        /// </summary>
        public string MainPropertyValueName { get; set; }

        /// <summary>
        /// 圖片路徑
        /// </summary>
        public string PictureURL { get; set; }

        /// <summary>
        /// 第2項目內容
        /// </summary>
        public List<SecondPropertyValue> SecondPropertyValueCell { get; set; }

        public MainPropertyValue()
        {
            SecondPropertyValueCell = new List<SecondPropertyValue>();
        }
    }

    /// <summary>
    /// 第2項目內容
    /// </summary>
    public class SecondPropertyValue
    {
        /// <summary>
        /// 可售數量
        /// </summary>
        public int CanSaleQty { get; set; }

        /// <summary>
        /// 第2項目值ID
        /// </summary>
        public int SecondPropertyValueID { get; set; }

        /// <summary>
        /// 第2項目值名稱
        /// </summary>
        public string SecondPropertyValueName { get; set; }

        public SecondPropertyValue()
        {
            CanSaleQty = 0;
        }
    }

    /// <summary>
    /// Kendo 下拉式選單內容
    /// </summary>
    public class KendoSelectData
    {
        /// <summary>
        /// 顯示的名稱
        /// </summary>
        public string Text { get; set; }

        /// <summary>
        /// 顯示名稱對應 DB 的 ID 值
        /// </summary>
        public int Value { get; set; }
    }

    public class PropertySelection
    {
        /// <summary>
        /// 主項目選項
        /// </summary>
        public List<KendoSelectData> MainSetectionCell { get; set; }

        /// <summary>
        /// 次項目選項
        /// </summary>
        public List<KendoSelectData> SecondSetectionCell { get; set; }

        /// <summary>
        /// 主項目 + 次項目的商品屬性集合
        /// </summary>
        public string AesPropertyCell { get; set; }

        /// <summary>
        /// 已選擇的主項目ID
        /// </summary>
        /// <remarks>編輯時使用</remarks>
        public string MainSelectedID { get; set; }

        /// <summary>
        /// 已選擇的次項目ID
        /// </summary>
        /// <remarks>編輯時使用</remarks>
        public string SecondSelectedID { get; set; }

        public PropertySelection()
        {
            MainSetectionCell = new List<KendoSelectData>();
            SecondSetectionCell = new List<KendoSelectData>();
            AesPropertyCell = string.Empty;
            MainSelectedID = string.Empty;
            SecondSelectedID = string.Empty;
        }
    }

    public class PropertyValueSelection
    {
        /// <summary>
        /// 展開維度
        /// </summary>
        public int ExpandDimension { get; set; }

        /// <summary>
        /// 主項目名稱
        /// </summary>
        public string MainPropertyName { get; set; }

        /// <summary>
        /// 次項目名稱
        /// </summary>
        public string SecondPropertyName { get; set; }

        /// <summary>
        /// 主項目選項
        /// </summary>
        public List<KendoSelectData> MainSetectionCell { get; set; }

        /// <summary>
        /// 次項目選項
        /// </summary>
        public List<KendoSelectData> SecondSetectionCell { get; set; }

        /// <summary>
        /// 已勾選的主項目選項 ID
        /// </summary>
        /// <remarks>編輯時使用</remarks>
        public List<int> MainCheckedIDCell { get; set; }

        /// <summary>
        /// 已勾選的次項目選項 ID
        /// </summary>
        /// <remarks>編輯時使用</remarks>
        public List<int> SecondCheckedIDCell { get; set; }

        /// <summary>
        /// 已儲存的規格商品內容
        /// </summary>
        /// <remarks>編輯時使用</remarks>
        public TwoDimensionProductProperty TwoDimensionProductProperty { get; set; }

        public PropertyValueSelection()
        {
            ExpandDimension = 0;
            MainPropertyName = string.Empty;
            SecondPropertyName = string.Empty;
            MainSetectionCell = new List<KendoSelectData>();
            SecondSetectionCell = new List<KendoSelectData>();
            MainCheckedIDCell = new List<int>();
            SecondCheckedIDCell = new List<int>();
            TwoDimensionProductProperty = new TwoDimensionProductProperty();
        }
    }
}