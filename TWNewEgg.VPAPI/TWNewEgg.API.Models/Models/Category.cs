using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TWNewEgg.API.Models
{
    /*---------- add by thisway ----------*/
    /// <summary>
    /// Manage Items (APIModel)
    /// <para>Website Page:Create Items / Manage Items</para>
    /// </summary>
    public class Category
    {
        /// <summary>
        /// products category's English name
        /// <para>DB Form:TWSQLDB.dbo.category</para>
        /// </summary>
        public List<string> Title { get; set; }

        /// <summary>
        /// products category's TW name
        /// <para>DB Form:TWSQLDB.dbo.category</para>
        /// </summary>
        public List<string> ProductDescription { get; set; }

        /// <summary>
        /// products category's layer
        /// <para>DB Form:TWSQLDB.dbo.category</para>
        /// </summary>
        public List<int> Layer { get; set; }

        /// <summary>
        /// products' parent category
        /// <para>DB Form:TWSQLDB.dbo.category</para>
        /// </summary>
        public List<int> ParentID { get; set; }

        /// <summary>
        /// products' ID
        /// <para>DB Form:TWSQLDB.dbo.category</para>
        /// </summary>
        public List<int> ID { get; set; }

        /// <summary>
        /// products' show order
        /// <para>DB Form:TWSQLDB.dbo.category</para>
        /// </summary>
        public List<int> Showorder { get; set; }

        /// <summary>
        /// 產品中文名稱
        /// <para>DB Form:TWSQLDB.dbo.Product.NameTW</para>
        /// </summary>
        public List<string> Name { get; set; }

        /// <summary>
        /// 產品英文名稱
        /// <para>DB Form:TWSQLDB.dbo.Product.NameTW</para>
        /// </summary>
        public List<string> NameTW { get; set; }
    }
    /*---------- end by thisway ----------*/
}
