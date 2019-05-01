using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public string startLevel;

    public void StartGame()
    {
        SceneManager.LoadScene(startLevel);
    }
}