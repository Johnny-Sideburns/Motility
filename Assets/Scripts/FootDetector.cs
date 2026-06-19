using UnityEngine;

public class FootDetector : MonoBehaviour
{
    [SerializeField] private CapsuleCollider _collider;
    private Vector3 _prevPos, _center;
    private bool _movingDown, _movingUp, _hasTarget, _isGrounded;
    private Vector3 _targetPosition;
    private Quaternion _targetRotation;
    public bool HasTarget => _hasTarget;
    public bool IsGrounded => _isGrounded;
    public Vector3 GetTargetPositon => _targetPosition;
    public Quaternion GetTargetRotation => _targetRotation;

    void FixedUpdate()
    {
        RaisingLowering();
        if (Vector3.Distance(_targetPosition, transform.position) < 0.01)
        {
            _isGrounded = true;
        }
        if (_movingUp)
        {
            _hasTarget = false;
            _isGrounded = false;
        }
    }
    void Awake()
    {
        _center = _collider.center;
    }
    void OnTriggerEnter(Collider other)
    {
        if (_movingDown && !_hasTarget)
        {
            GetPoint();
        }

    }

    void OnTriggerStay(Collider other)
    {
        if (_movingDown && !_hasTarget)
        {
            //GetPoint();
        }
    }

    void OnTriggerExit(Collider other)
    {
        
    }

    void RaisingLowering()
    {
        //maybe check this in relation to the parent transform to see if the foot is moving down on purpose...
        Vector3 footVel = (transform.position - _prevPos) / Time.deltaTime;
        _prevPos = transform.position;// + transform.parent.position;
        //Debug.Log(footVel);
        _movingDown = footVel.y < 0f;
        _movingUp   = footVel.y > 0f;
    }

    void GetPoint()
    {
        if (Physics.Raycast(transform.position +_center, - transform.up, out RaycastHit hit, 1f))
        {
            _targetPosition = hit.point - _center;

            //to get the target rotation a second point is needed, this first attempts with heel then toe
            if (Physics.Raycast(transform.position +_center - transform.forward * _collider.height/2, - transform.up, out RaycastHit hit0, 1f))
            {
                //establish forward by projecting on plane using normal
                Vector3 forward = (hit.point - hit0.point).normalized;
                forward = Vector3.ProjectOnPlane(forward, hit.normal).normalized;

                _targetRotation = Quaternion.LookRotation(forward, hit.normal);
                _hasTarget = true;
                Debug.Log("Target aquired" + _targetPosition);


            } else if (Physics.Raycast(transform.position +_center -transform.forward * _collider.height/2, - transform.up, out RaycastHit hit1, 1f))
            {

                Vector3 forward = (hit.point - hit1.point).normalized;
                forward = Vector3.ProjectOnPlane(forward, hit.normal).normalized;

                _targetRotation = Quaternion.LookRotation(forward, hit.normal);
                _hasTarget = true;
                Debug.Log("Target aquired" + _targetPosition);

            }
            
        }
    }
}
