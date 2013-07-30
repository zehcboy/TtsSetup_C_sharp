using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace TtsSetup
{
    class TdApp : Application
    {
        public static ISharedPreferences Prefs { get; private set; }
        public static Context App { get; private set; }

        private const String PREFS_NAME = "TtsDiscovery";

        public override void OnCreate()
        {
            base.OnCreate();
            App = this;
            Prefs = GetSharedPreferences(PREFS_NAME, (FileCreationMode)5); // MultiProcess | WorldReadable
        }

        public static String ConvertAssetToStringNoComments(String assetName) // strips lines starting with ;
        {
            StringBuilder text = null;
            try
            {
                using (var str = new StreamReader(Application.Context.Assets.Open(assetName)))
                {
                    text = new StringBuilder();
                    while (str.Peek() > 0)
                    {
                        String line = str.ReadLine();
                        if (line == null) break;
                        line = line.Trim();
                        if (!line.StartsWith(";"))
                        {
                            text.Append(line + "\n");
                        }
                    }
                }
            }
            catch (Exception) { }

            return text == null ? null : text.ToString();
        }

        //public static String ConvertAssetToString(String assetName)
        //{
        //    StringBuilder text = null;
        //    try
        //    {
        //        using (var str = new StreamReader(Application.Context.Assets.Open(assetName)))
        //        {
        //            text = new StringBuilder();
        //            while (str.Peek() > 0)
        //            {
        //                String line = str.ReadLine();
        //                if (line == null) break;
        //                line = line.Trim();
        //                text.Append(line);
        //                text.Append("\n");
        //            }
        //        }
        //    }
        //    catch (Exception) { }

        //    return text == null ? null : text.ToString();
        //}

        public static String ConvertAssetToString(String assetName)
        {
            String text;
            try
            {
                using (var sr = new StreamReader(Application.Context.Assets.Open(assetName)))
                    text = sr.ReadToEnd();
            }
            catch (Exception)
            {
                text = null;
            }
            return text;
        }

    }
}