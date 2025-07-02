using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] private float speed = 15f;
    private GameObject target;
    private int damage;

    public void Launch(GameObject tgt, int dmg)
    {
        target = tgt;
        damage = dmg;
        Destroy(gameObject, 5f);
    }

    void Update()
    {
        if (target == null) { Destroy(gameObject); return; }

        transform.position = Vector3.MoveTowards(
            transform.position, target.transform.position, speed * Time.deltaTime);

        if (Vector3.Distance(transform.position, target.transform.position) < 0.2f)
        {
            if (target.TryGetComponent<IDamageable>(out var dmg))
                dmg.TakeDamage(damage, gameObject);
            Destroy(gameObject);
        }
    }
}