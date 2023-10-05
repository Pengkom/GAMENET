using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class GameManager : MonoBehaviour
{
    public GameObject playerPrefab;

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
        int randomPointX = Random.Range(-10, 10);
        int randomPointZ = Random.Range(-10, 10);
        PhotonNetwork.Instantiate(playerPrefab.name, new Vector3(randomPointX, 0, randomPointZ), Quaternion.identity);
    }
}
