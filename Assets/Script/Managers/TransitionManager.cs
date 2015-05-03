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
	public AudioMixer masterMixer;
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
			masterMixer.SetFloat("masterVol", fadeIn ? 0f : -40f);

			float timeSpent = 0f;
			while(timeSpent < time)
			{
				timeSpent += Time.deltaTime;
				colorFade = fadeImage.color;
				colorFade.a = Mathf.Lerp(fadeIn ? 0f : 1f, fadeIn ? 1f : 0f, timeSpent/time);
				fadeImage.color = colorFade;
				if(fadeAudio) masterMixer.SetFloat("masterVol", Mathf.Lerp(fadeIn ? 0f : -40f, fadeIn ? -40f : 0f, timeSpent/time));
				yield return 0;
			}
		}

		//Set finals
		colorFade = fadeImage.color;
		colorFade.a = fadeIn ? 1f : 0f;
		fadeImage.color = colorFade;
		masterMixer.SetFloat("masterVol", fadeIn ? -80f : 0f);

		if(!fadeIn) transitionPanel.SetActive(false);
		if(!string.IsNullOrEmpty(scene)) Application.LoadLevel(scene);
	}
}
