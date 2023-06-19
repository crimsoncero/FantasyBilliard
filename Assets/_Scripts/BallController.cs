using DG.Tweening;
using Unity.Burst.CompilerServices;
using Unity.Mathematics;
using UnityEngine;

public class BallController : BallHandler
{


    [Header("Force Mapping")]
    [SerializeField] private float _forceMultiplier = 1.5f;
    [SerializeField] private float _minForceInput;
    [SerializeField] private float _maxForceInput;
    [SerializeField] private float _minForceOutput;
    [SerializeField] private float _maxForceOutput;



    [Header("Shot Indicator Links")]
    [SerializeField] private SpriteRenderer _magicCircleRen;
    [SerializeField] private LineRenderer _lineRen;
    [SerializeField] private Sprite _p1CircleSprite;
    [SerializeField] private Sprite _p2CircleSprite;


    [Header("Cue Stick Ranges")]
    [SerializeField] private float _minAim = 0.5f;
    [SerializeField] private float _maxAim = 50f;
    
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
        Vector3 ballScreenPos = GameManager.Instance.MainCamera.WorldToScreenPoint(transform.position);
        ballScreenPos.z = 0;
        Vector3 touchPos = new Vector3(Input.GetTouch(0).position.x, Input.GetTouch(0).position.y, 0);
        Vector3 hitVector = (ballScreenPos - touchPos);
        hitVector = new Vector3(hitVector.x, hitVector.z, hitVector.y);

        _currentDirVector = hitVector.normalized;

        float x = Mathf.Abs(hitVector.x);
        float z = Mathf.Abs(hitVector.z);
        float force = Mathf.Clamp(Vector2.Distance(Vector2.zero, new Vector2(x,z)), _minForceInput,_maxForceInput);

        _currentForce = math.remap(_minForceInput, _maxForceInput, _minForceOutput, _maxForceOutput, force);

    }

    void Shoot()
    {
        Vector3 hitVector = _currentDirVector * (_currentForce * _forceMultiplier);

        Debug.Log($"Shot Info: Init Force : {_currentForce} \n Force Multiplied : {_currentForce * _forceMultiplier}" +
            $" HitVector : {hitVector}");

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
            _lineRen.enabled = true;
            DrawAimLine();
        }
        else
        {
            _lineRen.enabled = false;
        }


    }

    public void Appear(Vector3 pos)
    {
        float appearTime = 2f;

        _rb.isKinematic = false;
        _rb.detectCollisions = true;
        _transform.position = pos;
        DOVirtual.Float(1, 0, appearTime, v => _material.SetFloat("_Dissolve", v));


    }

    void DrawAimLine()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, _currentDirVector, out hit, Mathf.Infinity))
        {
            _lineRen.SetPosition(0, this.transform.position);

            float forceDist = Vector3.Distance(_currentDirVector * (_currentForce * 1f), this.transform.position);

            if(forceDist < hit.distance)
            {
                _lineRen.SetPosition(1, this.transform.position + (_currentDirVector * (_currentForce * 1f)));
            }
            else
            {
                _lineRen.SetPosition(1, hit.point);
            }
        }
    }
}
