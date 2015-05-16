using UnityEngine;
using System.Collections;

public class KeepDirection : MonoBehaviour {

	public Vector3 direction;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		transform.forward = direction;
	}
}
