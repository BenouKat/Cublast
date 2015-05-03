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
	}
	
	public void connect()
	{

	}

	public void playOffline(bool active)
	{
		connectionPanel.SetActive(!active);
		warningPanel.SetActive(active);
	}

	public void activeCreatePanel(bool active)
	{
		connectionPanel.SetActive(!active);
		creationPanel.SetActive(active);
	}
}
