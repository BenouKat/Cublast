using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

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
	Vector2 rangeArrow = new Vector2(0f, 0f);
	
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
		float cursorPrecision = 0.001f;

		//BPM and STOPS indexs
		int BPMIndex = 1;
		int STOPIndex = 0;
		int BPMCount = s.bpms.Count;
		int STOPCount = s.stops.Count;

		//Mesure indexs
		double mesureIndex = 0;
		double prevMesureIndex = 0;

		//Time indexs
		double bufferBPMTime = 0;
		double savedBPMTime = 0;
		double currentSTOPTime = 0;
		double currentTime = 0;

		musicalBumps = new List<double>();

		foreach (List<string> mesure in s.stepchart) {
			for(int beatLine=0; beatLine<mesure.Count; beatLine++)
			{

				//Get current BPS for this beatLine
				double currentBPS = s.getBPS(s.bpms.ElementAt(BPMIndex - 1).Value);

				//If there's BPM or STOP changes
				if(BPMIndex < BPMCount || STOPIndex < STOPCount)
				{
					//Searching for BPM and STOP pass before the mesure
					while((BPMIndex < BPMCount && s.mesureBPMS[BPMIndex] < mesureIndex - cursorPrecision)
					      || STOPIndex < STOPCount && s.mesureSTOPS[STOPIndex] < mesureIndex - cursorPrecision)
					{
						if(BPMIndex < BPMCount && (STOPIndex >= STOPCount || s.mesureBPMS[BPMIndex] <= s.mesureSTOPS[STOPIndex]))
						{
							bufferBPMTime += (s.mesureBPMS[BPMIndex] - prevMesureIndex)/currentBPS;
							savedBPMTime += bufferBPMTime;
							bufferBPMTime = 0;

							prevMesureIndex = s.mesureBPMS[BPMIndex];
							BPMIndex++;
							currentBPS = s.getBPS(s.bpms.ElementAt(BPMIndex - 1).Value);
						}

						if(STOPIndex < STOPCount && (BPMIndex >= BPMCount || s.mesureBPMS[BPMIndex] >= s.mesureSTOPS[STOPIndex]))
						{
							currentSTOPTime += s.stops.ElementAt(STOPIndex).Value;
							STOPIndex++;
						}
					}
				}

				//Time of the mesure
				bufferBPMTime += (mesureIndex - prevMesureIndex)/currentBPS;
				currentTime = bufferBPMTime + savedBPMTime + currentSTOPTime;

				if((beatLine)%(mesure.Count/4) == 0) musicalBumps.Add(currentTime);


				//Unmanaged beat line, parsing to options
				string unmanagedLine = mesure[beatLine].Trim();

				//Options parsing
				foreach(OptionsMod mod in SongOptionManager.instance.currentOptions)
				{
					switch(mod)
					{
					case OptionsMod.NOMINES:
						unmanagedLine = unmanagedLine.Replace('M', '0');
						break;
					case OptionsMod.NOJUMPS:
						if(unmanagedLine.Where(c => "124".Contains(c)).Count() == 2)
						{
							for(int i=0; i<4;i++)
							{
								if("124".Contains(unmanagedLine[i]))
								{
									unmanagedLine = unmanagedLine.Remove(i, 1).Insert(i, "0");
									break;
								}
							}
						}
						break;
					case OptionsMod.NOHANDS:
						if(unmanagedLine.Where(c => "124".Contains(c)).Count() >= 3)
						{
							for(int i=0; i<4;i++)
							{
								if("124".Contains(unmanagedLine[i]))
								{
									unmanagedLine = unmanagedLine.Remove(i, 1).Insert(i, "0");
									if(unmanagedLine.Where(c => "124".Contains(c)).Count() < 3) i = 5;
								}
							}
						}
						break;
					case OptionsMod.NOFREEZE:
						unmanagedLine = unmanagedLine.Replace('2', '1').Replace('3', '0');
						break;
					case OptionsMod.NOROLLS:
						unmanagedLine = unmanagedLine.Replace('4', '1').Replace('3', '0');
						break;
					case OptionsMod.ROLLTOFREEZE:
						unmanagedLine = unmanagedLine.Replace('4', '2');
						break;
					}

				}

				//Final line
				char[] finalBeatLine = unmanagedLine.ToCharArray();

				//To do !!
				List<Arrow> beatLineArrows = new List<Arrow>();


			}
		}
	}
	#endregion
}
