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

	private enum FileTags
	{
		//Header tags
		TITLE, SUBTITLE, ARTIST, OFFSET, SAMPLESTART, SAMPLELENGTH, HEADERTAG_LIMIT, 
		//Other string tags
		BANNER, DISPLAYBPM
	}
	
	private static string songContent;

	public string[] chartFileAllowed = new string[2] { ".sm", ".dwi" };
	public string[] chartFileRestricted = new string[2] { ".old", "._" };
	public string[] bannerFileAllowed = new string[4] { ".png", ".jpg", ".bmp", ".jpeg" };
	public string[] audioFileAllowed = new string[4] { ".ogg", ".OGG", ".mp3", ".MP3" };
	public string[] noteFileAllowed = new string[1] { "single" };
	public string[] noteFileRestricted = new string[9] { "double", "pump", "ez2", "para", "ds3ddx", "pnm", "bm", "maniax", "techno" };

	public bool isAllowedFile(string file, string[] allowedArray, string[] restrictedArray = null)
	{
		bool valid = false;
		for (int i=0; i<allowedArray.Length; i++) {
			if(file.Contains(allowedArray[i]))
			{
				valid = true;
				break;
			}
		}

		if (valid && restrictedArray != null) {
			for(int i=0; i<restrictedArray.Length; i++)
			{
				if(file.Contains(restrictedArray[i]))
				{
					valid = false;
					break;
				}
			}
		}

		return valid;
		
	}


	public Dictionary<Difficulty, Song> readChart(string directory){

		//read all file
		string[] files = (string[]) Directory.GetFiles(directory);
		
		string stream = files.FirstOrDefault(c => c.ToLower().Contains(".sm") && isAllowedFile(c.ToLower(), chartFileAllowed, chartFileRestricted));
		if(stream == null) stream = files.FirstOrDefault(c => c.ToLower().Contains(".dwi") && isAllowedFile(c.ToLower(), chartFileAllowed, chartFileRestricted));
		if(stream == null) return null;
		StreamReader sr = new StreamReader(stream);
		songContent = sr.ReadToEnd();
    	sr.Close();
		
		//split all line and put on a list
		string[] thesong = songContent.Split(new string[] { Environment.NewLine }, StringSplitOptions.None);
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
		Dictionary<FileTags, string> wordDictionary = new Dictionary<FileTags, string> ();

		//get generic information

		//Classic headers
		for (int i=0; i<System.Enum.GetValues(typeof(FileTags)).Length; i++) {
			string tag = listLine.FirstOrDefault(c => c.Contains(((FileTags)i).ToString()));
			string tagValue = "";
			if(tag != null && tag.Contains(':') && i < (int)FileTags.HEADERTAG_LIMIT)
			{
				string[] tagSplit = tag.Split(':');
				if(tagSplit.Length > 1)
				{
					//Remove comments
					if(tagSplit[1].Contains("//")) tagSplit[1] = tagSplit[1].Split('/')[0];
					tagValue = tagSplit[1].Replace(";", "");
				}
			}

			if(!wordDictionary.ContainsKey((FileTags)i))
				wordDictionary.Add((FileTags)i, tagValue);
		}

		//Others tags
		//BANNER
		string banner = listLine.FirstOrDefault(c => c.Contains("BANNER"));
		string bannerValue = "";
		string bannerDirectory = "";
		if (banner != null && banner.Contains(':'))
			bannerValue = banner.Split(':')[1].Replace(";", "");

		if(!String.IsNullOrEmpty(bannerValue))
		{
			bannerDirectory = files.FirstOrDefault(c => c.Contains(bannerValue));
		}else{
			bannerDirectory = files.FirstOrDefault(c => (c.ToLower().Contains("bn") || c.ToLower().Contains("banner")) 
			                                       && isAllowedFile (c.ToLower(), bannerFileAllowed));
		}

		if (!String.IsNullOrEmpty (bannerDirectory)) {
			bannerDirectory = "file://" + bannerDirectory.Replace ('\\', '/');
		}

		wordDictionary[FileTags.BANNER] = bannerDirectory;


		//BPM and STOP

		Dictionary<double, double> ReferenceBPMList = new Dictionary<double, double>();
		Dictionary<double, double> ReferenceSTOPList = new Dictionary<double, double>();
		
		Dictionary<double, double> BPMListInTime = new Dictionary<double, double>();
		Dictionary<double, double> STOPListInTime = new Dictionary<double, double>();
		
		List<double> BPMListInMesure = new List<double>();
		List<double> STOPListInMesure = new List<double>();
			
			
			
		//BPMS
		string BPMLine = listLine.FirstOrDefault (c => c.Contains ("BPMS"));
		string[] BPMLineSplit = (BPMLine != null && BPMLine.Contains (':')) ? BPMLine.Split (':') : null;
		string[] BPMLineList = new string[0];

		if (BPMLineSplit != null) BPMLineList = BPMLineSplit[1].Split(',');

		double BPMLineKey = 0;
		double BPMLineValue = 0;
		foreach(string bpmValue in BPMLineList){
			string[] BPMSeparator = bpmValue.Replace(";","").Split('=');

			if(System.Double.TryParse(BPMSeparator[0], out BPMLineKey) 
			   && System.Double.TryParse(BPMSeparator[1], out BPMLineValue))
			{
				if(!ReferenceBPMList.ContainsKey(BPMLineKey)) 
					ReferenceBPMList.Add(BPMLineKey, BPMLineValue);
			}
		}
			
		//STOPS
			
		string STOPLine = listLine.FirstOrDefault(c => c.Contains("STOPS"));
		if(STOPLine != null)
		{
			int indexSTOPLine = listLine.IndexOf(STOPLine);
			string STOPBuffer = "";
			if(!listLine.ElementAt(indexSTOPLine).Contains("STOPS:;")){
				bool exitLoop = false;
				while(!exitLoop){
					if( listLine.ElementAt(indexSTOPLine).Contains("//")){
						STOPBuffer += listLine.ElementAt(indexSTOPLine).Split('/')[0];
					}else{
						STOPBuffer += listLine.ElementAt(indexSTOPLine);
					}
					
					if(listLine.ElementAt(indexSTOPLine).Contains(";")) exitLoop = true;
					indexSTOPLine++;
				}
				STOPBuffer = STOPBuffer.Replace("/n", "");
				STOPBuffer = STOPBuffer.Replace(";", "");
			}
			
			
			if(!string.IsNullOrEmpty (STOPBuffer)){
				string[] STOPArraySplit = STOPBuffer.Split(':');
				string[] STOPArray = STOPArraySplit[1].Split(',');

				double STOPLineKey = 0;
				double STOPLineValue = 0;
				foreach(string stopValue in STOPArray){
	
					string[] STOPSeparator = stopValue.Split('=');

					if(System.Double.TryParse(STOPSeparator[0], out STOPLineKey) 
					   && System.Double.TryParse(STOPSeparator[1], out STOPLineValue))
					{
						if(!ReferenceSTOPList.ContainsKey(STOPLineKey)) 
						ReferenceSTOPList.Add(STOPLineKey, STOPLineValue);
					}
				}
			}
		}
			
		//Not changed from old code but it's seems okay
		double previousbps = 0;
		double stoptime = 0;
		double previoustime = 0;
		double previousmesure = 0;
		
		while(ReferenceSTOPList.Count != 0 || ReferenceBPMList.Count != 0){
			if(ReferenceBPMList.Count == 0 || (ReferenceSTOPList.First().Key < ReferenceBPMList.First().Key)){
				
				
				STOPListInTime.Add(previoustime + stoptime + (ReferenceSTOPList.First().Key - previousmesure)/previousbps, ReferenceSTOPList.First().Value);
				STOPListInMesure.Add(ReferenceSTOPList.First().Key);
				
				previoustime += (ReferenceSTOPList.First().Key - previousmesure)/previousbps;
				previousmesure = ReferenceSTOPList.First().Key;
				stoptime += ReferenceSTOPList.First().Value;

				ReferenceSTOPList.Remove(ReferenceSTOPList.First().Key);
			
			
			}else if(ReferenceSTOPList.Count != 0 || (ReferenceSTOPList.First().Key > ReferenceBPMList.First().Key)){
				
				
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
			
			
		string displayBPM = listLine.FirstOrDefault(c => c.Contains("DISPLAYBPM"));
		string displayBPMValue = "";
		if(displayBPM != null){
			string[] displayBPMSplit = displayBPM.Split(':');
			double displayBPMFirstValue = 0;
			double displayBPMSecondValue = 0;
			bool displayBPMSucceed = false;

			if(System.Double.TryParse(displayBPMSplit[1].Replace(";", ""), out displayBPMFirstValue))
			{
				if(displayBPMSplit.Count() > 2)
				{
					if(System.Double.TryParse(displayBPMSplit[2].Replace(";", ""), out displayBPMSecondValue))
					{
						displayBPMValue = displayBPMFirstValue.ToString("0") + "$" + displayBPMSecondValue.ToString("0");
						displayBPMSucceed = true;
					}
				}else{
					displayBPMValue = displayBPMFirstValue.ToString("0");
					displayBPMSucceed = true;
				}
			}

			//Gravity blast special fix
			if(!displayBPMSucceed)
			{
				int indexDisplayBPM = 0;
				for(int i=0; i<displayBPMSplit.Count(); i++){
					if(displayBPMSplit[i].Contains("DISPLAYBPM")){
						indexDisplayBPM = i + 1;
					}
				}

				if(System.Double.TryParse(displayBPMSplit[indexDisplayBPM].Replace(";", ""), out displayBPMFirstValue))
				{
					if((displayBPMSplit.Count() - indexDisplayBPM - 1) > 2)
					{
						if(System.Double.TryParse(displayBPMSplit[indexDisplayBPM+1].Replace(";", ""), out displayBPMFirstValue))
						{
							displayBPMValue = displayBPMFirstValue.ToString("0") + "$" + displayBPMSecondValue.ToString("0");
							displayBPMSucceed = true;
						}
					}else{
						displayBPMValue = displayBPMFirstValue.ToString("0");
						displayBPMSucceed = true;
					}
				}
			}

		}else{
			double minBPM = BPMListInTime.Min(c => c.Value);
			double maxBPM = BPMListInTime.Max(c => c.Value);
			if(minBPM == maxBPM){
				displayBPMValue = maxBPM.ToString("0");	
			}else{
				displayBPMValue = minBPM.ToString("0") + "$" + maxBPM.ToString("0");
			}
		}

		wordDictionary [FileTags.DISPLAYBPM] = displayBPMValue;

		//For all difficulties
		foreach(int index in indexNotes){
			
			Song theNewsong = new Song();
			
			//Tag getted
			theNewsong.title = wordDictionary[FileTags.TITLE];
			theNewsong.subtitle = wordDictionary[FileTags.SUBTITLE];
			theNewsong.artist = wordDictionary[FileTags.ARTIST];
			theNewsong.banner = wordDictionary[FileTags.BANNER];
			if(!System.Double.TryParse(wordDictionary[FileTags.OFFSET], out theNewsong.offset)) theNewsong.offset = 0;
			if(!System.Double.TryParse(wordDictionary[FileTags.SAMPLESTART], out theNewsong.samplestart)) theNewsong.samplestart = 0;
			if(!System.Double.TryParse(wordDictionary[FileTags.SAMPLELENGTH], out theNewsong.samplestart)) theNewsong.samplelenght = 0;
			theNewsong.bpms = BPMListInTime;
			theNewsong.stops = STOPListInTime;
			theNewsong.mesureBPMS = BPMListInMesure;
			theNewsong.mesureSTOPS = STOPListInMesure;
			theNewsong.bpmToDisplay = displayBPM;

			theNewsong.song = files.FirstOrDefault(c => isAllowedFile(c, audioFileAllowed));
			if(theNewsong.song != null) theNewsong.song = "file://" + theNewsong.song.Replace('\\', '/');
				
				
			//Song information
			int indexBeginInformation = index;
			bool exitTrigger = false;
			string chartmode = listLine.ElementAt(indexBeginInformation + 1).Replace(":","").Trim();

			if(isAllowedFile(chartmode, noteFileAllowed, noteFileRestricted))
			{
				theNewsong.stepartist = listLine.ElementAt(indexBeginInformation + 2).Replace(":","").Trim();
				theNewsong.setDifficulty(listLine.ElementAt(indexBeginInformation + 3).Replace(":","").Trim());
				if(!System.Int32.TryParse(listLine.ElementAt(indexBeginInformation + 4).Replace(":","").Trim(), out theNewsong.level))
				{
					exitTrigger = true;
				}

					
					
				//getting stepchart
				int indexBeginStepchart = indexBeginInformation+6;
				while(listLine.ElementAt(indexBeginStepchart).Contains("//") || 
				      String.IsNullOrEmpty(listLine.ElementAt(indexBeginStepchart).Trim()) || 
				      listLine.ElementAt(indexBeginStepchart) == "")
				{
					
					if(listLine.ElementAt(indexBeginStepchart).Contains("NOTES")) exitTrigger = true;

					indexBeginStepchart++;	
				}
				
				if(listLine.ElementAt(indexBeginStepchart).Contains("NOTES")) exitTrigger = true;
				//if(theNewsong.title == "The Last Kiss") Debug.Log(listLine.ElementAt(beginstepchart));
				
				if(!exitTrigger){
					int numberOfSteps = 0;
					int numberOfMines = 0;
					int numberOfRoll = 0;
					int numberOfFreezes = 0;
					int numberOfJump = 0;
					int numberOfStepsWJ = 0;
					int numberOfStepsAbs = 0;
					theNewsong.stepchart.Add(new List<string>());
					for(int i = indexBeginStepchart; !listLine.ElementAt(i).Contains(";"); i++){
						if(listLine.ElementAt(i).Contains(",")){
							theNewsong.stepchart.Add(new List<string>());
						}else if(!String.IsNullOrEmpty(listLine.ElementAt(i))){
							theNewsong.stepchart.Last().Add(listLine.ElementAt(i));	
							int stepThisMesure = listLine.ElementAt(i).Count(c => "124".Contains(c));
							numberOfSteps += stepThisMesure;
							numberOfFreezes += listLine.ElementAt(i).Count(c => c == '2');
							numberOfRoll += listLine.ElementAt(i).Count(c => c == '4');
							numberOfMines += listLine.ElementAt(i).Count(c => c == 'M');
							numberOfStepsWJ += stepThisMesure;
							numberOfStepsAbs += stepThisMesure;
							
							if(stepThisMesure == 2){
								numberOfStepsWJ -= stepThisMesure;
								numberOfStepsAbs -= stepThisMesure - 1;
								numberOfJump++;
							}
							if(stepThisMesure >= 3){
								numberOfStepsWJ -= stepThisMesure;
								numberOfStepsAbs -= stepThisMesure - 1;
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

					if(!exitTrigger) fakeCreation(theNewsong);
						//A mettre
					//if(outputSongs.ContainsKey(theNewsong.difficulty))
					theNewsong.sip = new SongInfoProfil(theNewsong.title, theNewsong.subtitle, 
						theNewsong.numberOfSteps, theNewsong.difficulty, theNewsong.level);
					outputSongs.Add(theNewsong.difficulty, theNewsong);
				}
			}
		}
		
		return outputSongs;
		
	}
	
	
	void fakeCreation(Song s){
				
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



		
		//stepBySecondsAverage
		double timestart = -1000;
		double stoptime = 0;
		int countStep = 0;
		double stepbysecAv = 0f;
		
		
		//stepmax
		double maxStepPerSeconds = 0f;
		int numberStepBetweenTwoBeat = 0;
		double timestartMax = 0f;
		//double maxLenght = 0f;
		
		
		//longestStream
		double lenghtStream = 0f;
		double maxLenghtStream = 0f;
		double speedOfMaxStream = 0f;
		double previousSpeed = 0f;

		//Footswitch
		int numberOfFootswitch = 0;
	
		//Cross
		int numberOfCross = 0;
	
		//Hands
		int numberOfHands = 0;
		
		//freeze
		bool[] freezed = new bool[4] { false, false, false, false };
		
		//graph
		Dictionary<double, double> listNumberStep = new Dictionary<double, double>();
		
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


		
				bufferBPMTime += (mesureIndex - prevMesureIndex)/currentBPS;
				currentTime = bufferBPMTime + savedBPMTime + currentSTOPTime;
				
				char[] finalBeatLine = mesure[beatLine].Trim().ToCharArray();



				//Start chart Analysis
				if((beatLine*8f)%(mesure.Count) == 0){
					var newMax = numberStepBetweenTwoBeat/(currentTime - timestartMax);
					if(!listNumberStep.ContainsKey(currentTime)){
						listNumberStep.Add(currentTime, newMax);
					}else{
						listNumberStep.Add(currentTime + 0.00001, newMax);
					}
					if(maxStepPerSeconds < newMax){
						maxStepPerSeconds = newMax;
					}
					
					
					
					if(Mathf.Abs((float)(newMax - previousSpeed)) < 0.001f 
					   && numberStepBetweenTwoBeat > 0 && newMax > 4f){
						
						lenghtStream += (currentTime - timestartMax);
						if(lenghtStream > maxLenghtStream){
							speedOfMaxStream = newMax;
							maxLenghtStream = lenghtStream;
						}
					}else{
						lenghtStream = 0;
					}
					
					previousSpeed = newMax;
					numberStepBetweenTwoBeat = 0;
					timestartMax = currentTime;
				}

				bool arrowFound = finalBeatLine.Any(c => "124".Contains(c));
				bool actionFound = finalBeatLine.Any(c => "1234M".Contains(c));
				int numberOfArrows = 0;
				Lanes laneSelected = Lanes.LEFT;
				
				
				for(int i =0;i<4; i++){
					if("24".Contains(finalBeatLine[i]))
					{
						freezed[i] = true;
					}else if(finalBeatLine[i] == '3' && freezed[i])
					{
						freezed[i] = false;
					}
				}

				numberOfArrows = freezed.Count(c => c) + finalBeatLine.Count(c => c == '1');
				
				if(numberOfArrows >= 3f){
					numberOfHands++;
				}


				if(actionFound){
					stoptime = currentTime;	
				}
				
				if(arrowFound){
				
					if(timestart == -1000) timestart = currentTime;
					
					countStep++;
					numberStepBetweenTwoBeat++;
					
					if(numberOfArrows < 2){
						switch(laneSelected){
							case Lanes.LEFT:
								//fs
								if(verifyFootswitch(0,2,0,1)
								   || verifyFootswitch(0,0,2,1))
								{
									numberOfFootswitch++;
								}
								setFootswitch(1,0,0,0);

								//cross
								if(verifyCross(0,1,0,1) || verifyCross(0,0,1,1))
								{
									numberOfCross++;
								}
								setCross(1,0,0,0);

								break;

							case Lanes.DOWN:
								//fs
								if(verifyFootswitch(1,0,0,0) || verifyFootswitch(1,1,0,0) || verifyFootswitch(0,0,0,1) || verifyFootswitch(0,1,0,1))
								{
									addFootswitch(0,1,0,0);
								}else{
									setFootswitch(0,0,0,0);
								}

								//cross
								if(verifyCross(1,0,0,0) || verifyCross(0,0,0,1))
								{
									addCross(0,1,0,0);
								}else if(verifyCross(1,0,1,0))
								{
									addCross(0,0,-1,0);
								}

								break;
							case Lanes.UP:
								//fs
								if(verifyFootswitch(1,0,0,0) || verifyFootswitch(1,0,1,0) || verifyFootswitch(0,0,0,1) || verifyFootswitch(0,0,1,1))
								{
									addFootswitch(0,0,1,0);
								}else{
									setFootswitch(0,0,0,0);
								}

								//cross
								if(verifyCross(1,0,0,0) || verifyCross(0,0,0,1))
								{
									addCross(0,0,1,0);
								}else if(verifyCross(1,1,0,0))
								{
									addCross(0,-1,0,0);
								}
								break;
							case Lanes.RIGHT:
								//fs
								if(verifyFootswitch(1,2,0,0)
								   || verifyFootswitch(1,0,2,0))
								{
									numberOfFootswitch++;
								}
								setFootswitch(0,0,0,1);
								
								//cross
								if(verifyCross(1,1,0,0) || verifyCross(1,0,1,0))
								{
									numberOfCross++;
								}
								setCross(0,0,0,1);
								break;
						}
					}else{
						setCross (0,0,0,0);
						setFootswitch(0,0,0,0);
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

		s.intensityGraph = new double[100];
		
		double lastSignificantValue = 0;
		double cutLoop = s.duration/(double)100;
		
		for(int i=0; i<100; i++){
			if(i == 0){
				s.intensityGraph[i] = 0;
			}else{
				if(listNumberStep.Where(c => c.Key <= cutLoop*i).Count() == 0){
					s.intensityGraph[i] = lastSignificantValue;
				}else{
					double average = 0;
					int countAverage = 0;
					List<double> keyToRemove = new List<double>();
					foreach(var val in listNumberStep.Where(c => c.Key <= (cutLoop*i))){
						average += val.Value;
						lastSignificantValue = val.Value;
						keyToRemove.Add(val.Key);
						countAverage++;
					}
					foreach(var rem in keyToRemove){
						listNumberStep.Remove(rem);	
					}
					s.intensityGraph[i] = average/(double)countAverage;
				}
				
			}
		}
		
		
	}

	int[] footswitchStatus = new int[4] { 0, 0, 0, 0 }; //left down up right
	int[] crossStatus = new int[4] { 0, 0, 0, 0 }; //left down up right
	public bool verifyConcordance(int[] status, int[] step)
	{
		for (int i=0; i<step.Length; i++) {
			if(status[i] != step[i]) return false;
		}
		return true;
	}

	public bool verifyFootswitch(params int[] step)
	{
		return verifyConcordance(footswitchStatus, step);
	}

	public bool verifyCross(params int[] step)
	{
		return verifyConcordance(crossStatus, step);
	}

	public void setConcordance(int[] status, int[] step)
	{
		for (int i=0; i<step.Length; i++) {
			status[i] = step[i];
		}
	}
	
	public void setFootswitch(params int[] step)
	{
		setConcordance(footswitchStatus, step);
	}
	
	public void setCross(params int[] step)
	{
		setConcordance(crossStatus, step);
	}

	public void addConcordance(int[] status, int[] step)
	{
		for (int i=0; i<step.Length; i++) {
			status[i] += step[i];
		}
	}
	
	public void addFootswitch(params int[] step)
	{
		addConcordance(footswitchStatus, step);
	}
	
	public void addCross(params int[] step)
	{
		addConcordance(crossStatus, step);
	}
	
	
}
