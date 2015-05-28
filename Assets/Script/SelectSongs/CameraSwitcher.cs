using UnityEngine;
using System.Collections;

public class CameraSwitcher : MonoBehaviour {


	public Animation anim;
	public PackManager packManager;
	public SongSelectionManager songSelectionManager;
	public GameObject UIPack;
	public GameObject UISong;

	public static CameraSwitcher instance;
	void Awake()
	{
		if(instance == null) instance = this;
	}

	bool navigate = false;
	public void goToSong(SongPack packSelected)
	{
		if(!navigate) StartCoroutine(goToSongRoutine(packSelected));
	}

	IEnumerator goToSongRoutine(SongPack packSelected)
	{
		navigate = true;
		anim.Play("PackToSongMenu");

		packManager.enabled = false;
		UIPack.SetActive (false);

		yield return new WaitForSeconds(0.5f);

		packManager.gameObject.SetActive(false);
		songSelectionManager.gameObject.SetActive(true);
		switch (packManager.currentDifficultyTypePack) {
		case 0:
			songSelectionManager.difficultySelected = Difficulty.EASY;
			break;
		case 1:
			songSelectionManager.difficultySelected = Difficulty.MEDIUM;
			break;
		case 2:
			songSelectionManager.difficultySelected = Difficulty.EXPERT;
			break;
		}
		UISong.SetActive (true);
		songSelectionManager.packSelected(packSelected);


		yield return new WaitForSeconds(0.5f);

		songSelectionManager.enabled = true;

		navigate = false;
	}

	public void goToPack()
	{
		if(!navigate) StartCoroutine(goToPackRoutine());
	}
	
	IEnumerator goToPackRoutine()
	{
		navigate = true;
		anim.Play("SongToPackMenu");

		songSelectionManager.enabled = false;
		
		UISong.SetActive (false);
		
		yield return new WaitForSeconds(0.25f);
		
		packManager.gameObject.SetActive(true);
		songSelectionManager.gameObject.SetActive(false);
		UIPack.SetActive (true);
		
		yield return new WaitForSeconds(0.25f);

		packManager.enabled = true;
		
		navigate = false;
	}
}
