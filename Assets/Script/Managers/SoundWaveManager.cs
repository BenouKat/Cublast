﻿using UnityEngine;
using System.Collections;

public enum SpectrumCut
{
	EXP,
	AVERAGE,
	TOP
}

public class SoundWaveManager : MonoBehaviour {

	public static SoundWaveManager instance;

	void Awake()
	{
		if(instance == null)
		{
			instance = this;
		}
	}

	public AudioSource source;

	public int samplesLength = 1024;  // array size
	public float RMSrefValue = 0.1f; // RMS value for 0 dB
	public float threshold = 0.02f;      // minimum amplitude to extract pitch
	float rmsValue; // sound level - RMS
	public float RMSValue { get{ return rmsValue; }}
	float dbValue;   // sound level - dB
	public float DBValue { get{ return dbValue; }}
	float pitchValue; // sound pitch - Hz
	public float PitchValue { get{ return pitchValue; }}
	float[] samples;
	float[] spectrum;


	public bool calculateVolume;
	public bool calculatePitch;
	public bool calculateDb;
	public bool calculateSpectrum;

	bool activeAnalyse;
	public bool isAnalyseActive()
	{
		return activeAnalyse;
	}

	public void init(AudioSource source)
	{
		if(source != null) this.source = source;
		samples = new float[samplesLength];
		spectrum = new float[samplesLength];
	}

	public void activeAnalysis(bool active)
	{
		activeAnalyse = active;
	}

	#region getSpectrums
	//Deprecated
	public float[] getSpectrumBand(int cutCount, float height = 1f, SpectrumCut cut = SpectrumCut.EXP)
	{
		float[] bandArray = new float[Mathf.Clamp(cutCount, 1, samplesLength)];
		getSpectrumBand (ref bandArray, height, cut);
		return bandArray;
	}

	//Deprecated
	public void getSpectrumBand(ref float[] bandArray, float height = 1f, SpectrumCut cut = SpectrumCut.EXP)
	{
		float[] heightArray = new float[bandArray.Length];
		for(int i=0; i<heightArray.Length; i++) heightArray[i] = height;
		getSpectrumBand (ref bandArray, heightArray, cut);
	}

	public void getSpectrumBand(ref float[] bandArray, float[] height, SpectrumCut cut = SpectrumCut.EXP)
	{
		switch (cut) {
		case SpectrumCut.EXP:
			getSpectrumExp(ref bandArray, height);
			break;
		case SpectrumCut.AVERAGE:
			getSpectrumAverage(ref bandArray, height);
			break;
		case SpectrumCut.TOP:
			getSpectrumTop(ref bandArray, height);
			break;
		}
	}

	void getSpectrumAverage(ref float[] bandArray, float[] height)
	{
		if (!activeAnalyse) return;
		int sizePerCut = (int)((float)samplesLength / (float)bandArray.Length);

		for(int i=0; i<sizePerCut; i++)
		{
			int cutPosition = sizePerCut*i;
			float outputSpectrum = 0f;
			for(int j=cutPosition; j<cutPosition + sizePerCut; j++)
			{
				outputSpectrum += Mathf.Clamp(spectrum[j] * (Utils.indexOrLast(height, i) + j*j), 0f, 400f);
			}

			bandArray[i] = outputSpectrum / (float)sizePerCut;
		}
	}

	void getSpectrumTop(ref float[] bandArray, float[] height)
	{
		if (!activeAnalyse) return;
		int sizePerCut = (int)((float)samplesLength / (float)bandArray.Length);
		
		for(int i=0; i<sizePerCut; i++)
		{
			int cutPosition = sizePerCut*i;
			float outputSpectrum = -1000;
			for(int j=cutPosition; j<cutPosition + sizePerCut; j++)
			{
				if(spectrum[j] > outputSpectrum) outputSpectrum = Mathf.Clamp(spectrum[j] * (Utils.indexOrLast(height, i) + j*j), 0f, 400f);
			}
			bandArray[i] = outputSpectrum;
		}
	}
	
	void getSpectrumExp(ref float[] bandArray, float[] height)
	{
		if (!activeAnalyse) return;
		float coeff = Mathf.Log (spectrum.Length);
		int offsets = 0;

		for(int i=0; i<bandArray.Length; i++)
		{
			float next = Mathf.Exp(coeff / bandArray.Length * (i + 1));
			float weight = 1f / (next - (float)offsets);
			float sum = 0;
			while(offsets < next)
			{
				sum += spectrum[offsets];
				offsets++;
			}
			bandArray[i] = Mathf.Sqrt(weight * sum)*Utils.indexOrLast(height, i);
		}
	}
	#endregion


	#region Analyse
	void AnalyzeSound(){


		if (calculateVolume) {
			source.GetOutputData(samples, 0); // fill array with samples

			float sum = 0;
			for (int i=0; i < samplesLength; i++){
				sum += samples[i]*samples[i]; // sum squared samples
			}
			rmsValue = Mathf.Sqrt(sum/samplesLength); // rms = square root of average

			if(calculateDb)
			{
				dbValue = 20f*Mathf.Log10(rmsValue/RMSrefValue); // calculate dB
				if (dbValue < -160f) dbValue = -160f; // clamp it to -160dB min
			}
		}


		// get sound spectrum
		if (calculateSpectrum) {
			source.GetSpectrumData(spectrum, 0, FFTWindow.BlackmanHarris);

			if(calculatePitch)
			{
				float maxV = 0;
				int maxN = 0;
				for (int i=0; i < samplesLength; i++){ // find max 
					if (spectrum[i] > maxV && spectrum[i] > threshold){
						maxV = spectrum[i];
						maxN = i; // maxN is the index of max
					}
				}
				float freqN = maxN; // pass the index to a float variable
				if (maxN > 0 && maxN < samplesLength-1){ // interpolate index using neighbours
					var dL = spectrum[maxN-1]/spectrum[maxN];
					var dR = spectrum[maxN+1]/spectrum[maxN];
					freqN += 0.5f*(dR*dR - dL*dL);
				}
				pitchValue = freqN*AudioSettings.outputSampleRate/samplesLength; // convert index to frequency
			}
		}
	}
	#endregion

	void Update()
	{
		if (activeAnalyse && source != null) {
			AnalyzeSound();
		}
	}
}
