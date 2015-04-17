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
	public Text bestPersonalText;
	public Text bestInternetText;
	public Text bestInternetName;

	void Start()
	{
		if (SongOptionManager.instance.currentBestPersonal < 0) {
			bestPersonalText.text = "--";
		} else {
			bestPersonalText.text = SongOptionManager.instance.currentBestPersonal.ToString("00.00") + "%";
		}

		if (SongOptionManager.instance.currentBestInternet < 0) {
			bestInternetText.text = "--";
		} else {
			bestInternetText.text = SongOptionManager.instance.currentBestInternet.ToString("00.00") + "%";
		}

		scoreText.text = "00.00%";

		score = 0;
	}

	public void addScore(Precision prec)
	{
		addScore(GameManager.instance.ScoreWeightValues[(int)prec]);
	}

	public void addScore(double score)
	{
		//Faire le score !
	}
}
