using UnityEngine;
using System.Collections;

public class TempPassScene : MonoBehaviour {

	// Use this for initialization
	void Start () {
		LoadManager.instance.Loading();
	}
	
	// Update is called once per frame
	void Update () {
		if(LoadManager.instance.loadingIsDone)
		{
			Application.LoadLevel("SoloChart");
			this.enabled = false;
		}
	}
}
