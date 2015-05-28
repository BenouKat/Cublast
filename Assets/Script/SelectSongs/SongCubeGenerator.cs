using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SongCubeGenerator : MonoBehaviour {
	
	public float rotationPerCubes;
	public int maxDisplayed;
	public float diameter;
	public float rayX;
	public float rayY;
	public GameObject model;
	public Vector3 axisRotation;
	public Transform rollRoot;
	Transform rotator;

	private int minToShow;
	private int maxToShow;
	public float speedRotation;
	float rotationCounter = 0f;
	float totalRotationCounter = 0f;

	private Vector3 basePosition;
	public List<SongCube> songCubes = new List<SongCube>();
	Vector3 velocityRotation;
	
	void Awake()
	{
		basePosition = rollRoot.position;
	}
	
	// Use this for initialization
	public void instanceAllSongs (List<SongData> songDatas) {
		
		foreach(SongCube cube in songCubes)
		{
			Destroy(cube.gameObject);
		}
		songCubes.Clear ();

		rollRoot.rotation = Quaternion.identity;
		
		if(rotator == null) rotator = new GameObject("temp").transform;
		rotator.position = rollRoot.position;
		rotator.rotation = rollRoot.rotation;

		if(tempRotator == null) tempRotator = new GameObject("temp2").transform;
		tempRotator.transform.position = rotator.transform.position;
		tempRotator.transform.rotation = rotator.transform.rotation;

		for(int i=0; i<songDatas.Count; i++)//songDatas.Count; i++)
		{
			GameObject songCube = Instantiate(model, rotator.position, rotator.rotation) as GameObject;
			songCube.transform.SetParent(rollRoot);
			songCube.transform.localScale = Vector3.one;
			if(i <= maxDisplayed && rotator.right.x >= -0.2f) songCube.SetActive(true);

			SongCube songCubeObj = songCube.GetComponent<SongCube>();
			songCubeObj.songData = songDatas[i];
			songCubeObj.refresh();
			songCubes.Add(songCubeObj);
			
			turn(rotator, -rotationPerCubes);
		}

		rotator.position = basePosition;
		rotator.rotation = Quaternion.identity;

		rotationCounter = 0f;
		totalRotationCounter = 0f;

		minToShow = 0;
		maxToShow = (int)(maxDisplayed/2);
	}
	
	public void turn(Transform rotator, float degree)
	{
		rotator.Rotate(axisRotation*degree, Space.Self);
		float lerpValueX = Quaternion.Angle(Quaternion.identity, rotator.rotation)/180f;
		if(lerpValueX > 1f) lerpValueX = 2 - lerpValueX;

		float lerpValueY = Quaternion.Angle(Quaternion.identity, rotator.rotation)/90f;
		if(lerpValueY > 1f) lerpValueY = 2 - lerpValueY;

		rotator.position = basePosition;
		rotator.position += Vector3.Lerp(Vector3.zero, Vector3.right*rayX, lerpValueX);
		rotator.position += Vector3.Lerp(Vector3.zero, Vector3.up*(rotator.right.y >= 0 ? rayY : -rayY), lerpValueY);
	}

	Transform tempRotator;
	Vector3 finalPosition;
	Quaternion finalRotation;
	public void roll()
	{
		finalPosition = Vector3.Lerp(tempRotator.transform.position, rotator.transform.position, 0.1f);
		finalRotation = Quaternion.Slerp(tempRotator.transform.rotation, rotator.transform.rotation, 0.1f);

		tempRotator.transform.position = finalPosition;
		tempRotator.transform.rotation = finalRotation;

		if(Quaternion.Angle(tempRotator.transform.rotation, rotator.transform.rotation) > 0.1f || Vector3.Distance(tempRotator.transform.position, rotator.transform.position) > 0.1f)
		{
			GameObject currentSongCube;
			for(int i=0; i<songCubes.Count; i++)
			{
				currentSongCube = songCubes[i].gameObject;
				if(i >= minToShow && i <= maxToShow && tempRotator.right.x >= -0.2f)
				{
					if(!currentSongCube.activeInHierarchy) currentSongCube.SetActive(true);
					currentSongCube.transform.position = tempRotator.transform.position;
				}else if(currentSongCube.activeInHierarchy){
					currentSongCube.SetActive(false);
				}
				

				
				turn(tempRotator.transform, -rotationPerCubes);
			}

			tempRotator.transform.position = finalPosition;
			tempRotator.transform.rotation = finalRotation;
		}


	}

	float oldRotationCounter = 0f;
	float rotationThisTurn = 0f;
	public void checkScrollWheel () {
		if(Input.GetAxis("Mouse ScrollWheel") != 0f)
		{
			oldRotationCounter = totalRotationCounter;
			rotationThisTurn = -speedRotation*Input.GetAxis("Mouse ScrollWheel");
			totalRotationCounter += rotationThisTurn;
			if(totalRotationCounter < 0f)
			{
				rotationThisTurn = -oldRotationCounter;
				totalRotationCounter = 0f;
			}else if(totalRotationCounter > rotationPerCubes*(songCubes.Count - 1))
			{
				totalRotationCounter = rotationPerCubes*(songCubes.Count - 1);
				rotationThisTurn = rotationPerCubes*(songCubes.Count - 1) - oldRotationCounter;
			}

			rotationCounter += rotationThisTurn;
			computeCubeToShow();
			turn(rotator, rotationThisTurn);
		}

		roll();
	}

	void computeCubeToShow()
	{
		if(rotationCounter > rotationPerCubes)
		{
			rotationCounter -= rotationPerCubes;
			maxToShow++;
			if(maxToShow - minToShow > maxDisplayed)
			{
				minToShow++;
			}

		}else if(rotationCounter < 0f)
		{
			rotationCounter += rotationPerCubes;
			maxToShow--;
			if(minToShow > 0)
			{
				minToShow--;
			}
		}
	}
}
