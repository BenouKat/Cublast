using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ConnectionMenu : MonoBehaviour {

	public GameObject connectionPanel;
	public InputField username;
	public InputField password;
	public Toggle automaticConnection;

	public GameObject waitingPanel;
	public Text textWaiting;

	public GameObject errorPanel;
	public Text errorText;

	public GameObject creationPanel;
	public InputField usernameCreation;
	public InputField passwordCreation;
	public InputField passwordConfirmCreation;

	public GameObject warningPanel;

	// Use this for initialization
	void Start () {
		connectionPanel.SetActive(true);
		waitingPanel.SetActive(false);
		errorPanel.SetActive(false);
		creationPanel.SetActive(false);

		if (PlayerPrefs.HasKey ("username")) {
			username.text = PlayerPrefs.GetString("username");
		}

		if (PlayerPrefs.HasKey ("autoconnect") && PlayerPrefs.GetInt ("autoconnect") == 1) {
			password.text = PlayerPrefs.GetString("password");
			Invoke ("connect", 0.2f);
		}
	}
	
	public void connect()
	{
		StartCoroutine (waitingForConnection ());
		ServerManager.instance.connect (username.text, password.text, 
		                                delegate(PlayerIOClient.PlayerIOError value) {
			StopCoroutine (waitingForConnection ());
			waitingPanel.SetActive(false);
			errorPanel.SetActive(true);
			errorText.text = GameLocalization.instance.Translate("ErrorName");
		});
	}

	public void playOffline(bool active)
	{
		connectionPanel.SetActive(!active);
		warningPanel.SetActive(active);
	}

	public void confirmPlayOffline()
	{

	}

	public void backToConnectPanel()
	{
		connectionPanel.SetActive(true);
		waitingPanel.SetActive(false);
		errorPanel.SetActive(false);
		creationPanel.SetActive(false);
	}

	public void activeCreatePanel(bool active)
	{
		connectionPanel.SetActive(!active);
		creationPanel.SetActive(active);
	}

	public void register()
	{
		if (string.IsNullOrEmpty (usernameCreation.text) && 
		    System.Text.RegularExpressions.Regex.Match (usernameCreation.text, "^[a-zA-Z0-9]+$").Success) {

			if(!string.IsNullOrEmpty(passwordCreation.text) && passwordCreation.text.Equals(passwordConfirmCreation.text))
			{
				ServerManager.instance.checkNameAvailable(usernameCreation.text, delegate(bool result)
				{
					if(result)
					{
						StartCoroutine (waitingForConnection ());
						username.text = usernameCreation.text;
						password.text = passwordCreation.text;
						ServerManager.instance.register(usernameCreation.text, passwordCreation.text, 
						                                delegate(PlayerIOClient.PlayerIOError value) {
							StopCoroutine (waitingForConnection ());
							waitingPanel.SetActive(false);
							passwordCreation.text = "";
							passwordConfirmCreation.text = "";
							errorPanel.SetActive(true);
							errorText.text = GameLocalization.instance.Translate("ErrorIncorrectPassword");
						});
					}else{
						waitingPanel.SetActive(false);
						usernameCreation.text = "";
						errorPanel.SetActive(true);
						errorText.text = GameLocalization.instance.Translate("ErrorAlreadyTaken");
					}
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
		waitingPanel.SetActive(true);
		textWaiting.text = GameLocalization.instance.Translate("Connection");
		while (!ServerManager.instance.connected && !ServerManager.instance.connectedToRoom) {
			yield return 0;
		}
		textWaiting.text = GameLocalization.instance.Translate("RetrieveData");
		while (ServerManager.instance.user == null) {
			yield return 0;
		}

		PlayerPrefs.SetString("username", username.text);


		if (automaticConnection) {
			PlayerPrefs.SetString("password", password.text);
			PlayerPrefs.SetInt("autoconnect", 1);
		}else{
			PlayerPrefs.SetString("password", "");
			PlayerPrefs.SetInt("autoconnect", 0);
		}
	}
}
