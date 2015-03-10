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
	public int difficulty;
	public int level;
	
	public double score;
	public double speedmodpref;
	public bool fail;
	
	public SongInfoProfil (string name, string sub, int steps, Difficulty dif, int lvl)
	{
		songName = name;
		subtitle = sub;
		numberOfSteps = steps;
		difficulty = (int)dif;
		level = lvl;
		
		score = -1;
		speedmodpref = -1;
	}

	public bool CompareId(SongInfoProfil sid){
		return sid.songName == this.songName && sid.subtitle == this.subtitle && 
			sid.numberOfSteps == this.numberOfSteps && sid.difficulty == this.difficulty && 
				sid.level == this.level;	
	}
	
	public SongInfoProfil Copy(){
		return new SongInfoProfil(this.songName, this.subtitle, this.numberOfSteps, (Difficulty) this.difficulty, this.level);
	}
}


