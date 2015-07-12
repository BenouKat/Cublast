using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class SongOptionPanel : MonoBehaviour {

	//Speedmod
	public InputField multipSpeedmod;
	public GameObject oneSpeedObj;
	public GameObject twoSpeedObj;
	public InputField bpmSpeedmod;
	public InputField lowerBPMSpeedmod;
	public InputField higherBPMSpeedmod;

	public InputField rate;

	public Text skinSelectedText;
	private int skinSelectedIndex;
	public Toggle bigNotesToggle;

	public Text objectiveText;
	public int objectiveIndex;
	public Text hitJudgeText;
	public int hitJudgeIndex;
	public Text scoreJudgeText;
	public int scoreJudgeIndex;
	public Text lifeJudgeText;
	public int lifeJudgeIndex;
	public Text deathConditionText;
	public int deathConditionIndex;

	public List<Button> displayOptionButton;

	SongInfoProfil currentSIP = null;

	double firstBPM;
	double secondBPM;

	void Start()
	{
		currentSIP = GameManager.instance.prefs.scoreOnSong.Find(c => c.CompareId(SongSelectionManager.instance.songSelected.sip));

		twoSpeedObj.SetActive(SongSelectionManager.instance.songSelected.bpmToDisplay.Contains("$"));
		oneSpeedObj.SetActive(!twoSpeedObj.activeSelf);

		if(currentSIP != null)
		{
			multipSpeedmod.text = currentSIP.speedmodpref.ToString("0.00");

			if(twoSpeedObj.activeSelf)
			{
				firstBPM = double.Parse(SongSelectionManager.instance.songSelected.bpmToDisplay.Split('$')[0]);
				secondBPM = double.Parse(SongSelectionManager.instance.songSelected.bpmToDisplay.Split('$')[1]);
				lowerBPMSpeedmod.text = ((int)(Mathf.Min((float)firstBPM, (float)secondBPM)*currentSIP.speedmodpref)).ToString();
				higherBPMSpeedmod.text = ((int)(Mathf.Max((float)firstBPM, (float)secondBPM)*currentSIP.speedmodpref)).ToString();
			}else{
				firstBPM = double.Parse(SongSelectionManager.instance.songSelected.bpmToDisplay);
				bpmSpeedmod.text = ((int)firstBPM * currentSIP.speedmodpref).ToString();
			}
		}else{

			if(oneSpeedObj.activeSelf && GameManager.instance.prefs.inBPMMode)
			{
				firstBPM = double.Parse(SongSelectionManager.instance.songSelected.bpmToDisplay);
				bpmSpeedmod.text = ((int)GameManager.instance.prefs.lastBPM).ToString();

				multipSpeedmod.text = (GameManager.instance.prefs.lastBPM / firstBPM).ToString("0.00");
			}else{

				multipSpeedmod.text = GameManager.instance.prefs.lastSpeedmod.ToString("0.00");

				if(twoSpeedObj.activeSelf)
				{
					firstBPM = double.Parse(SongSelectionManager.instance.songSelected.bpmToDisplay.Split('$')[0]);
					secondBPM = double.Parse(SongSelectionManager.instance.songSelected.bpmToDisplay.Split('$')[1]);
					lowerBPMSpeedmod.text = ((int)(Mathf.Min((float)firstBPM, (float)secondBPM)*GameManager.instance.prefs.lastSpeedmod)).ToString();
					higherBPMSpeedmod.text = ((int)(Mathf.Max((float)firstBPM, (float)secondBPM)*GameManager.instance.prefs.lastSpeedmod)).ToString();
				}else{
					firstBPM = double.Parse(SongSelectionManager.instance.songSelected.bpmToDisplay);
					bpmSpeedmod.text = ((int)firstBPM * GameManager.instance.prefs.lastSpeedmod).ToString();
				}
			}
		}

		rate.text = ((int)(SongOptionManager.instance.rateSelected * 100)).ToString();

		skinSelectedIndex = SongOptionManager.instance.skinSelected;
		skinSelectedText.text = GameLocalization.instance.Translate("SkinType").Split('$')[SongOptionManager.instance.skinSelected];

		bigNotesToggle.isOn = SongOptionManager.instance.bigNotes;

		objectiveIndex = SongOptionManager.instance.raceSelected;
		objectiveText.text = GameLocalization.instance.Translate("RaceType").Split('$')[SongOptionManager.instance.raceSelected];
		SongOptionManager.instance.convertRaceToMinimumScore();

		hitJudgeIndex = SongOptionManager.instance.hitJudgeSelected;
		hitJudgeText.text = GameLocalization.instance.Translate("HitJudge").Split('$')[SongOptionManager.instance.hitJudgeSelected];

		scoreJudgeIndex = SongOptionManager.instance.scoreJudgeSelected;
		scoreJudgeText.text = GameLocalization.instance.Translate("ScoreJudge").Split('$')[SongOptionManager.instance.scoreJudgeSelected];

		lifeJudgeIndex = SongOptionManager.instance.lifeJudgeSelected;
		lifeJudgeText.text = GameLocalization.instance.Translate("LifeJudge").Split('$')[SongOptionManager.instance.lifeJudgeSelected];

		deathConditionIndex = SongOptionManager.instance.deathConditionSelected;
		deathConditionText.text = GameLocalization.instance.Translate("DeathCondition").Split('$')[SongOptionManager.instance.deathConditionSelected];

		foreach(OptionsMod mod in SongOptionManager.instance.currentOptions)
		{
			
		}
	}
}
