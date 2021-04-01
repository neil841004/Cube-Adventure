using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class AnimationScript : MonoBehaviour
{
    private Animator anim;
    private PlayerMovement move;
    private Collision coll;
    private Rigidbody rb;
    public ParticleSystem shineParticle;
    // public GameObject shine;
    bool isShine = false;
    // public DOTweenAnimation shineAnim;
    // SpriteRenderer shineSprite;


    int wallSide = -1;
    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
        coll = GetComponent<Collision>();
        move = GetComponent<PlayerMovement>();
        rb = GetComponent<Rigidbody>();
        // shineSprite = shine.GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        if (move.bodyDownCount == 42 && !isShine)
        {
            shineParticle.Play();
            isShine = true;
        }
        else if (move.bodyDownCount < 42)
        {
            isShine = false;
        }
    }

    void LateUpdate()
    {
        if (rb.velocity.x > 0) wallSide = 1;
        else if (rb.velocity.x < 0) wallSide = -1;
        if (coll.onRightWall.Length > 0) wallSide = 1;
        if (coll.onLeftWall.Length > 0) wallSide = -1;
        anim.SetBool("onGround", coll.OnGround());
        anim.SetBool("isJump", move.isJump);
        anim.SetBool("isDash", move.isAnimDash);
        anim.SetBool("isWallJump", move.isWallJumpAnim);
        anim.SetBool("isDownJump", move.isDownJump);
        anim.SetBool("isPushWall", move.isPushWallAnim);
        anim.SetBool("onGroundDash", coll.OnGroundDash());
        anim.SetBool("onMovePF", coll.onMovePF);
        anim.SetFloat("ySpeed", rb.velocity.y);
        anim.SetFloat("xSpeed", Mathf.Abs(rb.velocity.x));
        anim.SetFloat("fallLandSpeed", move.fallLandSpeed);
        anim.SetInteger("wallSide", wallSide);
    }
}
