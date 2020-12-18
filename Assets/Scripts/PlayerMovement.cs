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

    float x;
    public float xRaw, yRaw;


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

    public bool isTutorial = false;

    [Space]
    [Header("Object")]
    public GameObject cube;
    public GameObject cubeMesh;
    Vector3 DeathV3;

    [Space]
    [Header("Polish")]
    public ParticleSystem deathParticle;
    public ParticleSystem rebirthParticle;
    public ParticleSystem DashToWallParticle;

    public TrailRenderer trail_1, trail_2, trail_3, trail_4, trail_5;
    public Vector3 EntryPoint, checkPoint;
    Tween rbTween;

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
        if (!isWin) x = Input.GetAxis("Horizontal");
        else if (isWin && x < 0) x += 0.02f;
        else if (isWin && x > 0) x -= 0.02f;
        xRaw = Input.GetAxisRaw("Horizontal");
        yRaw = Input.GetAxisRaw("Vertical");
        //下壓身體
        if (yRaw == -1 && !isJump && !isAnimDash && !isDeath && !isStickWall && coll.OnGround() && !isWin)
        {
            bodyDown = true;
        }
        if (yRaw != -1 || isJump || isAnimDash || isDeath || isStickWall || !coll.OnGround())
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
        if (Input.GetButtonDown("Dash") && hasDashed && !IsPushWall() && !isDeath && !isWin)
        {
            if (xRaw != 0)
            {
                Dash(xRaw);
                GameObject.FindWithTag("GM").SendMessage("ScreenShake_Dash");
            }
        }
        //衝刺Trail

        if (!isDash && !IsPushWall())
        {
            if (coll.OnGroundDash()) hasDashed = true;
        }

        //撞牆噴粒子
        if (isAnimDash && IsPushWall())
        {
            DashToWallParticle.Play();
        }

        if (!isJump && !IsPushWall() || rb.velocity.y < 0) isDownJump = false;

        if (IsPushWall() || coll.OnGround()) isJump = false;

        //判斷落下
        if (rb.velocity.y < 0 || coll.OnGround())
        {
            isJumpUp = false;
        }

        // 重置
        if (Input.GetKeyDown(KeyCode.R))
        {
            transform.position = EntryPoint;
        }
        else if (Input.GetKeyDown(KeyCode.T) && !isDeath)
        {
            Death();
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
            if ((IsPushWall() || coll.OnGround() || isDash || isStickWall) && !isWallJump)
            {
                isWallJumpAnim = false;
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

        //方塊轉向
        if (isStickWall || IsPushWall()) cube.transform.DORotate(new Vector3(0, coll.wallSide * -90, 0), 0.07f);
        else if (!isStickWall && !isAnimDash) cube.transform.DORotate(new Vector3(0, x * -60, 0), .18f);


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
        if (isJumpUp)
        {
            if (!Input.GetButton("Jump"))
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
        if (IsPushWall() && !isStickWall && !coll.OnGround() && !isWallJump) //持續黏牆
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
            trail_1.time -= 0.011f;
            trail_2.time -= 0.011f;
            trail_3.time -= 0.011f;
            trail_4.time -= 0.011f;
            trail_5.time -= 0.011f;
        }
        else if (trail_5.time < 0f)
        {
            trail_1.enabled = false;
            trail_2.enabled = false;
            trail_3.enabled = false;
            trail_4.enabled = false;
            trail_5.enabled = false;
        }

    }

    void BodyDown() {
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
        }
        else if (!bodyDown)
        {
            if (((yRaw != -1 && coll.OnGround()) || isJump || (!coll.OnGround() && !isWallJumpAnim && !IsPushWall())) && !isAnimDash)
            {
                canMove = true;
            }
            if (!isDeath)
            {
                cubeMesh.transform.DOScale(new Vector3(0.5f, 0.5f, 0.5f), 0.3f);
                cubeMesh.transform.DOLocalMoveY(0.08f, 0.3f);
            }
            if (bodyDownCount > 0) bodyDownCount -= 5;
            else bodyDownCount = 0;
        }
    }

    void Move()
    {
        float movement = x * speed;

        //WallJump位移修正

        if (IsPushWall() || !canMove || isStickWall) return;
        if (isWallJumpAnim && xRaw == 0) return;
        if (isWallJumpAnim && canMove && (coll.wallSide == xRaw))
        {
            isWallJumpAnim = false;
        }
        if (isWallJumpAnim && canMove && (coll.wallSide == -xRaw) && !coll.OnWall())
        {
            movement = xRaw * speed;
            DOTween.To(() => wallJumpButtonCount, x => wallJumpButtonCount = x, 11, 0.25f).SetId("walljumpTween");
        }

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
        DOVirtual.Float(9, 0, .26f, RigidbodyDrag);
        rb.drag = 7.8f;
        isWallJump = true;
        isWallJumpAnim = true;
        rb.velocity = new Vector2(0, 0);
        StopCoroutine("canStickWallEnumerator");
        StartCoroutine("canStickWallEnumerator");
        StopCoroutine("DisableMovement");
        StartCoroutine("DisableMovement");
        rb.AddForce(Vector3.up * jumpForce * 2f);
        if (coll.wallSide == 1) rb.AddForce(-Vector3.right * jumpForce * 1.35f);
        if (coll.wallSide == -1) rb.AddForce(Vector3.right * jumpForce * 1.35f);
        callWallJump = false;
        wallJumpButtonCount = 0;
    }

    // 衝刺
    void Dash(float x)
    {
        StopCoroutine("DashWait");
        StopCoroutine("NextDash");
        StopCoroutine("DisableMovement");
        StopCoroutine("StickWall");
        //FindObjectOfType<RippleEffect>().Emit(Camera.main.WorldToViewportPoint(transform.position));
        isStickWall = false;
        isWallJump = false;
        speed = speedOrigin;
        cubeMesh.transform.DOLocalRotate(new Vector3(-540, 0, 0), 0.4f, RotateMode.FastBeyond360);
        hasDashed = false;
        isDash = true;
        isAnimDash = true;
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
        anim.Play("Dash");
        rb.velocity = Vector2.zero;
        Vector3 dir = new Vector2(x, 0);
        rb.velocity += dir.normalized * dashSpeed;
        GetComponent<BetterJumping>().fallMultiplier = 0.15f;
        StartCoroutine("DashWait");
        StartCoroutine("NextDash");
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
        yield return new WaitForSeconds(.5f);
        isDash = false;
        if (coll.OnGroundDash() && !IsPushWall()) hasDashed = true;
    }
    IEnumerator DashJump()
    {

        yield return new WaitForSeconds(.15f);
        canMove = true;
        isAnimDash = false;
        yield return new WaitForSeconds(.25f);
        isDash = false;
        hasDashed = true;
    }

    //衝刺時的阻力
    void RigidbodyDrag(float x)
    {
        rb.drag = x;
    }

    //返回至原速度
    void speedBackOrigin(float x)
    {
        if (coll.wallSide == 1 && Input.GetAxisRaw("Horizontal") == -1 && !coll.OnGround()) { speed = speedOrigin; return; }
        if (coll.wallSide == -1 && Input.GetAxisRaw("Horizontal") == 1 && !coll.OnGround()) { speed = speedOrigin; return; }
        speed = x;
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
        if (!isStickWall) DOVirtual.Float(0, speedOrigin, .45f, speedBackOrigin);
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
        if (coll.CompareTag("Hazard") && !isDeath) Death();
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

    public void Death() 
    {
        deathParticle.Play();
        GameObject.FindWithTag("GM").SendMessage("ScreenShake_Death");
        isDeath = true;
        isDeathNotBack = true;
        StartCoroutine("Rebirth");
        DeathV3 = this.transform.position;
        cubeMesh.transform.DOScale(0,0.2f);
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
    }
}
