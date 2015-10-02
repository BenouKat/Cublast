using UnityEngine;
using System.Collections;

public class AudioSelectionManager : MonoBehaviour {

	public AudioSource mainMusic;
	public AudioSource songMusic;
	public AudioClip clipInMemory;
	public float speedTransition;
	public float speedFadeSample;

	bool isPlayingPreview;
	float timeStartSample;
	float sampleStart;
	float sampleLength;

	Song previewPlayed;

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

	public bool checkPreviewAlreadyPlayed(Song song)
	{
		if (song != previewPlayed) {
			previewPlayed = song;
			return true;
		}
		return false;
	}

	public void playPreview(double sampleStart, double sampleLength)
	{
		clipInMemory.LoadAudioData();
		songMusic.clip = clipInMemory;
		//while(songMusic.clip.loadState == AudioDataLoadState.Loading || songMusic.clip.loadState == AudioDataLoadState.Unloaded){};
		songMusic.Play();
		this.sampleStart = (float)sampleStart;
		this.sampleLength = (float)sampleLength;
		if ((int)(sampleLength + 0.5f) == 12) { //ITG defaultValue
			this.sampleLength = 25f;
		}
		songMusic.time = this.sampleStart;
		timeStartSample = Time.time;
		isPlayingPreview = true;
	}

	public void stopPreview()
	{
		isPlayingPreview = false;
		previewPlayed = null;
	}

	void Update()
	{
		if(isPlayingPreview && mainMusic.volume > 0f)
		{
			mainMusic.volume -= speedTransition*Time.deltaTime;
			songMusic.volume += speedTransition*Time.deltaTime;

			if(mainMusic.volume < 0.5f) SoundWaveManager.instance.source = songMusic;
		}else if(!isPlayingPreview && songMusic.volume > 0f)
		{
			mainMusic.volume += speedTransition*Time.deltaTime;
			songMusic.volume -= speedTransition*Time.deltaTime;

			if(mainMusic.volume > 0.5f) SoundWaveManager.instance.source = mainMusic;
		}

		if (isPlayingPreview && sampleLength > 0f && Time.time > timeStartSample + sampleLength - speedFadeSample) {
			StartCoroutine (fadeSample());
			timeStartSample = Time.time + speedFadeSample*2f;
		}
	}

	IEnumerator fadeSample()
	{
		float timeSpent = 0f;
		while (isPlayingPreview && timeSpent < speedFadeSample) {
			timeSpent += Time.deltaTime;
			songMusic.volume = Mathf.Lerp(1f, 0f, timeSpent/speedFadeSample);
			yield return 0;
		}

		if (isPlayingPreview) {
			songMusic.time = sampleStart;
			songMusic.volume = 1f;
			timeStartSample = Time.time;
		}


	}
}
