using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class MainMenuConnectedAnim : MonoBehaviour {

	public AudioSource connectionMusic;
	public GameObject introSound;
	public AudioSource mainMusic;

	public Animation gamePanelAnim;
	public Animation camAnim;
	public Transform cameraMain;
	public Transform cameraPosition;

	public GameObject primaryDecor;
	public GameObject secondaryDecor;

	public CanvasGroup cache;

	float timeStartLastAnim;
	int lastAnim;
	bool cacheClosed = false;
	// Use this for initialization
	void Start () {

		if (GameManager.instance.gameInitialized) {
			connectionMusic.gameObject.SetActive (false);
			cameraMain.position = cameraPosition.position;
			mainMusic.gameObject.SetActive (true);
			gamePanelAnim.gameObject.SetActive (true);
			gamePanelAnim.GetComponent<CanvasGroup> ().alpha = 1f;
			primaryDecor.SetActive (false);
			secondaryDecor.SetActive (true);
		} else {
			cache.alpha = 1f;
			timeStartLastAnim = -1000f;
		}

	}

	void Update()
	{
		if (primaryDecor.activeInHierarchy) {

			if(Time.time > timeStartLastAnim + 10f)
			{
				int thisAnim = Random.Range(1, 4);
				if(thisAnim == lastAnim) thisAnim++;
				if(thisAnim >= 4) thisAnim = 1;
				camAnim.Play ("MMCamAnim" + thisAnim.ToString());
				lastAnim = thisAnim;
				StartCoroutine(showCache(false, 1f));
				cacheClosed = false;
				timeStartLastAnim = Time.time;

			}else if(Time.time > timeStartLastAnim + 9f && !cacheClosed)
			{
				cacheClosed = true;
				StartCoroutine(showCache(true, 1f));
			}

		}
	}

	public void goConnection()
	{
		StartCoroutine (animMusic ());
		GameManager.instance.gameInitialized = true;
	}

	bool cancel = false;
	IEnumerator showCache(bool show, float time)
	{
		float timeSpent = 0f;
		while (timeSpent < time && !cancel) {
			timeSpent += Time.deltaTime;
			cache.alpha = Mathf.Lerp(show ? 0f : 1f, show ? 1f : 0f, timeSpent/time);
			yield return 0;
		}
		cancel = false;
	}

	IEnumerator animMusic()
	{
		timeStartLastAnim = Mathf.Infinity;
		cancel = true;
		yield return 0;
		cancel = false;

		yield return StartCoroutine (showCache (true, 0.25f));

		cameraMain.transform.rotation = cameraPosition.transform.rotation;
		camAnim.Play ("MMCameraGo");

		yield return 0;

		cache.alpha = 0f;

		yield return new WaitForSeconds (0.2f);

		introSound.SetActive (true);
		primaryDecor.SetActive (false);
		secondaryDecor.SetActive (true);




		connectionMusic.GetComponent<TweenSoundFade> ().cancel = true;
		float timeSpent = 0f;
		float connectionMusicBaseVolume = connectionMusic.volume;
		while (timeSpent < 1f) {
			timeSpent += Time.deltaTime;
			connectionMusic.volume = Mathf.Lerp(connectionMusicBaseVolume, 0f, timeSpent);
			yield return 0;
		}
		connectionMusic.volume = 0f;


		yield return new WaitForSeconds (1.2f);

		gamePanelAnim.gameObject.SetActive(true);
		gamePanelAnim.Play();
		GlobalMenu.instance.activeMenu (true);
		GlobalMenu.instance.activeBackButton (true, "Quit");
		mainMusic.gameObject.SetActive (true);


	}
}
