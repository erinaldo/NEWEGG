using System.Runtime.Serialization;

namespace Newegg.Mobile.MvcApplication.Models
{
    /// <summary>
    /// Initializes a new instance of the UIReviewSummary class.
    /// </summary>
    [DataContract]
    public class UIReviewSummary
    {
        /// <summary>
        /// Gets or sets Rating.
        /// </summary>
        [DataMember(Name = "Rating")]
        public int Rating { get; set; }
        
        /// <summary>
        /// Gets or sets TotalReviews.
        /// </summary>
        [DataMember(Name = "TotalReviews")]
        public string TotalReviews { get; set; }
    }
}