using UnityEngine;
using System.Collections;

public class GlobalMenu : MonoBehaviour {

	public static GlobalMenu instance;
	void Awake()
	{
		if(instance == null) instance = this;
	}

	public GameObject panelMenu;
	public Animation animationMenu;

	public GameObject backButton;

	public void activeMenu(bool active)
	{
		panelMenu.SetActive(active);
		if(active) animationMenu.Play();
	}

	public void activeBackButton(bool active)
	{
		backButton.SetActive(active);
	}

	public void activeBackButtonClicked()
	{
		Events.instance.FireBackButtonClicked();
	}
}
