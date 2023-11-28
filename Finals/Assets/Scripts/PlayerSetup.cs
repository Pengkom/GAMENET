using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class PlayerSetup : MonoBehaviourPunCallbacks
{
    public Camera camera;

    void Start()
    {
        this.GetComponent<PlayerMovement>().enabled = photonView.IsMine;
        this.GetComponent<NutritionManager>().enabled = photonView.IsMine;
        camera.enabled = photonView.IsMine;
    }
}
