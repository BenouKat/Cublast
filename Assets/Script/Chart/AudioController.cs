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
			sounds.Add(transform.GetChild(i).gameObject);
		}
	}

	public AudioSource audioSource;

	List<GameObject> sounds = new List<GameObject>();

	public void loadSong(Song s)
	{
		audioSource.clip = s.SetAudioClip ();
		audioSource.clip.LoadAudioData ();
		while(audioSource.clip.loadState == AudioDataLoadState.Loading || audioSource.clip.loadState == AudioDataLoadState.Unloaded){};
	}

	public void startSong()
	{
		audioSource.Play ();
		
		SoundWaveManager.instance.init(audioSource);
		SoundWaveManager.instance.activeAnalysis(true);
	}

	public void stopSongFailed()
	{
		audioSource.Stop ();
	}

	public void playSoundOnShot(string sound)
	{
		sounds.Find(c => c.name == sound).SetActive(true);
	}
}
