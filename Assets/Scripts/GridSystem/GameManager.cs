using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private GridManager gridManager;

    private void Start()
    {
        gridManager.InitializeGrid();

        GameStateManager.Instance.SetState(GameState.Playing);
        AudioManager.Instance.PlayGameMusic();
    }
}