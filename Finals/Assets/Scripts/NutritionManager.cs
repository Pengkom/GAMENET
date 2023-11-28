using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class NutritionManager : MonoBehaviourPunCallbacks
{
    [Header("Growth")]
    public float currentSize = 1f;
    public float nutritionValue;
    public int points;

    [Header("Game")]
    public bool gameStart = false;

    void Start()
    {
        Invoke("SetStart", 0.5f);

        nutritionValue = 1f;
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
        this.GetComponent<CircleCollider2D>().enabled = false;

        Collider2D collider = Physics2D.OverlapCircle(this.transform.position, this.transform.localScale.x / 2);

        this.GetComponent<CircleCollider2D>().enabled = true;

        if (collider != null && gameStart)
        {
            if (collider.CompareTag("Food"))
            {
                currentSize *= 1.02f;
                nutritionValue += 1;
                points += 2;

                GameManager.instance.ResetPos(collider.gameObject);
            }

            if (collider.CompareTag("Player"))
            {
                if (collider.GetComponent<NutritionManager>().nutritionValue < nutritionValue)
                {
                    currentSize *= (collider.GetComponent<NutritionManager>().nutritionValue * 0.1f);

                    photonView.RPC("EatLogic", RpcTarget.AllBuffered, collider);
                    GameManager.instance.ResetPos(collider.gameObject);

                    StartCoroutine(CallResetPlayer(collider.gameObject));
                }
            }
        }
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
/*        if (gameStart) //Oopsy brokesies
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

        GameManager.instance.ResetPos(collision.gameObject);*/

    }

    [PunRPC]
    public void EatLogic(GameObject player)
    {
        
        points += player.GetComponent<NutritionManager>().points;
        nutritionValue += (player.GetComponent<NutritionManager>().nutritionValue * 0.1f);

        player.GetComponent<PlayerMovement>().IsAlive = false;
        player.GetComponent<SpriteRenderer>().enabled = false;
        player.GetComponent<CircleCollider2D>().enabled = false;
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

}
