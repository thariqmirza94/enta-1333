using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Collider))]
public class Health : MonoBehaviour, IDamageable
{
    [field: SerializeField] public int MaxHP { get; private set; } = 100;
    public int CurrentHP { get; private set; }

    [SerializeField] private bool isBase = false;
    [SerializeField] private GameObject floatingTextPrefab;
    [SerializeField] private HealthBarUI barPrefab;
    [SerializeField] private Vector3 offset = new Vector3(0, 2f, 0); // Default Y offset

    public UnityEvent OnDeath;

    private HealthBarUI bar;

    void Awake()
    {
        CurrentHP = MaxHP;

        if (barPrefab != null)
        {
            bar = Instantiate(barPrefab, transform.position + offset, quaternion.identity);
            bar.transform.SetParent(transform); // Follow this object as it moves
            bar.Init(this);
        }
    }

    public void TakeDamage(int amount, GameObject source)
    {
        AudioManager.Instance.PlaySFX("damage");
        
        CurrentHP = Mathf.Max(CurrentHP - amount, 0);

        if (floatingTextPrefab)
        {
            var floating = Instantiate(floatingTextPrefab, transform.position + Vector3.up * 1.5f, Quaternion.identity);
            floating.GetComponent<FloatingText>().Init(amount);
        }

        bar?.Refresh();

        if (CurrentHP == 0)
            Die();
    }

    void Die()
    {
        
        OnDeath?.Invoke();
        GoldManager.Instance?.AddGold(5); // Reward on kill
        if (isBase)
        {
            GameLoopManager loop = FindObjectOfType<GameLoopManager>();
            if (loop != null)
                loop.GameOver(false);
        }
        Destroy(gameObject);
    }
}