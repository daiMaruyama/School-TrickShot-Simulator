using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct ReplayFrame
{
    public Vector3 position;
    public Quaternion rotation;
}

public class ObjectRecorder : MonoBehaviour
{
    // 自分自身のデータは自分で持つ
    [SerializeField] List<ReplayFrame> _replayData = new List<ReplayFrame>();

    bool _isRecording = false;
    bool _isReplaying = false;
    int _replayIndex = 0;

    Rigidbody _rb;

    void Awake()
    {
        _rb = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        // 録画モード
        if (_isRecording)
        {
            _replayData.Add(new ReplayFrame
            {
                position = transform.position,
                rotation = transform.rotation
            });
        }
        // 再生モード
        else if (_isReplaying)
        {
            if (_replayIndex < _replayData.Count)
            {
                // 物理演算を切って動かす（Rigidbodyがないオブジェクトにも対応できるようnullチェック）
                if (_rb != null)
                {
                    _rb.MovePosition(_replayData[_replayIndex].position);
                    _rb.MoveRotation(_replayData[_replayIndex].rotation);
                }
                else
                {
                    // Rigidbodyがない場合（ただの動く床など）は直接Transformを動かす
                    transform.position = _replayData[_replayIndex].position;
                    transform.rotation = _replayData[_replayIndex].rotation;
                }
                _replayIndex++;
            }
        }
    }

    // --- 外部から命令するためのメソッド ---

    public void StartRecord()
    {
        _replayData.Clear();
        _isRecording = true;
        _isReplaying = false;
    }

    public void StopRecord()
    {
        _isRecording = false;
    }

    // リトライ時などにデータを消す
    public void ResetRecorder()
    {
        _isRecording = false;
        _isReplaying = false;
        _replayData.Clear();
    }

    // 保存用にデータを渡す
    public List<ReplayFrame> GetReplayData()
    {
        return new List<ReplayFrame>(_replayData);
    }

    // エンディングなどで再生する時用
    public void StartReplay()
    {
        _isRecording = false;
        _isReplaying = true;
        _replayIndex = 0;

        if (_rb != null) _rb.isKinematic = true;
    }
}