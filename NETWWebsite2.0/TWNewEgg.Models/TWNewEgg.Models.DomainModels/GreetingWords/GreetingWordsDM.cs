using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TWNewEgg.Models.DomainModels.Store;

namespace TWNewEgg.Models.DomainModels.GreetingWords
{
    /// <summary>
    /// -----------------------add by bruce 20160329
    ///1 登入問候語
    ///2 節日問候卡
    /// </summary>
    public class GreetingWordsDM
    {
        ///// <summary>
        ///// Related to CategoryId
        ///// </summary>
        //public enum CategoryNameTW
        //{
        //    //HomeHotWord
        //    首頁熱門關鍵字 = 0,
        //    // LoginedWord
        //    登入問候語 = 1,
        //    // HolidayWord
        //    節日問候卡 = 2
        //};

        ///// <summary>
        ///// 取得列舉的名稱
        ///// </summary>
        //public string getCategoryNameTW()
        //{
        //    //get { return Enum.Parse(typeof(CategoryNameTW), this.CategoryId.ToString()).ToString(); }

        //    int category_id = this.CategoryId;
        //    return Enum.Parse(typeof(CategoryNameTW), category_id.ToString()).ToString();
        //}

        ///// <summary>
        ///// 取得列舉的名稱
        ///// </summary>
        //public static string getCategoryNameTW(int category_id)
        //{
        //    return Enum.Parse(typeof(CategoryNameTW), category_id.ToString()).ToString();
        //}


        ///// <summary>
        ///// 系統流水編號
        ///// </summary>
        //public int ID { get; set; }

        ///// <summary>
        ///// 文字描述
        ///// </summary>
        //public string Description { get; set; }

        ///// <summary>
        ///// 顯示順序
        ///// </summary>
        //public int Showorder { get; set; }

        ///// <summary>
        ///// 是否顯示, 1:顯示, 0:不顯示
        ///// </summary>
        //public int ShowAll { get; set; }

        ///// <summary>
        ///// 點選連結
        ///// </summary>
        //public string Clickpath { get; set; }

        ///// <summary>
        ///// 分類Id
        ///// -----------------------
        /////0 首頁熱門關鍵字
        /////1 登入問候語
        /////2 節日問候卡
        ///// </summary>
        //public int CategoryId { get; set; }

        ///// <summary>
        ///// 開始時間
        ///// </summary>
        //public DateTime StartDate { get; set; }

        ///// <summary>
        ///// 結束時間
        ///// </summary>
        //public DateTime EndDate { get; set; }

        ///// <summary>
        ///// 創建者
        ///// </summary>
        //public string CreateUser { get; set; }

        ///// <summary>
        ///// 創建日期
        ///// </summary>
        //public DateTime CreateDate { get; set; }

        ///// <summary>
        ///// 最後修改者
        ///// </summary>
        //public string UpdateUser { get; set; }

        ///// <summary>
        ///// 最後修改日期
        ///// </summary>
        //public DateTime? UpdateDate { get; set; }

        ///// <summary>
        ///// 自訂代碼文字
        ///// -----------------------
        ///// use for 節日問候卡
        ///// </summary>
        //public string CodeText { get; set; }

        ///// <summary>
        ///// 圖片位置
        ///// -----------------------
        /////use for 節日問候卡
        ///// </summary>
        //public string ImagesURL { get; set; }
    }


}
