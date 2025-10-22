using UnityEngine;
using UnityEngine.InputSystem;

public class PauseMenu : MonoBehaviour
{
    public GameObject container;

    void Update()
    {
        var k = Keyboard.current;

        if (k.escapeKey.isPressed)
        {
            container.SetActive(true);
            Time.timeScale = 0;
        }
    }

    public void ResumeButton()
    {
        container.SetActive(false);
        Time.timeScale = 1;
    }

    public void MainMenuButton()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("MainMenu");
    }

}
