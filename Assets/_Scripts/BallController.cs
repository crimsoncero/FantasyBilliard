using Unity.Burst.CompilerServices;
using Unity.Mathematics;
using UnityEngine;

public class BallController : BallHandler
{

  

    [SerializeField] private Camera _camera;
    



    [Header("Force Mapping")]
    [SerializeField] private float _forceMultiplier = 1.5f;
    [SerializeField] private float _minForceInput;
    [SerializeField] private float _maxForceInput;
    [SerializeField] private float _minForceOutput;
    [SerializeField] private float _maxForceOutput;



    [Header("Shot Indicator Links")]
    [SerializeField] private Transform _indicatorRot;
    [SerializeField] private SpriteRenderer _magicCircleRen;
    [SerializeField] private Transform _cueStick;
    [SerializeField] private LineRenderer _lineRen;
    [SerializeField] private Sprite _p1CircleSprite;
    [SerializeField] private Sprite _p2CircleSprite;


    [Header("Cue Stick Ranges")]
    [SerializeField] private float _minStickX = -6.3f;
    [SerializeField] private float _maxStickX = -10f;
    [SerializeField] private float _minStickY = 1f;
    [SerializeField] private float _maxStickY = 2f;
    
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


        float x = Mathf.Clamp(Mathf.Abs(hitVector.x), _minForceInput, _maxForceInput);
        float z = Mathf.Clamp(Mathf.Abs(hitVector.z), _minForceInput, _maxForceInput);
        float force = Vector2.Distance(Vector2.zero, new Vector2(x, z));

        _currentForce = math.remap(_minForceInput, _maxForceInput, _minForceOutput, _maxForceOutput, force);

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
        if (GameManager.Instance.CurrentPlayer == Player.P1)
            _magicCircleRen.sprite = _p1CircleSprite;
        else
            _magicCircleRen.sprite = _p2CircleSprite;
        _magicCircleRen.enabled = _canShoot;

        if (_isAiming)
        {
            float shotAngle = Vector3.SignedAngle(Vector3.right, _currentDirVector, Vector3.up);
            _indicatorRot.rotation = Quaternion.Euler(0, shotAngle, 0);
            //_cueStick.gameObject.SetActive(true);

            float stickX = math.remap(_minForceOutput, _maxForceOutput, _minStickX, _maxStickX, _currentForce);
            float stickY = math.remap(_minForceOutput, _maxForceOutput, _minStickY, _maxStickY, _currentForce);
            _cueStick.SetLocalPositionAndRotation(new Vector3(stickX, stickY, 0), _cueStick.localRotation);


            _lineRen.enabled = true;
            DrawAimLine();
        }
        else
        {
            _cueStick.gameObject.SetActive(false);
            _lineRen.enabled = false;


        }


    }
    


    void DrawAimLine()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, _currentDirVector, out hit, Mathf.Infinity))
        {
            _lineRen.SetPosition(0, this.transform.position);
            _lineRen.SetPosition(1, hit.point);
        }
    }
}
