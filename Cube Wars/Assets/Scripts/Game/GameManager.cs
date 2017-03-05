using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;

public class GameManager : NetworkBehaviour {

	List<PlayerManager> lstPlayers = new List<PlayerManager>();

	int currentPlayers = 0;

	void OnStartServer() {
		Debug.Log("qwert");
	}

	void OnClientConnect() {
		Debug.Log("asdf");
	}

	void Update() {
		if (isServer) {

			if(NetworkServer.connections.Count != currentPlayers) {
				currentPlayers = NetworkServer.connections.Count;
				//Debug.Log(currentPlayers);
			}				
		}
	}

	void OnConnectedToServer() {
		Debug.Log("bbb");
	}

	void OnPlayerConnected(NetworkPlayer player) {

		Debug.Log("aaa");

		//add to list
		//broadcast to all players
		//http://answers.unity3d.com/questions/311549/need-help-with-creating-nicks-in-multiplayer.html


	}

	[Command]
	public void CmdNewPlayerToList(string newPlayer) {
		//lstPlayers.Add(newPlayer);
		RpcPlayerConnected(newPlayer);

	}

	[ClientRpc]
	public void RpcPlayerConnected(string name) {
		Debug.Log("Player " + name + " has connected.");
	}
}
