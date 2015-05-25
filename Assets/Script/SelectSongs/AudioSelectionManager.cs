using UnityEngine;
using System.Collections;

public class AudioSelectionManager : MonoBehaviour {

	public AudioSource mainMusic;
	public AudioSource songMusic;
	public AudioClip clipInMemory;
	public float speedTransition;

	bool isPlayingPreview;

	public static AudioSelectionManager instance;
	void Awake()
	{
		if(instance == null) instance = this;
	}

	void Start()
	{
		mainMusic.volume = 1f;
		songMusic.volume = 0f;
	}

	public void playPreview()
	{
		clipInMemory.LoadAudioData();
		songMusic.clip = clipInMemory;
		while(songMusic.clip.loadState == AudioDataLoadState.Loading || songMusic.clip.loadState == AudioDataLoadState.Unloaded){};
		songMusic.Play();
		isPlayingPreview = true;
	}

	public void stopPreview()
	{
		isPlayingPreview = false;
	}

	void Update()
	{
		if(isPlayingPreview && mainMusic.volume > 0f)
		{
			mainMusic.volume -= speedTransition*Time.deltaTime;
			songMusic.volume += speedTransition*Time.deltaTime;

			if(songMusic.volume > 0.5f) SoundWaveManager.instance.source = songMusic;
		}else if(!isPlayingPreview && songMusic.volume > 0f)
		{
			mainMusic.volume += speedTransition*Time.deltaTime;
			songMusic.volume -= speedTransition*Time.deltaTime;

			if(mainMusic.volume > 0.5f) SoundWaveManager.instance.source = mainMusic;
		}
	}
}
