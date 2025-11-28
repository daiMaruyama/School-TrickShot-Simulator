using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class BallController : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] float maxPower = 10f;
    [SerializeField] float chargeSpeed = 10f;
    
    struct ReplayFrame
    {
        public Vector3 position;
        public Quaternion rotation;
    }

    Rigidbody _rb;
    List<ReplayFrame> _replayData = new List<ReplayFrame>();


    InputAction _fireAction;
    InputAction _replayAction;

    bool _isCharging = false;
    float _currentPower = 0f;
    bool _isRecording = false;
    bool _isReplaying = false;
    int _replayIndex = 0;

    Vector3 _startPos;
    Quaternion _startRot;

    void Awake()
    {
        _rb = GetComponent<Rigidbody>();

        _fireAction = new InputAction(binding: "<Keyboard>/space");
        _replayAction = new InputAction(binding: "<Keyboard>/r");
    }

    void OnEnable()
    {
        _fireAction.Enable();
        _replayAction.Enable();
    }

    void OnDisable()
    {
        _fireAction.Disable();
        _replayAction.Disable();
    }

    void Start()
    {
        _rb.isKinematic = true;
        _startPos = transform.position;
        _startRot = transform.rotation;
    }

    void Update()
    {
        if (!_isRecording && !_isReplaying)
        {
            if (_fireAction.IsPressed())
            {
                _isCharging = true;
                _currentPower += chargeSpeed * Time.deltaTime;
                _currentPower = Mathf.Clamp(_currentPower, 0, maxPower);
                Debug.Log($"Power: {_currentPower:F2}");
            }

            if (_fireAction.WasReleasedThisFrame() && _isCharging)
            {
                ThrowBall();
            }
        }

        if (_replayAction.WasPressedThisFrame() && !_isReplaying)
        {
            StartReplay();
        }
    }

    void FixedUpdate()
    {
        if (_isRecording)
        {
            _replayData.Add(new ReplayFrame
            {
                position = transform.position,
                rotation = transform.rotation
            });
        }

        else if (_isReplaying)
        {
            if (_replayIndex < _replayData.Count)
            {
                _rb.MovePosition(_replayData[_replayIndex].position);
                _rb.MoveRotation(_replayData[_replayIndex].rotation);
                _replayIndex++;
            }
            else
            {
                Debug.Log("Replay Finished");
                _isReplaying = false;
            }
        }
    }

    void ThrowBall()
    {
        _isCharging = false;
        _isRecording = true;
        _rb.isKinematic = false;

        Vector3 forceDir = (transform.forward + transform.up).normalized;
        _rb.AddForce(forceDir * _currentPower, ForceMode.Impulse);
    }

    public void StartReplay()
    {
        transform.position = _startPos;
        transform.rotation = _startRot;
        _rb.linearVelocity = Vector3.zero;
        _rb.angularVelocity = Vector3.zero;

        _isRecording = false;
        _isReplaying = true;
        _rb.isKinematic = true;

        _replayIndex = 0;

        Debug.Log("Start Replay...");
    }
}