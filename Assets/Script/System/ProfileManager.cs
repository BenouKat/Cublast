using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;
using System.Reflection;

public class ProfileManager{
	
	private static ProfileManager instance;
	
	public bool alreadyLoaded;
	
	public static ProfileManager Instance{
		get{
			if(instance == null){ 
				instance = new ProfileManager();
			}
			return instance;
		}
	}

	public Profile prefs;
	
	private ProfileManager ()
	{
		alreadyLoaded = false;
	}
	
	public void setCurrentProfile(Profile p){
		
		prefs = p;
		prefs.loadOptions();
	}
	
	public bool SaveProfile () {

		if(!Directory.Exists(Application.dataPath + GameManager.instance.DEBUGPATH + "UserData/")){
			Directory.CreateDirectory(Application.dataPath + GameManager.instance.DEBUGPATH + "UserData");
		}
		if(File.Exists(Application.dataPath + GameManager.instance.DEBUGPATH + "UserData/local.prefs")){
			File.Move(Application.dataPath + GameManager.instance.DEBUGPATH + "UserData/local.prefs", Application.dataPath + GameManager.instance.DEBUGPATH + "UserData/local.oldprefs");
		}

		Stream stream = File.Open(Application.dataPath + GameManager.instance.DEBUGPATH + "UserData/local.prefs", FileMode.Create);
		BinaryFormatter bformatter = new BinaryFormatter();
	    bformatter.Binder = new VersionDeserializationBinder(); 
		
		try{
			bformatter.Serialize(stream, prefs);
			stream.Close();
			
			if(File.Exists(Application.dataPath + GameManager.instance.DEBUGPATH + "UserData/local.oldprefs")){
				File.Delete(Application.dataPath + GameManager.instance.DEBUGPATH + "UserData/local.oldprefs");
			}
		}catch(Exception e){
			
			stream.Close();
			File.Delete(Application.dataPath + GameManager.instance.DEBUGPATH + "UserData/local.prefs");
			if(File.Exists(Application.dataPath + GameManager.instance.DEBUGPATH + "UserData/local.oldprefs")){
				File.Move(Application.dataPath + GameManager.instance.DEBUGPATH + "UserData/local.oldprefs", Application.dataPath + GameManager.instance.DEBUGPATH + "UserData/local.prefs");
			}
			Debug.Log(e.Message);
			return false;
		}
		return true;
		
		
	}
	
	public byte[] getProfileStream()
	{
		BinaryFormatter bf = new BinaryFormatter();
		bf.Binder = new VersionDeserializationBinder();
		MemoryStream ms = new MemoryStream();
		bf.Serialize(ms, prefs);
		
		return ms.ToArray();
	}
	
	public void LoadProfiles () {
		if(File.Exists(Application.dataPath + GameManager.instance.DEBUGPATH + "UserData/local.prefs")){

			Profile pr = new Profile ();
			Stream stream = File.Open(Application.dataPath + GameManager.instance.DEBUGPATH + "UserData/local.prefs", FileMode.Open);
			BinaryFormatter bformatter = new BinaryFormatter();
			bformatter.Binder = new VersionDeserializationBinder(); 
			pr = (Profile)bformatter.Deserialize(stream);
			stream.Close();
			
			setCurrentProfile(pr);

			alreadyLoaded = true;
		}
	}
	
	
	public SongInfoProfil FindTheSongStat(SongInfoProfil sip){
		if(prefs.scoreOnSong.Count > 0){
			SongInfoProfil returnSip = prefs.scoreOnSong.FirstOrDefault(c => c.CompareId(sip));
			return returnSip;
		}
		return null;
	}
}


