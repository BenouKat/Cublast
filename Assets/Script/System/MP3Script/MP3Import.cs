using UnityEngine;
using UnityEditor;
using System;
using System.Collections.Generic;
using NAudio;
using NAudio.Wave;
using System.Threading;

public class MP3Import
{
	private static MP3Import _instance;
	public static MP3Import instance {
		get{
			if(_instance == null)
			{
				_instance = new MP3Import();
			}
			return _instance;
		}
	}

	public Song songSelected;
	public string mp3File;
	public string outputFile;

	public void Mp3ToWav()
	{
		string[] mp3Filesplit = mp3File.Split('/');
		string characChaine = "";
		bool goFile = false;
		for (int i=0; i<mp3Filesplit.Length; i++) {
			if(mp3Filesplit[i].Equals("Songs"))
			{
				goFile = true;
			}

			if(goFile)
			{
				characChaine += mp3Filesplit[i] + (i == mp3Filesplit.Length - 1 ? "" : "/");
			}
		}
		using (Mp3FileReader reader = new Mp3FileReader(characChaine))
		{
			WaveFileWriter.CreateWaveFile(characChaine.Replace (".mp3", ".wav").Replace (".MP3", ".wav"), reader);
			songSelected.isWavAvailable = true;
		}
	}
	
	public void createMP3(Song song)
	{
		if (!System.IO.File.Exists (song.songWav)) {
			songSelected = song;
			mp3File = song.song;
			outputFile = song.songWav;
			ThreadStart threadDelegate = new ThreadStart (Mp3ToWav);
			Thread newThread = new Thread (threadDelegate);
			newThread.Start ();
		} else {
			song.isWavAvailable = true;
		}
	}
	
	public void cleanMP3(string path)
	{
		if (System.IO.File.Exists (path)) {
			try{
				System.IO.File.Delete(path);
			}catch(Exception e){
				Debug.Log(e.Message);
			}

		}
	}
}