using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenuManager : MonoBehaviour
{
    [SerializeField] private GameObject pausePanel;

    private bool isPaused = false;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && GameStateManager.Instance.CurrentState == GameState.Playing)
        {
            TogglePause();
        }
    }

    public void TogglePause()
    {
        isPaused = !isPaused;

        Time.timeScale = isPaused ? 0f : 1f;
        pausePanel.SetActive(isPaused);
        GameStateManager.Instance.SetState(isPaused ? GameState.Paused : GameState.Playing);
    }

    public void ResumeGame()
    {
        isPaused = false;
        Time.timeScale = 1f;
        pausePanel.SetActive(false);
        GameStateManager.Instance.SetState(GameState.Playing);
    }

    public void RestartGame()
    {
        Time.timeScale = 1f;
        SceneLoader.Instance.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void ExitToMainMenu()
    {
        Time.timeScale = 1f;
        SceneLoader.Instance.LoadScene("MainMenu");
    }
}