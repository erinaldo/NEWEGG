using System.Runtime.Serialization;

namespace Newegg.Mobile.MvcApplication.Models
{
	/// <summary>
    /// Initializes a new instance of the UIReviewInfo class.
	/// </summary>
    [DataContract]
    public class UIReviewInfo
    {
		/// <summary>
        /// Gets or sets Title.
		/// </summary>
        [DataMember(Name = "Title")]
        public string Title { get; set; }
        
        /// <summary>
        /// Gets or sets Rating.
        /// </summary>
        [DataMember(Name = "Rating")]
		public int Rating { get; set; }
        
        /// <summary>
        /// Gets or sets PublishDate.
        /// </summary>
        [DataMember(Name = "PublishDate")]
		public string PublishDate { get; set; }
        
        /// <summary>
        /// Gets or sets LoginNickName.
        /// </summary>
        [DataMember(Name = "LoginNickName")]
		public string LoginNickName { get; set; }
		
        /// <summary>
        /// Gets or sets BoughtTimeTypeString.
        /// </summary>
        [DataMember(Name = "BoughtTimeTypeString")]
		public string BoughtTimeTypeString { get; set; }
		
        /// <summary>
        /// Gets or sets TechLevelTypeString.
        /// </summary>
        [DataMember(Name = "TechLevelTypeString")]
		public string TechLevelTypeString { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether IsNewReview.
        /// </summary>
        [DataMember(Name = "IsNewReview")]
		public bool IsNewReview { get; set; }
		
        /// <summary>
        /// Gets or sets a value indicating whether PurchaseMark.
        /// </summary>
        [DataMember(Name = "PurchaseMark")]
		public bool PurchaseMark { get; set; }
		
        /// <summary>
        /// Gets or sets Cons.
        /// </summary>
        [DataMember(Name = "Cons")]
		public string Cons { get; set; }
		
        /// <summary>
        /// Gets or sets Pros.
        /// </summary>
        [DataMember(Name = "Pros")]
		public string Pros { get; set; }
		
        /// <summary>
        /// Gets or sets Comments.
        /// </summary>
        [DataMember(Name = "Comments")]
		public string Comments { get; set; }
		
        /// <summary>
        /// Gets or sets TotalConsented.
        /// </summary>
        [DataMember(Name = "TotalConsented")]
		public int TotalConsented { get; set; }
		
        /// <summary>
        /// Gets or sets TotalVoting.
        /// </summary>
        [DataMember(Name = "TotalVoting")]
		public int TotalVoting { get; set; }
    }
}