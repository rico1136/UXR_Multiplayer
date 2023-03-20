using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using UltimateXR.Mechanics.Weapons;
using UnityEngine;
using UnityEngine.Events;

public class UxrNetworkedPlayerSpawner : MonoBehaviourPunCallbacks
{
    [HideInInspector]public GameObject spawnedPlayerPrefab;
        public GameObject playerPrefab;
    
        [System.Serializable]
        public class SgPlayer
        {
            public int viewID;
            public GameObject playerObject;
            public UxrActor actor;
        }
    
        [SerializeField]public List<SgPlayer> players = new List<SgPlayer>();
        public override void OnJoinedRoom()
        {
            base.OnJoinedRoom();
            SpawnPlayer(playerPrefab.name);
        }

        public void SpawnPlayer(string ObjectToSpawn)
        {
            spawnedPlayerPrefab = PhotonNetwork.Instantiate(ObjectToSpawn, transform.position, transform.rotation);
            PhotonView.Get(this).RPC("AddPlayerToList",RpcTarget.AllBuffered,spawnedPlayerPrefab.GetComponent<PhotonView>().ViewID);
        }
    
        public override void OnLeftRoom()
        {
            base.OnLeftRoom();
            PhotonView.Get(this).RPC("RemoveFromList",RpcTarget.AllBuffered,spawnedPlayerPrefab.GetComponent<PhotonView>().ViewID);
            PhotonNetwork.Destroy(spawnedPlayerPrefab);
        }
    
        [PunRPC]
        public void AddPlayerToList(int viewId)
        {
            
            PhotonView pv = PhotonView.Find(viewId);
            GameObject go = pv.gameObject;
            
            SgPlayer tmpPlayer = new SgPlayer();
            tmpPlayer.viewID = viewId;
            tmpPlayer.playerObject = go;
            tmpPlayer.actor = go.GetComponentInChildren<UxrActor>();
            
            players.Add(tmpPlayer);
        }
        [PunRPC]
        public void RemoveFromList(int viewId)
        {
            SgPlayer objectToRemove = players.Find(x => x.viewID == viewId);
            if (objectToRemove != null)
            {
                players.Remove(objectToRemove);
            }
        }
}
