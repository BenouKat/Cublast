using UnityEngine;
using System.Collections;

public class MainMenuConnectedAnim : MonoBehaviour {

	public GameObject flareAnim;

	public GameObject visualizerRoot;
	public GameObject diversLight;
	public GameObject cubeGenerator;

	public AudioSource connectionMusic;
	public GameObject introSound;
	public AudioSource mainMusic;

	public Transform[] cubesVisualizer;
	public Light[] lightVisualizer;


	
	private float[] spectrumDatas;
	public float[] visualizerBarHeights;
	public float heightLimit = 3.5f;

	public float maxLightIntensity;
	
	public float xSpacing = 0.5f;
	public int bandNumber = 8;
	public int spectrumPrec = 8;
	public int bandIngore = 8;
	
	private Vector3 flatY = new Vector3(1f, 0f, 1f);
	
	// Use this for initialization
	void Start () {
		/*if (GameManager.instance.gameInitialized) {
			connectionMusic.gameObject.SetActive (false);
			visualizerRoot.SetActive (true);
			diversLight.SetActive (true);
			cubeGenerator.SetActive (false);
			mainMusic.gameObject.SetActive (true);
			SoundWaveManager.instance.init (mainMusic);
			SoundWaveManager.instance.activeAnalysis (true);
			flare.brightness = minFlare;
		} else {*/
			flareAnim.SetActive (true);
			StartCoroutine (animMusic ());
		//}

		spectrumDatas = new float[spectrumPrec];
	}

	IEnumerator animMusic()
	{
		introSound.SetActive (true);

		float timeSpent = 0f;
		while (timeSpent < 1f) {
			timeSpent += Time.deltaTime;
			connectionMusic.volume = Mathf.Lerp(1f, 0.5f, timeSpent);
			yield return 0;
		}

		connectionMusic.gameObject.SetActive (false);
		visualizerRoot.SetActive (true);
		diversLight.SetActive (true);
		cubeGenerator.SetActive (false);

		yield return new WaitForSeconds (1f);

		mainMusic.gameObject.SetActive (true);
		SoundWaveManager.instance.init (mainMusic);
		SoundWaveManager.instance.activeAnalysis (true);
	}
	
	// Update is called once per frame
	private float smoothFramerate;
	void Update () {

		if (SoundWaveManager.instance.isActiveAndEnabled) {
			SoundWaveManager.instance.getSpectrumBand(ref spectrumDatas, visualizerBarHeights, SpectrumCut.EXP);
			
			smoothFramerate = Mathf.Lerp(0.2f, 0.9f, Mathf.Clamp(Time.deltaTime / 0.033f, 0f, 1f));
			for(int i=0; i<bandNumber; i++)
			{
				cubesVisualizer[i].localScale = Vector3.Lerp(cubesVisualizer[i].localScale, flatY + Vector3.up*Mathf.Clamp(spectrumDatas[i+bandIngore], 0f, heightLimit), smoothFramerate);
				lightVisualizer[i].intensity = (cubesVisualizer[i].localScale.y / heightLimit)*maxLightIntensity;
			}
		}
	}
}
