using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public List<List<ReplayFrame>> AllStageReplays = new List<List<ReplayFrame>>();
    public int CurrentStageIndex = 0;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void OnStageClear(List<ReplayFrame> winningData)
    {
        Debug.Log($"<color=cyan>Stage {CurrentStageIndex + 1} Cleared! Data Saved.</color>");

        while (AllStageReplays.Count <= CurrentStageIndex)
        {
            AllStageReplays.Add(null);
        }
        AllStageReplays[CurrentStageIndex] = winningData;

        // 追加: 次のステージへの遷移処理を開始
        StartCoroutine(NextStageSequence());
    }

    // 追加: 遷移用コルーチン
    IEnumerator NextStageSequence()
    {
        // UIが出てから少し余韻を持たせる（2秒待機）
        yield return new WaitForSeconds(2.0f);

        // 次のステージへ（今は同じシーンをリロードしてループ）
        // CurrentStageIndex++; // ステージが増えたらここを有効化

        int sceneIndex = SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadScene(sceneIndex);
    }
}