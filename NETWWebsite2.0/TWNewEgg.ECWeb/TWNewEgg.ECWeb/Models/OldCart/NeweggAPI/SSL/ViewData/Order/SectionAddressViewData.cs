using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Newegg.Mobile.MvcApplication.Models
{
    /// <summary>
    /// Billing Shipping Address.
    /// </summary>
    public class SectionAddressViewData
    {
        /// <summary>
        /// Initializes a new instance of the SectionAddressViewData class.
        /// </summary>
        /// <param name="address">Address Info.</param>
        /// <param name="addressList">Address List.</param>
        /// <param name="title">Section title.</param>
        /// <param name="editAction">Edit Action.</param>
        public SectionAddressViewData(UIAddressInfo address, List<string> addressList, string title, string editAction)
        {
            this.Address = address;
            this.SectionTitle = title;
            this.EditAction = editAction;
            this.AddressList = addressList;
        }

        /// <summary>
        /// Initializes a new instance of the SectionAddressViewData class.
        /// </summary>
        /// <param name="addressList">Address List.</param>
        /// <param name="title">Section title.</param>
        public SectionAddressViewData(List<string> addressList, string title)
            : this(null, addressList, title, string.Empty)
        {
        }

        /// <summary>
        /// Initializes a new instance of the SectionAddressViewData class.
        /// </summary>
        /// <param name="address"> Address Info.</param>
        /// <param name="title">Section title.</param>
        /// <param name="editAction">Edit Action.</param>
        public SectionAddressViewData(UIAddressInfo address, string title, string editAction)
            : this(address, null, title, editAction)
        {
        }

        /// <summary>
        /// Gets or sets Address.
        /// </summary>
        public UIAddressInfo Address { get; set; }

        /// <summary>
        /// Gets or sets IsDisplayEditButton.
        /// </summary>
        public string SectionTitle { get; set; }

        /// <summary>
        /// Gets or sets EditAction.
        /// </summary>
        public string EditAction { get; set; }

        /// <summary>
        /// Gets or sets AddressList.
        /// </summary>
        public List<string> AddressList { get; set; }
    }
}
