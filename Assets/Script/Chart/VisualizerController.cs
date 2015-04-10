using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class VisualizerController : MonoBehaviour {

	public AudioSource source;

	public GameObject cubeModel;
	Transform[] leftVisualizer;
	Transform[] rightVisualizer;

	private float[] spectrumDatas;
	public float[] visualizerBarHeights;
	public float heightLimit = 3.5f;

	public float xSpacing = 0.5f;
	public int bandNumber = 8;
	public int spectrumPrec = 8;
	public int bandIngore = 8;

	private Vector3 flatY = new Vector3(1f, 0f, 1f);


	// Use this for initialization
	void Start () {

		//Adjust in case of not 16:9
		float ratio = (float)Screen.width / (float)Screen.height;
		float startPositionBaseRatio = Mathf.Abs(cubeModel.transform.position.x / (16f/9f));
		float startPositionCurrentRatio = startPositionBaseRatio*ratio;

		leftVisualizer = new Transform[bandNumber];
		rightVisualizer = new Transform[bandNumber];
		spectrumDatas = new float[spectrumPrec];

		//Adjustment
		Vector3 basePositionLeft = new Vector3(-startPositionCurrentRatio, cubeModel.transform.position.y, cubeModel.transform.position.z);
		Vector3 basePositionRight = new Vector3(startPositionCurrentRatio, cubeModel.transform.position.y, cubeModel.transform.position.z);

		//Inst visualizer bar
		for(int i=0; i<bandNumber; i++)
		{
			GameObject bandLeft = Instantiate(cubeModel, basePositionLeft + (Vector3.right*xSpacing*i), cubeModel.transform.rotation) as GameObject;
			bandLeft.transform.SetParent(cubeModel.transform.parent);
			bandLeft.transform.localScale = flatY;
			leftVisualizer[i] = bandLeft.transform;

			GameObject bandRight = Instantiate(cubeModel, basePositionRight - (Vector3.right*xSpacing*i), cubeModel.transform.rotation) as GameObject;
			bandRight.transform.SetParent(cubeModel.transform.parent);
			bandRight.transform.localScale = flatY;
			rightVisualizer[i] = bandRight.transform;
		}

		cubeModel.SetActive(false);

		SoundWaveManager.instance.init(source);
		SoundWaveManager.instance.activeAnalysis(true);

	}
	
	// Update is called once per frame
	private float smoothFramerate;
	void Update () {

		SoundWaveManager.instance.getSpectrumBand(ref spectrumDatas, visualizerBarHeights, SpectrumCut.EXP);

		smoothFramerate = Mathf.Lerp(0.2f, 0.9f, Mathf.Clamp(Time.deltaTime / 0.033f, 0f, 1f));
		for(int i=0; i<bandNumber; i++)
		{
			leftVisualizer[i].localScale = Vector3.Lerp(leftVisualizer[i].localScale, flatY + Vector3.up*Mathf.Clamp(spectrumDatas[i+bandIngore], 0f, heightLimit), smoothFramerate);
			rightVisualizer[i].localScale = Vector3.Lerp(rightVisualizer[i].localScale, flatY + Vector3.up*Mathf.Clamp(spectrumDatas[i+bandIngore], 0f, heightLimit), smoothFramerate);
		}

	}

	public float getCurrentSpectrum(int index)
	{
		if(index < 0 || index >= leftVisualizer.Length) return 0f;
		return leftVisualizer [index].localScale.y / heightLimit;
	}
}
