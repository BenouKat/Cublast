using UnityEngine;
using System.Collections;
using PlayerIOClient;
using System;

[System.Serializable]
public class User {

	public bool loaded = false;
	public DateTime dateCreation;
	public DateTime lastConnection;
	public DateTime lastDisconnection;
	public int playTime;
	
	public int songCleared;
	public int worldRecords;
	
	public string currentSongPlayed;
	public string lastSongPlayed;
	public float lastScore;
	
	public User()
	{
		dateCreation = DateTime.UtcNow;
		lastConnection = DateTime.UtcNow;
		lastDisconnection = DateTime.UtcNow.AddHours(-1);
		songCleared = 0;
		worldRecords = 0;
		currentSongPlayed = "";
		lastSongPlayed = "";
		lastScore = 0f;
	}
	
	public User(DatabaseObject userDbo)
	{
		loaded = true;
		dateCreation = userDbo.GetDateTime("DateCreation");
		lastConnection = userDbo.GetDateTime("LastConnection");
		lastDisconnection = userDbo.GetDateTime("LastDisconnection");
		playTime = userDbo.GetInt("PlayTime");
		
		songCleared = userDbo.GetInt("SongCleared");
		worldRecords = userDbo.GetInt("WorldRecords");
		currentSongPlayed = userDbo.GetString("CurrentSongPlayed");
		lastSongPlayed = userDbo.GetString("LastSongPlayed");
		lastScore = userDbo.GetFloat("LastScore");
	}
}
