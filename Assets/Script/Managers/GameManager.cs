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
	public double timeBeforeFreezeMiss = 0.350;
	
	//DATA GAME
	[HideInInspector] public double[] ScoreWeightValues;
	[HideInInspector] public double[] LifeWeightValues;
	[HideInInspector] public double[] PrecisionValues;
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
	public string DEBUGPATH = "/";

	
	public Profile prefs;
	public UnityEngine.Audio.AudioMixer masterMixer;
	public Texture2D emptyPackTexture;

	//MAIN MENU
	[HideInInspector] public bool gameInitialized;

	
	public static GameManager instance;

	void Awake()
	{
		if(instance == null){ 
			instance = this;

			#if UNITY_EDITOR || UNITY_EDITOR_64
			DEBUGPATH = "/../";
			#else
			DEBUGPATH = "/";
			#endif


			instance.init();

		}
		DontDestroyOnLoad (this);
	}
	
	public void init(){
		ScoreWeightValues = new double[System.Enum.GetValues(typeof(Precision)).Length];
		LifeWeightValues = new double[System.Enum.GetValues(typeof(Precision)).Length];
		PrecisionValues = new double[System.Enum.GetValues(typeof(Precision)).Length];

		for (int i=0; i<Enum.GetValues(typeof(Precision)).Length - 1; i++) {
			ScoreWeightValues[i] = findPrecValue(scoreWeightBase, (Precision)i).value;
			LifeWeightValues[i] = findPrecValue(lifeWeightBase, (Precision)i).value;
			if(i < (int)Precision.MISS) PrecisionValues[i] = findPrecValue(precisionBase, (Precision)i).value;

		}

		ProfileManager.instance.LoadProfiles ();
		prefs = ProfileManager.instance.prefs;
		setPrefsValues();
	}
	
	public void LoadScoreJudge(Judge j){

		if (j == Judge.NORMAL) {
			for (int i=0; i<Enum.GetValues(typeof(Precision)).Length - 1; i++) {
				ScoreWeightValues[i] = findPrecValue(scoreWeightBase, (Precision)i).value;
			}
		} else {
			JudgeValue judgeVal = scoreJudgeBase.Find (c => c.judge == j);

			for (int i=0; i<Enum.GetValues(typeof(Precision)).Length - 1; i++) {
				PrecisionValue precValue =  findPrecValue(judgeVal.newValues, (Precision)i);
				ScoreWeightValues[i] = precValue == null ? ScoreWeightValues[i] : precValue.value;
			}
		}

	}
	
	
	public void LoadHitJudge(Judge j){
		if (j == Judge.NORMAL) {
			for (int i=0; i<(int)Precision.MISS; i++) {
				PrecisionValues[i] = findPrecValue(precisionBase, (Precision)i).value;
			}
		} else {
			JudgeValue judgeVal = precisionJudgeBase.Find (c => c.judge == j);
			
			for (int i=0; i<(int)Precision.MISS; i++) {
				PrecisionValue precValue =  findPrecValue(judgeVal.newValues, (Precision)i);
				PrecisionValues[i] = precValue == null ?  PrecisionValues[i] : precValue.value;
			}
		}
	}
	
	
	public void LoadLifeJudge(Judge j){
		if (j == Judge.NORMAL) {
			for (int i=0; i<Enum.GetValues(typeof(Precision)).Length - 1; i++) {
				LifeWeightValues[i] = findPrecValue(lifeWeightBase, (Precision)i).value;
			}
			startRegenAfterMiss = baseRegenComboAfterMiss;
		} else {
			JudgeValue judgeVal = lifeJudgeBase.Find (c => c.judge == j);
			
			for (int i=0; i<Enum.GetValues(typeof(Precision)).Length - 1; i++) {
				PrecisionValue precValue =  findPrecValue(judgeVal.newValues, (Precision)i);
				LifeWeightValues[i] = precValue == null ?  LifeWeightValues[i] : precValue.value;
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

	public void setPrefsValues()
	{
		QualitySettings.vSyncCount = prefs.enableVSync ? 1 : 0;
		Application.targetFrameRate = prefs.enableVSync ? 60 : -1;
		QualitySettings.antiAliasing = prefs.antiAliasing;
		setMasterVolume(prefs.generalVolume);
	}

	public void setMasterVolume(float volume)
	{
		if(volume > 0.75f)
		{
			masterMixer.SetFloat("masterVol", Mathf.Lerp(-5f, 0f, (volume - 0.75f)/0.25f));
		}else if(volume > 0.5f)
		{
			masterMixer.SetFloat("masterVol", Mathf.Lerp(-10f, -5f, (volume - 0.5f)/0.25f));
		}else if(volume > 0.25f)
		{
			masterMixer.SetFloat("masterVol", Mathf.Lerp(-25f, -10f, (volume - 0.25f)/0.25f));
		}else{
			masterMixer.SetFloat("masterVol", Mathf.Lerp(-80f, -25f, volume/0.25f));
		}

	}

	/*void OnApplicationQuit()
	{
		if (LoadManager.instance != null && LoadManager.instance.songPacks != null) {
			foreach(SongPack sp in LoadManager.instance.songPacks)
			{
				if(sp.songsData != null)
				{
					foreach(SongData sd in SongData)
					{
						sd.songs.First().Value.cleanWav();
					}
				}
			}
		}
	}*/
}
