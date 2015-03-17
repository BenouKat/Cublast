using UnityEngine;
using System.Collections;

public class UnpatchSong : MonoBehaviour {

	public bool go;
	Patcher patch;
	// Use this for initialization
	void Start () {
		patch = GetComponent<Patcher> ();
	}
	
	// Update is called once per frame
	void Update () {
		if (go) {
			go = false;
			patch.Patch(@"E:\Test.ogg");
		}
	}
}
