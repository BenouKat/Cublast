using UnityEngine;
using System.Collections;

public class Rotate : MonoBehaviour {

	public Vector3 vectorRotation;
	public bool world = true;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		transform.Rotate(vectorRotation*Time.deltaTime, world ? Space.World : Space.Self);
	}
}
