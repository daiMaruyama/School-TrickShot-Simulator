using UnityEngine;
using UnityEngine.InputSystem;

public class BallController : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] float maxPower = 10f;
    [SerializeField] float chargeSpeed = 10f;

    Rigidbody _rb;
    ObjectRecorder _recorder;

    InputAction _fireAction;
    InputAction _retryAction;
    InputAction _debugReplayAction;

    bool _isCharging = false;
    float _currentPower = 0f;

    // もう投げたかを管理するフラグ
    bool _hasThrown = false;

    Vector3 _startPos;
    Quaternion _startRot;

    void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        _recorder = GetComponent<ObjectRecorder>();

        _fireAction = new InputAction(binding: "<Keyboard>/space");
        _retryAction = new InputAction(binding: "<Keyboard>/r");
        _debugReplayAction = new InputAction(binding: "<Keyboard>/p");
    }

    void OnEnable()
    {
        _fireAction.Enable();
        _retryAction.Enable();
        _debugReplayAction.Enable();
    }

    void OnDisable()
    {
        _fireAction.Disable();
        _retryAction.Disable();
        _debugReplayAction.Disable();
    }

    void Start()
    {
        _rb.isKinematic = true;
        _startPos = transform.position;
        _startRot = transform.rotation;
    }

    void Update()
    {
        // まだ投げていないときだけ、チャージ処理を行う
        if (!_hasThrown)
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

        if (_retryAction.WasPressedThisFrame())
        {
            ResetBall();
        }

        if (_debugReplayAction.WasPressedThisFrame())
        {
            Debug.Log("Debug Replay Start!");
            _recorder.StartReplay();
        }
    }

    void ThrowBall()
    {
        _isCharging = false;

        // 投げた瞬間に済みマークをつける
        _hasThrown = true;

        _recorder.StartRecord();

        _rb.isKinematic = false;
        Vector3 forceDir = (transform.forward + transform.up).normalized;
        _rb.AddForce(forceDir * _currentPower, ForceMode.Impulse);
    }

    public void ResetBall()
    {
        transform.position = _startPos;
        transform.rotation = _startRot;

        _rb.linearVelocity = Vector3.zero;
        _rb.angularVelocity = Vector3.zero;
        _rb.isKinematic = true;

        _isCharging = false;
        _currentPower = 0f;

        // リセットされたので、また投げられるようにする
        _hasThrown = false;

        _recorder.ResetRecorder();

        Debug.Log("Reset! Try Again.");
    }
}