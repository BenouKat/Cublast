using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AudioController : MonoBehaviour {

	public static AudioController instance;
	void Awake()
	{
		if (instance == null)
			instance = this;

		for(int i=0; i<transform.childCount; i++)
		{
			sounds.Add(transform.GetChild(i).gameObject.GetComponent<AudioSource>());
		}
	}

	public AudioSource audioSource;

	List<AudioSource> sounds = new List<AudioSource>();
	AudioSource currentSource;

	public void loadSong(Song s)
	{
		audioSource.clip = s.SetAudioClip ();
		audioSource.clip.LoadAudioData ();
		while(audioSource.clip.loadState == AudioDataLoadState.Loading || audioSource.clip.loadState == AudioDataLoadState.Unloaded){};
	}

	public void startSong()
	{
		audioSource.Play ();
		audioSource.pitch = (float)SongOptionManager.instance.rateSelected;
		
		SoundWaveManager.instance.init(audioSource);
		SoundWaveManager.instance.activeAnalysis(true);
	}

	public void stopSongFailed()
	{
		audioSource.Stop ();
	}

	public void playSoundOnShot(string sound)
	{
		sounds.Find(c => c.name == sound).gameObject.SetActive(true);
	}

	public void playSound(string sound)
	{
		playSound(sound, 1f, 1f);
	}

	public void playSound(string sound, float pitchMin, float pitchMax)
	{
		currentSource = sounds.Find(c => c.name == sound);
		currentSource.pitch = (pitchMin == pitchMax) ? pitchMin : Random.Range(pitchMin, pitchMax);
		currentSource.PlayOneShot(currentSource.clip);
	}
}
