using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Reflection;
using System.Runtime.Serialization;
using System.Web.Mvc;
using Newegg.Mobile.MvcApplication.Globalization;
using Global = Newegg.Mobile.MvcApplication.Globalization.Models.AddressDetail;

namespace Newegg.Mobile.MvcApplication.Models
{
    /// <summary>
    /// Initializes a new instance of the UIAddressInfo class.
    /// </summary>
    [DataContract]
    public class UIAddressInfo
    {
        /// <summary>
        /// Gets BuildSelectList.
        /// </summary>
        public static List<SelectListItem> BuildSelectList
        {
            get
            {
                var stateList = new List<SelectListItem>();
                stateList.Add(new SelectListItem { Text = ShippingAddress.Select, Value = string.Empty });
                PropertyInfo[] properties = typeof(StateList).GetProperties();
                foreach (var item in properties)
                {
                    if (item.PropertyType == typeof(string))
                    {
                        var state = new SelectListItem();
                        state.Text = item.GetValue(properties, null).ToString();
                        state.Value = item.Name;
                        stateList.Add(state);
                    }
                }

                return stateList;
            }
        }

        /// <summary>
        /// Gets or sets Id.
        /// </summary>
        [DataMember(Name = "Id")]
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets FirstName.
        /// </summary>
        [Required(ErrorMessageResourceType = typeof(Global.AddressDetail), ErrorMessageResourceName = "FirstName")]
        [RegularExpression(@"\S*", ErrorMessageResourceType = typeof(Global.AddressDetail), ErrorMessageResourceName = "FirstNameCharacters")]
        [StringLength(25, ErrorMessageResourceType = typeof(Global.AddressDetail), ErrorMessageResourceName = "FirstNameLength")]
        public string FirstName { get; set; }

        /// <summary>
        /// Gets or sets MI.
        /// </summary>
        [DataMember(Name = "MI")]
        [RegularExpression(@"\S*", ErrorMessageResourceType = typeof(Global.AddressDetail), ErrorMessageResourceName = "MidNameCharacters")]
        [StringLength(2, ErrorMessageResourceType = typeof(Global.AddressDetail), ErrorMessageResourceName = "MINameLength")]
        public string MI { get; set; }

        /// <summary>
        /// Gets or sets LastName.
        /// </summary>
        [Required(ErrorMessageResourceType = typeof(Global.AddressDetail), ErrorMessageResourceName = "LastName")]
        [RegularExpression(@"\S*", ErrorMessageResourceType = typeof(Global.AddressDetail), ErrorMessageResourceName = "LastNameCharacters")]
        [StringLength(25, ErrorMessageResourceType = typeof(Global.AddressDetail), ErrorMessageResourceName = "LastNameLength")]
        public string LastName { get; set; }

        /// <summary>
        /// Gets or sets ContactWith.
        /// </summary>
        [DataMember(Name = "ContactWith")]
        public string ContactWith
        {
            get
            {
                string contactWith = null;
                contactWith += this.FirstName;
                contactWith += !string.IsNullOrEmpty(this.MI) ? " " + this.MI : string.Empty;
                contactWith += " " + this.LastName;
                return contactWith;
            }

            set
            {
                if (value != null && value.IndexOf(' ') > -1)
                {
                    string[] arr = value.Split(' ');
                    if (arr.Length == 2)
                    {
                        this.FirstName = arr[0];
                        this.LastName = arr[1];
                    }
                    else if (arr.Length == 3)
                    {
                        this.FirstName = arr[0];
                        this.MI = arr[1];
                        this.LastName = arr[2];
                    }
                    else if (arr.Length > 3)
                    {
                        this.LastName = string.Empty;
                        this.FirstName = arr[0];
                        this.MI = arr[1];
                        for (int i = 2; i < arr.Length; i++)
                        {
                            this.LastName += arr[i] + " ";
                        }
                    }
                }
                else
                {
                    this.FirstName = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets ShippingContactWith.
        /// </summary>
        [DataMember(Name = "ShippingContactWith")]
        public string ShippingContactWith { get; set; }

        /// <summary>
        /// Gets or sets Address1.
        /// </summary>
        [Required(ErrorMessageResourceType = typeof(Global.AddressDetail), ErrorMessageResourceName = "Address1")]
        [StringLength(40, ErrorMessageResourceType = typeof(Global.AddressDetail), ErrorMessageResourceName = "Address1Length")]
        [DataMember(Name = "Address1")]
        public string Address1 { get; set; }

        /// <summary>
        /// Gets or sets Address2.
        /// </summary>
        [DataMember(Name = "Address2")]
        [StringLength(40, ErrorMessageResourceType = typeof(Global.AddressDetail), ErrorMessageResourceName = "Address2Length")]
        public string Address2 { get; set; }

        /// <summary>
        /// Gets or sets State.
        /// </summary>
        [Required(ErrorMessageResourceType = typeof(Global.AddressDetail), ErrorMessageResourceName = "State")]
        [DataMember(Name = "State")]
        public string State { get; set; }

        /// <summary>
        /// Gets or sets City.
        /// </summary>
        [Required(ErrorMessageResourceType = typeof(Global.AddressDetail), ErrorMessageResourceName = "City")]
        [DataMember(Name = "City")]
        [StringLength(20, ErrorMessageResourceType = typeof(Global.AddressDetail), ErrorMessageResourceName = "CityLength")]
        public string City { get; set; }

        /// <summary>
        /// Gets or sets Country.
        /// </summary>
        [DataMember(Name = "Country")]
        public string Country { get; set; }

        /// <summary>
        /// Gets or sets ZipCode.
        /// </summary>        
        [Required(ErrorMessageResourceType = typeof(Global.AddressDetail), ErrorMessageResourceName = "ZipCode")]
        [DataMember(Name = "ZipCode")]
        [StringLength(10, ErrorMessageResourceType = typeof(Global.AddressDetail), ErrorMessageResourceName = "ZipCodeLength")]
        public string ZipCode { get; set; }

        /// <summary>
        /// Gets or sets CompanyName.
        /// </summary>
        [DataMember(Name = "CompanyName")]
        [StringLength(50, ErrorMessageResourceType = typeof(Global.AddressDetail), ErrorMessageResourceName = "CompanyNameLength")]
        public string CompanyName { get; set; }

        /// <summary>
        /// Gets or sets PhoneArea.
        /// </summary>
        [NonNullValue("PhoneArea", "PhoneNum1", "PhoneNum2", ErrorMessageResourceType = typeof(Global.AddressDetail), ErrorMessageResourceName = "Phone")]
        [OnlyDigital("PhoneArea", "PhoneNum1", "PhoneNum2", "Ext", ErrorMessageResourceType = typeof(Global.AddressDetail), ErrorMessageResourceName = "OnlyDigital")]
        [PhoneFormat("PhoneArea", "PhoneNum1", "PhoneNum2", ErrorMessageResourceType = typeof(Global.AddressDetail), ErrorMessageResourceName = "PhoneFormat")]
        [DataMember(Name = "PhoneArea")]
        public string PhoneArea { get; set; }

        /// <summary>
        /// Gets or sets PhoneNum1.
        /// </summary>
        [DataMember(Name = "PhoneNum1")]
        public string PhoneNum1 { get; set; }

        /// <summary>
        /// Gets or sets PhoneNum2.
        /// </summary>
        [DataMember(Name = "PhoneNum2")]
        public string PhoneNum2 { get; set; }

        /// <summary>
        /// Gets or sets Ext.
        /// </summary>
        [DataMember(Name = "Ext")]
        public string Ext { get; set; }

        /// <summary>
        /// Gets or sets Phone.
        /// </summary>
        [DataMember(Name = "Phone")]
        public string Phone
        {
            get
            {
                string phone = null;
                phone = this.PhoneArea + "-" + this.PhoneNum1 + "-" + this.PhoneNum2;
                if (this.Ext != null && this.Ext.Length > 0)
                {
                    phone += "EXT" + this.Ext;
                }

                return phone;
            }

            set
            {
                if (value != null)
                {
                    value = value.Replace("EXT", "-");
                    if (value.IndexOf('-') > -1)
                    {
                        string[] arr = value.Split('-');
                        if (arr.Length == 3)
                        {
                            this.PhoneArea = arr[0];
                            this.PhoneNum1 = arr[1];

                            if (arr[2].Length > 4)
                            {
                                this.PhoneNum2 = arr[2].Substring(0, 4);
                                this.Ext = arr[2].Substring(4);
                            }
                            else
                            {
                                this.PhoneNum2 = arr[2];
                                this.Ext = string.Empty;
                            }
                        }
                        else if (arr.Length == 4)
                        {
                            this.PhoneArea = arr[0];
                            this.PhoneNum1 = arr[1];
                            this.PhoneNum2 = arr[2];
                            this.Ext = arr[3];
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Gets or sets Fax.
        /// </summary>
        [DataMember(Name = "Fax")]
        public string Fax { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether IsDefault.
        /// </summary>
        [DataMember(Name = "IsDefault")]
        public bool IsDefault { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether IsAlsoBillingAddress.
        /// </summary>
        [DataMember(Name = "IsAlsoBillingAddress")]
        public bool IsAlsoBillingAddress { get; set; }

        /// <summary>
        /// Gets or sets LoginName.
        /// </summary>
        [DataMember(Name = "LoginName")]
        public string LoginName { get; set; }

        /// <summary>
        /// Gets or sets AddressLabel.
        /// </summary>
        [DataMember(Name = "AddressLabel")]
        [StringLength(50, ErrorMessageResourceType = typeof(Global.AddressDetail), ErrorMessageResourceName = "AddressLabelLength")]
        public string AddressLabel { get; set; }

        /// <summary>
        /// Gets or sets AddressSource.
        /// </summary>
        [DataMember(Name = "AddressSource")]
        public string AddressSource { get; set; }
    }
}