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
	public GameObject songSelectedObject;

	void Start()
	{

	}

	void Update()
	{
		generator.checkScrollWheel ();
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

	public IEnumerator selectOnSong(GameObject songSelected)
	{
		songInQuestion = songSelected;
		isSelectingSong = true;
		cgRolling = generator.GetComponent<SongCubeGenerator> ().rollRoot.GetComponent<CanvasGroup> ();
		generator.GetComponent<SongCubeGenerator> ().enabled = false;
		fakeBar = Instantiate (songInQuestion, songInQuestion.transform.position, songInQuestion.transform.rotation) as GameObject;
		fakeBar.transform.parent = songInQuestion.transform.parent;
		fakeBar.GetComponent<RectTransform> ().pivot = new Vector2 (0.5f, 0.5f);
		songInQuestion.SetActive (false);

		float sizeObj = 1f;
		float canvasFloat = 1f;
		float timePast = 0f;
		float firstPhase = 0.5f;
		float secondPhase = 2.5f;
		bool canceled = false;
		bool optionCalled = false;

		while (timePast < firstPhase + secondPhase) {

			if(timePast < firstPhase)
			{
				sizeObj = Mathf.Lerp(1f, 1.2f, timePast/firstPhase);
				canvasFloat = Mathf.Lerp(1f, 0.7f, timePast/firstPhase);
			}else{
				sizeObj = Mathf.Lerp(1.2f, 1.4f, (timePast + firstPhase)/secondPhase);
				canvasFloat = Mathf.Lerp(0.7f, 0.2f, (timePast + firstPhase)/secondPhase);
			}

			fakeBar.transform.localScale = Vector3.one * sizeObj;
			cgRolling.alpha = canvasFloat;


			canceled = checkCancelSong();
			optionCalled = checkCallOption();

			if(canceled || optionCalled) timePast = 10000f;

			yield return 0;
			timePast += Time.deltaTime;
		}

		if (!canceled && !optionCalled) {
			launchSong(false);
		} else if (canceled) {
			cancelSelectSong(sizeObj, canvasFloat);
		} else if (optionCalled) {
			openOption();
		}
	}

	public bool checkCancelSong()
	{
		return Input.GetMouseButtonDown (1) || Input.GetKeyDown (KeyCode.Escape) || Input.GetKeyDown (KeyCode.Backspace);
	}

	public bool checkCallOption()
	{
		return Input.GetMouseButtonDown (0);
	}

	public IEnumerator cancelSelectSong(float currentSize, float currentAlpha)
	{
		float timePast = 0f;
		float timeRecover = 0.3f;
		float sizeObj = 1f;
		float canvasFloat = 1f;

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
		isSelectingSong = false;
	}

	public IEnumerator openOption()
	{
		optionUI.GetComponent<Animation> ().Play ("OpenOptionUI");
		generator.GetComponent<Animation> ().Play ("CanvasGroupClose");
	}

	public IEnumerator cancelOption()
	{
		generator.GetComponent<Animation> ().Play ("CanvasGroupOpen");
		cgRolling.alpha = 1f;
		Destroy (fakeBar);
		songInQuestion.SetActive (true);

		optionUI.GetComponent<Animation> ().Play ("CloseOptionUI");
		yield return new WaitForSeconds (0.33f);
		
		generator.GetComponent<SongCubeGenerator> ().enabled = true;
		isSelectingSong = false;
	}

	public IEnumerator launchSong(bool fromOption)
	{
		if (!fromOption) {
			generator.GetComponent<Animation> ().Play ("CanvasGroupClose");
			yield return new WaitForSeconds (0.33f);
		} else {
			optionUI.GetComponent<Animation> ().Play ("CanvasGroupClose");
			yield return new WaitForSeconds (0.33f);
		}

	}

}
