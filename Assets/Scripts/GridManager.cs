using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    [SerializeField] private GridSettings gridSettings;
    public GridSettings GridSettings => gridSettings;

    [SerializeField] private List<TerrainType> terrainTypes = new();

    private GridNode[,] gridNodes;

    [Header("Debug for editor playmode only")]
    [SerializeField] private List<GridNode> AllNodes = new();

    public bool IsInitialized { get; private set; } = false;

    public GridNode GetNodeAt(int x, int y)
    {
        if (x >= 0 && x < gridSettings.GridSizeX && y >= 0 && y < gridSettings.GridSizeY)
            return gridNodes[x, y];
        return null;
    }

    public GridNode[,] GetGrid()
    {
        return gridNodes;
    }
    public void InitializeGrid()
    { 
        gridNodes = new GridNode[gridSettings.GridSizeX, gridSettings.GridSizeY];

        for(int x = 0; x < gridSettings.GridSizeX; x++)
        {
            for(int y = 0; y < gridSettings.GridSizeY; y++)
            {
                Vector3 worldPos = gridSettings.UseXZPlane
                    ? new Vector3(x, 0, y) * gridSettings.NodeSize
                    : new Vector3(x, y, 0) * gridSettings.NodeSize;

                TerrainType newTerrain = terrainTypes[Random.Range(0, terrainTypes.Count)];

                GridNode node = new GridNode
                {
                    Name = $"Cell_{(x + gridSettings.GridSizeX * x + y)}",
                    WorldPosition = worldPos,
                    Walkable = newTerrain.Walkable, // Default all nodes walkable
                    Weight = newTerrain.MovementCost, // Default weight, useful for varied terrain costs
                    TerrainColor = newTerrain.GizmoColor
                };
                gridNodes[x, y] = node;
            }
        }
        IsInitialized = true;
    }

    /*public GridNode GetNodeAt(int x, int y)
    {
        if (x >= 0 && x < gridSettings.GridSizeX && y >= 0 && y < gridSettings.GridSizeY)
            return gridNodes[x, y];
        return null;
    }*/

    private void OnDrawGizmos()
    {
        if(gridNodes == null || gridSettings == null) return;
        Gizmos.color = Color.green;
        for(int x = 0; x < gridSettings.GridSizeX; x++)
        {
            for(int y = 0; y < gridSettings.GridSizeY; y++)
            {
                GridNode node = gridNodes[x, y];
                Gizmos.color = node.Walkable ? node.TerrainColor : Color.red;
                Gizmos.DrawWireCube(node.WorldPosition, Vector3.one * gridSettings.NodeSize * 0.9f);
            }
        }    
    }
}