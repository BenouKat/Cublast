using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SongOptionManager : MonoBehaviour {

	public static SongOptionManager instance;
	
	void Awake()
	{
		if(instance == null){ 
			instance = this;
		}
		DontDestroyOnLoad (this);
	}


	public List<OptionsMod> currentOptions;
	public double speedmodSelected;
	public double rateSelected;
}
