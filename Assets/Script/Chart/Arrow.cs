using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Arrow : MonoBehaviour {

	public ArrowState state;
	public double dateValidation = 0;
	public List<Arrow> linkedArrows;

	public bool isMine;

	//Time related
	public double scheduledTime;


	public Precision validateArrow(double currentTime)
	{
		if (state != ArrowState.NONE) return;
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
		return Utils.getPrec(Mathf.Abs(selected.scheduledTime - selected.dateValidation));
	}
}
