using UnityEngine;
using System.Collections;

public class CameraOptionsController : MonoBehaviour {

	public UnityStandardAssets.ImageEffects.Antialiasing fxaa;
	public UnityStandardAssets.ImageEffects.ScreenSpaceAmbientOcclusion ssao;
	public UnityStandardAssets.ImageEffects.Bloom bloom;

	// Use this for initialization
	void Awake () {
		optionController();
		Events.instance.CameraOptionChanged += new CameraOptionChangedHandler(optionController);
	}

	void OnDestroy()
	{
		Events.instance.CameraOptionChanged -= new CameraOptionChangedHandler(optionController);
	}

	public void optionController()
	{
		if(GameManager.instance != null)
		{
			if(!GameManager.instance.prefs.onlyOnGame || Application.loadedLevelName.Contains("Chart"))
			{
				if(fxaa != null)
				{
					fxaa.enabled = GameManager.instance.prefs.enableFXAA;
				}

				if(ssao != null)
				{
					ssao.enabled = GameManager.instance.prefs.enablePostProcessEffects;
				}

				if(bloom != null)
				{
					bloom.enabled = GameManager.instance.prefs.enableBloom;
				}
			}

		}
	}
}
