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


	public List<OptionsMod> currentOptions = new List<OptionsMod>();
	public double speedmodSelected = 1f;
	public double rateSelected;
	public int skinSelected = 0;
}
