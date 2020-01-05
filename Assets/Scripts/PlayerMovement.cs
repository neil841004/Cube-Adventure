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
    public bool isJumpUp = false;
    public bool canMove = true;
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        coll = GetComponent<Collision>();
    }
    private void Update()
    {
        if (Input.GetButtonDown("Jump"))
        {
            if (coll.OnGround()) Jump();
            if (IsPushWall() && !coll.OnGround()) WallJump();
        }
        if (IsPushWall())
        {
            rb.velocity = new Vector2(rb.velocity.x, -0.8f);
        }
        Debug.Log(Input.GetAxis("Horizontal"));
        if(rb.velocity.y<0){
            isJumpUp = false;
        }
        if(isJumpUp){
            if(Input.GetButtonUp("Jump")){
            rb.velocity = new Vector3(rb.velocity.x,0);
        }
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
        rb.velocity = new Vector2(movement, rb.velocity.y);
    }
    void Jump()
    {
        rb.AddForce(Vector3.up * jumpForce);
        isJumpUp = true;
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
        rb.AddForce(Vector3.up * jumpForce*1f);
        if (coll.wallSide == 1) rb.AddForce(-Vector3.right * jumpForce * .8f);
        if (coll.wallSide == -1) rb.AddForce(Vector3.right * jumpForce * .8f);
    }
    IEnumerator DisableMovement(float time)
    {
        canMove = false;
        yield return new WaitForSeconds(time);
        canMove = true;
    }

}
