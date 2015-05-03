using UnityEngine;
using System.Collections;

public class TweenSoundFade : MonoBehaviour {

	public AudioSource source;
	public float time;
	public float from;
	public float to;
	public float delay;

	// Use this for initialization
	void Start () {
		StartCoroutine(tweenSound());
	}
	
	IEnumerator tweenSound()
	{
		//Pass fiew frame to avoid loading garbage
		yield return 0; yield return 0;yield return 0;yield return 0;
		float timeSpent = 0f;
		while(timeSpent < time)
		{
			timeSpent += Time.deltaTime;
			source.volume = Mathf.Lerp(from < to ? from : to, from < to ? to : from, timeSpent/time);
			yield return 0;
		}
	}
}
