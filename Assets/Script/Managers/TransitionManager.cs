using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.Audio;

public class TransitionManager : MonoBehaviour {

	public static TransitionManager instance;
	void Awake()
	{
		if(instance == null) instance = this;
		DontDestroyOnLoad(this.gameObject);
	}

	public GameObject transitionPanel;
	public Image fadeImage;

	//Recording fadeOut parameters
	float recordedTimeFadeOut;
	bool recordedFadeAudioOut;
	bool fadeRecorded = false;

	void OnLevelWasLoaded()
	{
		if(fadeRecorded)
		{
			StartCoroutine(fadeRoutine(false, "", recordedTimeFadeOut, recordedFadeAudioOut));
			fadeRecorded = false;
		}

	}

	public void changeSceneWithTransition(string scene, float timeFadeIn, float timeFadeOut, bool fadeAudioIn, bool fadeAudioOut)
	{
		recordedTimeFadeOut = timeFadeOut;
		recordedFadeAudioOut = fadeAudioOut;
		StartCoroutine(fadeRoutine(true, scene, timeFadeIn, fadeAudioIn));
		fadeRecorded = true;
	}

	IEnumerator fadeRoutine(bool fadeIn, string scene, float time, bool fadeAudio)
	{
		//Always true and active
		transitionPanel.SetActive(true);
		Color colorFade = fadeImage.color;
		colorFade.a = fadeIn ? 0f : 1f;
		fadeImage.color = colorFade;

		//Avoid start garbage
		yield return 0;yield return 0;
		if(time > 0f)
		{
			GameManager.instance.setMasterVolume(fadeIn ? GameManager.instance.prefs.generalVolume : 0f);

			float timeSpent = 0f;
			while(timeSpent < time)
			{
				timeSpent += Time.deltaTime;
				colorFade = fadeImage.color;
				colorFade.a = Mathf.Lerp(fadeIn ? 0f : 1f, fadeIn ? 1f : 0f, timeSpent/time);
				fadeImage.color = colorFade;
				if(fadeAudio) GameManager.instance.setMasterVolume(Mathf.Lerp(fadeIn ? GameManager.instance.prefs.generalVolume : 0f,
				                                                           fadeIn ? 0f : GameManager.instance.prefs.generalVolume, timeSpent/time));
				yield return 0;
			}
		}

		//Set finals
		colorFade = fadeImage.color;
		colorFade.a = fadeIn ? 1f : 0f;
		fadeImage.color = colorFade;
		GameManager.instance.setMasterVolume(fadeIn ? 0f : GameManager.instance.prefs.generalVolume);

		if(!fadeIn) transitionPanel.SetActive(false);
		if(!string.IsNullOrEmpty(scene)) Application.LoadLevel(scene);
	}
}
