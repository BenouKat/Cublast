using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.UI;

public enum WarningInfoSongID { STOP, SLOWDOWN, SPEEDUP }

public class SongInfoPanel : MonoBehaviour {

	public bool opened = false;
	public bool closed = true;
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

	public float maxYGraph = 80f;
	public float maxXGraph = 330f;

	public SongTop currentSongTop;

	List<GameObject> warningObjectPool = new List<GameObject>();

	void Start()
	{
		tempTexture = new Texture2D (1024, 512);
	}

	public void songSelected(Song song)
	{
		opened = true;
		closed = false;
		root.GetComponent<Animation> ().Stop ();
		root.GetComponent<CanvasGroup> ().alpha = 1f;
		root.SetActive (true);

		if (song.GetBanner (tempTexture) == null) {
			bannerImage.texture = SongSelectionManager.instance.packImage.texture ?? GameManager.instance.emptyPackTexture;
		} else {
			bannerImage.texture = tempTexture ?? SongSelectionManager.instance.packImage.texture ?? GameManager.instance.emptyPackTexture;
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
		steps.text = song.numberOfSteps.ToString() + " " + GameLocalization.instance.Translate ("Steps");
		jumps.text = song.numberOfJumps.ToString() + " " + GameLocalization.instance.Translate ("Jumps");
		hands.text = song.numberOfHands.ToString () + " " + GameLocalization.instance.Translate ("Hands");
		mines.text = song.numberOfMines.ToString () + " " + GameLocalization.instance.Translate ("Mines");
		freeze.text = song.numberOfFreezes.ToString () + " " + GameLocalization.instance.Translate ("Freezes");
		rolls.text = song.numberOfRolls.ToString () + " " + GameLocalization.instance.Translate ("Rolls");
		cross.text = song.numberOfCross.ToString () + " " + GameLocalization.instance.Translate ("DetectedCross");
		footswitch.text = song.numberOfFootswitch.ToString () + " " + GameLocalization.instance.Translate ("DetectedFS");

		firstScore.text = "???";
		secondScore.text = "???";
		thirdScore.text = "???";

		int length = song.intensityGraph.Length;
		graph.SetVertexCount(length);
		for (int i=0; i<length; i++) {

			graph.SetPosition(i, new Vector3(Mathf.Lerp(0f, maxXGraph, (float)i/(float)length), 
			                                 Mathf.Lerp(0f, maxYGraph, (float)(song.intensityGraph[i] / song.stepPerSecondMaximum)), 
			                                 0f));
		}

		GameObject poolObject = null;

		foreach (GameObject goPool in warningObjectPool) {
			goPool.SetActive(false);
		}

		for (int i=0; i<song.stops.Count; i++) {
			poolObject = warningObjectPool.Find(c => c.name == WarningInfoSongID.STOP.ToString() && c.activeSelf);
			if(poolObject == null)
			{
				poolObject = Instantiate(warningObjects[(int)WarningInfoSongID.STOP], 
				                         warningObjects[(int)WarningInfoSongID.STOP].transform.position,
				                         warningObjects[(int)WarningInfoSongID.STOP].transform.rotation) as GameObject;
				poolObject.transform.SetParent(warningObjects[(int)WarningInfoSongID.STOP].transform.parent);
				poolObject.transform.localScale = Vector3.one;
				warningObjectPool.Add(poolObject);
			}
			poolObject.GetComponent<RectTransform>().anchoredPosition = new Vector2(Mathf.Lerp(0f, maxXGraph, (float)(song.stops.ElementAt(i).Key / song.duration)), 0f);
			poolObject.SetActive(true);
		}

		if (song.bpms.Count > 0) {
			double currentBPM = song.bpms.ElementAt (0).Value;
			for(int i=1; i<song.bpms.Count; i++)
			{
				if(song.bpms.ElementAt(i).Value < currentBPM)
				{
					poolObject = warningObjectPool.Find(c => c.name == WarningInfoSongID.SLOWDOWN.ToString() && c.activeSelf);
					if(poolObject == null)
					{
						poolObject = Instantiate(warningObjects[(int)WarningInfoSongID.SLOWDOWN], 
						                         warningObjects[(int)WarningInfoSongID.SLOWDOWN].transform.position,
						                         warningObjects[(int)WarningInfoSongID.SLOWDOWN].transform.rotation) as GameObject;
						poolObject.transform.SetParent(warningObjects[(int)WarningInfoSongID.SLOWDOWN].transform.parent);
						poolObject.transform.localScale = Vector3.one;
						warningObjectPool.Add(poolObject);
					}
					poolObject.GetComponent<RectTransform>().anchoredPosition = new Vector2(Mathf.Lerp(0f, maxXGraph, (float)(song.bpms.ElementAt(i).Key / song.duration)), 0f);
					poolObject.SetActive(true);
				}else if(song.bpms.ElementAt(i).Value > currentBPM)
				{
					poolObject = warningObjectPool.Find(c => c.name == WarningInfoSongID.SPEEDUP.ToString() && c.activeSelf);
					if(poolObject == null)
					{
						poolObject = Instantiate(warningObjects[(int)WarningInfoSongID.SPEEDUP], 
						                         warningObjects[(int)WarningInfoSongID.SPEEDUP].transform.position,
						                         warningObjects[(int)WarningInfoSongID.SPEEDUP].transform.rotation) as GameObject;
						poolObject.transform.SetParent(warningObjects[(int)WarningInfoSongID.SPEEDUP].transform.parent);
						poolObject.transform.localScale = Vector3.one;
						warningObjectPool.Add(poolObject);
					}
					poolObject.GetComponent<RectTransform>().anchoredPosition = new Vector2(Mathf.Lerp(0f, maxXGraph, (float)(song.bpms.ElementAt(i).Key / song.duration)), 0f);
					poolObject.SetActive(true);
				}

				currentBPM = song.bpms.ElementAt(i).Value;
			}


		}

		userScore.text = "...";
		firstScore.text = "...";
		secondScore.text = "...";
		thirdScore.text = "...";

		SongInfoProfil sipSong = GameManager.instance.prefs.scoreOnSong.Find (c => c.CompareId (song.sip));
		if (sipSong != null) {
			userScore.text = sipSong.score.ToString ("0.00") + "%";
		} else {
			userScore.text = GameLocalization.instance.Translate("NoRecord");
		}

		currentSongTop = null;

		if (ServerManager.instance.isOnline ()) {
			ServerManager.instance.retrieveScore (song.sip.getSongNetId (), delegate(SongTop songTop) {
				if (this != null) {
					currentSongTop = songTop;
					firstScore.text = "---";
					secondScore.text = "---";
					thirdScore.text = "---";

					if(songTop.users.Count > 0) firstScore.text = songTop.users[0] + " (" + ((float)songTop.score[0] / 100f).ToString("0.00") + "%)";
					if(songTop.users.Count > 1) secondScore.text = songTop.users[1] + " (" + ((float)songTop.score[1] / 100f).ToString("0.00") + "%)";
					if(songTop.users.Count > 2) thirdScore.text = songTop.users[2] + " (" + ((float)songTop.score[2] / 100f).ToString("0.00") + "%)";

					for(int i=0; i<songTop.users.Count; i++)
					{
						if(songTop.users[i].Equals(ServerManager.instance.username))
						{
							userScore.text = ((float)songTop.score[2] / 100f).ToString("0.00") + "% (" + i + "e)";
						}
					}
				}
			}, delegate() {
				firstScore.text = "---";
				secondScore.text = "---";
				thirdScore.text = "---";
			});
		} else {
			firstScore.text = "---";
			secondScore.text = "---";
			thirdScore.text = "---";
		}
	}

	float timeStartClose;
	public float timeClose = 0.5f;
	public void close()
	{
		opened = false;
		closed = false;
		timeStartClose = Time.time;
	}
	
	// Update is called once per frame
	void Update () {
		if (!opened && !closed && Time.time > timeStartClose) {
			root.GetComponent<Animation> ().Play ("CloseSongInfoPanel");
			closed = true;
		}
	}
}
