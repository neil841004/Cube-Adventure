﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private Collision coll;
    [HideInInspector]
    public Rigidbody rb;

    [Header("Float")]
    public float margin;
    public float speed = 8f;
    public float jumpForce = 500f;
    public float fallSpeedMax = -15f;

    [Space]
    [Header("Booleans")]
    public bool startJump = false;
    public bool isWallJump = false;
    public bool isJumpUp = false;
    public bool canMove = true;
    bool speedTime;

    [Space]
    [Header("Object")]
    public GameObject cube;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        coll = GetComponent<Collision>();
    }

    void Update()
    {
        // 跳躍狀態判斷
        if (Input.GetButtonDown("Jump")) 
        {
            if ((!IsPushWall() && coll.OnGround()) || (IsPushWall() && coll.OnGround())) startJump = true;
            else if (IsPushWall() && !coll.OnGround()) isWallJump = true;
        }

        //判斷落下
        if (rb.velocity.y < 0)
        {
            isJumpUp = false;
        }

        // 重置
        if (Input.GetKeyDown(KeyCode.R))
        {
            transform.position = new Vector3(0, 2.75f, -0.5f);
        }
    }
    private void FixedUpdate()
    {
        Move();
        Jump();
        if (IsPushWall() && !coll.OnGround()) WallJump();

        // 短按跳躍落下
        if (isJumpUp)
        {
            if (Input.GetButtonUp("Jump"))
            {
                rb.velocity = new Vector3(rb.velocity.x, 0);
            }
        }

        //下墜設最大值
        if (!isJumpUp)
        {
            if (rb.velocity.y <= fallSpeedMax)
            {
                rb.velocity = new Vector3(rb.velocity.x, fallSpeedMax);
            }
        }

        // 滑牆
        if (IsPushWall() && !isWallJump)
        {
            rb.velocity = new Vector2(0, rb.velocity.y);
            GetComponent<BetterJumping>().fallMultiplier = 0.15f;
            cube.transform.eulerAngles = new Vector3(0,Input.GetAxis("Horizontal")*-90,0);
            if (rb.velocity.y <= fallSpeedMax * 0.4f)
            {
                rb.velocity = new Vector3(rb.velocity.x, fallSpeedMax * 0.4f);
            }
        }
        if (!IsPushWall())
        {
            GetComponent<BetterJumping>().fallMultiplier = 2.5f;
            cube.transform.eulerAngles = new Vector3(rb.velocity.y*0.7f,Input.GetAxis("Horizontal")*-60,0);
        }

        // 蹬牆跳後撞牆不會卡住
        if(isWallJump && coll.OnWall()){
            rb.velocity = new Vector2(0, rb.velocity.y);
        }

        // 蹬牆跳返回加速度
        if (speedTime)
        {
            if (speed < 8) speed += 0.9f;
            if (speed >= 8)
            {
                speed = 8;
                speedTime = false;
            }
        }
    }

    void Move()
    {
        if (IsPushWall() || !canMove) return;
        float movement = Input.GetAxis("Horizontal") * speed;
        rb.velocity = new Vector2(movement, rb.velocity.y);
    }

    void Jump()
    {
        if (startJump)
        {
            rb.velocity = new Vector3(rb.velocity.x, 0);
            rb.AddForce(Vector3.up * jumpForce);
            isJumpUp = true;
            startJump = false;
        }
    }

    // 黏牆狀態
    bool IsPushWall()
    {
        if (coll.OnWall())
        {
            if (coll.wallSide == 1 && Input.GetAxis("Horizontal") > 0)
            {
                return true;
            }
            else if (coll.wallSide == -1 && Input.GetAxis("Horizontal") < 0)
            {
                return true;
            }
            else return false;
        }
        else return false;
    }

    // 蹬牆跳
    void WallJump()
    {
        if (isWallJump)
        {
            rb.velocity = new Vector2(rb.velocity.x, 0);
            StopCoroutine(DisableMovement(0));
            StartCoroutine(DisableMovement(.13f));
            rb.AddForce(Vector3.up * jumpForce * 1f);
            if (coll.wallSide == 1) rb.AddForce(-Vector3.right * jumpForce * .9f);
            if (coll.wallSide == -1) rb.AddForce(Vector3.right * jumpForce * .9f);
        }
    }

    IEnumerator DisableMovement(float time)
    {
        canMove = false;
        speed = 0;
        yield return new WaitForSeconds(time);
        canMove = true;
        isWallJump = false;
        speedTime = true;
    }
}
