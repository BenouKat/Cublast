using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TestSoundWave : MonoBehaviour {

	public List<GameObject> cubes;

	float[] bands = new float[8];

	SoundWaveManager waveManager;

	// Use this for initialization
	void Start () {
		waveManager = GetComponent<SoundWaveManager>();
		waveManager.init (GetComponent<AudioSource> ());
		waveManager.activeAnalysis (true);
	}
	
	// Update is called once per frame
	void Update () {
		waveManager.getSpectrumBand (ref bands, 10, SpectrumCut.AVERAGE);
		for (int i=0; i<cubes.Count; i++) {
			cubes[i].transform.localScale = new Vector3(1f, bands[i], 1f);
		}
	}
}
