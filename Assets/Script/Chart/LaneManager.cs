using UnityEngine;
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

	public Transform left;
	public Transform right;
	public Transform up;
	public Transform down;
	
	List<Arrow> leftArrows;
	List<Arrow> rightArrows;
	List<Arrow> upArrows;
	List<Arrow> downArrows;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	public List<Arrow> getLaneArrows(Lanes lane)
	{
		switch(lane)
		{
			case Lanes.LEFT:
				return leftArrows;
			case Lanes.DOWN:
				return downArrows;
			case Lanes.UP:
				return upArrows;
			case Lanes.RIGHT:
				return rightArrows;				
		}
	}
	
	public void pushArrow(Arrow ar, double time, Lanes lane)
	{
		//Placement, gestion du temps
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
}
