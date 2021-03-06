﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TWNewEgg.Models.ViewModels.Store
{
    public class BreadcrumbItem
    {
        /// <summary>
        /// 項目顯示的文字
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// 對應的分類ID
        /// </summary>
        public int CategoryID { get; set; }

        /// <summary>
        /// 是否為任選館
        /// </summary>
        public bool IsOptionStore { get; set; }
    }
}
