using UnityEngine;
using System.Collections;

public class DisableOnParticleEnd : MonoBehaviour {

	ParticleSystem currentParticles;

	void Start()
	{
		currentParticles = GetComponent<ParticleSystem>();
	}

	// Update is called once per frame
	void Update () {
		if(!currentParticles.IsAlive())
		{
			gameObject.SetActive(false);
		}
	}
}
