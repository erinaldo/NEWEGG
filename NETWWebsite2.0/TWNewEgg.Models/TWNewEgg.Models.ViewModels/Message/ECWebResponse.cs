using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TWNewEgg.Models.ViewModels.Message
{
    public class ECWebResponse
    {
        public enum StatusCode
        {
            成功= 0,
            系統錯誤 = 1,
            轉頁付款 = 2,
            返回購物車頁 = 3,
            處理中 = 4,
            流程錯誤 = 5
        };
        public object Data { get; set; }
        public int Status { get; set; }
        public string Message { get; set; }
        public ErrorMessage Error { get; set; }
    }
    
}
