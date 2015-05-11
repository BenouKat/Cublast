using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class MainMenuMode : MonoBehaviour {

	public Button multiButton;

	public Animation cameraAnim;

	public Animation optionMenu;

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

	public void backFromOption()
	{
		optionMenu.Play ("CloseOptionMenu");
		cameraAnim.Play("OptionToMainMenu");
		Invoke ("playCameraShake", 0.5f);
	}
	
	public void playCameraShake()
	{
		cameraAnim.Play("OptionToMainMenu");
	}

	public void openOption()
	{
		optionMenu.gameObject.SetActive(true);
		optionMenu.Play ("OpenOptionMenu");
	}

	public void goToOption()
	{
		CancelInvoke ("playCameraShake");
		cameraAnim.Play("GoToOption");
		Invoke ("openOption", 0.5f);
	}

	public void goToQuit()
	{
		Application.Quit();
	}
}
