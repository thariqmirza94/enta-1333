using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// World-space HP bar that follows the camera and
/// drives a UI Slider
/// </summary>
public class HealthBarUI : MonoBehaviour
{
    [SerializeField] private Slider  slider;      // reference to the Slider component
    private Health  hp;          // unit’s health script
    private Transform cam;       // main camera

    /// <summary>Called by Health.cs right after instantiation.</summary>
    public HealthBarUI Init(Health h)
    {
        hp   = h;
        cam  = Camera.main.transform;

        // Configure slider range once
        slider.minValue = 0f;
        slider.maxValue = hp.MaxHP;
        Refresh();

        return this;
    }

    void LateUpdate()
    {
        // Billboard so the bar always faces the camera
        if (cam != null)
            transform.LookAt(transform.position + cam.forward);

        // Optionally auto-refresh each frame (comment if you prefer manual)
        Refresh();
    }

    /// <summary>Call this after hp changes (Health.cs does this).</summary>
    public void Refresh()
    {
        if (slider != null && hp != null)
            slider.value = hp.CurrentHP;
    }
}