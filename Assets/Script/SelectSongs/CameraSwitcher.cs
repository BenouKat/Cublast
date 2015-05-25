using UnityEngine;
using System.Collections;

public class CameraSwitcher : MonoBehaviour {


	public Animation anim;
	public PackManager packManager;
	public SongSelectionManager songSelectionManager;

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

		yield return new WaitForSeconds(0.5f);

		packManager.gameObject.SetActive(false);
		songSelectionManager.gameObject.SetActive(true);
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
		
		yield return new WaitForSeconds(0.25f);
		
		packManager.gameObject.SetActive(false);
		songSelectionManager.gameObject.SetActive(true);
		
		yield return new WaitForSeconds(0.25f);

		packManager.enabled = true;
		
		navigate = false;
	}
}
