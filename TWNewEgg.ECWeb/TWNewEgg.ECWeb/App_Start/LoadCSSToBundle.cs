using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Optimization;
using TWNewEgg.Framework.Cache;
using TWNewEgg.ECWeb.Models.CSSCache;

namespace TWNewEgg.ECWeb.App_Start
{
    public class LoadCSSToBundle
    {
        public static void UpdateCSSBundle(BundleCollection bundles)
        {
            if (bundles != null)
            {
                var cssFileInfo = CacheConfiguration.Instance.GetFromCache<CSSConfig>(CONSTNMAE.CSSCONFIGNAME, null, false);
                if (cssFileInfo != null && cssFileInfo.cssFiles != null && cssFileInfo.cssFiles.Count > 0)
                {
                    foreach (var singleCSSFile in cssFileInfo.cssFiles)
                    {
                        if (string.IsNullOrEmpty(singleCSSFile.cssKeyName) || singleCSSFile.cssLocations.Count == 0)
                        {
                            continue;
                        }
                        if (bundles.Where(x => x.Path == singleCSSFile.cssKeyName).Count() == 0)
                        {
                            bundles.Add(new StyleBundle(singleCSSFile.cssKeyName).Include(singleCSSFile.cssLocations.Select(x => x.cssValue).ToArray()));
                        }
                    }
                }
            }
        }
    }
}