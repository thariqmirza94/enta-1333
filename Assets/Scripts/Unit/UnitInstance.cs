using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitInstance : UnitBase
{
    [Header("Movement")]
    [SerializeField] private float _moveSpeed = 3f; // Units per second.

    private Pathfinder _pathfinder; // Reference to the Pathfinder.
    private GridManager _gridManager;
    private List<GridNode> _currentPath = new List<GridNode>(); // The current path to follow.
    private int _pathIndex = 0; // Current waypoint index.
    private Vector3? _targetWorldPosition = null; // The current target position.
    private bool _isMoving = false; // Is the unit currently moving?

    public bool IsMoving => _isMoving;

    public List<GridNode> CurrentPath => _currentPath;

    public void Initialize(Pathfinder pathfinder, GridManager grid,UnitType unitType)
    {
        _pathfinder = pathfinder;
        _unitType = unitType;
        _gridManager = grid;
        if (_pathfinder == null)
        {
            Debug.Log($"{gameObject.name} doesn't have pathfinder");
        }
    }

    private void Update()
    {

        // guard *first*:
        if (!_isMoving
            || _currentPath == null
            || _currentPath.Count == 0
            || _pathIndex >= _currentPath.Count)
        {
            return;
        }

        // now it’s safe to log and index:
        Debug.Log($"[Update] {name} is moving to {_currentPath[_pathIndex].WorldPosition}");

        Vector3 nextWaypoint = new Vector3(
            _currentPath[_pathIndex].WorldPosition.x,
            transform.position.y,
            _currentPath[_pathIndex].WorldPosition.z
        );

        Vector3 direction = (nextWaypoint - transform.position).normalized;
        float step = _moveSpeed * Time.deltaTime;
        transform.position = Vector3.MoveTowards(transform.position, nextWaypoint, step);

        if (Vector3.Distance(transform.position, nextWaypoint) < 0.05f)
        {
            _pathIndex++;
            if (_pathIndex >= _currentPath.Count)
            {
                _isMoving = false;
            }
        }
    }

    public void TargetSet(Vector3 worldPosition)
    {

        Debug.Log($"[SetTarget] {name} trying to move to {worldPosition}");

        if (_pathfinder == null)
        {
            Debug.LogError($"[SetTarget] Pathfinder is NULL for {name}");
            return;
        }

        transform.position = _gridManager.GetNodeFromWorldPosition(transform.position).WorldPosition;

        _currentPath = _pathfinder.Findpath(transform.position, worldPosition);

        if (_currentPath == null)
        {
            Debug.LogError($"[SetTarget] {name} path is NULL.");
            return;
        }

        if (_currentPath.Count <= 1)
        {
            Debug.LogWarning($"[SetTarget] {name} path too short. Count = {_currentPath.Count}");
            return;
        }

        _pathIndex = 0;
        _targetWorldPosition = worldPosition;
        _isMoving = true;

        Debug.Log($"[SetTarget] {name} path assigned with {_currentPath.Count} nodes");

        for (int i = 0; i < _currentPath.Count - 1; i++)
        {
            Debug.DrawLine(
                _currentPath[i].WorldPosition + Vector3.up * 1f,
                _currentPath[i + 1].WorldPosition + Vector3.up * 1f,
                Color.cyan, 5f
            );
        }
    }

    public void SetTarget(GridNode node)
    {
        TargetSet(node.WorldPosition);
        Debug.Log("SetTarget");
    }

    public override void MoveTo(GridNode targetNode)
    {
        SetTarget(targetNode);
        Debug.Log("MoveTo");
    }
}
