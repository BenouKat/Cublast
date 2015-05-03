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
		DontDestroyOnLoad (this);
	}

	public TextAsset localisation;
	public Language currentLanguage;

	public Hashtable dictionary;

	public void reloadDictionary()
	{
		string[] allLines = localisation.text.Split ('\n');

		dictionary = new Hashtable();

		for (int i=0; i<allLines.Length; i++) {
			string[] lineSplit = allLines[i].Split(';');
			dictionary.Add(lineSplit[0], lineSplit[1 + (int)currentLanguage]);
		}
	}

	public string Translate(string key)
	{
		return ((string)dictionary[key]).Replace("\\n", "\n");
	}
}
