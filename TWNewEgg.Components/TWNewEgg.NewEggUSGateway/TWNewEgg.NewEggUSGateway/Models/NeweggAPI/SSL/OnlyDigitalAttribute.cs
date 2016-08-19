using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Web.Mvc;

namespace Newegg.Mobile.MvcApplication.Models
{
    /// <summary>
    /// Only Digital authentication.
    /// </summary>
    public class OnlyDigitalAttribute : ValidationAttribute, IClientValidatable
    {
        /// <summary>
        /// Properties string.
        /// </summary>
        private readonly string[] propertiesmatter;

        /// <summary>
        /// Initializes a new instance of the OnlyDigitalAttribute class.
        /// </summary>
        /// <param name="properties">Properties string.</param>
        public OnlyDigitalAttribute(params string[] properties)
        {
            this.propertiesmatter = properties;
        }

        /// <summary>
        /// Get Client Validation Rules.
        /// </summary>
        /// <param name="metadata">Meta data.</param>
        /// <param name="context">Controller Context.</param>
        /// <returns>Return rule.</returns>
        public IEnumerable<ModelClientValidationRule> GetClientValidationRules(ModelMetadata metadata, ControllerContext context)
        {
            var errorMessage = this.FormatErrorMessage(metadata.DisplayName);
            var rule = new ModelClientValidationRule { ErrorMessage = errorMessage, ValidationType = "onlydigital", };
            rule.ValidationParameters["properties"] = string.Join(",", this.propertiesmatter);
            yield return rule;
        }

        /// <summary>
        /// IsValid value.
        /// </summary>
        /// <param name="value">Is Valid.</param>
        /// <param name="validationContext">Validation Context.</param>
        /// <returns>Return null.</returns>
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (this.propertiesmatter == null || this.propertiesmatter.Length < 1)
            {
                return null;
            }

            foreach (var property in this.propertiesmatter)
            {
                var propertyInfo = validationContext.ObjectType.GetProperty(property);
                if (propertyInfo == null)
                {
                    return new ValidationResult(string.Format("unknown property {0}", property));
                }

                var propertyValue = propertyInfo.GetValue(validationContext.ObjectInstance, null);
                if (propertyValue is string && !string.IsNullOrEmpty(propertyValue as string))
                {
                    return null;
                }

                if (propertyValue != null)
                {
                    return null;
                }
            }

            return new ValidationResult(FormatErrorMessage(validationContext.DisplayName));
        }
    }
}
