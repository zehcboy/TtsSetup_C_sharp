package ttssetup;


public class TestVoiceAsync_TtsInit
	extends java.lang.Object
	implements
		mono.android.IGCUserPeer,
		android.speech.tts.TextToSpeech.OnInitListener
{
	static final String __md_methods;
	static {
		__md_methods = 
			"n_onInit:(I)V:GetOnInit_IHandler:Android.Speech.Tts.TextToSpeech/IOnInitListenerInvoker, Mono.Android, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null\n" +
			"";
		mono.android.Runtime.register ("TtsSetup.TestVoiceAsync/TtsInit, TtsSetup, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", TestVoiceAsync_TtsInit.class, __md_methods);
	}


	public TestVoiceAsync_TtsInit ()
	{
		super ();
		if (getClass () == TestVoiceAsync_TtsInit.class)
			mono.android.TypeManager.Activate ("TtsSetup.TestVoiceAsync/TtsInit, TtsSetup, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", "", this, new java.lang.Object[] {  });
	}


	public void onInit (int p0)
	{
		n_onInit (p0);
	}

	private native void n_onInit (int p0);

	java.util.ArrayList refList;
	public void monodroidAddReference (java.lang.Object obj)
	{
		if (refList == null)
			refList = new java.util.ArrayList ();
		refList.add (obj);
	}

	public void monodroidClearReferences ()
	{
		if (refList != null)
			refList.clear ();
	}
}
