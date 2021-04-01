using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BgCube : MonoBehaviour
{
    Material _material;
    Color _color;
    float colorValue;
    float zCoeff = 0.0016f;
    float colorValueMax = 0.45f;
    
    // Start is called before the first frame update
    void Start()
    {
        _material = this.GetComponent<MeshRenderer>().material;
        colorValue = transform.position.z * zCoeff;
        if (colorValue > colorValueMax) colorValue = colorValueMax;
        _color = new Color(colorValue, colorValue, colorValue);
        _material.SetColor("_FinalAddColor", _color);
    }

    // Update is called once per frame
    void Update()
    {

    }
}
