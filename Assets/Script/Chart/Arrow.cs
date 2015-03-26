using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Arrow : MonoBehaviour {

	[HideInInspector] public ArrowState state = ArrowState.NONE;
	[HideInInspector]public double dateValidation = 0;
	[HideInInspector]public List<Arrow> linkedArrows;
	[HideInInspector]public ArrowType type = ArrowType.NORMAL;

	//Time related
	[HideInInspector]public double scheduledTime;

	//Object related
	public MeshRenderer coloredObject;

	public FreezeController freezeController;
	public FreezeController rollController;


	public Precision validateArrow(double currentTime)
	{
		if (state != ArrowState.NONE) return Precision.NONE;
		state = ArrowState.VALIDATED;
		dateValidation = currentTime;
		if (linkedArrows.Count != 0 && linkedArrows.Exists(c => c.state == ArrowState.NONE)) {
			return Precision.NONE;
		}
		double maxDateValid = dateValidation;
		Arrow selected = this;
		if (linkedArrows.Count != 0) {

			foreach (Arrow linkArrow in linkedArrows) {
				if (linkArrow.dateValidation > maxDateValid) {
					selected = linkArrow;
				}
			}
		}
		return Utils.getPrec((double)Mathf.Abs((float)(selected.scheduledTime - selected.dateValidation)));
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
