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
	public string lastSpeedmodUsed;
	public string lastBPM;
	public bool inBPMMode;
	public int numberOfSkinSelected;
	
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

	
	//Songs
	public List<SongInfoProfil> scoreOnSong;

	
	public Profile ()
	{
		scoreOnSong = new List<SongInfoProfil>();
		lastSpeedmodUsed = "";
		lastBPM = "";
		inBPMMode = false;
		generalVolume = 1f;
		enableBloom = true;
		enablePostProcessEffects = true;
		enableSoundEffects = true;
		enableVisualEffects = true;
		onlyOnGame = true;
		useTheCacheSystem = false;
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
		SongInfoProfil oldSIP = scoreOnSong.FirstOrDefault(c => c.CompareId(thesip));
		if(oldSIP != null){
			if(oldSIP.score < scoreEarned){
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


