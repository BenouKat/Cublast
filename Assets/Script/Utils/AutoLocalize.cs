using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class AutoLocalize : MonoBehaviour {

	public string key;
	public string prefix;
	public string postfix;
	public bool toUpperCase;

	string textTemp;

	void Awake () {
		textTemp = prefix + GameLocalization.instance.Translate(key) + postfix;
		if (toUpperCase)
			textTemp = textTemp.ToUpper ();

		GetComponent<Text>().text = textTemp;
	}
}
