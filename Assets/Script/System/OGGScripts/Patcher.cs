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
			if (patcherOption.FileMeetsConditions (ogglength, filePath)) {
				/* Fait planter Unity pour le moment...
				//double lengthToPatchTo = ogglength.getRealTime (filePath);
				//ogglength.closeOggFile();
				//LiveDebugger.instance.log(lengthToPatchTo.ToString()); */
				ogglength.ChangeSongLength (filePath, -1);
			}
		}
	}

	public bool IsPatchedSong(string filePath)
	{
		if (File.Exists (filePath)) {
			return patcherOption.FileMeetsConditions (ogglength, filePath);
		}
		return false;
	}

}

/*
 	Thanks to Greg Najda for C++ equivalent
 	https://github.com/LHCGreg/itgoggpatch
*/