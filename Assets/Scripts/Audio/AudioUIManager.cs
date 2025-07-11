using UnityEngine;
using UnityEngine.UI;

public class AudioUIManager : MonoBehaviour
{
    public Toggle musicToggle;
    public Toggle sfxToggle;

    private void Start()
    {
        musicToggle.onValueChanged.AddListener(OnMusicToggle);
        sfxToggle.onValueChanged.AddListener(OnSFXToggle);
    }

    void OnMusicToggle(bool isOn)
    {
        AudioManager.Instance.MuteMusic(!isOn);
    }

    void OnSFXToggle(bool isOn)
    {
        AudioManager.Instance.MuteSFX(!isOn);
    }
}