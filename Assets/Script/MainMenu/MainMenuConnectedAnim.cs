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

	public Animation gamePanelAnim;

	public UnityStandardAssets.ImageEffects.ScreenSpaceAmbientOcclusion SSAO;

	public Color[] availableCubeColor;
	public Color[] availableLightColor;

	public Light[] coloredLight;
	public Material cubeMaterial;

	public Transform[] cubesVisualizer;
	public Light lightVisualizer;
	
	private float[] spectrumDatas;
	public float[] visualizerBarHeights;
	public float heightLimit = 3.5f;

	public float minLightIntensity;
	public float maxLightIntensity;

	public int bandNumber = 8;
	public int spectrumPrec = 8;
	public int bandIngore = 8;
	
	private Vector3 flatY = new Vector3(1f, 0f, 1f);
	
	// Use this for initialization
	void Start () {

		flareAnim.SetActive (true);

		if (GameManager.instance.gameInitialized) {
			connectionMusic.gameObject.SetActive (false);
			visualizerRoot.SetActive (true);
			diversLight.SetActive (true);
			cubeGenerator.SetActive (false);
			mainMusic.gameObject.SetActive (true);
			SoundWaveManager.instance.init (mainMusic);
			SoundWaveManager.instance.activeAnalysis (true);
			gamePanelAnim.gameObject.SetActive(true);
			gamePanelAnim.GetComponent<CanvasGroup>().alpha = 1f;
			SSAO.enabled = true;
			flareAnim.GetComponent<Animation>().enabled = false;
			flareAnim.GetComponent<LensFlare>().brightness = 0.3f;
		} else {
			StartCoroutine (animMusic ());
			GameManager.instance.gameInitialized = true;
		}
		
		spectrumDatas = new float[spectrumPrec];

		int randomColor = Random.Range(0, availableCubeColor.Length);
		cubeMaterial.SetColor("_EmissionColor", availableCubeColor[randomColor]);
		foreach(Light l in coloredLight)
		{
			l.color = availableLightColor[randomColor];
		}
		flareAnim.GetComponent<LensFlare>().color = availableLightColor[randomColor];
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
		gamePanelAnim.gameObject.SetActive(true);


		yield return new WaitForSeconds (0.7f);

		gamePanelAnim.Play();
		SSAO.enabled = true;

		yield return new WaitForSeconds(0.3f);

		mainMusic.gameObject.SetActive (true);
		SoundWaveManager.instance.init (mainMusic);
		SoundWaveManager.instance.activeAnalysis (true);
	}
	
	// Update is called once per frame
	private float smoothFramerate;

	void Update () {

		if (SoundWaveManager.instance.isAnalyseActive()) {
			SoundWaveManager.instance.getSpectrumBand(ref spectrumDatas, visualizerBarHeights, SpectrumCut.EXP);
			
			smoothFramerate = Mathf.Lerp(0.05f, 0.9f, Mathf.Clamp(Time.deltaTime / 0.033f, 0f, 1f));
			for(int i=0; i<bandNumber; i++)
			{
				cubesVisualizer[i].localScale = Vector3.Lerp(cubesVisualizer[i].localScale, flatY + Vector3.up*Mathf.Clamp(spectrumDatas[i+bandIngore], 0f, heightLimit), smoothFramerate);
				if(i==(bandNumber/2)) lightVisualizer.intensity = ((cubesVisualizer[i].localScale.y / heightLimit)*(maxLightIntensity - minLightIntensity)) + minLightIntensity;
			}
		}
	}
}
