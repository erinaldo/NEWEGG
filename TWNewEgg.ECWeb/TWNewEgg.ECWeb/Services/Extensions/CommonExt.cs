using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Resources;
using System.Web;

namespace TWNewEgg.Common
{
    public static class ResourceService
    {
        public static ResourceSet GetResourceSet(ResourceManager manager)
        {
            ResourceSet resourceSet = manager.GetResourceSet(CultureInfo.CurrentUICulture, true, true);
            foreach (DictionaryEntry entry in resourceSet)
            {
                object resourceKey = entry.Key;
                object resource = entry.Value;
            }
            return resourceSet;
        }
    }
}