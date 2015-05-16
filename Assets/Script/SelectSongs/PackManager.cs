using UnityEngine;
using System.Collections;

public class PackManager : MonoBehaviour {

	public static PackManager instance;
	void Awake()
	{
		if(instance != null) instance = this;
	}

	public PackCubeGenerator generator;
	public Transform rotator;
	Transform targetRotator;
	public Texture2D emptyPackTexture;

	int currentPackIndex = 0;
	int currentCubeIndex = 0;
	public Vector3 axisRotation;
	public float speedRotation;

	float rotationPerCubes;
	float rotationCounter;

	// Use this for initialization
	void Start () {
		GameObject rotatorObj = new GameObject("targetRotator");
		targetRotator = rotatorObj.transform;
		targetRotator.rotation = rotator.rotation;
		targetRotator.SetParent(transform);

		rotationPerCubes = 360f / generator.packCubes.Count;
		rotationCounter = rotationPerCubes;
		reloadAllTexturesAndPack();
	}

	public void reloadAllTexturesAndPack()
	{
		int indexPack = currentPackIndex-2;
		int indexCube = currentCubeIndex-2;
		SongPack currentPack;
		for(int i=0; i<generator.packCubes.Count;i++)
		{
			if(indexPack >= LoadManager.instance.songPacks.Count)
			{
				indexPack -= LoadManager.instance.songPacks.Count;
			}else if(indexPack < 0)
			{
				indexPack += LoadManager.instance.songPacks.Count;
			}

			if(indexCube >= generator.packCubes.Count)
			{
				indexCube -= generator.packCubes.Count;
			}else if(indexCube < 0)
			{
				indexCube += generator.packCubes.Count;
			}

			currentPack = LoadManager.instance.songPacks[indexPack];

			if(currentPack.banner != null)
			{
				generator.packCubes[indexCube].objectRenderer.material.mainTexture = currentPack.banner;
			}else{
				generator.packCubes[indexCube].objectRenderer.material.mainTexture = emptyPackTexture;
			}


			indexPack++;
			indexCube++;
		}

	}

	void checkCubeRotation()
	{
		if(rotationCounter >= rotationPerCubes)
		{
			rotationCounter -= rotationPerCubes;
			currentPackIndex++;
			currentCubeIndex++;
		}else if(rotationCounter <= -rotationPerCubes)
		{
			rotationCounter += rotationPerCubes;
			currentPackIndex--;
			currentCubeIndex--;
		}

		if(currentPackIndex < 0)
		{
			currentPackIndex += LoadManager.instance.songPacks.Count;
		}else if(currentPackIndex >= LoadManager.instance.songPacks.Count)
		{
			currentPackIndex -= LoadManager.instance.songPacks.Count;
		}

		if(currentCubeIndex < 0)
		{
			currentCubeIndex += generator.packCubes.Count;
		}else if(currentCubeIndex >= generator.packCubes.Count)
		{
			currentCubeIndex -= generator.packCubes.Count;
		}

		reloadAllTexturesAndPack();
	}

	bool notAligned = false;
	// Update is called once per frame
	void Update () {
		if(Input.GetAxis("Mouse ScrollWheel") != 0f)
		{
			targetRotator.Rotate(axisRotation*speedRotation*Input.GetAxis("Mouse ScrollWheel"), Space.Self);
			rotationCounter += speedRotation*Input.GetAxis("Mouse ScrollWheel");

			checkCubeRotation();

			notAligned = true;
		}

		if(Quaternion.Angle(targetRotator.rotation, rotator.rotation) > 0.01f)
		{
			rotator.rotation = Quaternion.Slerp(rotator.rotation, targetRotator.rotation, 0.1f);

		}else if(notAligned)
		{
			rotator.rotation = targetRotator.rotation;
			notAligned = false;
		}
	}
}
