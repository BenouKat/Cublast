using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;
using System.Reflection;

public class SongPack
{
	public string name;
	public Texture2D banner;
	public List<SongData> songsData;

	public SongPack()
	{
		songsData = new List<SongData> ();
	}

	public SongPack(string name, Texture2D banner)
	{
		songsData = new List<SongData> ();
		this.name = name;
		this.banner = banner;
	}
}

public class SongData
{
	public string name;
	public Dictionary<Difficulty, Song> songs;

	public SongData()
	{
		songs = new Dictionary<Difficulty, Song> ();
	}
}

[Serializable]
public class SearchAllowed
{
	public Sort sortType;
	public int minimumCharacRequired;
}

public class LoadManager : MonoBehaviour{
	
	public static LoadManager instance;

	void Awake()
	{
		if(instance == null){
			instance = this;	
			init ();
		}
		DontDestroyOnLoad (this);
	}
	
	public List<SongPack> songPacks;
	public List<SearchAllowed> searchAllowed;

	public int packBeforeReturn = 10;
	public int songsBeforeReturn = 100;

	[HideInInspector] public bool loadingIsDone;
	
	void init(){
		loadingIsDone = false;
	}
	
	public void Loading()
	{
		StartCoroutine (LoadingFromDisc ());
	}

	public void Loading(SerializableSongStorage sss)
	{
		StartCoroutine (LoadingFromCacheFile (sss));
	}

	public IEnumerator LoadingFromDisc()
	{
		songPacks = new List<SongPack>();
		
		
		yield return StartCoroutine(LoadPacks ());
		
		yield return StartCoroutine (LoadSongs ());
		
		loadingIsDone = true;
	}

	public IEnumerator LoadingFromCacheFile(SerializableSongStorage sss){
		
		songPacks = new List<SongPack>();
		
		yield return StartCoroutine (LoadPacks ());
		
		yield return StartCoroutine (LoadSongsFromCache (sss));
		
		loadingIsDone = true;
		
		
	}

	public IEnumerator LoadPacks()
	{
		//Récupération de tous les dossiers
		string[] packpath = (string[]) Directory.GetDirectories(Application.dataPath + GameManager.instance.DEBUGPATH + "Songs/");
		int length = lastDir((string) packpath[0]).Count();
		int packOK = 0;

		foreach(string el in packpath){
			var path = Directory.GetFiles(el).FirstOrDefault(c => c.Contains(".png") || c.Contains(".jpg") || c.Contains(".jpeg"));
			Texture2D texTmp = new Texture2D(512,256);
			
			//Chargement de la texture bannière si trouvée
			if(!String.IsNullOrEmpty(path)){
				WWW www = new WWW("file://" + path);
				while(!www.isDone){}
				www.LoadImageIntoTexture(texTmp);
			}else{
				texTmp = null;
			}
			
			//Ajout au songPack
			SongPack sp = new SongPack(lastDir(el)[length - 1], texTmp);
			songPacks.Add(sp);

			packOK++;
			if(packOK >= packBeforeReturn)
			{
				packOK = 0;
				yield return 0;
			}
		}

		yield return 0;
	}

	public IEnumerator LoadSongs()
	{
		int songOK = 0;
		foreach(SongPack spack in songPacks){
			
			//Récupération de toutes les chansons
			string[] songpath = (string[]) Directory.GetDirectories(Application.dataPath + GameManager.instance.DEBUGPATH + "Songs/" + spack.name);	

			foreach(string sp in songpath){

				SongData songData = new SongData();
				//Lecture de la chart
				songData.songs = OpenChart.Instance.readChart(sp.Replace('\\', '/'));
				if(songData.songs != null && songData.songs.Count > 0) songData.name = songData.songs.First().Value.title;
				spack.songsData.Add(songData);

				songOK++;
				if(songOK >= songsBeforeReturn)
				{
					songOK = 0;
					yield return 0;
				}
			}
			spack.songsData.RemoveAll(c => string.IsNullOrEmpty(c.name));
			spack.songsData = spack.songsData.OrderBy(c => c.name).ToList();


		}

		yield return 0;
	}

	public IEnumerator LoadSongsFromCache(SerializableSongStorage sss)
	{
		int songOK = 0;
		foreach(SongPack spack in songPacks){
			//Récupération de toutes les chansons
			string[] songpath = (string[]) Directory.GetDirectories(Application.dataPath + GameManager.instance.DEBUGPATH + "Songs/" + spack.name);		//DEBUG
			var lengthsp = lastDir ((string) songpath[0]).Count();

			//Récupérartion des chansons du store
			var packsss = sss.getStore().Where(c => c.packName == spack.name);
			foreach(string sp in songpath){

				//Récupération de la chanson du store
				SongData songData = new SongData();

				string songName = lastDir(sp)[lengthsp - 1];
				IEnumerable<SerializableSong> sameSong = packsss.Where(c => c.songFileName == songName);

				//Pour toutes les chansons trouvées
				if(sameSong.Count() > 0){

					foreach(SerializableSong oneSong in sameSong){
						//Copie
						Song theUnpackedSong = new Song();
						oneSong.transfertLoad(theUnpackedSong);
						songData.songs.Add(theUnpackedSong.difficulty, theUnpackedSong);
					}
				}else{
					//Récupération depuis le disc, de base
					songData.songs = OpenChart.Instance.readChart(sp.Replace('\\', '/'));
				}

				if(songData.songs != null && songData.songs.Count > 0) songData.name = songData.songs.First().Value.title;
				spack.songsData.Add(songData);

				songOK++;
				if(songOK >= songsBeforeReturn)
				{
					songOK = 0;
					yield return 0;
				}
				
			}
			sss.getStore().RemoveAll(c => c.packName == spack.name);

			spack.songsData.RemoveAll(c => string.IsNullOrEmpty(c.name));
			spack.songsData = spack.songsData.OrderBy(c => c.name).ToList();
		}
		
		yield return 0;
	}
	
	
	

	
	public bool isSongFolderEmpty(){
		return Directory.GetDirectories(Application.dataPath + GameManager.instance.DEBUGPATH + "Songs/").Length == 0;
	}
	
	private string[] lastDir(string dir){
		return dir.Replace('\\', '/').Split ('/');
	}

	public SongData FindSongData(string pack, string song){
		return songPacks.Find (c => c.name == pack).songsData.FirstOrDefault(c => c.name == song);
	}

	public SongData FindSongData(SongPack pack, string song){
		return pack.songsData.FirstOrDefault(c => c.name == song);
	}
	
	public Song FindSong(SongInfoProfil sip)
	{
		foreach (SongPack sp in songPacks) {
			foreach(SongData sd in sp.songsData)
			{
				Song song = FindSong(sd, sip);
				if(song != null) return song;
			}
		}

		return null;
	}

	public Song FindSong(SongData sd, SongInfoProfil sip)
	{
		foreach(KeyValuePair<Difficulty,Song> s in sd.songs)
		{
			if(s.Value.sip.CompareId(sip))
			{
				return s.Value;
			}
		}
		return null;
	}
	
	public List<SongData> ListSong(){
		List<SongData> temp = new List<SongData> ();
		foreach (SongPack sp in songPacks) {
			temp.AddRange(sp.songsData);
		}
		return temp;
	}
	
	public List<SongData> SortListSong(List<SongData> thePreviousList, Sort sortMethod, string param)
	{
		switch(sortMethod){
			case Sort.NAME:
				return thePreviousList.Where(c => c.name.Contains(param)).ToList();

			case Sort.STARTWITH:
				return thePreviousList.Where(c => c.name.StartsWith(param)).ToList();
			
			case Sort.ARTIST:
				return thePreviousList.Where(c => !c.songs.Values.Any(d => d.artist.Contains(param))).ToList();
			
			case Sort.STEPARTIST:
			return thePreviousList.Where(c => !c.songs.Values.Any(d => d.stepartist.Contains(param))).ToList();
			
			case Sort.DIFFICULTY:
				int dif = 0;
				if(Int32.TryParse(param, out dif)){
				return thePreviousList.Where(c => !c.songs.Values.Any(d => (int)d.difficulty == dif)).ToList();
				}
			break;

			case Sort.BPM:
				int dif2 = 0;
				if(Int32.TryParse(param, out dif2)){
				return thePreviousList.Where(c => !c.songs.Values.Any(d => d.bpms.Values.Contains((double)dif2))).ToList();
				}
			break;
		
		}
		return thePreviousList;
	}
	
	public bool isAllowedToSearch(Sort sortMethod, string search){
		return search.Trim().Length >= searchAllowed.Find(c => c.sortType == sortMethod).minimumCharacRequired;
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
		
		for(int i=0; i < songPacks.Count(); i++)
		{
			packName += songPacks[i].name;
			if(i < songPacks.Count() - 1)
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
		string[] cacheFiles = (string[]) Directory.GetFiles(Application.dataPath + GameManager.instance.DEBUGPATH + "Cache");
		for(int i=0; i<cacheFiles.Length; i++){
			File.Delete(cacheFiles[i]);	
		}
		
		SerializableSongStorage sss = new SerializableSongStorage();
		sss.packTheStore();
		List<List<SerializableSong>> decoupStore = sss.decoupSerial();
		
		for(int i=0; i<decoupStore.Count; i++){
			using(Stream stream = File.Open(Application.dataPath + GameManager.instance.DEBUGPATH + "Cache/" + "dataSong" + i + ".cache", FileMode.Create))
			{
				BinaryFormatter bformatter = new BinaryFormatter();
				bformatter.Binder = new VersionDeserializationBinder(); 
				SerializableSongStorage minisss = new SerializableSongStorage();
				minisss.store = decoupStore[i];
				
				try{
					bformatter.Serialize(stream, minisss);
					minisss = null;
					
				}catch(Exception e){
					Debug.Log(e.Message);
					string[] cacheFilesDel = (string[]) Directory.GetFiles(Application.dataPath + GameManager.instance.DEBUGPATH + "Cache");
					for(int j=0; j<cacheFilesDel.Length; j++){
						File.Delete(cacheFilesDel[j]);	
					};
					sss.destroy();
					sss.getStore().Clear();
					sss = null;
					decoupStore.Clear();
					decoupStore = null;
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
			string[] cacheFiles = (string[]) Directory.GetFiles(Application.dataPath + GameManager.instance.DEBUGPATH + "Cache");
			SerializableSongStorage sss = new SerializableSongStorage ();
			for(int i=0; i<cacheFiles.Length; i++){
				string file = cacheFiles.ToList().FirstOrDefault(c => c.Contains("dataSong"+ i +".cache"));
				SerializableSongStorage minisss = new SerializableSongStorage ();
				using(Stream stream = File.Open(file, FileMode.Open))
				{
					BinaryFormatter bformatter = new BinaryFormatter();
					bformatter.Binder = new VersionDeserializationBinder(); 
					minisss = (SerializableSongStorage)bformatter.Deserialize(stream);
					sss.store.AddRange(minisss.store);
					minisss = null;
				}
			}
			LoadingFromCacheFile(sss);
			sss.destroy();
			sss.getStore().Clear();
			sss = null;
			return true;
			
		}
			
		
		return false;
	}
}
