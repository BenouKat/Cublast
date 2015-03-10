using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;

[Serializable]
public class SerializableSong {

	//global information
	public string packName;
	public string songFileName;
	
	private string title;
	private string subtitle;
	private string artist;
	private string banner;
	private string bpmToDisplay;
	private string background;
	private double offset;
	private double samplestart;
	private double samplelenght;
	private List<double> mesureBPMS;
	private List<double> mesureSTOPS;
	private List<double> bpmsKey;
	private List<double> bpmsValue;
	private List<double> stopsKey;
	private List<double> stopsValue;
	private string stepartist;
	private Difficulty difficulty;
	private int level;
	private int numberOfSteps;
	private int numberOfStepsWithoutJumps;
	private int numberOfStepsAbsolute;
	private int numberOfFreezes;
	private int numberOfRolls;
	private int numberOfMines;
	private int numberOfJumps;
	private int numberOfHands;
	private double duration;
	private double[] intensityGraph;
	
	private double stepPerSecondAverage;
	private double stepPerSecondMaximum;
	private double stepPerSecondStream;
	private double longestStream;
	private int numberOfCross;
	private int numberOfFootswitch;
	private string song;
	
	private SongInfoProfil sip;
	
	//stepchart
	public List<List<string>> stepchart;
	
	
	public double distanceRed;
	
	public SerializableSong(){
		mesureBPMS = new List<double>();
		mesureSTOPS = new List<double>();
		bpmsKey = new List<double>();
		bpmsValue = new List<double>();
		stopsKey = new List<double>();
		stopsValue = new List<double>();
		stepchart = new List<List<string>>();
		intensityGraph = new double[100];
	}
	
	public void transfertLoad(Song s){
		s.title = this.title;
		s.subtitle = this.subtitle;
		s.artist = this.artist;
		s.banner = this.banner;
		s.bpmToDisplay = this.bpmToDisplay;
		s.background = this.background;
		s.offset = this.offset;
		s.samplestart = this.samplestart;
		s.samplelenght = this.samplelenght;
		s.mesureBPMS = this.mesureBPMS;
		s.mesureSTOPS = this.mesureSTOPS;

		s.bpms = new Dictionary<double, double>();
		for(int i=0;i<this.bpmsKey.Count;i++){
			s.bpms.Add(this.bpmsKey[i], this.bpmsValue[i]);
		}
		
		s.stops = new Dictionary<double, double>();
		for(int i=0;i<this.stopsKey.Count;i++){
			s.stops.Add(this.stopsKey[i], this.stopsValue[i]);
		}
		
		s.stepartist = this.stepartist;
		s.difficulty = this.difficulty;
		s.level = this.level;
		s.numberOfSteps = this.numberOfSteps;
		s.numberOfStepsWithoutJumps = this.numberOfStepsWithoutJumps;
		s.numberOfStepsAbsolute = this.numberOfStepsAbsolute;
		s.numberOfFreezes = this.numberOfFreezes;
		s.numberOfRolls = this.numberOfRolls;
		s.numberOfMines = this.numberOfMines;
		s.numberOfJumps = this.numberOfJumps;
		s.numberOfHands = this.numberOfHands;
		s.duration = this.duration;
		s.intensityGraph = this.intensityGraph;
		
		s.stepPerSecondAverage = this.stepPerSecondAverage;
		s.stepPerSecondMaximum = this.stepPerSecondMaximum;
		s.stepPerSecondStream = this.stepPerSecondStream;
		s.longestStream = this.longestStream;
		s.numberOfCross = this.numberOfCross;
		s.numberOfFootswitch = this.numberOfFootswitch;
		s.song = this.song;
		s.sip = this.sip;
		s.stepchart = this.stepchart;
		s.distanceRed = this.distanceRed;
	}
	
	public void transfertSave(Song s, string packname, string songname){
		this.packName = packname;
		this.songFileName = songname;
		this.title = s.title;
		this.subtitle = s.subtitle;
		this.artist = s.artist;
		this.banner = s.banner;
		this.bpmToDisplay = s.bpmToDisplay;
		this.background = s.background;
		this.offset = s.offset;
		this.samplestart = s.samplestart;
		this.samplelenght = s.samplelenght;
		this.mesureBPMS = s.mesureBPMS;
		this.mesureSTOPS = s.mesureSTOPS;
		
		this.stepartist = s.stepartist;
		this.difficulty = s.difficulty;
		
		for(int i=0; i<s.bpms.Count; i++){
			this.bpmsKey.Add(s.bpms.ElementAt(i).Key);
			this.bpmsValue.Add(s.bpms.ElementAt(i).Value);
		}
		
		for(int i=0; i<s.stops.Count; i++){
			this.stopsKey.Add(s.stops.ElementAt(i).Key);
			this.stopsValue.Add(s.stops.ElementAt(i).Value);
		}
		
		this.level = s.level;
		this.numberOfSteps = s.numberOfSteps;
		this.numberOfStepsWithoutJumps = s.numberOfStepsWithoutJumps;
		this.numberOfStepsAbsolute = s.numberOfStepsAbsolute;
		this.numberOfFreezes = s.numberOfFreezes;
		this.numberOfRolls = s.numberOfRolls;
		this.numberOfMines = s.numberOfMines;
		this.numberOfJumps = s.numberOfJumps;
		this.numberOfHands = s.numberOfHands;
		this.duration = s.duration;
		this.intensityGraph = s.intensityGraph;
		this.stepPerSecondAverage = s.stepPerSecondAverage;
		this.stepPerSecondMaximum = s.stepPerSecondMaximum;
		this.stepPerSecondStream = s.stepPerSecondStream;
		this.longestStream = s.longestStream;
		this.numberOfCross = s.numberOfCross;
		this.numberOfFootswitch = s.numberOfFootswitch;
		this.song = s.song;
		this.sip = s.sip;
		this.stepchart = s.stepchart;
		this.distanceRed = s.distanceRed;
	}
	
	
	
	
}