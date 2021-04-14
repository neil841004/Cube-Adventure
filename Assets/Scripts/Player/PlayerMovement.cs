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
    int onWallNotInputCount = 0;

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
    bool canSend = false; //按下傳送後有0.1秒可以傳送

    [Space]
    [Header("Object")]
    public GameObject cube;
    public GameObject cubeMesh;
    public GameObject aimCircle;
    public GameObject aimTriangle;
    public GameObject checkPoint;

    [Space]
    [Header("Polish")]
    public ParticleSystem deathParticle;
    public ParticleSystem rebirthParticle;
    public ParticleSystem DashToWallParticle;
    public ParticleSystem AccumulateParticle;
    public ParticleSystem DashParticle;
    public ParticleSystem DownJumpParticle;


    [Space]
    public TrailRenderer trail_1, trail_2, trail_3, trail_4, trail_5;
    public Vector3 EntryPoint, checkPointV3;
    PlayerSound _sound;
    Vector3 DeathV3;
    Tween rbTween, faceRotateTween, meshRotateTween;
    GameObject gm;
    BetterJumping _betterJumping;

    void Start()
    {
        // Time.timeScale = 0.15f;
        rb = GetComponent<Rigidbody>();
        coll = GetComponent<Collision>();
        anim = GetComponent<Animator>();
        EntryPoint = this.transform.position;
        checkPointV3 = EntryPoint;
        gm = GameObject.FindWithTag("GM");
        _betterJumping = GetComponent<BetterJumping>();
        _sound = GetComponent<PlayerSound>();
    }

    void Update()
    {
        xRaw = Input.GetAxisRaw("Horizontal");
        if (xRaw != 0) xRaw = xRaw > 0 ? 1 : -1;
        if (isDeath) { xRaw = 0; x = 0; }
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
                const float kDeceleration = 3.45f;
                x = Mathf.MoveTowards(x, 0f, kDeceleration * Time.deltaTime);
                if (!isStickWall && !isAnimDash)
                {
                    faceRotateTween.Kill();
                    faceRotateTween = cube.transform.DORotate(new Vector3(0, 0, 0), 0.42f);
                }
            }
            if (xRaw == 0 && !coll.OnGround() && !isWallJumpAnim && !isDownJump)
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
            if (xRaw == 0 && coll.OnWall()) //黏牆沒有輸入左右，一段時間後速度歸零
            {
                if (onWallNotInputCount <= 20)
                    onWallNotInputCount++;
                if (onWallNotInputCount > 20) { x = 0; }
            }
            else if (xRaw != 0 || !coll.OnWall())
            {
                onWallNotInputCount = 0;
            }
            if (isAnimDash && rb.velocity.x > 0) x = 1;
            else if (isAnimDash && rb.velocity.x < 0) x = -1;
            if (bodyDown)
            {
                if (bodyDownCount == 1)
                {
                    _sound.PlaySound(4, 0.46f);
                }
                if (bodyDownCount <= 40)
                {
                    if (xRaw == 0) x = 0;
                    else x = xRaw > 0 ? 1 : -1;
                }
                if (x >= -1 && x <= 1 && xRaw != 0)
                {
                    x += xRaw > 0 ? 3.8f * Time.deltaTime : -3.8f * Time.deltaTime;
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

        if (rb.velocity.y < -14) _sound.strongLanding = true;
        else if (rb.velocity.y > -5 && _sound.strongLanding == true) { StartCoroutine("CancelStrongLanding"); }



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
                gm.SendMessage("ScreenShake_Dash");
            }
            else if (xRaw != 0)
            {
                Dash(xRaw);
                gm.SendMessage("ScreenShake_Dash");
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
        // if (Input.GetKeyDown(KeyCode.T))
        // {
        //     transform.position = EntryPoint;
        // }
        // else if (Input.GetButtonDown("Rebirth") && !isDeath)
        // {
        //     Death(true);
        // }

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

        //傳送
        if (Input.GetButtonDown("Send"))
        {
            canSend = true;
            StopCoroutine("sendIenumerator");
            StartCoroutine("sendIenumerator");
        }
    }


    private void FixedUpdate()
    {

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
                _betterJumping.fallMultiplier = 0.15f;
            }
            rb.velocity = new Vector2(0, rb.velocity.y);

            if (rb.velocity.y <= fallSpeedMax * 0.25f)
            {
                rb.velocity = new Vector3(rb.velocity.x, fallSpeedMax * 0.25f);
            }
        }
        if (!IsPushWall() && !isWallJump && !isStickWall && !isDash)
        {
            _betterJumping.fallMultiplier = fallMultiplier;
        }

        // 黏牆相關
        if (isWallJump)
        {
            StopCoroutine("StickWall");
        }
        if (IsPushWall() && !isStickWall && !coll.OnGround() && !isWallJump && !coll.OnGroundEdge()) //持續黏牆
        {
            _sound.PlayOneSound(12, 0.34f, 0.08f);
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
                    _betterJumping.fallMultiplier = fallMultiplier;
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
            trail_1.time -= 0.011f;
            trail_2.time -= 0.011f;
            trail_3.time -= 0.011f;
            trail_4.time -= 0.011f;
            trail_5.time -= 0.011f;
        }
        else if (trail_5.time <= 0f)
        {
            trail_1.enabled = false;
            trail_2.enabled = false;
            trail_3.enabled = false;
            trail_4.enabled = false;
            trail_5.enabled = false;
        }

        if (coll.OnGroundEdge() && xRaw == coll.wallSide && (!Input.GetButton("Accumulate") && Input.GetAxis("AccumulateTrigger") != 1))
        {
            rb.AddForce(Vector3.up * 38f);
            if (xRaw == 1) rb.AddForce(Vector3.left * 18);
            if (xRaw == -1) rb.AddForce(Vector3.right * 18);
        }
        if (coll.OnGroundEdge() && xRaw == 0 && (!Input.GetButton("Accumulate") && Input.GetAxis("AccumulateTrigger") != 1))
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
            cubeMesh.transform.DOLocalMoveY(-0.16f, 0.3f);
            if (rb.velocity.x != 0 && !isDash && bodyDownCount <= 15)
            {
                rb.velocity = new Vector2(x * (4.5f - (bodyDownCount * 0.3f)), rb.velocity.y);
            }
            if (bodyDownCount > 5) AccumulateParticle.Play();
            if (bodyDownCount < 42) bodyDownCount++;
            if (bodyDownCount == 41)
            {
                _sound.PlayOneSound(5, 0.54f);
                _sound.PlayLoopSound(6, 0.48f);
            }
            if (bodyDownCount >= 42)
            {
                aimTriangle.SetActive(true);
            }
        }
        else if (!bodyDown)
        {
            AccumulateParticle.Stop();
            _sound.StopSound(4);
            if ((((!Input.GetButton("Accumulate") && Input.GetAxis("AccumulateTrigger") == 0) && coll.OnGround()) || isJump || (!coll.OnGround() && !isWallJumpAnim && !IsPushWall())) && !isAnimDash && !isDeath)
            {
                canMove = true;
                if (!isDownJump && coll.OnGround() && bodyDownCount != 0)
                {
                    DOVirtual.Float(0, speedOrigin, 0.4f, speedBackOrigin).SetId("speedBackTween");
                }
            }
            if (!isDeath)
            {
                cubeMesh.transform.DOScale(new Vector3(0.5f, 0.5f, 0.5f), 0.2f);
                cubeMesh.transform.DOLocalMoveY(0.04f, 0.2f);
            }
            if (bodyDownCount > 0) bodyDownCount -= 5;
            else bodyDownCount = 0;
            if (bodyDownCount > 1 && bodyDownCount < 10) _sound.StopLoopSound(6);
        }
        if (bodyDownCount < 42) aimTriangle.SetActive(false);
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
                    _betterJumping.enabled = true;
                    rb.useGravity = true;
                    _betterJumping.fallMultiplier = fallMultiplier;
                    rbTween.Kill();
                    rbTween = DOVirtual.Float(3, 0, 0.6f, RigidbodyDrag);
                    StartCoroutine("DashJump");
                }
                StopCoroutine("JumpSetTrue");
                isJump = true;
                rb.velocity = new Vector3(rb.velocity.x, 0);
                if (bodyDownCount >= 42)
                {
                    StartTrail();
                    DownJumpParticle.Play();
                    AccumulateParticle.Stop();
                    isDownJump = true;
                    rb.AddForce(Vector3.up * jumpForce * 2.25f);
                    faceAngle = x >= 0 ? -900 : 900;
                    meshRotateTween.Kill();

                    meshRotateTween = cubeMesh.transform.DOLocalRotate(new Vector3(0, 0, faceAngle), 1f, RotateMode.FastBeyond360);
                    gm.SendMessage("ScreenShake_DownJump");
                    _sound.PlayOneSound(7, 0.82f);
                }
                else
                {
                    rb.AddForce(Vector3.up * jumpForce);
                    _sound.PlayOneSound(0, 0.37f, 0.03f);
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

    //關閉強力落地
    IEnumerator CancelStrongLanding()
    {
        yield return new WaitForSeconds(0.15f);
        _sound.strongLanding = false;
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
        rbTween = DOVirtual.Float(9.5f, 0, .26f, RigidbodyDrag);
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
        cubeMesh.transform.DOLocalRotate(new Vector3(-540, 0, 0), 0.47f, RotateMode.FastBeyond360);
        // DashParticle.Play();
        // if (xRaw > 0) DashParticleR.Play();
        // else if (xRaw < 0) DashParticleL.Play();
        // if (xRaw == 0)
        // {
        //     if (x > 0) DashParticleR.Play();
        //     else if (x < 0) DashParticleL.Play();
        // }
        hasDashed = false;
        isDash = true;
        isAnimDash = true;
        StartTrail();
        anim.Play("Dash");
        _sound.PlayOneSound(1, 0.42f, 0.08f);
        rb.velocity = Vector2.zero;
        Vector3 dir = new Vector2(value, 0);
        rb.velocity += dir.normalized * dashSpeed;
        _betterJumping.fallMultiplier = 0.15f;
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
        trail_1.time = 0.33f;
        trail_2.time = 0.22f;
        trail_3.time = 0.17f;
        trail_4.time = 0.30f;
        trail_5.time = 0.41f;
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
        rbTween = DOVirtual.Float(10, 0, 1f, RigidbodyDrag);
        canMove = false;
        _betterJumping.enabled = false;
        rb.useGravity = false;
        yield return new WaitForSeconds(.15f);
        if (!isDeath)
        {
            canMove = true;
        }
        isAnimDash = false;
        _betterJumping.enabled = true;
        rb.useGravity = true;
        yield return new WaitForSeconds(.07f);
        _betterJumping.fallMultiplier = fallMultiplier;
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
        _betterJumping.fallMultiplier = 0.15f;
        rb.velocity = new Vector2(0, rb.velocity.y);
        yield return new WaitForSeconds(.35f);
        DOVirtual.Float(0, speedOrigin, .5f, speedBackOrigin);
        if (coll.wallSide != xRaw)
        {
            isStickWall = false;
            _betterJumping.fallMultiplier = fallMultiplier;
        }
    }
    private void OnTriggerEnter(Collider co)
    {
        if (co.CompareTag("Hazard") && !isDeath) Death(true);
        if (co.CompareTag("DeathZone") && !isDeath)
        {
            GameObject.FindWithTag("Camera").SendMessage("CameraStop");
            canMove = false;
            StartCoroutine("DeathZoneIenumerator");
        }
    }

    IEnumerator DeathZoneIenumerator()
    {
        yield return new WaitForSeconds(0.35f);
        Death(false);
        yield return new WaitForSeconds(0.8f);
        GameObject.FindWithTag("Camera").SendMessage("CameraStart");
    }

    private void OnTriggerStay(Collider co)
    {
        if (co.CompareTag("End") && coll.OnGround())
        {
            Ending();
            foreach (Transform child in co.transform) child.gameObject.SetActive(true);
        }
        if (co.CompareTag("Portal") && canSend)
        {
            // gm.SendMessage("PortalEffect");
            this.transform.position = co.GetComponent<Portal>().destination.transform.position;
            StartCoroutine("DisableMovement");
            co.GetComponent<Portal>().SendMessage("PortalStart");
            GetComponentInChildren<PlayerMesh>().SendMessage("SendAnim");
            StopCoroutine("sendIenumerator");
            canSend = false;
        }
    }

    IEnumerator sendIenumerator()
    {

        yield return new WaitForSeconds(0.1f);
        canSend = false;
    }

    public void Ending()
    {
        if (!isWin)
        {
            StopAllCoroutines();
            canMove = false;
            speed = 0;
            StartCoroutine("Win");
            isWin = true;
            GameObject.FindWithTag("UI_cpTip").GetComponent<ProgressTip>().SendMessage("PassEnd");
        }
    }

    IEnumerator Win()
    {
        gm.SendMessage("Win");
        yield return new WaitForSeconds(0.5f);
        // gm.SendMessage("Win");
        // yield return new WaitForSeconds(0.5f);
        anim.SetBool("isWin", isWin);
    }

    public void Death(bool active)
    {
        if (active) deathParticle.Play();
        _sound.PlayOneSound(8, 0.88f);
        gm.SendMessage("ScreenShake_Death");
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
        gm.SendMessage("Death");
        cubeMesh.SetActive(false);
        canMove = false;
        yield return new WaitForSeconds(.6f);
        transform.position = checkPointV3;
        isDeathNotBack = false;
        if (!isTutorial)
        {
            GameObject.FindWithTag("Camera").GetComponent<CinemachineVirtualCamera>().GetCinemachineComponent<CinemachineFramingTransposer>().m_XDamping = 0;
            GameObject.FindWithTag("Camera").GetComponent<CinemachineVirtualCamera>().GetCinemachineComponent<CinemachineFramingTransposer>().m_YDamping = 0;
        }
        yield return new WaitForSeconds(.25f);
        gm.SendMessage("ResetLevel");
        transform.position = checkPointV3;
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
        transform.position = checkPointV3;
        if (checkPoint)
        {
            checkPoint.GetComponent<Animator>().Play("CheckPoint_Revival", 0, 0);
            _sound.PlayOneSound(11, 0.7f);
        }
        cubeMesh.transform.DOScale(0.5f, 1f).SetEase(Ease.OutElastic);
        _sound.PlayOneSound(9, 0.78f);

        yield return new WaitForSeconds(0.25f);
        isDeath = false;
        canMove = true;
        speed = speedOrigin;
    }
}
