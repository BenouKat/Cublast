using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Arrow : MonoBehaviour {

	public ArrowState state = ArrowState.NONE;
	public double dateValidation = 0;
	public List<Arrow> linkedArrows;
	public ArrowType type = ArrowType.NORMAL;
	public Lanes currentLane;
	public bool attached; //Attached for freeze
	//This tag indicate if an arrow is already "too late" (decent +)
	//If it's the case and if the next arrow is valid, 
	public bool tagAsMissed;

	//Time related
	public double scheduledTime;
	public Precision precisionValid = Precision.NONE;

	//Object related
	public MeshRenderer coloredObject;

	public FreezeController freezeController;
	public FreezeController rollController;


	public Precision checkAndProcessValidateArrow(double currentTime)
	{
		if (state != ArrowState.NONE) return Precision.NONE;
		if (currentTime + GameManager.instance.PrecisionValues [Precision.WAYOFF] < scheduledTime)
			return Precision.NONE;
		state = ArrowState.WAITINGLINKED;
		dateValidation = currentTime;
		if (linkedArrows.Count != 0 && linkedArrows.Exists (c => c.state != ArrowState.WAITINGLINKED)) {
			return Precision.NONE;
		}

		precisionValid = Utils.getPrec ((double)Mathf.Abs ((float)(scheduledTime - dateValidation)));
		state = precisionValid <= Precision.GREAT ? ArrowState.VALIDATED : ArrowState.MISSED;
		foreach (Arrow linkArrow in linkedArrows) { 
			linkArrow.state = state; 
			linkArrow.precisionValid = precisionValid;
		}
		//Debug.Log (precisionValid.ToString() + " // " + Mathf.Abs ((float)(scheduledTime - dateValidation)).ToString("0.0000"));
		return precisionValid;
	}

	public bool checkAndProcessValidateMine(double currentTime)
	{
		if (state != ArrowState.NONE) return false; //Already treated
		if ((currentTime - scheduledTime) >= 0) {
			state = ArrowState.VALIDATED;
			dateValidation = currentTime;
			return true;
		}
		return false;
	}

	public Precision getArrowPrec(double currentTime)
	{
		return Utils.getPrec((double)Mathf.Abs((float)(scheduledTime - currentTime)));
	}

	public void tryTagAsMissed(double currentTime)
	{
		if (state != ArrowState.VALIDATED && !tagAsMissed 
		    && currentTime > scheduledTime &&
		    getArrowPrec (currentTime) > Precision.GREAT) {
			tagAsMissed = true;
		}
	}

	public bool checkMissFreeze(double currentTime)
	{
		return state == ArrowState.VALIDATED && (currentTime - getFreezeController (type).timeLastHit) > GameManager.instance.timeBeforeFreezeMiss;
	}

	public bool checkTimeEndFreeze(double currentTime)
	{
		return state == ArrowState.VALIDATED && currentTime >= getFreezeController (type).timeEndScheduled;
	}

	public void computeFreezePosition(double currentTime)
	{
		if (state == ArrowState.VALIDATED) {
			getFreezeController (type).animFreeze (currentTime);
		}
	}

	public bool checkAndProcessMissArrow(double currentTime)
	{
		if (state != ArrowState.NONE && state != ArrowState.WAITINGLINKED)
			return false; //Already treated

		if ((currentTime - scheduledTime) > GameManager.instance.PrecisionValues [Precision.WAYOFF]) {
			state = ArrowState.MISSED;
			if (linkedArrows.Count != 0) {
				
				foreach (Arrow linkArrow in linkedArrows) {
					linkArrow.state = ArrowState.MISSED;
				}
			}
			return true;
		}
		return false;
	}

	public bool checkAndProcessMissMine(double currentTime)
	{
		if (state != ArrowState.NONE)
			return false; //Already treated

		if ((currentTime - scheduledTime) > 0) {
			state = ArrowState.MISSED;
			return true;
		}
		return false;
	}

	public FreezeController getFreezeController(ArrowType type)
	{
		switch(type)
		{
		case ArrowType.FREEZE:
			return freezeController;
		case ArrowType.ROLL:
			return rollController;
		default:
			return null;
		}
	}
}
