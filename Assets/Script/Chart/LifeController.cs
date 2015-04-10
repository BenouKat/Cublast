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
	public Transform center;
	public Transform[] lifeBars;
	public Material lifeMaterial;
	public Color[] colors;
	public Color finalColor;
	bool isFullLife;

	public VisualizerController visualizer;
	float lifeSeparator;

	public Text uiLife;

	// Use this for initialization
	void Start () {
		currentHP = currentRealHP = startHP;
		lifeSeparator = maxHP / (float)lifeBars.Length;
	}
	
	// Update is called once per frame
	void Update () {
		if (!Mathf.Approximately (currentRealHP, currentHP)) {
			currentHP = Mathf.SmoothDamp(currentHP, currentRealHP, velocityRef, 0.1f);
			uiLife.text = ((int)(currentHP + 0.5f)).ToString("00");

			for(int i=0; i<lifeBars; i++)
			{
				if(i*lifeSeparator > currentHP)
				{
					lifeBars[i].localScale = Vector3.right + Vector3.up;
				}else if(i*lifeSeparator <= currentHP && (i+1*lifeSeparator) > currentHP)
				{
					if((i+1*lifeSeparator) > currentHP)
					{
						lifeBars[i].localScale = Vector3.right + Vector3.up + 
							Vector3.forward*((currentHP - (i*lifeSeparator)) / lifeSeparator);
					}else{
						lifeBars[i].localScale = Vector3.one;
					}
				}
			}
		}


	}

	public void addHPbyPrecision(Precision prec)
	{
		addHP (GameManager.instance.LifeWeightValues [prec]);
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
