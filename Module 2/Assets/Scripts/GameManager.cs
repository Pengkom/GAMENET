using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.UIElements;

public class GameManager : MonoBehaviour
{
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

    public GameObject playerPrefab;
    public List<Transform> respawnPoints = new List<Transform>();

    void Start()
    {
        if (PhotonNetwork.IsConnectedAndReady)
        {
            StartCoroutine(DelayedPlayerSpawn());
        }
    }

    IEnumerator DelayedPlayerSpawn()
    {
        yield return new WaitForSeconds(1);
        int randInt = (int)Random.Range(0, respawnPoints.Count - 1);
        Debug.Log(randInt);

        GameObject player = PhotonNetwork.Instantiate(playerPrefab.name, 
            new Vector3(respawnPoints[randInt].position.x, 0, respawnPoints[randInt].position.z), 
            Quaternion.identity);
    }
}
