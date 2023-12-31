using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

public class GameManager : MonoBehaviourPunCallbacks
{
    [SerializeField] private GameObject playerPrefab;

    public static GameManager instance;

    private void Awake()
    {
        if (instance != null)
        {
            Destroy(this.gameObject);
        }
        else
        {
            instance = this;
        }
    }

    void Start()
    {
        if (PhotonNetwork.IsConnected)
        {
            if (playerPrefab != null)
            {
                StartCoroutine(DelayedPlayerSpawn());
            }
        }
    }

    public override void OnJoinedRoom()
    {
        Debug.Log("Player:" + PhotonNetwork.NickName + " has joined the Room!");
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        Debug.Log("Player:" + newPlayer.NickName + " has joined the Room " + PhotonNetwork.CurrentRoom.Name);
        Debug.Log("Room has now " + PhotonNetwork.CurrentRoom.PlayerCount + "/20 players.");
    }

    IEnumerator DelayedPlayerSpawn()
    {
        yield return new WaitForSeconds(3);
        int xRandomPoint = Random.Range(-20, 20);
        int zRandomPoint = Random.Range(-20, 20);
        PhotonNetwork.Instantiate(playerPrefab.name, new Vector3(xRandomPoint, 0.1f, zRandomPoint), Quaternion.identity);
    }

    public override void OnLeftRoom()
    {
        SceneManager.LoadScene("GameLauncherScene");
    }
    
    public void LeaveRoom()
    {
        PhotonNetwork.LeaveRoom();
    }
}
