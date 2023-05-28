using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallGFXControl : MonoBehaviour
{
    [SerializeField] Material _material;
    [SerializeField] [ColorUsage(true,true)] Color _color;

    private void Awake()
    {
        _material.SetColor(Shader.PropertyToID("_Emission"), _color);
    }

    private void OnValidate()
    {
        _material = this.GetComponent<MeshRenderer>().sharedMaterial;
    }

}
