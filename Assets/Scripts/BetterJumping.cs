using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BetterJumping : MonoBehaviour
{
    Rigidbody rb;
    public float fallMultiplier = 2.5f;
    public float lowJumpMultiplier = 2f;
    PlayerMovement move;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        move = GetComponent<PlayerMovement>();
    }

    // Update is called once per frame
    void Update()
    {
        if (rb.velocity.y < 0)
        {
            rb.velocity += Vector3.up * Physics.gravity.y * (fallMultiplier - 1) * Time.deltaTime;
        }
        else if (rb.velocity.y > 0 && !Input.GetButton("Jump") && !move.isDownJump)
        {
            rb.velocity += Vector3.up * Physics.gravity.y * (lowJumpMultiplier - 1) * Time.deltaTime;
        }
        else if (move.isDownJump && rb.velocity.y >= 0) {
            rb.velocity += Vector3.up * Physics.gravity.y * 2.05f * Time.deltaTime;
        }
    }
}
