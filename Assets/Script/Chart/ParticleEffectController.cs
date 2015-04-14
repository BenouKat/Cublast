using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ParticleEffectController : MonoBehaviour {

	public ParticleSystem[] particlesPrecision;
	public ParticleSystem[] particlesCombo;
	public ParticleSystem freeze;
	public ParticleSystem roll;
	public ParticleSystem mine;
	public ParticleSystem endFreeze;

	void Awake()
	{
		activeComboParticle (false);
	}

	public void play(Precision prec)
	{
		if (prec <= Precision.WAYOFF) {
			//if(particlesPrecision[(int)prec].isPlaying) particlesPrecision[(int)prec].Stop();
			particlesPrecision[(int)prec].Play();
		}

	}

	public void activeComboParticle(bool active)
	{
		foreach (ParticleSystem ps in particlesCombo) {
			ps.gameObject.SetActive(active);
		}
	}

	public void playFreeze()
	{
		freeze.Play ();
	}

	public void playRoll()
	{
		roll.Play ();
	}
	
	public void stopFreezeOrRoll()
	{
		freeze.Stop ();
		roll.Stop ();
	}

	public void playEndFreeze()
	{

	}


	public void playMine()
	{
		mine.Play ();
	}

}
