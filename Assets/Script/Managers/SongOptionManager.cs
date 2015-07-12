using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SongOptionManager : MonoBehaviour {

	public static SongOptionManager instance;
	
	void Awake()
	{
		if(instance == null){ 
			instance = this;
		}
		DontDestroyOnLoad (this);
	}


	public Song currentSongPlayed;
	public double currentBestPersonal;
	public double currentBestInternet;
	public string currentBestInternetName;

	public List<OptionsMod> currentOptions = new List<OptionsMod>();
	public double speedmodSelected = 2f;
	public double rateSelected = 1;
	public int skinSelected = 0;
	public bool bigNotes = false;
	public int hitJudgeSelected = 2;
	public int scoreJudgeSelected = 2;
	public int lifeJudgeSelected = 2;
	public int deathConditionSelected = 0;
	public int raceSelected = 0;
	public double minimumScoreRace = -1;

	public void convertRaceToMinimumScore()
	{
		switch(raceSelected)
		{
		case 0:
			minimumScoreRace = -1;
			break;
		case 1:
			minimumScoreRace = GameManager.instance.noteBase.Find(c => c.note == ScoreNote.CMINUS).score;
			break;
		case 2:
			minimumScoreRace = GameManager.instance.noteBase.Find(c => c.note == ScoreNote.BMINUS).score;
			break;
		case 3:
			minimumScoreRace = GameManager.instance.noteBase.Find(c => c.note == ScoreNote.AMINUS).score;
			break;
		case 4:
			minimumScoreRace = GameManager.instance.noteBase.Find(c => c.note == ScoreNote.AMINUS).score;
			break;
		case 5:
			minimumScoreRace = GameManager.instance.noteBase.Find(c => c.note == ScoreNote.SMINUS).score;
			break;
		case 6:
			minimumScoreRace = GameManager.instance.noteBase.Find(c => c.note == ScoreNote.BRONZE).score;
			break;
		case 7:
			minimumScoreRace = GameManager.instance.noteBase.Find(c => c.note == ScoreNote.SILVER).score;
			break;
		case 8:
			minimumScoreRace = GameManager.instance.noteBase.Find(c => c.note == ScoreNote.GOLD).score;
			break;
		case 9:
			minimumScoreRace = GameManager.instance.noteBase.Find(c => c.note == ScoreNote.QUAD).score;
			break;
		}
	}
}
