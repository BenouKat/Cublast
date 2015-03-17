using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class Patcher : MonoBehaviour {

	Ogglength ogglength;
	PatcherOptions patcherOption;


	void Awake()
	{
		ogglength = GetComponent<Ogglength>();
		patcherOption = new PatcherOptions();
	}

	public void Patch(List<string> filePaths)
	{
		foreach (string filePath in filePaths) {
			Patch (filePath);
		}
	}

	public void Patch(string filePath)
	{
		if (File.Exists (filePath)) {
			LiveDebugger.instance.log ("The file exist");
			if (patcherOption.FileMeetsConditions (ogglength, filePath)) {
				//double lengthToPatchTo = ogglength.getRealTime (filePath);
				StartCoroutine(ogglength.getRealTimeTest(filePath));
				//LiveDebugger.instance.log ("length to patch : " + lengthToPatchTo);
				//ogglength.ChangeSongLength (filePath, lengthToPatchTo);
			}
		}
	}

}

/*
 	Thanks to Greg Najda for C++ equivalent
 	https://github.com/LHCGreg/itgoggpatch
*/