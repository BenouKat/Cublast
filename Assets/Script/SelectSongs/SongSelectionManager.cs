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

	public Color[] songBarColor;
	public Color[] songBarSelectedColor;
	public Color[] outlineColor;
	public GameObject[] particuleColor;
	public Color[] cubeHeartColor;
	public Color[] visualizerBarColor;

	public Material cubeHeartMaterial;
	public Material visualizerMaterial;

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
}
