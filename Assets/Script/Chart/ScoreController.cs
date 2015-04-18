using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ScoreController : MonoBehaviour {

	public static ScoreController instance;
	
	void Awake()
	{
		if (instance == null)
			instance = this;
	}

	double score;
	public Text scoreText;
	public Image bestPersonalBG;
	public Text bestPersonalText;
	public Text bestPersonalName;
	public Image bestInternetBG;
	public Text bestInternetText;
	public Text bestInternetName;

	public Color bestReachedBG;
	public Color bestReachedText;

	double totalNoteToValid;

	void Start()
	{
		if (SongOptionManager.instance.currentBestPersonal <= 0) {
			bestPersonalText.text = "--";
		} else {
			bestPersonalText.text = SongOptionManager.instance.currentBestPersonal.ToString("00.00") + "%";
		}

		if (SongOptionManager.instance.currentBestInternet <= 0) {
			bestInternetText.text = "--";
			bestInternetName.text = "WR : --";
		} else {
			bestInternetText.text = SongOptionManager.instance.currentBestInternet.ToString("00.00") + "%";
			bestInternetName.text = "WR : " + SongOptionManager.instance.currentBestInternetName;
		}

		scoreText.text = "00.00%";

		totalNoteToValid = SongOptionManager.instance.currentSongPlayed.numberOfStepsAbsolute + 
			SongOptionManager.instance.currentSongPlayed.numberOfFreezes + 
			SongOptionManager.instance.currentSongPlayed.numberOfRolls;

		score = 0;
	}

	public void addScoreByPrecision(Precision prec)
	{
		if(ChartManager.instance.isGameOver()) return;
		addScore((GameManager.instance.ScoreWeightValues[(int)prec]*100) / totalNoteToValid);
	}

	double oldScore;
	public void addScore(double scoreAdd)
	{
		oldScore = score;
		score = (double)Mathf.Clamp((float)(score + scoreAdd), 0f, 100f);
		
		if(SongOptionManager.instance.currentBestInternet > 0 && score >= SongOptionManager.instance.currentBestInternet 
		   && oldScore < SongOptionManager.instance.currentBestInternet)
		{
			bestInternetBG.color = bestReachedBG;
			bestInternetText.color = bestReachedText;
			bestInternetName.color = bestReachedText;
		}
		if(SongOptionManager.instance.currentBestPersonal > 0 && score >= SongOptionManager.instance.currentBestPersonal 
		   && oldScore < SongOptionManager.instance.currentBestPersonal)
		{
			bestPersonalBG.color = bestReachedBG;
			bestPersonalText.color = bestReachedText;
			bestPersonalName.color = bestReachedText;
		}

		scoreText.text = score.ToString("00.00") + "%";

	}
}
