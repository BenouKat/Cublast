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
	
	//Profiles
	public int portPref;
	
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
		portPref = 25000;
		generalVolume = 1f;
		enableBloom = true;
		enableDepthOfField = true;
		onlyOnGame = true;
		useTheCacheSystem = false;
		antiAliasing = QualitySettings.antiAliasing;
		KeyCodeUp = KeyCode.UpArrow;
		KeyCodeDown = KeyCode.DownArrow;
		KeyCodeLeft = KeyCode.LeftArrow;
		KeyCodeRight = KeyCode.RightArrow;
		SecondaryKeyCodeUp = KeyCode.Z;
		SecondaryKeyCodeDown = KeyCode.S;
		SecondaryKeyCodeLeft = KeyCode.Q;
		SecondaryKeyCodeRight = KeyCode.D;
		loadOptions();
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
	
	public void loadOptions(){
	
		GameManager.instance.userGOS = this.userGOS;
		GameManager.instance.mouseMolSpeed = this.mouseMolSpeed;
		GameManager.instance.dancepadMode = this.dancepadMode;
		GameManager.instance.quickMode = this.quickMode;
		GameManager.instance.generalVolume = this.generalVolume;
		GameManager.instance.enableBloom = this.enableBloom;
		GameManager.instance.enableDepthOfField = this.enableDepthOfField;
		GameManager.instance.onlyOnGame = this.onlyOnGame;
		GameManager.instance.antiAliasing = this.antiAliasing;
		GameManager.instance.useTheCacheSystem = this.useTheCacheSystem;
		
		GameManager.instance.KeyCodeUp = this.KeyCodeUp;
		GameManager.instance.KeyCodeDown = this.KeyCodeDown;
		GameManager.instance.KeyCodeLeft = this.KeyCodeLeft;
		GameManager.instance.KeyCodeRight = this.KeyCodeRight;
		GameManager.instance.SecondaryKeyCodeUp = this.SecondaryKeyCodeUp;
		GameManager.instance.SecondaryKeyCodeDown = this.SecondaryKeyCodeDown;
		GameManager.instance.SecondaryKeyCodeLeft = this.SecondaryKeyCodeLeft;
		GameManager.instance.SecondaryKeyCodeRight = this.SecondaryKeyCodeRight;
	}
	
	public void saveOptions(){
	
		this.userGOS = GameManager.instance.userGOS;
		this.mouseMolSpeed = GameManager.instance.mouseMolSpeed;
		this.dancepadMode = GameManager.instance.dancepadMode;
		this.quickMode = GameManager.instance.quickMode;
		this.generalVolume = GameManager.instance.generalVolume;
		this.enableBloom = GameManager.instance.enableBloom;
		this.enableDepthOfField = GameManager.instance.enableDepthOfField;
		this.onlyOnGame = GameManager.instance.onlyOnGame;
		this.antiAliasing = GameManager.instance.antiAliasing;
		this.useTheCacheSystem = GameManager.instance.useTheCacheSystem;
		
		this.KeyCodeUp = GameManager.instance.KeyCodeUp;
		this.KeyCodeDown = GameManager.instance.KeyCodeDown;
		this.KeyCodeLeft = GameManager.instance.KeyCodeLeft;
		this.KeyCodeRight = GameManager.instance.KeyCodeRight;
		this.SecondaryKeyCodeUp = GameManager.instance.SecondaryKeyCodeUp;
		this.SecondaryKeyCodeDown = GameManager.instance.SecondaryKeyCodeDown;
		this.SecondaryKeyCodeLeft = GameManager.instance.SecondaryKeyCodeLeft;
		this.SecondaryKeyCodeRight = GameManager.instance.SecondaryKeyCodeRight;
	}
	
}


