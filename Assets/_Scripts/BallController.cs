using Unity.Mathematics;
using UnityEngine;

public class BallController : BallHandler
{

  

    [SerializeField] private Camera _camera;
    [SerializeField] private SpriteRenderer _magicCircleRenderer;
    [SerializeField] private Transform _cueStick;



    [Header("Force Mapping")]
    [SerializeField] private float _forceMultiplier = 1.5f;
    [SerializeField] private float minForceInput;
    [SerializeField] private float maxForceInput;
    [SerializeField] private float minForceOutput;
    [SerializeField] private float maxForceOutput;

    [Header("Cue Stick Ranges")]
    [SerializeField] private float minX = -6.3f;
    [SerializeField] private float maxX = -10f;
    [SerializeField] private float minZ = -1f;
    [SerializeField] private float maxZ = -3f;
    
    private Vector3 _currentDirVector = Vector3.zero;
    private float _currentForce = 0;
    private bool _isAiming = false;
    private bool _shootTrigger = false;


    private bool _canShoot
    {
        get { return GameManager.Instance.CanShoot; }
        set { GameManager.Instance.CanShoot = value; }
    }


    protected override void Update()
    {
        base.Update();
        // Touch checks if can shoot
        if (_canShoot && Input.touchCount > 0)
        {
            

            // First Touch Began:
            if(Input.GetTouch(0).phase == TouchPhase.Began) 
            {
                _isAiming = true;
                UpdateAimVector();
            }

            // While Aiming Actions
            if (_isAiming)
            {
                // Second Touch Registered, disable aiming
                if (Input.touchCount > 1)
                {
                    _isAiming = false;
                }
                // First Touch Moved:
                if (Input.GetTouch(0).phase == TouchPhase.Moved)
                {
                    UpdateAimVector();
                }
                // First Touch Ended:
                if (Input.GetTouch(0).phase == TouchPhase.Ended)
                {
                    _isAiming = false;
                    _shootTrigger = true;
                }
            }
           
        }

        UpdateIndicators();
    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate();

        if (_shootTrigger)
        {
            Shoot();
            _shootTrigger = false;
        }
    }


    void UpdateAimVector()
    {
        //Calculate hit vector
        Vector3 ballScreenPos = _camera.WorldToScreenPoint(transform.position);
        ballScreenPos.z = 0;
        Vector3 touchPos = new Vector3(Input.GetTouch(0).position.x, Input.GetTouch(0).position.y, 0);
        Vector3 hitVector = (ballScreenPos - touchPos);
        hitVector = new Vector3(hitVector.x, hitVector.z, hitVector.y);

        _currentDirVector = hitVector.normalized;


        float x = Mathf.Clamp(Mathf.Abs(hitVector.x), minForceInput, maxForceInput);
        float z = Mathf.Clamp(Mathf.Abs(hitVector.z), minForceInput, maxForceInput);
        float force = Vector2.Distance(Vector2.zero, new Vector2(x, z));

        _currentForce = math.remap(minForceInput, maxForceInput, minForceOutput, maxForceOutput, force);

    }

    void Shoot()
    {
        Vector3 hitVector = _currentDirVector * (_currentForce * _forceMultiplier);
        _rb.AddForce(hitVector, ForceMode.Impulse);

        // Once Exit ready:
        GameManager.Instance.OnPlayerExit();

    }


    void UpdateIndicators()
    {
        _magicCircleRenderer.enabled = _canShoot;

        if (_isAiming)
        {
            _cueStick.gameObject.SetActive(true);
        }
        else
        {
            _cueStick.gameObject.SetActive(false);

        }


    }
}
