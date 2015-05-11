using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using PlayerIOClient;


public class ServerManager : MonoBehaviour {

	public static ServerManager instance;
		
	void Awake () {
		if(instance == null) instance = this;
		DontDestroyOnLoad (this);
	}

	//Player IO
	private PlayerIOClient.Client PIOclient;
	private Connection PIOconnection;

	public bool connected;
	public bool connectedToRoom;
	public bool roomCrashed;
	public string errorRoomMessage;

	//Online user
	public string username;
	public User user;

	public bool debug;

	void onConnectionSuccess(string user, Client client){
		Debug.Log("Connected !");
		connected = true;
		username = user;

		if(username.StartsWith("simple"))
		{
			username = username.Substring(6, username.Length - 6);
		}

		PIOclient = client;

		joinSoloRoom (user);

	}

	void onRoomConnectionSuccess(string typeRoom, Connection connection)
	{
		Debug.Log("Room Joined !");
		connectedToRoom = true;
		PIOconnection = connection;
		PIOconnection.OnMessage += onMessage;
		PIOconnection.OnDisconnect += onDisconnect; 
	}

	void onConnectionFailed(string errorMessage)
	{
		Debug.Log("Failure !");
		connected = false;
		connectedToRoom = false;
		roomCrashed = true;
		this.errorRoomMessage = errorMessage;
	}

	void onDisconnect(object sender, string error){
		connected = false;
		connectedToRoom = false;
		Debug.LogWarning("Disconnected !");	
	}
	
	void onMessage(object sender, PlayerIOClient.Message m) {
		switch(m.Type)
		{
		case "initializeDataSuccess":
			retrieveDatas(username);
			sendPacks();
			break;
		}
	}

	//Stay connected
	float lastTokenAlive = 0f;
	public float durationBetweenTokens = 60f;
	void Update()
	{
		if (PIOconnection != null && PIOconnection.Connected && Time.time > lastTokenAlive + durationBetweenTokens) {
			PIOconnection.Send("aliveToken");
			lastTokenAlive = Time.time;
		}
	}



	#region connectionMethods
	public void connect(string user, string password, Callback<PlayerIOError> errorCallback = null){
		
		Debug.Log("Connection...");
		connected = false;
		roomCrashed = false;
		
		PlayerIOClient.PlayerIO.Authenticate("cublast-2gjvwklc0aitw2udmikgq", "public",
		                                     new Dictionary<string, string> {
			{"username", user },
			{"password", password }
		},null,
		//Success
		delegate(Client client) {
			PIOclient = client;
			onConnectionSuccess(client.ConnectUserId, client);
		},
		//Error !
		delegate(PlayerIOError error) {
			if(errorCallback != null) errorCallback(error);
			Debug.LogError(error.Message);
		}
		);
		
	}
	
	public void register(string user, string password, Callback<PlayerIOError> errorCallback = null){

		connected = false;
		roomCrashed = false;

		PlayerIOClient.PlayerIO.Authenticate("cublast-2gjvwklc0aitw2udmikgq", "public",
		                                     new Dictionary<string, string> {
			{"register", "true" },
			{"username", user },
			{"password", password }
		},null,
		//Success
		delegate(Client client) {
			onConnectionSuccess(client.ConnectUserId, client);
		},
		//Error !
		delegate(PlayerIOError error) {
			if(errorCallback != null) errorCallback(error);
			Debug.LogError(error.Message);
		}
		);
	}

	public void joinSoloRoom(string user, Callback<PlayerIOError> errorCallback = null)
	{
		if(debug) {
			PIOclient.Multiplayer.DevelopmentServer = new ServerEndpoint("127.0.0.1",8184);
		}

		connectedToRoom = false;

		PIOclient.Multiplayer.CreateJoinRoom(user, "CublastSolo", false, null, null,
			delegate(Connection connection) {

				onRoomConnectionSuccess("CublastSolo", connection);
			},
			delegate(PlayerIOError error) {
				if(errorCallback != null) errorCallback(error);
				Debug.LogError("Error : " + error.ToString());
				onConnectionFailed(error.Message);
			}
			);
	}

	public void joinOrCreateMultiplayerRoom(string user, string roomID, string roomName, bool publicRoom, Callback<PlayerIOError> errorCallback = null)
	{
		if(debug) {
			PIOclient.Multiplayer.DevelopmentServer = new ServerEndpoint("127.0.0.1",8184);
		}

		connectedToRoom = false;

		PIOclient.Multiplayer.CreateJoinRoom(roomID, "CublastMulti", publicRoom,  
		                                     new Dictionary<string, string> { {"name", roomName} },
											 new Dictionary<string, string> { {"user", user} },
		//Success
		delegate(Connection connection) {

			onRoomConnectionSuccess("CublastMulti", connection);
		},
		//Error
		delegate(PlayerIOError error) {
			if(errorCallback != null) errorCallback(error);
			Debug.LogError("Error : " + error.ToString());
			onConnectionFailed(error.Message);
		}
		);
	}

	void OnApplicationQuit()
	{
		disconnect();
	}
	
	public void disconnect(){
		if (PIOconnection != null && PIOconnection.Connected) {
			PIOconnection.Disconnect();
		}
	}
	#endregion


	#region loadings
	//Loadings
	public void retrieveDatas(string username)
	{
		Debug.Log("Retrieving from " + username);
		PIOclient.BigDB.Load("Users", username, delegate(DatabaseObject userDbo) {
			user = new User(userDbo);
		});
	}

	#endregion

	#region methods
	#endregion

	//Requests
	#region requests
	public void sendSongCleared(Song s, double score, int level)
	{
		PIOconnection.Send ("SongCleared", s.sip.getSongNetId (), s.title, level, Utils.EncryptScore(score, s.sip.getSongNetId()));
	}

	public void sendCurrentSong(Song s, int level)
	{
		if (s == null) {
			PIOconnection.Send ("UpdateCurrentSong", "", 0);
		} else {
			PIOconnection.Send ("UpdateCurrentSong", s.sip.getSongNetId (), level);
		}
	}

	public void sendPacks()
	{
		string packChain = "";
		foreach (SongPack sp in LoadManager.instance.songPacks) {
			packChain += sp.name.ToString().Replace("|", "") + "|";
		}
		PIOconnection.Send ("UpdatePacks", packChain);
	}

	public void sendChat(string toUser, string content)
	{
		PIOconnection.Send ("SendRequest", "Chat", toUser, content);
	}

	public void sendInviteFriend(string toUser)
	{
		PIOconnection.Send ("SendRequest", "FriendInvite", toUser);
	}

	public void sendInviteToRoom(string toUser)
	{
		PIOconnection.Send ("SendRequest", "RoomInvite", toUser);
	}
	
	#endregion
}