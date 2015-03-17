using System.Collections;
using System.Collections.Generic;
using System.IO;

public class Patcher {

	Ogglength ogglength;
	PatcherOptions patcherOption;


	public Patcher()
	{
		ogglength = new Ogglength();
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
				double lengthToPatchTo = ogglength.getRealTime (filePath);
				LiveDebugger.instance.log ("length to patch : " + lengthToPatchTo);
				//ogglength.ChangeSongLength (filePath, lengthToPatchTo);
			}
		}
	}

}

/*
 	Thanks to Greg Najda for C++ equivalent
 	https://github.com/LHCGreg/itgoggpatch
*/