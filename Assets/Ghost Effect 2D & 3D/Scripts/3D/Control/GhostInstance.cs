using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GhostInstance : MonoBehaviour {

    #region Importante
    private static GhostInstance instance;
    [HideInInspector]
    public MeshRenderer playerMeshRenderer;
    [HideInInspector]
    public SkinnedMeshRenderer playerSkinnedMeshRenderer;

    public bool meshRenderer, skinnedMeshRenderer;

    [SerializeField]
    GameObject playerGhost;

    public static GhostInstance Instance
    {
        get
        {
            if (instance == null)
            {
                instance = GameObject.FindObjectOfType<GhostInstance>();
            }
            return instance;
        }
    }

    #endregion

    public bool onGhost;

    private int i = 0;
    [HideInInspector]
    // Use this for initialization
    void Start () {
        if (meshRenderer)
        {
            MeshRenderer mR = GetComponent<MeshRenderer>();
            skinnedMeshRenderer = false;
            // meshRenderer = false;
        }

        if (skinnedMeshRenderer)
        {
            SkinnedMeshRenderer sMR = GetComponent<SkinnedMeshRenderer>();
            meshRenderer = false;
            // skinnedMeshRenderer = false;
        }
    }
	
	// Update is called once per frame
	void Update () {
        i++;
        if (onGhost)
        {
            if (i % 3 == 0)
            {
                GameObject Ghost = Instantiate(playerGhost, transform.position, transform.rotation);
            }
            if (i > 100) {
                i = 0;
                onGhost = false;
            }
        }
	}
}
