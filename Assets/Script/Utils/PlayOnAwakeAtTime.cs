using UnityEngine;
using System.Collections;

public class PlayOnAwakeAtTime : MonoBehaviour {

	public AudioSource source;
	public float time;
	public bool random;
	public float randomStart;
	public float randomEnd;

	void Awake()
	{
		source.time = random ? Random.Range(randomStart, randomEnd) : time;
		source.Play();
	}
}
