using System.Collections; // コルーチンに必要
using UnityEngine;

public class GoalDetector : MonoBehaviour
{
    // ゴールしてから録画を止めるまでの「余韻」の時間（秒）
    [SerializeField] float waitTimeAfterGoal = 1.5f;
    [SerializeField] UIManager uiManager;

    // 二重判定防止用フラグ
    bool _isCleared = false;

    void OnTriggerEnter(Collider other)
    {
        // すでにクリア済みなら無視する
        if (_isCleared) return;

        if (other.CompareTag("Player"))
        {
            var recorder = other.GetComponent<ObjectRecorder>();
            if (recorder != null)
            {
                // ここでコルーチン（時間差処理）を開始！
                StartCoroutine(ClearSequence(recorder));
            }
        }
    }

    // 時間差で実行される処理
    IEnumerator ClearSequence(ObjectRecorder recorder)
    {
        _isCleared = true; // 連続ゴール防止

        if (uiManager != null)
        {
            uiManager.ShowClearEffect();
        }

        Debug.Log("<color=yellow>GOAL!! (Saving in " + waitTimeAfterGoal + "s...)</color>");

        // 1. 指定した秒数だけ待つ（ボールが底に落ちるのを待つ）
        yield return new WaitForSeconds(waitTimeAfterGoal);

        // 2. 録画をストップ！
        recorder.StopRecord();

        // 3. データを吸い出す
        var winningData = recorder.GetReplayData();

        // 4. マネージャーに提出
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnStageClear(winningData);
        }
    }
}