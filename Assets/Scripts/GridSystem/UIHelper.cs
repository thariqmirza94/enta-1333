using UnityEngine;
using UnityEngine.UI;

public class UIHelper : MonoBehaviour
{
    private void Start()
    {
        Button button = GetComponent<Button>();
        if (button != null && GameStateManager.Instance != null)
        {
            button.onClick.AddListener(GameStateManager.Instance.ReturnToMainMenu);
        }
        else
        {
            Debug.LogWarning("Failed to bind Main Menu button.");
        }
    }
}
