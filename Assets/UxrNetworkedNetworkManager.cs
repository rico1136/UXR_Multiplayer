using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.Events;

public class UxrNetworkedNetworkManager : MonoBehaviourPunCallbacks
{
    public string gameVersion = "69";

    public UnityEvent OnJoinedRoomMasterClient;
    public UnityEvent OnJoinedRoomPlayer;
    private void Awake()
    {
        PhotonNetwork.AutomaticallySyncScene = true;
    }

    // Start is called before the first frame update
    void Start()
    {
        if (PhotonNetwork.IsConnected)
        {
            //go to lobby
        }
        else
        {
            ConnectToServer();
        }
    }

    void ConnectToServer()
    {
        PhotonNetwork.ConnectUsingSettings();
        PhotonNetwork.GameVersion = gameVersion;

        Debug.Log("Try Connect To Server...");
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log("Connected To Server.");
        base.OnConnectedToMaster();
      
        Debug.Log("Joining Lobby");

        //CREATE THE ROOM
        RoomOptions roomOptions = new RoomOptions();
        roomOptions.MaxPlayers = 4;
        roomOptions.IsVisible = true;
        roomOptions.IsOpen = true;
        roomOptions.PublishUserId = true;
        
        //Set custom properties of room
        ExitGames.Client.Photon.Hashtable roomProps = new ExitGames.Client.Photon.Hashtable();
        roomProps.Add("test", "");
        
        roomOptions.CustomRoomProperties = roomProps;

        PhotonNetwork.JoinOrCreateRoom("SG_Test", roomOptions, TypedLobby.Default);
    }

    public override void OnJoinedRoom()
    {
        base.OnJoinedRoom();
        if (PhotonNetwork.IsMasterClient)
        {
            PhotonNetwork.AutomaticallySyncScene = true;
        }
    }
}
