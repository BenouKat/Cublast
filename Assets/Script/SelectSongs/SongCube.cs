using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.UI;

public class SongCube : MonoBehaviour {

	public SongData songData;
	public Song song;

	public Image background;
	public Outline outlineEffect;
	public Text title;
	public Text subtitle;
	public List<GameObject> difficultyButton;

	public Difficulty selectedDifficulty = Difficulty.NONE;

	public SongInfoPanel panel;

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
				if(!found && songData.songs.ContainsKey((Difficulty)i))
				{
					selectedDifficulty = (Difficulty)i;
					found = true;
				}
			}

			if(!found)
			{
				for(int i=(int)selected; i<(int)Difficulty.NONE; i++)
				{
					if(!found && songData.songs.ContainsKey((Difficulty)i))
					{
						selectedDifficulty = (Difficulty)i;
						found = true;
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

		if (isPointed) {
			background.color = SongSelectionManager.instance.songBarSelectedColor[(int)selectedDifficulty];
			panel.songSelected (songData.songs [selectedDifficulty]);
		} else {
			pointOutSong();
		}
		outlineEffect.effectColor = SongSelectionManager.instance.outlineColor[(int)selectedDifficulty];
	}

	public void selectSong()
	{
		if(SongSelectionManager.instance.cantLaunchSong) return;
		song = songData.songs.First().Value;
		if (!musicPreviewLaunched && AudioSelectionManager.instance.checkPreviewAlreadyPlayed (song)) {
			if(song.isMP3())
			{
				if(!MP3Loaded) StartCoroutine(loadAndAssignMP3(song));
			}else{
				AudioSelectionManager.instance.clipInMemory = song.SetAudioClip();
				AudioSelectionManager.instance.playPreview(song.samplestart, song.samplelenght);
			}
			musicPreviewLaunched = true;
		}
		changeDifficulty(selectedDifficulty.ToString());
		song = songData.songs [selectedDifficulty];
		SongSelectionManager.instance.selectOnSong (this);
	}

	public void pointOnSong()
	{
		if(SongSelectionManager.instance.isSelectingSong) return;
		if (selectedDifficulty == Difficulty.NONE) return;
		background.color = SongSelectionManager.instance.songBarSelectedColor[(int)selectedDifficulty];
		timeStartPointed = Time.time;
		isPointed = true;
		musicPreviewLaunched = false;
		panel.songSelected (songData.songs [selectedDifficulty]);
	}

	public void forcePointOut()
	{
		isPointed = true;
		pointOutSong ();
	}

	public void pointOutSong()
	{
		if (selectedDifficulty == Difficulty.NONE || songData == null) return;
		background.color = SongSelectionManager.instance.songBarColor[(int)selectedDifficulty];

		if (isPointed && !SongSelectionManager.instance.isSelectingSong) {
			AudioSelectionManager.instance.stopPreview ();
		}

		if ((isPointed || musicPreviewLaunched)  && !SongSelectionManager.instance.isSelectingSong) {

			if(MP3Loaded)
			{
				song = songData.songs.First().Value;
				song.cleanWav();
				MP3Loaded = false;
			}

		}

		isPointed = false;
		panel.close (songData.songs [selectedDifficulty]);
	}

	void OnDisable()
	{
		pointOutSong();
	}

	bool isPointed;
	float timeStartPointed = -1000f;
	bool musicPreviewLaunched = false;
	bool MP3Loaded = false;
	// Update is called once per frame
	void Update () {
		if(!Mathf.Approximately(Quaternion.Angle(transform.rotation, Quaternion.identity), 0f))
		{
			transform.rotation = Quaternion.identity;
		}

		if(isPointed && !musicPreviewLaunched && Time.time > timeStartPointed  + 0.5f)
		{
			song = songData.songs.First().Value;
			if(AudioSelectionManager.instance.checkPreviewAlreadyPlayed(song))
			{
				if(song.isMP3())
				{
					if(!MP3Loaded) StartCoroutine(loadAndAssignMP3(song));
				}else{
					AudioSelectionManager.instance.clipInMemory = song.SetAudioClip();
					AudioSelectionManager.instance.playPreview(song.samplestart, song.samplelenght);
				}
			}

			musicPreviewLaunched = true;
		}
	}

	IEnumerator loadAndAssignMP3(Song s)
	{
		s.setWav ();
		while (!s.isWavAvailable) {
			yield return 0;
		}

		if (isPointed) {
			AudioSelectionManager.instance.clipInMemory = s.SetAudioClip ();
			AudioSelectionManager.instance.playPreview (song.samplestart, song.samplelenght);
			musicPreviewLaunched = true;
			MP3Loaded = true;
		} else {
			s.cleanWav();
			MP3Loaded = false;
		}
	}

	void OnDestroy()
	{
		if(MP3Loaded)
		{
			song = songData.songs.First().Value;
			song.cleanWav();
			MP3Loaded = false;
		}
	}


	public void stopPreviewCash()
	{
		if(MP3Loaded)
		{
			song = songData.songs.First().Value;
			song.cleanWav();
			MP3Loaded = false;
		}

		AudioSelectionManager.instance.enabled = false;
		AudioSelectionManager.instance.mainMusic.volume = 0f;
		AudioSelectionManager.instance.songMusic.volume = 0f;
	}
}
