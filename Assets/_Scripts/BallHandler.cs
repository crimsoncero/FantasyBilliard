using UnityEngine;

public class BallHandler : MonoBehaviour
{

    public bool IsMoving { get; private set; }

    [SerializeField] protected BallData _ballData;
    [SerializeField] protected BallController _ballController;
    [SerializeField] protected Rigidbody _rb;
    


    private void Awake()
    {
        IsMoving = false;
    }

    protected virtual void Update()
    {
        Debug.Log("is moving is : " + IsMoving);
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
}
