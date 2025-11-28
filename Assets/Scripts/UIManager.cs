using UnityEngine;

public class UIManager : MonoBehaviour
{
    [SerializeField] GameObject clearTextObject;

    void Start()
    {
        if (clearTextObject != null)
            clearTextObject.SetActive(false);
    }

    public void ShowClearEffect()
    {
        if (clearTextObject != null)
            clearTextObject.SetActive(true);
    }
}