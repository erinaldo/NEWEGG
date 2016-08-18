using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace Newegg.Mobile.MvcApplication.Models
{
    /// <summary>
    /// Initializes a new instance of the UINoteItemInfo class.
    /// </summary>
    [DataContract]
    public class UINoteItemInfo
    {
        /// <summary>
        /// Initializes a new instance of the UINoteItemInfo class.
        /// </summary>
        public UINoteItemInfo()
        {
        }

        /// <summary>
        /// Initializes a new instance of the UINoteItemInfo class.
        /// </summary>
        /// <param name="summary">Param summary.</param>
        /// <param name="link">Param link.</param>
        /// <param name="color">Param color.</param>
        /// <param name="title">Param title.</param>
        /// <param name="content">Param content.</param>
        public UINoteItemInfo(string summary, string link, UIColorType color, string title, string content)
        {
            this.Summary = summary;
            this.SummaryLink = link;
            this.SummaryColor = color;
            this.Title = title;
            this.Content = content;
        }

        /// <summary>
        /// Initializes a new instance of the UINoteItemInfo class.
        /// </summary>
        /// <param name="summary">Param summary.</param>
        /// <param name="color">Param color.</param>
        public UINoteItemInfo(string summary, UIColorType color)
            : this(summary, string.Empty, color, string.Empty, string.Empty)
        {
        }

        /// <summary>
        /// Initializes a new instance of the UINoteItemInfo class.
        /// </summary>
        /// <param name="summary">Param summary.</param>
        /// <param name="link">Param link.</param>
        /// <param name="color">Param color.</param>
        public UINoteItemInfo(string summary, string link, UIColorType color)
            : this(summary, link, color, string.Empty, string.Empty)
        {
        }

        /// <summary>
        /// Initializes a new instance of the UINoteItemInfo class.
        /// </summary>
        /// <param name="summary">Param summary.</param>
        /// <param name="color">Param color.</param>
        /// <param name="title">Param title.</param>
        /// <param name="content">Param content.</param>
        public UINoteItemInfo(string summary, UIColorType color, string title, string content)
            : this(summary, string.Empty, color, title, content)
        {
        }

        /// <summary>
        /// Initializes a new instance of the UINoteItemInfo class.
        /// </summary>
        /// <param name="summary">Param summary.</param>
        /// <param name="color">Param color.</param>
        /// <param name="content">Param content.</param>
        public UINoteItemInfo(string summary, UIColorType color, string content)
            : this(summary, string.Empty, color, string.Empty, content)
        {
        }

        /// <summary>
        /// Gets or sets Summary.
        /// </summary>
        [DataMember(Name = "Summary")]
        public string Summary { get; set; }

        /// <summary>
        /// Gets or sets SummaryLink.
        /// </summary>
        [DataMember(Name = "SummaryLink")]
        public string SummaryLink { get; set; }

        /// <summary>
        /// Gets or sets SummaryColor.
        /// </summary>
        [DataMember(Name = "SummaryColor")]
        public UIColorType SummaryColor { get; set; }

        /// <summary>
        /// Gets or sets Title.
        /// </summary>
        [DataMember(Name = "Title")]
        public string Title { get; set; }

        /// <summary>
        /// Gets or sets Content.
        /// </summary>
        [DataMember(Name = "Content")]
        public string Content { get; set; }

        /// <summary>
        /// Gets or sets PromotionInfo.
        /// </summary>
        [DataMember(Name = "PromotionInfo")]
        public UIPreferredAccountPromotionInfo PromotionInfo { get; set; }
    }
}
