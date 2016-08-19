using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TWNewEgg.Models.ViewModels.Activity
{
    public class ActivityVM
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string HtmlContext { get; set; }
        
        /// <summary>
        /// 0 才顯示 
        /// 1 不顯示
        /// </summary>
        public int ShowType { get; set; }
        /// <summary>
        /// 解析JSON
        /// "{/"Header/": true, /"Topper/": true, /"Bottomer/": true, /"Footer/": true, /"FloatMenu/": true}"
        /// </summary>
        public Nullable<int> ActionType { get; set; }
        public string MetaTitle { get; set; }
        public string MetaKeyWord { get; set; }
        public string MetaDescription { get; set; }
        public string SectionInfor{ get; set; }

        /// <summary>
        /// 解析ActionType，True or false
        /// </summary>
        public string Header { get; set; }
        public string Topper { get; set; }
        public string Bottomer { get; set; }
        public string Footer { get; set; }
        public string FloatMenu { get; set; }
    }
}
