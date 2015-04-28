using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;

[Serializable]
public class SongInfoProfil{
	
	//ID
	public string songName;
	public string subtitle;
	public int numberOfSteps;
	public double keyIdentifier;
	public int difficulty;
	public int level;
	
	public double score;
	public double speedmodpref;
	public bool fail;
	
	public SongInfoProfil (string name, string sub, int steps, Difficulty dif, int lvl, double keyId)
	{
		songName = name;
		subtitle = sub;
		numberOfSteps = steps;
		difficulty = (int)dif;
		level = lvl;
		keyIdentifier = keyId;
		
		score = -1;
		speedmodpref = -1;
	}

	public bool CompareId(SongInfoProfil sid){
		return sid.songName == this.songName && sid.subtitle == this.subtitle && 
			sid.numberOfSteps == this.numberOfSteps && sid.difficulty == this.difficulty && 
				sid.level == this.level && keyIdentifier == sid.keyIdentifier;	
	}

	public string getSongNetId()
	{
		return this.songName.Substring(0, Mathf.Clamp(this.songName.Length, 0, 15))
								  + this.songName.Length <= 15 ? "" : this.songName.Substring(this.songName.Length-15, 15)
		                          + string.IsNullOrEmpty(this.subtitle) ? "" : this.subtitle.Substring(0, Mathf.Clamp(this.songName.Length, 0, 15))
		                          + this.numberOfSteps.ToString() 
		                          + this.difficulty.ToString() 
		                          + this.level.ToString()
								  + (this.keyIdentifier.ToString().Replace(".", "").Replace(",", ""));
	}
	
	public SongInfoProfil Copy(){
		return new SongInfoProfil(this.songName, this.subtitle, this.numberOfSteps, (Difficulty) this.difficulty, this.level, this.keyIdentifier);
	}
}


