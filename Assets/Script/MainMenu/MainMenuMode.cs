using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class MainMenuMode : MonoBehaviour {

	public Button multiButton;

	public Animation optionMenu;

	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void backFromOption()
	{
		optionMenu.Play ("CloseOptionMenu");
		optionMenu.GetComponent<MainMenuOptions> ().closePanel ();
		Invoke ("playCameraShake", 1f);
	}
	
	public void playCameraShake()
	{
		optionMenu.gameObject.SetActive(false);
	}

	public void openOption()
	{
		optionMenu.gameObject.SetActive(true);
		optionMenu.Play ("OpenOptionMenu");
	}

	public void goToOption()
	{
		CancelInvoke ("playCameraShake");
		Invoke ("openOption", 0.5f);
	}

	public void goToQuit()
	{
		Application.Quit();
	}
}
