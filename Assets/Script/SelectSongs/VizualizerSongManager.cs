using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class VizualizerSongManager : MonoBehaviour {

	public Transform rootVisualizer;
	public GameObject visualizerModel;
	
	public int numberOfBar;
	public float diameter;
	public Vector3 axisRotation;

	Transform[] visualizerBar;
	float[] spectrumDatas;
	float[] visualizerBarHeights;

	public float heightLimit = 4f;

	public int bandNumber;
	public int spectrumPrec;
	public int bandIngore;

	Vector3 flatY = new Vector3(1f, 0f, 1f);

	// Use this for initialization
	void Start () {
		GameObject rotatorTemp = new GameObject ("rotator");
		rotatorTemp.transform.position = rootVisualizer.position;
		rotatorTemp.transform.rotation = rootVisualizer.rotation;
		float rotationPerCubes = 360f / numberOfBar;

		visualizerBar = new Transform[numberOfBar];
		visualizerBarHeights = new float[bandNumber];

		for(int i=0; i<numberOfBar; i++)
		{
			GameObject barInst = Instantiate(visualizerModel, rotatorTemp.transform.position, rotatorTemp.transform.rotation) as GameObject;
			barInst.transform.Translate(rotatorTemp.transform.up*diameter/2f, Space.World);
			barInst.transform.SetParent(rootVisualizer);
			barInst.SetActive(true);

			visualizerBar[i] = barInst.transform;
			if(i < bandNumber) visualizerBarHeights[i] = heightLimit*Mathf.Lerp(1f, 4f, (float)i/(float)bandNumber);

			rotatorTemp.transform.Rotate(axisRotation*rotationPerCubes, Space.Self);
		}
		
		Destroy (rotatorTemp);


		//Visualizer part
		spectrumDatas = new float[spectrumPrec];

		SoundWaveManager.instance.init (null);
		SoundWaveManager.instance.activeAnalysis (true);
	}

	// Update is called once per frame
	private float smoothFramerate;
	void Update () {
		
		SoundWaveManager.instance.getSpectrumBand(ref spectrumDatas, visualizerBarHeights, SpectrumCut.EXP);
		
		smoothFramerate = Mathf.Lerp(0.2f, 0.9f, Mathf.Clamp(Time.deltaTime / 0.033f, 0f, 1f));
		for(int i=0; i<bandNumber; i++)
		{
			visualizerBar[i].localScale = Vector3.Lerp(visualizerBar[i].localScale, flatY + Vector3.up*Mathf.Clamp(spectrumDatas[i+bandIngore], 0f, heightLimit), smoothFramerate);
			visualizerBar[i+(bandNumber*2)].localScale = Vector3.Lerp(visualizerBar[i+(bandNumber*2)].localScale, flatY + Vector3.up*Mathf.Clamp(spectrumDatas[i+bandIngore], 0f, heightLimit), smoothFramerate);
		}

		for(int i=bandNumber - 1; i>=0; i--)
		{
			visualizerBar[((bandNumber*2) - 1)-i].localScale = Vector3.Lerp(visualizerBar[((bandNumber*2) - 1)-i].localScale, flatY + Vector3.up*Mathf.Clamp(spectrumDatas[i+bandIngore], 0f, heightLimit), smoothFramerate);
			visualizerBar[((bandNumber*4) - 1)-i].localScale = Vector3.Lerp(visualizerBar[((bandNumber*4) - 1)-i].localScale, flatY + Vector3.up*Mathf.Clamp(spectrumDatas[i+bandIngore], 0f, heightLimit), smoothFramerate);
		}
		
	}
}
