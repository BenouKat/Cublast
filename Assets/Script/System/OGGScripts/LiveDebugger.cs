using UnityEngine;
using System.Collections;

public class LiveDebugger : MonoBehaviour {

	public static LiveDebugger instance;

	void Awake()
	{
		if (instance == null) {
			instance = this;
		}
	}

	public void log(string s)
	{
		Debug.Log (s);
	}
}
