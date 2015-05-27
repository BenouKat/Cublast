using UnityEngine;
using System.Collections;
using UnityEngine.UI;

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
	public float percentDecal = 0.5f;

	SongPack currentPack;
	public int currentDifficultyTypePack;
	public PackCube currentCube;
	public Text packname;
	public Text fakepackname;
	public Text songCountText;

	public Color[] difficultyColors;

	// Use this for initialization
	void Start () {
		GameObject rotatorObj = new GameObject("targetRotator");
		targetRotator = rotatorObj.transform;
		targetRotator.rotation = rotator.rotation;
		targetRotator.SetParent(transform);

		rotationPerCubes = 360f / generator.packCubes.Count;
		rotationCounter = rotationPerCubes*percentDecal;
		reloadAllTexturesAndPack();
		reloadUI();
		currentCube.selectPack(true);
	}

	public void reloadAllTexturesAndPack()
	{
		int indexPack = currentPackIndex-2;
		int indexCube = currentCubeIndex-2;

		if (LoadManager.instance.songPacks.Count == 1) {
			indexPack += 1;
			indexCube += 1;
		}

		SongPack currentLoopPack;
		currentPack = LoadManager.instance.songPacks[currentPackIndex];
		currentCube = generator.packCubes[currentCubeIndex];
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

			currentLoopPack = LoadManager.instance.songPacks[indexPack];

			if(currentLoopPack.banner != null)
			{
				generator.packCubes[indexCube].objectRenderer.material.mainTexture = currentLoopPack.banner;
			}else{
				generator.packCubes[indexCube].objectRenderer.material.mainTexture = emptyPackTexture;
			}
			generator.packCubes[indexCube].pack = currentLoopPack;

			indexPack++;
			indexCube++;
		}
	}

	public void reloadUI()
	{
		packname.text = currentPack.name;
		fakepackname.text = currentPack.name;
		int songCount = 0;
		int beginnersCount = 0;
		int mediumCount = 0;
		bool beginnersFound = false;
		bool mediumFound = false;
		foreach(SongData sd in currentPack.songsData)
		{
			beginnersFound = false;
			mediumFound = false;
			foreach(Song s in sd.songs.Values)
			{
				if(!beginnersFound && s.level >= 2 && s.level <= 4)
				{
					beginnersCount++;
					beginnersFound = true;
				}else if(!mediumFound && s.level >= 5 && s.level <= 8)
				{
					mediumCount++;
					mediumFound = true;
				}
			}
			songCount++;
		}

		string textCount = songCount + " " + GameLocalization.instance.Translate("Songs") + "\n";
		float beginnersRatio = (float)beginnersCount/((float)songCount);
		float mediumRatio = (float)mediumCount/((float)songCount);
		if(beginnersRatio >= 0.5f)
		{
			songCountText.text = textCount + GameLocalization.instance.Translate("BeginnersValid");
			currentDifficultyTypePack = 0;

		}else if(mediumRatio >= 0.5f)
		{
			songCountText.text = textCount + GameLocalization.instance.Translate("MediumValid");
			currentDifficultyTypePack = 1;

		}else if(beginnersRatio >= 0.15f)
		{
			songCountText.text = textCount + GameLocalization.instance.Translate("BegginersContain");
			currentDifficultyTypePack = 0;

		}else if(mediumRatio >= 0.15f)
		{
			songCountText.text = textCount + GameLocalization.instance.Translate("MediumContain");
			currentDifficultyTypePack = 1;

		}else{
			songCountText.text = textCount + GameLocalization.instance.Translate("ExpertOnly");
			currentDifficultyTypePack = 2;
		}
		songCountText.color = difficultyColors[currentDifficultyTypePack];
	}

	void checkCubeRotation()
	{
		if(rotationCounter > rotationPerCubes)
		{
			rotationCounter -= rotationPerCubes;
			currentPackIndex++;
			currentCubeIndex++;
		}else if(rotationCounter < 0f)
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

		currentCube.selectPack(false);
		reloadAllTexturesAndPack();
		reloadUI();
		currentCube.selectPack(true);
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

	public void onPackSelected(PackCube pack)
	{
		CameraSwitcher.instance.goToSong(pack.pack);
	}
}
