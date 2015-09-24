using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class LoadSongUI : MonoBehaviour {

	public GameObject loadingPanel;
	public GameObject emptyErrorPanel;
	public GameObject errorPanel;

	public Text percentCompletion;
	public Text currentPackLoaded;

	bool quitOnEchap = false;
	bool openWebsiteOnQuit = false;

	// Use this for initialization
	void Start () {
		StartCoroutine(init ());	
	}

	void Update()
	{
		if(Input.anyKeyDown)
		{
			if(quitOnEchap)
			{
				Application.Quit();
			}
		}
	}

	void OnApplicationQuit()
	{
		if(openWebsiteOnQuit)
		{
			Application.OpenURL("http://cublast.blogspot.com");
		}
	}

	IEnumerator init()
	{
		percentCompletion.text = "";
		currentPackLoaded.text = GameLocalization.instance.Translate("PackScan");

		yield return new WaitForSeconds(1f); 

		loadingPanel.SetActive(true);

		yield return new WaitForSeconds(0.7f);

		if(LoadManager.instance.isSongFolderEmpty())
		{
			emptyErrorPanel.SetActive(true);
			loadingPanel.SetActive(false);
			quitOnEchap = true;
			openWebsiteOnQuit = true;
		}else{
			LoadManager.instance.Loading();

			while(!LoadManager.instance.loadingIsDone)
			{
				if(LoadManager.instance.totalSongLoaded > 0)
				{
					percentCompletion.text = (100f*((float)LoadManager.instance.totalSongLoaded / (float)LoadManager.instance.totalSongFound)).ToString("0") + "%";
					currentPackLoaded.text = GameLocalization.instance.Translate("PackLoad").Replace("_PACK", LoadManager.instance.currentPackLoaded);
				}

				yield return 0;
			}
			
			loadingPanel.SetActive(false);
			
			if(LoadManager.instance.songPacks.Count <= 0 || !LoadManager.instance.songPacks.Exists(c => c.songsData.Count > 0))
			{
				quitOnEchap = true;
				errorPanel.SetActive(true);
			}else{
				//TransitionManager.instance.changeSceneWithTransition("MainMenu", 0.5f, 1f, true, true);
				TransitionManager.instance.changeSceneWithTransition("SoloChart", 0.5f, 1f, true, true);
				//TransitionManager.instance.changeSceneWithTransition("SelectSongs", 0.5f, 0.2f, true, true);
			}
		}


	}
}
