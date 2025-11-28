using UnityEngine;

public class GoalDetector : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("<color=yellow>GOAL!! Nice Shot!</color>");

            // ここに「成功演出」や「リプレイ保存確定」の処理を後で書く
        }
    }
}