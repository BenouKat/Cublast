using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameLocalization : MonoBehaviour {

	public static GameLocalization instance;

	void Awake()
	{
		if (instance == null)
			instance = this;

		reloadDictionary ();
	}

	public TextAsset localisation;
	public Language currentLanguage;

	public Hashtable dictionary;

	public void reloadDictionary()
	{
		string[] allLines = localisation.text.Split (new string[] { System.Environment.NewLine }, System.StringSplitOptions.None);

		dictionary = new Hashtable();

		for (int i=0; i<allLines.Length; i++) {
			string[] lineSplit = allLines[i].Split(';');
			dictionary.Add(lineSplit[0], lineSplit[1 + (int)currentLanguage]);
		}
	}

	public string Translate(string key)
	{
		return (string)dictionary[key];
	}
}
