using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [Header("UI Panels")]
    public GameObject controlsPanel;

    public void PlayGame()
    {
        
        SceneManager.LoadScene(1);
    }

    public void OpenControls()
    {
        controlsPanel.SetActive(true);
    }

    public void CloseControls()
    {
        controlsPanel.SetActive(false);
    }

    public void QuitGame()
    {
        Application.Quit();
        
    }
}