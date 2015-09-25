using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class SongOptionPanel : MonoBehaviour {

	//Presentation
	public Text songTitle;
	public Text bpm;
	public RawImage bannerImage;

	//Speedmod
	public InputField multipSpeedmod;
	public GameObject oneSpeedObj;
	public GameObject twoSpeedObj;
	public InputField bpmSpeedmod;
	public InputField lowerBPMSpeedmod;
	public InputField higherBPMSpeedmod;

	public InputField rate;

	public Text skinSelectedText;
	public Toggle bigNotesToggle;

	public Text objectiveText;
	public Text hitJudgeText;
	public Button leftHit;
	public Button rightHit;
	public Text scoreJudgeText;
	public Button leftScore;
	public Button rightScore;
	public Text lifeJudgeText;
	public Button leftLife;
	public Button rightLife;
	public Text deathConditionText;

	public List<Button> displayOptionButton;
	public Sprite disableButton;
	public Sprite enabledButton;

	public List<GameObject> difficultyButtons;

	public bool okBPM;
	public bool okRate;

	SongInfoProfil currentSIP = null;

	double firstBPM;
	double secondBPM;

	void Start()
	{
		songTitle.text = SongSelectionManager.instance.songSelected.title;

		bannerImage.texture = SongSelectionManager.instance.songSelected.GetBanner((Texture2D)bannerImage.texture) ?? GameManager.instance.emptyPackTexture;


		currentSIP = GameManager.instance.prefs.scoreOnSong.Find (c => c.CompareId (SongSelectionManager.instance.songSelected.sip));

		twoSpeedObj.SetActive (SongSelectionManager.instance.songSelected.bpmToDisplay.Contains ("$"));
		oneSpeedObj.SetActive (!twoSpeedObj.activeSelf);

		if (currentSIP != null) {
			multipSpeedmod.text = currentSIP.speedmodpref.ToString ("0.00");

			if (twoSpeedObj.activeSelf) {
				firstBPM = double.Parse (SongSelectionManager.instance.songSelected.bpmToDisplay.Split ('$') [0]);
				secondBPM = double.Parse (SongSelectionManager.instance.songSelected.bpmToDisplay.Split ('$') [1]);
				lowerBPMSpeedmod.text = ((int)(Mathf.Min ((float)firstBPM, (float)secondBPM) * currentSIP.speedmodpref)).ToString ();
				higherBPMSpeedmod.text = ((int)(Mathf.Max ((float)firstBPM, (float)secondBPM) * currentSIP.speedmodpref)).ToString ();
				bpm.text = ((int)firstBPM).ToString() + " - " + ((int)secondBPM).ToString();
			} else {
				firstBPM = double.Parse (SongSelectionManager.instance.songSelected.bpmToDisplay);
				bpmSpeedmod.text = ((int)firstBPM * currentSIP.speedmodpref).ToString ();
				bpm.text = ((int)firstBPM).ToString();
			}
		} else {

			if (oneSpeedObj.activeSelf && GameManager.instance.prefs.inBPMMode) {
				firstBPM = double.Parse (SongSelectionManager.instance.songSelected.bpmToDisplay);
				bpmSpeedmod.text = ((int)GameManager.instance.prefs.lastBPM).ToString ();

				multipSpeedmod.text = (GameManager.instance.prefs.lastBPM / firstBPM).ToString ("0.00");
				bpm.text = ((int)firstBPM).ToString();
			} else {

				multipSpeedmod.text = GameManager.instance.prefs.lastSpeedmod.ToString ("0.00");

				if (twoSpeedObj.activeSelf) {
					firstBPM = double.Parse (SongSelectionManager.instance.songSelected.bpmToDisplay.Split ('$') [0]);
					secondBPM = double.Parse (SongSelectionManager.instance.songSelected.bpmToDisplay.Split ('$') [1]);
					lowerBPMSpeedmod.text = ((int)(Mathf.Min ((float)firstBPM, (float)secondBPM) * GameManager.instance.prefs.lastSpeedmod)).ToString ();
					higherBPMSpeedmod.text = ((int)(Mathf.Max ((float)firstBPM, (float)secondBPM) * GameManager.instance.prefs.lastSpeedmod)).ToString ();
					bpm.text = ((int)firstBPM).ToString() + " - " + ((int)secondBPM).ToString();
				} else {
					firstBPM = double.Parse (SongSelectionManager.instance.songSelected.bpmToDisplay);
					bpmSpeedmod.text = ((int)firstBPM * GameManager.instance.prefs.lastSpeedmod).ToString ();
					bpm.text = ((int)firstBPM).ToString();
				}
			}
		}

		bpm.text += "\nBPM";

		rate.text = ((int)(SongOptionManager.instance.rateSelected * 100)).ToString ();

		skinSelectedText.text = GameLocalization.instance.Translate ("SkinType").Split ('$') [SongOptionManager.instance.skinSelected];

		bigNotesToggle.isOn = SongOptionManager.instance.bigNotes;

		objectiveText.text = GameLocalization.instance.Translate ("RaceType").Split ('$') [SongOptionManager.instance.raceSelected];
		SongOptionManager.instance.convertRaceToMinimumScore ();

		hitJudgeText.text = GameLocalization.instance.Translate ("HitJudgeDiff").Split ('$') [SongOptionManager.instance.hitJudgeSelected];

		scoreJudgeText.text = GameLocalization.instance.Translate ("ScoreJudgeDiff").Split ('$') [SongOptionManager.instance.scoreJudgeSelected];

		lifeJudgeText.text = GameLocalization.instance.Translate ("LifeJudgeDiff").Split ('$') [SongOptionManager.instance.lifeJudgeSelected];

		deathConditionText.text = GameLocalization.instance.Translate ("DeathCondition").Split ('$') [SongOptionManager.instance.deathConditionSelected];

		foreach (Button b in displayOptionButton) {
			b.GetComponent<Image>().sprite = disableButton;
		}

		foreach(OptionsMod mod in SongOptionManager.instance.currentOptions)
		{
			Button b = displayOptionButton.Find(c => c.name == mod.ToString());
			if(b != null)
			{
				b.GetComponent<Image>().sprite = enabledButton;
			}
		}
		
		foreach (GameObject buttons in difficultyButtons) {
			if(!SongSelectionManager.instance.songDataSelected.songs.ContainsKey(
				(Difficulty)System.Enum.Parse(typeof(Difficulty), buttons.name)))
			{
				buttons.SetActive(false);
			}else{
				Song s = SongSelectionManager.instance.songDataSelected.songs[(
					(Difficulty)System.Enum.Parse(typeof(Difficulty), buttons.name))];
				buttons.transform.FindChild("Level").GetComponent<Text>().text = s.level.ToString();
			}
		}

		applyChangeDifficulty (SongSelectionManager.instance.songSelected.difficulty);
	}

	public void changingBPMPerMultip(string value)
	{
		GameManager.instance.prefs.inBPMMode = false;
		if (string.IsNullOrEmpty (value)) {
			okBPM = false;
			return;
		} else {
			float bpm = 0f;
			if(!float.TryParse(value, out bpm))
			{
				okBPM = false;
				return;
			}
			if((bpm < 0.01f || bpm > 99f))
			{
				okBPM = false;
				return;
			}
			GameManager.instance.prefs.lastSpeedmod = bpm;
			if (twoSpeedObj.activeSelf) {
				lowerBPMSpeedmod.text = ((int)(Mathf.Min ((float)firstBPM, (float)secondBPM) * GameManager.instance.prefs.lastSpeedmod)).ToString ();
				higherBPMSpeedmod.text = ((int)(Mathf.Max ((float)firstBPM, (float)secondBPM) * GameManager.instance.prefs.lastSpeedmod)).ToString ();
			} else {
				bpmSpeedmod.text = ((int)firstBPM * GameManager.instance.prefs.lastSpeedmod).ToString ();
			}
			SongOptionManager.instance.speedmodSelected = GameManager.instance.prefs.lastSpeedmod;
		}
	}

	public void changingBPMPerValue(string value)
	{
		GameManager.instance.prefs.inBPMMode = true;
		if (string.IsNullOrEmpty (value)) {
			okBPM = false;
			return;
		} else {
			float bpm = 0f;
			if(!float.TryParse(value, out bpm))
			{
				okBPM = false;
				return;
			}
			if((bpm < 1 || bpm > 9999))
			{
				okBPM = false;
				return;
			}
			GameManager.instance.prefs.lastBPM = bpm;
			multipSpeedmod.text = (GameManager.instance.prefs.lastBPM / firstBPM).ToString ("0.00");
			SongOptionManager.instance.speedmodSelected = bpm / firstBPM;
		}
	}

	public void changingBPMPerHighValue(string value)
	{
		GameManager.instance.prefs.inBPMMode = false;
		if (string.IsNullOrEmpty (value)) {
			okBPM = false;
			return;
		} else {
			float bpm = 0f;
			if(!float.TryParse(value, out bpm))
			{
				okBPM = false;
				return;
			}
			if((bpm < 1 || bpm > 9999))
			{
				okBPM = false;
				return;
			}
			GameManager.instance.prefs.lastSpeedmod = bpm / Mathf.Min ((float)firstBPM, (float)secondBPM);
			multipSpeedmod.text = (bpm / Mathf.Min ((float)firstBPM, (float)secondBPM)).ToString ("0.00");
			higherBPMSpeedmod.text = ((int)(Mathf.Max ((float)firstBPM, (float)secondBPM) * GameManager.instance.prefs.lastSpeedmod)).ToString ();
			SongOptionManager.instance.speedmodSelected = GameManager.instance.prefs.lastSpeedmod;
		}
	}

	public void changingBPMPerLowValue(string value)
	{
		GameManager.instance.prefs.inBPMMode = false;
		if (string.IsNullOrEmpty (value)) {
			okBPM = false;
			return;
		} else {
			float bpm = 0f;
			if(!float.TryParse(value, out bpm))
			{
				okBPM = false;
				return;
			}
			if((bpm < 1 || bpm > 9999))
			{
				okBPM = false;
				return;
			}
			GameManager.instance.prefs.lastSpeedmod = bpm / Mathf.Max ((float)firstBPM, (float)secondBPM);
			multipSpeedmod.text = (bpm / Mathf.Max ((float)firstBPM, (float)secondBPM)).ToString ("0.00");
			lowerBPMSpeedmod.text = ((int)(Mathf.Min ((float)firstBPM, (float)secondBPM) * GameManager.instance.prefs.lastSpeedmod)).ToString ();
			SongOptionManager.instance.speedmodSelected = GameManager.instance.prefs.lastSpeedmod;
		}
	}

	public void changingRate(string newRate)
	{
		if (string.IsNullOrEmpty (newRate)) {
			okRate = false;
			return;
		} else {
			float theNewRate = 0f;
			if(!float.TryParse(newRate, out theNewRate))
			{
				okRate = false;
				return;
			}
			if(theNewRate < 1f || theNewRate > 200f)
			{
				okRate = false;
				return;
			}
			SongOptionManager.instance.rateSelected = theNewRate;
		}
	}

	public void changingSkin(bool next)
	{
		if (next) {
			SongOptionManager.instance.skinSelected++;
			if(SongOptionManager.instance.skinSelected > 3)
			{
				SongOptionManager.instance.skinSelected = 0;
			}
		} else {
			SongOptionManager.instance.skinSelected--;
			if(SongOptionManager.instance.skinSelected < 0)
			{
				SongOptionManager.instance.skinSelected = 3;
			}
		}
		objectiveText.text = GameLocalization.instance.Translate ("SkinType").Split ('$') [SongOptionManager.instance.skinSelected];
	}

	public void changingBigNotes(bool active)
	{
		SongOptionManager.instance.bigNotes = active;
	}

	public void changingRace(bool next)
	{
		if (next) {
			SongOptionManager.instance.raceSelected++;
			if(SongOptionManager.instance.raceSelected > System.Enum.GetValues(typeof(ScoreNote)).Length - 1)
			{
				SongOptionManager.instance.raceSelected = 0;
			}
		} else {
			SongOptionManager.instance.raceSelected--;
			if(SongOptionManager.instance.raceSelected < 0)
			{
				SongOptionManager.instance.raceSelected = System.Enum.GetValues(typeof(ScoreNote)).Length - 1;
			}
		}
		objectiveText.text = GameLocalization.instance.Translate ("RaceType").Split ('$') [SongOptionManager.instance.raceSelected];
		SongOptionManager.instance.convertRaceToMinimumScore();
	}

	public void changingLifejudge(bool next)
	{
		leftLife.gameObject.SetActive (true);
		rightLife.gameObject.SetActive (true);
		if (next) {
			SongOptionManager.instance.lifeJudgeSelected++;
			if(SongOptionManager.instance.lifeJudgeSelected >= System.Enum.GetValues(typeof(Judge)).Length - 1)
			{
				rightLife.gameObject.SetActive(true);
			}
		} else {
			SongOptionManager.instance.lifeJudgeSelected--;
			if(SongOptionManager.instance.lifeJudgeSelected <= 0)
			{
				leftLife.gameObject.SetActive(true);
			}
		}
		lifeJudgeText.text = GameLocalization.instance.Translate ("LifeJudgeDiff").Split ('$') [SongOptionManager.instance.lifeJudgeSelected];

	}

	public void changingScorejudge(bool next)
	{
		leftScore.gameObject.SetActive (true);
		rightScore.gameObject.SetActive (true);
		if (next) {
			SongOptionManager.instance.scoreJudgeSelected++;
			if(SongOptionManager.instance.scoreJudgeSelected >= System.Enum.GetValues(typeof(Judge)).Length - 1)
			{
				rightScore.gameObject.SetActive(true);
			}
		} else {
			SongOptionManager.instance.scoreJudgeSelected--;
			if(SongOptionManager.instance.scoreJudgeSelected <= 0)
			{
				leftScore.gameObject.SetActive(true);
			}
		}
		scoreJudgeText.text = GameLocalization.instance.Translate ("ScoreJudgeDiff").Split ('$') [SongOptionManager.instance.scoreJudgeSelected];

	}

	public void changingHitJudge(bool next)
	{
		leftHit.gameObject.SetActive (true);
		rightHit.gameObject.SetActive (true);
		if (next) {
			SongOptionManager.instance.hitJudgeSelected++;
			if(SongOptionManager.instance.hitJudgeSelected >= System.Enum.GetValues(typeof(Judge)).Length - 1)
			{
				rightHit.gameObject.SetActive(true);
			}
		} else {
			SongOptionManager.instance.hitJudgeSelected--;
			if(SongOptionManager.instance.hitJudgeSelected <= 0)
			{
				leftHit.gameObject.SetActive(true);
			}
		}
		hitJudgeText.text = GameLocalization.instance.Translate ("HitJudgeDiff").Split ('$') [SongOptionManager.instance.hitJudgeSelected];

	}

	public void changingDeathCondition(bool next)
	{
		if (next) {
			SongOptionManager.instance.deathConditionSelected++;
			if(SongOptionManager.instance.deathConditionSelected > System.Enum.GetValues(typeof(DeathMode)).Length - 1)
			{
				SongOptionManager.instance.deathConditionSelected = 0;
			}
		} else {
			SongOptionManager.instance.deathConditionSelected--;
			if(SongOptionManager.instance.deathConditionSelected < 0)
			{
				SongOptionManager.instance.deathConditionSelected = System.Enum.GetValues(typeof(DeathMode)).Length - 1;
			}
		}
		objectiveText.text = GameLocalization.instance.Translate ("DeathCondition").Split ('$') [SongOptionManager.instance.deathConditionSelected];
	}

	public void pushOption(GameObject go)
	{
		OptionsMod mod = (OptionsMod) System.Enum.Parse (typeof(OptionsMod), go.name);

		if (SongOptionManager.instance.currentOptions.Contains (mod)) {
			SongOptionManager.instance.currentOptions.Remove(mod);
			go.GetComponent<Image>().sprite = disableButton;
		} else {
			SongOptionManager.instance.currentOptions.Add(mod);
			go.GetComponent<Image>().sprite = enabledButton;
		}
	}

	public void changeDifficulty(string difficulty)
	{
		SongSelectionManager.instance.difficultyChanged(difficulty);

		applyChangeDifficulty((Difficulty)System.Enum.Parse(typeof(Difficulty), difficulty));
	}

	public void applyChangeDifficulty(Difficulty diff)
	{
		foreach(GameObject difGO in difficultyButtons)
		{
			Difficulty d = (Difficulty)System.Enum.Parse(typeof(Difficulty), difGO.name);
			
			if(d == diff)
			{
				difGO.GetComponent<Button>().interactable = false;
				difGO.transform.FindChild("Level").GetComponent<CanvasGroup>().alpha = 1f;
			}else{
				difGO.GetComponent<Button>().interactable = true;
				difGO.transform.FindChild("Level").GetComponent<CanvasGroup>().alpha = 0.4f;
			}
		}
	}

	public void playChart()
	{
		if (okBPM && okRate) {
			//Anim !
		}
	}
}
