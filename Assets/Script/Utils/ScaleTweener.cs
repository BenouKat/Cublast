using UnityEngine;
using System.Collections;

public class ScaleTweener : MonoBehaviour {

	Vector3 baseTween;
	public float scaleMax;
	public float speedRetrieve;

	float tweenerProgression = 1f;

	// Use this for initialization
	void Start () {
		baseTween = transform.localScale;
	}
	
	// Update is called once per frame
	void Update () {
		if (tweenerProgression < 1f) {
			tweenerProgression += Time.deltaTime / speedRetrieve;
			if(tweenerProgression > 1f) tweenerProgression = 1f;
			if(scaleMax < 1f)
			{
				transform.localScale = baseTween * Mathf.Lerp(scaleMax, 1f, tweenerProgression);
			}else{
				transform.localScale = baseTween * Mathf.Lerp(1f, scaleMax, tweenerProgression);
			}
		}
	}

	public void activeTween()
	{
		tweenerProgression = 0f;
	}
}
