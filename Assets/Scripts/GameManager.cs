using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    /*[SerializeField] private GridManager gridManager;
    [SerializeField] private UnitManager unitManager;
    [SerializeField] private Pathfinding pathfinding;

    private void Awake()
    {
        gridManager.InitializeGrid();
        //unitManager.SpawnDummyUnit();
    }

    private void Start()
    {
        gridManager.InitializeGrid();

        Vector2Int start = new Vector2Int(0, 0);
        Vector2Int goal = new Vector2Int(9, 9); // adjust as needed

        var path = pathfinding.FindPath(start, goal);
        Debug.Log($"Path found: {path?.Count ?? 0} steps");
    }*/
    [SerializeField] private GridManager gridManager;
    [SerializeField] private UnitManager unitManager;

    private void Awake()
    {
        // Ensure the grid is initialized only once
        if (!gridManager.IsInitialized)
        {
            gridManager.InitializeGrid();
        }

        // Optional: spawn a dummy unit if needed
        // unitManager.SpawnDummyUnit();
    }
}
