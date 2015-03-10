using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;
using System.IO;
using System.Diagnostics;

public class GameManager : MonoBehaviour{

	
	
	
	//DATA GAME
	public Dictionary<string, float> ScoreWeightValues;
	public Dictionary<string, float> LifeWeightValues;
	public Dictionary<Precision, double> PrecisionValues;
	
	//Debug
	[HideInInspector] public String DEBUGPATH = Application.isEditor ? "/../" : "/";
	
	//KEYS
	public KeyCode KeyCodeUp;
	public KeyCode KeyCodeDown;
	public KeyCode KeyCodeLeft;
	public KeyCode KeyCodeRight;
	public KeyCode SecondaryKeyCodeUp;
	public KeyCode SecondaryKeyCodeDown;
	public KeyCode SecondaryKeyCodeLeft;
	public KeyCode SecondaryKeyCodeRight;
	
	//Defaults values
	public Color[] diffColor = new Color[6] { new Color(0.68f, 0.40f, 1f, 1f), new Color(0.396f, 1f, 0.415f, 1f) ,
		new Color(0.965f, 1f, 0.47f, 1f), new Color(1f, 0.208f, 0.208f, 1f), 
		new Color(0.208f, 0.57f, 1f, 1f), new Color(1f, 1f, 1f, 1f) };
	public Color[] precColor = new Color[6] { new Color(0.4f, 1f, 1f, 1f), new Color(1f, 1f, 0.4f, 1f), 
		new Color(0.4f, 1f, 0.4f, 1f), new Color(0.8f, 0.3f, 1f, 1f),
		new Color(1f, 0.6f, 0.3f, 1f), new Color(1f, 0.4f, 0.4f, 1f) };
	
	
	//MAIN MENU
	public bool alreadyPressStart;
	
	//SYSTEM
	public int baseRegenComboAfterMiss = 5;
	public bool BPMEntryMode;
	public int BPMChoiceMode;
	
	//GENERAL OPTION
	
	//General
	public float globalOffsetSeconds = -0.100f;

	
	//OPTION WHEEL
	public Sort sortMethod;
	public string packSelected = "";
	public Song songSelected;
	public Difficulty difficultySelected;
	public float speedmodSelected;
	public float rateSelected;
	public int skinSelected;
	public Judge scoreJudgeSelected;
	public Judge hitJudgeSelected;
	public Judge lifeJudgeSelected;
	public int raceSelected;
	public bool[] displaySelected;
	public int deathSelected;
	public double personalScore;
	public double recordScore;
	public string recordName;
	

	//SONG RESULT
	public float scoreEarned;
	public List<double> precAverage;
	public Dictionary<double, int> timeCombo;
	public Dictionary<double, double> lifeGraph;
	public Dictionary<string, int> scoreCount;
	public double firstEx;
	public double firstGreat;
	public double firstMisteak;
	public bool perfect;
	public bool fullFantCombo;
	public bool fullExCombo;
	public bool fullCombo;
	public bool fail;
	public bool clear;
	public double firstArrow;

	
	public static GameManager instance;


	void Awake()
	{
		if(instance == null){ 
			instance = this;
			instance.init();
		}
	}
	
	
	
	
	public void init(){
		ScoreWeightValues = new Dictionary<string, float>();
		ScoreWeightValues.Add("FANTASTIC",1f);
		ScoreWeightValues.Add("EXCELLENT",0.8f);
		ScoreWeightValues.Add("GREAT",0.4f);
		ScoreWeightValues.Add("DECENT",0f);
		ScoreWeightValues.Add("WAYOFF",-1.2f);
		ScoreWeightValues.Add("MISS",-2.4f);
		ScoreWeightValues.Add("FREEZE",1f);
		ScoreWeightValues.Add("UNFREEZE",0f);
		ScoreWeightValues.Add("MINE",-1.2f);
		
		
		LifeWeightValues = new Dictionary<string, float>();
		LifeWeightValues.Add("FANTASTIC",0.8f);
		LifeWeightValues.Add("EXCELLENT",0.7f);
		LifeWeightValues.Add("GREAT",0.4f);
		LifeWeightValues.Add("DECENT",0f);
		LifeWeightValues.Add("WAYOFF",-5f);
		LifeWeightValues.Add("MISS",-10f);
		LifeWeightValues.Add("FREEZE",0.8f);
		LifeWeightValues.Add("UNFREEZE",-8f);
		LifeWeightValues.Add("MINE",-5f);
		
		PrecisionValues = new Dictionary<Precision, double>();
		PrecisionValues.Add(Precision.FANTASTIC, 0.0215);
		PrecisionValues.Add(Precision.EXCELLENT, 0.043);
		PrecisionValues.Add(Precision.GREAT, 0.102);
		PrecisionValues.Add(Precision.DECENT, 0.135);
		PrecisionValues.Add(Precision.WAYOFF, 0.180);
		
		KeyCodeLeft = KeyCode.E;
		KeyCodeDown = KeyCode.T;
		KeyCodeUp = KeyCode.U;
		KeyCodeRight = KeyCode.O;
		SecondaryKeyCodeLeft = KeyCode.LeftArrow;
		SecondaryKeyCodeDown = KeyCode.DownArrow;
		SecondaryKeyCodeUp = KeyCode.UpArrow;
		SecondaryKeyCodeRight = KeyCode.RightArrow;
		
		BPMChoiceMode = 1;
		BPMEntryMode = false;
		
		alreadyPressStart = false;
		
		sortMethod = Sort.NAME;
		
		rateSelected = 0f;
	}
	
	public void LoadScoreJudge(Judge j){
		switch(j){
			case Judge.BEGINNER:
				ScoreWeightValues["EXCELLENT"] = 1f;
				ScoreWeightValues["GREAT"] = 0.9f;
				ScoreWeightValues["DECENT"] = 0.5f;
				ScoreWeightValues["WAYOFF"] = 0f;
				ScoreWeightValues["MISS"] = 0f;
				ScoreWeightValues["MINE"] = -0.01f;
			break;
			case Judge.EASY:
				ScoreWeightValues["EXCELLENT"] = 0.9f;
				ScoreWeightValues["GREAT"] = 0.6f;
				ScoreWeightValues["DECENT"] = 0f;
				ScoreWeightValues["WAYOFF"] = 0f;
				ScoreWeightValues["MISS"] = -1f;
				ScoreWeightValues["MINE"] = -0.5f;
			break;
			case Judge.NORMAL:
				ScoreWeightValues["EXCELLENT"] = 0.8f;
				ScoreWeightValues["GREAT"] = 0.4f;
				ScoreWeightValues["DECENT"] = 0f;
				ScoreWeightValues["WAYOFF"] = -1.2f;
				ScoreWeightValues["MISS"] = -2.4f;
				ScoreWeightValues["MINE"] = -1.2f;
			break;
			case Judge.HARD:
				ScoreWeightValues["EXCELLENT"] = 0.5f;
				ScoreWeightValues["GREAT"] = 0.2f;
				ScoreWeightValues["DECENT"] = -0.5f;
				ScoreWeightValues["WAYOFF"] = -2f;
				ScoreWeightValues["MISS"] = -4f;
				ScoreWeightValues["MINE"] = -2f;
			break;
			case Judge.EXPERT:
				ScoreWeightValues["EXCELLENT"] = 0.3f;
				ScoreWeightValues["GREAT"] = 0f;
				ScoreWeightValues["DECENT"] = -2f;
				ScoreWeightValues["WAYOFF"] = -5f;
				ScoreWeightValues["MISS"] = -10f;
				ScoreWeightValues["MINE"] = -10f;
			break;
		}
	}
	
	
	public void LoadHitJudge(Judge j){
		switch(j){
			case Judge.BEGINNER:
				PrecisionValues[Precision.FANTASTIC] = 0.135;
				PrecisionValues[Precision.EXCELLENT] = 0.180;
				PrecisionValues[Precision.GREAT] = 0.280;
				PrecisionValues[Precision.DECENT] = 0.350;
				PrecisionValues[Precision.WAYOFF] = 0.5;
			break;
			case Judge.EASY:
				PrecisionValues[Precision.FANTASTIC] = 0.043;
				PrecisionValues[Precision.EXCELLENT] = 0.102;
				PrecisionValues[Precision.GREAT] = 0.180;
				PrecisionValues[Precision.DECENT] = 0.240;
				PrecisionValues[Precision.WAYOFF] = 0.3;
			break;
			case Judge.NORMAL:	
				PrecisionValues[Precision.FANTASTIC] = 0.0215;
				PrecisionValues[Precision.EXCELLENT] = 0.043;
				PrecisionValues[Precision.GREAT] = 0.102;
				PrecisionValues[Precision.DECENT] = 0.135;
				PrecisionValues[Precision.WAYOFF] = 0.180;
			break;
			case Judge.HARD:	
				PrecisionValues[Precision.FANTASTIC] = 0.015;
				PrecisionValues[Precision.EXCELLENT] = 0.035;
				PrecisionValues[Precision.GREAT] = 0.070;
				PrecisionValues[Precision.DECENT] = 0.1;
				PrecisionValues[Precision.WAYOFF] = 0.135;
			break;
			case Judge.EXPERT:	
				PrecisionValues[Precision.FANTASTIC] = 0.0075;
				PrecisionValues[Precision.EXCELLENT] = 0.015;
				PrecisionValues[Precision.GREAT] = 0.0215;
				PrecisionValues[Precision.DECENT] = 0.043;
				PrecisionValues[Precision.WAYOFF] = 0.102;
			break;
		}
		
	}
	
	
	public void LoadLifeJudge(Judge j){
		switch(j){
			case Judge.BEGINNER:
				LifeWeightValues["FANTASTIC"] = 5f;
				LifeWeightValues["EXCELLENT"] = 2f;
				LifeWeightValues["GREAT"] = 1f;
				LifeWeightValues["DECENT"] = 0.2f;
				LifeWeightValues["WAYOFF"] = -0.2f;
				LifeWeightValues["MISS"] = -1f;
				LifeWeightValues["MINE"] = -0.1f;
				LifeWeightValues["FREEZE"] = 5f;
				LifeWeightValues["UNFREEZE"] = -0.2f;
				baseRegenComboAfterMiss = 0;
			break;
			case Judge.EASY:
				LifeWeightValues["FANTASTIC"] = 1.5f;
				LifeWeightValues["EXCELLENT"] = 1f;
				LifeWeightValues["GREAT"] = 0.5f;
				LifeWeightValues["DECENT"] = 0f;
				LifeWeightValues["WAYOFF"] = -2.5f;
				LifeWeightValues["MISS"] = -5f;
				LifeWeightValues["MINE"] = -2.5f;
				LifeWeightValues["FREEZE"] = 1.5f;
				LifeWeightValues["UNFREEZE"] = -3.5f;
				baseRegenComboAfterMiss = 2;
			break;
			case Judge.NORMAL:	
				LifeWeightValues["FANTASTIC"] = 0.8f;
				LifeWeightValues["EXCELLENT"] = 0.7f;
				LifeWeightValues["GREAT"] = 0.4f;
				LifeWeightValues["DECENT"] = 0f;
				LifeWeightValues["WAYOFF"] = -5f;
				LifeWeightValues["MISS"] = -10f;
				LifeWeightValues["MINE"] = -5f;
				LifeWeightValues["FREEZE"] = 0.8f;
				LifeWeightValues["UNFREEZE"] = -8f;
				baseRegenComboAfterMiss = 5;
			break;
			case Judge.HARD:	
				LifeWeightValues["FANTASTIC"] = 0.5f;
				LifeWeightValues["EXCELLENT"] = 0.4f;
				LifeWeightValues["GREAT"] = 0.05f;
				LifeWeightValues["DECENT"] = -5f;
				LifeWeightValues["WAYOFF"] = -20f;
				LifeWeightValues["MISS"] = -25f;
				LifeWeightValues["MINE"] = -10f;
				LifeWeightValues["FREEZE"] = 0.5f;
				LifeWeightValues["UNFREEZE"] = -20f;
				baseRegenComboAfterMiss = 10;
			break;
			case Judge.EXPERT:	
				LifeWeightValues["FANTASTIC"] = 0.2f;
				LifeWeightValues["EXCELLENT"] = 0.1f;
				LifeWeightValues["GREAT"] = 0f;
				LifeWeightValues["DECENT"] = -20f;
				LifeWeightValues["WAYOFF"] = -50f;
				LifeWeightValues["MISS"] = -100f;
				LifeWeightValues["MINE"] = -25f;
				LifeWeightValues["FREEZE"] = 0f;
				LifeWeightValues["UNFREEZE"] = -25f;
				baseRegenComboAfterMiss = 20;
			break;
		}
		
		
	}
	
	public float giveTargetScoreOfRace(int index){
		switch(index){
			case 0:
				return 0f;
			case 1:
				return 55f;
			case 2:
				return 68f;
			case 3:
				return 80f;
			case 4:
				return 89f;
			case 5:
				return 96f;
			case 6:
				return 98f;
			case 7:
				return 99f;
			case 8:
				return 100f;
			default:
				return 0f;
		}
	}
	
	public string giveNoteOfScore(float score){
		if(score >= 100f){
			return "=;QUAD";	
		}
		if(score >= 99f){
			return "=;GOLD";	
		}
		if(score >= 98f){
			return "=;SILVER";
		}
		if(score >= 96f){
			return "=;BRONZE";
		}

		//S
		if(score >= 95f){
			return "+;S";
		}
		if(score >= 90f){
			return "=;S";	
		}
		if(score >= 89f){
			return "-;S";	
		}
		
		//A
		if(score >= 88f){
			return "+;A";
		}
		if(score >= 81f){
			return "=;A";	
		}
		if(score >= 80f){
			return "-;A";	
		}
		
		//B
		if(score >= 78f){
			return "+;B";	
		}
		if(score >= 70f){
			return "=;B";	
		}
		if(score >= 68f){
			return "-;B";	
		}
		
		//C
		if(score >= 65f){
			return "+;C";	
		}
		if(score >= 58f){
			return "=;C";	
		}
		if(score >= 55f){
			return "-;C";	
		}
		
		return "=;BAD";
	}
}
