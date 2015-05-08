using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class AutoLocalize : MonoBehaviour {

	public string key;
	public string prefix;
	public string postfix;

	void Awake () {
		GetComponent<Text>().text = prefix + GameLocalization.instance.Translate(key) + postfix;
	}
}
