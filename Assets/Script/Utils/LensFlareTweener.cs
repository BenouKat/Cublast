using UnityEngine;
using System.Collections;

public class LensFlareTweener : MonoBehaviour {

	LensFlare flare;

	public float startBrightness;
	public float endBrightness;
	public float time;
	
	IEnumerator tween()
	{
		float timeSpent = 0f;
		while (timeSpent < time) {
			timeSpent += Time.deltaTime;
			flare.brightness = Mathf.Lerp(startBrightness, endBrightness, timeSpent/time);
			yield return 0;
		}

		flare.enabled = false;
	}

	public void enableTween()
	{
		flare = GetComponent<LensFlare> ();
		flare.enabled = true;
		StartCoroutine (tween ());
	}
}
