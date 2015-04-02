using UnityEngine;
using System.Collections;

public class EmissionTweener : MonoBehaviour {

	public Color baseEmissionColor;
	Color currentEmissionColor;
	public float baseEmissionPower = 1f;
	float currentEmissionPower = 1f;
	public float speedEmissionDecrease = 1f;
	public Material concernedMaterial;

	public bool letInUpdate;

	void Start()
	{
		currentEmissionPower = baseEmissionPower;
		refreshEmission ();
	}

	void Update()
	{
		if (letInUpdate) {
			let ();
		}
	}

	public void pulse()
	{
		currentEmissionPower = 1f;
		refreshEmission();
	}
	
	public void enableLetInUpdate(bool active)
	{
		letInUpdate = active;
	}
	
	public void let()
	{
		currentEmissionPower -= Time.deltaTime / speedEmissionDecrease;
		refreshEmission();
	}
	
	public void refreshEmission()
	{
		currentEmissionColor = currentEmissionPower*baseEmissionColor;
		concernedMaterial.SetColor("_EmissionColor", currentEmissionColor);
	}
}
