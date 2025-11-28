using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    // どこからでも呼べる「唯一の」インスタンス（シングルトン）
    public static GameManager Instance { get; private set; }

    // 全ステージ分の成功リプレイデータを保存するリストのリスト
    // List<List<ReplayFrame>> という「入れ子」構造にします
    public List<List<ReplayFrame>> AllStageReplays = new List<List<ReplayFrame>>();

    // 今プレイ中のステージ番号（0始まり）
    public int CurrentStageIndex = 0;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // シーン遷移しても死なない！
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // ゴールした時に呼ばれる
    public void OnStageClear(List<ReplayFrame> winningData)
    {
        Debug.Log($"<color=cyan>Stage {CurrentStageIndex + 1} Cleared! Data Saved.</color>");

        // データを保存
        AllStageReplays.Add(winningData);

        // 次の動き（例: 2秒後に次のステージへ、など）
    }
}