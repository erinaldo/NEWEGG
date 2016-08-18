using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TWNewEgg.ECWeb.Utility
{
    /// <summary>
    /// Http Context Item.
    /// </summary>
    public static class ContextItemUility
    {
        /// <summary>
        /// Get Item Value.
        /// </summary>
        /// <typeparam name="T">T type.</typeparam>
        /// <param name="itemKey">Item key.</param>
        /// <returns>HttpContext Current Item's Content.</returns>
        public static T GetItemValue<T>(string itemKey)
        {
            try
            {
                if (HttpContext.Current.Items[itemKey] != null && HttpContext.Current.Items[itemKey].GetType() == typeof(T))
                {
                    return (T)HttpContext.Current.Items[itemKey];
                }

                return default(T);
            }
            catch (Exception)
            {
                return default(T);
            }
        }

        /// <summary>
        /// Get Item Value.
        /// </summary>
        /// <param name="itemKey">Item Key.</param>
        /// <returns>HttpContext Current Item's Content.</returns>
        public static string GetItemValue(string itemKey)
        {
            try
            {
                return HttpContext.Current.Items[itemKey].ToString();
            }
            catch (Exception)
            {
                return string.Empty;
            }
        }

        /// <summary>
        /// Set Item Value.
        /// </summary>
        /// <param name="itemKey">Item Key.</param>
        /// <param name="content">Items Content.</param>
        public static void SetItemValue(string itemKey, object content)
        {
            try
            {
                HttpContext.Current.Items[itemKey] = content;
            }
            catch (Exception)
            {
            }
        }
    }
}