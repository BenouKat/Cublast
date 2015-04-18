using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class AutoLocalize : MonoBehaviour {

	public string key;

	void Awake () {
		GetComponent<Text>().text = GameLocalization.instance.Translate(key);
	}
}
