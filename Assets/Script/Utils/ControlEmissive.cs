using UnityEngine;
using System.Collections;

public class ControlEmissive : MonoBehaviour {

	public Color emissiveMaxColor;

	public float emissiveCursor;

	public Material mat;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		mat.SetColor("_EmissionColor", Color.Lerp(Color.black, emissiveMaxColor, emissiveCursor));
	}
}
