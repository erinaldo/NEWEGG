using System.Runtime.Serialization;

namespace Newegg.Mobile.MvcApplication.Models
{
    /// <summary>
    /// Initializes a new instance of the UIProductReviewBarInfo class.
    /// </summary>
    [DataContract]
    public class UIProductReviewBarInfo
    {
        /// <summary>
        /// Gets or sets RatingCounts.
        /// </summary>
        [DataMember(Name = "RatingCounts")]
        public int RatingCounts { get; set; }

        /// <summary>
        /// Gets or sets Rating1Count.
        /// </summary>
        [DataMember(Name = "Rating1Count")]
        public string Rating1Count { get; set; }

        /// <summary>
        /// Gets or sets Rating1Percent.
        /// </summary>
        [DataMember(Name = "Rating1Percent")]
        public int Rating1Percent { get; set; }

        /// <summary>
        /// Gets or sets Rating2Count.
        /// </summary>
        [DataMember(Name = "Rating2Count")]
        public string Rating2Count { get; set; }

        /// <summary>
        /// Gets or sets Rating2Percent.
        /// </summary>
        [DataMember(Name = "Rating2Percent")]
        public int Rating2Percent { get; set; }

        /// <summary>
        /// Gets or sets Rating3Count.
        /// </summary>
        [DataMember(Name = "Rating3Count")]
        public string Rating3Count { get; set; }

        /// <summary>
        /// Gets or sets Rating3Percent.
        /// </summary>
        [DataMember(Name = "Rating3Percent")]
        public int Rating3Percent { get; set; }

        /// <summary>
        /// Gets or sets Rating4Count.
        /// </summary>
        [DataMember(Name = "Rating4Count")]
        public string Rating4Count { get; set; }

        /// <summary>
        /// Gets or sets Rating4Percent.
        /// </summary>
        [DataMember(Name = "Rating4Percent")]
        public int Rating4Percent { get; set; }

        /// <summary>
        /// Gets or sets Rating5Count.
        /// </summary>
        [DataMember(Name = "Rating5Count")]
        public string Rating5Count { get; set; }

        /// <summary>
        /// Gets or sets Rating5Percent.
        /// </summary>
        [DataMember(Name = "Rating5Percent")]
        public int Rating5Percent { get; set; }
    }
}
