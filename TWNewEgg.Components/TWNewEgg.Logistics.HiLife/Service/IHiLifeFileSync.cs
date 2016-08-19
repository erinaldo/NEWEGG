using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using TWNewEgg.Logistics.HiLife.Model;

namespace TWNewEgg.Logistics.HiLife.Service
{
    /// <summary>
    /// 檔案同步模組
    /// </summary>
    public interface IHiLifeFileSync
    {
        /// <summary>
        /// 讀取同步設定檔, 並做相關的建置
        /// </summary>
        /// <param name="argStrConfigFilePath">結構檔的完整路徑及檔名</param>
        /// <returns>讀取成功並且建置成功回傳true, 否則回傳false</returns>
        bool LoadFileSyncSchedule(string argStrConfigFilePath);

        /// <summary>
        /// 將傳入字串依設定進行正規補齊或截斷
        /// </summary>
        /// <param name="argStrValue">傳入字串</param>
        /// <param name="argNumTargetLengh">規定長度</param>
        /// <param name="argPadSymbol">補齊符號</param>
        /// <param name="argAlign">對齊方式</param>
        /// <returns>正規格式的字串</returns>
        string ParseText(string argStrValue, int argNumTargetLengh, char argPadSymbol, HiLifeBasic.Align argAlign);

        /// <summary>
        /// Log Message
        /// </summary>
        /// <param name="argStrMessage">Message</param>
        void LogSyncMessage(string argStrMessage);

        /// <summary>
        /// 根據FileTime傳回離下次執行的時間有多少毫秒
        /// </summary>
        /// <param name="argFileType">HiLifefileType</param>
        /// <returns>-1 or micro seconds</returns>
        int UntilNextTime(HiLifeFileType argFileType);

        /// <summary>
        /// 新增一個檔案排程
        /// </summary>
        /// <param name="argFileType">HiLifeFileType</param>
        /// <param name="argBoolSyncFirst">true:無視時間先Copy資料再進入排程, false:依排程設定copy資料</param>
        /// <returns>設定排程成功:true, else return false</returns>
        bool StartNewSyncSchedule(HiLifeFileType argFileType, bool argBoolSyncFirst);

        /// <summary>
        /// 啟動所有檔案同步服務
        /// </summary>
        /// <param name="argBoolSyncFirst">true:重啟時先同步所有檔案; false:重啟之後,檔案依排程時間再同步</param>
        /// <returns>true:啟動成功; false:啟動失敗</returns>
        bool StartAllSchedule(bool argBoolSyncFirst);

        /// <summary>
        /// 停止所有檔案同步服務
        /// </summary>
        /// <returns>true: 停止成功; false:停止失敗</returns>
        bool StopAllSchedule();

        /// <summary>
        /// 暫停該檔的排程
        /// </summary>
        /// <param name="argFileType">HiLifeFileType</param>
        /// <returns>true:停止成功; false:停止失敗</returns>
        bool StopSyncSchedule(HiLifeFileType argFileType);

        /// <summary>
        /// 取得欄位結構物件
        /// </summary>
        /// <returns>根據XML所解析所有FileType Field結構, 並存放在Dictionary</returns>
        Dictionary<string, Dictionary<string, Dictionary<string, HiLifeBasicFieldStruct>>> GetFieldStruct();

        /// <summary>
        /// 設定Log檔案的目錄(僅目錄)
        /// </summary>
        /// <param name="argStrFilePath"></param>
        void SetLogFilePath(string argStrFilePath);

        /// <summary>
        /// 取得檔案同步時間、同步路徑
        /// </summary>
        /// <returns>Dictionary<F00, 同步路徑物件></returns>
        Dictionary<string, SyncTimes> GetFileSync();

    }
}
