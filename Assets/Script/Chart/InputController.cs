using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class InputController : MonoBehaviour {

	KeyCode[] primaryInputs;
	KeyCode[] secondaryInputs;

	// Use this for initialization
	void Start () {

		primaryInputs = new KeyCode[System.Enum.GetValues(typeof(Lanes)).Length];
		secondaryInputs = new KeyCode[System.Enum.GetValues(typeof(Lanes)).Length];
		for(int i=0; i<primaryInputs.Length; i++)
		{
			switch((Lanes)i)
			{
			case Lanes.DOWN:
				primaryInputs[i] = GameManager.instance.prefs.KeyCodeDown;
				secondaryInputs[i] = GameManager.instance.prefs.SecondaryKeyCodeDown;
				break;
			case Lanes.UP:
				primaryInputs[i] = GameManager.instance.prefs.KeyCodeUp;
				secondaryInputs[i] = GameManager.instance.prefs.SecondaryKeyCodeUp;
				break;
			case Lanes.RIGHT:
				primaryInputs[i] = GameManager.instance.prefs.KeyCodeRight;
				secondaryInputs[i] = GameManager.instance.prefs.SecondaryKeyCodeRight;
				break;
			case Lanes.LEFT:
				primaryInputs[i] = GameManager.instance.prefs.KeyCodeLeft;
				secondaryInputs[i] = GameManager.instance.prefs.SecondaryKeyCodeLeft;
				break;
			}
		}
	}
	
	// Update is called once per frame
	void Update () {
		for(int i=0;i<4;i++)
		{
			if(Input.GetKeyDown(primaryInputs[i]) || Input.GetKeyDown(secondaryInputs[i]))
			{
				ChartManager.instance.hitLane((Lanes)i);
			}

			if(Input.GetKey(primaryInputs[i]) || Input.GetKey(secondaryInputs[i]))
			{
				ChartManager.instance.holdLane((Lanes)i);
			}

			if(Input.GetKeyUp(primaryInputs[i]) || Input.GetKeyUp(secondaryInputs[i]))
			{
				ChartManager.instance.releaseLane((Lanes)i);
			}
		}
	}
}
