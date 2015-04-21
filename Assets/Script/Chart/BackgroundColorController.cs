using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BackgroundColorController : MonoBehaviour {

	public static BackgroundColorController instance;
	void Awake()
	{
		if(instance == null) instance = this;
	}
	//Transition
	public List<Color> colorList;
	public List<Color> visualizerColorList;
	public float speedTransition;
	//Exposure
	public float fadeExposureTransition;
	public float baseExposure;
	public float maxExposure;
	float currentExposure;

	//System
	int currentIndex = 0;
	int nextIndex = 1;

	Color[] colorArray;
	Color[] colorVisualizerArray;

	//Materials
	public Material backgroundMaterial;
	public Material visualizerMaterial;

	// Use this for initialization
	void Start () {
		List<Color> tempColorList = new List<Color>();
		tempColorList.AddRange(colorList);
		List<Color> tempColorVisuList = new List<Color>();
		tempColorVisuList.AddRange(visualizerColorList);

		colorArray = new Color[tempColorList.Count];
		colorVisualizerArray = new Color[tempColorVisuList.Count];

		int arrayCounter = 0;
		int indexSelected = 0;
		while(tempColorList.Count > 0)
		{
			indexSelected = Random.Range(0, tempColorList.Count);

			colorArray[arrayCounter] = tempColorList[indexSelected];
			colorVisualizerArray[arrayCounter] = tempColorVisuList[indexSelected];
			tempColorList.RemoveAt(indexSelected);
			tempColorVisuList.RemoveAt(indexSelected);

			arrayCounter++;
		}

		backgroundMaterial.SetColor("_SkyTint", colorArray[currentIndex]);
		visualizerMaterial.SetColor("_EmissionColor", colorVisualizerArray[currentIndex]);
		currentExposure = baseExposure;
		backgroundMaterial.SetFloat("_Exposure", currentExposure);
	}
	
	// Update is called once per frame
	float timeSpent = 0f;
	void Update () {
		timeSpent += Time.deltaTime;
		backgroundMaterial.SetColor("_SkyTint", Color.Lerp(colorArray[currentIndex], colorArray[nextIndex], timeSpent/speedTransition));
		visualizerMaterial.SetColor("_EmissionColor", Color.Lerp(colorVisualizerArray[currentIndex], colorVisualizerArray[nextIndex], timeSpent/speedTransition));
		if(timeSpent >= speedTransition) setNextIndex();

		if(currentExposure > baseExposure)
		{
			currentExposure = Mathf.Clamp(currentExposure - (Time.deltaTime/fadeExposureTransition), baseExposure, maxExposure);
			backgroundMaterial.SetFloat("_Exposure", currentExposure);
		}
	}

	void setNextIndex()
	{
		currentIndex = nextIndex;
		nextIndex++;
		if(nextIndex >= colorArray.Length) nextIndex = 0;
		timeSpent = 0f;
	}

	public void bumpExposure()
	{
		currentExposure = maxExposure;
		backgroundMaterial.SetFloat("_Exposure", currentExposure);
	}
}
