using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;

namespace TtsSetup
{
    [Activity(Label = "Tts Tests (C#)", MainLauncher = true, Icon = "@drawable/icon", Theme = "@android:style/Theme.Black")]
    public class MainActivity : Activity
    {
        public const String TAG = "XTtsSetup";
        long grandTotal = 0;
        long grandTotalWithFiles = 0;
        int numRuns = 0;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            SetContentView(Resource.Layout.main);

            FindViewById<Button>(Resource.Id.testButton).Click += OnTestClick;
            FindViewById<Button>(Resource.Id.setupButton).Click += OnSetupClick;
        }

        protected override void OnPause()
        {
            base.OnPause();
            Log.Debug(TAG, "-----------------------------------------------------------------------------");
            Log.Debug(TAG, "Grand total time (" + numRuns + " runs): " + grandTotal + " ms" + ", with file reading total: " +
                    grandTotalWithFiles + " ms");
            Log.Debug(TAG, "-----------------------------------------------------------------------------");
            Log.Debug(TAG, ".");
            Log.Debug(TAG, ".");
            Log.Debug(TAG, ".");

        }

        void OnSetupClick(object sender, EventArgs e)
        {
#if !API8
            var intent = new Intent(this, typeof(VoiceSelectorActivity));
            StartActivity(intent);
#endif
        }

        void OnTestClick(object sender, EventArgs e)
        {
            String[] fname =
                {
                    //"BookZero.txt",
                    "Abbaye.htm",
                    "Boeing.htm",
                    "Edward.htm",
                    "Imperial_German_Navy.htm",
                    "Kosciuszko.htm",
                    "Larry.htm",
                    "ManualRus.htm",
                    "Nebezpeci.htm",
                    "Netherlands.htm",
                    "Rus2.htm"
                };
            String[] lang =
                {
                    //"eng",
                    "fra",
                    "eng",
                    "eng",
                    "eng",
                    "pol",
                    "eng",
                    "rus",
                    "cze",
                    "eng",
                    "rus"
                };
            long totalTime = 0;
            var sw0 = new Stopwatch();
            sw0.Start();
            for (int i = 0; i < fname.Length; i++)
            {
                String text = TdApp.ConvertAssetToString(fname[i]);
                var sw = new Stopwatch();
                sw.Start();
                var sentences = TtsSentenceExtractor.Extract(text, lang[i]);
                sw.Stop();
                Log.Debug(TAG,
                          fname[i] + ": sentences.Count = " + sentences.Count + ", time = " + sw.ElapsedMilliseconds);
                totalTime += sw.ElapsedMilliseconds;
            }
            sw0.Stop();
            grandTotal += totalTime;
            grandTotalWithFiles += sw0.ElapsedMilliseconds;
            numRuns++;
            Log.Debug(TAG, "----------------------------------------------------------------");
            Log.Debug(TAG, "Total time: " + totalTime + " ms" + ", with file reading total: " + sw0.ElapsedMilliseconds);
            Log.Debug(TAG, ".");
            Log.Debug(TAG, ".");
            Log.Debug(TAG, ".");
        }
    }
}