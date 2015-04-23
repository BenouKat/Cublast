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

	public UnityStandardAssets.ImageEffects.Grayscale grayscale;

	public void showCleared(ComboType comboType)
	{
		NoteController.instance.gameObject.SetActive (false);
		ClearedObject.SetActive (true);
		if (comboType != ComboType.NONE) {
			ComboObjects[(int)comboType].SetActive(true);
		}
	}

	public void showFailed()
	{
		FailedObject.SetActive (true);
		LifeController.instance.playDeath ();
		TimeController.instance.stopUpdate ();
		StartCoroutine (endingAnimation ());
	}

	IEnumerator endingAnimation()
	{
		grayscale.enabled = true;
		grayscale.effectAmount = 0f;

		float timeSpent = 0f;
		while (timeSpent < grayscaleTransition) {
			timeSpent += Time.deltaTime;
			grayscale.effectAmount = Mathf.Lerp(0f, 1f, timeSpent/grayscaleTransition);
			yield return 0;
		}
	}
}
