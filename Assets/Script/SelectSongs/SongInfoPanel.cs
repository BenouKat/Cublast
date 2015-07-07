using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public enum WarningInfoSongID { STOP, SLOWDOWN, SPEEDUP }

public class SongInfoPanel : MonoBehaviour {

	Texture2D tempTexture;
	public GameObject root;
	public RawImage bannerImage;
	public Text songTitle;
	public Text artist;
	public Text stepArtist;
	public Text BPMMeter;
	public Text MaxSteam;
	public Text MaxSPS;
	public Text steps;
	public Text jumps;
	public Text hands;
	public Text mines;
	public Text freeze;
	public Text rolls;
	public Text cross;
	public Text footswitch;
	public Text firstScore;
	public Text secondScore;
	public Text thirdScore;
	public Text userScore;
	public LineRenderer graph;
	public GameObject[] warningObjects;
	public RectTransform minWarning;
	public RectTransform maxWarning;

	void Start()
	{
		tempTexture = new Texture2D (1024, 512);
	}

	public void songSelected(Song song)
	{
		root.GetComponent<Animation> ().Stop ();
		root.GetComponent<CanvasGroup> ().alpha = 1f;
		root.SetActive (true);

		if (song.GetBanner (tempTexture) == null) {
			bannerImage.texture = GameManager.instance.emptyPackTexture;
		} else {
			bannerImage.texture = tempTexture ?? GameManager.instance.emptyPackTexture;
		}

		songTitle.text = song.title;
		artist.text = "- " + song.artist + " -";
		stepArtist.text = GameLocalization.instance.Translate("Stepby").Replace("_NAME", song.stepartist);
		if (song.bpmToDisplay.Contains ("$")) {
			BPMMeter.text = song.bpmToDisplay.Split('$')[0] + " - " + song.bpmToDisplay.Split('$')[1] + " BPM"; 
		} else {
			BPMMeter.text = song.bpmToDisplay + " BPM"; 
		}
		MaxSteam.text = GameLocalization.instance.Translate ("Maxstream")
			.Replace ("_TIME", song.longestStream.ToString ("0.0") + "s")
				.Replace ("_SPS", song.stepPerSecondStream.ToString("0.0"));
		MaxSPS.text = song.stepPerSecondMaximum.ToString("0.0") + " " + GameLocalization.instance.Translate ("StepSec");
		steps.text = song.numberOfSteps.ToString() + GameLocalization.instance.Translate ("Steps");
		jumps.text = song.numberOfJumps.ToString() + GameLocalization.instance.Translate ("Jumps");
		hands.text = song.numberOfHands.ToString () + GameLocalization.instance.Translate ("Hands");
		mines.text = song.numberOfMines.ToString () + GameLocalization.instance.Translate ("Mines");
		freeze.text = song.numberOfFreezes.ToString () + GameLocalization.instance.Translate ("Freezes");
		rolls.text = song.numberOfRolls.ToString () + GameLocalization.instance.Translate ("Rolls");
		cross.text = song.numberOfCross.ToString () + GameLocalization.instance.Translate ("DetectedCross");
		footswitch.text = song.numberOfFootswitch.ToString () + GameLocalization.instance.Translate ("DetectedFS");

		firstScore.text = "???";
		secondScore.text = "???";
		thirdScore.text = "???";


	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
