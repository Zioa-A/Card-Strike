using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class GameManager : MonoBehaviour
{
    [Header("Game Settings")]
    public bool isGameOver = false;

    [Header("UI")]
    public GameObject stageClearPanel;
    public GameObject losePanel;
    public GameObject pausePanel;

    void Start()
    {
        // Hide panels at the start of the stage.
        if (stageClearPanel != null)
        {
            stageClearPanel.SetActive(false);
        }

        if (losePanel != null)
        {
            losePanel.SetActive(false);
        }

        if (pausePanel != null)
        {
            pausePanel.SetActive(false);
        }
    }

    public void StageCleared()
    {
        isGameOver = true;

        Debug.Log("Stage cleared!");

        if (stageClearPanel != null)
        {
            stageClearPanel.SetActive(true);
        }

        // Level 1 automatically moves to Level 2 after 2 seconds.
        if (SceneManager.GetActiveScene().name == "Level 1")
        {
            StartCoroutine(GoToLevel2AfterDelay());
        }

        // Level 2 stays on the Victory panel. Changes to the Code by Michail to add more scenes
        else if (SceneManager.GetActiveScene().name == "Level 2")
        {
            StartCoroutine(GoToLevel3AfterDelay());
            //Debug.Log("Final level cleared. Victory!");
        }

        else if (SceneManager.GetActiveScene().name == "Level 3")
        {
            StartCoroutine(GoToLevel4AfterDelay());

        }
        else if (SceneManager.GetActiveScene().name == "Level 4")
        {
            StartCoroutine(GoToLevel5AfterDelay());
        }
        else if (SceneManager.GetActiveScene().name == "Level 5")
        {
            Debug.Log("Thanks for playing the demo!");
        }

    }

    public void PlayerLoses()
    {
        isGameOver = true;

        Debug.Log("Player lost!");

        if (losePanel != null)
        {
            Debug.Log("Activating lose panel.");
            losePanel.SetActive(true);
        }
    }

    public bool IsGameOver()
    {
        return isGameOver;
    }

    // Restarts the current level.
    // Useful for the Lose panel.
    public void RestartStage()
    {
        SceneManager.LoadScene(
            SceneManager.GetActiveScene().name
        );
    }

    // Used by the Victory panel.
    // Starts the game again from Level 1.
    public void RestartGame()
    {
        SceneManager.LoadScene("Level 1");
    }

    // Takes the player back to the main menu.
    public void MainMenu()
    {
        SceneManager.LoadScene("starting_menu");
    }

    // Used by the Play button on the main menu.
    public void StartGame()
    {
        SceneManager.LoadScene("Level 1");
    }

    // Waits 2 seconds after clearing Level 1,
    // then automatically loads Level 2.
    private IEnumerator GoToLevel2AfterDelay()
    {
        yield return new WaitForSeconds(2f);

        SceneManager.LoadScene("Level 2");
    }
    // Adding more scenes with delay logic from before
    private IEnumerator GoToLevel3AfterDelay()
    {
        yield return new WaitForSeconds(2f);

        SceneManager.LoadScene("Level 3");
    }

    private IEnumerator GoToLevel4AfterDelay()
    {
        yield return new WaitForSeconds(2f);

        SceneManager.LoadScene("Level 4");
    }

    private IEnumerator GoToLevel5AfterDelay()
    {
        yield return new WaitForSeconds(2f);

        SceneManager.LoadScene("Level 5");
    }

    public void Quit()
    {
        Application.Quit();

        Debug.Log("Game closed.");
    }

    public void OpenPauseMenu()
    {
        if (pausePanel != null)
        {
            pausePanel.SetActive(true);
        }
    }

    public void ClosePauseMenu()
    {
        if (pausePanel != null)
        {
            pausePanel.SetActive(false);
        }
    }
}