using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PackCubeGenerator : MonoBehaviour {


	public int cubeToCreate;
	public float diameter;
	public GameObject model;
	public Vector3 axisRotation;

	public List<PackCube> packCubes;
	
	// Use this for initialization
	void Awake () {
		GameObject rotator = new GameObject("temp");
		rotator.transform.position = transform.position;
		rotator.transform.rotation = transform.rotation;
		float rotationPerCubes = 360f / cubeToCreate;
		for(int i=0; i<cubeToCreate; i++)
		{
			GameObject packCube = Instantiate(model, rotator.transform.position, rotator.transform.rotation) as GameObject;
			packCube.transform.Translate(-rotator.transform.forward*diameter/2f, Space.World);
			packCube.transform.SetParent(transform);
			packCube.SetActive(true);

			PackCube packCubeObj = packCube.GetComponent<PackCube>();
			packCubeObj.rotationValue = i*rotationPerCubes;
			packCubes.Add(packCubeObj);
			
			rotator.transform.Rotate(axisRotation*rotationPerCubes, Space.Self);
		}
		
		Destroy (rotator);
	}
}
