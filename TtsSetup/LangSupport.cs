using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Java.Util;

namespace TtsSetup
{
    class LangSupport
    {
        public static Locale LocaleFromString(String languageCode)
        {
            int n = languageCode.IndexOf("-", System.StringComparison.Ordinal);
            if (n <= 0)
            {
                return new Locale(languageCode);
            }
            else
            {
                String lang = languageCode.Substring(0, n);
                String country = languageCode.Substring(n + 1);
                n = country.IndexOf("-", System.StringComparison.Ordinal);
                String variant = null;
                if (n > 0)
                {
                    variant = country.Substring(n + 1);
                    country = country.Substring(0, n);
                }
                return variant != null ? new Locale(lang, country, variant) : new Locale(lang, country);
            }
        }

        public static void SetSelectedTtsEng(String packageName)
        {
//            SharedPreferences.Editor ed = getPrefs().edit();
//            if (packageName == null)
//            {
//                selectedTtsEng = null;
//                ed.remove(ENG_PREF);
//            }
//            else
//            {
//                selectedTtsEng = packageName;
//                ed.putString(ENG_PREF, selectedTtsEng);
//            }
//            ed.commit();
        }

        public static void SetPrefferedVoice(String iso3lang, String voi_eng)
        {
//            // voi_eng like "eng_usa_heather|com.acapelagroup.android.tts"
//            SharedPreferences.Editor ed = getPrefs().edit();
//            if (voi_eng == null)
//            {
//                ed.remove("prefv_" + iso3lang);
//            }
//            else
//            {
//                ed.putString("prefv_" + iso3lang, voi_eng);
//            }
//            ed.commit();
        }
    }
}