using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private Collision coll;
    [HideInInspector]
    public Rigidbody rb;
    public float margin;
    public float speed = 5f;
    public float jumpForce = 500f;

    [Space]
    [Header("Booleans")]
    public bool isJump = false;
    public bool canMove = true;
    public bool isWallJump = false;
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        coll = GetComponent<Collision>();
    }
    private void Update()
    {
        if (Input.GetButtonDown("Jump"))
        {
            if (coll.OnGround()) { Jump(); }
            if (IsPushWall() && !coll.OnGround()) WallJump();
        }
        if (IsPushWall())
        {
            rb.velocity = new Vector2(rb.velocity.x, -0.5f);
        }
        if (Input.GetKeyDown(KeyCode.UpArrow) && canMove)
        {
            canMove = false;
        }
        else if (Input.GetKeyDown(KeyCode.UpArrow) && !canMove)
        {
            canMove = true;
        }
        if (coll.OnGround())
        {
            isWallJump = false;
        }

    }
    private void FixedUpdate()
    {
        if (!IsPushWall() && canMove)
        {
            Move();
        }
    }
    void Move()
    {
        float movement = Input.GetAxis("Horizontal") * speed;
        if (!isWallJump)
        {
            rb.velocity = new Vector2(movement, rb.velocity.y);
        }
    }
    void Jump()
    {
        rb.AddForce(Vector3.up * jumpForce);
    }


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
    void WallJump()
    {
        StopCoroutine(DisableMovement(0));
        StartCoroutine(DisableMovement(.1f));
        rb.AddForce(Vector3.up * jumpForce / 1.5f);
        if (coll.wallSide == 1) rb.AddForce(-Vector3.right * jumpForce * 1.3f);
        if (coll.wallSide == -1) rb.AddForce(Vector3.right * jumpForce * 1.3f);
        isWallJump = true;
        if (isWallJump)
        {
            rb.velocity = Vector2.Lerp(rb.velocity, (new Vector2(Input.GetAxis("Horizontal") * speed, rb.velocity.y)), 10 * Time.deltaTime);
        }
    }
    IEnumerator DisableMovement(float time)
    {
        canMove = false;
        yield return new WaitForSeconds(time);
        canMove = true;
    }

}
