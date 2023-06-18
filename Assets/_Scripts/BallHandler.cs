using DG.Tweening;
using UnityEngine;
using UnityEngine.AddressableAssets;

public class BallHandler : MonoBehaviour
{

    public bool IsMoving { get; private set; }
    [SerializeField] protected Transform _transform;
    [SerializeField] protected BallData _ballData;
    [SerializeField] protected Rigidbody _rb;
    [SerializeField]
    [Range(0f, 1f)] protected float _dissolve = 0;


    public BallData BallData { get { return _ballData; } }
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

    protected virtual void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Hole"))
        {
            GameManager.Instance.EnteredHole(_ballData);
            Vanish();
        }
    }

    protected void OnCollisionEnter(Collision collision)
    {
        if(collision.collider.tag == "Cue Ball")
        {
            if(GameManager.Instance.FirstContact == null)
            {
                GameManager.Instance.FirstContact = _ballData;
            }
        }
    }




    protected void Vanish()
    {
        float vanishTime = 2f;
        float floatHeight = 5f;

        _rb.isKinematic = true;
        _rb.detectCollisions = false;

        _transform.DOMoveY(transform.position.y + floatHeight, vanishTime).SetEase(Ease.InOutSine);
        DOVirtual.Float(0, 1, vanishTime,v => _material.SetFloat("_Dissolve", v));

    }

    public void Appear()
    {
        float appearTime = 3f;
        float appearHeight = 5f;

        _rb.isKinematic = false;
        _rb.detectCollisions = true;

        _transform.position = new Vector3(transform.position.x, transform.position.y + appearHeight, transform.position.z);


        _transform.DOMoveY(transform.position.y - appearHeight, appearTime).SetEase(Ease.InOutSine).OnComplete(() => OnAppear());
        DOVirtual.Float(1, 0, appearTime, v => _material.SetFloat("_Dissolve", v));


    }

    protected void OnAppear()
    {
        _rb.isKinematic = false;
        _rb.detectCollisions = true;
        _transform.position = new Vector3(transform.position.x, 0, transform.position.z);
    }

    private void OnValidate()
    {
        _material = new Material(_ballData.Material);
        _rb.GetComponent<MeshRenderer>().material = _material;
        _material.SetTexture("_MainTex", _ballData.Texture);
    }
}
