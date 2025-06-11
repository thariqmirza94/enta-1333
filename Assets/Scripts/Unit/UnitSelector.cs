using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class UnitSelector : MonoBehaviour
{
    [SerializeField] private Camera _camera;
    [SerializeField] private LayerMask _unitLayer;
    [SerializeField] private GridManager _gridManager;

    private ArmyManager _playerArmy;
    private ArmyPathFindingTester _tester;

    public List<UnitInstance> _selectedUnits = new();

    private IEnumerator Start()
    {
        // Wait one frame to ensure ArmyPathFindingTester has initialized
        yield return null;

        _tester = GameObject.FindAnyObjectByType<ArmyPathFindingTester>();
        if (_tester == null)
        {
            Debug.LogError("[UnitSelector] ArmyPathFindingTester not found in the scene.");
            yield break;
        }

        _playerArmy = _tester.PlayerArmy;

        if (_playerArmy == null)
        {
            Debug.LogError("[UnitSelector] PlayerArmy is null. Make sure army with ID 0 exists.");
        }
        else
        {
            Debug.Log("[UnitSelector] PlayerArmy successfully assigned.");
        }
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (TrySelectUnit())
            {
                Debug.Log("[Select] Unit selected.");
            }
            else
            {
                Debug.Log("[Select] No unit selected.");
            }
        }

        if (Input.GetMouseButtonDown(1))
        {
            if (_selectedUnits.Count > 0)
            {
                CommandSelectedUnits();
                Debug.Log("[Command] Move command issued.");
            }
            else
            {
                Debug.Log("[Command] No units selected to move.");
            }
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            _selectedUnits.Clear();
            Debug.Log("[Select] Selection cleared.");
        }
    }

    bool TrySelectUnit()
    {

        if (_playerArmy == null)
        {
            Debug.LogError("[Select] Cannot check units. PlayerArmy is null.");
            return false;
        }


        Ray ray = _camera.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit, 100f, _unitLayer))
        {
            UnitInstance unit = hit.collider.GetComponent<UnitInstance>();
            if (unit != null && _playerArmy.Units.Contains(unit))
            {
                if (!_selectedUnits.Contains(unit))
                {
                    _selectedUnits.Add(unit);
                    Debug.Log($"[Select] Added {unit.name} to selection");
                }
                else
                {
                    Debug.LogWarning("Not added to selection");
                }
                    return true;
            }
        }
        return false;
    }

  
    private void CommandSelectedUnits()
    {
        if (_camera == null || _gridManager == null) return;

        Ray ray = _camera.ScreenPointToRay(Mouse.current.position.ReadValue());
        Plane ground = new(Vector3.up, Vector3.zero);

        if (ground.Raycast(ray, out float enter))
        {
            Vector3 hitPoint = ray.GetPoint(enter);
            GridNode node = _gridManager.GetNodeFromWorldPosition(hitPoint);
            if (!node.Walkable)
            {
                Debug.Log("SelectionManager: Target node is not walkable.");
                return;
            }

            foreach (UnitBase unit in _selectedUnits)
                unit.MoveTo(node);
            Debug.Log("Asking unit to move");
        }
    }
}
