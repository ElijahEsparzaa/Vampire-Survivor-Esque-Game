using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    //Movement
    [HideInInspector]
    public float lastHorizontalVector;
    [HideInInspector]
    public float lastVerticalVector;
    [HideInInspector]
    public UnityEngine.Vector2 moveDir;
    [HideInInspector]
    public UnityEngine.Vector2 lastMovedVector;

    Rigidbody2D rb;
    PlayerStats player;

    // Start is called before the first frame update
    void Start()
    {
        player = GetComponent<PlayerStats>();
        rb = GetComponent<Rigidbody2D>();
        lastMovedVector = new UnityEngine.Vector2(1, 0f);
    }

    // Update is called once per frame
    void Update()
    {
        InputManagement();
    }

    void FixedUpdate()
    {
        Move();
    }

    void InputManagement()
    {
        if (GameManager.instance.isGameOver)
        {
            return;
        }

        float moveX = Input.GetAxisRaw("Horizontal");
        float moveY = Input.GetAxisRaw("Vertical");

        moveDir = new UnityEngine.Vector2(moveX, moveY).normalized;

        if(moveDir.x != 0)
        {
            lastHorizontalVector = moveDir.x;
            lastMovedVector = new UnityEngine.Vector2(lastHorizontalVector, 0f); //recent x direction
        }

        if(moveDir.y != 0)
        {
            lastVerticalVector = moveDir.y;
            lastMovedVector = new UnityEngine.Vector2(0f, lastVerticalVector); //recent y direction
        }

        if(moveDir.x != 0 && moveDir.y != 0)
        {
            lastMovedVector = new UnityEngine.Vector2(lastHorizontalVector, lastVerticalVector);//while moving
        }
    }

    void Move()
    {
        if (GameManager.instance.isGameOver)
        {
            return;
        }

        rb.velocity = moveDir * player.CurrentMoveSpeed;
    }
}
