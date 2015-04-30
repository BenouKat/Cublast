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

	public bool debug;

	void Start()
	{
		connect ("BenouKat", "test");
	}

	void onConnectionSuccess(string user, Client client){
		Debug.Log("Connected !");
		connected = true;
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

	void onDisconnect(object sender, string error){
		connected = false;
		connectedToRoom = false;
		Debug.LogWarning("Disconnected !");	
	}
	
	void onMessage(object sender, PlayerIOClient.Message m) {
		Debug.Log (m.ToString ());
	}

	public void connect(string user, string password){
		
		Debug.Log("Connection...");
		connected = false;
		
		PlayerIOClient.PlayerIO.Authenticate("cublast-2gjvwklc0aitw2udmikgq", "public",
		                                     new Dictionary<string, string> {
			{"username", user },
			{"password", password }
		},null,
		//Success
		delegate(Client client) {
			PIOclient = client;
			onConnectionSuccess(user, client);
		},
		//Error !
		delegate(PlayerIOError error) {
			Debug.LogError(error.Message);
		}
		);
		
	}
	
	public void register(string user, string password){
		
		PlayerIOClient.PlayerIO.Authenticate("cublast-2gjvwklc0aitw2udmikgq", "public",
		                                     new Dictionary<string, string> {
			{"register", "true" },
			{"username", user },
			{"password", password }
		},null,
		//Success
		delegate(Client client) {
			onConnectionSuccess(user, client);
		},
		//Error !
		delegate(PlayerIOError error) {
			Debug.LogError(error.Message);
		}
		);
	}

	public void joinSoloRoom(string user)
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
				Debug.LogError("Error : " + error.ToString());
			}
			);
	}

	public void joinOrCreateMultiplayerRoom(string user, string roomID, string roomName, bool publicRoom)
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
			Debug.LogError("Error : " + error.ToString());
		}
		);
	}
	
	public void disconnect(){
		if (PIOconnection.Connected) {
			PIOconnection.Disconnect();
		}
	}



	//Requests
	public void sendSongCleared(Song s, double score, int level)
	{
		PIOconnection.Send ("SongCleared", s.sip.getSongNetId (), s.title, level, score);
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
	

}