using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Pun.Demo.PunBasics;
using Photon.Realtime;
using ExitGames.Client.Photon;
using static LapController;

public class Shooting : MonoBehaviourPunCallbacks
{
    public Camera camera;

    public enum RaiseEventsCodes
    {
        WhoDiedEventCode = 0
    }

    public int playerCount;

    [Header("HP Related")]
    public float startHealth = 100;
    private float health;

    [Header("Projectile Related")]
    public GameObject bulletPrefab;
    public Transform gunBarrel;

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
        if (photonEvent.Code == (byte)RaiseEventsCodes.WhoDiedEventCode)
        {
            object[] data = (object[])photonEvent.CustomData;

            string nickNameOfFinishedPlayer = (string)data[0];
            playerCount = (int)data[1];
            int viewID = (int)data[2];

            GameObject orderUIText = RacingGameManager.instance.finisherTextUI[playerCount - 1];
            orderUIText.SetActive(true);

            if (viewID == photonView.ViewID)
            {
                orderUIText.GetComponent<Text>().text = playerCount + " " + nickNameOfFinishedPlayer + "(YOU)";
                orderUIText.GetComponent<Text>().color = Color.green;
            }
            else
            {
                orderUIText.GetComponent<Text>().text = playerCount + " " + nickNameOfFinishedPlayer;
            }
        }

    }

    void Start()
    {
        health = startHealth;
        playerCount = PhotonNetwork.CurrentRoom.PlayerCount;
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            FireLaser();
        }

        if (Input.GetMouseButtonDown(1))
        {
            FireProjectile();
        }
    }

    public void FireLaser()
    {
        RaycastHit hit;
        Ray ray = camera.ViewportPointToRay(new Vector3(0.5f, 0.6f));

        if (Physics.Raycast(ray, out hit, 200))
        {
            Debug.Log(hit.collider.gameObject.name);

            if (hit.collider.gameObject.CompareTag("Player") && !hit.collider.gameObject.GetComponent<PhotonView>().IsMine)
            {
                hit.collider.GetComponent<PhotonView>().RPC("TakeDamage", RpcTarget.AllBuffered, 25);
            }
        }
    }

    public void FireProjectile()
    {
        GameObject bullet = Instantiate(bulletPrefab, gunBarrel.position, gunBarrel.rotation);
        bullet.GetComponent<Rigidbody>().AddForce(gunBarrel.forward * 1400);
        Destroy(bullet, 3.0f);
    }

    [PunRPC]
    public void TakeDamage(int damage, PhotonMessageInfo info)
    {
        this.health  = this.health - damage;
        health = Mathf.Clamp(health, 0, startHealth);

        Debug.Log(health);

        if (health <= 0)
        {
            Die();
        }
    }

    public void Die()
    {
        Debug.Log("Called DIE");

        GetComponent<PlayerSetup>().camera.transform.parent = null;
        GetComponent<VehicleMovement>().enabled = false;
        GetComponent<BoxCollider>().enabled = false;
        GetComponent<Rigidbody>().useGravity = false;
        this.enabled = false;

        string nickName = photonView.Owner.NickName;
        int viewID = photonView.ViewID;

        //event data
        object[] data = new object[] { nickName, playerCount, viewID };

        RaiseEventOptions raiseEventOptions = new RaiseEventOptions
        {
            Receivers = ReceiverGroup.All,
            CachingOption = EventCaching.AddToRoomCache
        };

        SendOptions sendOptions = new SendOptions
        {
            Reliability = false
        };

        //I can't debug what's wrong with my Win function and this thing
        PhotonNetwork.RaiseEvent((byte)RaiseEventsCodes.WhoDiedEventCode, data, raiseEventOptions, sendOptions);

        playerCount--;
        playerCount = Mathf.Clamp(playerCount, 1, 4);

        if (playerCount == 1)
        {
            Win();
        }
    }

    public void Win()
    {
        Shooting[] players = FindObjectsOfType<Shooting>();

        foreach (Shooting sh in players)
        {
            if (sh.health > 0)
            {
                string nickName = sh.photonView.Owner.NickName;
                int viewID = sh.photonView.ViewID;
                Debug.Log("Win Called");

                //event data
                object[] data = new object[] { nickName, playerCount, viewID };

                RaiseEventOptions raiseEventOptions = new RaiseEventOptions
                {
                    Receivers = ReceiverGroup.All,
                    CachingOption = EventCaching.AddToRoomCache
                };

                SendOptions sendOptions = new SendOptions
                {
                    Reliability = false
                };

                PhotonNetwork.RaiseEvent((byte)RaiseEventsCodes.WhoDiedEventCode, data, raiseEventOptions, sendOptions);
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Bullet") && health > 0)
        {
            photonView.RPC("TakeDamage", RpcTarget.AllBuffered, 25);
            Destroy(other.gameObject);
        }
    }
}
