using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Song {

	//global information
	public string title;
	public string subtitle;
	public string artist;
	public string banner;
	public string bpmToDisplay;
	public string background;
	public double offset;
	public double samplestart;
	public double samplelenght;
	public List<double> mesureBPMS;
	public List<double> mesureSTOPS;
	public Dictionary<double, double> bpms;
	public Dictionary<double, double> stops;
	public string stepartist;
	public Difficulty difficulty;
	public int level;
	public int numberOfSteps;
	public int numberOfStepsWithoutJumps;
	public int numberOfStepsAbsolute;
	public int numberOfFreezes;
	public int numberOfRolls;
	public int numberOfMines;
	public int numberOfJumps;
	public int numberOfHands;
	public double duration;
	public double[] intensityGraph;
	
	public double stepPerSecondAverage;
	public double stepPerSecondMaximum;
	public double stepPerSecondStream;
	public double longestStream;
	public int numberOfCross;
	public int numberOfFootswitch;
	public string song;
	
	public SongInfoProfil sip;
	
	//stepchart
	public List<List<string>> stepchart;
	
	
	public double distanceRed;
	
	public Song(){
		bpms = new Dictionary<double, double>();
		stops = new Dictionary<double, double>();
		stepchart = new List<List<string>>();
		
	}
	
	public Song getRatedSong(float rate){
		var ratedSong = new Song();
		
		ratedSong.title = this.title;
		ratedSong.subtitle = this.subtitle;
		ratedSong.artist = this.artist;
		ratedSong.banner = this.banner;
		ratedSong.bpmToDisplay = this.bpmToDisplay;
		ratedSong.background = this.background;
		ratedSong.offset = this.offset/(1f + (rate/100f));
		ratedSong.mesureBPMS = this.mesureBPMS;
		ratedSong.mesureSTOPS = this.mesureSTOPS;
		
		ratedSong.bpms = new Dictionary<double, double>(); 
		
		foreach(var bpm in this.bpms){
			ratedSong.bpms.Add(bpm.Key/(1f + (rate/100f)), bpm.Value*(1f + (rate/100f)));
		}
		
		ratedSong.stops = new Dictionary<double, double>();
		
		foreach(var stop in this.stops){
			ratedSong.stops.Add(stop.Key/(1f + (rate/100f)), stop.Value/(1f + (rate/100f)));
		}
		
		ratedSong.stepartist = this.stepartist;
		ratedSong.difficulty = this.difficulty;
		ratedSong.level = this.level;
		ratedSong.numberOfSteps = this.numberOfSteps;
		ratedSong.numberOfStepsWithoutJumps = this.numberOfStepsWithoutJumps;
		ratedSong.numberOfStepsAbsolute = this.numberOfStepsAbsolute;
		ratedSong.numberOfFreezes = this.numberOfFreezes;
		ratedSong.numberOfRolls = this.numberOfRolls;
		ratedSong.numberOfMines = this.numberOfMines;
		ratedSong.numberOfJumps = this.numberOfJumps;
		ratedSong.numberOfHands = this.numberOfHands;
		ratedSong.duration = this.duration/(1f + (rate/100f));

		ratedSong.stepPerSecondAverage = this.stepPerSecondAverage;
		ratedSong.stepPerSecondMaximum = this.stepPerSecondMaximum;
		ratedSong.stepPerSecondStream = this.stepPerSecondStream;
		ratedSong.longestStream = this.longestStream;
		ratedSong.numberOfCross = this.numberOfCross;
		ratedSong.numberOfFootswitch = this.numberOfFootswitch;
		ratedSong.song = this.song;

		ratedSong.sip = this.sip;
		
		ratedSong.stepchart = this.stepchart;
		
		return ratedSong;
	}
	
	/*
	public AudioClip SetAudioClip(AudioClip ac){
		var thewww = new WWW(song);
		while(!thewww.isDone){ }
		return thewww.GetAudioClip(false, true);
	}
	*/
	
	public Texture2D GetBanner(Texture2D tex){
		if(banner != "noBanner"){
			WWW www = new WWW(banner);
			while(!www.isDone){}
			
    		www.LoadImageIntoTexture(tex);
			
			www.Dispose();
			return tex;
			
		}
		return null;
	}
	
	public Texture2D GetBanner(){
		WWW www = new WWW(banner);
		while(!www.isDone){}
		var tex = new Texture2D(256,256);
		www.LoadImageIntoTexture(tex);
		
		www.Dispose();
		return tex;
	}
	
	public Texture2D GetBackground(Texture2D tex){
		if(background != "noBanner"){
			WWW www = new WWW(background);
			while(!www.isDone){}
			
    		www.LoadImageIntoTexture(tex);
			
			www.Dispose();
			return tex;
			
		}
		return null;
	}
	
	public Texture2D GetBackground(){
		WWW www = new WWW(background);
		while(!www.isDone){}
		var tex = new Texture2D(256,256);
		www.LoadImageIntoTexture(tex);
		
		www.Dispose();
		return tex;
	}
	
	public void setDifficulty(string dif){
		switch(dif){
		case "Challenge":
			difficulty = Difficulty.EXPERT;
			break;
		case "Hard":
			difficulty = Difficulty.HARD;
			break;
		case "Medium":
			difficulty = Difficulty.MEDIUM;
			break;
		case "Easy":
			difficulty = Difficulty.EASY;
			break;
		case "Beginner":
			difficulty = Difficulty.BEGINNER;
			break;
		case "Edit":
			difficulty = Difficulty.EDIT;
			break;
		default:
			difficulty = Difficulty.EDIT;
			break;
		}
	}
	
	
	
	
}