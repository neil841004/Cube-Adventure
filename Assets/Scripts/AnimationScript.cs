using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationScript : MonoBehaviour
{
    private Animator anim;
    private PlayerMovement move;
    private Collision coll;
    private Rigidbody rb;

    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
        coll = GetComponent<Collision>();
        move = GetComponent<PlayerMovement>();
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        anim.SetBool("onGround",coll.OnGround());
        anim.SetBool("isJump",move.isJump);
        anim.SetBool("isDash",move.isAnimDash);
        anim.SetBool("isWallJump",move.isWallJump);
        anim.SetBool("isPushWall",move.IsPushWall());
        anim.SetFloat("ySpeed",rb.velocity.y);
        anim.SetFloat("xSpeed",Mathf.Abs(rb.velocity.x));
    }
}
