using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public string startLevel;

    // public string levelSelect;

    public void NewGame()
    {
        SceneManager.LoadScene(startLevel);
    }

    // public void LevelSelect()
    // {
    // Application.LoadLevel (levelSelect);
    // }

    public void QuitGame()
    {
        Debug.Log("Game Exited");
        Application.Quit();
    }
}