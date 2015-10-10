using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

[System.Serializable]
public class ArrowModel {
	public GameObject model;
	public bool canBeTurned;
	public Material associatedMaterial;
}

public class TimeBuffer
{
	public double startvalue;
	public double available;
	public double buffer;
	public double completed;

	public void init(double startValue)
	{
		startvalue = startValue;
		available = startvalue;
		buffer = 0;
		completed = 0;
	}

	public void flushBuffer()
	{
		completed += buffer;
		available -= buffer;
	}
}

public class ChartManager : MonoBehaviour {

	public static ChartManager instance;

	void Awake()
	{
		if(instance == null)
		{
			instance = this;
		}
		painter = GetComponent<ArrowPainter>();
		emissionTweener = GetComponent<EmissionTweener> ();

		//SongOptionManager.instance.currentSongPlayed = LoadManager.instance.songPacks.First().songsData.Find(c => c.name.Contains("Bangkok")).songs[Difficulty.EXPERT];
		//SongOptionManager.instance.speedmodSelected = 3f;

		if(GameManager.instance != null)
		{
			systemOffset += GameManager.instance.prefs.globalOffsetSeconds + SongOptionManager.instance.currentSongPlayed.offset;
		}

		currentTime = -1;
		currentSyncTime = -1;
	}
	
	//Models
	public LaneManager modelLane;
	public LaneManager chartLane;
	public LaneManager mineLane;
	ArrowPainter painter;
	EmissionTweener emissionTweener;
	ScaleTweener[] scaleTweeners;

	public List<ArrowModel> arrowModels;
	public List<ArrowModel> arrowColored;
	public GameObject mine;
	
	//Manager global variable
	public Transform scrollingObject;
	public float systemSpeedmod = 1f; //Resize the chart to ITG Like base spacement
	public double systemOffset = -0.150; //Engine Offset
	public float timeBeforeStart;

	//Private global variable
	double[] musicalBumps;
	double[] musicalJumps;
	Vector2 rangeArrow = new Vector2(0f, 0f);
	float cameraForward;
	int numberOfLanes;

	bool chartStart = false;
	bool gameOver = false;
	bool songHasntStarted = true;

	double actualBPM = 0;
	TimeBuffer actualSTOPBuffer;
	int BPMIndex = 1;
	int BPMCount;
	int STOPIndex = 0;
	int STOPCount;

	//Time values
	double lastScrollingPosition = 0; //Last scrolling position since last BPM change
	public double currentTime = 0; //Total time, stops included
	double currentSyncTime = 0; //Total time since last BPM, stops excluded


	//Variable Pool
	private Arrow currentCheckedArrow;
	private Arrow currentFrozenArrow;

	// Use this for initialization
	void Start () {

		//Inital values
		numberOfLanes = System.Enum.GetValues (typeof(Lanes)).Length;

		//Chart and scene creation
		ArrowModel modelSelected = arrowModels[SongOptionManager.instance.skinSelected];
		scaleTweeners = new ScaleTweener[numberOfLanes];
		for (int i=0; i<numberOfLanes; i++) {
			GameObject modelInst = Instantiate(modelSelected.model, Vector3.zero, modelSelected.model.transform.rotation) as GameObject;
			modelInst.transform.SetParent(modelLane.getLane((Lanes)i));
			modelInst.transform.localPosition = Vector3.zero;
			if(modelSelected.canBeTurned) Utils.turnOnLane(modelInst.transform, (Lanes)i);
			scaleTweeners[i] = modelInst.GetComponent<ScaleTweener>();
		}

		createChart(SongOptionManager.instance.currentSongPlayed);

		emissionTweener.concernedMaterial = modelSelected.associatedMaterial;
		emissionTweener.init ();

		//Data initialization
		actualBPM = SongOptionManager.instance.currentSongPlayed.bpms.First ().Value;
		actualSTOPBuffer = null;
		BPMIndex = 1;
		STOPIndex = 0;
		BPMCount = SongOptionManager.instance.currentSongPlayed.bpms.Count;
		STOPCount = SongOptionManager.instance.currentSongPlayed.stops.Count;

		//Initialisations
		chartLane.lockLane ();
		mineLane.lockLane ();
		initComputeTime ();
		TimeController.instance.init (rangeArrow);
		moveChart ();
		AudioController.instance.loadSong (SongOptionManager.instance.currentSongPlayed);
		Invoke ("callChartStart", 1f);
	}
	
	// Update is called once per frame
	void Update () {
		if (chartStart && !gameOver) {

			if(songHasntStarted && currentTime >= systemOffset)
			{
				songHasntStarted = false;
				currentTime = systemOffset;
				currentSyncTime = systemOffset;
				AudioController.instance.startSong();
			}

			checkLanesStatus ();
			computeTime ();
			moveChart ();
			processMusicalBump ();
		}
	}

	#region updates methods
	private TimeBuffer timeBuffer = new TimeBuffer();
	private double nextBPMKey, nextBPMValue, nextSTOPKey, nextSTOPValue;
	public void initComputeTime()
	{
		if (BPMIndex < BPMCount) {
			nextBPMKey = SongOptionManager.instance.currentSongPlayed.bpms.ElementAt (BPMIndex).Key;
		} else {
			nextBPMKey = 9999999;
		}
		if (STOPIndex < STOPCount) {
			nextSTOPKey = SongOptionManager.instance.currentSongPlayed.stops.ElementAt (STOPIndex).Key;
		} else {
			nextSTOPKey = 9999999;
		}
			
	}

	public void computeTime()
	{
		//The time is a buffer. This buffer will decrease for each time operation we're gonna make within the frame.
		timeBuffer.init((double)Time.deltaTime);
		timeBuffer.buffer = 0;
		timeBuffer.completed = 0;

		//The current time ins't affected by these operations, it increase directly with the full buffer.
		currentTime += timeBuffer.startvalue;

		if (BPMIndex < BPMCount || STOPIndex < STOPCount) {
			while (nextBPMKey <= currentTime || nextSTOPKey <= currentTime) 
			{
				if (nextBPMKey <= currentTime && nextBPMKey <= nextSTOPKey) {
					//Move chart to the exact BPM change
					nextBPMValue = SongOptionManager.instance.currentSongPlayed.bpms.ElementAt (BPMIndex).Value;
					timeBuffer.buffer = nextBPMKey - (currentTime - timeBuffer.available);
					addTimeToSyncTime (timeBuffer.buffer);
					timeBuffer.flushBuffer ();
					
					moveChart ();
					lastScrollingPosition = getScrollingObjectPosition ();
					currentSyncTime = 0;
					
					actualBPM = nextBPMValue;
					BPMIndex++;
				} else if (nextSTOPKey <= currentTime && nextSTOPKey <= nextBPMKey) {
					nextSTOPValue = SongOptionManager.instance.currentSongPlayed.stops.ElementAt (STOPIndex).Value;
					timeBuffer.buffer = nextSTOPKey - (currentTime - timeBuffer.available);
					addTimeToSyncTime (timeBuffer.buffer);
					timeBuffer.flushBuffer ();
					moveChart ();
					
					actualSTOPBuffer = new TimeBuffer ();
					actualSTOPBuffer.init (nextSTOPValue);
					STOPIndex++;
				}

				initComputeTime ();
			}
		}

		addTimeToSyncTime(timeBuffer.available);
	}

	public void addTimeToSyncTime(double time)
	{
		if (actualSTOPBuffer != null) {
			actualSTOPBuffer.buffer = time;
			if(actualSTOPBuffer.available < time)
			{
				time -= actualSTOPBuffer.available;
			}else{
				time = 0;
			}
			actualSTOPBuffer.flushBuffer();
			if(actualSTOPBuffer.available <= 0) actualSTOPBuffer = null;
		}

		currentSyncTime += time;
	}

	public void moveChart()
	{
		scrollingObject.position = -(Vector3.up * (float)getScrollingObjectPosition ()) + Vector3.forward*cameraForward;
	}

	//For ending only
	public void manualMoveChart(float distance)
	{
		scrollingObject.position -= Vector3.up*distance;
	}

	//Check for misses or freeze validation
	public void checkLanesStatus()
	{
		for (int i=0; i<numberOfLanes; i++) {
			currentCheckedArrow = chartLane.getNextLaneArrows((Lanes)i);
			currentFrozenArrow = chartLane.getFrozenLaneArrows((Lanes)i);

			//Freezes
			if(currentFrozenArrow != null)
			{
				currentFrozenArrow.computeFreezePosition(currentTime);
				
				if(currentFrozenArrow.checkTimeEndFreeze(currentTime))
				{
					chartLane.validArrow((Lanes)i, currentFrozenArrow, false, true);
				}else if(currentFrozenArrow.checkMissFreeze(currentTime))
				{
					chartLane.missArrow((Lanes)i, currentFrozenArrow, true, false);
				}
			}

			//Normal and freezes begin
			if(currentCheckedArrow != null)
			{
				//Validated arrow for previous inputs : Confirmation
				if(currentCheckedArrow.state == ArrowState.VALIDATED)
				{
					//The arrow is validated
					if(currentCheckedArrow.type == ArrowType.NORMAL)
					{
						chartLane.validArrow((Lanes)i, currentCheckedArrow);
					}else //For freeze and roll
					if(currentCheckedArrow.type == ArrowType.FREEZE || currentCheckedArrow.type == ArrowType.ROLL)
					{
						//If not attached
						if(!currentCheckedArrow.attached){

							chartLane.validArrow((Lanes)i, currentCheckedArrow);
							currentCheckedArrow.computeFreezePosition(currentTime);

							//Enable freeze
							currentCheckedArrow.getFreezeController(currentCheckedArrow.type).hit(currentTime);
							if(currentCheckedArrow.type == ArrowType.ROLL)
							{
								modelLane.getParticleEffect((Lanes)i).playRoll();
								currentCheckedArrow.getFreezeController(currentCheckedArrow.type).enableLetInUpdate(true);
							}else{
								modelLane.getParticleEffect((Lanes)i).playFreeze();
							}


		
						}
					}
				}

				//Check missed arrow this turn
				if(currentCheckedArrow.state == ArrowState.MISSED || (currentCheckedArrow.state == ArrowState.NONE || currentCheckedArrow.state == ArrowState.WAITINGLINKED) 
				   && currentCheckedArrow.checkAndProcessMissArrow(currentTime))
				{
					chartLane.missArrow((Lanes)i, currentCheckedArrow, true);
				}
				  
			}

			//Mines
			currentCheckedArrow = mineLane.getNextLaneArrows((Lanes)i);
			if(currentCheckedArrow != null && currentCheckedArrow.state == ArrowState.NONE 
			   && currentCheckedArrow.checkAndProcessMissMine(currentTime))
			{
				mineLane.missArrow((Lanes)i, currentCheckedArrow);
			}
		}

		//Go to trash old tag as missed but not officialy missed yet
		chartLane.autoMissArrowFromTrash ();

		//Check for game over
		if (chartLane.isNoMoreArrow () && mineLane.isNoMoreArrow ()) {
			callGameOver(true);
		}
	}

	public int indexBump = 0;
	public int indexJump = 0;
	public void processMusicalBump()
	{
		if (indexBump < musicalBumps.Length && musicalBumps [indexBump] <= currentTime) {

			indexBump++;

			//All bumps effect
			emissionTweener.pulse();
			if((indexBump-1) % 2 == 0)LifeController.instance.rotateTick.tick ();
		}


		if (indexJump < musicalJumps.Length && musicalJumps [indexJump] <= currentTime) {
			
			indexJump++;
			
			//All jumps effect
			//BackgroundColorController.instance.bumpExposure(); naze
			EffectController.instance.activeJumpParticles();
		}
	}
	#endregion

	#region Inputs
	//Validation des notes, maintien des rolls, activations des freezes
	//Pas de validation directe : En attente de la routine de chart manager
	public void hitLane(Lanes lane)
	{
		//Check the next arrow
		currentCheckedArrow = chartLane.getNextLaneArrows(lane);
		currentFrozenArrow = chartLane.getFrozenLaneArrows(lane);

		//First we check the freeze
		if(currentFrozenArrow != null)
		{
			//If the freeze is already valid, and the next arrow is ready for a great, take it
			if(currentFrozenArrow.getFreezeController(currentFrozenArrow.type).isAlreadyValid() 
			   && currentCheckedArrow != null && currentCheckedArrow.getArrowPrec(currentTime) <= Precision.GREAT)
			{
				chartLane.validArrow(lane, currentFrozenArrow, false, true);
				currentFrozenArrow = null;
			}else{
				validFreezeCurrentArrow(currentFrozenArrow);
			}
		}

		if (currentCheckedArrow != null && currentFrozenArrow == null) {

			//Tag the next arrow as missed : If the precision is more than decent
			currentCheckedArrow.tryTagAsMissed(currentTime);

			//If it's the case, the arrow is missed, so we check the next one
			if(currentCheckedArrow.tagAsMissed)
			{
				//If the next one is ok to be valid, we valid it instead of the missed one
				currentCheckedArrow = chartLane.getNextLaneValidArrows(lane);
				if(currentCheckedArrow != null && 
				   currentCheckedArrow.getArrowPrec(currentTime) <= Precision.GREAT)
				{
					chartLane.pushNextArrow(lane, true);
				}

				currentCheckedArrow = chartLane.getNextLaneArrows(lane);
			}

			//Validation process of an arrow
			currentCheckedArrow.checkAndProcessValidateArrow (currentTime);
		}

		scaleTweeners [(int)lane].activeTween ();

	}

	//Maintien des frrezes, explosion des mines
	public void holdLane(Lanes lane)
	{
		currentFrozenArrow = chartLane.getFrozenLaneArrows(lane);
		if (currentFrozenArrow != null)
		{
			if(currentFrozenArrow.type == ArrowType.FREEZE)
			{
				currentFrozenArrow.getFreezeController(currentFrozenArrow.type).hit(currentTime);
			}
		}

		currentCheckedArrow = mineLane.getNextLaneArrows(lane);
		if(currentCheckedArrow != null && currentCheckedArrow.state == ArrowState.NONE 
		   && currentCheckedArrow.checkAndProcessValidateMine(currentTime))
		{
			mineLane.validArrow(lane, currentCheckedArrow);
		}
	}

	//Desactivation des freezes
	public void releaseLane(Lanes lane)
	{
		currentFrozenArrow = chartLane.getFrozenLaneArrows(lane);
		if (currentFrozenArrow != null)
		{
			if(currentFrozenArrow.type == ArrowType.FREEZE)
			{
				currentFrozenArrow.getFreezeController(currentFrozenArrow.type).enableLetInUpdate(true);
			}
		}
	}

	#endregion

	#region management

	public void callChartStart()
	{
		chartStart = true;

	}

	public void callGameOver(bool clear = false)
	{
		if (!gameOver) {
			gameOver = true;
			if(clear)
			{
				EndingController.instance.showCleared(ComboController.instance.getCurrentComboType());
			}else{
				EndingController.instance.showFailed();
			}

		}
	}

	#endregion

	#region Utils

	public void validFreezeCurrentArrow(Arrow arrow)
	{
		arrow.getFreezeController(arrow.type).hit(currentTime);
		if(arrow.type == ArrowType.FREEZE)
		{
			arrow.getFreezeController(arrow.type).enableLetInUpdate(false);
		}
	}

	public double getScrollingObjectPosition()
	{
		return (Utils.getBPS (actualBPM) * currentSyncTime * SongOptionManager.instance.speedmodSelected * systemSpeedmod) + lastScrollingPosition;
	}

	public bool isGameOver()
	{
		return gameOver;
	}

	#endregion
	
	#region chartCreation

	void createChart(Song s)
	{
		float currentYPosition = -modelLane.transform.localPosition.y;
		cameraForward = -modelLane.transform.localPosition.z;
		
		float cursorPrecision = 0.001f;

		//BPM and STOPS indexs
		BPMIndex = 1;
		STOPIndex = 0;
		BPMCount = s.bpms.Count;
		STOPCount = s.stops.Count;

		//Mesure indexs
		double mesureIndex = 0;
		double prevMesureIndex = 0;

		//Time indexs
		double bufferBPMTime = 0;
		double savedBPMTime = 0;
		double currentSTOPTime = 0;
		double currentBufferTime = 0;

		List<double> tempMusicalBumps = new List<double>();
		List<double> tempMusicalJumps = new List<double>();

		foreach (List<string> mesure in s.stepchart) {
			for(int beatLine=0; beatLine<mesure.Count; beatLine++)
			{

				//Get current BPS for this beatLine
				double currentBPS = Utils.getBPS(s.bpms.ElementAt(BPMIndex - 1).Value);

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
							currentBPS = Utils.getBPS(s.bpms.ElementAt(BPMIndex - 1).Value);
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
				currentBufferTime = bufferBPMTime + savedBPMTime + currentSTOPTime;

				if((beatLine)%(mesure.Count/4) == 0) tempMusicalBumps.Add(currentBufferTime);


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
					case OptionsMod.ROLLTOFREEZE:
						unmanagedLine = unmanagedLine.Replace('4', '2');
						break;
					}

				}

				//Final line
				char[] finalBeatLine = unmanagedLine.ToCharArray();

				List<Arrow> beatLineArrows = new List<Arrow>();
				ArrowModel modelSkinSelected = arrowColored[SongOptionManager.instance.skinSelected];
				GameObject arrowObj = null; //Obj arrow variable;
				Arrow currentArrow = null; //Arrow variable;
				LaneManager currentLaneManager = chartLane;
				//Read the 4 lane of a beat
				for(int i=0; i<numberOfLanes; i++)
				{
					if("124M".Contains(finalBeatLine[i]))
					{
						currentLaneManager = chartLane;
						if(finalBeatLine[i] == 'M')
						{
							currentLaneManager = mineLane;
							arrowObj = Instantiate(mine, Vector3.zero, mine.transform.rotation) as GameObject;
						}else{
							arrowObj = Instantiate(modelSkinSelected.model, Vector3.zero, modelSkinSelected.model.transform.rotation) as GameObject;
						}
						arrowObj.transform.SetParent(currentLaneManager.getLane((Lanes)i));
						arrowObj.transform.localPosition = -Vector3.up*currentYPosition;

						currentArrow = arrowObj.GetComponent<Arrow>();

						if(modelSkinSelected.canBeTurned && currentArrow.rotationRoot != null)
						{
							Utils.turnOnLane(currentArrow.rotationRoot, (Lanes)i);
						}


						if(finalBeatLine[i] != 'M') currentArrow.coloredObject.material.color = painter.getMesureColor(mesure.Count, beatLine+1);
						currentArrow.scheduledTime = currentBufferTime;
						currentArrow.currentLane = (Lanes)i;

						Arrow savedArrow = currentArrow;
						if(finalBeatLine[i] != 'M') beatLineArrows.Add(savedArrow);
						
						currentLaneManager.pushArrow(currentArrow, (Lanes)i);

					}

					switch(finalBeatLine[i])
					{
					case '2':
						currentArrow.type = ArrowType.FREEZE;
						break;
					case '4':
						currentArrow.type = ArrowType.ROLL;
						break;
					case '3':
						currentArrow = chartLane.getLaneArrows((Lanes)i).Last();
						FreezeController controller = currentArrow.getFreezeController(currentArrow.type);
						controller.gameObject.SetActive(true);
						controller.init(currentArrow, currentYPosition + currentArrow.transform.position.y, currentBufferTime, currentArrow.coloredObject.material.color);
						break;
					case 'M':
						currentArrow.type = ArrowType.MINE;
						arrowObj.transform.Rotate(Vector3.forward*Random.Range(0f, 360f));
						break;
					}
				}

				if(beatLineArrows.Count > 1)
				{
					tempMusicalJumps.Add(currentBufferTime);
					foreach(Arrow arrow in beatLineArrows)
					{
						arrow.linkedArrows.AddRange(beatLineArrows);
						arrow.linkedArrows.Remove(arrow);
					}
				}

				if(BPMIndex < BPMCount)
				{
					if(Mathf.Abs((float)(s.mesureBPMS[BPMIndex] - mesureIndex)) < cursorPrecision)
					{
						savedBPMTime += bufferBPMTime;
						bufferBPMTime = 0;
						BPMIndex++;
					}
				}
				
				if(STOPIndex < STOPCount)
				{
					if(Mathf.Abs((float)(s.mesureSTOPS[STOPIndex] - mesureIndex)) < cursorPrecision)
					{
						currentSTOPTime += s.stops.ElementAt(STOPIndex).Value;
						STOPIndex++;
					}
				}

				prevMesureIndex = mesureIndex;
				mesureIndex += ((double)4/(double)mesure.Count);

				currentYPosition += (float)(((double)4/(double)mesure.Count)*SongOptionManager.instance.speedmodSelected*systemSpeedmod);
			}
		}

		musicalBumps = tempMusicalBumps.ToArray ();
		musicalJumps = tempMusicalJumps.ToArray ();
		rangeArrow = new Vector2(Mathf.Min((float)chartLane.getMinArrowTime(), (float)mineLane.getMinArrowTime()), Mathf.Max((float)chartLane.getMaxArrowTime(), (float)mineLane.getMaxArrowTime()));
	}
	#endregion
}
