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

	//Display variable
	public Transform[] lifeBars;
	public Material lifeMaterial;
	public Color[] colors;
	public Color finalColor;
	bool isFullLife;

	float lifeSeparator;
	float colorSeparator;

	// Use this for initialization
	void Start () {
		currentHP = currentRealHP = startHP;
		lifeSeparator = maxHP / (float)lifeBars.Length;
		colorSeparator = maxHP / (float)colors.Length;
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

	public IEnumerator colorToMax(float timeLerp)
	{
		float timeSpent = 0f;
		while (timeSpent < timeLerp && isFullLife) {
			timeSpent += Time.deltaTime;
			lifeMaterial.SetColor("_EmissionColor", Color.Lerp(colors[colors.Length - 1] , finalColor, timeSpent/timeLerp));
			yield return 0;
		}
	}

	public void addHPbyPrecision(Precision prec)
	{
		addHP ((float)GameManager.instance.LifeWeightValues [(int)prec]);
	}

	public void addHP(float HP)
	{
		currentRealHP = Mathf.Clamp (currentRealHP + HP, 0f, maxHP);

		if (!isFullLife && Mathf.Approximately (currentRealHP, maxHP)) 
		{
			isFullLife = true;
			StartCoroutine(colorToMax(0.5f));
		}else if(isFullLife && !Mathf.Approximately (currentRealHP, maxHP))
		{
			isFullLife = false;
		}
	}
}
