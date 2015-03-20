using UnityEngine;
using System.Collections;

public class TestSound : MonoBehaviour {

	int qSamples = 1024;  // array size
	int cutLenght = 1024;
	float refValue = 0.1f; // RMS value for 0 dB
	float threshold = 0.02f;      // minimum amplitude to extract pitch
	float rmsValue;   // sound level - RMS
	float dbValue;    // sound level - dB
	float pitchValue; // sound pitch - Hz

	public float volume;
	public float db;
	public float pitch;
	
	private float[] samples; // audio samples
	private float[] spectrum; // audio spectrum
	private float fSample;

	public GameObject cubeToInst;
	public GameObject[] cubeInst;

	public AudioSource source;
	
	void Start () {
		samples = new float[qSamples];
		spectrum = new float[qSamples];
		fSample = AudioSettings.outputSampleRate;

		cubeInst = new GameObject[cutLenght];
		for(int i=0; i<cutLenght; i++)
		{
			cubeInst[i] = Instantiate(cubeToInst, new Vector3((float)i*10f, 0f, 0f), cubeToInst.transform.rotation) as GameObject; 
		}
	}
	
	void AnalyzeSound(){
		source.GetOutputData(samples, 0); // fill array with samples
		float sum = 0;
		for (int i=0; i < qSamples; i++){
			sum += samples[i]*samples[i]; // sum squared samples
		}
		rmsValue = Mathf.Sqrt(sum/qSamples); // rms = square root of average
		dbValue = 20f*Mathf.Log10(rmsValue/refValue); // calculate dB
		if (dbValue < -160f) dbValue = -160f; // clamp it to -160dB min
		// get sound spectrum
		source.GetSpectrumData(spectrum, 0, FFTWindow.BlackmanHarris);
		float maxV = 0;
		int maxN = 0;
		for (int i=0; i < qSamples; i++){ // find max 
			if (spectrum[i] > maxV && spectrum[i] > threshold){
				maxV = spectrum[i];
				maxN = i; // maxN is the index of max
			}
		}
		float freqN = maxN; // pass the index to a float variable
		if (maxN > 0 && maxN < qSamples-1){ // interpolate index using neighbours
			var dL = spectrum[maxN-1]/spectrum[maxN];
			var dR = spectrum[maxN+1]/spectrum[maxN];
			freqN += 0.5f*(dR*dR - dL*dL);
		}
		pitchValue = freqN*AudioSettings.outputSampleRate/qSamples; // convert index to frequency
	}

	void AnalyseSamples()
	{
		source.GetSpectrumData(spectrum, 0, FFTWindow.BlackmanHarris);

		for(int i=0; i<cutLenght; i++)
		{
			if(cubeInst[i].transform.position.y < Mathf.Clamp(spectrum[i]*(400f+i*i),0f,400f))
			{
				cubeInst[i].transform.position = new Vector3(cubeInst[i].transform.position.x,
				                                             Mathf.Clamp(spectrum[i]*(400f+i*i),0f,400f), cubeInst[i].transform.position.z);
			}
		}
	}

	void Update () {
		//AnalyzeSound();

		AnalyseSamples ();
		cubeFall ();
		if(volume > 0f) transform.localScale = new Vector3 (1f, volume * rmsValue, 1f);
		if(db > 0f) transform.localScale = new Vector3 (1f, db * dbValue, 1f);
		if(pitch > 0f) transform.localScale = new Vector3 (1f, pitch * pitchValue, 1f);

	}

	void cubeFall()
	{
		for(int i=0; i<cutLenght; i++)
		{
			if(cubeInst[i].transform.position.y > 0f)
			{
				cubeInst[i].transform.position -= Vector3.up*Time.deltaTime*1000f;
			}
		}
	}
}
