using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Cinemachine;

public class PlayerMovement : MonoBehaviour
{
    private Collision coll;
    private Animator anim;
    [HideInInspector]
    public Rigidbody rb;

    [Header("Float")]
    public float speed = 7f;
    public float speedOrigin = 7f;
    public float jumpForce = 500f;
    public float fallSpeedMax = -15f;
    public float UpSpeedMimi = 3.5f;
    public float dashSpeed = 20;
    public float fallLandSpeed;
    public float fallMultiplier = 2.35f;
    public float xRaw;
    float x;
    float faceAngle;


    [Space]

    [Header("Int")]
    public int bodyDownCount = 0;
    int wallJumpButtonCount = 0;

    [Space]

    [Header("Booleans")]
    public bool isJumpUp = false; // Y軸速度>0
    public bool startJump = false;
    public bool isWallJump = false; // 開始跳躍並持續一段時間
    public bool isWallJumpAnim = false;
    public bool isJump = false;
    bool callWallJump = false; // 開始跳躍後即關閉
    public bool fall = false; //判斷是否放開跳躍鍵

    public bool canMove = true;

    public bool isStickWall = false; //跳躍到另一個牆上時
    public bool isPushWallAnim = false;
    bool canStickWall = false; //從牆壁跳躍後的一個短瞬間內為true

    public bool canEdgeJump = false; //是否可以GhostJump
    bool EdgeJumpFlag = false; //GhostJump的Flag

    public bool hasDashed = false; //擁有衝刺能量
    public bool isDash = false; //衝刺過程
    public bool isAnimDash = false; //衝刺動畫

    public bool isDeath = false;
    public bool bodyDown = false;
    public bool isDownJump = false;

    public bool isWin = false;
    bool isDeathNotBack = false; //死亡後還沒回歸原位之前
    bool m_isAxisInUse = false; //搖桿Trigger按下

    public bool isTutorial = false;

    [Space]
    [Header("Object")]
    public GameObject cube;
    public GameObject cubeMesh;
    public GameObject aimCircle;
    public GameObject aimTriangle;
    Vector3 DeathV3;

    [Space]
    [Header("Polish")]
    public ParticleSystem deathParticle;
    public ParticleSystem rebirthParticle;
    public ParticleSystem DashToWallParticle;

    public TrailRenderer trail_1, trail_2, trail_3, trail_4, trail_5;
    public Vector3 EntryPoint, checkPoint;
    Tween rbTween, faceRotateTween, meshRotateTween;

    void Start()
    {
        // Time.timeScale = 0.15f;
        rb = GetComponent<Rigidbody>();
        coll = GetComponent<Collision>();
        anim = GetComponent<Animator>();
        EntryPoint = this.transform.position;
        checkPoint = EntryPoint;
    }

    void Update()
    {
        xRaw = Input.GetAxisRaw("Horizontal");
        if (xRaw != 0) xRaw = xRaw > 0 ? 1 : -1;
        if (!isWin)
        {
            //輸入
            if (xRaw != 0 && Mathf.Abs(x) <= 1 && !bodyDown)
            {
                if (coll.OnGround()) x += xRaw * 2.95f * Time.deltaTime;
                else if (!coll.OnGround()) x += xRaw * 6.4f * Time.deltaTime;
                if (xRaw > 0 && x > 0.9f) x = 1;
                if (xRaw < 0 && x < -0.9f) x = -1;
                if (!isStickWall && !isAnimDash && !IsPushWall())
                {
                    faceRotateTween.Kill();
                    if (isDownJump && !IsPushWall()) faceRotateTween = cube.transform.DORotate(new Vector3(0, 0, 0), 0.1f);
                    if (!isDownJump || (isDownJump && faceAngle == 0)) faceRotateTween = cube.transform.DORotate(new Vector3(0, x * -60, 0), 0.52f);
                }
            }
            if (xRaw == 0 && coll.OnGround() && !bodyDown)
            {
                x += x > 0 ? -2.15f * Time.deltaTime : 2.15f * Time.deltaTime;
                if (x > -0.1f && x < 0.1f) x = 0;
                if (!isStickWall && !isAnimDash)
                {
                    faceRotateTween.Kill();
                    faceRotateTween = cube.transform.DORotate(new Vector3(0, 0, 0), 0.22f);
                }
            }
            if (xRaw == 0 && !coll.OnGround() && !isWallJumpAnim)
            {
                if (x > -0.12f && x < 0.12f) x = 0;
                if (x > 0) x -= 0.91f * Time.deltaTime;
                if (x < 0) x += 0.91f * Time.deltaTime;
                faceRotateTween.Kill();
                if (coll.OnWall()) faceRotateTween = cube.transform.DORotate(new Vector3(0, x * -90, 0), 0.1f);
                else if (!coll.OnWall() && !isDownJump) faceRotateTween = cube.transform.DORotate(new Vector3(0, x * -60, 0), 0.1f);
            }
            if (isWallJumpAnim) x = 0;
            if (((xRaw == -1 && x > 0) || (xRaw == 1 && x < 0)) && coll.OnGround() && !bodyDown)
            {
                x = 0;
            }
            if (isAnimDash && rb.velocity.x > 0) x = 1;
            else if (isAnimDash && rb.velocity.x < 0) x = -1;
            if (bodyDown)
            {
                if (bodyDownCount <= 48)
                {
                    if (xRaw == 0) x = 0;
                    else x = xRaw > 0 ? 1 : -1;
                }
                if (x >= -1 && x <= 1 && xRaw != 0)
                {
                    x += xRaw > 0 ? 0.02f : -0.02f;
                    if (x >= 1 && xRaw == 1) x = 1;
                    else if (x <= -1 && xRaw == -1) x = -1;
                }
            }

        }
        else if (isWin)
        {
            if (x < 0) x += 0.02f;
            else if (x > 0) x -= 0.02f;
            faceRotateTween.Kill();
            faceRotateTween = cube.transform.DORotate(new Vector3(0, 0, 0), 0.3f);
        }
        //瞄準方向
        aimCircle.transform.DORotate(new Vector3(0, 0, x * -18.5f), 0.1f);

        //方塊轉向
        if (isStickWall || IsPushWall())
        {
            faceRotateTween.Kill();
            meshRotateTween.Kill();
            cubeMesh.transform.localRotation = Quaternion.Euler(0, 0, 0);
            faceRotateTween = cube.transform.DORotate(new Vector3(0, coll.wallSide * -90, 0), 0.07f);
        }
        if (isAnimDash)
        {
            faceRotateTween.Kill();
            faceRotateTween = cube.transform.DORotate(new Vector3(0, x * -60, 0), 0.07f);
        }
        if (isWallJumpAnim)
        {
            faceRotateTween.Kill();
            faceRotateTween = cube.transform.DORotate(new Vector3(0, coll.wallSide * -60, 0), 0.1f);
        }
        if (!isDownJump)
        {
            meshRotateTween.Kill();
            if (!isDash)
                cubeMesh.transform.localRotation = Quaternion.Euler(0, 0, 0);
        }
        if (coll.OnWall())
        {
            meshRotateTween.Kill();
            cubeMesh.transform.localRotation = Quaternion.Euler(0, 0, 0);
        }


        if (isJumpUp)
        {
            if ((Input.GetButtonUp("Jump") || !Input.GetButton("Jump")) && !isDownJump && rb.velocity.y > 0)
            {
                fall = true;
            }
        }

        //下壓身體
        if ((Input.GetButton("Accumulate") || Input.GetAxis("AccumulateTrigger") == 1) && !isJump && !isAnimDash && !isDeath && !isStickWall && coll.OnGround() && !isWin)
        {
            bodyDown = true;
            faceRotateTween.Kill();
            faceRotateTween = cube.transform.DORotate(new Vector3(0, 0, 0), 0.3f);
        }
        if ((!Input.GetButton("Accumulate") && Input.GetAxis("AccumulateTrigger") == 0) || isJump || isAnimDash || isDeath || isStickWall || !coll.OnGround())
        {
            bodyDown = false;
        }

        if (wallJumpButtonCount == 11)
        {
            isWallJumpAnim = false;
            wallJumpButtonCount = 0;
        }

        // 跳躍狀態判斷
        if (Input.GetButtonDown("Jump"))
        {
            if (((coll.OnGroundJump() && !IsPushWall()) || (coll.OnGround() && IsPushWall()) || (coll.OnEdge() && !coll.OnWall() && canEdgeJump)) && rb.velocity.y < 1.5f) StartCoroutine("JumpSetTrue");
            else if ((IsPushWall() && !coll.OnGround()) || isStickWall && !coll.OnGround()) callWallJump = true;
        }

        //衝刺
        if ((Input.GetButtonDown("Dash") || Input.GetAxis("DashTrigger") == 1) && hasDashed && !IsPushWall() && !isDeath && !isWin && !m_isAxisInUse)
        {
            if (xRaw == 0 && x != 0)
            {
                if (x > 0) Dash(1);
                if (x < 0) Dash(-1);
                GameObject.FindWithTag("GM").SendMessage("ScreenShake_Dash");
            }
            else if (xRaw != 0)
            {
                Dash(xRaw);
                GameObject.FindWithTag("GM").SendMessage("ScreenShake_Dash");
            }
            StartCoroutine("DashTriggerSetTrue");
        }

        if ((!Input.GetButton("Dash") && Input.GetAxis("DashTrigger") == 0)) m_isAxisInUse = false;

        //碰地可再次衝刺
        if (coll.OnGroundDash() && !isDash) hasDashed = true;

        //撞牆噴粒子
        if (isAnimDash && IsPushWall())
        {
            DashToWallParticle.Play();
        }

        if (coll.OnGround() || isDash || isDeath || rb.velocity.y < -14) isDownJump = false;
        if (IsPushWall() && rb.velocity.y < 0) isDownJump = false;

        if (IsPushWall() || coll.OnGround()) isJump = false;

        //判斷落下
        if (rb.velocity.y < 0 || coll.OnGround())
        {
            isJumpUp = false;
        }

        // 重置
        if (Input.GetKeyDown(KeyCode.T))
        {
            transform.position = EntryPoint;
        }
        else if (Input.GetButtonDown("Rebirth") && !isDeath)
        {
            Death(false);
        }

        // 蹬牆轉向時速度增加
        if (isWallJump)
        {
            if (coll.wallSide == 1 && xRaw == -1) speed = speedOrigin;
            else if (coll.wallSide == -1 && xRaw == 1) speed = speedOrigin;
        }

        //牆跳動畫
        if (isWallJumpAnim)
        {
            if ((IsPushWall() || coll.OnGround() || isDash || isStickWall || coll.OnWall()) && !isWallJump)
            {
                isWallJumpAnim = false;
                DOTween.Kill("speedBackTween", false);
                speed = speedOrigin;
                wallJumpButtonCount = 0;
            }
        }

        //黏牆動畫
        if ((IsPushWall() || isStickWall) && !isDeath)
        {
            isPushWallAnim = true;
            cubeMesh.transform.DOKill();
            cubeMesh.transform.localRotation = Quaternion.Euler(0.0f, 0.0f, 0.0f); ;
        }
        if (!isStickWall || isWallJump) isPushWallAnim = false;
    }

    private void FixedUpdate()
    {
        // if (coll.OnGroundEdge() && xRaw == coll.wallSide)
        // {
        //     rb.AddForce(Vector3.up * 38f);
        //     if (xRaw == 1) rb.AddForce(Vector3.left * 18);
        //     if (xRaw == -1) rb.AddForce(Vector3.right * 18);
        // }
        // if (coll.OnGroundEdge() && xRaw == 0)
        // {
        //     if (coll.wallSide == 1) rb.AddForce(Vector3.left * 6);
        //     if (coll.wallSide == -1) rb.AddForce(Vector3.right * 6);
        // }

        BodyDown();
        if (!isDeath && !isWin)
        {
            Move();
            Jump();
            EdgeJump();
        }
        else if (isDeath && isDeathNotBack)
        {
            this.transform.position = DeathV3;
        }

        if ((IsPushWall() && !coll.OnGround() && !isWallJump && callWallJump) || (isStickWall && !coll.OnGround() && !isWallJump && callWallJump)) WallJump();

        // 短按跳躍落下
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
        if (IsPushWall() && !isWallJump && !coll.OnGroundEdge())
        {
            if (!coll.OnGround())
            {
                DOTween.Kill("speedBackTween", false);
                speed = 0;
                rb.velocity = new Vector2(0, rb.velocity.y);
                GetComponent<BetterJumping>().fallMultiplier = 0.15f;
            }
            rb.velocity = new Vector2(0, rb.velocity.y);

            if (rb.velocity.y <= fallSpeedMax * 0.25f)
            {
                rb.velocity = new Vector3(rb.velocity.x, fallSpeedMax * 0.25f);
            }
        }
        if (!IsPushWall() && !isWallJump && !isStickWall && !isDash)
        {
            GetComponent<BetterJumping>().fallMultiplier = fallMultiplier;
        }

        // 黏牆相關
        if (isWallJump)
        {
            StopCoroutine("StickWall");
        }
        if (IsPushWall() && !isStickWall && !coll.OnGround() && !isWallJump && !coll.OnGroundEdge()) //持續黏牆
        {
            StopCoroutine("StickWall");
            isStickWall = true;
            speed = 0;
            rb.velocity = new Vector2(0, rb.velocity.y);
            canStickWall = false;
        }
        if (!IsPushWall() && isStickWall && coll.OnWall() && !canStickWall) //黏牆時放開方向鍵
        {
            StartCoroutine("StickWall");
        }
        if (coll.OnGround())
        {
            StopCoroutine("StickWall");
            if (isStickWall) speed = speedOrigin;
            isStickWall = false;

        }
        if (coll.OnGround() && !isAnimDash && isDash)
        {
            rbTween.Kill();
            rb.drag = 0;
        }
        if (isWallJump && coll.OnWall() && canStickWall) //牆跳到另一牆壁時觸發黏牆
        {
            StopCoroutine("StickWall");
            StartCoroutine("StickWall");
            canStickWall = false;
        }
        if (isStickWall && !coll.OnWall()) //黏牆滑落到離開牆面 or 
        {
            StopCoroutine("StickWall");
            isStickWall = false;
            if (!isWallJump)
            {
                StopCoroutine("DisableMovement");
                if (!isDeath)
                {
                    canMove = true;
                }
                DOVirtual.Float(0, speedOrigin, .45f, speedBackOrigin);
            }
        }

        //判斷墜地速度
        if (!coll.OnGround())
        {
            fallLandSpeed = rb.velocity.y;
        }

        //衝刺Trail
        if (!isAnimDash && trail_5.time > 0f)
        {
            trail_1.time -= 0.01f;
            trail_2.time -= 0.01f;
            trail_3.time -= 0.01f;
            trail_4.time -= 0.01f;
            trail_5.time -= 0.01f;
        }
        else if (trail_5.time < 0f)
        {
            trail_1.enabled = false;
            trail_2.enabled = false;
            trail_3.enabled = false;
            trail_4.enabled = false;
            trail_5.enabled = false;
        }

        if (coll.OnGroundEdge() && xRaw == coll.wallSide)
        {
            rb.AddForce(Vector3.up * 38f);
            if (xRaw == 1) rb.AddForce(Vector3.left * 18);
            if (xRaw == -1) rb.AddForce(Vector3.right * 18);
        }
        if (coll.OnGroundEdge() && xRaw == 0)
        {
            if (coll.wallSide == 1) rb.AddForce(Vector3.left * 5);
            if (coll.wallSide == -1) rb.AddForce(Vector3.right * 5);
        }

    }

    void BodyDown()
    {
        if (bodyDown)
        {
            canMove = false;
            cubeMesh.transform.DOScale(new Vector3(0.7f, 0.22f, 0.7f), 0.3f);
            cubeMesh.transform.DOLocalMoveY(-0.08f, 0.3f);
            if (rb.velocity.x != 0 && !isDash && bodyDownCount <= 25)
            {
                rb.velocity = new Vector2(x * (5 - (bodyDownCount * 0.2f)), rb.velocity.y);
            }
            if (bodyDownCount < 50) bodyDownCount++;
            if (bodyDownCount >= 50)
            {
                aimTriangle.SetActive(true);
            }
        }
        else if (!bodyDown)
        {
            if ((((!Input.GetButton("Accumulate") && Input.GetAxis("AccumulateTrigger") == 0) && coll.OnGround()) || isJump || (!coll.OnGround() && !isWallJumpAnim && !IsPushWall())) && !isAnimDash)
            {
                canMove = true;
            }
            if (!isDeath)
            {
                cubeMesh.transform.DOScale(new Vector3(0.5f, 0.5f, 0.5f), 0.2f);
                cubeMesh.transform.DOLocalMoveY(0.08f, 0.2f);
            }
            if (bodyDownCount > 0) bodyDownCount -= 5;
            else bodyDownCount = 0;
        }
        if (bodyDownCount < 50) aimTriangle.SetActive(false);
    }

    void Move()
    {
        if ((IsPushWall() && !coll.OnGroundEdge()) || !canMove || isStickWall) return;
        if (isWallJumpAnim && xRaw == 0) return;
        if (isWallJumpAnim && canMove && coll.OnWall())
        {
            isWallJumpAnim = false;
        }
        if (isWallJumpAnim && canMove && (coll.wallSide == xRaw))
        {
            DOTween.Kill("speedBackTween", false);
            if (!isStickWall) DOVirtual.Float(0, speedOrigin, 0.4f, speedBackOrigin).SetId("speedBackTween");
            isWallJumpAnim = false;
        }
        if (isWallJumpAnim && canMove && (coll.wallSide == -xRaw) && !coll.OnWall())
        {
            x = xRaw;
            isWallJumpAnim = false;
        }

        float movement = x * speed;


        rb.velocity = new Vector2(movement, rb.velocity.y);
    }

    void Jump()
    {
        if ((rb.velocity.y < 1 && rb.velocity.y > -1) || canEdgeJump)
        {
            if (startJump)
            {
                if (isDash && coll.OnGroundDash())
                {

                    StopCoroutine("DashWait");
                    StopCoroutine("NextDash");
                    StopCoroutine("DashJump");
                    GetComponent<BetterJumping>().enabled = true;
                    rb.useGravity = true;
                    GetComponent<BetterJumping>().fallMultiplier = fallMultiplier;
                    rbTween.Kill();
                    rbTween = DOVirtual.Float(2, 0, 0.6f, RigidbodyDrag);
                    StartCoroutine("DashJump");
                }
                StopCoroutine("JumpSetTrue");
                isJump = true;
                rb.velocity = new Vector3(rb.velocity.x, 0);
                if (bodyDownCount >= 50)
                {
                    isDownJump = true;
                    rb.AddForce(Vector3.up * jumpForce * 2.25f);
                    faceAngle = x >= 0 ? -900 : 900;
                    meshRotateTween.Kill();

                    meshRotateTween = cubeMesh.transform.DOLocalRotate(new Vector3(0, 0, faceAngle), 1f, RotateMode.FastBeyond360);
                    GameObject.FindWithTag("GM").SendMessage("ScreenShake_DownJump");
                }
                else
                {
                    rb.AddForce(Vector3.up * jumpForce);
                }
                isJumpUp = true;
                startJump = false;
                canEdgeJump = false;
                StopCoroutine("EdgeJumpIEumerator");
            }
        }
    }

    //Ghost Jump
    void EdgeJump()
    {
        if (isPushWallAnim)
        {
            canEdgeJump = false;
            return;
        }
        if (coll.OnGround())
        {
            canEdgeJump = false;
            EdgeJumpFlag = true;
            StopCoroutine("EdgeJumpIEumerator");
        }
        if (canEdgeJump) return;
        if (!coll.OnGround() && EdgeJumpFlag && !isJump)
        {
            canEdgeJump = true;
            StartCoroutine("EdgeJumpIEumerator");
            EdgeJumpFlag = false;
        }
    }
    IEnumerator EdgeJumpIEumerator()
    {
        canEdgeJump = true;
        yield return new WaitForSeconds(.3f);
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
        DOTween.Kill("walljumpTween", false);
        rbTween = DOVirtual.Float(9, 0, .26f, RigidbodyDrag);
        rb.drag = 7.8f;
        isWallJump = true;
        isWallJumpAnim = true;
        rb.velocity = new Vector2(0, 0);
        StopCoroutine("canStickWallEnumerator");
        StartCoroutine("canStickWallEnumerator");
        StopCoroutine("DisableMovement");
        StartCoroutine("DisableMovement");
        rb.AddForce(Vector3.up * jumpForce * 2f);
        if (coll.wallSide == 1) rb.AddForce(-Vector3.right * jumpForce * 1.4f);
        if (coll.wallSide == -1) rb.AddForce(Vector3.right * jumpForce * 1.4f);
        callWallJump = false;
        wallJumpButtonCount = 0;
    }

    // 衝刺
    void Dash(float value)
    {
        StopCoroutine("DashWait");
        StopCoroutine("NextDash");
        StopCoroutine("DisableMovement");
        StopCoroutine("StickWall");
        StopCoroutine("DashJump");
        //FindObjectOfType<RippleEffect>().Emit(Camera.main.WorldToViewportPoint(transform.position));
        isStickWall = false;
        isWallJump = false;
        speed = speedOrigin;
        meshRotateTween.Kill();
        cubeMesh.transform.localRotation = Quaternion.Euler(0, 0, 0);
        cubeMesh.transform.DOLocalRotate(new Vector3(-540, 0, 0), 0.4f, RotateMode.FastBeyond360);
        hasDashed = false;
        isDash = true;
        isAnimDash = true;
        StartTrail();
        anim.Play("Dash");
        rb.velocity = Vector2.zero;
        Vector3 dir = new Vector2(value, 0);
        rb.velocity += dir.normalized * dashSpeed;
        GetComponent<BetterJumping>().fallMultiplier = 0.15f;
        StartCoroutine("DashWait");
        StartCoroutine("NextDash");
    }
    void StartTrail()
    {
        trail_1.enabled = true;
        trail_2.enabled = true;
        trail_3.enabled = true;
        trail_4.enabled = true;
        trail_5.enabled = true;
        trail_1.time = 0.38f;
        trail_2.time = 0.27f;
        trail_3.time = 0.21f;
        trail_4.time = 0.35f;
        trail_5.time = 0.48f;
    }

    IEnumerator JumpSetTrue()
    {
        startJump = true;
        yield return new WaitForSeconds(0.5f);
        if (startJump)
        {
            startJump = false;
        }
    }
    IEnumerator DashTriggerSetTrue()
    {
        m_isAxisInUse = false;
        yield return new WaitForSeconds(0.15f);
        if (!m_isAxisInUse)
        {
            m_isAxisInUse = true;
        }
    }
    IEnumerator DashWait()
    {
        rbTween = DOVirtual.Float(7, 0, 1f, RigidbodyDrag);
        canMove = false;
        GetComponent<BetterJumping>().enabled = false;
        rb.useGravity = false;
        yield return new WaitForSeconds(.15f);
        if (!isDeath)
        {
            canMove = true;
        }
        isAnimDash = false;
        GetComponent<BetterJumping>().enabled = true;
        rb.useGravity = true;
        yield return new WaitForSeconds(.07f);
        GetComponent<BetterJumping>().fallMultiplier = fallMultiplier;
    }
    IEnumerator NextDash()
    {
        yield return new WaitForSeconds(.55f);
        isDash = false;
    }
    IEnumerator DashJump()
    {

        yield return new WaitForSeconds(.15f);
        canMove = true;
        isAnimDash = false;
        yield return new WaitForSeconds(.25f);
        isDash = false;
        yield return new WaitForSeconds(.15f);
        if (!isDash) hasDashed = true;
    }

    //衝刺時的阻力
    void RigidbodyDrag(float value)
    {
        rb.drag = value;
    }

    //返回至原速度
    void speedBackOrigin(float value)
    {
        if (coll.wallSide == 1 && Input.GetAxisRaw("Horizontal") == -1 && !coll.OnGround()) { speed = speedOrigin; return; }
        if (coll.wallSide == -1 && Input.GetAxisRaw("Horizontal") == 1 && !coll.OnGround()) { speed = speedOrigin; return; }
        speed = value;
    }

    IEnumerator DisableMovement()
    {
        canMove = false;
        speed = 0;
        yield return new WaitForSeconds(.23f);
        if (!isDeath)
        {
            canMove = true;
        }
        isWallJump = false;
        if (!isStickWall) DOVirtual.Float(0, speedOrigin, .55f, speedBackOrigin).SetId("speedBackTween");
    }

    //跳牆時極短瞬間無法黏牆
    IEnumerator canStickWallEnumerator()
    {
        canStickWall = false;
        yield return new WaitForSeconds(.01f);
        canStickWall = true;
    }

    IEnumerator StickWall()
    {
        isStickWall = true;
        speed = 0;
        GetComponent<BetterJumping>().fallMultiplier = 0.15f;
        rb.velocity = new Vector2(0, rb.velocity.y);
        yield return new WaitForSeconds(.35f);
        DOVirtual.Float(0, speedOrigin, .5f, speedBackOrigin);
        if (coll.wallSide != xRaw)
        {
            isStickWall = false;
            GetComponent<BetterJumping>().fallMultiplier = fallMultiplier;
        }
    }
    private void OnTriggerEnter(Collider coll)
    {
        if (coll.CompareTag("Hazard") && !isDeath) Death(true);
    }
    public void Ending()
    {
        if (!isWin)
        {
            StopAllCoroutines();
            canMove = false;
            speed = 0;
            StartCoroutine("Win");
        }
    }

    public void NextLevel()
    {
        GameObject.FindWithTag("GM").SendMessage("NextLevel");
    }
    IEnumerator Win()
    {
        isWin = true;
        yield return new WaitForSeconds(0.5f);
        GameObject.FindWithTag("GM").SendMessage("Win");
        yield return new WaitForSeconds(0.5f);
        anim.SetBool("isWin", isWin);
    }

    public void Death(bool active)
    {
        if (active) deathParticle.Play();
        GameObject.FindWithTag("GM").SendMessage("ScreenShake_Death");
        isDeath = true;
        isDeathNotBack = true;
        StartCoroutine("Rebirth");
        DeathV3 = this.transform.position;
        cubeMesh.transform.DOScale(0, 0.2f);
    }
    IEnumerator Rebirth()
    {
        //FindObjectOfType<RippleEffect>().Emit(Camera.main.WorldToViewportPoint(transform.position));

        yield return new WaitForSeconds(.2f);
        GameObject.FindWithTag("GM").SendMessage("Death");
        cubeMesh.SetActive(false);
        canMove = false;
        yield return new WaitForSeconds(.45f);
        transform.position = checkPoint;
        isDeathNotBack = false;
        if (!isTutorial)
        {
            GameObject.FindWithTag("Camera").GetComponent<CinemachineVirtualCamera>().GetCinemachineComponent<CinemachineFramingTransposer>().m_XDamping = 0;
            GameObject.FindWithTag("Camera").GetComponent<CinemachineVirtualCamera>().GetCinemachineComponent<CinemachineFramingTransposer>().m_YDamping = 0;
        }
        yield return new WaitForSeconds(.4f);
        GameObject.FindWithTag("GM").SendMessage("ResetLevel");
        transform.position = checkPoint;
        if (!isTutorial)
        {
            GameObject.FindWithTag("Camera").GetComponent<CinemachineVirtualCamera>().GetCinemachineComponent<CinemachineFramingTransposer>().m_XDamping = 0.65f;
            GameObject.FindWithTag("Camera").GetComponent<CinemachineVirtualCamera>().GetCinemachineComponent<CinemachineFramingTransposer>().m_YDamping = 1.5f;
        }
        yield return new WaitForSeconds(.2f);
        hasDashed = false;
        cubeMesh.SetActive(true);
        rb.velocity = Vector2.zero;
        rebirthParticle.Play();
        transform.position = checkPoint;
        cubeMesh.transform.DOScale(0.5f, 1.3f).SetEase(Ease.OutElastic);
        isDeath = false;

        yield return new WaitForSeconds(.4f);
        canMove = true;
        speed = speedOrigin;
    }
}
