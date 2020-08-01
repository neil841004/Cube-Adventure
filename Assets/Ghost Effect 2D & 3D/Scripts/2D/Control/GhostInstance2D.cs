using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GhostInstance2D : MonoBehaviour {

    #region Importante
    private static GhostInstance2D instance;
    [HideInInspector]
    public SpriteRenderer sprite;

    [SerializeField]
    GameObject playerGhost;

    public static GhostInstance2D Instance
    {
        get
        {
            if (instance == null)
            {
                instance = GameObject.FindObjectOfType<GhostInstance2D>();
            }
            return instance;
        }
    }

    #endregion

    public bool onGhost;

    // Use this for initialization
    void Start()
    {
           sprite = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        if (onGhost)
        {
            GameObject Ghost = Instantiate(playerGhost, transform.position, transform.rotation);
        }
    }
}
