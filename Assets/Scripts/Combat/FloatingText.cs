using TMPro;
using UnityEngine;

public class FloatingText : MonoBehaviour
{
    [SerializeField] private float floatSpeed = 1f;
    [SerializeField] private float duration = 1f;
    [SerializeField] private TextMeshProUGUI text;

    private float timer;

    public void Init(int amount)
    {
        text.text = amount.ToString();
        transform.LookAt(Camera.main.transform); // Optional: face camera initially
        Destroy(gameObject, duration);
    }

    void Update()
    {
        transform.position += Vector3.up * floatSpeed * Time.deltaTime;

        // Make the text face the camera flat (no tilting)
        Vector3 camForward = Camera.main.transform.forward;
        transform.rotation = Quaternion.LookRotation(camForward);
    }
}