using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    private void Start()
    {
        AudioManager.Instance.PlayMenuMusic();
        GameStateManager.Instance.SetState(GameState.MainMenu);
    }

    public void StartGame()
    {
        AudioManager.Instance.StopMusic();
        SceneManager.LoadScene("CombatSystem");
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}