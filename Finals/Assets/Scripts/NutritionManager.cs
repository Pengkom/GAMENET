using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using Photon.Pun.UtilityScripts;
using ExitGames.Client.Photon;
using static GameManager;

public class NutritionManager : MonoBehaviourPunCallbacks
{
    [Header("Growth")]
    public float currentSize = 1f;
    public float nutritionValue;
    public int points = 0;
    public Collider2D circleCollider;

    [Header("Game")]
    public bool gameStart = false;

    void Start()
    {
        Invoke("SetStart", 0.5f);

        nutritionValue = 1f;
        PhotonNetwork.LocalPlayer.SetScore(points);
        CallUpdateScore();

    }

    void Update()
    {
        if (this.GetComponent<PlayerMovement>().IsAlive)
        {
            transform.localScale = Vector3.Lerp(transform.localScale, new Vector3(currentSize, currentSize, 1), Time.deltaTime * 16.0f);

            OtherCollision();
        }

    }

    private void SetStart()
    {
        gameStart = true;
    }

    private void OtherCollision()
    {
        circleCollider.enabled = false;

        Collider2D collider = Physics2D.OverlapCircle(this.transform.position, this.transform.localScale.x / 2);

        circleCollider.enabled = true;

        if (collider != null && gameStart)
        {
            if (collider.CompareTag("Food"))
            {
                photonView.RPC("EatFoodLogic", RpcTarget.AllBuffered, collider.gameObject.GetPhotonView().ViewID);
            }

            if (collider.CompareTag("Player"))
            {
                if (collider.GetComponent<NutritionManager>().nutritionValue < nutritionValue)
                {
                    currentSize *= (collider.GetComponent<NutritionManager>().nutritionValue * 0.1f);

                    photonView.RPC("EatPlayerLogic", RpcTarget.AllBuffered, collider.gameObject.GetPhotonView().ViewID);
                    GameManager.instance.ResetPos(collider.gameObject);

                    StartCoroutine(CallResetPlayer(collider.gameObject));
                }
            }
        }
    }

    [PunRPC]
    public void EatFoodLogic(int foodID)
    {
        GameObject food = PhotonView.Find(foodID).gameObject;

        currentSize *= 1.02f;
        nutritionValue += 1;
        points += 2;

        PhotonNetwork.LocalPlayer.SetScore(points);
        CallUpdateScore();

        GameManager.instance.ResetPos(food);
    }

    [PunRPC]
    public void EatPlayerLogic(int playerID)
    {
        GameObject player = PhotonView.Find(playerID).gameObject;

        points += player.GetComponent<NutritionManager>().points;
        nutritionValue += (player.GetComponent<NutritionManager>().nutritionValue * 0.1f);

        player.GetComponent<PlayerMovement>().IsAlive = false;
        player.GetComponent<SpriteRenderer>().enabled = false;
        player.GetComponent<CircleCollider2D>().enabled = false;

        PhotonNetwork.LocalPlayer.SetScore(points);
        CallUpdateScore();
    }

    IEnumerator CallResetPlayer(GameObject player)
    {
        yield return new WaitForSeconds(5);

        photonView.RPC("ResetPlayer", RpcTarget.AllBuffered, player);

    }

    [PunRPC]
    public void ResetPlayer(GameObject player)
    {
        player.GetComponent<PlayerMovement>().IsAlive = true;
        player.GetComponent<SpriteRenderer>().enabled = true;
        player.GetComponent<CircleCollider2D>().enabled = true;
        player.GetComponent<NutritionManager>().currentSize = 1f;
        player.GetComponent<NutritionManager>().nutritionValue = 1f;
        
        player.gameObject.transform.localScale = Vector3.one;
    }

    private void CallUpdateScore()
    {
        object[] data = new object[] { };

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

    #region Broke code ref
    /*  Oopsy brokesie
     private void OnTriggerEnter2D(Collider2D collision)
    {
        if (gameStart) 
        {
            if (collision.CompareTag("Food"))
            {
                currentSize *= 1.02f;
                points += 2;
            }

            if (collision.CompareTag("Player"))
            {
                if (collision.GetComponent<NutritionManager>().nutritionValue < nutritionValue)
                {
                    currentSize *= (collision.GetComponent<NutritionManager>().nutritionValue * 0.1f);

                    photonView.RPC("EatLogic", RpcTarget.AllBuffered, collision);

                    StartCoroutine(CallResetPlayer(collision.gameObject));
                }
            }
        }

        GameManager.instance.ResetPos(collision.gameObject);
    }

     private void OtherCollision()
    {
        circleCollider.enabled = false;

        Collider2D collider = Physics2D.OverlapCircle(this.transform.position, this.transform.localScale.x / 2);

        circleCollider.enabled = true;

        if (collider != null && gameStart)
        {
            if (collider.CompareTag("Food"))
            {
                photonView.RPC("EatFoodLogic", RpcTarget.AllBuffered, collider);
            }

            if (collider.CompareTag("Player"))
            {
                if (collider.GetComponent<NutritionManager>().nutritionValue < nutritionValue)
                {
                    currentSize *= (collider.GetComponent<NutritionManager>().nutritionValue * 0.1f);

                    photonView.RPC("EatPlayerLogic", RpcTarget.AllBuffered, collider);
                    GameManager.instance.ResetPos(collider.gameObject);

                    StartCoroutine(CallResetPlayer(collider.gameObject));
                }
            }
        }

    }

    [PunRPC]
    public void EatPlayerLogic(GameObject player)
    {
        points += player.GetComponent<NutritionManager>().points;
        nutritionValue += (player.GetComponent<NutritionManager>().nutritionValue * 0.1f);

        player.GetComponent<PlayerMovement>().IsAlive = false;
        player.GetComponent<SpriteRenderer>().enabled = false;
        player.GetComponent<CircleCollider2D>().enabled = false;

        //PhotonNetwork.LocalPlayer.SetScore(points);
        //CallUpdateScore(points);
    }

    [PunRPC]
    public void EatFoodLogic(GameObject food)
    {
        currentSize *= 1.02f;
        nutritionValue += 1;
        points += 2;

        //PhotonNetwork.LocalPlayer.SetScore(points);
        //CallUpdateScore(points);

        GameManager.instance.ResetPos(food.gameObject);
    }*/
    #endregion
}
