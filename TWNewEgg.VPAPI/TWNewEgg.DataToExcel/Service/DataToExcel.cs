using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using System.ComponentModel;

namespace TWNewEgg.DataToExcel
{
    public static class Export
    {
        // 各欄欄寬
        private static List<int> columnWidth;

        /// <summary>
        /// 將 List 轉換為 Excel 2003 格式
        /// </summary>
        /// <typeparam name="T">泛型類別</typeparam>
        /// <param name="dataList">待轉換的資料列表</param>
        /// <param name="fileName">匯出的 Excel 檔案名稱</param>
        /// <param name="titleLine">抬頭的行數</param>
        /// <returns>成功或失敗訊息</returns>
        public static string ListToExcel<T>(IEnumerable<T> dataList, string fileName, string sheetName, int titleLine)
        {
            try
            {
                // 判斷輸入項目是否符合規範：
                // 1.待轉換的資料列表不得為空
                // 2.資料量是否符合 Excel 2003 欄位數量限制：256 column * 65536 row
                // 3.檔案名稱不得為空
                // 4.工作表名稱不得為空
                if (dataList != null
                 && dataList.Count() <= 65536 - titleLine && typeof(T).GetProperties().Count() <= 256
                 && !string.IsNullOrEmpty(fileName)
                 && !string.IsNullOrEmpty(sheetName))
                {
                    // 宣告 Excel 2003 格式空間 
                    HSSFWorkbook workbook = new HSSFWorkbook();

                    // 建立工作表
                    ISheet sheet = workbook.CreateSheet(sheetName);

                    // 抬頭至少為1行
                    if (titleLine < 1)
                    {
                        titleLine = 1;
                    }

                    // 填入欄位名稱
                    sheet = GetPropertyNames(sheet, typeof(T), titleLine);

                    // 填入資料
                    sheet = GetPropertyValues(sheet, dataList, titleLine);

                    // 設定欄寬
                    for(int i = 0; i < typeof(T).GetProperties().Count(); i++)
                    {
                        sheet.SetColumnWidth(i, (columnWidth[i] + 1) * 256);
                    }

                    // 匯出檔案
                    string saveDate = SaveFile(workbook, fileName);

                    return string.Format("Success_{0}", saveDate);
                }
                else
                {
                    // 回傳的錯誤訊息
                    string errorMessage = "Error: ";

                    // 資料量異常錯誤訊息
                    if (dataList.Count() > 65536 - titleLine || typeof(T).GetProperties().Count() > 256)
                    {
                        errorMessage += "資料量已超出可填寫範圍，Excel 2003 欄位數量限制：256 column * 65536 row。";
                    }

                    // 未輸入資料列表錯誤訊息
                    if (dataList == null)
                    {
                        errorMessage += "未輸入資料列表。";
                    }

                    // 未輸入資料列表錯誤訊息
                    if (string.IsNullOrEmpty(fileName))
                    {
                        errorMessage += "未輸入檔案名稱。";
                    }

                    // 未輸入資料列表錯誤訊息
                    if (string.IsNullOrEmpty(sheetName))
                    {
                        errorMessage += "未輸入工作表名稱。";
                    }

                    return errorMessage;
                }
            }
            catch(Exception ex)
            {
                return string.Format("Error: {0}", ex.ToString());
            }
        }

        /// <summary>
        /// 填入欄位名稱
        /// </summary>
        /// <param name="sheet">待填入欄位名稱的工作表</param>
        /// <param name="type">待轉換資料的型別資訊</param>
        /// <returns>填完欄位名稱的工作表</returns>
        private static ISheet GetPropertyNames(ISheet sheet, Type type, int titleLine)
        {
            // 建立欄位名稱的 Row
            IRow titleRow = sheet.CreateRow(0);

            // 宣告欄位名稱存放空間
            List<string> titleList = new List<string>();

            // 讀取欄位名稱，並存入欄位名稱列表
            foreach (PropertyInfo propertyInfo in type.GetProperties())
            {
                // 判斷是否有 DisplayName
                if (propertyInfo.GetCustomAttribute(typeof(DisplayNameAttribute), false) != null)
                {
                    // 使用 DisplayName 做為欄位名稱
                    titleList.Add((propertyInfo.GetCustomAttribute(typeof(DisplayNameAttribute), false) as DisplayNameAttribute).DisplayName);
                }
                else
                {
                    // 使用 Model 的項目名稱做為欄位名稱
                    titleList.Add(propertyInfo.Name);
                }
            }

            columnWidth = new List<int>();

            // 依序將欄位名稱放入，並設定預設欄寬
            for (int i = 0; i < titleList.Count; i++)
            {
                sheet.GetRow(0).CreateCell(i).SetCellValue(titleList[i]);

                // 依欄位名稱的內容大小，設為預設欄寬
                columnWidth.Add(Encoding.Default.GetBytes(titleList[i]).Length);
            }
            
            if (titleLine == 2)
            {
                // 宣告第2欄的欄位名稱存放空間
                List<string> titleList_Second = new List<string>();

                // 讀取欄位名稱，並存入欄位名稱列表
                foreach (PropertyInfo propertyInfo in type.GetProperties())
                {
                    // 使用 Model 的項目名稱做為欄位名稱
                    titleList_Second.Add(propertyInfo.Name);
                }

                //2014.11.17 需求：將英文名稱放第1欄，中文名稱放第2欄
                // 重新把第1欄名稱改放入英文名稱，並依內容調整欄寬
                for (int i = 0; i < titleList.Count; i++)
                {
                    sheet.GetRow(0).CreateCell(i).SetCellValue(titleList_Second[i]);

                    // 取得欄寬所需大小
                    int newColumnWidth = Encoding.Default.GetBytes(titleList_Second[i]).Length;

                    // 若所需欄寬，大於目前的欄寬，則更新欄寬值
                    if (newColumnWidth > columnWidth[i])
                    {
                        columnWidth[i] = newColumnWidth;
                    }
                }

                // 建立第2個欄位名稱的 Row
                IRow titleRow_second = sheet.CreateRow(1);

                // 把第1欄名稱改放入中文名稱
                for (int i = 0; i < titleList.Count; i++)
                {
                    sheet.GetRow(1).CreateCell(i).SetCellValue(titleList[i]);
                }
            }
            
            return sheet;
        }

        /// <summary>
        /// 填入資料
        /// </summary>
        /// <typeparam name="T">泛型類別</typeparam>
        /// <param name="sheet">待填入資料的工作表</param>
        /// <param name="dataList">待轉換的資料列表</param>
        /// <returns>填完資料的工作表</returns>
        private static ISheet GetPropertyValues<T>(ISheet sheet, IEnumerable<T> dataList, int titleLine)
        {
            for (int i = titleLine; i < dataList.Count() + titleLine; i++)
            {
                // 讀取單筆資料
                var data = dataList.ElementAt(i - titleLine);

                // 建立 Row
                IRow Row = sheet.CreateRow(i);

                // 宣告每個欄位資料的暫存空間
                List<string> valueList = new List<string>();

                // 讀取每個欄位的資料，並存入暫存列表
                foreach (PropertyInfo propertyInfo in data.GetType().GetProperties())
                {
                    // 判斷每個欄位是否有值，有則寫入該值，沒有則給空值
                    if (propertyInfo.GetValue(data, null) != null)
                    {
                        valueList.Add(propertyInfo.GetValue(data, null).ToString());
                    }
                    else
                    {
                        valueList.Add(string.Empty);
                    }
                }

                // 依序將各欄位資料寫入，並依內容調整欄寬
                for (int j = 0; j < valueList.Count; j++)
                {
                    sheet.GetRow(i).CreateCell(j).SetCellValue(valueList[j]);

                    // 取得欄寬所需大小
                    int newColumnWidth = Encoding.Default.GetBytes(valueList[j]).Length;

                    // 若所需欄寬，大於目前的欄寬，則更新欄寬值
                    if (newColumnWidth > columnWidth[j])
                    {
                        columnWidth[j] = newColumnWidth;
                    }
                }
            }

            return sheet;
        }

        /// <summary>
        /// 匯出檔案
        /// </summary>
        /// <param name="workbook">待匯出的 Excel 2003 格式</param>
        /// <param name="fileName">匯出的 Excel 檔案名稱</param>
        private static string SaveFile(HSSFWorkbook workbook, string fileName)
        {
            // 指定存放 Excel 檔的資料夾
            string saveFolder = AppDomain.CurrentDomain.BaseDirectory + "ToExcel";

            // 判斷指定的資料夾是否存在，不存在則建立資料夾
            if (!Directory.Exists(saveFolder))
            {
                Directory.CreateDirectory(saveFolder);
            }

            // 產生日期 (年月日時分秒微秒)
            string saveDate = DateTime.Now.ToString("yyyyMMddHHmmssfff");

            // 檔案儲存路徑 指定位置 Jack.W.Wu 140730
            string savePath = saveFolder + "\\" + fileName + "_" + saveDate + ".xls";

            // 匯出 Excel 檔案 指定位置 Jack.W.Wu 140730
            FileStream file = new FileStream(savePath, FileMode.Create);
            workbook.Write(file);
            file.Close();

            return saveDate;
        }
    }
}