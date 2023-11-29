using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using System;
using UnityEngine.UIElements;
using UnityEngine.UI;
using Photon.Realtime;
using ExitGames.Client.Photon.StructWrapping;
using Random = UnityEngine.Random;
using Photon.Pun.Demo.Cockpit;
using UnityEngine.Tilemaps;
using Unity.VisualScripting;
using Photon.Pun.UtilityScripts;
using ExitGames.Client.Photon;
//Why is there so many wtf

public class GameManager : MonoBehaviourPunCallbacks
{
    public static GameManager instance = null;

    public int scoreToWin = 0;
    public GameObject playerPrefab;
    public List<Player> playerList = new List<Player>();
    public GameObject[] scoreBoard;
    public GameObject winDisplay;

    [Header("Map Values")]
    public int initFoodToSpawn;
    public GameObject foodPrefab;
    public GameObject map;
    private Tilemap mapTilemap;
    private Vector3 minWorldPos;
    private Vector3 maxWorldPos;

    public enum RaiseEventsCode
    {
        UpdateScoreEventCode = 0
    }

    private void OnEnable()
    {
        PhotonNetwork.NetworkingClient.EventReceived += OnEvent;
    }

    private void OnDisable()
    {
        PhotonNetwork.NetworkingClient.EventReceived -= OnEvent;
    }

    private void OnEvent(EventData photonEvent)
    {
        if (photonEvent.Code == (byte)RaiseEventsCode.UpdateScoreEventCode)
        {
            playerList.Clear();

            for (int i = 0; i < PhotonNetwork.CurrentRoom.PlayerCount; i++)
            {
                Player tempPlayer;
                PhotonNetwork.CurrentRoom.Players.TryGetValue(i+1, out tempPlayer); //i+1 took too long to debug :D
                if (tempPlayer != null)
                {
                    playerList.Add(tempPlayer);
                    int score = playerList[i].GetScore();
                    scoreBoard[i].SetActive(true);

                    //Why does it change both index 0 and 1
                    scoreBoard[i].GetComponent<Text>().text = 
                        score + "/" + scoreToWin + " " + playerList[i].NickName;

                    if (score >= scoreToWin)
                    {
                        winDisplay.SetActive(true);
                        winDisplay.GetComponent<Text>().text = playerList[i].NickName + " has Won!";
                        Time.timeScale = 0;
                    }
                }
            }
        }
    }

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

    void Start()
    {
        mapTilemap = map.GetComponent<Tilemap>();
        BoundsInt bounds = mapTilemap.cellBounds;
        minWorldPos = mapTilemap.GetCellCenterWorld(new Vector3Int(bounds.min.x, bounds.min.y, 0));
        maxWorldPos = mapTilemap.GetCellCenterWorld(new Vector3Int(bounds.max.x, bounds.max.y, 0));

        int tempScore;
        PhotonNetwork.CurrentRoom.CustomProperties.TryGetValue("WinPoints", out tempScore);
        scoreToWin = tempScore;

        foreach (GameObject sb in scoreBoard)
        {
            sb.SetActive(false);
        }
        winDisplay.SetActive(false);

        // Main Logic =======================================================================================
        if (PhotonNetwork.IsConnectedAndReady)
        {
            int foodToSpawn = initFoodToSpawn * PhotonNetwork.CurrentRoom.PlayerCount;
            for (int i = 0; i < foodToSpawn; i++)
            {
                SpawnFood();
            }

            PhotonNetwork.Instantiate(playerPrefab.name, GenerateSpawnPos(), Quaternion.identity);

        }

    }

    public void SpawnFood()
    {
        GameObject currentFood = PhotonNetwork.Instantiate(foodPrefab.name, GenerateSpawnPos(), Quaternion.identity);
        currentFood.GetComponent<SpriteRenderer>().color = Random.ColorHSV(0.0f, 1.0f, 0.9f, 1.0f, 0.9f, 1.0f);

    }

    public void ResetPos(GameObject gameObject)
    {
        gameObject.transform.position = GenerateSpawnPos();
    }

    private Vector3 GenerateSpawnPos()
    {
        float generateX = Random.Range(minWorldPos.x, maxWorldPos.x);
        float generateY = Random.Range(minWorldPos.y, maxWorldPos.y);

        return new Vector3(generateX, generateY, 0);
    }

    public void UpdateScoreBoard()
    {
        int o = 0;
        object[] data = new object[] { o };

        RaiseEventOptions raiseEventOptions = new RaiseEventOptions
        {
            Receivers = ReceiverGroup.All,
            CachingOption = EventCaching.AddToRoomCache
        };

        SendOptions sendOptions = new SendOptions
        {
            Reliability = false
        };

        PhotonNetwork.RaiseEvent((byte)RaiseEventsCode.UpdateScoreEventCode, data, raiseEventOptions, sendOptions);
    }

}
