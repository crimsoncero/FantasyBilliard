using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEditor.Build.Content;
using UnityEngine;


public class BallController : MonoBehaviour
{
    [SerializeField] private Rigidbody _rb;
    [SerializeField] private Camera _camera;


   


    [Header("Force Mapping")]
    [SerializeField] private float _forceMultiplier = 1.5f;
    [SerializeField] private float minForceInput;
    [SerializeField] private float maxForceInput;
    [SerializeField] private float minForceOutput;
    [SerializeField] private float maxForceOutput;



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
        }

        if(Input.GetMouseButtonUp(0) && _isAiming )
        {
            Shoot();
            _isAiming = false;
        }
    }



    Vector3 CalculateShot()
    {
        //Calculate hit vector
        Vector3 ballScreenPos = _camera.WorldToScreenPoint(transform.position);
        ballScreenPos.z = 0;
        Debug.Log("Ball Point: " + ballScreenPos);
        Debug.Log("Hit Point: " + _currentHitPoint);
        Vector3 hitVector = (ballScreenPos - _currentHitPoint);
        hitVector = new Vector3(hitVector.x, hitVector.z, hitVector.y);
        Debug.Log("Initial Vector: " + hitVector);

        Vector3 dirVector = hitVector.normalized;
        Debug.Log("Direction: " + dirVector);


        float x = Mathf.Clamp(Mathf.Abs(hitVector.x), minForceInput, maxForceInput);
        float z = Mathf.Clamp(Mathf.Abs(hitVector.z), minForceInput, maxForceInput);
        float force = Vector2.Distance(Vector2.zero, new Vector2(x, z));

        force = math.remap(minForceInput, maxForceInput, minForceOutput, maxForceOutput,force);
        Debug.Log("Force: " + force);

        return dirVector * force;
    }

    void Shoot()
    {
        Vector3 hitVector = CalculateShot();
        Debug.Log("Hit Vector: " + hitVector);
        _rb.AddForce(hitVector * _forceMultiplier, ForceMode.Impulse);
    }
}
