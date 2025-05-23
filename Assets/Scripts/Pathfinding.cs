using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pathfinding : MonoBehaviour
{
    [SerializeField] private GridManager gridManager;

    private List<GridNode> path = new();
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.I))
        {
            Reset();
        }
    }

    private void Start()
    {
        Reset();
    }
    private void Reset()
    {
        if (!gridManager.IsInitialized)
            gridManager.InitializeGrid();

        Vector2Int start, goal;
        GridNode[,] grid = gridManager.GetGrid();
        int width = grid.GetLength(0);
        int height = grid.GetLength(1);

        // Find two random walkable points
        do
        {
            start = new Vector2Int(Random.Range(0, width), Random.Range(0, height));
        } while (!grid[start.x, start.y].Walkable);

        do
        {
            goal = new Vector2Int(Random.Range(0, width), Random.Range(0, height));
        } while (!grid[goal.x, goal.y].Walkable || goal == start);

        path = BFS(start, goal, grid);
        Debug.Log($"Path from {start} to {goal} found with {path?.Count ?? 0} steps.");
    }

    private List<GridNode> BFS(Vector2Int start, Vector2Int goal, GridNode[,] grid)
    {
        int width = grid.GetLength(0);
        int height = grid.GetLength(1);
        bool[,] visited = new bool[width, height];
        Dictionary<Vector2Int, Vector2Int> cameFrom = new();
        Queue<Vector2Int> queue = new();

        queue.Enqueue(start);
        visited[start.x, start.y] = true;

        Vector2Int[] directions = new Vector2Int[]
        {
            Vector2Int.up,
            Vector2Int.down,
            Vector2Int.left,
            Vector2Int.right
        };

        while (queue.Count > 0)
        {
            Vector2Int current = queue.Dequeue();
            if (current == goal)
                break;

            foreach (Vector2Int dir in directions)
            {
                Vector2Int next = current + dir;
                if (next.x >= 0 && next.y >= 0 && next.x < width && next.y < height)
                {
                    if (!visited[next.x, next.y] && grid[next.x, next.y].Walkable)
                    {
                        visited[next.x, next.y] = true;
                        cameFrom[next] = current;
                        queue.Enqueue(next);
                    }
                }
            }
        }

        // Reconstruct path
        List<GridNode> resultPath = new();
        if (!cameFrom.ContainsKey(goal)) return null; // no path

        Vector2Int step = goal;
        while (step != start)
        {
            resultPath.Add(grid[step.x, step.y]);
            step = cameFrom[step];
        }
        resultPath.Add(grid[start.x, start.y]);
        resultPath.Reverse();

        return resultPath;
    }

    private void OnDrawGizmos()
    {
        if (path == null) return;

        Gizmos.color = Color.cyan;
        foreach (GridNode node in path)
        {
            Gizmos.DrawSphere(node.WorldPosition, 0.2f);
        }
    }
}