using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class MainMenuConnection : MonoBehaviour {

	public GameObject globalPanelConnection;

	public GameObject connectionPanel;
	public InputField username;
	public InputField password;
	public Toggle automaticConnection;
	public GameObject alreadyConnectedObject;

	public GameObject waitingPanel;
	public Text textWaiting;

	public GameObject errorPanel;
	public Text errorText;

	public GameObject creationPanel;
	public InputField usernameCreation;
	public InputField passwordCreation;
	public InputField passwordConfirmCreation;

	public GameObject warningPanel;

	public GameObject mainMenuAnim;

	bool isConnecting = false;
	bool fromRegister = false;
	string connectionError = "";
	public bool forceMenu = false;

	// Use this for initialization
	void Start () {

		if(GameManager.instance.gameInitialized && !forceMenu)
		{
			goToMainMenu();
			return;
		}

		connectionPanel.SetActive(true);
		waitingPanel.SetActive(false);
		errorPanel.SetActive(false);
		if(warningPanel != null) warningPanel.SetActive(false);
		creationPanel.SetActive(false);

		if(alreadyConnectedObject != null && forceMenu)
		{
			alreadyConnectedObject.SetActive(ServerManager.instance.connected && ServerManager.instance.connectedToRoom);
		}

		if (PlayerPrefs.HasKey ("username")) {
			username.text = PlayerPrefs.GetString("username");
		}

		if (PlayerPrefs.HasKey ("autoconnect") && PlayerPrefs.GetInt ("autoconnect") == 1) {
			password.text = PlayerPrefs.GetString("password");
			if(!forceMenu)
			{
				Invoke ("connect", 0.2f);
			}
		}else{
			automaticConnection.isOn = false;
		}
	}

	void Update()
	{
		if (Input.GetKeyDown(KeyCode.Tab))
		{
			Selectable next = EventSystem.current.currentSelectedGameObject.GetComponent<Selectable>().FindSelectableOnDown();
			
			if (next != null)
			{
				InputField inputfield = next.GetComponent<InputField>();
				if (inputfield != null)
					inputfield.OnPointerClick(new PointerEventData(EventSystem.current));  //if it's an input field, also set the text caret
				
				EventSystem.current.SetSelectedGameObject(next.gameObject, new BaseEventData(EventSystem.current));
			}
			//else Debug.Log("next nagivation element not found");
			
		}
	}
	
	public void connect()
	{
		if(isConnecting) return;
		if (forceMenu && ServerManager.instance.connected)
			ServerManager.instance.disconnect ();
		StartCoroutine (waitingForConnection ());
		fromRegister = false;
		ServerManager.instance.connect (username.text, password.text, 
		                                delegate(PlayerIOClient.PlayerIOError value) {
			isConnecting = false;
			connectionError = value.Message;
		});
	}

	public void playOffline(bool active)
	{
		connectionPanel.SetActive(!active);
		waitingPanel.SetActive(false);
		errorPanel.SetActive(false);
		warningPanel.SetActive(active);
	}

	public void activeCreatePanel(bool active)
	{
		connectionPanel.SetActive(!active);
		waitingPanel.SetActive(false);
		errorPanel.SetActive(false);
		creationPanel.SetActive(active);
	}

	public void goToMainMenu()
	{
		if(forceMenu)
		{
			activeCreatePanel(false);
		}else{
			mainMenuAnim.SetActive(true);
			globalPanelConnection.SetActive(false);
		}

	}

	public void register()
	{
		if(isConnecting) return;
		if (!string.IsNullOrEmpty (usernameCreation.text) && 
		    System.Text.RegularExpressions.Regex.Match (usernameCreation.text, "^[a-zA-Z0-9]+$").Success) {

			if(!string.IsNullOrEmpty(passwordCreation.text) && passwordCreation.text.Equals(passwordConfirmCreation.text))
			{
				if (forceMenu && ServerManager.instance.connected)
					ServerManager.instance.disconnect ();
				StartCoroutine (waitingForConnection ());
				username.text = usernameCreation.text;
				password.text = passwordCreation.text;
				fromRegister = true;
				ServerManager.instance.register(usernameCreation.text, passwordCreation.text, 
				                                delegate(PlayerIOClient.PlayerIOError value) {
					isConnecting = false;
					connectionError = value.Message;
					passwordCreation.text = "";
					passwordConfirmCreation.text = "";
				});

			}else{
				passwordCreation.text = "";
				passwordConfirmCreation.text = "";
				errorPanel.SetActive(true);
				errorText.text = GameLocalization.instance.Translate("ErrorIncorrectPassword");
			}
			
		} else {
			usernameCreation.text = "";
			errorPanel.SetActive(true);
			errorText.text = GameLocalization.instance.Translate("ErrorBadUsername");
		}
	}

	IEnumerator waitingForConnection()
	{
		yield return 0;
		connectionError = "";
		isConnecting = true;
		waitingPanel.SetActive(true);
		textWaiting.text = GameLocalization.instance.Translate("Connection");
		while ((!ServerManager.instance.connected || !ServerManager.instance.connectedToRoom) && !ServerManager.instance.roomCrashed && isConnecting) {
			yield return 0;
		}

		if(!ServerManager.instance.roomCrashed && isConnecting)
		{
			textWaiting.text = GameLocalization.instance.Translate("RetrieveData");
			while (!ServerManager.instance.user.loaded) {
				yield return 0;
			}
			
			PlayerPrefs.SetString("username", username.text);
			
			
			if (automaticConnection.isOn) {
				PlayerPrefs.SetString("password", password.text);
				PlayerPrefs.SetInt("autoconnect", 1);
			}else{
				PlayerPrefs.SetString("password", "");
				PlayerPrefs.SetInt("autoconnect", 0);
			}
			goToMainMenu();
		}else{
			waitingPanel.SetActive(false);
			errorPanel.SetActive(true);
			changeTextByErrorMessage(string.IsNullOrEmpty(connectionError) ? ServerManager.instance.errorRoomMessage : connectionError);
		}

		isConnecting = false;
	}


	public void changeTextByErrorMessage(string message)
	{
		string error = message.Split(';')[0];
		switch(error)
		{
		case "UnknownUser":
			errorText.text = GameLocalization.instance.Translate("ErrorName");
			break;
		case "InvalidPassword":
			errorText.text = GameLocalization.instance.Translate("ErrorPassword");
			break;
		case "InvalidRegistrationData":
			errorText.text = GameLocalization.instance.Translate("ErrorAlreadyTaken");
			break;
		default:
			if(error.Contains("GeneralError") || error.Contains("NoServersAvailable"))
			{
				errorText.text = GameLocalization.instance.Translate(fromRegister ? "ErrorServerButRegistred" : "ErrorServer");
			}else{
				errorText.text = GameLocalization.instance.Translate("ErrorUnknown");
			}

			break;
		}
	}
}
