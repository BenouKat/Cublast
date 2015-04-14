using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class LifeController : MonoBehaviour {

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

	public Text uiLife;

	// Use this for initialization
	void Start () {
		currentHP = currentRealHP = startHP;
		lifeSeparator = maxHP / (float)lifeBars.Length;
		processLifebar();
	}
	
	// Update is called once per frame
	void Update () {
		if (!Mathf.Approximately (currentRealHP, currentHP)) {
			currentHP = Mathf.SmoothDamp(currentHP, currentRealHP, ref velocityRef, 0.1f);
			//uiLife.text = ((int)(currentHP + 0.5f)).ToString("00");

			processLifebar();
		}


	}

	public void processLifebar()
	{
		for(int i=0; i<lifeBars.Length; i++)
		{
			if(i*lifeSeparator > currentHP)
			{
				lifeBars[i].localScale = Vector3.one * 0.001f;
			}else if(i*lifeSeparator <= currentHP && (i+1*lifeSeparator) > currentHP)
			{
				if((i+1*lifeSeparator) > currentHP)
				{
					lifeBars[i].localScale = Vector3.right + Vector3.forward + 
						Vector3.up*((currentHP - (i*lifeSeparator)) / lifeSeparator);
				}else{
					lifeBars[i].localScale = Vector3.one;
				}
			}
		}
	}

	public void addHPbyPrecision(Precision prec)
	{
		addHP ((float)GameManager.instance.LifeWeightValues [(int)prec]);
	}

	public void addHP(float HP)
	{
		currentRealHP = Mathf.Clamp (0f, maxHP, currentRealHP + HP);

		if (!isFullLife && Mathf.Approximately (currentRealHP, maxHP)) 
		{
			isFullLife = true;
		}else if(isFullLife && !Mathf.Approximately (currentRealHP, maxHP))
		{
			isFullLife = false;
		}
	}
}
