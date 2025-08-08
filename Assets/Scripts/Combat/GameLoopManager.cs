using System.Collections;
using UnityEngine;
using TMPro;

public class GameLoopManager : MonoBehaviour
{
    [SerializeField] private GameObject winLossPanel;
    [SerializeField] private TextMeshProUGUI resultText;
    
    [Header("Timer")]
    [SerializeField] private float totalGameTime = 120f;
    [SerializeField] private TextMeshProUGUI timerText;

    [Header("Enemy Spawning")]
    [SerializeField] private GameObject enemyPrefab;
    [SerializeField] private float spawnIntervalEarly = 2f;
    [SerializeField] private float spawnIntervalLate = 1f;
    [SerializeField] private GridManager gridManager;

    private float timer;
    private Coroutine spawnRoutine;

    private void Start()
    {
        GameStateManager.Instance.SetState(GameState.Playing);
        timer = totalGameTime;
        UpdateTimerUI();
        spawnRoutine = StartCoroutine(SpawnEnemies());
        StartCoroutine(TimerCountdown());
    }

    IEnumerator TimerCountdown()
    {
        while (timer > 0f)
        {
            yield return new WaitForSeconds(1f);
            timer -= 1f;
            UpdateTimerUI();
        }

        GameOver(true); // win condition
    }

    void UpdateTimerUI()
    {
        int minutes = Mathf.FloorToInt(timer / 60);
        int seconds = Mathf.FloorToInt(timer % 60);
        timerText.text = $"{minutes:00}:{seconds:00}";
    }

    IEnumerator SpawnEnemies()
    {
        while (timer > 0f)
        {
            float interval = timer > totalGameTime / 2f ? spawnIntervalEarly : spawnIntervalLate;
            TrySpawnEnemy();
            yield return new WaitForSeconds(interval);
        }
    }

    void TrySpawnEnemy()
    {
        GridNode node = FindValidSpawnNode();
        if (node == null) return;

        Vector3 pos = node.WorldPosition + Vector3.up * gridManager.GridSettings.NodeSize * 0.5f;
        GameObject enemy = Instantiate(enemyPrefab, pos, Quaternion.identity);
        node.Occupant = enemy;

// Inject references so they can move
        if (enemy.TryGetComponent<UnitAI>(out var ai))
            ai.Initialise(gridManager, FindObjectOfType<Pathfinding>());

        if (enemy.TryGetComponent<UnitMover>(out var mover))
            mover.Inject(gridManager, FindObjectOfType<Pathfinding>());
    }

    GridNode FindValidSpawnNode()
    {
        int attempts = 50;
        var gs = gridManager.GridSettings;

        for (int i = 0; i < attempts; i++)
        {
            int x = Random.Range(0, gs.GridSizeX);
            int y = Random.Range(0, gs.GridSizeY);

            // Must be outside safe zone
            if (IsInSafeZone(x, y)) continue;

            GridNode node = gridManager.GetNodeAt(x, y);
            if (node != null && node.Walkable && !node.HasOccupant())
                return node;
        }

        return null;
    }

    bool IsInSafeZone(int x, int y)
    {
        Vector2Int safeSize = new Vector2Int(8, 8); // match your GridManager value
        int centerX = gridManager.GridSettings.GridSizeX / 2;
        int centerY = gridManager.GridSettings.GridSizeY / 2;

        int startX = centerX - safeSize.x / 2;
        int startY = centerY - safeSize.y / 2;

        return x >= startX && x < startX + safeSize.x &&
               y >= startY && y < startY + safeSize.y;
    }

    public void GameOver(bool won)
    {
        StopAllCoroutines();
        Time.timeScale = 0f;

        if (winLossPanel != null && resultText != null)
        {
            winLossPanel.SetActive(true);
            resultText.text = won ? "You Win!" : "You Lose!";
        }

        GameStateManager.Instance.SetState(GameState.Paused);
    }
}
