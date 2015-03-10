using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

[Serializable]
public class SerializableSongStorage {

	public List<SerializableSong> store;
	
	
	public SerializableSongStorage(){
		store = new List<SerializableSong>();
	}
	
	public void packTheStore(){
		var allSongs = LoadManager.Instance.ListSong();
		foreach(var packs in allSongs){
			foreach(var songs in packs.Value){
				foreach(var song in songs.Value){
					var ss = new SerializableSong();
					ss.transfertSave(song.Value, packs.Key, songs.Key);
					store.Add(ss);
				}
			}
		}
	}
	
	public List<SerializableSong> getStore(){
		return store;
	}
	
	public void destroy(){
		for(int i=0; i<store.Count; i++){
			store[i] = null; 	
		}
	}
	
	public List<List<SerializableSong>> decoupSerial(){
		var decoup = new List<List<SerializableSong>>();
		decoup.Add(new List<SerializableSong>());
		var indexDecoup = 0;
		for(int i=0; i<store.Count; i++){
			if(i != 0 && i%500 == 0){
				indexDecoup++;
				decoup.Add(new List<SerializableSong>());
			}
			decoup[indexDecoup].Add(store[i]);
		}
		
		return decoup;
	}
	
	
	
}