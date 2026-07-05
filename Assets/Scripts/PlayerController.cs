using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float _speed;
    [SerializeField] private float _turnSpeed;
    [SerializeField] private float _maxForce;
    [SerializeField] private float _ignoreRadius;
    [SerializeField] private Transform _lookTarget;
    [SerializeField] private Rigidbody _rb;
    [SerializeField] private StanceController _stanceController;
    private float speed, turnSpeed, maxForce;
    private Camera cam;
    private Vector3 _mousePos;
    private float _turnVelocity;
    private Vector2 _move;
    private bool _turning;
    private float _gravity = -9.88f;

    public void OnMove(InputAction.CallbackContext context)
    {
        _move = context.ReadValue<Vector2>();
    }

    void Start()
    {
        cam = Camera.main;
        ResetSpeed();
    }

    // Update is called once per frame
    void Update()
    {
        Ray ray = cam.ScreenPointToRay(Mouse.current.position.ReadValue());
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit)){
                _mousePos = hit.point;
                if (WithinDistanceFlat(hit.point)) return;
                _lookTarget.position = new Vector3(hit.point.x,_lookTarget.position.y, hit.point.z);
        }
    }

    void FixedUpdate()
    {
        //if the mouse is too close to justify moving or turning.
        if (WithinDistanceFlat(_mousePos))
        {
            UpdateVelocity(new Vector3());
            return;
        }
        TurnTo(MouseDirectionFlat());
        
        //only move forwards for now
        //UpdateVelocity(new Vector3(_move.x, 0,_move.y) * _speed);
        UpdateVelocity(new Vector3(0,0,Mathf.Max(_move.y, 0)) * speed);
        AddGravity();
    }

    void UpdateVelocity(Vector3 targetVelocity)
    {
        //set horisontal velocity
        //allign
        Vector3 currentVelocity = _rb.linearVelocity;
        targetVelocity = transform.TransformDirection(targetVelocity);
        targetVelocity.y = currentVelocity.y;
        //apply force
        Vector3 velocityChange = Vector3.ClampMagnitude(targetVelocity - currentVelocity, maxForce);

        _rb.AddForce(velocityChange, ForceMode.VelocityChange);
    }
   
    private void TurnTo(Vector3 targetDir)
    {
        if (targetDir.magnitude < _ignoreRadius*2) return;
        //guard that lets the character stand and look around a bit before turning  
        float angleDifference = Vector2.Angle(new Vector2(targetDir.x,targetDir.z), new Vector2(transform.forward.x, transform.forward.z));
        if (_turning && angleDifference < 10)
        {
            _turning = false;
            return;
        }
        if (_rb.linearVelocity.magnitude < 0.1f && angleDifference < 50 && !_turning) return;
        _turning = true;
        
        //finding the angle to turn
        float targetAngle = Mathf.Atan2(targetDir.x,targetDir.z) * Mathf.Rad2Deg;

        //smoothing the angle for applying to rotation
        targetAngle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref _turnVelocity, turnSpeed);


        transform.localRotation = Quaternion.Euler(0f,targetAngle,0f);
    }

    private Vector3 MouseDirectionFlat(){
        //the difference between the mouse position and the rb position is the Vector from the rb position to the mouse position         
        Vector3 mouseDir = _mousePos - transform.position;
        return new Vector3(mouseDir.x, 0f,mouseDir.z);
    }
    //check to see if a point is within ignore radius in a horizontal plane
    private bool WithinDistanceFlat(Vector3 point)
    {
        float distance = Vector2.Distance(new Vector2(point.x, point.z), new Vector2(transform.position.x, transform.position.z));
        return distance < _ignoreRadius;
    }

    private void AddGravity()
    {
        if (!_stanceController.IsGrounded()){_rb.AddForce(new Vector3(0,_gravity * Time.fixedDeltaTime,0), ForceMode.VelocityChange); return;}
        _rb.linearVelocity = new Vector3(_rb.linearVelocity.x, 0, _rb.linearVelocity.z);

    }

    public void ResetSpeed()
    {
        speed = _speed;
        turnSpeed = _turnSpeed;
        maxForce = _maxForce;
    }
    public void FreezeSpeed()
    {
        speed = 0;
        turnSpeed = 0;
        maxForce = 0;
    }
}
