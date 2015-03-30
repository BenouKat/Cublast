using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class InputController : MonoBehaviour {

	Dictionary<Lanes, KeyCode> primaryInputs = new Dictionary<Lanes, KeyCode>();
	Dictionary<Lanes, KeyCode> secondaryInputs = new Dictionary<Lanes, KeyCode>();

	// Use this for initialization
	void Start () {
		primaryInputs.Add(Lanes.DOWN, GameManager.instance.prefs.KeyCodeDown);
		primaryInputs.Add(Lanes.UP, GameManager.instance.prefs.KeyCodeUp);
		primaryInputs.Add(Lanes.RIGHT, GameManager.instance.prefs.KeyCodeRight);
		primaryInputs.Add(Lanes.LEFT, GameManager.instance.prefs.KeyCodeLeft);

		secondaryInputs.Add(Lanes.DOWN, GameManager.instance.prefs.SecondaryKeyCodeDown);
		secondaryInputs.Add(Lanes.UP, GameManager.instance.prefs.SecondaryKeyCodeUp);
		secondaryInputs.Add(Lanes.RIGHT, GameManager.instance.prefs.SecondaryKeyCodeRight);
		secondaryInputs.Add(Lanes.LEFT, GameManager.instance.prefs.SecondaryKeyCodeLeft);
	}
	
	// Update is called once per frame
	void Update () {
		for(int i=0;i<4;i++)
		{
			if(Input.GetKeyDown(primaryInputs[(Lanes)i]) || Input.GetKeyDown(secondaryInputs[(Lanes)i]))
			{
				ChartManager.instance.hitLane((Lanes)i);
			}

			if(Input.GetKey(primaryInputs[(Lanes)i]) || Input.GetKey(secondaryInputs[(Lanes)i]))
			{
				ChartManager.instance.holdLane((Lanes)i);
			}

			if(Input.GetKeyUp(primaryInputs[(Lanes)i]) || Input.GetKeyUp(secondaryInputs[(Lanes)i]))
			{
				ChartManager.instance.releaseLane((Lanes)i);
			}
		}
	}
}
