package com.hyperionics.ttssetup;

import android.app.Activity;
import android.content.Context;
import android.content.Intent;
import android.os.Bundle;
import android.util.Log;
import android.widget.LinearLayout;
import android.view.ViewGroup;
import android.widget.ImageView;
import java.lang.Thread;
import java.util.Timer;
import java.util.TimerTask;

public class TrialSplashScreenb4e0136cf77e4ab6b18bc40a5241134c extends Activity {
	
	private TimerTask task;
	private Bundle initial_bundle;
	
	protected void onCreate (Bundle bundle)
	{
		super.onCreate (bundle);
		this.initial_bundle = bundle;
		ImageView iv = new ImageView (this);
		iv.setImageResource (R.drawable.monoandroidsplash);
		iv.setScaleType (ImageView.ScaleType.FIT_CENTER);
		ViewGroup.LayoutParams ivparams = new ViewGroup.LayoutParams (ViewGroup.LayoutParams.FILL_PARENT, ViewGroup.LayoutParams.FILL_PARENT);
		setContentView (iv);
		
		task = new TimerTask () {
			@Override
			public void run ()
			{
				finish ();
				Intent intent = new Intent (TrialSplashScreenb4e0136cf77e4ab6b18bc40a5241134c.this, ttssetup.TestVoiceAsync.class);
				intent.setFlags (Intent.FLAG_ACTIVITY_NEW_TASK);
				startActivity (intent);
			}
		};
		new Timer ().schedule (task, 3000);
	}
}
