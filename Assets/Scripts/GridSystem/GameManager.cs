using UnityEngine;

public class GameManager : MonoBehaviour
{
    private void Start()
    {
        GameStateManager.Instance.SetState(GameState.Playing);
        AudioManager.Instance.PlayGameMusic();
    }
}