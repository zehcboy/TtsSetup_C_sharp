using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using Java.Util;

namespace TtsSetup
{
    internal class TtsSentenceExtractor
    {
        public const String SIMPLE_HTML_ID_BARE = "Hyperionics-SimpleHTML";
        public const String SIMPLE_HTML_ID = "<!-- " + SIMPLE_HTML_ID_BARE;

        class StringPair
        {
            public readonly Regex Pat;
            public readonly String Str;

            public StringPair(Regex pat, String str)
            {
                Pat = pat;
                Str = str;
            }
        }

        private static String[] _abbrev;
        private static List<StringPair> _replace;
        private static readonly String[] SimpleMarkup = { "<br>", "<p>", "</p>", "<h1>", "</h1>", "<h2>", "</h2>", "<h3>", "</h3>", "<h4>", "</h4>", "<h5>", "</h5>", "<h6>", "</h6>" };
        private static String _lastLang;
        private static bool _breakSentences = false;

        private static void InitExtractor(Locale loc)
        {
            if (loc == null) return;
            String lang = loc.ISO3Language.ToLower();
            if (lang.Equals(_lastLang))
                return;
            //            TextToSpeech currentTTS = SpeakService.myTTS;
            //            if (currentTTS != null) {
            //                try {
            //                    // Stupid: getCurrentEngine() of TTS is hidden. Try to get current engine if we can -
            //                    // at some point we may not be using the default TTS engine...
            //                    java.lang.reflect.Method method;
            //                    method = SpeakService.myTTS.getClass().getMethod("getCurrentEngine");
            //                    String currEngine = (String) method.invoke(currentTTS);
            //                    _breakSentences = currEngine.equals("nuance.tts");
            //                } catch (Exception e) {
            //                    _breakSentences = currentTTS.getDefaultEngine().equals("nuance.tts");
            //                }
            //            } else {
            //                _breakSentences = false;
            //            }

            _lastLang = lang;
            _abbrev = null;
            _replace = null;
            String abbreviations = TdApp.ConvertAssetToString("abbrev-" + lang + ".txt");
            if (abbreviations != null)
            {
                _abbrev = new Regex("\n", RegexOptions.Multiline).Split(abbreviations.Replace("\r", ""));
            }

            String replacements = TdApp.ConvertAssetToStringNoComments("replace-" + lang + ".txt");
            if (replacements != null)
            {
                String[] rpl = new Regex("\n", RegexOptions.Multiline).Split(replacements.Replace("\r", ""));
                _replace = new List<StringPair>();
                foreach (String s in rpl)
                {
                    bool ignoreCase = true;
                    String ss = s;
                    if (ss.StartsWith("^"))
                    {
                        ignoreCase = false;
                        ss = ss.Substring(1).Trim();
                    }
                    int split = ss.IndexOf(" : ", StringComparison.Ordinal);
                    if (split > 0)
                    {
                        String s1 = (ignoreCase ? "(?i)" : "") + "\\Q" + ss.Substring(0, split) + "\\E";
                        String s2 = ss.Substring(split + 3);
                        _replace.Add(new StringPair(new Regex(s1, RegexOptions.Compiled), s2));
                    }
                }
                if (_replace.Count == 0)
                    _replace = null;
            }
        }

        public static List<String> Extract(String sTextEntire, String lang)
        {
            Locale loc = lang != null ? new Locale(lang) : Locale.Default;
            InitExtractor(loc);
            var ss = new List<String>();
            String pattern;

            // First split into paragraphs
            if (sTextEntire.StartsWith(SIMPLE_HTML_ID))
            {
                sTextEntire = sTextEntire.Substring(sTextEntire.IndexOf("-->", System.StringComparison.Ordinal) + 3);
                pattern = "<br>|<h[1-6]>|</h[1-6]>|<p>|</p>|<blockquote>|</blockquote>|<div>|</div>|<body>|</body>"; // Pattern.MULTILINE;
            }
            else
            {
                sTextEntire = sTextEntire.Replace("\r", "");
                pattern = "\n\n"; // Pattern.MULTILINE;
            }

            var wl = new List<String>();

            // Now split each paragraph into word lists and use Build() version...
            var regSpace = new Regex("[\\s]", RegexOptions.Multiline | RegexOptions.Compiled);
            Match m0 = new Regex(pattern, RegexOptions.Multiline).Match(sTextEntire);
            int st0 = 0;
            String tag = null;
            while (m0.Success)
            {
                int ed0 = m0.Index;
                int numSentences = 0;
                if (ed0 > st0)
                {
                    // m0.Value is our starting or enging tag, e.g. "<h1>"
                    String sParagraph = sTextEntire.Substring(st0, ed0 - st0);

                    wl.Clear();
                    Match m = regSpace.Match(sParagraph);
                    int st = 0, ed;
                    while (m.Success)
                    {
                        ed = m.Index;
                        if (ed > st)
                        {
                            wl.Add(sParagraph.Substring(st, ed - st));
                        }
                        st = m.Index + m.Length;
                        m = m.NextMatch();
                    }
                    ed = sParagraph.Length;
                    if (ed > st)
                    {
                        wl.Add(sParagraph.Substring(st, ed - st));
                    }
                    var sentences = Build(wl, loc);
                    numSentences = sentences.Count;
                    if (numSentences > 0)
                    {
                        if (tag != null && !tag.StartsWith("</"))
                            ss.Add(tag);
                        foreach (string t in sentences)
                            ss.Add(t);
                    }
                }
                tag = sTextEntire.Substring(m0.Index, m0.Length); // tag for the next loop pass
                if (numSentences > 0 && tag.StartsWith("</"))
                    ss.Add(tag);
                st0 = m0.Index + m0.Length;

                m0 = m0.NextMatch();
            }
            return ss;
        }

        private static List<String> Build(List<String> wl, Locale loc)
        {
            var ss = new List<String>();
            var currSent = new StringBuilder();
            int i;

            for (i = 0; i < wl.Count; i++)
            {
                String w = wl[i];
                bool isAbbrev = false;
                if (w.Length == 2 && w.EndsWith(".") && char.IsUpper(w[0]))
                {
                    w = w.Substring(0, 1) + " ";
                }
                else
                {
                    if (loc.ISO3Language.Equals("pol"))
                    {
                        // do this only for Polish, else for Spanish - Ivona pronounces regular '-' as "geeon"
                        w = w.Replace('\u2013', '-'); // dec 8211, "en dash" or long dash, Ivona PL reads as "przecinek"
                        w = w.Replace('\u2014', '-'); // dec 8211, 'EM DASH', Ivona PL reads as "przecinek"
                    }
                    w = w.Replace('\u00A0', ' '); // dec 160, no-break space
                    w = w.Replace("\u200B", " ");  // dec. 8203, 'zero width space' (do not _replace with empty, or we may get w empty and crash)
                    w = w.Replace('\u2019', '\''); // RIGHT SINGLE QUOTATION MARK (U+2019), mis-pronounced by Google TTS?
                    if (w[0] == '\u2026')  // dec 8230 ellipses ... remove at start
                        w = " " + w.Substring(1);
                    isAbbrev = IsAbbreviation(w);
                }
                bool endSentence = false;
                char lastCh = w[w.Length - 1];
                if (!isAbbrev)
                {
                    endSentence = lastCh == '.' && (i == wl.Count - 1 || !wl[i + 1].Equals(".")) ||
                            lastCh == '!' || lastCh == '?';
                    if (!endSentence && w.Length > 1 && (lastCh == '"' || lastCh == 0x201D || lastCh == ')'))
                    {
                        lastCh = w[w.Length - 2];
                        endSentence = lastCh == '.' && (i == wl.Count - 1 || !wl[i + 1].Equals(".")) ||
                                lastCh == '!' || lastCh == '?';
                    }
                    if (!endSentence)
                    {
                        endSentence = w.Contains(".[") || w.Contains("![") || w.Contains("?[");
                    }
                    if (!endSentence)
                    {
                        foreach (string t in SimpleMarkup)
                        {
                            if (w.Equals(t))
                            {
                                endSentence = true;
                                break;
                            }
                        }
                    }
                }
                // Split long sentences, Nuance TTS does not speak beyond 592 characters length...
                // at the next comma, hyphen, ( or ), ellipses..., colon :, semicolon ;
                if (_breakSentences && !endSentence && currSent.Length > 500)
                {
                    endSentence = lastCh == ',' || lastCh == '-' || lastCh == '(' || lastCh == ')' ||
                            lastCh == ':' || lastCh == ';' || currSent.Length > 580;

                }
                if (currSent.Length > 0 && (w.Length > 1 || !endSentence))
                {
                    currSent.Append(" ");
                }
                currSent.Append(w);
                if (endSentence || i == wl.Count - 1)
                {
                    ss.Add(currSent.ToString());
                    currSent.Clear();
                }
            }

            return ss;
        }

        private static bool IsAbbreviation(String inStr)
        {
            if (_abbrev != null)
            {
                foreach (String subs in _abbrev)
                {
                    if (inStr.Contains(subs)) // this way, inStr may be like "(e.g." and it will still match "e.g."
                        return true;
                }
            }
            return false;
        }
    }
}