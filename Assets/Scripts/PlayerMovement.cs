using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class PlayerMovement : MonoBehaviour
{
    private Collision coll;
    private Animator anim;
    [HideInInspector]
    public Rigidbody rb;

    [Header("Float")]
    public float speed = 8f;
    public float jumpForce = 500f;
    public float fallSpeedMax = -15f;
    public float UpSpeedMimi = 3.5f;
    public float dashSpeed = 20;
    public float fallLandSpeed;
    public float fallMultiplier = 2.35f;

    float x;
    float xRaw;

    [Space]

    [Header("Booleans")]
    public bool isJumpUp = false; // Y軸速度>0
    public bool startJump = false;
    public bool isWallJump = false; // 開始跳躍並持續一段時間
    public bool isWallJumpAnim = false;
    public bool isJump = false;
    bool callWallJump = false; // 開始跳躍後即關閉
    bool fall = false; //判斷是否放開跳躍鍵

    public bool canMove = true;

    public bool isStickWall = false; //跳躍到另一個牆上時
    bool canStickWall = false; //從牆壁跳躍後的一個短瞬間內為true

    public bool canEdgeJump = false; //是否可以GhostJump
    bool EdgeJumpFlag = false; //GhostJump的Flag

    public bool hasDashed = false; //擁有衝刺能量
    public bool isDash = false; //衝刺過程
    public bool isAnimDash = false; //衝刺動畫

    [Space]
    [Header("Object")]
    public GameObject cube;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        coll = GetComponent<Collision>();
        anim = GetComponent<Animator>();
    }

    void Update()
    {
        x = Input.GetAxis("Horizontal");
        xRaw = Input.GetAxisRaw("Horizontal");

        EdgeJump();

        // 跳躍狀態判斷
        if (Input.GetButtonDown("Jump"))
        {
            if ((!IsPushWall() && coll.OnGround()) || (IsPushWall() && coll.OnGround()) || (coll.OnEdge() && !coll.OnGround() && !coll.OnWall() && canEdgeJump)) startJump = true;
            else if ((IsPushWall() && !coll.OnGround()) || isStickWall && !coll.OnGround()) callWallJump = true;
        }

        //衝刺
        if (Input.GetButtonDown("Dash") && hasDashed && !isStickWall)
        {
            if (xRaw != 0) Dash(xRaw);
        }
        if (!isDash && !IsPushWall())
        {
            if (coll.OnGroundDash()) hasDashed = true;
        }

        if (IsPushWall() || coll.OnGround()) isJump = false;

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
            transform.position = new Vector3(6.038958f, 10.25f, -0.5f);
        }

        // 蹬牆轉向時速度增加
        if (isWallJump)
        {
            if (coll.wallSide == 1 && Input.GetAxisRaw("Horizontal") == -1) speed = 8;
            else if (coll.wallSide == -1 && Input.GetAxisRaw("Horizontal") == 1) speed = 8;
        }

        //牆跳動畫
        if(isWallJumpAnim){
            if(IsPushWall() || coll.OnGround() || isDash) isWallJumpAnim = false;
        }

        //方塊轉向
        if (isStickWall || IsPushWall()) cube.transform.DORotate(new Vector3(0, coll.wallSide * -90, 0), 0.07f);
        else if (!isStickWall) cube.transform.DORotate(new Vector3(0, x * -60, 0), .18f);
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
            if (rb.velocity.y <= fallSpeedMax * 0.4f)
            {
                rb.velocity = new Vector3(rb.velocity.x, fallSpeedMax * 0.4f);
            }
        }
        if (!IsPushWall() && !isWallJump && !isStickWall)
        {
            GetComponent<BetterJumping>().fallMultiplier = fallMultiplier;
        }

        // 黏牆
        if (IsPushWall() && !isStickWall && !coll.OnGround() && !isWallJump)
        {
            StopCoroutine("StickWall");
            isStickWall = true;
            speed = 0;
            rb.velocity = new Vector2(0, rb.velocity.y);
            canStickWall = false;
        }
        if (!IsPushWall() && isStickWall && coll.OnWall() && !canStickWall)
        {
            StartCoroutine("StickWall");
        }
        if (coll.OnGround())
        {
            StopCoroutine("StickWall");
            if(isStickWall)speed = 8;
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
            if (!isWallJump) DOVirtual.Float(0, 8, .5f, speedBackOrigin);
        }

        if(!coll.OnGround()){
            fallLandSpeed = rb.velocity.y;
        }
    }

    void Move()
    {
        if (IsPushWall() || !canMove || isStickWall) return;
        float movement = x * speed;
        rb.velocity = new Vector2(movement, rb.velocity.y);
    }

    void Jump()
    {
        if (startJump)
        {
            isJump = true;
            hasDashed = true;
            rb.velocity = new Vector3(rb.velocity.x, 0);
            rb.AddForce(Vector3.up * jumpForce);
            isJumpUp = true;
            startJump = false;
            canEdgeJump = false;
            StopCoroutine("EdgeJumpIEumerator");
        }
    }

    //Ghost Jump
    void EdgeJump()
    {
        if (coll.OnGround())
        {
            canEdgeJump = false;
            EdgeJumpFlag = true;
            StopCoroutine("EdgeJumpIEumerator");
        }
        if (canEdgeJump) return;
        if (!coll.OnGround() && EdgeJumpFlag)
        {
            canEdgeJump = true;
            StartCoroutine("EdgeJumpIEumerator");
            EdgeJumpFlag = false;
        }
    }
    IEnumerator EdgeJumpIEumerator()
    {
        canEdgeJump = true;
        yield return new WaitForSeconds(.2f);
        canEdgeJump = false;
    }

    // 黏牆狀態
    public bool IsPushWall()
    {
        if (coll.OnWall())
        {
            if (coll.wallSide == 1 && x > 0)
            {
                return true;
            }
            else if (coll.wallSide == -1 && x < 0)
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
        isWallJumpAnim = true;
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

    // 衝刺
    void Dash(float x)
    {
        StopCoroutine("DashWait");
        StopCoroutine("NextDash");
        hasDashed = false;
        isDash = true;
        isAnimDash = true;
        rb.velocity = Vector2.zero;
        Vector3 dir = new Vector2(x, 0);
        rb.velocity += dir.normalized * dashSpeed;
        StartCoroutine("DashWait");
        StartCoroutine("NextDash");
    }
    IEnumerator DashWait()
    {
        DOVirtual.Float(7, 0, 1f, RigidbodyDrag);
        canMove = false;
        GetComponent<BetterJumping>().enabled = false;
        rb.useGravity = false;
        yield return new WaitForSeconds(.15f);
        canMove = true;
        isAnimDash = false;
        GetComponent<BetterJumping>().enabled = true;
        rb.useGravity = true;
    }
    IEnumerator NextDash()
    {
        yield return new WaitForSeconds(.6f);
        isDash = false;
        if (coll.OnGroundDash() && !IsPushWall()) hasDashed = true;
    }

    //衝刺時的阻力
    void RigidbodyDrag(float x)
    {
        rb.drag = x;
    }

    //返回至原速度
    void speedBackOrigin(float x)
    {
        if (coll.wallSide == 1 && Input.GetAxisRaw("Horizontal") == -1 && !coll.OnGround()) { speed = 8; return; }
        if (coll.wallSide == -1 && Input.GetAxisRaw("Horizontal") == 1 && !coll.OnGround()) { speed = 8; return; }
        speed = x;
    }

    IEnumerator DisableMovement()
    {
        canMove = false;
        speed = 0;
        yield return new WaitForSeconds(.18f);
        canMove = true;
        isWallJump = false;
        if (!isStickWall) DOVirtual.Float(0, 8, .5f, speedBackOrigin);
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
        yield return new WaitForSeconds(.35f);
        DOVirtual.Float(0, 8, .5f, speedBackOrigin);
        isStickWall = false;
        GetComponent<BetterJumping>().fallMultiplier = 2.5f;
    }
}
