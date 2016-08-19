using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TWNewEgg.Models.ViewModels.DeviceAd
{

    /// <summary>
    /// 依據 BSATW-177 手機改版需求增加---------------add by bruce 20160616
    /// 行動設備的廣告資料查詢
    /// </summary>
    public class DeviceAdSearchVM //: BaseDB
    {
        //行動設備的廣告資料查詢
        public DeviceAdSearchVM()
        {
            this.DeviceAdSetIDs = new List<int>();
            this.Flag = string.Empty;
            this.ShowAll = string.Empty;
            this.IsBetweenDate = false;
            this.OrderByShoworder = false;
            this.DescByCreateDate = false;
            this.MaxCount = 0;
            this.CategoryIDs = new List<int>();
        }
        /// <summary>
        /// 廣告位置
        /// 是屬於哪一個行動設備的廣告設定, DeviceAdSet.ID值, 可以不給
        /// 用在查詢目錄時是, DeviceAdSet.Parent值, 可以不給
        /// 用在查詢內文時是, DeviceAdContent.DeviceAdSetID值, 必要
        /// 
        /// 1000	手機首頁輪播
        /// 1001	生活提案-大圖
        /// 1002	全館分類
        /// 1003	促案
        /// 1004	美國直購
        /// 1005	各館輪播
        /// 
        /// 1006	廣告一
        /// 1007	廣告二
        /// 1008	廣告三
        /// 1009	廣告四
        /// 1010	廣告五
        /// 1011	廣告六
        /// 
        /// 1012	生活提案一
        /// 1013	生活提案二
        /// 1014	生活提案三
        /// 1015	生活提案四
        /// 
        /// 1016	全館分類設定
        /// 1017	促案設定
        /// 1018	左方區塊
        /// 1019	右方區塊
        /// 
        /// 1020	手機版櫥窗輪播
        /// 1021	手機版分類輪播
        /// 
        /// </summary>
        public List<int> DeviceAdSetIDs { get; set; }

        /// <summary>
        /// ---------------------------------------------add by bruce 20160623
        /// 來自SubCategory_NormalStore與Category的ID
        ///
        ///95	生活家電qq	1
        ///33	設計風尚	2
        ///34	國際名品	3
        ///35	美妝保養	4
        ///94	戶外休旅	5
        ///100	運動健身	6
        ///101	電腦週邊	7
        ///99	數位３Ｃ	8
        ///96	居家用品	9
        ///97	親子寵物	10
        ///36	保健養生	11
        ///37	樂活食尚	12
        ///38	美國新蛋直購	13
        ///131	玩轉世界生活誌	14
        ///137	尋找禮品	15
        ///
        /// 
        ///737	美國新蛋直購	1
        ///2509	尋找禮品	2
        ///1279	設計風尚	3
        ///734	國際名品	4
        ///1929	美妝保養	5
        ///1930	保健養生	6
        ///736	樂活食尚	7
        ///735	運動健身	8
        ///7	戶外休旅	9
        ///1928	親子寵物	10
        ///6	居家用品	11
        ///3	生活家電	12
        ///5	服飾配件	13
        ///264	數位3C	14
        ///1	電腦週邊	15
        ///2505	世界城市好好逛	16
        /// </summary>
        public List<int> CategoryIDs { get; set; }

        /// <summary>
        /// 用在查詢目錄時是, phone=手機, pad=平版, pc=桌機
        /// 用在查詢子目錄時是, 填空值
        /// ---------------------------------------------add by bruce 20160606
        /// 用在查詢內文時是, del=已刪除
        /// ---------------------------------------------add by bruce 20160623
        /// 若CategoryID有值這裡代表是屬於這個CategoryID的index
        /// </summary>
        public string Flag { get; set; }

        /// <summary>
        /// 是否顯示, 顯示:show|1, 不顯示:hide|0
        /// </summary>
        public string ShowAll { get; set; }

        /// <summary>
        /// 在有效的日期內的資料---------------------add by bruce 20160616
        /// </summary>
        public bool IsBetweenDate { get; set; }

        /// <summary>
        /// 以建檔日期排序降序
        /// //DescByCreateDate -------------------add by bruce 20160616
        /// </summary>
        public bool DescByCreateDate { get; set; }


        /// <summary>
        /// 以建檔日期排序升序
        /// //OrderByShoworder -------------------add by bruce 20160621
        /// </summary>
        public bool OrderByShoworder { get; set; }

        /// <summary>
        /// 資料最大筆數-------------------add by bruce 20160622
        /// 為<=0時是不限筆數
        /// </summary>
        public int MaxCount { get; set; }
        
    }
    
}
