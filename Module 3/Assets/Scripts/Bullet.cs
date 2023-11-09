using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Bullet : MonoBehaviourPunCallbacks
{
    private bool hit = false;

    private void Start()
    {
        Invoke("Delet", 3.0f);
    }

    void Update()
    {
        transform.Translate(Vector3.forward * Time.deltaTime * 40);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") == true && !hit)
        {
            other.GetComponent<PhotonView>().RPC("TakeDamage", RpcTarget.AllBuffered, 25);
            Delet();
            hit = false;
            Debug.Log(other.GetComponent<Collider>());
        }
    }

    private void Delet()
    {
        Destroy(this.gameObject);
    }
}
