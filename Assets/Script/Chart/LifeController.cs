using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class LifeController : MonoBehaviour {


	public static LifeController instance;

	void Awake()
	{
		if (instance == null)
			instance = this;
	}



	//HP datas
	public float startHP;
	public float maxHP;
	public float alertHP;

	float currentRealHP;
	float currentHP;
	float velocityRef;

	//Components
	public RotateTick rotateTick;
	public ParticleSystem fullLifeParticle;
	public ParticleSystem dangerParticle;
	public ParticleSystem deadParticle;
	public LensFlareTweener lensFlareDead;

	//Display variable
	public Transform[] lifeBars;
	public Transform heartLifebar;
	public Transform centerLifeBars;

	public Material lifeMaterial;
	public Color[] colors;
	public Color finalColor;

	//Bools
	bool isFullLife;
	bool isInDanger;

	Rotate[] lifeBarsCubes;
	Vector3[] lifeInitialPosition;
	Vector3[] lifeFullLifePosition;
	Vector3 initialCenterScale;
	float lifeSeparator;
	float colorSeparator;

	// Use this for initialization
	void Start () {
		currentHP = currentRealHP = startHP;
		lifeSeparator = maxHP / (float)lifeBars.Length;
		colorSeparator = maxHP / (float)colors.Length;

		lifeBarsCubes = new Rotate[lifeBars.Length];
		lifeInitialPosition = new Vector3[lifeBars.Length];
		lifeFullLifePosition = new Vector3[lifeBars.Length];
		initialCenterScale = centerLifeBars.localScale;

		for (int i=0; i<lifeBars.Length; i++) {
			lifeBarsCubes[i] = lifeBars[i].GetChild(0).GetComponent<Rotate>();
			lifeInitialPosition[i] = lifeBars[i].localPosition;
		}

		processLifebar();
	}
	
	// Update is called once per frame
	void Update () {
		if (!Mathf.Approximately (currentRealHP, currentHP)) {
			currentHP = Mathf.SmoothDamp(currentHP, currentRealHP, ref velocityRef, 0.1f);

			processLifebar();
		}


	}

	private int indexColor;
	private Color tempColor;
	public void processLifebar()
	{

		//Debug.Log (currentHP);

		for(int i=0; i<lifeBars.Length; i++)
		{
			if(i*lifeSeparator > currentHP)
			{
				lifeBars[i].localScale = Vector3.one * 0.001f;
			}else if(i*lifeSeparator <= currentHP && ((i+1)*lifeSeparator) > currentHP)
			{
				if(i*lifeSeparator == currentHP)
				{
					lifeBars[i].localScale = Vector3.one * 0.001f;
				}else if(((i+1)*lifeSeparator) > currentHP)
				{
					lifeBars[i].localScale = Vector3.right + Vector3.forward + 
						Vector3.up*((currentHP - (i*lifeSeparator)) / lifeSeparator);
				}else{
					lifeBars[i].localScale = Vector3.one;
				}
			}
		}

		if(currentHP <= 0.1f) lifeBars[0].localScale = Vector3.one * 0.001f;

		if (!isFullLife) {
			indexColor = Mathf.Clamp((int)(currentHP/colorSeparator), 0, colors.Length - 2);
			tempColor = Color.Lerp (colors [indexColor], colors [indexColor + 1], (currentHP - (indexColor * colorSeparator)) / colorSeparator);
			
			lifeMaterial.SetColor("_EmissionColor", tempColor);
		}
	}

	//Actions
	public void addHPbyPrecision(Precision prec)
	{
		addHP ((float)GameManager.instance.LifeWeightValues [(int)prec]);
	}
	
	public void addHP(float HP)
	{
		currentRealHP = Mathf.Clamp (currentRealHP + HP, 0f, maxHP);

		if (currentRealHP <= 0f) ChartManager.instance.callGameOver ();
		
		if (!isFullLife && Mathf.Approximately (currentRealHP, maxHP)) 
		{
			isFullLife = true;
			StartCoroutine(colorToMax(0.5f));
			StartCoroutine(fullLifeCubeAnim(0.2f, 0.2f));
			fullLifeParticle.Play();
		}else if(isFullLife && !Mathf.Approximately (currentRealHP, maxHP))
		{
			isFullLife = false;
			StartCoroutine(notFullLifeAnim(0.1f, 0.1f));
			fullLifeParticle.Stop();
		}

		if (!isInDanger && currentRealHP <= alertHP) 
		{
			isInDanger = true;
			dangerParticle.Play();
		}else if(isInDanger && currentRealHP > alertHP)
		{
			isInDanger = false;
			dangerParticle.Stop();
		}
	}


	//Animations
	IEnumerator colorToMax(float timeLerp)
	{
		float timeSpent = 0f;
		while (timeSpent < timeLerp && isFullLife) {
			timeSpent += Time.deltaTime;
			lifeMaterial.SetColor("_EmissionColor", Color.Lerp(colors[colors.Length - 1] , finalColor, timeSpent/timeLerp));
			yield return 0;
		}
	}

	IEnumerator fullLifeCubeAnim(float timeLerp, float timeExplosion)
	{
		for (int i=0; i<lifeBars.Length; i++) {
			lifeFullLifePosition[i] = Random.onUnitSphere * 3.5f;
		}
		rotateTick.enabled = false;

		float timeSpent = 0f;
		while (timeSpent < timeLerp && isFullLife) {
			timeSpent += Time.deltaTime;
			centerLifeBars.localScale = Vector3.Lerp(initialCenterScale, initialCenterScale/2f, timeSpent/timeLerp);
			yield return 0;
		}

		for (int i=0; i<lifeBars.Length; i++) {
			lifeBarsCubes[i].enabled = true;
			lifeBarsCubes[i].vectorRotation = new Vector3(Random.Range(-10f, 10f), Random.Range(-10f, 10f), Random.Range(-10f, 10f));
			lifeBarsCubes[i].transform.Rotate(new Vector3(Random.Range(0f, 360f), Random.Range(0f, 360f), Random.Range(0f, 360f)));
		}

		timeSpent = 0f;
		while (timeSpent < timeExplosion && isFullLife) {
			timeSpent += Time.deltaTime;
			for (int i=0; i<lifeBars.Length; i++) {
				lifeBars[i].transform.localPosition = Vector3.Lerp(lifeInitialPosition[i], lifeFullLifePosition[i], timeSpent/timeExplosion);
			}
			yield return 0;
		}
	}

	IEnumerator notFullLifeAnim(float timeLerp, float timeUnlerp)
	{
		rotateTick.enabled = true;

		float timeSpent = 0f;
		while (timeSpent < timeLerp && !isFullLife) {
			timeSpent += Time.deltaTime;
			centerLifeBars.localScale = Vector3.Lerp(initialCenterScale/2f, initialCenterScale/5f, timeSpent/timeLerp);
			yield return 0;
		}

		for (int i=0; i<lifeBars.Length; i++) {
			lifeBarsCubes[i].enabled = false;
			lifeBarsCubes[i].transform.localRotation = Quaternion.identity;
			lifeBars[i].localPosition = lifeInitialPosition[i];
		}

		timeSpent = 0f;
		while (timeSpent < timeLerp && !isFullLife) {
			timeSpent += Time.deltaTime;
			centerLifeBars.localScale = Vector3.Lerp(initialCenterScale/5f, initialCenterScale, timeSpent/timeUnlerp);
			yield return 0;
		}
	}

	public void playDeath()
	{
		heartLifebar.gameObject.SetActive (false);
		centerLifeBars.gameObject.SetActive (false);

		dangerParticle.Stop();
		fullLifeParticle.Stop ();
		deadParticle.gameObject.SetActive (true);		
		deadParticle.Play ();
		lensFlareDead.enableTween ();
	}
}
