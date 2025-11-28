using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[System.Serializable]
public struct ReplayFrame
{
    public Vector3 position;
    public Quaternion rotation;
}

public class BallController : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] float maxPower = 10f;
    [SerializeField] float chargeSpeed = 10f;

    Rigidbody _rb;
    List<ReplayFrame> _replayData = new List<ReplayFrame>();

    InputAction _fireAction;
    InputAction _retryAction;

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
        _retryAction = new InputAction(binding: "<Keyboard>/r");
    }

    void OnEnable()
    {
        _fireAction.Enable();
        _retryAction.Enable();
    }

    void OnDisable()
    {
        _fireAction.Disable();
        _retryAction.Disable();
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

        if (_retryAction.WasPressedThisFrame())
        {
            ResetBall();
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
        }
    }

    void ThrowBall()
    {
        _isCharging = false;
        _isRecording = true;

        _replayData.Clear();

        _rb.isKinematic = false;
        Vector3 forceDir = (transform.forward + transform.up).normalized;
        _rb.AddForce(forceDir * _currentPower, ForceMode.Impulse);
    }

    public void ResetBall()
    {
        // 1. 位置と回転を初期状態に戻す
        transform.position = _startPos;
        transform.rotation = _startRot;

        // 2. 物理挙動を完全に止める
        _rb.linearVelocity = Vector3.zero;
        _rb.angularVelocity = Vector3.zero;
        _rb.isKinematic = true;

        // 3. フラグとパワーをリセット
        _isCharging = false;
        _isRecording = false;
        _isReplaying = false;
        _currentPower = 0f;

        // 4. データも一旦クリア（今のテイクは失敗扱いなので
        _replayData.Clear();

        Debug.Log("Reset! Try Again.");
    }

    /// <summary>
    /// ゴールした時に、外部（GoalDetectorなど）がデータを取得するために使います
    /// </summary>
    public List<ReplayFrame> GetReplayData()
    {
        // 参照渡し（中身が繋がった状態）で渡すと後で消去された時に困るので、
        // 「新しいリスト」としてコピーして渡します
        return new List<ReplayFrame>(_replayData);
    }
}