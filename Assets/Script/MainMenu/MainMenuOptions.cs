using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class MainMenuOptions : MonoBehaviour {

	public Sprite normalImageButton;
	public Sprite selectedImageButton;
	[HideInInspector] public Image currentlyPressed;
	public List<Button> menuButtons;
	public Image startingButton;

	GameObject currentPanel;

	public List<Button> inputButtons;
	[HideInInspector] public Button choosingButton;
	[HideInInspector] public Text choosingText;
	[HideInInspector] public bool isChoosingInput;
	public Color selectedInput;
	public Color normalInput;

	public InputField godField;
	public Toggle enableAudioEffect;
	public Toggle enableVisualEffect;
	public Toggle useCache;

	public Slider generalVolume;
	public Toggle enableFXAA;
	public Button[] antiAliasingButtons;
	public Sprite AANotActivated;
	public Sprite AAActivated;
	[HideInInspector] public int enabledAA;
	public Toggle enableBloom;
	public Toggle enablePostEffect;
	public Toggle enableVSync;
	public Toggle onlyInGame;

	// Use this for initialization
	void Start () {
		initializeOptions ();
		navigate(startingButton);
	}

	void OnEnable()
	{
		initializeOptions ();
		navigate(startingButton);
	}
	
	// Update is called once per frame
	void Update () {
		if(isChoosingInput && Input.anyKeyDown)
		{
			KeyCode outputKey = setKeyCodeAsInput(fetchKey());
			choosingText.text = outputKey.ToString();
			choosingText.color = normalInput;
			foreach(Button b in menuButtons)
			{
				b.interactable = true;
			}
			foreach(Button b in inputButtons)
			{
				b.interactable = true;
			}
			isChoosingInput = false;
		}
	}

	public void initializeOptions()
	{
		initKeyCodes ();
		foreach(Button button in inputButtons)
		{
			setInputAsKeyCode(button);
		}
		godField.text = GameManager.instance.prefs.globalOffsetSeconds.ToString();
		enableAudioEffect.isOn = GameManager.instance.prefs.enableSoundEffects;
		enableVisualEffect.isOn = GameManager.instance.prefs.enableVisualEffects;
		useCache.isOn = GameManager.instance.prefs.useTheCacheSystem;
		generalVolume.value = GameManager.instance.prefs.generalVolume;
		enableFXAA.isOn = GameManager.instance.prefs.enableFXAA;
		enabledAA = 0;
		if(GameManager.instance.prefs.antiAliasing > 0)
		{
			switch(GameManager.instance.prefs.antiAliasing)
			{
			case 2:
				antialiasingPushed(1);
				break;
			case 4:
				antialiasingPushed(2);
				break;
			case 8:
				antialiasingPushed(3);
				break;
			}
		}
		enableBloom.isOn = GameManager.instance.prefs.enableBloom;
		enablePostEffect.isOn = GameManager.instance.prefs.enablePostProcessEffects;
		enableVSync.isOn = GameManager.instance.prefs.enableVSync;
		onlyInGame.isOn = GameManager.instance.prefs.onlyOnGame;
	}

	public void saveOptions()
	{
		GameManager.instance.prefs.globalOffsetSeconds = float.Parse(godField.text);
		GameManager.instance.prefs.enableSoundEffects = enableAudioEffect.isOn;
		GameManager.instance.prefs.enableVisualEffects = enableVisualEffect.isOn;
		GameManager.instance.prefs.useTheCacheSystem = useCache.isOn;
		GameManager.instance.prefs.generalVolume = generalVolume.value;
		GameManager.instance.prefs.enableFXAA = enableFXAA.isOn;

		switch(enabledAA)
		{
		case 0:
			GameManager.instance.prefs.antiAliasing = 0;
			break;
		case 1:
			GameManager.instance.prefs.antiAliasing = 2;
			break;
		case 2:
			GameManager.instance.prefs.antiAliasing = 4;
			break;
		case 3:
			GameManager.instance.prefs.antiAliasing = 8;
			break;
		}

		GameManager.instance.prefs.enableBloom = enableBloom.isOn;
		GameManager.instance.prefs.enablePostProcessEffects = enablePostEffect.isOn;
		GameManager.instance.prefs.enableVSync = enableVSync.isOn;
		GameManager.instance.prefs.onlyOnGame = onlyInGame.isOn;

		GameManager.instance.prefs.saveOptions ();
		GameManager.instance.setPrefsValues();
		Events.instance.FireCameraOptionChanged();
	}

	//MainMenu navigation
	public void navigate(Image button)
	{
		if(currentlyPressed != null)
		{
			currentlyPressed.sprite = normalImageButton;
		}
		currentlyPressed = button;
		currentlyPressed.sprite = selectedImageButton;

		if(currentPanel != null)
		{
			currentPanel.SetActive(false);
		}
		currentPanel = button.transform.parent.FindChild("Panel").gameObject;
		currentPanel.SetActive(true);
	}

	public void closePanel()
	{
		if(currentPanel != null)
		{
			currentPanel.SetActive(false);
		}
	}

	public void setInput(Button button)
	{
		choosingButton = button;
		foreach(Button b in menuButtons)
		{
			b.interactable = false;
		}
		foreach(Button b in inputButtons)
		{
			b.interactable = false;
		}
		choosingText = choosingButton.transform.GetChild(0).GetComponent<Text>();
		choosingText.text = "...";
		choosingText.color = selectedInput;
		StartCoroutine(startCatchingEvent());
	}

	IEnumerator startCatchingEvent()
	{
		yield return 0;
		isChoosingInput = true;
	}

	public void onUseCache(bool toggled)
	{
		if(toggled && !LoadManager.instance.gotACache())
		{
			regenerateCache();
		}
	}

	public void regenerateCache()
	{
		LoadManager.instance.SaveCache();
	}

	public void onVolumeChange(float volume)
	{
		GameManager.instance.setMasterVolume(volume);
	}

	public void antialiasingPushed(int idAA)
	{
		if(idAA == enabledAA)
		{
			antiAliasingButtons[idAA-1].GetComponent<Image>().sprite = AANotActivated;
			enabledAA = 0;
		}else{
			antiAliasingButtons[idAA-1].GetComponent<Image>().sprite = AAActivated;
			if(enabledAA != 0)
			{
				antiAliasingButtons[enabledAA-1].GetComponent<Image>().sprite = AANotActivated;
			}
			enabledAA = idAA;
		}
	}



	//KEY CODES
	
	KeyCode setKeyCodeAsInput(KeyCode key)
	{
		if(choosingButton.name.Contains("left"))
		{
			if(choosingButton.name.Contains("1"))
			{
				if(key == GameManager.instance.prefs.SecondaryKeyCodeLeft)
				{
					return GameManager.instance.prefs.KeyCodeLeft;
				}
				GameManager.instance.prefs.KeyCodeLeft = key;
			}else{
				if(key == GameManager.instance.prefs.KeyCodeLeft)
				{
					return GameManager.instance.prefs.SecondaryKeyCodeLeft;
				}
				GameManager.instance.prefs.SecondaryKeyCodeLeft = key;
			}
		}else if(choosingButton.name.Contains("down"))
		{
			if(choosingButton.name.Contains("1"))
			{
				if(key == GameManager.instance.prefs.SecondaryKeyCodeDown)
				{
					return GameManager.instance.prefs.KeyCodeDown;
				}
				GameManager.instance.prefs.KeyCodeDown = key;
			}else{
				if(key == GameManager.instance.prefs.KeyCodeDown)
				{
					return GameManager.instance.prefs.SecondaryKeyCodeDown;
				}
				GameManager.instance.prefs.SecondaryKeyCodeDown = key;
			}
		}else if(choosingButton.name.Contains("up"))
		{
			if(choosingButton.name.Contains("1"))
			{
				if(key == GameManager.instance.prefs.SecondaryKeyCodeUp)
				{
					return GameManager.instance.prefs.KeyCodeUp;
				}
				GameManager.instance.prefs.KeyCodeUp = key;
			}else{
				if(key == GameManager.instance.prefs.KeyCodeUp)
				{
					return GameManager.instance.prefs.SecondaryKeyCodeUp;
				}
				GameManager.instance.prefs.SecondaryKeyCodeUp = key;
			}
		}else if(choosingButton.name.Contains("right"))
		{
			if(choosingButton.name.Contains("1"))
			{
				if(key == GameManager.instance.prefs.SecondaryKeyCodeRight)
				{
					return GameManager.instance.prefs.KeyCodeRight;
				}
				GameManager.instance.prefs.KeyCodeRight = key;
			}else{
				if(key == GameManager.instance.prefs.KeyCodeRight)
				{
					return GameManager.instance.prefs.SecondaryKeyCodeRight;
				}
				GameManager.instance.prefs.SecondaryKeyCodeRight = key;
			}
		}
		return key;
	}
	
	void setInputAsKeyCode(Button button)
	{
		Text buttonText = button.transform.GetChild(0).GetComponent<Text>();
		if(button.name.Contains("left"))
		{
			if(button.name.Contains("1"))
			{
				buttonText.text = GameManager.instance.prefs.KeyCodeLeft.ToString();
			}else{
				buttonText.text = GameManager.instance.prefs.SecondaryKeyCodeLeft.ToString();
			}
		}else if(button.name.Contains("down"))
		{
			if(button.name.Contains("1"))
			{
				buttonText.text = GameManager.instance.prefs.KeyCodeDown.ToString();
			}else{
				buttonText.text = GameManager.instance.prefs.SecondaryKeyCodeDown.ToString();
			}
		}else if(button.name.Contains("up"))
		{
			if(button.name.Contains("1"))
			{
				buttonText.text = GameManager.instance.prefs.KeyCodeUp.ToString();
			}else{
				buttonText.text = GameManager.instance.prefs.SecondaryKeyCodeUp.ToString();
			}
		}else if(button.name.Contains("right"))
		{
			if(button.name.Contains("1"))
			{
				buttonText.text = GameManager.instance.prefs.KeyCodeRight.ToString();
			}else{
				buttonText.text = GameManager.instance.prefs.SecondaryKeyCodeRight.ToString();
			}
		}
	}

	private List<KeyCode> validKeyCodes = new List<KeyCode>();
	private void initKeyCodes() {
		validKeyCodes.Clear ();
		validKeyCodes.AddRange((KeyCode[])System.Enum.GetValues(typeof(KeyCode)));
	}

	private KeyCode fetchKey()
	{
		for (int i = 0; i < validKeyCodes.Count; i++) {
			if (Input.GetKeyDown (validKeyCodes[i])) {
				return validKeyCodes[i];
			}
		}
		
		return KeyCode.None;
	}
}
