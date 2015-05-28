using UnityEngine;
using System.Collections;

public class PlayAnimationOnEnable : MonoBehaviour {

	void OnEnable()
	{
		GetComponent<Animation> ().Play ();
	}
}
