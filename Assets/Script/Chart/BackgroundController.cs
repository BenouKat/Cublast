using UnityEngine;
using System.Collections;

public class BackgroundController : MonoBehaviour {

	public static BackgroundController instance;
	void Awake()
	{
		if(instance == null) instance = this;
	}

	public float maxUp;
	public Transform BG1;
	public Transform BG2;

	public float speed;
	float speedCursor;
	public float maxBPM;

	// Update is called once per frame
	void Update () {
		BG1.position += Vector3.up*Time.deltaTime*speed*speedCursor;
		if(BG1.position.y >= maxUp) BG1.position -= Vector3.up*maxUp*2f;
		BG2.position += Vector3.up*Time.deltaTime*speed*speedCursor;
		if(BG2.position.y >= maxUp) BG2.position -= Vector3.up*maxUp*2f;
	}

	public void setSpeedCursor(float bpm)
	{
		speedCursor = Mathf.Lerp(0f, 2f, Mathf.Clamp(bpm/maxBPM, 0f, 1f));
	}
}
