﻿using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ComboController : MonoBehaviour {


	public static ComboController instance;
	void Awake()
	{
		if(instance == null) instance = this;
	}

	ComboType currentComboType;
	ComboType previousComboType;

	int combo;

	public int comboSimpleCap;
	public int comboGreatCap;

	public ParticleSystem[] particlesCombo;
	public ParticleSystem[] particlesNoneCombo;
	public ParticleSystem explosion;
	public LensFlareTweener explosionFlare;

	public Color[] materialColorCombo;
	public Color[] materialColorNoneCombo;
	public Color[] textColorCombo;
	public Color[] textColorNoneCombo;

	ParticleSystem currentlyPlaying;
	public Material materialCombo;
	public float transitionColorSpeed;



	//UI
	public Text comboText;
	public Outline comboOutline;
	public float maxAlpha;
	public float speedAlphaDiminution;

	// Use this for initialization
	void Start () {
		previousComboType = currentComboType = ComboType.FULLFANTASTIC;
		combo = 0;
		playParticle (particlesNoneCombo [(int)NoneComboType.NORMAL]);
		materialCombo.SetColor ("_EmissionColor", materialColorNoneCombo [(int)NoneComboType.NORMAL]);
		comboOutline.effectColor = textColorNoneCombo [(int)NoneComboType.NORMAL];
		writeCombo (false);
	}

	Color tempTextColor = new Color();
	void Update()
	{
		if (comboText.color.a > maxAlpha) {
			tempTextColor = comboText.color;
			tempTextColor.a = Mathf.Clamp(tempTextColor.a - (Time.deltaTime/speedAlphaDiminution), maxAlpha, 1f);
			comboText.color = tempTextColor;
		}
	}
	

	public void addCombo(Precision precision)
	{
		previousComboType = currentComboType;
		combo++;
		switch (currentComboType) {
		case ComboType.FULLFANTASTIC:
			if(precision == Precision.FANTASTIC) break;
			switch(precision)
			{
			case Precision.EXCELLENT:
				currentComboType = ComboType.FULLEXCELLENT;
				break;
			case Precision.GREAT:
				currentComboType = ComboType.FULLCOMBO;
				break;
			default:
				currentComboType = ComboType.NONE;
				break;
			}
			break;
		case ComboType.FULLEXCELLENT:
			if(precision <= Precision.EXCELLENT) break;
			if(precision == Precision.GREAT)
			{
				currentComboType = ComboType.FULLCOMBO;
			}else{
				currentComboType = ComboType.NONE;
			}
			break;
		case ComboType.FULLCOMBO:
			if(precision <= Precision.GREAT) break;
			currentComboType = ComboType.NONE;
			break;
		default:
			break;
		}

		processComboType ();
		writeCombo ();
	}

	public void breakCombo()
	{
		previousComboType = currentComboType;

		if (combo >= comboSimpleCap) {
			explosion.Play();
			explosionFlare.enableTween();
		}

		combo = 0;
		currentComboType = ComboType.NONE;

		processComboType ();
		writeCombo (false);
	}

	void processComboType()
	{
		if (combo == 0) {
			playParticle (particlesNoneCombo [(int)NoneComboType.MISSED]);
			comboOutline.effectColor = textColorNoneCombo[(int)NoneComboType.MISSED];
			changeComboMaterialColor(materialColorNoneCombo[(int)NoneComboType.MISSED]);
			ChartManager.instance.modelLane.activeAllComboParticles(false);
			return;
		}

		if (currentComboType == ComboType.NONE && combo == comboSimpleCap) {
			playParticle (particlesNoneCombo [(int)NoneComboType.NORMAL]);
			comboOutline.effectColor = textColorNoneCombo[(int)NoneComboType.NORMAL];
			changeComboMaterialColor(materialColorNoneCombo[(int)NoneComboType.NORMAL]);

		} else if (combo == comboGreatCap || (previousComboType != currentComboType && combo >= comboGreatCap)) {
			if(currentComboType == ComboType.NONE)
			{
				playParticle (particlesNoneCombo [(int)NoneComboType.GREAT]);
				comboOutline.effectColor = textColorNoneCombo[(int)NoneComboType.GREAT];
				changeComboMaterialColor(materialColorNoneCombo[(int)NoneComboType.GREAT]);
			}else{
				playParticle (particlesCombo [(int)currentComboType]);
				comboOutline.effectColor = textColorCombo[(int)currentComboType];
				changeComboMaterialColor(materialColorCombo[(int)currentComboType]);
			}
			ChartManager.instance.modelLane.activeAllComboParticles(true);
		}
	}

	void writeCombo(bool withAnim = true)
	{
		tempTextColor = comboText.color;
		tempTextColor.a = withAnim ? 1f : maxAlpha;
		comboText.color = tempTextColor;
		comboText.text = combo.ToString ("0");
	}

	void changeComboMaterialColor(Color colorToGo)
	{
		if (processingChangeColor)
			StopCoroutine ("changeComboMaterialColorRoutine");

		processingChangeColor = false;
		colorProcessed = colorToGo;
		StartCoroutine ("changeComboMaterialColorRoutine");
	}

	Color colorProcessed;
	bool processingChangeColor = false;
	IEnumerator changeComboMaterialColorRoutine()
	{
		processingChangeColor = true;
		float timeSpent = 0f;
		Color currentMaterialColor = materialCombo.GetColor ("_EmissionColor");
		while (timeSpent < transitionColorSpeed) {
			timeSpent += Time.deltaTime;
			materialCombo.SetColor("_EmissionColor", Color.Lerp(currentMaterialColor, colorProcessed, timeSpent/transitionColorSpeed));
			yield return 0;
		}
		processingChangeColor = false;
	}


	void playParticle(ParticleSystem ps)
	{
		if (currentlyPlaying != null) currentlyPlaying.Stop ();
		ps.Play ();
		currentlyPlaying = ps;
	}

	public ComboType getCurrentComboType()
	{
		return currentComboType;
	}
}
