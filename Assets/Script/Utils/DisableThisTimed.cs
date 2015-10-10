using UnityEngine;
using System.Collections;

public class DisableThisTimed : MonoBehaviour {

	public float time;

	// Use this for initialization
	void OnEnable () {
		Invoke ("disable", time);	
	}

	public void disable()
	{
		gameObject.SetActive(false);
	}
}
