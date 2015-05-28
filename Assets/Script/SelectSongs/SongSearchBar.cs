using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.UI;

public class SongSearchBar : MonoBehaviour {

	public InputField textField;
	public InputField numberField;
	public Text textSearch;
	public Animation anim;
	public GameObject searchButton;
	public GameObject hideButton;

	public int[] minCharacForSearch;

	bool isInSearch = false;
	public bool opened = false;
	Sort currentSortSystem = Sort.NAME;
	List<SongData> searchingList;
	int oldSearchLength = 0;
	// Use this for initialization
	void Start () {
		textField.gameObject.SetActive (!(currentSortSystem == Sort.DIFFICULTY || currentSortSystem == Sort.BPM));
		numberField.gameObject.SetActive ((currentSortSystem == Sort.DIFFICULTY || currentSortSystem == Sort.BPM));
		textSearch.text = GameLocalization.instance.Translate ("SearchBy" + currentSortSystem.ToString ());
		textField.text = "";
		numberField.text = "";
		searchButton.SetActive (!opened);
		hideButton.SetActive (opened);
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
				isInSearch = false;
			}
		} else {
			if(!isInSearch || search.Length < oldSearchLength || currentSortSystem == Sort.DIFFICULTY || currentSortSystem == Sort.BPM)
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
				searchByDifficulty(int.Parse(search));
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

	public void previousSearch()
	{
		int sortSystemint = (int)currentSortSystem;
		sortSystemint--;
		if (sortSystemint < 0) {
			sortSystemint = System.Enum.GetValues (typeof(Sort)).Length - 1;
		}
		currentSortSystem = (Sort)sortSystemint;

		cleanSearchBar ();
	}

	public void nextSearch()
	{
		int sortSystemint = (int)currentSortSystem;
		sortSystemint++;
		if (sortSystemint >= System.Enum.GetValues (typeof(Sort)).Length) {
			sortSystemint = 0;
		}
		currentSortSystem = (Sort)sortSystemint;

		cleanSearchBar ();
	}

	public void cleanSearchBar()
	{
		isInSearch = false;
		searchingList.Clear ();
		textField.gameObject.SetActive (!(currentSortSystem == Sort.DIFFICULTY || currentSortSystem == Sort.BPM));
		numberField.gameObject.SetActive ((currentSortSystem == Sort.DIFFICULTY || currentSortSystem == Sort.BPM));
		textSearch.text = GameLocalization.instance.Translate ("SearchBy" + currentSortSystem.ToString ());
		textField.text = "";
		numberField.text = "";
		onEditSearch ("");
	}

	public void callSearchBar()
	{
		opened = !opened;
		anim.Play(opened ? "OpenSearchBar" : "CloseSearchBar");
		searchButton.SetActive (!opened);
		hideButton.SetActive (opened);
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
