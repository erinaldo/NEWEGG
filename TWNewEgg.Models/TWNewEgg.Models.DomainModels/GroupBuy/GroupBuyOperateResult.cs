using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TWNewEgg.Models.DomainModels.GroupBuy
{
    public class GroupBuyOperateResult
    {
        public GroupBuyOperateResult()
        {
            this.IsSuccess = false;
            this.Msg = "";
            this.Data = null;
        }
        public bool IsSuccess { get; set; }
        public string Msg { get; set; }
        public object Data { get; set; }
    }
}