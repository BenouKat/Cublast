using UnityEngine;
using System.Collections;

public class EmissionTweener : MonoBehaviour {

	public Color baseEmissionColor;
	Color currentEmissionColor;
	public float baseEmissionPower = 1f;
	public float maxEmissionPower = 1f;
	float currentEmissionPower = 1f;
	public float speedEmissionDecrease = 1f;
	public Material concernedMaterial;

	public bool letInUpdate;

	void Start()
	{
		if (concernedMaterial != null)
			init ();
	}

	void Update()
	{
		if (letInUpdate) {
			let ();
		}
	}

	public void init()
	{
		currentEmissionPower = baseEmissionPower;
		refreshEmission ();
	}

	public void pulse()
	{
		currentEmissionPower = maxEmissionPower;
		refreshEmission();
	}
	
	public void enableLetInUpdate(bool active)
	{
		letInUpdate = active;
	}
	
	public void let()
	{
		if (currentEmissionPower <= 0)
			return;
		currentEmissionPower -= (Time.deltaTime / speedEmissionDecrease) * maxEmissionPower;
		refreshEmission();
	}
	
	public void refreshEmission()
	{
		currentEmissionColor = currentEmissionPower*baseEmissionColor;
		concernedMaterial.SetColor("_EmissionColor", currentEmissionColor);
	}
}
