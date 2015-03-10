using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;
using System.Reflection;

public class LoadManager{

	private static LoadManager instance;
	
	public static LoadManager Instance{
		get{
			if(instance == null){
			instance = new LoadManager();	
			}
			return instance;
		
		}
	}
	
	private Dictionary<string, Dictionary<string, Dictionary<Difficulty, Song>>> songs;
	private Dictionary<string, Texture2D> bannerPack;
	public bool alreadyLoaded;
	
	private LoadManager(){
		alreadyLoaded = false;
		
	}
	
	public void Loading(){
		bannerPack = new Dictionary<string, Texture2D>();
		//string[] packpath = (string[]) Directory.GetDirectories(Application.dataPath + "/Songs/"); 	//RELEASE
		string[] packpath = (string[]) Directory.GetDirectories(Application.dataPath + GameManager.instance.DEBUGPATH + "Songs/");  	//DEBUG
		var length = lastDir((string) packpath[0]).Count();
		songs = new Dictionary<string, Dictionary<string, Dictionary<Difficulty, Song>>>();
		foreach(string el in packpath){
			songs.Add(lastDir(el)[length - 1], new Dictionary<string, Dictionary<Difficulty, Song>>());
			var path = Directory.GetFiles(el).FirstOrDefault(c => c.Contains(".png") || c.Contains(".jpg") || c.Contains(".jpeg"));
			if(!String.IsNullOrEmpty(path)){
				WWW www = new WWW("file://" + path);
       			Texture2D texTmp = new Texture2D(512,256);
				while(!www.isDone){}
        		www.LoadImageIntoTexture(texTmp);
				bannerPack.Add(lastDir(el)[length - 1], texTmp);
			}else{
				bannerPack.Add(lastDir(el)[length - 1], (Texture2D) Resources.Load("Cublast"));
			}
			
			//Debug.Log("new pack : " + lastDir(el)[length - 1]);
		}
		
		
		
		foreach(var el in songs){
			//string[] songpath = (string[]) Directory.GetDirectories(Application.dataPath + "/Songs/" + el.Key);		//RELEASE
			string[] songpath = (string[]) Directory.GetDirectories(Application.dataPath + GameManager.instance.DEBUGPATH + "Songs/" + el.Key);		//DEBUG
			var lengthsp = lastDir ((string) songpath[0]).Count();
			foreach(string sp in songpath){
				//Debug.Log("new song : " + lastDir(sp)[lengthsp - 1]);
				var dic = OpenChart.Instance.readChart(sp.Replace('\\', '/'));
				if(dic != null && dic.Count != 0) songs[el.Key].Add(lastDir(sp)[lengthsp - 1] , dic);
				/*Debug.Log("Song : " + lastDir(sp)[lengthsp - 1] + 
					" added / pack : " + el.Key + 
					" / number diff : " + songs[el.Key][lastDir(sp)[lengthsp - 1]].Count() +
					" / number step expert : " + songs[el.Key][lastDir(sp)[lengthsp - 1]][Difficulty.EXPERT].numberOfSteps);*/
					
			}
			songs[el.Key].OrderBy(c => c.Value.First().Value.title);
		}
		
		
		//DEBUG
		if(songs.Keys.Count < 5){
			var cou = songs.Keys.Count + 1;
			for(int i=0; i<5-cou; i++){
				songs.Add("Empty folder " + (i + 1), new Dictionary<string, Dictionary<Difficulty, Song>>());
				bannerPack.Add("Empty folder " + (i + 1), (Texture2D) Resources.Load("Cublast"));
			}
		}
		alreadyLoaded = true;
	}
	
	
	
	public int LoadingFromCacheFile(SerializableSongStorage sss){
		var numberNotFound = 0;
		bannerPack = new Dictionary<string, Texture2D>();
		//string[] packpath = (string[]) Directory.GetDirectories(Application.dataPath + "/Songs/"); 	//RELEASE
		string[] packpath = (string[]) Directory.GetDirectories(Application.dataPath + GameManager.instance.DEBUGPATH + "Songs/");  	//DEBUG
		var length = lastDir((string) packpath[0]).Count();
		songs = new Dictionary<string, Dictionary<string, Dictionary<Difficulty, Song>>>();
		foreach(string el in packpath){
			songs.Add(lastDir(el)[length - 1], new Dictionary<string, Dictionary<Difficulty, Song>>());
			var path = Directory.GetFiles(el).FirstOrDefault(c => c.Contains(".png") || c.Contains(".jpg") || c.Contains(".jpeg"));
			if(!String.IsNullOrEmpty(path)){
				WWW www = new WWW("file://" + path);
       			Texture2D texTmp = new Texture2D(512, 256);
				while(!www.isDone){}
        		www.LoadImageIntoTexture(texTmp);
				bannerPack.Add(lastDir(el)[length - 1], texTmp);
			}else{
				bannerPack.Add(lastDir(el)[length - 1], (Texture2D) Resources.Load("Cublast"));
			}
			
			//Debug.Log("new pack : " + lastDir(el)[length - 1]);
		}
		
		
		
		foreach(var el in songs){
			//string[] songpath = (string[]) Directory.GetDirectories(Application.dataPath + "/Songs/" + el.Key);		//RELEASE
			string[] songpath = (string[]) Directory.GetDirectories(Application.dataPath + GameManager.instance.DEBUGPATH + "Songs/" + el.Key);		//DEBUG
			var lengthsp = lastDir ((string) songpath[0]).Count();
			var packsss = sss.getStore().Where(c => c.packName == el.Key);
			foreach(string sp in songpath){
				//Debug.Log("new song : " + lastDir(sp)[lengthsp - 1]);
				var dic = new Dictionary<Difficulty, Song>();
				var sameSong = packsss.Where(c => c.songFileName == lastDir(sp)[lengthsp - 1]);
				if(sameSong.Count() > 0){
					foreach(var oneSong in sameSong){
						var theUnpackedSong = new Song();
						oneSong.transfertLoad(theUnpackedSong);
						dic.Add(theUnpackedSong.difficulty, theUnpackedSong);
					}
					
				}else{
					dic = OpenChart.Instance.readChart(sp.Replace('\\', '/'));
					numberNotFound++;
				}
				if(dic != null && dic.Count != 0) songs[el.Key].Add(lastDir(sp)[lengthsp - 1] , dic);
				/*Debug.Log("Song : " + lastDir(sp)[lengthsp - 1] + 
					" added / pack : " + el.Key + 
					" / number diff : " + songs[el.Key][lastDir(sp)[lengthsp - 1]].Count() +
					" / number step expert : " + songs[el.Key][lastDir(sp)[lengthsp - 1]][Difficulty.EXPERT].numberOfSteps);*/
					
			}
			sss.getStore().RemoveAll(c => c.packName == el.Key);
			songs[el.Key].OrderBy(c => c.Key);
		}
		
		
		//DEBUG
		if(songs.Keys.Count < 5){
			var cou = songs.Keys.Count + 1;
			for(int i=0; i<5-cou; i++){
				songs.Add("Empty folder " + (i + 1), new Dictionary<string, Dictionary<Difficulty, Song>>());
				bannerPack.Add("Empty folder " + (i + 1), (Texture2D) Resources.Load("Cublast"));
			}
		}
		alreadyLoaded = true;
		return numberNotFound;
	}
	
	public bool isSongFolderEmpty(){
		return Directory.GetDirectories(Application.dataPath + GameManager.instance.DEBUGPATH + "Songs/").Length == 0;
	}
	
	private string[] lastDir(string dir){
		return dir.Replace('\\', '/').Split ('/');
	}

	public Dictionary<Difficulty, Song> FindSong(string pack, string song){
		return songs[pack][song];
	}
	
	public KeyValuePair<Difficulty, Dictionary<Difficulty, Song>> FindSong(SongInfoProfil sip)
	{
		for(int i=0; i<songs.Count; i++)
		{
			for(int j=0; j<songs.ElementAt(i).Value.Count; j++)
			{
				var thesong = songs.ElementAt(i).Value.ElementAt(j).Value.FirstOrDefault(c => c.Value.sip.CompareId(sip));
				if(!thesong.Equals(default(KeyValuePair<Difficulty, Song>)))
				{
					return new KeyValuePair<Difficulty, Dictionary<Difficulty, Song>>(thesong.Key, songs.ElementAt(i).Value.ElementAt(j).Value);	
				}
			}
		}
		return default(KeyValuePair<Difficulty, Dictionary<Difficulty, Song>>);
	}
	
	public Dictionary<string, Dictionary<string, Dictionary<Difficulty, Song>>> ListSong(){
		return songs;
	}
	
	public Dictionary<string, Dictionary<Difficulty, Song>> ListSong(Dictionary<string, Dictionary<Difficulty, Song>> previousList, string contains){
		
		var finalList = new Dictionary<string, Dictionary<Difficulty, Song>>();
		
		switch(GameManager.instance.sortMethod){
		
			case Sort.NAME:
				if(previousList.Count == 0){
					foreach(var packs in songs.Where(c => c.Value.Where(d => d.Value.First().Value.title.ToLower().Contains(contains.ToLower())).Count() > 0)){
						foreach(var song in packs.Value.Where(r => r.Value.First().Value.title.ToLower().Contains(contains.ToLower()))){
							finalList.Add(song.Key + ";" + packs.Key, song.Value);
						}
					}
				}else{
					finalList = previousList.Where(r => r.Value.First().Value.title.ToLower().Contains(contains.ToLower())).ToDictionary(v => v.Key, v => v.Value);
				}
			break;
			
		
			case Sort.STARTWITH:
				if(previousList.Count == 0){
					foreach(var packs in songs.Where(c => c.Value.Where(d => d.Value.First().Value.title.ToLower().StartsWith(contains.ToLower())).Count() > 0)){
						foreach(var song in packs.Value.Where(r => r.Value.First().Value.title.ToLower().StartsWith(contains.ToLower()))){
							finalList.Add(song.Key + ";" + packs.Key, song.Value);
						}
					}
				}else{
					finalList = previousList.Where(r => r.Value.First().Value.title.ToLower().StartsWith(contains.ToLower())).ToDictionary(v => v.Key, v => v.Value);
				}
			break;
			
			case Sort.ARTIST:
				if(previousList.Count == 0){
					foreach(var packs in songs.Where(c => c.Value.Where(d => d.Value.First().Value.artist.ToLower().Contains(contains.ToLower())).Count() > 0)){
						foreach(var song in packs.Value.Where(r => r.Value.First().Value.artist.ToLower().Contains(contains.ToLower()))){
							finalList.Add(song.Key + ";" + packs.Key, song.Value);
						}
					}
				}else{
					finalList = previousList.Where(r => r.Value.First().Value.artist.ToLower().Contains(contains.ToLower())).ToDictionary(v => v.Key, v => v.Value);
				}
			break;
			
			
			case Sort.STEPARTIST:
				if(previousList.Count == 0){
					foreach(var packs in songs.Where(c => c.Value.Where(d => d.Value.First().Value.stepartist.ToLower().Contains(contains.ToLower())).Count() > 0)){
						foreach(var song in packs.Value.Where(r => r.Value.First().Value.stepartist.ToLower().Contains(contains.ToLower()))){
							finalList.Add(song.Key + ";" + packs.Key, song.Value);
						}
					}
				}else{
					finalList = previousList.Where(r => r.Value.First().Value.stepartist.ToLower().Contains(contains.ToLower())).ToDictionary(v => v.Key, v => v.Value);
				}
			break;
			
			//remplacer par des try parse
			case Sort.DIFFICULTY:
				int dif = 0;
				if(Int32.TryParse(contains, out dif)){
					foreach(var packs in songs.Where(c => c.Value.Where(d => d.Value.Count(s => s.Value.level == dif && s.Value.difficulty <= Difficulty.EDIT) > 0).Count() > 0)){
						foreach(var song in packs.Value.Where(r => r.Value.Count(s => s.Value.level == dif && s.Value.difficulty <= Difficulty.EDIT) > 0)){
							finalList.Add(song.Key + ";" + packs.Key, song.Value);
						}
					}
				}
			break;
			
			//remplacer par des try parse
			case Sort.BPM:
				int dif2 = 0;
				if(Int32.TryParse(contains, out dif2)){
					foreach(var packs in songs.Where(c => c.Value.Where(d => d.Value.First().Value.bpmToDisplay.Contains(dif2.ToString())).Count() > 0)){
						foreach(var song in packs.Value.Where(r => r.Value.First().Value.bpmToDisplay.Contains(dif2.ToString()))){
							finalList.Add(song.Key + ";" + packs.Key, song.Value);
						}
					}
				}
			break;
		
		}
		return finalList;
	}
	
	public bool isAllowedToSearch(string search){
		switch(GameManager.instance.sortMethod){
			case Sort.NAME:
				return search.Trim().Length >= 3;
			case Sort.STARTWITH:
				return search.Trim().Length >= 1;
			case Sort.ARTIST:
				return search.Trim().Length >= 3;
			case Sort.STEPARTIST:
				return search.Trim().Length >= 3;
			case Sort.DIFFICULTY:
				return search.Trim().Length >= 1;
			case Sort.BPM:
				return search.Trim().Length >= 2;
			default:
				return false;
		}
	}
	
	public Dictionary<string, Texture2D> ListTexture(){
		return bannerPack;
	}
	
	
	private void renameSharpFolder(){
		string[] packpath = (string[]) Directory.GetDirectories(Application.dataPath + GameManager.instance.DEBUGPATH + "Songs/");
		for(int i=0; i< packpath.Length; i++){
			if(packpath[i].Contains("#")){
				Directory.Move(packpath[i], packpath[i].Replace("#", ""));
			}
			string[] songpath = Directory.GetDirectories(packpath[i]);
			for(int j=0; j< songpath.Length; j++){
				if(songpath[i].Contains("#")){
					Directory.Move(songpath[i], songpath[i].Replace("#", ""));
				}
			}
		}
		
	}
	
	public string getAllPackName()
	{
		var packName = "";
		
		for(int i=0; i < songs.Count(); i++)
		{
			packName += songs.ElementAt(i).Key;
			if(i < songs.Count() - 1)
			{
				packName += ";";	
			}
		}
		
		return packName;
	}
	
	
	
	public bool SaveCache () {

		if(!Directory.Exists(Application.dataPath + GameManager.instance.DEBUGPATH + "Cache/")){
				Directory.CreateDirectory(Application.dataPath + GameManager.instance.DEBUGPATH + "Cache");
		}
		var cacheFiles = (string[]) Directory.GetFiles(Application.dataPath + GameManager.instance.DEBUGPATH + "Cache");
		for(int i=0; i<cacheFiles.Length; i++){
			File.Delete(cacheFiles[i]);	
		}
		
		var sss = new SerializableSongStorage();
		sss.packTheStore();
		var decoupStore = sss.decoupSerial();
		
		for(int i=0; i<decoupStore.Count; i++){
			using(Stream stream = File.Open(Application.dataPath + GameManager.instance.DEBUGPATH + "Cache/" + "dataSong" + i + ".cache", FileMode.Create))
			{
				BinaryFormatter bformatter = new BinaryFormatter();
				bformatter.Binder = new VersionDeserializationBinder(); 
				var minisss = new SerializableSongStorage();
				minisss.store = decoupStore[i];
				
				try{
					bformatter.Serialize(stream, minisss);
					minisss = null;
					
				}catch(Exception e){
					var cacheFilesDel = (string[]) Directory.GetFiles(Application.dataPath + GameManager.instance.DEBUGPATH + "Cache");
					for(int j=0; j<cacheFilesDel.Length; j++){
						File.Delete(cacheFilesDel[j]);	
					};
					sss.destroy();
					sss.getStore().Clear();
					sss = null;
					decoupStore.Clear();
					decoupStore = null;
					Debug.Log(e.Message);
					return false;
				}
			}
		}
		
		sss.destroy();
		sss.getStore().Clear();
		sss = null;
		decoupStore.Clear();
		decoupStore = null;
		return true;
		
		
	}
	
	public bool LoadFromCache () {
	
		if(Directory.Exists(Application.dataPath + GameManager.instance.DEBUGPATH + "Cache/")){
			var cacheFiles = (string[]) Directory.GetFiles(Application.dataPath + GameManager.instance.DEBUGPATH + "Cache");
			var sss = new SerializableSongStorage ();
			Debug.Log(System.DateTime.Now.Second + ":" + System.DateTime.Now.Millisecond);
			for(int i=0; i<cacheFiles.Length; i++){
				var file = Directory.GetFiles(Application.dataPath + GameManager.instance.DEBUGPATH + "Cache").FirstOrDefault(c => c.Contains("dataSong"+ i +".cache"));
				var minisss = new SerializableSongStorage ();
				using(Stream stream = File.Open(file, FileMode.Open))
				{
					BinaryFormatter bformatter = new BinaryFormatter();
					bformatter.Binder = new VersionDeserializationBinder(); 
					minisss = (SerializableSongStorage)bformatter.Deserialize(stream);
					sss.store.AddRange(minisss.store);
					minisss = null;
				}
			}
			Debug.Log(LoadingFromCacheFile(sss));
			sss.destroy();
			sss.getStore().Clear();
			sss = null;
			return true;
			
		}
			
		
		return false;
	}
}
