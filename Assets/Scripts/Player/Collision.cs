using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class Collision : MonoBehaviour
{
    [Header("Layers")]
    public LayerMask groundLayer;

    [Space]

    public bool onMovePF = false;

    [Space]

    public Collider[] onGround;
    public Collider[] onGroundDash;
    public Collider[] onGroundJump;
    public Collider[] onEdge;
    public Collider[] onRightWall;
    public Collider[] onLeftWall;
    public Collider[] onUpWall;
    public int wallSide = -1;
    public GameObject shadow;
    public GameObject cube;
    FloorShadow floorShadow;

    [Space]

    [Header("Collision")]
    public Vector3 collisoinRadius;
    public Vector3 collisoinSideRadius;
    public Vector3 collisoinEdgeRadius;
    public Vector3 collisoinDashRadius;
    public Vector3 collisoinJumpRadius;
    public Vector3 bottomOffset, bottomEdgeOffset, bottomJumpOffset, rightOffset, leftOffset, upOffset;

    RaycastHit rayHit;
    Ray ray_1, ray_2, ray_3, ray_4;
    public Vector3 vector1, vector2, vector3, vector4;
    bool r1, r2, r3, r4, edgeShadow, edge_1, edge_2;
    float edge_d1, edge_d2;

    // Start is called before the first frame update
    void Start()
    {
        vector1 = new Vector3(-0.22f, 0, 0);
        vector2 = new Vector3(-0.1f, 0, 0);
        vector3 = new Vector3(0.1f, 0, 0);
        vector4 = new Vector3(0.22f, 0, 0);
        r1 = false;
        r2 = false;
        r3 = false;
        r4 = false;
        floorShadow = shadow.GetComponent<FloorShadow>();
    }

    private void Update()
    {
        ray_1 = new Ray(transform.position + vector1, Vector3.down);
        ray_2 = new Ray(transform.position + vector2, Vector3.down);
        ray_3 = new Ray(transform.position + vector3, Vector3.down);
        ray_4 = new Ray(transform.position + vector4, Vector3.down);
        r1 = false;
        r2 = false;
        r3 = false;
        r4 = false;
        edge_1 = false;
        edge_2 = false;
        edgeShadow = false;
        if (Physics.Raycast(ray_1, out rayHit, 0.27f))
        {
            if (rayHit.collider.tag == "Ground" || rayHit.collider.tag == "MovePF")
            {
                r1 = true;
            }
        }
        if (Physics.Raycast(ray_2, out rayHit, 0.27f))
        {
            if (rayHit.collider.tag == "Ground" || rayHit.collider.tag == "MovePF")
            {
                r2 = true;
            }
        }
        if (Physics.Raycast(ray_3, out rayHit, 0.27f))
        {
            if (rayHit.collider.tag == "Ground" || rayHit.collider.tag == "MovePF")
            {
                r3 = true;
            }
        }
        if (Physics.Raycast(ray_4, out rayHit, 0.27f))
        {
            if (rayHit.collider.tag == "Ground" || rayHit.collider.tag == "MovePF")
            {
                r4 = true;
            }
        }

        //fake shadow
        if (Physics.Raycast(ray_1, out rayHit, 5f))
        {
            if (rayHit.collider.tag == "Ground" || rayHit.collider.tag == "MovePF")
            {
                edge_1 = true;
                edge_d1 = rayHit.distance;
            }
        }
        if (Physics.Raycast(ray_4, out rayHit, 5f))
        {
            if (rayHit.collider.tag == "Ground" || rayHit.collider.tag == "MovePF")
            {
                edge_2 = true;
                edge_d2 = rayHit.distance;
            }
        }

        floorShadow.onFloor = false;
        if (edge_1 && edge_2 && Mathf.Abs(edge_d1 - edge_d2) < 0.5f)
        {
            if (Physics.Raycast(ray_2, out rayHit, 5f))
            {
                if (rayHit.collider.tag == "Ground" || rayHit.collider.tag == "MovePF")
                {
                    floorShadow.onFloor = true;
                    floorShadow.distance = rayHit.distance;
                    shadow.transform.rotation = Quaternion.Euler(90, 0, -cube.transform.eulerAngles.y);
                    shadow.transform.position = new Vector3(transform.position.x, rayHit.point.y + 0.02f, transform.position.z);
                }
            }
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        OnGroundDash();
        OnGround();
        OnWall();
        OnEdge();
        OnUpWall();
        OnGroundJump();
    }
    public bool OnGroundEdge()
    {
        if ((r1 || r4) && !r2 && !r3) return true;
        else return false;
    }
    public bool OnGround()
    {
        onGround = Physics.OverlapBox(transform.position + bottomOffset, collisoinRadius, Quaternion.identity, groundLayer);
        if (onGround.Length > 0 && onGround[0].tag != "DeathZone")
        {
            return true;
        }
        else
        {
            //if (onMovePF == true) return true;
            return false;
        }
    }
    public bool OnGroundDash()
    {
        onGroundDash = Physics.OverlapBox(transform.position + bottomOffset, collisoinDashRadius, Quaternion.identity, groundLayer);
        if (onGroundDash.Length > 0)
        {
            if (onGroundDash[0].tag == "MovePF") //站到移動平台上
            {
                this.transform.parent = onGroundDash[0].transform;
                Physics.autoSyncTransforms = true;
                onMovePF = true;
            }
            return true;
        }
        else
        {
            this.transform.parent = null;
            Physics.autoSyncTransforms = false;
            onMovePF = false;
            return false;
        }
    }
    public bool OnGroundJump()
    {
        onGroundJump = Physics.OverlapBox(transform.position + bottomJumpOffset, collisoinJumpRadius, Quaternion.identity, groundLayer);
        if (onGroundJump.Length > 0)
        {
            return true;
        }
        else return false;
    }
    public bool OnUpWall()
    {
        onUpWall = Physics.OverlapBox(transform.position + upOffset, collisoinRadius, Quaternion.identity, groundLayer);
        if (onUpWall.Length > 0)
        {
            return true;
        }
        else return false;
    }
    public bool OnEdge()
    {
        onEdge = Physics.OverlapBox(transform.position + bottomEdgeOffset, collisoinEdgeRadius, Quaternion.identity, groundLayer);
        if (onEdge.Length > 0)
        {
            return true;
        }
        else return false;
    }
    public bool OnWall()
    {
        onRightWall = Physics.OverlapBox(transform.position + rightOffset, collisoinSideRadius, Quaternion.Euler(0, 0, 90), groundLayer);
        onLeftWall = Physics.OverlapBox(transform.position + leftOffset, collisoinSideRadius, Quaternion.Euler(0, 0, -90), groundLayer);
        if (onRightWall.Length > 0)
        {
            wallSide = 1;
            if (onRightWall[0].tag == "DeathZone") return false;
        }
        if (onLeftWall.Length > 0)
        {
            wallSide = -1;
            if (onLeftWall[0].tag == "DeathZone") return false;
        }
        if (onRightWall.Length > 0 || onLeftWall.Length > 0)
        {
            return true;
        }
        else return false;
    }
    void OnDrawGizmos()
    {

        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(this.transform.position + bottomOffset, collisoinRadius * 2);
        Gizmos.DrawWireCube(this.transform.position + upOffset, collisoinRadius * 2);
        Gizmos.DrawWireCube(this.transform.position + bottomOffset, collisoinDashRadius * 2);
        Gizmos.DrawWireCube(this.transform.position + bottomJumpOffset, collisoinJumpRadius * 2);
        Gizmos.DrawWireCube(this.transform.position + bottomEdgeOffset, collisoinEdgeRadius * 2);
    }

    void DeactivateChildren(GameObject g, bool state)
    {
        g.SetActive(state);

        foreach (Transform child in g.transform)
        {
            DeactivateChildren(child.gameObject, state);
        }
    }
}
