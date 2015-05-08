using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class MainMenuMode : MonoBehaviour {

	public Button multiButton;

	public Animation cameraAnim;

	// Use this for initialization
	void Start () {
		if(!ServerManager.instance.connected || !ServerManager.instance.connectedToRoom)
		{
			multiButton.interactable = false;
		}
	}
	
	// Update is called once per frame
	void Update () {
	
	}


	public void goToOption()
	{
		cameraAnim.Play("GoToOption");
	}

	public void goToQuit()
	{
		Application.Quit();
	}
}
