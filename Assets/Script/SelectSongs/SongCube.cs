using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.UI;

public class SongCube : MonoBehaviour {

	public SongData songData;

	public Image background;
	public Outline outlineEffect;
	public Text title;
	public Text subtitle;
	public List<GameObject> difficultyButton;

	Difficulty selectedDifficulty = Difficulty.NONE;

	// Use this for initialization
	void Start () {
		transform.rotation = Quaternion.identity;
	}

	void OnEnable()
	{
		transform.rotation = Quaternion.identity;
		if(songData != null) applyChangeDifficulty(SongSelectionManager.instance.difficultySelected);
	}

	public void refresh()
	{
		Song exempleSong = songData.songs.ElementAt(0).Value;
		title.text = exempleSong.title;
		subtitle.text = exempleSong.subtitle;

		foreach(GameObject difGO in difficultyButton)
		{
			Difficulty d = (Difficulty)System.Enum.Parse(typeof(Difficulty), difGO.name);
			if(songData.songs.ContainsKey(d))
			{
				difGO.transform.FindChild("Level").GetComponent<Text>().text = songData.songs[d].level.ToString();
			}else{
				difGO.SetActive(false);
			}
		}

		applyChangeDifficulty(SongSelectionManager.instance.difficultySelected);
	}

	public void changeDifficulty(string difficulty)
	{
		SongSelectionManager.instance.difficultyChanged(difficulty);
	}

	public void applyChangeDifficulty(Difficulty selected)
	{
		if(!songData.songs.ContainsKey(selected))
		{
			bool found = false;
			for(int i=(int)selected; i>=0; i--)
			{
				if(songData.songs.ContainsKey((Difficulty)i))
				{
					selectedDifficulty = (Difficulty)i;
					found = true;
				}
			}

			if(!found)
			{
				for(int i=(int)selected; i<(int)Difficulty.NONE; i++)
				{
					if(songData.songs.ContainsKey((Difficulty)i))
					{
						selectedDifficulty = (Difficulty)i;
					}
				}
			}
		}else{
			selectedDifficulty = selected;
		}

		foreach(GameObject difGO in difficultyButton)
		{
			Difficulty d = (Difficulty)System.Enum.Parse(typeof(Difficulty), difGO.name);

			if(d == selectedDifficulty)
			{
				difGO.GetComponent<Button>().interactable = false;
				difGO.transform.FindChild("Level").GetComponent<CanvasGroup>().alpha = 1f;
			}else{
				difGO.GetComponent<Button>().interactable = true;
				difGO.transform.FindChild("Level").GetComponent<CanvasGroup>().alpha = 0.4f;
			}
		}

		pointOutSong();
		outlineEffect.effectColor = SongSelectionManager.instance.outlineColor[(int)selectedDifficulty];
	}

	public void selectSong()
	{

	}

	public void pointOnSong()
	{
		background.color = SongSelectionManager.instance.songBarSelectedColor[(int)selectedDifficulty];
		timeStartPointed = Time.time;
		isPointed = true;
		musicPreviewLaunched = false;
	}

	public void pointOutSong()
	{
		background.color = SongSelectionManager.instance.songBarColor[(int)selectedDifficulty];
		isPointed = false;
	}

	void OnDisable()
	{
		pointOutSong();
	}

	bool isPointed;
	float timeStartPointed = -1000f;
	bool musicPreviewLaunched = false;
	// Update is called once per frame
	void Update () {
		if(!Mathf.Approximately(Quaternion.Angle(transform.rotation, Quaternion.identity), 0f))
		{
			transform.rotation = Quaternion.identity;
		}

		if(isPointed && !musicPreviewLaunched && timeStartPointed > Time.time + 1.5f)
		{
			AudioSelectionManager.instance.clipInMemory = songData.songs[0].SetAudioClip();
			AudioSelectionManager.instance.playPreview();
		}
	}
}
