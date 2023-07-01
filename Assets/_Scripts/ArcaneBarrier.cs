using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArcaneBarrier : MonoBehaviour
{
    [SerializeField] MeshRenderer _meshRen;
    [SerializeField] float _dissolveTime;

    private Material _material;


    private void Awake()
    {
        _material = _meshRen.material;
        _meshRen.material = new Material(_material); // create a mat instance
        _material.SetFloat("_Dissolve", 0f);
        Appear();
    }

    public void Appear()
    {
        float currDissolve = _material.GetFloat("_Dissolve");
        DOVirtual.Float(currDissolve, 1, _dissolveTime, v => _material.SetFloat("_Dissolve", v));

    }


    public void DestroyTween()
    {
        float currDissolve = _material.GetFloat("_Dissolve");
        DOVirtual.Float(currDissolve, 0, _dissolveTime, v => _material.SetFloat("_Dissolve", v)).OnComplete(() => Destroy(this.gameObject));

    }
 

   
}
