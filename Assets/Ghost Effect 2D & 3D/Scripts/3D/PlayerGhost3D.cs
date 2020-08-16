using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerGhost3D : MonoBehaviour {

    GameObject obj;
    MeshRenderer mR;
    SkinnedMeshRenderer sMR;
    [SerializeField]
    float timer = .2f;
    public Color colorGhost;
    public bool meshRenderer, skinnedMeshRenderer;

    // Use this for initialization
    void Start()
    {
        obj = GetComponent<GameObject>();
        transform.position = GhostInstance.Instance.transform.position;
        transform.localScale = GhostInstance.Instance.transform.localScale;

        //obj.GetComponent<MeshRenderer>() = PlayBeatEmUp.Instance.playerSprite.sprite;
        //  sprite.color = new Vector4(50, -50, 50, 0.2f);
        // sprite.color = colorGhost;
        if (meshRenderer)
        {
            mR = GetComponent<MeshRenderer>();
            skinnedMeshRenderer = false;
            // meshRenderer = false;
            mR = GhostInstance.Instance.playerMeshRenderer;
        }

        if (skinnedMeshRenderer)
        {
            sMR = GetComponent<SkinnedMeshRenderer>();
            meshRenderer = false;
            sMR = GhostInstance.Instance.playerSkinnedMeshRenderer;
            // skinnedMeshRenderer = false;
        }
    }

    // Update is called once per frame
    void Update()
    {

        timer -= Time.deltaTime;

        if (timer <= 0)
        {

            Destroy(gameObject);
        }

    }
}