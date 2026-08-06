using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneController : MonoBehaviour
{
    [Header("Scene Names")]
    [SerializeField]
    private string mainMenuScene =
        "MainMenu";

    [SerializeField]
    private string gameScene =
        "Game";

    private void Awake()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void PlayGame()
    {
        LoadScene(gameScene);
    }

    public void RestartGame()
    {
        string currentScene =
            SceneManager.GetActiveScene().name;

        LoadScene(currentScene);
    }

    public void LoadMainMenu()
    {
        LoadScene(mainMenuScene);
    }

    private void LoadScene(string sceneName)
    {
        Time.timeScale = 1f;

        if (!Application.CanStreamedLevelBeLoaded(
                sceneName))
        {
            Debug.LogError(
                "Scene could not be loaded: " +
                sceneName +
                ". Add it to Build Settings.");

            return;
        }

        SceneManager.LoadScene(sceneName);
    }

    public void QuitGame()
    {
        Debug.Log("Quitting BAGSTABBER");

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying =
            false;
#else
        Application.Quit();
#endif
    }
}