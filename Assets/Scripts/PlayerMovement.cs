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
    public float UpSpeedMimi = 3.5f;

    [Space]
    [Header("Booleans")]
    public bool startJump = false;
    public bool isWallJump = false; // 開始跳躍並持續一段時間
    public bool callWallJump = false; // 開始跳躍後即關閉
    public bool isJumpUp = false; // Y軸速度>0
    public bool canMove = true;
    public bool isStickWall = false; //跳躍到另一個牆上時
    public bool canStickWall = false; //從牆壁跳躍後的一個短瞬間內為true
    public bool speedTime = false;
    public bool fall = false; //判斷是否放開跳躍鍵

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
            else if ((IsPushWall() && !coll.OnGround()) || isStickWall && !coll.OnGround()) callWallJump = true;
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
        else if (Input.GetKeyDown(KeyCode.T))
        {
            transform.position = new Vector3(-1.494052f, 20.75f, -0.5f);
        }

        // 蹬牆轉向時速度增加
        if (isWallJump)
        {
            if (coll.wallSide == 1 && Input.GetAxisRaw("Horizontal") == -1) speed = 8;
            else if (coll.wallSide == -1 && Input.GetAxisRaw("Horizontal") == 1) speed = 8;
        }
    }
    private void FixedUpdate()
    {
        Move();
        Jump();
        if ((IsPushWall() && !coll.OnGround() && !isWallJump && callWallJump) || (isStickWall && !coll.OnGround() && !isWallJump && callWallJump)) WallJump();

        // 短按跳躍落下
        if (isJumpUp)
        {
            if (Input.GetButtonUp("Jump"))
            {
                fall = true;
            }
        }
        if (fall && rb.velocity.y <= UpSpeedMimi)
        {
            rb.velocity = new Vector2(rb.velocity.x, 0);
            fall = false;
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
            if (!isStickWall)
                cube.transform.eulerAngles = new Vector3(0, Input.GetAxis("Horizontal") * -90, 0);
            if (rb.velocity.y <= fallSpeedMax * 0.4f)
            {
                rb.velocity = new Vector3(rb.velocity.x, fallSpeedMax * 0.4f);
            }
        }
        if (!IsPushWall() && !isWallJump && !isStickWall)
        {
            GetComponent<BetterJumping>().fallMultiplier = 2.5f;
            if (!isStickWall)
                cube.transform.eulerAngles = new Vector3(rb.velocity.y * 0.7f, Input.GetAxis("Horizontal") * -60, 0);
        }

        // 黏牆
        if (IsPushWall() && !isStickWall && !coll.OnGround() && !isWallJump)
        {
            StopCoroutine("StickWall");
            isStickWall = true;
            speed = 0;
            rb.velocity = new Vector2(0, rb.velocity.y);
            cube.transform.eulerAngles = new Vector3(rb.velocity.y * 0.7f, coll.wallSide * -90, 0);
            canStickWall = false;
        }
        if (!IsPushWall() && isStickWall && coll.OnWall() && !canStickWall)
        {
            StartCoroutine("StickWall");
        }
        if (coll.OnGround())
        {
            StopCoroutine("StickWall");
            speedTime = true;
            isStickWall = false;
        }
        if (isWallJump && coll.OnWall() && canStickWall)
        {
            StopCoroutine("StickWall");
            StartCoroutine("StickWall");
            canStickWall = false;
        }
        if (isStickWall && !coll.OnWall())
        {
            StopCoroutine("StickWall");
            isStickWall = false;
            if(!isWallJump)
            speedTime = true;
        }

        // 蹬牆跳返回加速度
        if (speedTime)
        {
            // if (speed < 8) speed = Mathf.Lerp(speed,8,Time.deltaTime*8f);
            if (speed < 8) speed += 0.4f;
            if (speed >= 7)
            {
                speed = 8;
                speedTime = false;
            }
        }
    }

    void Move()
    {
        if (IsPushWall() || !canMove || isStickWall) return;
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
        isWallJump = true;
        rb.velocity = new Vector2(0, 0);
        StopCoroutine("DisableMovement");
        StartCoroutine("DisableMovement");
        StopCoroutine("canStickWallEnumerator");
        StartCoroutine("canStickWallEnumerator");
        rb.AddForce(Vector3.up * jumpForce * 1f);
        if (coll.wallSide == 1) rb.AddForce(-Vector3.right * jumpForce * .8f);
        if (coll.wallSide == -1) rb.AddForce(Vector3.right * jumpForce * .8f);
        callWallJump = false;

    }

    IEnumerator DisableMovement()
    {
        canMove = false;
        speed = 0;
        yield return new WaitForSeconds(.18f);
        canMove = true;
        isWallJump = false;
        if (!isStickWall) speedTime = true;
    }

    IEnumerator canStickWallEnumerator()
    {
        canStickWall = false;
        yield return new WaitForSeconds(.02f);
        canStickWall = true;
    }

    IEnumerator StickWall()
    {
        isStickWall = true;
        speed = 0;
        GetComponent<BetterJumping>().fallMultiplier = 0.15f;
        rb.velocity = new Vector2(0, rb.velocity.y);
        cube.transform.eulerAngles = new Vector3(rb.velocity.y * 0.7f, coll.wallSide * -90, 0);
        yield return new WaitForSeconds(.35f);
        speedTime = true;
        isStickWall = false;
        GetComponent<BetterJumping>().fallMultiplier = 2.5f;
    }
}
