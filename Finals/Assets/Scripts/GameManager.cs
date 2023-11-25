using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using System;
using UnityEngine.UIElements;
using UnityEngine.UI;
using Photon.Realtime;
using UnityEditor.XR;
using ExitGames.Client.Photon.StructWrapping;

public class GameManager : MonoBehaviour
{
    public GameObject[] finisherTextUI;

    public static GameManager instance = null;

    public int scoreToWin = 0;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }

        DontDestroyOnLoad(gameObject);
    }   

    // Start is called before the first frame update
    void Start()
    {
        Application.targetFrameRate = 60;

        int tempScore;
        PhotonNetwork.CurrentRoom.CustomProperties.TryGetValue("WinPoints", out tempScore);
        scoreToWin = tempScore;

        if (PhotonNetwork.IsConnectedAndReady)
        {
            //Vector3 instantiatePosition = random;
            //PhotonNetwork.Instantiate(BlobPrefab.name, instantiatePosition, Quaternion.identity);
        }

        foreach (GameObject go in finisherTextUI)
        {
            go.SetActive(false);
        }
    }
}
