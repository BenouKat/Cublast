using UnityEngine;
using System.Collections;

public class EndingController : MonoBehaviour {

	public static EndingController instance;
	void Awake()
	{
		if (instance == null)
			instance = this;
	}

	public GameObject ClearedObject;
	public GameObject FailedObject;
	public GameObject[] ComboObjects;
	public float grayscaleTransition;
	public float slowdownTransition;

	public UnityStandardAssets.ImageEffects.Grayscale grayscale;

	public void showCleared(ComboType comboType)
	{
		NoteController.instance.gameObject.SetActive (false);
		ClearedObject.SetActive (true);
		if (comboType != ComboType.NONE) {
			ComboObjects[(int)comboType].SetActive(true);
			AudioController.instance.playSoundOnShot("Combo");
		}

		Invoke ("back", 3f);
	}

	public void showFailed()
	{
		LifeController.instance.playDeath ();
		TimeController.instance.stopUpdate ();
		AudioController.instance.stopSongFailed();
		AudioController.instance.playSoundOnShot("Failed");
		StartCoroutine (endingAnimation ());

		Invoke ("back", 3f);
	}

	public void back()
	{
		TransitionManager.instance.changeSceneWithTransition("SelectSongs", 0.5f, 0.2f, true, true);
	}

	IEnumerator endingAnimation()
	{
		float positionFirst = ChartManager.instance.scrollingObject.position.y;
		ChartManager.instance.computeTime ();
		ChartManager.instance.moveChart ();
		float positionLast = ChartManager.instance.scrollingObject.position.y;
		float distance = Mathf.Abs (positionLast - positionFirst);

		float timeSpent = 0f;
		while (timeSpent < slowdownTransition) {
			timeSpent += Time.deltaTime;
			ChartManager.instance.manualMoveChart(Mathf.Lerp(distance, 0f, timeSpent/slowdownTransition));
			yield return 0;
		}

		FailedObject.SetActive (true);
		grayscale.enabled = true;
		grayscale.effectAmount = 0f;

		timeSpent = 0f;
		while (timeSpent < grayscaleTransition) {
			timeSpent += Time.deltaTime;
			grayscale.effectAmount = Mathf.Lerp(0f, 1f, timeSpent/grayscaleTransition);
			yield return 0;
		}
	}
}
