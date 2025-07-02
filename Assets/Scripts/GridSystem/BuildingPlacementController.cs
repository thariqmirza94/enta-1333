using UnityEngine;
using UnityEngine.InputSystem;

public class BuildingPlacementController : MonoBehaviour
{
    [Header("Links")]
    [SerializeField] private Camera      mainCam;
    [SerializeField] private GridManager gridManager;
    [SerializeField] private LayerMask   groundMask;
    [SerializeField] private float liftadjust = 0;

    [Header("Prefabs (index 0-3 = Keys 1-4)")]
    [SerializeField] private GameObject[] prefabs;

    [Header("Footprint Sizes (match order above)")]
    [SerializeField] private Vector2Int[] sizes = {
        new Vector2Int(2,2), // Key 1  (building)
        new Vector2Int(1,1), // Key 2  (building)
        new Vector2Int(1,1), // Key 3  (unit)
        new Vector2Int(1,1)  // Key 4  (unit)
    };

    [Header("Ghost Materials")]
    [SerializeField] private Material validMat;
    [SerializeField] private Material invalidMat;

    // ─────────────────────────────────────────────────────────────
    GameObject ghost;
    int  currentIdx = -1;
    bool canPlace;
    int  originX, originY;
    Vector3 snapPos;

    /*─────────────────────────────────────────────────────────────*/
    void Update()
    {
        HandleHotkeys();

        if (ghost == null) return;

        SnapGhostToGrid();
        if (Mouse.current.leftButton.wasPressedThisFrame && canPlace)
            Place();
    }

    /*─────────────────────────────────────────────────────────────*/
    void HandleHotkeys()
    {
        if (Keyboard.current.digit1Key.wasPressedThisFrame) Select(0);
        if (Keyboard.current.digit2Key.wasPressedThisFrame) Select(1);
        if (Keyboard.current.digit3Key.wasPressedThisFrame) Select(2);
        if (Keyboard.current.digit4Key.wasPressedThisFrame) Select(3);
    }

    void Select(int idx)
    {
        if (idx == currentIdx || idx < 0 || idx >= prefabs.Length) return;

        currentIdx = idx;

        if (ghost) Destroy(ghost);
        ghost = Instantiate(prefabs[idx]);
        SetGhostVisual(validMat);
        ghost.tag = "Untagged";                 // avoid interference
        ghost.layer = 0;
    }

    /*─────────────────────────────────────────────────────────────*/
    void SnapGhostToGrid()
    {
        Ray r = mainCam.ScreenPointToRay(Mouse.current.position.ReadValue());
        if (!Physics.Raycast(r, out var hit, 1000f, groundMask)) return;

        GridNode node = gridManager.GetNodeFromWorldPosition(hit.point);
        if (node == null) return;

        GridSettings gs = gridManager.GridSettings;
        Vector2Int   sz = sizes[currentIdx];

        Vector3 local = gridManager.transform.InverseTransformPoint(node.WorldPosition);
        int gx = Mathf.RoundToInt(local.x / gs.NodeSize);
        int gy = Mathf.RoundToInt(local.z / gs.NodeSize);

        originX = Mathf.Clamp(gx, 0, gs.GridSizeX - sz.x);
        originY = Mathf.Clamp(gy, 0, gs.GridSizeY - sz.y);

        snapPos = gridManager.GetNodeAt(originX, originY).WorldPosition;

        // Lift only for the ghost preview (looks nicer)
        if (currentIdx == 2 || currentIdx == 3)
        {
            ghost.transform.position = snapPos + Vector3.up * liftadjust;
        }
        else
        {
            float lift = (currentIdx == 2 || currentIdx == 3) ? 0f : gs.NodeSize * 0.5f;
            ghost.transform.position = snapPos + Vector3.up * lift;
        }

        canPlace = RegionWalkable(originX, originY, sz.x, sz.y);
        SetGhostVisual(canPlace ? validMat : invalidMat);
    }

    bool RegionWalkable(int ox, int oy, int w, int h)
    {
        for (int dx = 0; dx < w; dx++)
        for (int dy = 0; dy < h; dy++)
            if (!gridManager.GetNodeAt(ox + dx, oy + dy).Walkable) return false;

        return true;
    }

    /*─────────────────────────────────────────────────────────────*/
    void Place()
    {
        bool isBuilding = (currentIdx == 0 || currentIdx == 1);

        // ── decide vertical lift ──
        float lift;
        if (isBuilding)
            lift = gridManager.GridSettings.NodeSize * 0.5f;               // buildings
        else if (prefabs[currentIdx].TryGetComponent<Collider>(out var col))
            lift = col.bounds.extents.y;                                   // units
        else
            lift = 0f;                                                     // fallback

        GameObject real = Instantiate(prefabs[currentIdx],
            snapPos + Vector3.up * lift,
            Quaternion.identity);

        // restore original tag
        real.tag = prefabs[currentIdx].tag;
        real.layer = prefabs[currentIdx].layer;

        /*── Inject AI / pathfinder refs ───────────────────────────*/
        Pathfinding pf = FindObjectOfType<Pathfinding>();

        if (real.TryGetComponent<UnitAI>(out var ai) && pf != null)
            ai.Initialise(gridManager, pf);

        if (real.TryGetComponent<UnitMover>(out var mover) && pf != null)
            mover.Inject(gridManager, pf);

        if (real.TryGetComponent<RangedTurret>(out var turret))
            turret.Initialise(gridManager);

        /*── Mark grid occupancy ──────────────────────────────────*/
        Vector2Int sz = sizes[currentIdx];

        for (int dx = 0; dx < sz.x; dx++)
        for (int dy = 0; dy < sz.y; dy++)
        {
            var node = gridManager.GetNodeAt(originX + dx, originY + dy);
            if (node == null) continue;

            if (isBuilding) node.Walkable = false;
            node.Occupant = real;
        }

        /*── Clean-up ghost ───────────────────────────────────────*/
        Destroy(ghost);
        ghost      = null;
        currentIdx = -1;
    }

    /*─────────────────────────────────────────────────────────────*/
    void SetGhostVisual(Material mat)
    {
        foreach (Renderer r in ghost.GetComponentsInChildren<Renderer>())
            r.material = mat;
    }
}
