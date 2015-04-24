using UnityEngine;
using System.Collections;

public class AudioController : MonoBehaviour {

	public static AudioController instance;
	void Awake()
	{
		if (instance == null)
			instance = this;
	}

	public AudioSource audioSource;

	public void loadSong(Song s)
	{
		audioSource.clip = s.SetAudioClip ();
		audioSource.clip.LoadAudioData ();
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
}
