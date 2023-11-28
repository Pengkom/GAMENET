using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public Camera camera;

    public float movementSpeed = 1f;
    public bool IsAlive = true;

    void LateUpdate()
    {
        if (IsAlive)
        {
            Movement();
            
            camera.orthographicSize = Mathf.Lerp(camera.orthographicSize, 4 * this.gameObject.transform.localScale.x, Time.deltaTime);
            camera.transform.rotation = Quaternion.Euler(Vector3.zero);
        }

    }

    private void Movement()
    {
        Vector2 mouse = Input.mousePosition;

        Vector3 moveTo = Vector3.MoveTowards(this.transform.position, camera.ScreenToWorldPoint(mouse), movementSpeed * Time.deltaTime);
        moveTo.z = 0f;

        transform.position = moveTo;
    }

}
