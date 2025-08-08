using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class SceneLoader : MonoBehaviour
{
    public static SceneLoader Instance;

    [SerializeField] private string loadingSceneName = "Loading";

    private string targetScene;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void LoadScene(string sceneName)
    {
        targetScene = sceneName;
        SceneManager.LoadScene(loadingSceneName);
    }

    public void OnLoadingSceneReady()
    {
        StartCoroutine(LoadTargetSceneAsync());
    }

    private IEnumerator LoadTargetSceneAsync()
    {
        yield return new WaitForSeconds(0.5f); // optional: simulate wait

        AsyncOperation op = SceneManager.LoadSceneAsync(targetScene);

        while (!op.isDone)
        {
            yield return null;
        }
    }
}