using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallController : MonoBehaviour
{
    [SerializeField] private Rigidbody _rb;
    [SerializeField] private Camera _camera;


    [SerializeField] private float _hitForce = 20;

    private Vector3 _currentHitPoint;
    private bool _isAiming = false;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetMouseButtonDown(0))
        {
            _isAiming = true;
        }

        if (Input.GetMouseButtonDown(1))
        {
            _isAiming = false;
        }

        if (Input.GetMouseButton(0))
        {
            _currentHitPoint = Input.mousePosition;
            Debug.Log("Hit Point: " + _currentHitPoint);
        }

        if(Input.GetMouseButtonUp(0) && _isAiming )
        {
            Shoot();
            _isAiming = false;
        }
    }

    void Shoot()
    {
        //Calculate hit vector
        Vector3 ballScreenPos = _camera.WorldToScreenPoint(transform.position);
        ballScreenPos.z = 0;
        Debug.Log("Ball Point: " + ballScreenPos);
        Vector3 _hitDirection = (ballScreenPos - _currentHitPoint).normalized;
        Debug.Log("Hit Direction: " + _hitDirection);

        _hitDirection = new Vector3(_hitDirection.x, _hitDirection.z, _hitDirection.y);

        _rb.AddForce(_hitDirection * _hitForce, ForceMode.Impulse);
    }
}
