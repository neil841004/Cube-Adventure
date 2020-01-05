using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private Collision coll;
    [HideInInspector]
    public Rigidbody rb;

    [Header("Float")]
    public float margin;
    public float speed = 5f;
    public float jumpForce = 500f;

    [Space]
    [Header("Booleans")]
    public bool isJumpUp = false;
    public bool canMove = true;
    public bool isWallJump = false;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        coll = GetComponent<Collision>();
    }

    void Update()
    {
        Debug.Log(GetComponent<BetterJumping>().fallMultiplier);
        Move();
        if (Input.GetButtonDown("Jump")) // 跳躍狀態判斷
        {
            if ((!IsPushWall() && coll.OnGround()) || (IsPushWall() && coll.OnGround())) Jump();
            else if (IsPushWall() && !coll.OnGround()) WallJump();
        }
        if (IsPushWall() && !isWallJump)
        {
            rb.velocity = new Vector2(0, rb.velocity.y);
            GetComponent<BetterJumping>().fallMultiplier = 0.2f;
        }
        if(!IsPushWall()){
            GetComponent<BetterJumping>().fallMultiplier = 2.5f;
        }
        if (rb.velocity.y < 0)
        {
            isJumpUp = false;
        }
        if (isJumpUp)
        {
            if (Input.GetButtonUp("Jump"))
            {
                rb.velocity = new Vector3(rb.velocity.x, 0);
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
        rb.AddForce(Vector3.up * jumpForce);
        isJumpUp = true;
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
        rb.velocity = new Vector2(rb.velocity.x, 0);
        StopCoroutine(DisableMovement(0));
        StartCoroutine(DisableMovement(.1f));
        rb.AddForce(Vector3.up * jumpForce * 1f);
        if (coll.wallSide == 1) rb.AddForce(-Vector3.right * jumpForce * 0.8f);
        if (coll.wallSide == -1) rb.AddForce(Vector3.right * jumpForce * 0.8f);
    }

    IEnumerator DisableMovement(float time)
    {
        canMove = false;
        yield return new WaitForSeconds(time);
        canMove = true;
        isWallJump = false;
    }
}
