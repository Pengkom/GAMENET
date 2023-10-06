using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class Shooting : MonoBehaviourPunCallbacks
{
    public Camera camera;
    public GameObject hitEffectPrefab;

    [Header("HP Related")]
    public float startHealth = 100;
    private float health;
    public Image healthBar;

    private Animator animator;

    private List<Transform> respawnPoints;

    [Header("Kill Feed")]
    public GameObject killFeedParent;
    public GameObject feedTextPrefab;
    public GameObject winText;

    private int lives = 10;

    void Start()
    {
        health = startHealth;
        healthBar.fillAmount = health / startHealth;
        animator = GetComponent<Animator>();
        respawnPoints = GameManager.instance.respawnPoints;
        killFeedParent = GameObject.FindGameObjectWithTag("Parent");
    }

    public void Fire()
    {
        RaycastHit hit;
        Ray ray = camera.ViewportPointToRay(new Vector3(0.5f, 0.5f));

        if (Physics.Raycast(ray, out hit, 200))
        {
            Debug.Log(hit.collider.gameObject.name);

            photonView.RPC("CreateHitEffects", RpcTarget.All, hit.point);

            if (hit.collider.gameObject.CompareTag("Player") && !hit.collider.gameObject.GetComponent<PhotonView>().IsMine)
            {
                hit.collider.GetComponent<PhotonView>().RPC("TakeDamage", RpcTarget.AllBuffered, 25);
            }
        }
    }

    [PunRPC]
    public void TakeDamage(int damage, PhotonMessageInfo info)
    {
        this.health -= damage;
        this.healthBar.fillAmount = health / startHealth;

        if (health <= 0)
        {
            Die();
            Debug.Log(info.Sender.NickName + " killed " + info.photonView.Owner.NickName);
            GameObject currentFeed = Instantiate(feedTextPrefab);
            currentFeed.transform.SetParent(killFeedParent.transform);
            currentFeed.GetComponent<Text>().text = info.Sender.NickName.ToString() + " killed " + info.photonView.Owner.NickName.ToString();
            Destroy(currentFeed, 5.0f);
        }
    }

    [PunRPC]
    public void CreateHitEffects(Vector3 position)
    {
        GameObject hitEffectGameObject = Instantiate(hitEffectPrefab, position, Quaternion.identity);
        Destroy(hitEffectGameObject, 0.2f);
    }

    public void Die()
    {
        if (photonView.IsMine)
        {
            animator.SetBool("isDead", true);
            StartCoroutine(RespawnCountdown());
        }
    }

    IEnumerator RespawnCountdown()
    {
        GameObject respawnText = GameObject.Find("Respawn Text");
        float respawnTime = 5.0f;

        while (respawnTime > 0)
        {
            yield return new WaitForSeconds(1.0f);
            respawnTime--;

            transform.GetComponent<PlayerMovementController>().enabled = false;
            respawnText.GetComponent<Text>().text = "You are killed. Respawning in " + respawnTime.ToString(".00"); 
        }

        animator.SetBool("isDead", false);
        respawnText.GetComponent<Text>().text = "";

        int randInt = (int)Random.Range(0, respawnPoints.Count - 1);

        this.transform.position = new Vector3(respawnPoints[randInt].position.x, 0, respawnPoints[randInt].position.z);
        transform.GetComponent<PlayerMovementController>().enabled = true;

        photonView.RPC("RegainHealth", RpcTarget.AllBuffered);
        photonView.RPC("Score", RpcTarget.AllBuffered);
    }

    [PunRPC]
    public void RegainHealth()
    {
        health = 100;
        healthBar.fillAmount = health / startHealth;
    }

    [PunRPC]
    public void Score()
    {
        lives--;

        if (lives == 0)
        {
            winText.SetActive(true);
        }
    }
}
