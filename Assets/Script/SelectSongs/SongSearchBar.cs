using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.UI;

public class SongSearchBar : MonoBehaviour {

	public int[] minCharacForSearch;

	public bool isInSearch;
	Sort currentSortSystem = Sort.NAME;
	List<SongData> searchingList;
	int oldSearchLength = 0;
	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void onEditSearch(string search)
	{
		if (search.Length < minCharacForSearch [(int)currentSortSystem]) {
			if(isInSearch)
			{
				searchingList.Clear ();
				SongSelectionManager.instance.generator.instanceAllSongs (SongSelectionManager.instance.currentPack.songsData);
			}
		} else {
			if(!isInSearch || search.Length < oldSearchLength)
			{
				searchingList = new List<SongData>();
				foreach(SongPack sp in LoadManager.instance.songPacks)
				{
					searchingList.AddRange(sp.songsData);
				}
				isInSearch = true;
			}
			switch(currentSortSystem)
			{
			case Sort.ARTIST:
				searchByArtist(search.ToLower());
				break;
			case Sort.BPM:
				searchByBPM(int.Parse(search));
				break;
			case Sort.DIFFICULTY:
				searchByBPM(int.Parse(search));
				break;
			case Sort.NAME:
				searchByName(search.ToLower());
				break;
			case Sort.STARTWITH:
				searchByStart(search.ToLower());
				break;
			case Sort.STEPARTIST:
				searchByStepArtist(search.ToLower());
				break;
			}
			SongSelectionManager.instance.generator.instanceAllSongs (searchingList);
		}
		oldSearchLength = search.Length;
	}

	public void searchByName(string search)
	{
		searchingList.RemoveAll (c => !c.name.ToLower ().Contains (search));
	}

	public void searchByStart(string search)
	{
		searchingList.RemoveAll (c => !c.name.ToLower ().StartsWith (search));
	}

	public void searchByArtist(string search)
	{
		searchingList.RemoveAll (c => !c.songs.First().Value.artist.ToLower ().Contains (search));
	}

	public void searchByStepArtist(string search)
	{
		searchingList.RemoveAll (c => !c.songs.First().Value.stepartist.ToLower ().Contains (search));
	}

	public void searchByDifficulty(int search)
	{
		searchingList.RemoveAll (c => !c.songs.Any(d => d.Value.level == search));
	}

	public void searchByBPM(int search)
	{
		searchingList.RemoveAll (c => !c.songs.Any(d => (d.Value.bpmToDisplay.Contains("$") ? 
		                                           (int)(float.Parse(d.Value.bpmToDisplay.Split('$')[0])) == search 
		                                           || 
		                                           (int)(float.Parse(d.Value.bpmToDisplay.Split('$')[1])) == search
		         									: 
		                                               (int)(float.Parse(d.Value.bpmToDisplay)) == search)));
	}
}
