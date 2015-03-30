using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;
using System.IO;
using System.Diagnostics;

[System.Serializable]
public class PrecisionValue
{
	public Precision precision;
	public double value;
}

[System.Serializable]
public class NoteValue
{
	public ScoreNote note;
	public double score;
}

[System.Serializable]
public class JudgeValue
{
	public Judge judge;
	public List<PrecisionValue> newValues;
	public int regenAfterMiss;
}

public class GameManager : MonoBehaviour{

	//EDITOR
	public List<PrecisionValue> scoreWeightBase;
	public List<PrecisionValue> lifeWeightBase;
	public List<PrecisionValue> precisionBase;

	public List<JudgeValue> scoreJudgeBase;
	public List<JudgeValue> lifeJudgeBase;
	public List<JudgeValue> precisionJudgeBase;

	public List<NoteValue> noteBase;

	public int baseRegenComboAfterMiss = 5;
	public double globalOffsetSeconds = -0.1;
	public double timeBeforeFreezeMiss = 0.350;
	
	//DATA GAME
	[HideInInspector] public Dictionary<Precision, double> ScoreWeightValues;
	[HideInInspector] public Dictionary<Precision, double> LifeWeightValues;
	[HideInInspector] public Dictionary<Precision, double> PrecisionValues;
	[HideInInspector] public int startRegenAfterMiss;


	//Defaults values
	public Color[] diffColor = new Color[6] { new Color(0.68f, 0.40f, 1f, 1f), new Color(0.396f, 1f, 0.415f, 1f) ,
		new Color(0.965f, 1f, 0.47f, 1f), new Color(1f, 0.208f, 0.208f, 1f), 
		new Color(0.208f, 0.57f, 1f, 1f), new Color(1f, 1f, 1f, 1f) };
	public Color[] precColor = new Color[6] { new Color(0.4f, 1f, 1f, 1f), new Color(1f, 1f, 0.4f, 1f), 
		new Color(0.4f, 1f, 0.4f, 1f), new Color(0.8f, 0.3f, 1f, 1f),
		new Color(1f, 0.6f, 0.3f, 1f), new Color(1f, 0.4f, 0.4f, 1f) };

	//Debug
	[HideInInspector]
#if UNITY_EDITOR || UNITY_EDITOR_64
	public string DEBUGPATH = "/../";
#else
	public string DEBUGPATH = "/";
#endif

	public Profile prefs;

	//MAIN MENU
	[HideInInspector] public bool alreadyPressStart;

	
	public static GameManager instance;

	void Awake()
	{
		if(instance == null){ 
			instance = this;
			instance.init();
		}
		DontDestroyOnLoad (this);
	}
	
	public void init(){
		ScoreWeightValues = new Dictionary<Precision, double>();
		LifeWeightValues = new Dictionary<Precision, double>();
		PrecisionValues = new Dictionary<Precision, double>();

		for (int i=0; i<Enum.GetValues(typeof(Precision)).Length - 1; i++) {
			ScoreWeightValues.Add((Precision)i, findPrecValue(scoreWeightBase, (Precision)i).value);
			LifeWeightValues.Add((Precision)i, findPrecValue(lifeWeightBase, (Precision)i).value);
			if(i < (int)Precision.MISS) PrecisionValues.Add((Precision)i, findPrecValue(precisionBase, (Precision)i).value);

		}

		ProfileManager.instance.LoadProfiles ();
		prefs = ProfileManager.instance.prefs;
		
		alreadyPressStart = false;
	}
	
	public void LoadScoreJudge(Judge j){

		if (j == Judge.NORMAL) {
			for (int i=0; i<Enum.GetValues(typeof(Precision)).Length - 1; i++) {
				ScoreWeightValues[(Precision)i] = findPrecValue(scoreWeightBase, (Precision)i).value;
			}
		} else {
			JudgeValue judgeVal = scoreJudgeBase.Find (c => c.judge == j);

			for (int i=0; i<Enum.GetValues(typeof(Precision)).Length - 1; i++) {
				PrecisionValue precValue =  findPrecValue(judgeVal.newValues, (Precision)i);
				ScoreWeightValues[(Precision)i] = precValue == null ? ScoreWeightValues[(Precision)i] : precValue.value;
			}
		}

	}
	
	
	public void LoadHitJudge(Judge j){
		if (j == Judge.NORMAL) {
			for (int i=0; i<(int)Precision.MISS; i++) {
				PrecisionValues[(Precision)i] = findPrecValue(precisionBase, (Precision)i).value;
			}
		} else {
			JudgeValue judgeVal = precisionJudgeBase.Find (c => c.judge == j);
			
			for (int i=0; i<(int)Precision.MISS; i++) {
				PrecisionValue precValue =  findPrecValue(judgeVal.newValues, (Precision)i);
				PrecisionValues[(Precision)i] = precValue == null ?  PrecisionValues[(Precision)i] : precValue.value;
			}
		}
	}
	
	
	public void LoadLifeJudge(Judge j){
		if (j == Judge.NORMAL) {
			for (int i=0; i<Enum.GetValues(typeof(Precision)).Length - 1; i++) {
				LifeWeightValues[(Precision)i] = findPrecValue(lifeWeightBase, (Precision)i).value;
			}
			startRegenAfterMiss = baseRegenComboAfterMiss;
		} else {
			JudgeValue judgeVal = lifeJudgeBase.Find (c => c.judge == j);
			
			for (int i=0; i<Enum.GetValues(typeof(Precision)).Length - 1; i++) {
				PrecisionValue precValue =  findPrecValue(judgeVal.newValues, (Precision)i);
				LifeWeightValues[(Precision)i] = precValue == null ?  LifeWeightValues[(Precision)i] : precValue.value;
			}
			startRegenAfterMiss = judgeVal.regenAfterMiss;
		}
	}

	public ScoreNote giveNoteOfScore(float score){
		List<NoteValue> tempList = noteBase.FindAll (c => c.score <= score);
		tempList.Sort(delegate(NoteValue x, NoteValue y) {
			return x.score.CompareTo(y.score);
		});
		return tempList.Last ().note;
	}

	public PrecisionValue findPrecValue(List<PrecisionValue> precList, Precision prec)
	{
		return precList.Find(c => c.precision == prec);
	}
}
