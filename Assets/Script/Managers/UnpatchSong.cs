using UnityEngine;
using System.Collections;

public class UnpatchSong : MonoBehaviour {

	public bool go;
	Patcher patch;
	// Use this for initialization
	void Start () {
		patch = new Patcher();
	}
	
	// Update is called once per frame
	void Update () {
		if (go) {
			go = false;
			Debug.Log("is patched song ? " + patch.IsPatchedSong(@"C:\Test.ogg").ToString());
			patch.Patch(@"C:\Test.ogg");
		}
	}
	
}
