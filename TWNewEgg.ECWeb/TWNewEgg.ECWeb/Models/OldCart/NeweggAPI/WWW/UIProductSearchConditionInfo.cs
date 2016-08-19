using System.Runtime.Serialization;

namespace Newegg.Mobile.MvcApplication.Models
{
    /// <summary>
    /// Initializes a new instance of the UIProductSearchCondition class.
    /// </summary>
    [DataContract]
    public class UIProductSearchCondition
    {
        /// <summary>
        /// Gets or sets Keyword.
        /// </summary>
        [DataMember(Name = "Keyword")]
        public string Keyword { get; set; }

        /// <summary>
        /// Gets or sets PageNumber.
        /// </summary>
        [DataMember(Name = "PageNumber")]
        public int PageNumber { get; set; }

        /// <summary>
        /// Gets or sets Sort.
        /// search order options.
        /// RATING - BestRating
        /// PRICE - LowestPrice
        /// PRICED - HighestPrice
        /// REVIEWS - MostReviews
        /// DESCRIPTION - DescriptionA
        /// DESCRIPDOWN - DescriptionZ
        /// BESTMATCH - BestMatch
        /// FEATURED - FeaturedItems
        /// refer to \Configuration\ProductList\SortByDisplayOption.config
        /// and      \Configuration\ProductList\SortOrderInfo.config.
        /// </summary>
        [DataMember(Name = "Sort")]
		public string Sort { get; set; }
    }
}