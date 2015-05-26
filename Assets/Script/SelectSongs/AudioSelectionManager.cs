using UnityEngine;
using System.Collections;

public class AudioSelectionManager : MonoBehaviour {

	public AudioSource mainMusic;
	public AudioSource songMusic;
	public AudioClip clipInMemory;
	public float speedTransition;

	bool isPlayingPreview;
	float timeStartSample;
	float sampleStart;
	float sampleLength;
	float transisionJauge = 1f;

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

	public void playPreview(double sampleStart, double sampleLength)
	{
		clipInMemory.LoadAudioData();
		songMusic.clip = clipInMemory;
		//while(songMusic.clip.loadState == AudioDataLoadState.Loading || songMusic.clip.loadState == AudioDataLoadState.Unloaded){};
		songMusic.Play();
		this.sampleStart = (float)sampleStart;
		this.sampleLength = (float)sampleLength;
		songMusic.time = Mathf.Clamp(this.sampleStart - speedTransition/2f, 0f, Mathf.Infinity);
		timeStartSample = Time.time;
		isPlayingPreview = true;
	}

	public void stopPreview()
	{
		StopCoroutine (fadeSample ());
		isPlayingPreview = false;
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

		if (isPlayingPreview && sampleLength > 0f && Time.time > timeStartSample + sampleLength - speedTransition) {
			StartCoroutine (fadeSample());
			timeStartSample = Time.time + speedTransition;
		}
	}

	IEnumerator fadeSample()
	{
		float timeSpent = 0f;
		while (isPlayingPreview && timeSpent < speedTransition) {
			timeSpent += Time.deltaTime;
			songMusic.volume = Mathf.Lerp(1f, 0f, timeSpent);
			yield return 0;
		}

		songMusic.time = sampleStart;
		songMusic.volume = 1f;
		timeStartSample = Time.time;
	}
}
