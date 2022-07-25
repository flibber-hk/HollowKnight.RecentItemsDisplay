using System;
using System.Collections.Generic;
using ItemChanger;
using Modding;

namespace RecentItemsDisplay
{
    public static class LanguageText
    {
        public static void Hook()
        {
            ModHooks.LanguageGetHook += RecentItemsLocalization;
        }

        private static string RecentItemsLocalization(string key, string sheetTitle, string orig)
        {
            if (!(string.IsNullOrEmpty(orig) || orig.StartsWith("#!#")))
            {
                // Has already been localized
                return orig;
            }

            if (RecentItemsLanguage.TryGetValue(new(sheetTitle, key), out string ret))
            {
                return ret;
            }

            return orig;
        }

        private static readonly Dictionary<LanguageKey, string> RecentItemsLanguage = new()
        {
            [new("RecentItems", "GRUBFATHER_MAIN")] = "Grubfather",
            [new("RecentItems", "DEFAULT_MESSAGE_FORMAT")] = "{0}<br>from {1}",

        };
    }
}
