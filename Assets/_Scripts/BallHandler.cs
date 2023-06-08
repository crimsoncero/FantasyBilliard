using UnityEngine;
using UnityEngine.AddressableAssets;

public class BallHandler : MonoBehaviour
{

    public bool IsMoving { get; private set; }

    [SerializeField] protected BallData _ballData;
    [SerializeField] protected Rigidbody _rb;
    
    protected Material _material;

    private void Awake()
    {
        IsMoving = false;
    }

    protected virtual void Update()
    {
    }

    protected virtual void FixedUpdate()
    {
        if (_rb.velocity.magnitude < 0.1)
        {
            _rb.velocity = Vector3.zero;
            _rb.angularVelocity = Vector3.zero;
            IsMoving = false;
        }
        else
        {
            IsMoving = true;
        }
    }

    private void OnValidate()
    {
        _material = new Material(_ballData.Material);
        _rb.GetComponent<MeshRenderer>().material = _material;
        _material.SetColor("_Emission", _ballData.Color);
    }
}
