using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class NoteController : MonoBehaviour {

	public static NoteController instance;
	void Awake()
	{
		if(instance == null) instance = this;
	}

	public Animator[] noteAnimation;
	public Image[] noteEarly;
	public Image[] noteLate;
	int previousAnimation = -1;

	// Use this for initialization
	void Start () {

	}

	public void showNote(Precision precision, NoteTiming timing = NoteTiming.NONE)
	{
		if(previousAnimation >= 0) noteAnimation[previousAnimation].Play("idleState");
		previousAnimation = (int)precision;
		noteAnimation[previousAnimation].Play("NoteAnim", -1, 0f);
		if(precision != Precision.FANTASTIC && precision != Precision.MISS)
		{
			if(timing == NoteTiming.EARLY)
			{
				noteEarly[previousAnimation-1].enabled = true;
				noteLate[previousAnimation-1].enabled = false;
			}else if(timing == NoteTiming.LATE){
				noteEarly[previousAnimation-1].enabled = false;
				noteLate[previousAnimation-1].enabled = true;
			}
		}

	}
}
