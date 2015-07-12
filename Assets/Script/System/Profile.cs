using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;

[Serializable]
public class Profile{
	
	//Inputs - Options
	public KeyCode KeyCodeUp;
	public KeyCode KeyCodeDown;
	public KeyCode KeyCodeLeft;
	public KeyCode KeyCodeRight;
	public KeyCode SecondaryKeyCodeUp;
	public KeyCode SecondaryKeyCodeDown;
	public KeyCode SecondaryKeyCodeLeft;
	public KeyCode SecondaryKeyCodeRight;

	//Recording
	public float lastSpeedmod;
	public float lastBPM;
	public bool inBPMMode;
	public int skinSelected;
	
	//general - Options
	public float globalOffsetSeconds;
	public bool useTheCacheSystem;
	//Option - Generate / Clear
	public bool enableSoundEffects;
	public bool enableVisualEffects;
	//Options - Disconnect / Connect
	
	//Audio/Video - Options
	public float generalVolume;
	public bool enableFXAA;
	public int antiAliasing;
	public bool enableBloom;
	public bool enablePostProcessEffects;
	public bool onlyOnGame;
	public bool enableVSync;

	
	//Songs
	public List<SongInfoProfil> scoreOnSong;

	
	public Profile ()
	{
		scoreOnSong = new List<SongInfoProfil>();
		lastSpeedmod = 1.5f;
		lastBPM = 250f;
		inBPMMode = false;
		skinSelected = 0;
		generalVolume = 1f;
		enableBloom = true;
		enablePostProcessEffects = true;
		enableSoundEffects = true;
		enableVisualEffects = true;
		onlyOnGame = true;
		useTheCacheSystem = false;
		enableVSync = false;
		antiAliasing = 0;
		enableFXAA = true;
		KeyCodeUp = KeyCode.UpArrow;
		KeyCodeDown = KeyCode.DownArrow;
		KeyCodeLeft = KeyCode.LeftArrow;
		KeyCodeRight = KeyCode.RightArrow;
		SecondaryKeyCodeUp = KeyCode.U;
		SecondaryKeyCodeDown = KeyCode.T;
		SecondaryKeyCodeLeft = KeyCode.E;
		SecondaryKeyCodeRight = KeyCode.O;
	}
	
	public void saveASong(SongInfoProfil sip, float scoreEarned, double speedmodPref, bool fail){
		SongInfoProfil thesip = sip.Copy();
		thesip.score = scoreEarned;
		thesip.speedmodpref = speedmodPref;
		thesip.fail = fail;
		SongInfoProfil oldSIP = scoreOnSong.FirstOrDefault(c => c.CompareId(thesip));
		if(oldSIP != null){
			if(oldSIP.score < scoreEarned && (!fail || !oldSIP.fail)){
				scoreOnSong.Remove(oldSIP);
				scoreOnSong.Add(thesip);
			}else{
				oldSIP.speedmodpref = speedmodPref;
			}
		}else{
			scoreOnSong.Add(thesip);	
		}
		saveOptions();
	}
	
	public void saveOptions(){
	
		ProfileManager.instance.SaveProfile ();
	}
	
}


