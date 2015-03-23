using UnityEngine;
using System.Collections;

public class ChartManager : MonoBehaviour {

	public static ChartManager instance;

	void Awake()
	{
		if(instance == null)
		{
			instance = this;
		}
	}
	
	
	public LaneManager modelLane;
	public LaneManager chartLane;
	public LaneManager mineLane;
	
	List<double> musicalBumps;
	
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	
	#region chartCreation
	void createChart(Song s)
	{
		float currentYPosition = 0f;
		
		musicalBumps = new List<double>();
	}
	#endregion
}
