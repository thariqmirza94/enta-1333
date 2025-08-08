using UnityEngine;
using UnityEngine.Events;
using TMPro;

public class GoldManager : MonoBehaviour
{
    public static GoldManager Instance;

    [SerializeField] private int startingGold = 50;
    [SerializeField] private TextMeshProUGUI goldText;
    private int currentGold;

    public UnityEvent<int> OnGoldChanged; // (optional, keep if needed elsewhere)
    private void UpdateUI() => goldText.text = $"{currentGold}";

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        currentGold = startingGold;
        UpdateUI();
    }

    public int CurrentGold => currentGold;

    public bool SpendGold(int amount)
    {
        if (currentGold < amount) return false;
        currentGold -= amount;
        OnGoldChanged?.Invoke(currentGold);
        UpdateUI();
        return true;
    }

    public void AddGold(int amount)
    {
        currentGold += amount;
        OnGoldChanged?.Invoke(currentGold);
        UpdateUI();
    }
}