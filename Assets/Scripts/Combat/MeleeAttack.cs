using UnityEngine;

[RequireComponent(typeof(UnitAI))]
public class MeleeAttack : MonoBehaviour
{
    [SerializeField] private float attackRate = 1f;      // hits / sec
    [SerializeField] private int   damage     = 10;
    [SerializeField] private float reach      = 1.2f;
    public GameObject Target { get; set; }

    float cooldown;
    UnitAI ai;

    void Awake() => ai = GetComponent<UnitAI>();

    void Update()
    {
        if (cooldown > 0f) cooldown -= Time.deltaTime;

        var target = ai.CurrentTarget;    // assume you expose this property
        if (target == null) return;

        if (Vector3.Distance(transform.position, target.transform.position) <= reach)
        {
            if (cooldown <= 0f)
            {
                if (target.TryGetComponent<IDamageable>(out var dmg))
                    dmg.TakeDamage(damage, gameObject);
                cooldown = 1f / attackRate;
            }
        }
    }
}