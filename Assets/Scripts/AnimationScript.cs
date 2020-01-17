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
    void LateUpdate()
    {
        if (coll.OnGround() && !move.IsPushWall())
        {
            anim.SetBool("onGround", true);
        }else 
        {
            anim.SetBool("onGround", false);
        }
        anim.SetBool("isJump", move.isJump);
        anim.SetBool("isDash", move.isAnimDash);
        anim.SetBool("isWallJump", move.isWallJumpAnim);
        anim.SetBool("isPushWall", move.isPushWallAnim);
        anim.SetBool("onGroundDash", coll.OnGroundDash());
        anim.SetFloat("ySpeed", rb.velocity.y);
        anim.SetFloat("xSpeed", Mathf.Abs(rb.velocity.x));
        anim.SetFloat("fallLandSpeed", move.fallLandSpeed);
        anim.SetInteger("wallSide", coll.wallSide);
    }
}
