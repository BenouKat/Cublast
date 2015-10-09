using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class DescriptionController : MonoBehaviour {

	public Text songTitle;
	public Text artistTitle;
	public Text difficulty;
	public Text stepArtist;

	// Use this for initialization
	void Start () {
		songTitle.text = SongOptionManager.instance.currentSongPlayed.title;
		artistTitle.text = SongOptionManager.instance.currentSongPlayed.artist;
		stepArtist.text = SongOptionManager.instance.currentSongPlayed.stepartist;
		difficulty.text = Utils.UppercaseFirst(SongOptionManager.instance.currentSongPlayed.difficulty.ToString ()) 
			+ " " + SongOptionManager.instance.currentSongPlayed.level.ToString ();
	}
}
