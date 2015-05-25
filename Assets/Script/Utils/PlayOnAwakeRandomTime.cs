using UnityEngine;
using System.Collections;

public class PlayOnAwakeRandomTime : MonoBehaviour {

	public float[] times;
	
	// Use this for initialization
	void Awake () {
		GetComponent<AudioSource>().Play();
		GetComponent<AudioSource>().time = times[Random.Range(0, times.Length)];
	}
}
