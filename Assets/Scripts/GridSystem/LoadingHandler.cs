using UnityEngine;

public class LoadingHandler : MonoBehaviour
{
    private void Start()
    {
        if (SceneLoader.Instance != null)
            SceneLoader.Instance.OnLoadingSceneReady();
    }
}