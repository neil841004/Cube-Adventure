using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrapShadow : MonoBehaviour
{
    RaycastHit rayHit;
    Ray ray;
    public float posX = 0;
    public float posY = 0;
    public float posZ = 0;
    public GameObject shadow;
    Vector3 rayPos;

    void Update()
    {
        shadow.SetActive(false);
        rayPos = new Vector3(transform.position.x + posX, transform.position.y + posY, transform.position.z + posZ);
        Debug.DrawRay(rayPos, Vector3.down);
        ray = new Ray(rayPos, Vector3.down);
        if (Physics.Raycast(ray, out rayHit, 5f))
        {
            if (rayHit.collider.tag == "Ground" || rayHit.collider.tag == "MovePF")
            {
                shadow.SetActive(true);
                shadow.transform.position = new Vector3(rayHit.point.x,rayHit.point.y +0.02f,rayHit.point.z);
            }
        }
    }
}
