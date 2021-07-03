using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TempEnemy : MonoBehaviour
{
    public float moveSpeed;
    bool moveRight = true;
    public float rightMost;
    public float leftMost;


    void Start()
    {
        //transform.Rotate(new Vector3(0, 180, 0));
    }

    void Update()
    {
        if (transform.position.x >= rightMost)
        {
            moveRight = false;
            transform.Rotate(new Vector3(0, 180, 0));
        }
            
        if (transform.position.x <= leftMost)
        {
            transform.Rotate(new Vector3(0, 180, 0));
            moveRight = true;
        }
            

        if (moveRight)
            transform.position = new Vector2(transform.position.x + moveSpeed * Time.deltaTime, transform.position.y);
        else
            transform.position = new Vector2(transform.position.x - moveSpeed * Time.deltaTime, transform.position.y);
    }
}
