using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Collision : MonoBehaviour
{
    [Header("Layers")]
    public LayerMask groundLayer;
    
    [Space]

    public Collider[] onGround;
    public Collider[] onRightWall;
    public Collider[] onLeftWall;
    public int wallSide;

    [Space]

    [Header("Collision")]
    public Vector3 collisoinRadius;
    public Vector3 bottomOffset, rightOffset, leftOffset;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        OnGround();
        OnWall();
    }
    public bool OnGround()
    {
        onGround = Physics.OverlapBox(transform.position + bottomOffset, collisoinRadius, Quaternion.identity, groundLayer);
        if (onGround.Length > 0)
        {
            return true;
        }
        else return false;
    }
    public bool OnWall(){
        onRightWall = Physics.OverlapBox(transform.position + rightOffset, collisoinRadius, Quaternion.Euler(0,0,90), groundLayer);
        onLeftWall = Physics.OverlapBox(transform.position + leftOffset, collisoinRadius, Quaternion.Euler(0,0,-90), groundLayer);
        if(onRightWall.Length >0)wallSide = 1;
        if(onLeftWall.Length >0)wallSide = -1;
        if(onRightWall.Length >0 || onLeftWall.Length >0){
            return true;
        }else return false;
    }
    void OnDrawGizmos()
    {
        
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(this.transform.position + bottomOffset, collisoinRadius*2);
    }
}
