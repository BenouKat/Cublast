using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class SwitchButtonImage : MonoBehaviour {

	public GameObject on;
	public GameObject off;
	public bool isOn;

	void Awake()
	{
		on.SetActive(isOn);
		off.SetActive(!isOn);
	}

	public void switchImage()
	{
		isOn = !isOn;
		on.SetActive(isOn);
		off.SetActive(!isOn);
	}
}
