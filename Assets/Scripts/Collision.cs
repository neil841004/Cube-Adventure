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

    [Space]

    [Header("Collision")]
    public Vector3 collisoinRadius;
    public Vector3 collisoinSideRadius;
    public Vector3 collisoinEdgeRadius;
    public Vector3 collisoinDashRadius;
    public Vector3 collisoinJumpRadius;
    public Vector3 bottomOffset, bottomEdgeOffset, bottomJumpOffset, rightOffset, leftOffset, upOffset;

    // Start is called before the first frame update
    void Start()
    {

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
    public bool OnGround()
    {
        onGround = Physics.OverlapBox(transform.position + bottomOffset, collisoinRadius, Quaternion.identity, groundLayer);
        if (onGround.Length > 0)
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
            if (onGroundDash[0].tag == "End") //站到移動平台上
            {
                GetComponent<PlayerMovement>().SendMessage("Ending");
                DeactivateChildren(GameObject.FindWithTag("End").gameObject, true);
            }
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
        //if(OnUpWall())return false;
        onRightWall = Physics.OverlapBox(transform.position + rightOffset, collisoinSideRadius, Quaternion.Euler(0, 0, 90), groundLayer);
        onLeftWall = Physics.OverlapBox(transform.position + leftOffset, collisoinSideRadius, Quaternion.Euler(0, 0, -90), groundLayer);
        if (onRightWall.Length > 0) wallSide = 1;
        if (onLeftWall.Length > 0) wallSide = -1;
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
