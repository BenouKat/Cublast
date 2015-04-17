using UnityEngine;
using System.Collections;

public class RotateTick : MonoBehaviour {

	public Vector3 vectorTickRotation;
	public float lengthTick;
	public bool world = true;

	private float startTick;

	// Use this for initialization
	void Start () {
		startTick = -1000;
	}
	
	void Update()
	{
		if (Time.time < startTick + lengthTick) {
			transform.Rotate(vectorTickRotation*(Time.deltaTime/lengthTick), world ? Space.World : Space.Self);
		}
	}

	public void tick()
	{
		startTick = Time.time;
	}
}
