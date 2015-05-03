using UnityEngine;
using System.Collections;

public class MainMenuCubeGeneration : MonoBehaviour {

	public float timeMin;
	public float timeMax;

	public Color[] rangeColor;

	public GameObject modelCube;
	float timeLastCube;
	float timeNextCube;
	int selectedIndex;

	void Start()
	{
		reloadTime();
	}

	// Update is called once per frame
	void Update () {
		if(Time.time > timeLastCube + timeNextCube)
		{
			GameObject coloredCube = Instantiate(modelCube, modelCube.transform.position, modelCube.transform.rotation) as GameObject;
			coloredCube.GetComponent<Renderer>().material.SetColor("_EmissionColor", rangeColor[selectedIndex]);
			coloredCube.transform.GetChild(0).GetComponent<Light>().color = rangeColor[selectedIndex];
			coloredCube.transform.SetParent(transform);
			coloredCube.SetActive(true);
			reloadTime();
		}
	}

	public void reloadTime()
	{
		timeLastCube = Time.time;
		timeNextCube = Random.Range(timeMin, timeMax);
		selectedIndex = Random.Range(0, rangeColor.Length);
	}
			                                                      
}
