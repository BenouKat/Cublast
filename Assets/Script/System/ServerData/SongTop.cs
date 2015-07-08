using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using PlayerIOClient;

public class SongTop
{
	public List<string> users;
	public List<int> score;
	
	public SongTop()
	{
		users = new List<string>();
		score = new List<int>();
	}
	
	public SongTop(DatabaseObject songTopDbo)
	{
		users = new List<string>();
		score = new List<int>();
		
		DatabaseArray arrayTop = songTopDbo.GetArray("Ranking");
		for (int i = 0; i < arrayTop.Count; i++)
		{
			DatabaseObject rankDbo = arrayTop.GetObject(i);
			users.Add(rankDbo.GetString("User"));
			score.Add(rankDbo.GetInt("Score"));
		}
	}
}
