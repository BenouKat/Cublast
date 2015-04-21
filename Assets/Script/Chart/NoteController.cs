using UnityEngine;
using System.Collections;

public class NoteController : MonoBehaviour {

	public static NoteController instance;
	void Awake()
	{
		if(instance == null) instance = this;
	}

	public Animator[] noteAnimation;
	int previousAnimation = -1;

	// Use this for initialization
	void Start () {

	}

	public void showNote(Precision precision)
	{
		if(previousAnimation >= 0) noteAnimation[previousAnimation].Play("idleState");
		previousAnimation = (int)precision;
		noteAnimation[previousAnimation].Play("NoteAnim", -1, 0f);
	}
}
