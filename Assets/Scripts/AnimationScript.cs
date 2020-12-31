﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class AnimationScript : MonoBehaviour
{
    private Animator anim;
    private PlayerMovement move;
    private Collision coll;
    private Rigidbody rb;
    public GameObject shine;
    bool isShine = false;
    public DOTweenAnimation shineAnim;


    int wallSide = -1;
    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
        coll = GetComponent<Collision>();
        move = GetComponent<PlayerMovement>();
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        if (move.bodyDownCount == 50 && !isShine)
        {
            shine.GetComponent<SpriteRenderer>().enabled = true;
            shineAnim.DORestart();
            isShine = true;
        }
        else if (move.bodyDownCount < 50)
        {
            isShine = false;
            
        }
    }
    // Update is called once per frame
    void LateUpdate()
    {
        if (rb.velocity.x > 0) wallSide = 1;
        else if (rb.velocity.x < 0) wallSide = -1;
        if (coll.onRightWall.Length > 0) wallSide = 1;
        if (coll.onLeftWall.Length > 0) wallSide = -1;
        // if (coll.OnGround() && !move.IsPushWall())
        // {
        //     anim.SetBool("onGround", true);
        // }else 
        // {
        //     anim.SetBool("onGround", false);
        // }
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
        //anim.SetFloat("xRaw", move.xRaw);
        anim.SetInteger("wallSide", wallSide);
    }
}
