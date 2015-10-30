using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class GlobalMenu : MonoBehaviour {

	public static GlobalMenu instance;
	void Awake()
	{
		if(instance == null) instance = this;
	}

	public GameObject panelMenu;
	public Animation animationMenu;

	public GameObject backButton;
	public Text backText;

	public void activeMenu(bool active)
	{
		panelMenu.SetActive(active);
		if(active) animationMenu.Play();
	}

	public void activeBackButton(bool active, string backText)
	{
		backButton.SetActive(active);
		backText = GameLocalization.instance.Translate(backText);
	}

	public void activeBackButtonClicked()
	{
		Events.instance.FireBackButtonClicked();
	}
}
