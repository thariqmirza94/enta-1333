using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pathfinder : MonoBehaviour 
{

    [SerializeField] private GridManager gridManager;

    private List<GridNode> path = new();

    private void Start()
    {
        Reset();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.I))
        {
            Reset();
        }
    }

    private void Reset()
    {
        if (!gridManager.IsInitialized)
            gridManager.InitializeGrid();

        GridNode[,] grid = gridManager.GetGrid();
        int width = grid.GetLength(0);
        int height = grid.GetLength(1);

        Vector2Int start, goal;

        do
        {
            start = new Vector2Int(Random.Range(0, width), Random.Range(0, height));
        } while (!grid[start.x, start.y].Walkable);

        do
        {
            goal = new Vector2Int(Random.Range(0, width), Random.Range(0, height));
        } while (!grid[goal.x, goal.y].Walkable || goal == start);

        path = AStar(start, goal, grid);
        Debug.Log($"A* Path from {start} to {goal} found with {path?.Count ?? 0} steps.");
    }

    private List<GridNode> AStar(Vector2Int start, Vector2Int goal, GridNode[,] grid)
    {
        int width = grid.GetLength(0);
        int height = grid.GetLength(1);

        List<Vector2Int> openSet = new() { start };
        HashSet<Vector2Int> closedSet = new();
        Dictionary<Vector2Int, Vector2Int> cameFrom = new();
        Dictionary<Vector2Int, float> gScore = new() { [start] = 0f };
        Dictionary<Vector2Int, float> fScore = new() { [start] = Heuristic(start, goal) };

        Vector2Int[] directions = new Vector2Int[]
        {
            Vector2Int.up,
            Vector2Int.down,
            Vector2Int.left,
            Vector2Int.right
        };

        while (openSet.Count > 0)
        {
            Vector2Int current = GetLowestFScore(openSet, fScore);

            if (current == goal)
                return ReconstructPath(cameFrom, start, goal, grid);

            openSet.Remove(current);
            closedSet.Add(current);

            foreach (Vector2Int dir in directions)
            {
                Vector2Int neighbor = current + dir;
                if (neighbor.x < 0 || neighbor.x >= width || neighbor.y < 0 || neighbor.y >= height)
                    continue;

                if (closedSet.Contains(neighbor))
                    continue;

                GridNode node = grid[neighbor.x, neighbor.y];
                if (!node.Walkable)
                    continue;

                float tentativeG = gScore[current] + node.Weight;

                if (!openSet.Contains(neighbor))
                {
                    openSet.Add(neighbor);
                }
                else if (tentativeG >= gScore.GetValueOrDefault(neighbor, float.MaxValue))
                {
                    continue;
                }

                cameFrom[neighbor] = current;
                gScore[neighbor] = tentativeG;
                fScore[neighbor] = tentativeG + Heuristic(neighbor, goal);
            }
        }

        return null; // no path found
    }

    private float Heuristic(Vector2Int a, Vector2Int b)
    {
        return Mathf.Abs(a.x - b.x) + Mathf.Abs(a.y - b.y); // Manhattan distance
    }

    private Vector2Int GetLowestFScore(List<Vector2Int> openSet, Dictionary<Vector2Int, float> fScore)
    {
        Vector2Int best = openSet[0];
        float bestScore = fScore.GetValueOrDefault(best, float.MaxValue);

        foreach (var node in openSet)
        {
            float score = fScore.GetValueOrDefault(node, float.MaxValue);
            if (score < bestScore)
            {
                best = node;
                bestScore = score;
            }
        }

        return best;
    }

    private List<GridNode> ReconstructPath(Dictionary<Vector2Int, Vector2Int> cameFrom, Vector2Int start, Vector2Int goal, GridNode[,] grid)
    {
        List<GridNode> result = new();
        Vector2Int current = goal;

        while (current != start)
        {
            result.Add(grid[current.x, current.y]);
            current = cameFrom[current];
        }

        result.Add(grid[start.x, start.y]);
        result.Reverse();
        return result;
    }

    private void OnDrawGizmos()
    {
        if (path == null) return;

        Gizmos.color = Color.red;
        foreach (GridNode node in path)
        {
            Gizmos.DrawSphere(node.WorldPosition, 0.2f);
        }
    }
}