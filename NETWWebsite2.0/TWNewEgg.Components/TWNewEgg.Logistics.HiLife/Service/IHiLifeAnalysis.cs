using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using TWNewEgg.DB.Models.Logistics.HiLife;
using TWNewEgg.Logistics.HiLife.Model;

namespace TWNewEgg.Logistics.HiLife.Service
{
    /// <summary>
    /// HiLife Data Analysis Model
    /// </summary>
    interface IHiLifeAnalysis
    {
        /// <summary>
        /// 根據HeadId取得整份文件
        /// </summary>
        /// <typeparam name="T">HiLifeF00, HiLifeF01, HiLifeF02....HiLifeF09, HiLifeTxt</typeparam>
        /// <param name="argNumHeadId">HeadId</param>
        /// <returns>HiLifeF00, HiLifeF01, HiLifeF02....HiLifeF09, HiLifeTxt的資料</returns>
        T GetHiLifeDocument<T>(int argNumHeadId);

        /// <summary>
        /// 根據Head的建立日期取得整份文件
        /// </summary>
        /// <typeparam name="T">HiLifeF00, HiLifeF01, HiLifeF02....HiLifeF09, HiLifeTxt</typeparam>
        /// <param name="argDateTime">建立日期, 僅比對Year, Month, Day的資料</param>
        /// <returns>日期內符合的資料</returns>
        List<T> GetHiLifeDocument<T>(DateTime argDateTime);

        /// <summary>
        /// 新增一筆Head物件
        /// </summary>
        /// <typeparam name="T">F01Head, F02Head ... F09Head</typeparam>
        /// <param name="argObjHead">欲增加的Head物件</param>
        /// <returns>新增成功回傳Head Id, 新增失敗回傳0或-1</returns>
        int CreateHead<T>(T argObjHead);

        /// <summary>
        /// 修改一筆Head物件
        /// </summary>
        /// <typeparam name="T">F01Head, F02Head ... F09Head</typeparam>
        /// <param name="argObjHead">欲修改的Head物件</param>
        /// <returns>修改成功回傳true, 修改失敗回傳false</returns>
        bool UpdateHead<T>(T argObjHead);

        /// <summary>
        /// 根據Head Id取得Head
        /// </summary>
        /// <typeparam name="T">F01Head, F02Head ... F09Head</typeparam>
        /// <param name="argNumHeadId">Head Id</param>
        /// <returns>有資料傳回對應T型態的Head物件, 無資料回傳null</returns>
        T GetHead<T>(int argNumHeadId);

        /// <summary>
        /// 根據日期取得Head列表
        /// </summary>
        /// <typeparam name="T">F01Head, F02Head ... F09Head</typeparam>
        /// <param name="argDateTime">查詢日期, 僅比對Year, Month, Day</param>
        /// <returns>有資料傳回對應T型態的List物件, 無資料回傳null</returns>
        List<T> GetHead<T>(DateTime argDateTime);

        /// <summary>
        /// 新增一筆Body物件
        /// </summary>
        /// <typeparam name="T">F00Body, F01Body, ... F09Body, TxtBody</typeparam>
        /// <param name="argObjBody">欲新增的Body物件</param>
        /// <returns>新增成功回傳Body Id, 新增失敗回傳0或-1</returns>
        int CreateBody<T>(T argObjBody);

        /// <summary>
        /// 修改一筆Body物件
        /// </summary>
        /// <typeparam name="T">F00Body, F01Body, ... F09Body, TxtBody</typeparam>
        /// <param name="argObjBody">欲修改的Body物件</param>
        /// <returns>修改成功回傳true, 修改失敗回傳false</returns>
        bool UpdateBody<T>(T argObjBody);

        /// <summary>
        /// 修改List內的所有Body
        /// </summary>
        /// <typeparam name="T">F00Body, F01Body, ... F09Body, TxtBody</typeparam>
        /// <param name="argListBody">要修改的Body列表</param>
        /// <returns>修改成功回傳true, 修改失敗回傳false</returns>
        bool UpdateBody<T>(List<T> argListBody);

        /// <summary>
        /// 根據Body Id取得Body物件
        /// </summary>
        /// <typeparam name="T">F00Body, F01Body, ... F09Body, TxtBody</typeparam>
        /// <param name="argNumBodyId">Body Id</param>
        /// <returns>有資料傳回對應T型態的Body物件, 無資料回傳null</returns>
        T GetBody<T>(int argNumBodyId);

        /// <summary>
        /// 根據Head Id回傳相對應的所有Body物件
        /// </summary>
        /// <typeparam name="T">F00Body, F01Body, ... F09Body, TxtBody</typeparam>
        /// <param name="argNumHeadId">Head Id</param>
        /// <returns>有資料傳回對應T型態的Body列表, 無資料回傳null</returns>
        List<T> GetBodyByHeadId<T>(int argNumHeadId);

        /// <summary>
        /// 新增Tail(表尾)物件
        /// </summary>
        /// <typeparam name="T">F01Tail, F02Tail ... F09Tail</typeparam>
        /// <param name="argObjTail">欲新增的Tail物件</param>
        /// <returns>>新增成功回傳Tail Id, 新增失敗回傳0或-1</returns>
        int CreateTail<T>(T argObjTail);

        /// <summary>
        /// 修改Tail(表尾)物件
        /// </summary>
        /// <typeparam name="T">F01Tail, F02Tail ... F09Tail</typeparam>
        /// <param name="argObjTail">欲修改的Tail物件</param>
        /// <returns>修改成功回傳true, 修改失敗回傳false</returns>
        bool UpdateTail<T>(T argObjTail);

        /// <summary>
        /// 根據Tail Id取得Tail物件
        /// </summary>
        /// <typeparam name="T">F01Tail, F02Tail ... F09Tail</typeparam>
        /// <param name="argNumTailId">Tail Id</param>
        /// <returns>有資料傳回對應T型態的Tail物件, 無資料回傳null</returns>
        T GetTail<T>(int argNumTailId);

        /// <summary>
        /// 根據Head Id取得Tail物件
        /// </summary>
        /// <typeparam name="T">F01Tail, F02Tail ... F09Tail</typeparam>
        /// <param name="argNumHeadId">Head Id</param>
        /// <returns>有資料傳回對應T型態的Tail物件, 無資料回傳null</returns>
        T GetTailByHeadId<T>(int argNumHeadId);

        /// <summary>
        /// 根據建立時間取得Body列表
        /// </summary>
        /// <typeparam name="T">F00Body, F01Body, ... F09Body, TxtBody</typeparam>
        /// <param name="argCreateDate">建立時間, 僅比對Year, Month, Day</param>
        /// <returns>有資料傳回對應T型態的List of Body物件, 無資料回傳null</returns>
        List<T> GetBodyByCreareDate<T>(DateTime argCreateDate);

        /// <summary>
        /// 根據檔案Type開始進行資料轉換並存進DB
        /// </summary>
        /// <param name="argFileType">HiLifeFileType</param>
        /// <param name="argStrFilePath">來源完整檔案名稱(包含路徑及檔名)</param>
        /// <param name="argEncodingType">System.Text.Encoding</param>
        /// <returns>轉換筆數, 若回傳-1請先進行GetFieldStruct進行結構檔設定</returns>
        int ParseFileToDb(HiLifeFileType argFileType, string argStrFilePath, Encoding argEncodingType);

        /// <summary>
        /// 產生F02或F06的傳輸檔案
        /// </summary>
        /// <param name="argFileType">HiLifeFileType.F02或HiLifeFileType.F06</param>
        /// <param name="argSearchDate">從資料庫抓取哪一天的交易資料(僅比對Year, Month, Day)</param>
        /// <param name="argOutputDate">產生F02及F06的檔案日期</param>
        /// <param name="argStrTargetPath">產生的檔案最後要輸出至哪一個目錄</param>
        /// <returns>產生成功回傳true, 失敗回傳false</returns>
        bool ParseDbToFile(HiLifeFileType argFileType, DateTime argSearchDate, DateTime argOutputDate, string argStrTargetPath);

        /// <summary>
        /// 根據結構檔進行欄位長度、對齊方式等等的設定
        /// </summary>
        /// <param name="argStrConfigFilePath">結構檔的完整路徑及檔案</param>
        /// <returns>結構欄位的物件</returns>
        Dictionary<string, Dictionary<string, Dictionary<string, HiLifeBasicFieldStruct>>> GetFieldStruct(string argStrConfigFilePath);

        /// <summary>
        /// Log Message
        /// </summary>
        /// <param name="argStrMessage">Message</param>
        void LogMessage(string argStrMessage);

        /// <summary>
        /// 設定Log檔案的目錄(僅目錄)
        /// </summary>
        /// <param name="argStrFilePath"></param>
        void SetLogFilePath(string argStrFilePath);

        /// <summary>
        /// 根據Bc1與Bc2查到F02訂單出貨資訊
        /// </summary>
        /// <typeparam name="T">HiLife Model</typeparam>
        /// <param name="argT">欲查詢的HiLifeModel List</param>
        /// <returns>null或是F02Body列表</returns>
        List<F02Body> GetF02BodyByBc1AndBc2<T>(List<T> argListT);
    }
}
