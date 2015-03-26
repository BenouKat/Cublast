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
	
	List<Arrow> leftArrows = new List<Arrow>();
	List<Arrow> rightArrows = new List<Arrow>();
	List<Arrow> upArrows = new List<Arrow>();
	List<Arrow> downArrows = new List<Arrow>();

	// Use this for initialization
	void Start () {
	
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
		return null;
	}
	
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
}
