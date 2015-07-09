using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class SwitchButtonImage : MonoBehaviour {

	public Image on;
	public Image off;
	public bool isOn;

	void Awake()
	{
		on.enabled = isOn;
		off.enabled = !isOn;
	}

	public void switchImage()
	{
		isOn = !isOn;
		on.enabled = isOn;
		off.enabled = !isOn;
	}
}
