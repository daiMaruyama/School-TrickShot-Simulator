using UnityEngine;

public class GoalDetector : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("<color=yellow>GOAL!! Nice Shot!</color>");

            // 1. ボールのコントローラーを取得
            var ball = other.GetComponent<BallController>();

            if (ball != null)
            {
                // 2. データを引っこ抜く
                var winningData = ball.GetReplayData();

                // 3. マネージャーに提出する
                // （GameManagerが存在する時だけ呼ぶ安全策）
                if (GameManager.Instance != null)
                {
                    GameManager.Instance.OnStageClear(winningData);
                }
            }
        }
    }
}