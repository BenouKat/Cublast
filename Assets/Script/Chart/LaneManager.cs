﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public enum Lanes
{
	LEFT,
	DOWN,
	UP,
	RIGHT
}

public class LaneManager : MonoBehaviour {

	int numberOfLanes;

	public Transform left;
	public Transform right;
	public Transform up;
	public Transform down;

	public GameObject particleControllerModel;

	ParticleEffectController particleControllerLeft;
	ParticleEffectController particleControllerRight;
	ParticleEffectController particleControllerUp;
	ParticleEffectController particleControllerDown;
	
	List<Arrow> leftArrows = new List<Arrow>();
	List<Arrow> rightArrows = new List<Arrow>();
	List<Arrow> upArrows = new List<Arrow>();
	List<Arrow> downArrows = new List<Arrow>();
	List<Arrow> trashOfArrowMissed = new List<Arrow> ();

	Arrow[] leftArrowsArray;
	Arrow[] rightArrowsArray;
	Arrow[] upArrowsArray;
	Arrow[] downArrowsArray;

	Arrow nextLeft;
	Arrow nextRight;
	Arrow nextUp;
	Arrow nextDown;

	Arrow frozenLeft;
	Arrow frozenRight;
	Arrow frozenUp;
	Arrow frozenDown;

	int[] indexes;

	//Set the particles controller
	public bool setParticleControllersAtStart = false;


	//Once locked, arrow list will turn to arrowArray for performance, which increase the cost of some functions.
	public bool locked;

	void Awake()
	{
		numberOfLanes = System.Enum.GetValues (typeof(Lanes)).Length;
		indexes = new int[numberOfLanes];
		for(int i=0; i<numberOfLanes; i++){ indexes[i] = 0; };
	}

	// Use this for initialization
	void Start () {
		if (setParticleControllersAtStart) {
			setParticleSystemController();
		}
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public Transform getLane(Lanes lane)
	{
		switch(lane)
		{
		case Lanes.LEFT:
			return left;
		case Lanes.DOWN:
			return down;
		case Lanes.UP:
			return up;
		case Lanes.RIGHT:
			return right;				
		}
		return null;
	}

	public ParticleEffectController getParticleEffect(Lanes lane)
	{
		switch(lane)
		{
		case Lanes.LEFT:
			return particleControllerLeft;
		case Lanes.DOWN:
			return particleControllerDown;
		case Lanes.UP:
			return particleControllerUp;
		case Lanes.RIGHT:
			return particleControllerRight;				
		}
		return null;
	}

	public ParticleEffectController setParticleEffect(Lanes lane, ParticleEffectController pec)
	{
		switch(lane)
		{
		case Lanes.LEFT:
			particleControllerLeft = pec;
			break;
		case Lanes.DOWN:
			particleControllerDown = pec;
			break;
		case Lanes.UP:
			particleControllerUp = pec;
			break;
		case Lanes.RIGHT:
			particleControllerRight = pec;		
			break;
		}
		return null;
	}
	
	public List<Arrow> getLaneArrows(Lanes lane)
	{
		switch(lane)
		{
			case Lanes.LEFT:
				return locked ? leftArrowsArray.ToList() : leftArrows;
			case Lanes.DOWN:
				return locked ? downArrowsArray.ToList() : downArrows;
			case Lanes.UP:
				return locked ? upArrowsArray.ToList() : upArrows;
			case Lanes.RIGHT:
				return locked ? rightArrowsArray.ToList() : rightArrows;				
		}
		return null;
	}

	public Arrow[] getLaneArrowsArray(Lanes lane)
	{
		switch(lane)
		{
		case Lanes.LEFT:
			return leftArrowsArray;
		case Lanes.DOWN:
			return downArrowsArray;
		case Lanes.UP:
			return upArrowsArray;
		case Lanes.RIGHT:
			return rightArrowsArray;				
		}
		return null;
	}

	public void setParticleSystemController()
	{
		for(int i=0;i<numberOfLanes;i++)
		{
			GameObject pecInst = Instantiate(particleControllerModel, 
			                                 getLane((Lanes)i).position, 
			                                 particleControllerModel.transform.rotation) as GameObject;
			pecInst.transform.SetParent(getLane((Lanes)i));
			setParticleEffect((Lanes)i, pecInst.GetComponent<ParticleEffectController>());
		}
	}

	public void activeAllComboParticles(bool active)
	{
		for(int i=0; i<numberOfLanes; i++)
		{
			getParticleEffect((Lanes)i).activeComboParticle(active);
		}
	}

	public bool isNoMoreArrow()
	{
		return nextLeft == null && nextDown == null && nextUp == null && nextRight == null
			&& frozenUp == null && frozenRight == null && frozenLeft == null && frozenDown == null;
	}

	public void lockLane()
	{
		locked = true;
		leftArrowsArray = leftArrows.ToArray ();
		leftArrows.Clear ();
		if(leftArrowsArray.Length > 0) nextLeft = leftArrowsArray[0];
		rightArrowsArray = rightArrows.ToArray ();
		rightArrows.Clear ();
		if(rightArrowsArray.Length > 0) nextRight = rightArrowsArray[0];
		upArrowsArray = upArrows.ToArray ();
		upArrows.Clear ();
		if(upArrowsArray.Length > 0) nextUp = upArrowsArray[0];
		downArrowsArray = downArrows.ToArray ();
		downArrows.Clear ();
		if(downArrowsArray.Length > 0) nextDown = downArrowsArray[0];
	}

	public void validArrow(Lanes lane, Arrow arrowValid, bool checkLinked = true, bool endFreezeValidation = false)
	{
		if (arrowValid != null) {
			if(arrowValid.type == ArrowType.MINE)
			{
				ChartManager.instance.modelLane.getParticleEffect (lane).playMine();
				LifeController.instance.addHPbyPrecision(Precision.MINE);
				ScoreController.instance.addScoreByPrecision(Precision.MINE);
				AudioController.instance.playSound("MineBoom", 0.8f, 1.2f);
			}else{
				if(arrowValid.type == ArrowType.NORMAL || !endFreezeValidation)
				{
					ChartManager.instance.modelLane.getParticleEffect (lane).play (arrowValid.precisionValid);

					if(checkLinked)
					{
						ComboController.instance.addCombo(arrowValid.precisionValid);
						LifeController.instance.addHPbyPrecision(arrowValid.precisionValid);
						ScoreController.instance.addScoreByPrecision(arrowValid.precisionValid);
						NoteController.instance.showNote(arrowValid.precisionValid, arrowValid.timingValid);
					}

					if(arrowValid.type == ArrowType.FREEZE || arrowValid.type == ArrowType.ROLL)
					{
						attachToModelLane(ChartManager.instance.modelLane, arrowValid, lane);
						ChartManager.instance.validFreezeCurrentArrow(arrowValid);
					}


				}else{
					ChartManager.instance.modelLane.getParticleEffect (lane).playEndFreeze();
					ChartManager.instance.modelLane.getParticleEffect(lane).stopFreezeOrRoll();
					LifeController.instance.addHPbyPrecision(Precision.FREEZE);
					ScoreController.instance.addScoreByPrecision(Precision.FREEZE);
				}
			}
		}

		if (!endFreezeValidation && checkLinked && arrowValid.linkedArrows.Count != 0) {
			foreach(Arrow arrow in arrowValid.linkedArrows)
			{
				validArrow(arrow.currentLane, arrow, false);
			}
		}

		//If its not a freeze waiting to be completed
		if ((arrowValid.type == ArrowType.FREEZE || arrowValid.type == ArrowType.ROLL)) {
			if(!endFreezeValidation)
			{
				setFrozenLaneArrows(lane, arrowValid);
				pushNextArrow (lane);
			}else{
				arrowValid.gameObject.SetActive(false);
				setFrozenLaneArrows(lane, null);
			}
		}else{
			arrowValid.gameObject.SetActive (false);
			pushNextArrow (lane);
		}



	}

	public void attachToModelLane(LaneManager modelLane, Arrow arrow, Lanes lane)
	{
		arrow.transform.SetParent (modelLane.getLane (lane));
		arrow.transform.localPosition = new Vector3 (0f, 0f, arrow.transform.localPosition.z);
		modelLane.getLaneArrows(lane).Add (arrow);
		arrow.attached = true;
	}

	public void distachFromModelLane(LaneManager modelLane, Lanes lane)
	{
		foreach(Arrow arrow in modelLane.getLaneArrows(lane))
		{
			arrow.transform.SetParent(getLane(lane));
			arrow.attached = false;
		}
		modelLane.getLaneArrows(lane).Clear();
	}

	public void missArrow(Lanes lane, Arrow arrowMissed, bool missIsBad = false, bool firstCall = true)
	{
		if (missIsBad) {
			if(arrowMissed.type == ArrowType.NORMAL) {
				ChartManager.instance.modelLane.getParticleEffect (lane).play (arrowMissed.precisionValid);
				if(firstCall)
				{
					LifeController.instance.addHPbyPrecision(Precision.MISS);
					ScoreController.instance.addScoreByPrecision(Precision.MISS);
					ComboController.instance.breakCombo();
					NoteController.instance.showNote(Precision.MISS);
				}
			}else if(arrowMissed.type != ArrowType.MINE)
			{
				if(arrowMissed.attached)
				{
					ChartManager.instance.modelLane.getParticleEffect (lane).stopFreezeOrRoll();
					LifeController.instance.addHPbyPrecision(Precision.UNFREEZE);
					ScoreController.instance.addScoreByPrecision(Precision.UNFREEZE);
				}else{
					if(firstCall)
					{
						LifeController.instance.addHPbyPrecision(Precision.MISS);
						ScoreController.instance.addScoreByPrecision(Precision.MISS);
						ComboController.instance.breakCombo();
						NoteController.instance.showNote(Precision.MISS);
					}
				}
			}
		}

		if (arrowMissed.attached)
		{
			distachFromModelLane (ChartManager.instance.modelLane, lane);
			setFrozenLaneArrows(lane, null);
		}else{
			pushNextArrow (lane);
		}
			

		if (firstCall && arrowMissed.linkedArrows.Count != 0) {
			foreach(Arrow arrow in arrowMissed.linkedArrows)
			{
				missArrow(arrow.currentLane, arrow, missIsBad, false);
			}
		}


	}

	private List<Arrow> removeFromTrash = new List<Arrow>();
	public void autoMissArrowFromTrash()
	{
		if (trashOfArrowMissed.Count > 0) {
			removeFromTrash.Clear();
			foreach(Arrow trashedArrow in trashOfArrowMissed)
			{
				if(trashedArrow.checkAndProcessMissArrow(ChartManager.instance.currentTime))
				{
					missTrashedArrow(trashedArrow);

					if(trashedArrow.linkedArrows.Count != 0) {
						foreach(Arrow arrowLinked in trashedArrow.linkedArrows)
						{
							missTrashedArrow(arrowLinked, false);
						}
					}
				}
			}
			if(removeFromTrash.Count > 0)
			{
				foreach(Arrow arrowToRemove in removeFromTrash) { trashOfArrowMissed.Remove(arrowToRemove); }
			}
		}
	}

	public void missTrashedArrow(Arrow arrow, bool checkLinked = true)
	{
		if (arrow.attached)
		{
			setFrozenLaneArrows(arrow.currentLane, null);
			distachFromModelLane (ChartManager.instance.modelLane, arrow.currentLane);
		}
			

		removeFromTrash.Add (arrow);
	}

	public void playParticleEffect(Precision precision, Lanes lane)
	{
		getParticleEffect (lane).play (precision);
	}

	#region ArrayAndSinglecontrollers
	private Arrow[] currentArray;
	private int newIndex;
	public void pushNextArrow(Lanes lane, bool untilNextValid = false)
	{
		currentArray = getLaneArrowsArray (lane);

		if (untilNextValid) {
			trashOfArrowMissed.Add(currentArray[indexes[(int)lane]]);
			for(int i=indexes[(int)lane]+1; i<=currentArray.Length; i++)
			{
				newIndex = i;
				if(i == currentArray.Length)
				{
					newIndex = currentArray.Length;
				}else{
					currentArray[i].tryTagAsMissed(ChartManager.instance.currentTime);
					if(currentArray[i].tagAsMissed){
						trashOfArrowMissed.Add(currentArray[i]);
					}else{
						break;
					}
				}
			}
			indexes[(int)lane] = newIndex;
		} else {
			indexes [(int)lane] += 1;
			newIndex = indexes [(int)lane];
		}
		
		if (newIndex < currentArray.Length) {
			setNextLaneArrows (lane, currentArray [newIndex]);
		} else {
			setNextLaneArrows (lane, null);
		}
	}

	public Arrow getNextLaneArrows(Lanes lane)
	{
		switch(lane)
		{
		case Lanes.LEFT:
			return nextLeft;
		case Lanes.DOWN:
			return nextDown;
		case Lanes.UP:
			return nextUp;
		case Lanes.RIGHT:
			return nextRight;				
		}
		return null;
	}

	public Arrow getNextLaneValidArrows(Lanes lane)
	{
		currentArray = getLaneArrowsArray (lane);
		for(int i=indexes[(int)lane]; i<=currentArray.Length; i++)
		{
			if(i == currentArray.Length)
			{
				return null;
			}else if(!currentArray[i].tagAsMissed){
				return currentArray[i];
			}
		}
		return null;
	}

	public Arrow setNextLaneArrows(Lanes lane, Arrow arrow)
	{
		switch(lane)
		{
		case Lanes.LEFT:
			return nextLeft = arrow;
		case Lanes.DOWN:
			return nextDown = arrow;
		case Lanes.UP:
			return nextUp = arrow;
		case Lanes.RIGHT:
			return nextRight = arrow;				
		}
		return null;
	}

	public Arrow getFrozenLaneArrows(Lanes lane)
	{
		switch(lane)
		{
		case Lanes.LEFT:
			return frozenLeft;
		case Lanes.DOWN:
			return frozenRight;
		case Lanes.UP:
			return frozenUp;
		case Lanes.RIGHT:
			return frozenDown;				
		}
		return null;
	}

	public Arrow setFrozenLaneArrows(Lanes lane, Arrow arrow)
	{
		switch(lane)
		{
		case Lanes.LEFT:
			return frozenLeft = arrow;
		case Lanes.DOWN:
			return frozenRight = arrow;
		case Lanes.UP:
			return frozenUp = arrow;
		case Lanes.RIGHT:
			return frozenDown = arrow;				
		}
		return null;
	}
	#endregion

	#region Listcontrollers
	public void pushArrow(Arrow ar, Lanes lane)
	{
		getLaneArrows(lane).Add(ar);
	}
	
	public Arrow getFirstArrow(Lanes lane)
	{
		return getLaneArrows(lane).FirstOrDefault();
	}
	
	public void removeArrow(Lanes lane)
	{
		getLaneArrows(lane).RemoveAt(0);
	}

	public double getMinArrowTime()
	{
		List<double> times = new List<double>();
		string[] lanes = System.Enum.GetNames(typeof(Lanes));
		foreach(string lane in lanes)
		{
			List<Arrow> laneMin = getLaneArrows((Lanes)System.Enum.Parse(typeof(Lanes), lane));
			if(laneMin.Count > 0) times.Add(laneMin.First().scheduledTime);
		}
		return times.Count > 0 ? times.Min() : (double)999999;
	}

	public double getMaxArrowTime()
	{
		List<double> times = new List<double>();
		string[] lanes = System.Enum.GetNames(typeof(Lanes));
		foreach(string lane in lanes)
		{
			List<Arrow> laneMax = getLaneArrows((Lanes)System.Enum.Parse(typeof(Lanes), lane));
			if(laneMax.Count > 0)
			{
				Arrow arrowLast = laneMax.Last();
				double scheduledTime = arrowLast.scheduledTime;
				if(arrowLast.type == ArrowType.FREEZE || arrowLast.type == ArrowType.ROLL)
				{
					scheduledTime = arrowLast.getFreezeController(arrowLast.type).timeEndScheduled;
				}
				times.Add(scheduledTime);
			}
		}
		return times.Count > 0 ? times.Max() : (double)-999999;
	}
	#endregion

}
