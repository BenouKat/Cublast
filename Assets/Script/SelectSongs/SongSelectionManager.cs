using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class SongSelectionManager : MonoBehaviour {

	public static SongSelectionManager instance;

	void Awake()
	{
		if(instance == null)
		{
			instance = this;
		}
	}

	public SongInfoPanel infoPanel;
	public SongCubeGenerator generator;
	public Difficulty difficultySelected = Difficulty.NONE;
	public SongPack currentPack;
	public RawImage packImage;
	public SongSearchBar searchBar;
	public Song songSelected;
	public SongData songDataSelected;

	public Color[] songBarColor;
	public Color[] songBarSelectedColor;
	public Color[] outlineColor;
	public GameObject[] particuleColor;
	public Color[] cubeHeartColor;
	public Color[] visualizerBarColor;

	public Material cubeHeartMaterial;
	public Material visualizerMaterial;

	public SongOptionPanel optionUI;

	public bool isSelectingSong = false;
	public bool cantLaunchSong = false;
	public GameObject songSelectedObject;

	public SongLaunchPanel songLaunchPanel;
	public GameObject contextLaunchUI;
	public RectTransform animLaunch;

	public GameObject cacheRaycast;

	public GameObject[] medals;

	void Start()
	{

	}

	void Update()
	{
		if (!isSelectingSong && !cantLaunchSong) {
			generator.checkScrollWheel ();
		}

	}

	public void selectAppropriateDifficulty()
	{
		if (!packContainsDifficulty (difficultySelected)) {
			bool found = false;
			for(int i=(int)difficultySelected; i>=0; i--)
			{
				if(!found && packContainsDifficulty ((Difficulty)i))
				{
					difficultySelected = (Difficulty)i;
					found = true;
				}
			}
			
			if(!found)
			{
				for(int i=(int)difficultySelected; i<(int)Difficulty.NONE; i++)
				{
					if(packContainsDifficulty ((Difficulty)i))
					{
						difficultySelected = (Difficulty)i;
					}
				}
			}
		}
	}

	public bool packContainsDifficulty(Difficulty d)
	{
		foreach (SongData data in currentPack.songsData) {
			if(data.songs.ContainsKey(d)) return true;
		}
		return false;
	}

	public void packSelected(SongPack pack)
	{
		currentPack = pack;
		selectAppropriateDifficulty ();
		generator.instanceAllSongs(currentPack.songsData);
		applyRootVisualEffects ();
		packImage.texture = currentPack.banner ?? GameManager.instance.emptyPackTexture;
	}

	public void nextPack()
	{
		PackManager.instance.selectNextPack ();
		currentPack = PackManager.instance.currentPack;
		generator.instanceAllSongs(currentPack.songsData);
		packImage.texture = currentPack.banner ?? GameManager.instance.emptyPackTexture;
		if (searchBar.opened) {
			searchBar.cleanSearchBar();
			searchBar.callSearchBar ();
		}
			
	}

	public void previousPack()
	{
		PackManager.instance.selectPreviousPack ();
		currentPack = PackManager.instance.currentPack;
		generator.instanceAllSongs(currentPack.songsData);
		packImage.texture = currentPack.banner ?? GameManager.instance.emptyPackTexture;
		if (searchBar.opened) {
			searchBar.cleanSearchBar();
			searchBar.callSearchBar ();
		}
	}

	public void returnToPackSelection()
	{
		if (searchBar.opened) {
			searchBar.cleanSearchBar();
			searchBar.callSearchBar ();
		}
		CameraSwitcher.instance.goToPack ();
	}

	public void difficultyChanged(string difficulty)
	{
		difficultySelected = (Difficulty)System.Enum.Parse(typeof(Difficulty), difficulty);
		foreach(SongCube sc in generator.songCubes)
		{
			if(sc.gameObject.activeInHierarchy)
			{
				sc.applyChangeDifficulty(difficultySelected);
			}
		}

		applyRootVisualEffects ();
	}

	public void applyRootVisualEffects()
	{
		for(int i=0; i<particuleColor.Length; i++)
		{
			if((Difficulty)i == difficultySelected)
			{
				particuleColor[i].SetActive(true);
			}else if(particuleColor[i].activeInHierarchy)
			{
				particuleColor[i].SetActive(false);
			}
		}
		
		cubeHeartMaterial.color = cubeHeartColor[(int)difficultySelected];
		visualizerMaterial.SetColor("_EmissionColor", visualizerBarColor[(int)difficultySelected]);
	}

	private GameObject fakeBar;
	private CanvasGroup cgRolling;
	private GameObject songInQuestion;
	private SongCube songCubeObject;

	public void selectOnSong(SongCube songCubeObj)
	{
		songCubeObject = songCubeObj;
		songDataSelected = songCubeObj.songData;
		songSelected = songCubeObj.songData.songs [songCubeObj.selectedDifficulty];
		StartCoroutine (selectOnSongRoutine (songCubeObj.gameObject));
	}

	public IEnumerator selectOnSongRoutine(GameObject songObject)
	{
		songInQuestion = songObject;
		isSelectingSong = true;
		cantLaunchSong = true;
		cacheRaycast.SetActive (true);
		cgRolling = generator.GetComponent<SongCubeGenerator> ().rollRoot.GetComponent<CanvasGroup> ();
		generator.GetComponent<SongCubeGenerator> ().enabled = false;
		fakeBar = Instantiate (songInQuestion, songInQuestion.transform.position, songInQuestion.transform.rotation) as GameObject;
		fakeBar.transform.SetParent(cgRolling.transform.parent);
		fakeBar.transform.localScale = Vector3.one;
		fakeBar.transform.position = songInQuestion.transform.position;
		fakeBar.GetComponent<RectTransform> ().pivot = new Vector2 (0.5f, 0.5f);
		fakeBar.GetComponent<RectTransform> ().anchoredPosition = new Vector2 (
			fakeBar.GetComponent<RectTransform> ().anchoredPosition.x + 750f,
			fakeBar.GetComponent<RectTransform> ().anchoredPosition.y);

		songInQuestion.SetActive (false);
		infoPanel.GetComponent<Animation> ().Play ("CanvasGroupClose");
		contextLaunchUI.SetActive (true);
		contextLaunchUI.GetComponent<Animation> ().Play ("CanvasGroupOpen");
		animLaunch.GetComponent<Animation> ().Stop ();
		animLaunch.GetComponent<Animation> ().Play ("AnimSongDisappear");
		animLaunch.anchoredPosition = new Vector2(0f, 0f);

		//Adjust speedmod
		SongInfoProfil currentSIP = GameManager.instance.prefs.scoreOnSong.Find (c => c.CompareId (SongSelectionManager.instance.songSelected.sip));
		if (currentSIP != null) {
			SongOptionManager.instance.speedmodSelected = currentSIP.speedmodpref;
		}

		if (GameManager.instance.prefs.inBPMMode) {
			double firstBPM = 0;
			if(SongSelectionManager.instance.songSelected.bpmToDisplay.Contains ("$"))
			{
				firstBPM = double.Parse (SongSelectionManager.instance.songSelected.bpmToDisplay.Split ('$') [0]);
				double secondBPM = double.Parse (SongSelectionManager.instance.songSelected.bpmToDisplay.Split ('$') [1]);
				firstBPM = (double)Mathf.Max((float)firstBPM, (float)secondBPM);
			}else{
				firstBPM = double.Parse (SongSelectionManager.instance.songSelected.bpmToDisplay);
			}

			SongOptionManager.instance.speedmodSelected = (GameManager.instance.prefs.lastBPM / firstBPM);
		}

		float sizeObj = 1f;
		float canvasFloat = 1f;
		float timePast = 0f;
		bool canceled = false;
		bool optionCalled = false;

		while (timePast < 3f) {

			sizeObj = Mathf.Lerp(1f, 1.1f, animLaunch.anchoredPosition.x);
			canvasFloat = Mathf.Lerp(1f, 0f, animLaunch.anchoredPosition.y);

			fakeBar.transform.localScale = Vector3.one * sizeObj;
			cgRolling.alpha = canvasFloat;


			canceled = checkCancelSong();
			optionCalled = checkCallOption();

			if(canceled || optionCalled) timePast = 10000f;

			yield return 0;
			timePast += Time.deltaTime;
		}

		if (!canceled && !optionCalled) {
			callLaunchSong(false);
		} else if (canceled) {
			StartCoroutine(cancelSelectSong(sizeObj, canvasFloat));
		} else if (optionCalled) {
			StartCoroutine(openOption());
		}
	}

	public bool checkCancelSong()
	{
		return Input.GetMouseButtonDown (1) || Input.GetKeyDown (KeyCode.Escape) || Input.GetKeyDown (KeyCode.Backspace);
	}

	public bool checkCallOption()
	{
		return Input.GetMouseButtonDown (0) || Input.GetKeyDown(KeyCode.Return);
	}

	public void callCancelOption()
	{
		StartCoroutine (cancelOption ());
	}

	public IEnumerator cancelSelectSong(float currentSize, float currentAlpha)
	{
		float timePast = 0f;
		float timeRecover = 0.3f;
		float sizeObj = 1f;
		float canvasFloat = 1f;
		infoPanel.GetComponent<Animation> ().Play ("CanvasGroupOpen");
		contextLaunchUI.GetComponent<Animation> ().Play ("CanvasGroupClose");


		while (timePast < timeRecover) {
			sizeObj = Mathf.Lerp(currentSize, 1f, timePast/timeRecover);
			canvasFloat = Mathf.Lerp(currentAlpha, 1f, timePast/timeRecover);

			fakeBar.transform.localScale = Vector3.one * sizeObj;
			cgRolling.alpha = canvasFloat;

			yield return 0;
			timePast += Time.deltaTime;
		}

		cgRolling.alpha = 1f;
		Destroy (fakeBar);
		generator.GetComponent<SongCubeGenerator> ().enabled = true;
		songInQuestion.SetActive (true);
		cacheRaycast.SetActive (false);
		isSelectingSong = false;
		cantLaunchSong = false;
	}

	public IEnumerator openOption()
	{
		cacheRaycast.SetActive (false);
		optionUI.gameObject.SetActive (true);
		optionUI.GetComponent<Animation> ().Play ("OpenOptionUI");
		generator.GetComponent<Animation> ().Play ("CanvasGroupClose");
		contextLaunchUI.GetComponent<Animation> ().Play ("CanvasGroupClose");
		yield return 0;
	}

	public IEnumerator cancelOption()
	{
		generator.GetComponent<Animation> ().Play ("CanvasGroupOpen");
		cgRolling.alpha = 1f;
		Destroy (fakeBar);
		songInQuestion.SetActive (true);
		infoPanel.GetComponent<Animation> ().Play ("CanvasGroupOpen");

		optionUI.GetComponent<Animation> ().Play ("CloseOptionUI");
		optionUI.GetComponent<CanvasGroup> ().blocksRaycasts = false;
		optionUI.GetComponent<CanvasGroup> ().interactable = false;
		isSelectingSong = false;
		songCubeObject.forcePointOut ();

		yield return new WaitForSeconds (0.33f);

		contextLaunchUI.gameObject.SetActive (false);
		optionUI.gameObject.SetActive (false);
		optionUI.GetComponent<CanvasGroup> ().blocksRaycasts = true;
		optionUI.GetComponent<CanvasGroup> ().interactable = true;
		generator.GetComponent<SongCubeGenerator> ().enabled = true;

		cantLaunchSong = false;
	}

	public void callLaunchSong(bool fromOption)
	{
		StartCoroutine (launchSong (fromOption));
	}

	public IEnumerator launchSong(bool fromOption)
	{
		contextLaunchUI.GetComponent<Animation> ().Play ("CanvasGroupClose");
		if (!fromOption) {
			generator.GetComponent<Animation> ().Play ("CanvasGroupClose");
			//yield return new WaitForSeconds (0.33f);
		} else {
			optionUI.GetComponent<Animation> ().Play ("CanvasGroupClose");
			//yield return new WaitForSeconds (0.33f);
		}

		CameraSwitcher.instance.anim.Play ("SongToLaunch");

		songCubeObject.stopPreviewCash ();
		SongOptionManager.instance.currentSongPlayed = songSelected;
		songLaunchPanel.gameObject.SetActive (true);
		songLaunchPanel.titleSong.text = songSelected.title;
		songLaunchPanel.pseudo.text = !string.IsNullOrEmpty (ServerManager.instance.username) ? 
			ServerManager.instance.username : GameLocalization.instance.Translate ("Invited");
		songLaunchPanel.titleDifficulty.text = GameLocalization.instance.Translate (songSelected.difficulty.ToString ());

		songLaunchPanel.mode.text = GameLocalization.instance.Translate (SongOptionManager.instance.isRanked() ? "SoloMode" : "UnrankedSolo");
		songLaunchPanel.localRecord.text = infoPanel.localRecord > 0f ? 
			infoPanel.localRecord.ToString ("0.00") + "%" : GameLocalization.instance.Translate ("NoLocalRecord");
		songLaunchPanel.worldRecord.text = (infoPanel.worldRecordScore > 0f && !string.IsNullOrEmpty(infoPanel.worldRecordName)) ?
			infoPanel.worldRecordName + " (" + infoPanel.worldRecordScore.ToString("0.00") + "%)" : GameLocalization.instance.Translate ("NoWorldRecord");



		yield return new WaitForSeconds(5f);

		TransitionManager.instance.changeSceneWithTransition("SoloChart", 0.2f, 0.5f, false, false);
	}

}
