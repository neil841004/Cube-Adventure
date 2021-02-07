using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloorShadow : MonoBehaviour
{
    public float distance; //max 3
    float scaleSize;
    public bool onFloor; //是否開啟影子
    Color _color; //影子透明度
    public PlayerMovement move;
    // Start is called before the first frame update
    void Start()
    {
        _color = this.GetComponent<SpriteRenderer>().color;
    }

    // Update is called once per frame
    void Update()
    {
        if (move.isDeath || move.isWin)
        {
            _color.a = 0;
            this.GetComponent<SpriteRenderer>().color = _color;
            return;
        }

        if (onFloor)
        {
            _color.a = 0.4f - (distance * 0.055f);
            scaleSize = 0.22f - (distance * 0.022f);
            transform.localScale = new Vector3(scaleSize, scaleSize, 1);
        }
        else _color.a = 0;
        this.GetComponent<SpriteRenderer>().color = _color;
    }
}
