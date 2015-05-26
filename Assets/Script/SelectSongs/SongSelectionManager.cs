using UnityEngine;
using System.Collections;

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
	public Difficulty difficultySelected = Difficulty.EASY;

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

	public void packSelected(SongPack pack)
	{
		generator.instanceAllSongs(pack.songsData);
		applyRootVisualEffects ();
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
