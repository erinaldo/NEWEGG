using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Newegg.Mobile.MvcApplication.Models
{
    /// <summary>
    /// Initializes a new instance of the UIRegistrationTagInfo class.
    /// </summary>
    [DataContract]
    public class UIRegistrationTagInfo
    {
        /// <summary>
        /// Gets or sets CustomerID.
        /// </summary>
        [DataMember(Name = "CustomerID")]
        public string CustomerID { get; set; }

        /// <summary>
        /// Gets or sets CustomerEmail.
        /// </summary>
        [DataMember(Name = "CustomerEmail")]
        public string CustomerEmail { get; set; }

        /// <summary>
        /// Gets or sets CustomerCity.
        /// </summary>
        [DataMember(Name = "CustomerCity")]
        public string CustomerCity { get; set; }

        /// <summary>
        /// Gets or sets CustomerState.
        /// </summary>
        [DataMember(Name = "CustomerState")]
        public string CustomerState { get; set; }

        /// <summary>
        /// Gets or sets CustomerZip.
        /// </summary>
        [DataMember(Name = "CustomerZip")]
        public string CustomerZip { get; set; }

        /// <summary>
        /// Gets or sets NewsletterName.
        /// </summary>
        [DataMember(Name = "NewsletterName")]
        public string NewsletterName { get; set; }

        /// <summary>
        /// Gets or sets NewsletterOption.
        /// </summary>
        [DataMember(Name = "NewsletterOption")]
        public string NewsletterOption { get; set; }

        /// <summary>
        /// Gets or sets FirstName.
        /// </summary>
        [DataMember(Name = "FirstName")]
        public string FirstName { get; set; }

        /// <summary>
        /// Gets or sets Age.
        /// </summary>
        [DataMember(Name = "Age")]
        public int Age { get; set; }

        /// <summary>
        /// Gets or sets Country.
        /// </summary>
        [DataMember(Name = "Country")]
        public string Country { get; set; }

        /// <summary>
        /// Gets or sets MinimumIncomeLevel.
        /// </summary>
        [DataMember(Name = "MinimumIncomeLevel")]
        public string MinimumIncomeLevel { get; set; }

        /// <summary>
        /// Gets or sets MaximumIncomeLevel.
        /// </summary>
        [DataMember(Name = "MaximumIncomeLevel")]
        public string MaximumIncomeLevel { get; set; }

        /// <summary>
        /// Gets or sets EducationLevel.
        /// </summary>
        [DataMember(Name = "EducationLevel")]
        public string EducationLevel { get; set; }

        /// <summary>
        /// Gets or sets ExtraString.
        /// </summary>
        [DataMember(Name = "ExtraString")]
        public IList<string> ExtraString { get; set; }
    }
}
