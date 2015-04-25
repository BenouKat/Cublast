using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;

[Serializable]
public class Profile{
	
	//Parameters
	public float globalOffsetSeconds;
	public KeyCode KeyCodeUp;
	public KeyCode KeyCodeDown;
	public KeyCode KeyCodeLeft;
	public KeyCode KeyCodeRight;
	public KeyCode SecondaryKeyCodeUp;
	public KeyCode SecondaryKeyCodeDown;
	public KeyCode SecondaryKeyCodeLeft;
	public KeyCode SecondaryKeyCodeRight;
	public string lastSpeedmodUsed;
	public string lastBPM;
	public bool inBPMMode;
	public int numberOfSkinSelected;
	
	//general
	public float userGOS;
	public int mouseMolSpeed;
	public bool dancepadMode;
	public bool quickMode;
	public bool useTheCacheSystem;
	
	//Audio
	public float generalVolume;
	
	//Video
	public bool enableBloom;
	public bool enableDepthOfField;
	public bool onlyOnGame;
	public int antiAliasing;
	
	
	//Songs
	public List<SongInfoProfil> scoreOnSong;
	
	//Stat
	public double gameTime;
	public int victoryOnline;

	
	public Profile ()
	{
		scoreOnSong = new List<SongInfoProfil>();
		gameTime = 0f;
		victoryOnline = 0;
		lastSpeedmodUsed = "";
		lastBPM = "";
		inBPMMode = false;
		userGOS = 0.0f;
		mouseMolSpeed = 1;
		dancepadMode = false;
		quickMode = false;
		generalVolume = 1f;
		enableBloom = true;
		enableDepthOfField = true;
		onlyOnGame = true;
		useTheCacheSystem = false;
		antiAliasing = 0;
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
		if(scoreOnSong.Any(c => c.CompareId(thesip))){
			var theold = scoreOnSong.FirstOrDefault(c => c.CompareId(thesip));
			if(theold.score < scoreEarned){
				scoreOnSong.Remove(theold);
				scoreOnSong.Add(thesip);	
			}else{
				theold.speedmodpref = speedmodPref;
			}
		}else{
			scoreOnSong.Add(thesip);	
		}
		
	}
	
	public void updateGameTime(double gt){
		gameTime += gt;
	}
	
	public void saveOptions(){
	
		ProfileManager.instance.SaveProfile ();
	}
	
}


