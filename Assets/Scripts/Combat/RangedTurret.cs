using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Health))]
public class RangedTurret : MonoBehaviour
{
    [Header("Grid & Detection")]
    [SerializeField] private GridManager gridManager;
    [SerializeField] private int squareRange = 4;

    [Header("Hitscan Damage")]
    [SerializeField] private float shotsPerSecond = 3f;
    [SerializeField] private int   damagePerShot  = 5;
    [SerializeField] private Transform muzzle;          // where VFX originate

    [Header("Line-of-Sight")]
    [SerializeField] private LayerMask obstructionMask;
    [SerializeField] private bool useLineOfSight = true;

    [Header("Scan Settings")]
    [SerializeField] private float scanInterval = 0.3f;

    [Header("Rotation")]
    [SerializeField] private float turnSpeedDeg = 720f; // ★ rotate – deg/sec

    //────────────────────────────────────────────────────
    float  scanTimer;
    float  shotCooldown;
    Health targetHealth;

    public void Initialise(GridManager gm) => gridManager = gm;

    void Update()
    {
        if (gridManager == null) return;

        // 0. Validate current target
        if (!IsTargetValid(targetHealth))
            targetHealth = null;

        // 1. Periodic scan
        scanTimer -= Time.deltaTime;
        if (scanTimer <= 0f)
        {
            ScanForEnemy();
            scanTimer = scanInterval;
        }

        // 2. Face target (if any) ★ rotate
        if (targetHealth != null)
        {
            Vector3 lookDir = targetHealth.transform.position - transform.position;
            lookDir.y = 0f;                     // lock to Y-axis
            if (lookDir.sqrMagnitude > 0.001f)
            {
                Quaternion targetRot = Quaternion.LookRotation(lookDir);
                transform.rotation = Quaternion.RotateTowards(
                    transform.rotation,
                    targetRot,
                    turnSpeedDeg * Time.deltaTime);
            }
        }

        // 3. Fire if we have a valid target
        if (targetHealth != null)
        {
            shotCooldown -= Time.deltaTime;
            if (shotCooldown <= 0f)
            {
                FireAtTarget();
                shotCooldown = 1f / shotsPerSecond;
            }
        }
    }

    void ScanForEnemy()
    {
        targetHealth = null;

        Collider[] hits = Physics.OverlapBox(
            transform.position,
            Vector3.one * squareRange * gridManager.GridSettings.NodeSize * 0.5f,
            Quaternion.identity,
            LayerMask.GetMask("Enemy")           // enemies must be on this layer
        );

        float closestSqr = float.MaxValue;

        foreach (var hit in hits)
        {
            var hp = hit.GetComponent<Health>();
            if (!IsTargetValid(hp)) continue;

            float sq = (hp.transform.position - transform.position).sqrMagnitude;
            if (sq < closestSqr)
            {
                closestSqr = sq;
                targetHealth = hp;
            }
        }
    }

    bool IsTargetValid(Health h)
    {
        if (h == null || h.CurrentHP <= 0) return false;

        float nodeSize  = gridManager.GridSettings.NodeSize;
        float sqRange   = squareRange * nodeSize;
        sqRange        *= sqRange; // squared
        if ((h.transform.position - transform.position).sqrMagnitude > sqRange)
            return false;

        if (useLineOfSight && !HasLineOfSight(h.transform.position))
            return false;

        return true;
    }

    bool HasLineOfSight(Vector3 targetPos)
    {
        Vector3 origin = muzzle ? muzzle.position : transform.position + Vector3.up;
        Vector3 dir    = (targetPos + Vector3.up) - origin;
        return !Physics.Raycast(origin, dir.normalized, dir.magnitude, obstructionMask);
    }

    //────────────────────────────────────────────────────
    void FireAtTarget()
    {
        if (targetHealth == null || targetHealth.CurrentHP <= 0)
        {
            targetHealth = null;
            return;
        }

        targetHealth.TakeDamage(damagePerShot, gameObject);

        // optional debug ray
        Debug.DrawLine(
            muzzle ? muzzle.position : transform.position + Vector3.up,
            targetHealth.transform.position + Vector3.up,
            Color.red, 0.1f);
    }
}
