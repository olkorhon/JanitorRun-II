
using UnityEngine;
using Prototype.NetworkLobby;
using UnityEngine.Networking;

/// <summary>
/// Class that contains hook function for information passing from lobby to game scene
/// </summary>

/*
    Hook that transfers name and colout which are selected in the lobby to the game player in the game scene.    

    @Author: Mikael Martinviita
*/


public class NetworkLobbyHook : LobbyHook {

    public override void OnLobbyServerSceneLoadedForPlayer(NetworkManager manager, GameObject lobbyPlayer, GameObject gamePlayer)
    {
        LobbyPlayer lobby = lobbyPlayer.GetComponent<LobbyPlayer>();
        PlayerObject cube = gamePlayer.GetComponent<PlayerObject>();

        Debug.Log("lobby.name:  " + lobby.playerName);
        cube.nickname = lobby.playerName; //Set name
        cube.color = lobby.playerColor; //Set colour
    }

}
