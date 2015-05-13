using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TempPassScene : MonoBehaviour {

	// Use this for initialization
	void Start () {
		TransitionManager.instance.changeSceneWithTransition("LoadingSongs", 0f, 0.2f, false, true);
		this.enabled = false;
	}
}
