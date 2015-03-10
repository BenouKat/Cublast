using UnityEngine;
using System.Collections;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System;

public class OpenChart{
	

	private static OpenChart instance;
	
	public static OpenChart Instance{
		get{
			if(instance == null){
			instance = new OpenChart();	
			}
			return instance;
		
		}
	}
	
	private OpenChart(){
	
	}
	
	
	private static string songContent;
	
	
	public Dictionary<Difficulty, Song> readChart(string directory){
		
		
		
		//read file
		
		var files = (string[]) Directory.GetFiles(directory);
		
		var stream = files.FirstOrDefault(c => (c.ToLower().Contains(".sm")) && !c.ToLower().Contains(".old")  && !c.ToLower().Contains(".dwi") && !c.Contains("._"));
		if(stream == null) stream = files.FirstOrDefault(c => (c.ToLower().Contains(".dwi"))   && !c.ToLower().Contains(".old") && !c.Contains("._"));
		if(stream == null) return null;
		StreamReader sr = new StreamReader(stream);
		songContent = sr.ReadToEnd();
    	sr.Close();
		
		//split all line and put on a list
		var thesong = songContent.Split(new string[] { Environment.NewLine }, StringSplitOptions.None);
		List<string> listLine = new List<string>();
		listLine.AddRange(thesong);
		List<int> indexNotes = new List<int>();
		
		//traitement des commentaires.
		/* Non : Impossible de savoir si single ou double :(
		for(int i=0; i < listLine.Count;i++){
			if(listLine.ElementAt(i).Contains("//")){
				listLine[i] = listLine.ElementAt(i).Split("//".ToCharArray())[0];
			}
		}*/
		
		for(int i=0; i < listLine.Count;i++){
			if(listLine.ElementAt(i).Contains("NOTES")){
				indexNotes.Add(i);
			}
		}
		
		Dictionary<Difficulty, Song>  outputSongs = new Dictionary<Difficulty, Song> ();
		
		try{
			//get generic information
			var thetitle = listLine.FirstOrDefault(c => c.Contains("TITLE")).Split(':');
			var title = thetitle[1].Replace(";","");
			var thesubtitle = listLine.FirstOrDefault(c => c.Contains("SUBTITLE")).Split(':');
			var subtitle = thesubtitle[1].Replace(";","");
			var theartist = listLine.FirstOrDefault(c => c.Contains("ARTIST")).Split(':');
			var artist = theartist[1].Replace(";","");
			var theoffset = listLine.FirstOrDefault(c => c.Contains("OFFSET")).Split(':');
			if(theoffset[1].Contains("//")) theoffset[1] = theoffset[1].Split('/')[0]; //Special Destination fix
			var offset = System.Convert.ToDouble(theoffset[1].Replace(";",""));
			var thesamplestart = listLine.FirstOrDefault(c => c.Contains("SAMPLESTART")).Split(':');
			var samplestart = System.Convert.ToDouble(thesamplestart[1].Replace(";",""));
			var thesamplelenght = listLine.FirstOrDefault(c => c.Contains("SAMPLELENGTH")).Split(':');
			var samplelenght = System.Convert.ToDouble(thesamplelenght[1].Replace(";",""));
			//"file://" + 
			var theBanner = listLine.FirstOrDefault(c => c.Contains("BANNER")).Split(':');
			var bannerTemp = "";
			if(!String.IsNullOrEmpty(theBanner[1].Replace(";","")))
			{
				bannerTemp = files.FirstOrDefault(c => c.Contains(theBanner[1].Replace(";","")));
			}else{
				bannerTemp = files.FirstOrDefault(c => (c.ToLower().Contains("bn") || c.ToLower().Contains("banner")) && (c.ToLower().Contains(".png") || c.ToLower().Contains(".jpg") || c.ToLower().Contains(".bmp") || c.ToLower().Contains(".jpeg")));	
			}
			var banner = "noBanner";
			if(!String.IsNullOrEmpty(bannerTemp)){
				banner = "file://" +  bannerTemp.Replace('\\', '/');
			}
			Dictionary<double, double> ReferenceBPMList = new Dictionary<double, double>();
			Dictionary<double, double> ReferenceSTOPList = new Dictionary<double, double>();
			
			Dictionary<double, double> BPMListInTime = new Dictionary<double, double>();
			Dictionary<double, double> STOPListInTime = new Dictionary<double, double>();
			
			List<double> BPMListInMesure = new List<double>();
			List<double> STOPListInMesure = new List<double>();
			
			
			
			//getting bpms with mesure
			string[] thebpm = listLine.FirstOrDefault(c => c.Contains("BPMS")).Split(':');
			string thebpmline = thebpm[1];
			string[] splitbpm = thebpmline.Split(',');
			/*double previousbps = 0;
			double previoustime = 0;
			double previousmesure = 0;
			List<double> timeToMesureBPM = new List<double>();*/
			foreach(string s in splitbpm){
				string[] mysplit = s.Replace(";","").Split('=');
				if(!ReferenceBPMList.ContainsKey(System.Convert.ToDouble(mysplit[0]))) ReferenceBPMList.Add(System.Convert.ToDouble(mysplit[0]), System.Convert.ToDouble(mysplit[1]));
				//theBpmList.Add(previousbps == 0 ? 0 : previoustime + (System.Convert.ToDouble(mysplit[0]) - previousmesure)/previousbps, System.Convert.ToDouble(mysplit[1]));
				/*previousbps = System.Convert.ToDouble(mysplit[1])/(double)60.0;
				previoustime = theBpmList.Last().Key;
				previousmesure = System.Convert.ToDouble(mysplit[0]);
				timeToMesureBPM.Add(System.Convert.ToDouble(mysplit[0]));
				Debug.Log ("bpm : " + theBpmList.Last().Key);*/
			}
			
			//getting stops mesured
			
			int stopbegin = listLine.IndexOf(listLine.FirstOrDefault(c => c.Contains("STOPS")));
			string thestop = "";
			bool breaker = false;
			if(!listLine.ElementAt(stopbegin).Contains("STOPS:;")){
				for(int i=stopbegin; !breaker; i++){
					if( listLine.ElementAt(i).Contains("//")){
						thestop += listLine.ElementAt(i).Split('/')[0];
					}else{
						thestop += listLine.ElementAt(i);
					}
					
					if(listLine.ElementAt(i).Contains(";")) breaker = true;
				}
				thestop = thestop.Replace("/n", "");
				thestop = thestop.Replace(";", "");
			}
			
			
			if(thestop != ""){
				string[] thestopsplit = thestop.Split(':');
				string thestopline = thestopsplit[1];
				string[] splitstop = thestopline.Split(',');
				/*previousbps = theBpmList.First().Value;
				previoustime = 0;
				previousmesure = 0;*/
				foreach(string s in splitstop){
	
					var mysplit = s.Split('=');
					
					ReferenceSTOPList.Add(System.Convert.ToDouble(mysplit[0]), System.Convert.ToDouble(mysplit[1]));
					/*
					var theMesure = timeToMesureBPM.IndexOf(timeToMesureBPM.FirstOrDefault(d => d == timeToMesureBPM.Where(c => c <= System.Convert.ToDouble(mysplit[0])).Max()));
					previousbps = System.Convert.ToDouble(theBpmList.ElementAt(theMesure).Value)/(double)60.0;
					previoustime = theBpmList.ElementAt(theMesure).Key;
					previousmesure = timeToMesureBPM.ElementAt(theMesure);
					
					theStopList.Add(previoustime + ((System.Convert.ToDouble(mysplit[0])) - previousmesure)/previousbps, System.Convert.ToDouble(mysplit[1]));
					
					Debug.Log("real mesure : " + (System.Convert.ToDouble(mysplit[0])) + " analyse : " + theStopList.Last().Key + " : " + theStopList.Last().Value + " : " + 
						theMesure + " previous time : " + previoustime + " previous mesure : " + 
						previousmesure + " previous bps " + previousbps + " (" + previousbps*60.0 + ") bpm");*/
				}
			}
			
			//change bpm and stops mesured by time
			double previousbps = 0;
			double stoptime = 0;
			double previoustime = 0;
			double previousmesure = 0;
			
			while(ReferenceSTOPList.Count != 0 && ReferenceBPMList.Count != 0){
				if((ReferenceSTOPList.First().Key < ReferenceBPMList.First().Key)){
					
					
					STOPListInTime.Add(previoustime + stoptime + (ReferenceSTOPList.First().Key - previousmesure)/previousbps, ReferenceSTOPList.First().Value);
					STOPListInMesure.Add(ReferenceSTOPList.First().Key);
					
					previoustime += (ReferenceSTOPList.First().Key - previousmesure)/previousbps;
					previousmesure = ReferenceSTOPList.First().Key;
					stoptime += ReferenceSTOPList.First().Value;
					
					
					ReferenceSTOPList.Remove(ReferenceSTOPList.First().Key);
				
				
				}else if((ReferenceSTOPList.First().Key > ReferenceBPMList.First().Key)){
					
					
					BPMListInTime.Add(previousbps == 0 ? 0 : previoustime + stoptime + (ReferenceBPMList.First().Key - previousmesure)/previousbps, ReferenceBPMList.First().Value);
					BPMListInMesure.Add(ReferenceBPMList.First().Key);
					
					previoustime += (previousbps == 0 ? 0 : (ReferenceBPMList.First().Key - previousmesure)/previousbps);
					previousbps = BPMListInTime.Last().Value/(double)60.0;
					previousmesure = ReferenceBPMList.First().Key;
					
					
					ReferenceBPMList.Remove(ReferenceBPMList.First().Key);
					
				}else if(ReferenceSTOPList.First().Key == ReferenceBPMList.First().Key){
					
					
					STOPListInTime.Add(previousbps == 0 ? 0 : previoustime + stoptime + (ReferenceSTOPList.First().Key - previousmesure)/previousbps, ReferenceSTOPList.First().Value);
					STOPListInMesure.Add(ReferenceSTOPList.First().Key);
					
					BPMListInTime.Add(previousbps == 0 ? 0 : previoustime + stoptime + (ReferenceBPMList.First().Key - previousmesure)/previousbps, ReferenceBPMList.First().Value);
					BPMListInMesure.Add(ReferenceBPMList.First().Key);
					
					previoustime += (previousbps == 0 ? 0 : (ReferenceBPMList.First().Key - previousmesure)/previousbps);
					previousbps = BPMListInTime.Last().Value/(double)60.0;
					previousmesure = ReferenceBPMList.First().Key;
					stoptime += ReferenceSTOPList.First().Value;
					
					
					
					ReferenceSTOPList.Remove(ReferenceSTOPList.First().Key);
					ReferenceBPMList.Remove(ReferenceBPMList.First().Key);
				}
				
			}
			
			while(ReferenceSTOPList.Count != 0){
	
					
					STOPListInTime.Add(previoustime + stoptime + (ReferenceSTOPList.First().Key - previousmesure)/previousbps, ReferenceSTOPList.First().Value);
					STOPListInMesure.Add(ReferenceSTOPList.First().Key);
					
					previoustime += (ReferenceSTOPList.First().Key - previousmesure)/previousbps;
					previousmesure = ReferenceSTOPList.First().Key;
					stoptime += ReferenceSTOPList.First().Value;
					
					
					ReferenceSTOPList.Remove(ReferenceSTOPList.First().Key);
				
			}
			
			while(ReferenceBPMList.Count != 0){
				
					
					BPMListInTime.Add(previousbps == 0 ? 0 : previoustime + stoptime + (ReferenceBPMList.First().Key - previousmesure)/previousbps, ReferenceBPMList.First().Value);
					BPMListInMesure.Add(ReferenceBPMList.First().Key);
					
					previoustime += (previousbps == 0 ? 0 : (ReferenceBPMList.First().Key - previousmesure)/previousbps);
					previousbps = BPMListInTime.Last().Value/(double)60.0;
					previousmesure = ReferenceBPMList.First().Key;
					
					
					ReferenceBPMList.Remove(ReferenceBPMList.First().Key);
					
			}
			
			
			var displayBPM = "";
			if(listLine.FirstOrDefault(c => c.Contains("DISPLAYBPM")) != null){
				try{
					string[] thes = listLine.FirstOrDefault(c => c.Contains("DISPLAYBPM")).Split(':');
					if(thes.Count() > 2){
						displayBPM = System.Convert.ToDouble(thes[1].Replace(";", "")).ToString("0") + " -> " + System.Convert.ToDouble(thes[2].Replace(";", "")).ToString("0");
					}else{
						displayBPM = System.Convert.ToDouble(thes[1].Replace(";", "")).ToString("0");
					}
				}catch{ //Special Gravity Blast fix
					string[] thes = listLine.FirstOrDefault(c => c.Contains("DISPLAYBPM")).Split(':');
					var theindex = 0;
					for(int o=0; o<thes.Count(); o++){
						if(thes[o].Contains("DISPLAYBPM")){
							theindex = o + 1;
						}
					}
					if((thes.Count() - theindex - 1) > 2){
						displayBPM = System.Convert.ToDouble(thes[theindex].Replace(";", "")).ToString("0") + " -> " + System.Convert.ToDouble(thes[theindex+1].Replace(";", "")).ToString("0");
					}else{
						displayBPM = System.Convert.ToDouble(thes[theindex].Replace(";", "")).ToString("0");
					}
				}
			}else{
				var themin = BPMListInTime.Min(c => c.Value);
				var themax = BPMListInTime.Max(c => c.Value);
				if(themin == themax){
					displayBPM = themax.ToString("0");	
				}else{
					displayBPM = themin.ToString("0") + " -> " + themax.ToString("0");
				}
			}
			
			//debug
			/*foreach(var el in theStopList){
				Debug.Log(el.Key);	
			}*/
			
			//For all difficulties
			foreach(int index in indexNotes){
			
				var theNewsong = new Song();
				
				//easy variable getted
				theNewsong.title = title;
				theNewsong.subtitle = subtitle;
				theNewsong.artist = artist;
				theNewsong.banner = banner;
				theNewsong.offset = offset;
				theNewsong.samplestart = samplestart;
				theNewsong.samplelenght = samplelenght;
				theNewsong.bpms = BPMListInTime;
				theNewsong.stops = STOPListInTime;
				theNewsong.mesureBPMS = BPMListInMesure;
				theNewsong.mesureSTOPS = STOPListInMesure;
				theNewsong.bpmToDisplay = displayBPM;
				/*if(files.FirstOrDefault(c => c.Contains(".ogg")) == null){
					foreach(var fil in files){
						Debug.Log(fil);	
					}
				}*/
				theNewsong.song = "file://" + files.FirstOrDefault(c => c.Contains(".ogg") || c.Contains(".OGG") || c.Contains(".mp3") || c.Contains(".MP3")).Replace('\\', '/');
				
				
				//getting song information
				int beginInformation = index;
				string dl = "";
				var theinfo = listLine.ElementAt(beginInformation + 1).Replace(":","").Trim();

				if(!theinfo.Contains("single") || theinfo.Contains("double") || theinfo.Contains("pump") || theinfo.Contains("ez2") || 
					theinfo.Contains("para") || theinfo.Contains("ds3ddx") || theinfo.Contains("pnm") || 
					theinfo.Contains("bm") || theinfo.Contains("maniax") || theinfo.Contains("techno"))
				{
					dl = "STOP";
				}
				theNewsong.stepartist = listLine.ElementAt(beginInformation + 2).Replace(":","").Trim();
				theNewsong.setDifficulty(dl + listLine.ElementAt(beginInformation + 3).Replace(":","").Trim());
				theNewsong.level = System.Convert.ToInt32(listLine.ElementAt(beginInformation + 4).Replace(":","").Trim());
				
				
				//getting stepchart
				int beginstepchart = beginInformation+6;
				while(listLine.ElementAt(beginstepchart).Contains("//") || 
					String.IsNullOrEmpty(listLine.ElementAt(beginstepchart).Trim()) || 
					listLine.ElementAt(beginstepchart) == ""){
					
						if(listLine.ElementAt(beginstepchart).Contains("NOTES")) dl = "STOP";
						beginstepchart++;	
				}
				
				if(listLine.ElementAt(beginstepchart).Contains("NOTES")) dl = "STOP";
				//if(theNewsong.title == "The Last Kiss") Debug.Log(listLine.ElementAt(beginstepchart));
				
				if(dl != "STOP"){
					int numberOfSteps = 0;
					int numberOfMines = 0;
					int numberOfRoll = 0;
					int numberOfFreezes = 0;
					int numberOfJump = 0;
					int numberOfStepsWJ = 0;
					int numberOfStepsAbs = 0;
					theNewsong.stepchart.Add(new List<string>());
					for(int i = beginstepchart; !listLine.ElementAt(i).Contains(";"); i++){
						if(listLine.ElementAt(i).Contains(",")){
							theNewsong.stepchart.Add(new List<string>());
						}else if(!String.IsNullOrEmpty(listLine.ElementAt(i))){
							theNewsong.stepchart.Last().Add(listLine.ElementAt(i));	
							numberOfSteps += listLine.ElementAt(i).Count(c => c == '1');
							numberOfSteps += listLine.ElementAt(i).Count(c => c == '2');
							numberOfSteps += listLine.ElementAt(i).Count(c => c == '4');
							numberOfFreezes += listLine.ElementAt(i).Count(c => c == '2');
							numberOfRoll += listLine.ElementAt(i).Count(c => c == '4');
							numberOfMines += listLine.ElementAt(i).Count(c => c == 'M');
							numberOfStepsWJ += listLine.ElementAt(i).Count(c => c == '1');
							numberOfStepsWJ += listLine.ElementAt(i).Count(c => c == '2');
							numberOfStepsWJ += listLine.ElementAt(i).Count(c => c == '4');
							numberOfStepsAbs += listLine.ElementAt(i).Count(c => c == '1');
							numberOfStepsAbs += listLine.ElementAt(i).Count(c => c == '2');
							numberOfStepsAbs += listLine.ElementAt(i).Count(c => c == '4');
							
							var countmesure = listLine.ElementAt(i).Count(c => c == '1') + listLine.ElementAt(i).Count(c => c == '2') + listLine.ElementAt(i).Count(c => c == '4');
							if(countmesure == 2){
								numberOfStepsWJ -= countmesure;
								numberOfStepsAbs -= countmesure - 1;
								numberOfJump++;
							}
							if(countmesure >= 3){
								numberOfStepsWJ -= countmesure;
								numberOfStepsAbs -= countmesure - 1;
								//numberOfStepsAbs++;
							}
						}
					}
					
					theNewsong.numberOfSteps = numberOfSteps;
					theNewsong.numberOfFreezes = numberOfFreezes;
					theNewsong.numberOfRolls = numberOfRoll;
					theNewsong.numberOfMines = numberOfMines;
					theNewsong.numberOfJumps = numberOfJump;
					theNewsong.numberOfStepsWithoutJumps = numberOfStepsWJ;
					theNewsong.numberOfStepsAbsolute = numberOfStepsAbs;

					if(dl == "") fakeCreation(theNewsong);
						//A mettre
					//if(outputSongs.ContainsKey(theNewsong.difficulty))
					theNewsong.sip = new SongInfoProfil(theNewsong.title, theNewsong.subtitle, 
						theNewsong.numberOfSteps, theNewsong.difficulty, theNewsong.level);
					outputSongs.Add(theNewsong.difficulty, theNewsong);
				}
			
			}
		}catch(Exception e){
			Debug.Log(directory + " // " + e.Message + " //st : " + e.StackTrace);	
		}
		
		return outputSongs;
		
	}
	
	
	void fakeCreation(Song s){
				
		var theBPMCounter = 1;
		var theSTOPCounter = 0;
		double mesurecount = 0;
		double prevmesure = 0;
		double timecounter = 0;
		double timeBPM = 0;
		double timestop = 0;
		double timetotal = 0;
		float prec = 0.001f;
		
		//stepBySecondsAverage
		double timestart = -10f;
		double stoptime = 0;
		int countStep = 0;
		double stepbysecAv = 0f;
		
		
		//stepmax
		double maxStepPerSeconds = 0f;
		int numberStepBetweenTwoBeat = 0;
		double timestartMax = -10f;
		//double maxLenght = 0f;
		
		
		//longestStream
		double lenghtStream = 0f;
		double maxLenghtStream = 0f;
		double speedOfMaxStream = 0f;
		double previousSpeed = 0f;
		
		
		//Footswitch
		int numberOfFootswitch = 0;
		int[] casevalidate = new int[4]; //left down up right
		casevalidate[0] = 0;
		casevalidate[1] = 0;
		casevalidate[2] = 0;
		casevalidate[3] = 0;
		
		//Cross
		int numberOfCross = 0;
		int[] caseCrossvalidate = new int[4]; //left down up right
		caseCrossvalidate[0] = 0;
		caseCrossvalidate[1] = 0;
		caseCrossvalidate[2] = 0;
		caseCrossvalidate[3] = 0;
		
		//Hands
		int numberOfHands = 0;
		
		//freeze
		var freezed = new int[4];
		freezed[0] = 0;
		freezed[1] = 0;
		freezed[2] = 0;
		freezed[3] = 0;
		
		//graph
		var listNumberStep = new Dictionary<double, double>();
		
		foreach(var mesure in s.stepchart){
			
			for(int beat=0;beat<mesure.Count;beat++){
			
			#region BPMChange
				var bps = s.getBPS(s.bpms.ElementAt(theBPMCounter-1).Value);
				if(theBPMCounter < s.bpms.Count && theSTOPCounter < s.stops.Count){
					while((theBPMCounter < s.bpms.Count && theSTOPCounter < s.stops.Count) 
						&& (s.mesureBPMS.ElementAt(theBPMCounter) < mesurecount - prec || s.mesureSTOPS.ElementAt(theSTOPCounter) < mesurecount - prec)){
					
						if(s.mesureBPMS.ElementAt(theBPMCounter) < s.mesureSTOPS.ElementAt(theSTOPCounter)){
							timecounter += (s.mesureBPMS.ElementAt(theBPMCounter) - prevmesure)/bps;
							
							timeBPM += timecounter;
							timecounter = 0;
							prevmesure = s.mesureBPMS.ElementAt(theBPMCounter);
							theBPMCounter++;
							bps = s.getBPS(s.bpms.ElementAt(theBPMCounter-1).Value);
							//Debug.Log("And bpm change before / bpm");
						}else if(s.mesureBPMS.ElementAt(theBPMCounter) > s.mesureSTOPS.ElementAt(theSTOPCounter)){
							timestop += s.stops.ElementAt(theSTOPCounter).Value;
							theSTOPCounter++;
							//Debug.Log("And stop change before");
						}else{
							timecounter += (s.mesureBPMS.ElementAt(theBPMCounter) - prevmesure)/bps;
							timeBPM += timecounter;
							timecounter = 0;
							prevmesure = s.mesureBPMS.ElementAt(theBPMCounter);
							theBPMCounter++;
							bps = s.getBPS(s.bpms.ElementAt(theBPMCounter-1).Value);
							
							timestop += s.stops.ElementAt(theSTOPCounter).Value;
							theSTOPCounter++;
							//Debug.Log("And bpm change before");
							//Debug.Log("And stop change before");
						}
						
					}
				}else if(theBPMCounter < s.bpms.Count){
					while((theBPMCounter < s.bpms.Count) && s.mesureBPMS.ElementAt(theBPMCounter) < mesurecount - prec){
						
						timecounter += (s.mesureBPMS.ElementAt(theBPMCounter) - prevmesure)/bps;
							
						timeBPM += timecounter;
						timecounter = 0;
						prevmesure = s.mesureBPMS.ElementAt(theBPMCounter);
						theBPMCounter++;
						bps = s.getBPS(s.bpms.ElementAt(theBPMCounter-1).Value);
					
					}
				}else if(theSTOPCounter < s.stops.Count){
					while((theSTOPCounter < s.stops.Count) && s.mesureSTOPS.ElementAt(theSTOPCounter) < mesurecount - prec){
						
						timestop += s.stops.ElementAt(theSTOPCounter).Value;
						theSTOPCounter++;
					
					}
				}
				
				#endregion
				timecounter += (mesurecount - prevmesure)/bps;
				
				
				timetotal = timecounter + timeBPM + timestop;
				
				char[] note = mesure.ElementAt(beat).TrimStart(' ').Trim().ToCharArray();
				
				if((beat*8f)%(mesure.Count) == 0){
					var newMax = numberStepBetweenTwoBeat/(timetotal - timestartMax);
					if(!listNumberStep.ContainsKey(timetotal)){
						listNumberStep.Add(timetotal, newMax);
					}else{
						listNumberStep.Add(timetotal + 0.00001, newMax);
					}
					if(maxStepPerSeconds < newMax){
						maxStepPerSeconds = newMax;
					}
					
					
					
					if(Mathf.Abs((float)(newMax - previousSpeed)) < 0.001f && numberStepBetweenTwoBeat > 0 && newMax > 4f){
						
						lenghtStream += (timetotal - timestartMax);
						if(lenghtStream > maxLenghtStream){
							speedOfMaxStream = newMax;
							maxLenghtStream = lenghtStream;
						}
					}else{
						lenghtStream = 0;
					}
					
					previousSpeed = newMax;
					
					numberStepBetweenTwoBeat = 0;
					timestartMax = timetotal;
				}
				var barr = false;
				var barrstop = false;
				var iselected = -1;
				var doubleselection = false;
				var tripleselect = 0;
				
				
				for(int i =0;i<4; i++){
					
					if(note[i] == '1'){
						if(iselected == -1){
							iselected = i;
						}else{
							doubleselection = true;
						}
						tripleselect++;
						barr = true;
						
					}else if(note[i] == '2'){
						barr = true;
						if(iselected == -1){
							iselected = i;
						}else{
							doubleselection = true;
						}
						freezed[i] = 1;
					}else if(note[i] == '3'){
						freezed[i] = 0;
						barrstop = true;
					}else if(note[i] == '4'){
						barr = true;
						if(iselected == -1){
							iselected = i;
						}else{
							doubleselection = true;
						}
						freezed[i] = 1;
					}else if(note[i] == 'M'){
						barrstop = true;
					}
					
				}
				
				for(int i=0;i<4;i++){
					if(freezed[i] == 1 && barr) tripleselect++;
				}
				
				if(tripleselect >= 3f){
					numberOfHands++;
				}
				if(barr || barrstop){
					stoptime = timetotal;	
				}
				
				if(barr){
				
					if(timestart == -10f) timestart = timetotal;
					
					countStep++;
					numberStepBetweenTwoBeat++;
					
					if(!doubleselection){
						switch(iselected){
							case 0:
							//fs
								if(((casevalidate[1] == 2 && casevalidate[2] == 0) || (casevalidate[2] == 2 && casevalidate[1] == 0)) && casevalidate[0] == 0 && casevalidate[3] == 1){
									numberOfFootswitch++;
									casevalidate[0] = 1;
									casevalidate[1] = 0;
									casevalidate[2] = 0;
									casevalidate[3] = 0;
								}else{
									casevalidate[0] = 1;
									casevalidate[1] = 0;
									casevalidate[2] = 0;
									casevalidate[3] = 0;
								}
							//cross
								if((caseCrossvalidate[0] == 0 && (((caseCrossvalidate[1] == 1 || caseCrossvalidate[1] == 2) && caseCrossvalidate[2] == 0) 
								|| (caseCrossvalidate[1] == 0 && (caseCrossvalidate[2] == 1 || caseCrossvalidate[2] == 2))) && caseCrossvalidate[3] == 1)){
									numberOfCross++;
									caseCrossvalidate[0] = 1;
									caseCrossvalidate[1] = 0;
									caseCrossvalidate[2] = 0;
									caseCrossvalidate[3] = 0;
								}else{
									caseCrossvalidate[0] = 1;
									caseCrossvalidate[1] = 0;
									caseCrossvalidate[2] = 0;
									caseCrossvalidate[3] = 0;
								}
								break;
							case 1:
							//fs
								if(((casevalidate[0] == 0 && casevalidate[3] == 1) || (casevalidate[0] == 1 && casevalidate[3] == 0)) && casevalidate[2] == 0 &&  casevalidate[1] < 2){
									casevalidate[1]++;
								}else{
									casevalidate[0] = 0;
									casevalidate[1] = 0;
									casevalidate[2] = 0;
									casevalidate[3] = 0;
								}
							//cross
								if((caseCrossvalidate[0] == 1 || caseCrossvalidate[3] == 1)){
									if(caseCrossvalidate[0] == 1 && caseCrossvalidate[1] == 1 && caseCrossvalidate[2] == 2 && caseCrossvalidate[3] == 0){
										caseCrossvalidate[2] = 0;
									}
									if(caseCrossvalidate[0] == 0 && caseCrossvalidate[1] == 1 && caseCrossvalidate[2] == 2 && caseCrossvalidate[3] == 1){
										caseCrossvalidate[2] = 0;
									}
								
									caseCrossvalidate[1] = 2;
									if(caseCrossvalidate[2] == 2) caseCrossvalidate[2] = 1;
								
								}else{
									caseCrossvalidate[0] = 0;
									caseCrossvalidate[1] = 0;
									caseCrossvalidate[2] = 0;
									caseCrossvalidate[3] = 0;
								}
								break;
							case 2:
							//fs
								if(((casevalidate[0] == 0 && casevalidate[3] == 1) || (casevalidate[0] == 1 && casevalidate[3] == 0)) && casevalidate[1] == 0 &&  casevalidate[2] < 2){
									casevalidate[2]++;
								}else{
									casevalidate[0] = 0;
									casevalidate[1] = 0;
									casevalidate[2] = 0;
									casevalidate[3] = 0;
								}
							//cross
								if((caseCrossvalidate[0] == 1 || caseCrossvalidate[3] == 1)){
									if(caseCrossvalidate[0] == 1 && caseCrossvalidate[1] == 2 && caseCrossvalidate[2] == 1 && caseCrossvalidate[3] == 0){
										caseCrossvalidate[1] = 0;
									}
									if(caseCrossvalidate[0] == 0 && caseCrossvalidate[1] == 2 && caseCrossvalidate[2] == 1 && caseCrossvalidate[3] == 1){
										caseCrossvalidate[1] = 0;
									}
								
									caseCrossvalidate[2] = 2;
									if(caseCrossvalidate[1] == 2) caseCrossvalidate[1] = 1;
									
									
								}else{
									caseCrossvalidate[0] = 0;
									caseCrossvalidate[1] = 0;
									caseCrossvalidate[2] = 0;
									caseCrossvalidate[3] = 0;
								}
								break;
							case 3:
							//fs
								if(((casevalidate[1] == 2 && casevalidate[2] == 0) || (casevalidate[2] == 2 && casevalidate[1] == 0)) && casevalidate[3] == 0 && casevalidate[0] == 1){
									numberOfFootswitch++;
									casevalidate[0] = 0;
									casevalidate[1] = 0;
									casevalidate[2] = 0;
									casevalidate[3] = 1;
								}else{
									casevalidate[0] = 0;
									casevalidate[1] = 0;
									casevalidate[2] = 0;
									casevalidate[3] = 1;
								}
							//cross
								if(caseCrossvalidate[0] == 1 && (( caseCrossvalidate[1] == 2 && caseCrossvalidate[2] == 0) 
									|| (caseCrossvalidate[1] == 0 &&  caseCrossvalidate[2] == 2) && caseCrossvalidate[3] == 0)){
									numberOfCross++;
									caseCrossvalidate[0] = 0;
									caseCrossvalidate[1] = 0;
									caseCrossvalidate[2] = 0;
									caseCrossvalidate[3] = 1;
								}else{
									caseCrossvalidate[0] = 0;
									caseCrossvalidate[1] = 0;
									caseCrossvalidate[2] = 0;
									caseCrossvalidate[3] = 1;
								}
								break;
						}
					}else{
						casevalidate[0] = 0;
						casevalidate[1] = 0;
						casevalidate[2] = 0;
						casevalidate[3] = 0;
						caseCrossvalidate[0] = 0;
						caseCrossvalidate[1] = 0;
						caseCrossvalidate[2] = 0;
						caseCrossvalidate[3] = 0;
					}
				}
				
					
				#region ChangeBPM
				if(theBPMCounter < s.bpms.Count){
					if(Mathf.Abs((float)(s.mesureBPMS.ElementAt(theBPMCounter) - mesurecount)) < prec){
							timeBPM += timecounter;
							timecounter = 0;
							theBPMCounter++;
							//Debug.Log("And bpm change");
					}
				}
				
				if(theSTOPCounter < s.stops.Count){
					if(Mathf.Abs((float)(s.mesureSTOPS.ElementAt(theSTOPCounter) - mesurecount)) < prec){
							timestop += s.stops.ElementAt(theSTOPCounter).Value;
							theSTOPCounter++;
							//Debug.Log("And stop");
					}
				}
				
				#endregion
				prevmesure = mesurecount;
				mesurecount += (4f/(float)mesure.Count);
				
			}
		}
		
		stepbysecAv = (double)countStep/(stoptime - timestart);
		s.duration = stoptime;
		s.stepPerSecondAverage = stepbysecAv;
		s.stepPerSecondMaximum = maxStepPerSeconds;
		//s.timeMaxStep = maxLenght;
		s.stepPerSecondStream = speedOfMaxStream;
		s.longestStream = maxLenghtStream;
		s.numberOfFootswitch = numberOfFootswitch;
		s.numberOfCross = numberOfCross - numberOfFootswitch;
		s.numberOfHands = numberOfHands;
		
		
		
		//Old Graph
		s.intensityGraph = new double[100];
		
		/*
		var theamountOfTime = s.duration/100f;
		double thetotaltime = 0;
		double themoy = 0;
		double thecountmoy = 0;
		var countintensity = 0;
		double theprevioustimetotal = 0;
		for(int i=0;i<listNumberStep.Count && thetotaltime < s.duration;i++){
			if((thetotaltime - theprevioustimetotal) >= theamountOfTime){
				var thevalue = themoy/thecountmoy;
				s.intensityGraph[countintensity] = (float)thevalue;
				countintensity++;
				thecountmoy = 0;
				themoy = 0;
				theprevioustimetotal = thetotaltime;
			}else{
				themoy += listNumberStep.ElementAt(i).Value;
				thecountmoy++;
				thetotaltime = listNumberStep.ElementAt(i).Key;
			}
		}*/
		
		double thelastgoodvalue = 0;
		var thecut = s.duration/(double)100;
		
		for(int i=0; i<100; i++){
			if(i == 0){
				s.intensityGraph[i] = 0;
			}else{
				if(listNumberStep.Where(c => c.Key <= thecut*i).Count() == 0){
					s.intensityGraph[i] = thelastgoodvalue;
				}else{
					double moy = 0;
					int numbermoy = 0;
					List<double> keyToRemove = new List<double>();
					foreach(var val in listNumberStep.Where(c => c.Key <= (thecut*i))){
						moy += val.Value;
						thelastgoodvalue = val.Value;
						keyToRemove.Add(val.Key);
						numbermoy++;
					}
					foreach(var rem in keyToRemove){
						listNumberStep.Remove(rem);	
					}
					s.intensityGraph[i] = moy/(double)numbermoy;
				}
				
			}
		}
		
		
	}
	
	
}
