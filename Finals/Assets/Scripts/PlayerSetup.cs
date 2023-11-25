using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class PlayerSetup : MonoBehaviourPunCallbacks
{
    public Camera camera;

    // Start is called before the first frame update
    void Start()
    {
        this.camera = transform.Find("Camera").GetComponent<Camera>();
        //GetComponent<VehicleMovement>().enabled = photonView.IsMine;
        camera.enabled = photonView.IsMine;
    }
}
