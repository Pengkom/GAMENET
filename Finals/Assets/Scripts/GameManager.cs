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

public class GameManager : MonoBehaviour
{
    public static GameManager instance = null;

    public GameObject[] finisherTextUI;

    public int scoreToWin = 0;
    public GameObject playerPrefab;

    [Header("Map Values")]
    public int initFoodToSpawn;
    public GameObject foodPrefab;
    public GameObject map;
    private Tilemap mapTilemap;
    private Vector3 minWorldPos;
    private Vector3 maxWorldPos;

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
        mapTilemap = map.GetComponent<Tilemap>();
        BoundsInt bounds = mapTilemap.cellBounds;
        minWorldPos = mapTilemap.GetCellCenterWorld(new Vector3Int(bounds.min.x, bounds.min.y, 0));
        maxWorldPos = mapTilemap.GetCellCenterWorld(new Vector3Int(bounds.max.x, bounds.max.y, 0));

        int tempScore;
        PhotonNetwork.CurrentRoom.CustomProperties.TryGetValue("WinPoints", out tempScore);
        scoreToWin = tempScore;

        if (PhotonNetwork.IsConnectedAndReady)
        {
            PhotonNetwork.Instantiate(playerPrefab.name, GenerateSpawnPos(), Quaternion.identity);

            int foodToSpawn = initFoodToSpawn * PhotonNetwork.CurrentRoom.MaxPlayers;
            for (int i = 0; i < foodToSpawn; i++)
            {
                SpawnFood();
            }
        }

        foreach (GameObject go in finisherTextUI)
        {
            go.SetActive(false);
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
}
