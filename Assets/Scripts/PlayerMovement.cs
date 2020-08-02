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
    float xRaw;

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
    bool isDeathNotBack =false; //死亡後還沒回歸原位之前

    [Space]
    [Header("Object")]
    public GameObject cube;
    public GameObject cubeMesh;
    Vector3 DeathV3;

    [Space]
    [Header("Polish")]
    public ParticleSystem deathParticle;
    public ParticleSystem rebirthParticle;
    public ParticleSystem dashParticleR;
    public ParticleSystem dashParticleL;
    public ParticleSystem DashToWallParticle;

    void Start()
    {
        // Time.timeScale = 0.15f;
        rb = GetComponent<Rigidbody>();
        coll = GetComponent<Collision>();
        anim = GetComponent<Animator>();
    }

    void Update()
    {
        x = Input.GetAxis("Horizontal");
        xRaw = Input.GetAxisRaw("Horizontal");

        // 跳躍狀態判斷
        if (Input.GetButtonDown("Jump"))
        {
            if ((!IsPushWall() && coll.OnGround()) || (IsPushWall() && coll.OnGround()) || (coll.OnEdge() && !coll.OnWall() && canEdgeJump)) startJump = true;
            else if ((IsPushWall() && !coll.OnGround()) || isStickWall && !coll.OnGround()) callWallJump = true;
        }

        //衝刺
        if (Input.GetButtonDown("Dash") && hasDashed && !IsPushWall() && !isDeath)
        {
            if (xRaw != 0) Dash(xRaw);
        }
        if (!isDash && !IsPushWall())
        {
            if (coll.OnGroundDash()) hasDashed = true;
        }
        if (isAnimDash && IsPushWall())
        {
            DashToWallParticle.Play();
            GameObject.FindWithTag("GM").SendMessage("ScreenShake_S");
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
            transform.position = new Vector3(1.75f, 2.75f, -0.5f);
        }
        else if (Input.GetKeyDown(KeyCode.T))
        {
            transform.position = new Vector3(6.038958f, 10.25f, -0.5f);
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
            if ((IsPushWall() || coll.OnGround() || isDash || isStickWall) && !isWallJump) isWallJumpAnim = false;
        }

        //黏牆動畫
        if (IsPushWall() || isStickWall)
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
        if (!isDeath)
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
            if (rb.velocity.y <= fallSpeedMax * 0.4f)
            {
                rb.velocity = new Vector3(rb.velocity.x, fallSpeedMax * 0.4f);
            }
        }
        if (!IsPushWall() && !isWallJump && !isStickWall && !isDash)
        {
            GetComponent<BetterJumping>().fallMultiplier = fallMultiplier;
        }

        // 黏牆相關
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
                canMove = true;
                DOVirtual.Float(0, speedOrigin, .45f, speedBackOrigin);
            }
        }

        //判斷墜地速度
        if (!coll.OnGround())
        {
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
        if ((rb.velocity.y < 1 && rb.velocity.y > -1) || canEdgeJump)
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
        if (coll.wallSide == 1) rb.AddForce(-Vector3.right * jumpForce * 1.3f);
        if (coll.wallSide == -1) rb.AddForce(Vector3.right * jumpForce * 1.3f);
        callWallJump = false;
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
        anim.Play("Dash");
        cubeMesh.GetComponent<GhostInstance>().onGhost = true;
        //if (xRaw == 1)dashParticleL.Play();
        //if(xRaw == -1)dashParticleR.Play();
        rb.velocity = Vector2.zero;
        Vector3 dir = new Vector2(x, 0);
        rb.velocity += dir.normalized * dashSpeed;
        GetComponent<BetterJumping>().fallMultiplier = 0.15f;
        GameObject.FindWithTag("GM").SendMessage("ScreenShake_S");
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
        yield return new WaitForSeconds(.07f);
        GetComponent<BetterJumping>().fallMultiplier = fallMultiplier;
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
        if (coll.wallSide == 1 && Input.GetAxisRaw("Horizontal") == -1 && !coll.OnGround()) { speed = speedOrigin; return; }
        if (coll.wallSide == -1 && Input.GetAxisRaw("Horizontal") == 1 && !coll.OnGround()) { speed = speedOrigin; return; }
        speed = x;
    }

    IEnumerator DisableMovement()
    {
        canMove = false;
        speed = 0;
        yield return new WaitForSeconds(.23f);
        canMove = true;
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
        isStickWall = false;
        GetComponent<BetterJumping>().fallMultiplier = fallMultiplier;
    }
    private void OnTriggerEnter(Collider coll)
    {
        if (coll.CompareTag("Hazard") && !isDeath)
        {
            deathParticle.Play();
            GameObject.FindWithTag("GM").SendMessage("ScreenShake_L");
            cube.gameObject.SetActive(false);
            isDeath = true;
            isDeathNotBack = true;
            StartCoroutine("Rebirth");
            DeathV3 = this.transform.position;
        }
    }
    IEnumerator Rebirth()
    {
        FindObjectOfType<RippleEffect>().Emit(Camera.main.WorldToViewportPoint(transform.position));
        yield return new WaitForSeconds(.4f);
        GameObject.FindWithTag("GM").SendMessage("Death");
        yield return new WaitForSeconds(.3f);
        isDeathNotBack = false;
        transform.position = new Vector3(1.75f, 2.75f, -0.5f);
        GameObject.FindWithTag("Camera").GetComponent<CinemachineVirtualCamera>().GetCinemachineComponent<CinemachineTransposer>().m_XDamping = 0;
        GameObject.FindWithTag("Camera").GetComponent<CinemachineVirtualCamera>().GetCinemachineComponent<CinemachineTransposer>().m_YDamping = 0;
        yield return new WaitForSeconds(.5f);
        GameObject.FindWithTag("GM").SendMessage("ResetLevel");
        transform.position = new Vector3(1.75f, 2.75f, -0.5f);
        GameObject.FindWithTag("Camera").GetComponent<CinemachineVirtualCamera>().GetCinemachineComponent<CinemachineTransposer>().m_XDamping = 1.65f;
        GameObject.FindWithTag("Camera").GetComponent<CinemachineVirtualCamera>().GetCinemachineComponent<CinemachineTransposer>().m_YDamping = .8f;
        rebirthParticle.Play();
        cube.gameObject.SetActive(true);
        isDeath = false;
    }
}
