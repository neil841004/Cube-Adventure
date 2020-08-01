using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerGhost : MonoBehaviour {
    SpriteRenderer sprite;
    [SerializeField]
    float timer = 0.2f;
    public Color colorGhost;

    // Use this for initialization
    void Start()
    {
        sprite = GetComponent<SpriteRenderer>();
        transform.position = GhostInstance2D.Instance.transform.position;
        transform.localScale = GhostInstance2D.Instance.transform.localScale;

        sprite.sprite = GhostInstance2D.Instance.sprite.sprite;
        //  sprite.color = new Vector4(50, -50, 50, 0.2f);
        sprite.color = colorGhost;
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

