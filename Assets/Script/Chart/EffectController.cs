using UnityEngine;
using System.Collections;

public class EffectController : MonoBehaviour {

	public static EffectController instance;
	void Awake()
	{
		if(instance == null) instance = this;
	}

	public ParticleSystem jumpParticles;

	public void activeJumpParticles()
	{
		jumpParticles.gameObject.SetActive(true);
		if(jumpParticles.isPlaying)
		{
			jumpParticles.Stop();
			jumpParticles.Clear();
		}
		jumpParticles.Play();
	}
}
