﻿using UnityEngine;
using System.Collections;


public delegate void BackButtonClickedHandler();

public class Events : MonoBehaviour {

	public static Events instance;
	void Awake()
	{
		if(instance == null) instance = this;
		DontDestroyOnLoad (this);
	}


	public event BackButtonClickedHandler BackButtonClicked;
	public void FireBackButtonClicked()
	{
		if(BackButtonClicked != null) BackButtonClicked();
	}
}