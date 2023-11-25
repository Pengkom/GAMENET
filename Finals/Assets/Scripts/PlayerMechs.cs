using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMechs : MonoBehaviour
{
    public Camera camera;

    public float movementSpeed = 1f;

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 mouse = Input.mousePosition;

        Vector3 moveTo = Vector3.MoveTowards(this.transform.position, camera.ScreenToWorldPoint(mouse), movementSpeed * Time.deltaTime);
        moveTo.z = 0f;

        transform.position = moveTo;
    }

}
