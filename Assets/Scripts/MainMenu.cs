using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public GameObject startMainMenu;
    public GameObject levelSelect;
    public void StartGame(string sceneName)
    {
        // Reset time scale before loading scene
        Time.timeScale = 1f;

        // Load the scene
        SceneManager.LoadScene(sceneName);
    }

    public void GoToLevelSelect()
    {
        startMainMenu.SetActive(false);
        levelSelect.SetActive(true);
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
